using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceRecognizer : MonoBehaviour
{
    // ── Fields ───────────────────────────────────────────────
    public ControllerSelector controllerSelector;
    public BERTInference bertInference;

    private KeywordRecognizer keywordRecognizer;

    private string[] keywords = new string[]
    {
        "put that",
        "grab that",
        "drop that",
        "move that"
    };

    // ── Unity Lifecycle ──────────────────────────────────────
    void Start()
    {
        keywordRecognizer = new KeywordRecognizer(keywords);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();

        Debug.Log("Voice recognizer ready — say 'Put that'!");
    }

    void OnDestroy()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }

    // ── Callback ─────────────────────────────────────────────
    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Heard: " + args.text);
        bertInference.ProcessVoiceCommand(args.text);
    }
}