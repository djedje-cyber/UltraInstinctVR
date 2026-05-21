using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using System.Linq;

public class BERTInference : MonoBehaviour
{
    // ── Fields ───────────────────────────────────────────────
    [SerializeField] private ModelAsset modelAsset;
    public ControllerSelector controllerSelector;

    private Worker worker;
    private Unity.InferenceEngine.Model model;
    private Dictionary<string, int> vocab       = new Dictionary<string, int>();
    private Dictionary<string, float[]> commandEmbeddings = new Dictionary<string, float[]>();

    // ── Unity Lifecycle ──────────────────────────────────────
    void Start()
    {
        model  = ModelLoader.Load(modelAsset);
        worker = new Worker(model, BackendType.GPUCompute);

        LoadVocab();

        // Pre-compute known command embeddings
        commandEmbeddings["select"] = GetEmbedding("put that");
        commandEmbeddings["drop"]   = GetEmbedding("drop that");
    }

    void OnDestroy()
    {
        worker.Dispose();
    }

    // ── Public API (called by VoiceRecognizer) ───────────────
    public void ProcessVoiceCommand(string spokenText)
    {
        Debug.Log("Processing: " + spokenText);

        float[] embedding = GetEmbedding(spokenText);
        string  command   = FindClosestCommand(embedding);

        Debug.Log($"BERT matched '{spokenText}' → command: {command}");

        ExecuteCommand(command);
    }

    // ── Command Execution ────────────────────────────────────
    void ExecuteCommand(string command)
    {
        switch (command)
        {
            case "select":
                controllerSelector.SelectHoveredObject();
                break;
            case "drop":
                controllerSelector.ClearSelection();
                break;
            default:
                Debug.Log("Unknown command: " + command);
                break;
        }
    }

    // ── Embedding Matching ───────────────────────────────────
    string FindClosestCommand(float[] inputEmbedding)
    {
        string bestMatch = "";
        float  bestScore = -1f;

        foreach (var cmd in commandEmbeddings)
        {
            float score = CosineSimilarity(inputEmbedding, cmd.Value);
            Debug.Log($"  '{cmd.Key}' similarity: {score:F3}");

            if (score > bestScore)
            {
                bestScore = score;
                bestMatch = cmd.Key;
            }
        }

        Debug.Log($"Best match: {bestMatch} (score: {bestScore:F3})");
        return bestMatch;
    }

    float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot  += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return dot / (Mathf.Sqrt(magA) * Mathf.Sqrt(magB));
    }

    // ── BERT Inference ───────────────────────────────────────
    float[] GetEmbedding(string sentence)
    {
        int[] inputIds      = Tokenize(sentence);
        int[] attentionMask = Enumerable.Repeat(1, inputIds.Length).ToArray();

        var inputTensor = new Tensor<int>(new TensorShape(1, inputIds.Length), inputIds);
        var maskTensor  = new Tensor<int>(new TensorShape(1, attentionMask.Length), attentionMask);

        worker.SetInput("input_ids", inputTensor);
        worker.SetInput("attention_mask", maskTensor);
        worker.Schedule();

        var output    = worker.PeekOutput() as Tensor<float>;
        var cpuOutput = output.ReadbackAndClone();

        // Extract [CLS] token — represents whole sentence
        float[] embedding = new float[768];
        for (int i = 0; i < 768; i++)
            embedding[i] = cpuOutput[0, 0, i];

        inputTensor.Dispose();
        maskTensor.Dispose();
        cpuOutput.Dispose();

        return embedding;
    }

    // ── Tokenizer ────────────────────────────────────────────
    int[] Tokenize(string sentence)
    {
        sentence = sentence.ToLower();
        string[] words = sentence.Split(' ');

        List<int> ids = new List<int>() { vocab["[CLS]"] };
        foreach (string word in words)
            ids.Add(vocab.ContainsKey(word) ? vocab[word] : vocab["[UNK]"]);
        ids.Add(vocab["[SEP]"]);

        return ids.ToArray();
    }

    // ── Vocab Loader ─────────────────────────────────────────
    void LoadVocab()
    {
        TextAsset vocabFile = Resources.Load<TextAsset>("bert-vocab");

        if (vocabFile == null)
        {
            Debug.LogError("bert-vocab.txt not found in Resources folder!");
            return;
        }

        string[] lines = vocabFile.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
            vocab[lines[i].Trim()] = i;

        Debug.Log("Vocab loaded: " + vocab.Count + " tokens");
    }
}