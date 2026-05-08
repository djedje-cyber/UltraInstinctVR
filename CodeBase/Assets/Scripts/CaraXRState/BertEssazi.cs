using UnityEngine;
using Unity.InferenceEngine;
public class BERTInference : MonoBehaviour
{
    public ModelAsset modelAsset; // Drag your .onnx file here

    private IWorker worker;
    private Model model;

    void Start()
    {
        model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, model);
    }

    public void RunInference(int[] inputIds, int[] attentionMask)
    {
        // Create tensors
        var inputTensor = new TensorInt(new TensorShape(1, inputIds.Length), inputIds);
        var maskTensor = new TensorInt(new TensorShape(1, attentionMask.Length), attentionMask);

        // Set inputs
        worker.SetInput("input_ids", inputTensor);
        worker.SetInput("attention_mask", maskTensor);

        // Run
        worker.Schedule();

        // Get output
        var output = worker.PeekOutput() as TensorFloat;
        output.MakeReadable();

        Debug.Log("Output shape: " + output.shape);

        inputTensor.Dispose();
        maskTensor.Dispose();
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}