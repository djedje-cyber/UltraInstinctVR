using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display performances statistics 
/// </summary>
public class StatsUI : MonoBehaviour
{
    /// <summary>
    /// To display stats data
    /// </summary>
    private Text _DisplayText;

    private ProfilerRecorder m_frameTimeRecorder;
    private ProfilerRecorder m_drawCallsRecorder;
    private ProfilerRecorder m_trianglesRecorder;

    private readonly StringBuilder sb = new();
    private static readonly List<ProfilerRecorderSample> samples = new();

    public void Toggle()
    {
        enabled = !enabled;
    }

    private void Start()
    {
        _DisplayText = GetComponentInChildren<Text>();
        if (_DisplayText == null)
            Debug.LogError("A component UnityEngine.UI.Text is required on any child of this object", this);
    }

    /// <summary>
    /// Unity OnEnable function
    /// </summary>
    protected void OnEnable()
    {
        m_frameTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        m_drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        m_trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
    }

    /// <summary>
    /// Unity Ondisable function
    /// </summary>
    protected void OnDisable()
    {
        m_frameTimeRecorder.Dispose();
        m_drawCallsRecorder.Dispose();
        m_trianglesRecorder.Dispose();
    }

    /// <summary>
    /// Unity Update function
    /// </summary>
    protected void Update()
    {
        sb.Clear();
        if (m_frameTimeRecorder.Valid)
        {
            double avgFrame = GetRecorderFrameAverage(m_frameTimeRecorder);
            sb.AppendLine($"Frame Time: {avgFrame * 1e-6f:F1} ms ({1 / (avgFrame * 1e-9f):N0})");
        }
        sb.AppendLine($"Delta Time: {Time.deltaTime * 1000f:F1} ms ({1 / Time.deltaTime:N0})");
        if (m_drawCallsRecorder.Valid)
            sb.AppendLine($"Draw Calls: {m_drawCallsRecorder.LastValue}");
        if (m_trianglesRecorder.Valid)
            sb.AppendLine($"Triangles: {m_trianglesRecorder.LastValue}");
        _DisplayText.text = sb.ToString();
    }

    /// <summary>
    ///  Code from : https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.html 
    /// </summary>
    /// <param name="recorder">main thread recorder</param>
    /// <returns>average main thread time</returns>
    private static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        int samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        samples.Clear();
        recorder.CopyTo(samples);
        for (int i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samples.Count;
        return r;
    }
}
