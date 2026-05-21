using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using System.Linq;

public class puthatthere : MonoBehaviour
{
    // ── Fields ───────────────────────────────────────────────
    [SerializeField] private ModelAsset modelAsset;

    private Worker worker;
    private Unity.InferenceEngine.Model model;
    private Dictionary<string, int> vocab = new Dictionary<string, int>();

    // ── Unity Lifecycle ──────────────────────────────────────
    void Start()
    {
        Model model  = ModelLoader.Load(modelAsset);
        worker = new Worker(model, BackendType.GPUCompute);

        LoadVocab();
        Tokenize("Put that");
    }

    void OnDestroy()
    {
        worker.Dispose();
    }

    // ── Vocab ────────────────────────────────────────────────
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

    // ── Tokenizer ────────────────────────────────────────────
    void Tokenize(string sentence)
    {
        sentence = sentence.ToLower();
        string[] words = sentence.Split(' ');

        List<int> inputIds      = new List<int>() { vocab["[CLS]"] };
        List<int> attentionMask = new List<int>() { 1 };

        foreach (string word in words)
        {
            int id = vocab.ContainsKey(word) ? vocab[word] : vocab["[UNK]"];
            inputIds.Add(id);
            attentionMask.Add(1);
        }

        inputIds.Add(vocab["[SEP]"]);
        attentionMask.Add(1);

        Debug.Log("Token IDs: " + string.Join(", ", inputIds));
        RunInference(inputIds.ToArray(), attentionMask.ToArray());
    }

    // ── Inferencece ────────────────────────────────────────────
    void RunInference(int[] inputIds, int[] attentionMask)
    {
        var inputTensor = new Tensor<int>(new TensorShape(1, inputIds.Length), inputIds);
        var maskTensor  = new Tensor<int>(new TensorShape(1, attentionMask.Length), attentionMask);

        worker.SetInput("input_ids", inputTensor);
        worker.SetInput("attention_mask", maskTensor);
        worker.Schedule();

        var output  = worker.PeekOutput() as Tensor<float>;
        var cpuOutput = output.ReadbackAndClone();

        Debug.Log("Output shape: " + cpuOutput.shape);
        Debug.Log("First 5 values: " + string.Join(", ",
            Enumerable.Range(0, 5).Select(i => cpuOutput[0, 0, i])));

        inputTensor.Dispose();
        maskTensor.Dispose();
        cpuOutput.Dispose();
    }
}