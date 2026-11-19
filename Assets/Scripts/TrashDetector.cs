using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public class TrashDetector : MonoBehaviour
{
    [Header("Model")]
    public ModelAsset modelAsset;
    public TextAsset classesAsset;

    [Header("Settings")]
    [Range(0.1f, 0.9f)] public float confidenceThreshold = 0.4f;
    public int maxDetections = 5;

    private Worker worker;
    private string[] labels;
    private const int ImageSize = 640;

    void Start()
    {
        labels = classesAsset.text.Split('\n');
        var model = ModelLoader.Load(modelAsset);

        // Build a graph that does NMS for us
        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model);
        var output = Functional.Forward(model, inputs)[0]; // shape (1, 4+numClasses, 8400)

        var boxCoords = output[0, 0..4, ..].Transpose(0, 1);          // (8400, 4)
        var allScores = output[0, 4.., ..];                           // (numClasses, 8400)
        var scores = Functional.ReduceMax(allScores, 0);              // (8400)
        var classIDs = Functional.ArgMax(allScores, 0);               // (8400)

        var centersToCorners = new Tensor<float>(new TensorShape(4, 4), new float[]
        {
            1, 0, 1, 0,
            0, 1, 0, 1,
           -0.5f, 0, 0.5f, 0,
            0, -0.5f, 0, 0.5f
        });
        var boxCorners = Functional.MatMul(boxCoords, Functional.Constant(centersToCorners));

        var indices = Functional.NMS(boxCorners, scores, 0.5f, confidenceThreshold);
        var finalCoords = Functional.IndexSelect(boxCoords, 0, indices);
        var finalClasses = Functional.IndexSelect(classIDs, 0, indices);
        var finalScores = Functional.IndexSelect(scores, 0, indices);

        worker = new Worker(graph.Compile(finalCoords, finalClasses, finalScores), BackendType.GPUCompute);
    }

    public List<(string className, float confidence)> Detect(Texture2D photo)
    {
        var results = new List<(string, float)>();

        // Resize & convert photo to tensor
        using var input = TextureConverter.ToTensor(photo, ImageSize, ImageSize, 3);

        worker.Schedule(input);

        var coords = worker.PeekOutput(0) as Tensor<float>;
        var classIds = worker.PeekOutput(1) as Tensor<int>;
        var scores = worker.PeekOutput(2) as Tensor<float>;

        var coordsCpu = coords.ReadbackAndClone();
        var classIdsCpu = classIds.ReadbackAndClone();
        var scoresCpu = scores.ReadbackAndClone();

        int count = Mathf.Min(maxDetections, scoresCpu.shape[0]);

        for (int i = 0; i < count; i++)
        {
            float conf = scoresCpu[i];
            if (conf < confidenceThreshold) continue;

            int classIndex = classIdsCpu[i];
            string name = (classIndex >= 0 && classIndex < labels.Length)
                ? labels[classIndex].Trim()
                : "unknown";

            results.Add((name, conf));
        }

        coordsCpu.Dispose();
        classIdsCpu.Dispose();
        scoresCpu.Dispose();

        return results;
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}