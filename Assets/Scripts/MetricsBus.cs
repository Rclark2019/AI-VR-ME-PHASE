using System;
using UnityEngine;

public class MetricsBus : MonoBehaviour
{
    // Core time-series metrics
    public float Accuracy  { get; private set; }
    public float Velocity  { get; private set; }
    public float Fatigue   { get; private set; }
    public float Confidence { get; private set; }

    // Playback position
    public int   FrameIndex     { get; private set; }
    public float ElapsedSeconds { get; private set; }

    // Anomaly information for the current update
    public bool   AnomalyDetected { get; private set; }
    public string AnomalyReason   { get; private set; }


    public event Action OnMetricsUpdated;

    public void ResetAll()
    {
        Accuracy       = 0f;
        Velocity       = 0f;
        Fatigue        = 0f;
        Confidence     = 0f;
        FrameIndex     = 0;
        ElapsedSeconds = 0f;
        AnomalyDetected = false;
        AnomalyReason   = string.Empty;

        OnMetricsUpdated?.Invoke();
    }

    public void UpdateFromFrame(
        float accuracy,
        float velocity,
        float fatigue,
        float confidence,
        int   frameIndex,
        float elapsedSeconds,
        bool  anomalyDetected,
        string anomalyReason)
    {
        Accuracy       = accuracy;
        Velocity       = velocity;
        Fatigue        = fatigue;
        Confidence     = confidence;
        FrameIndex     = frameIndex;
        ElapsedSeconds = elapsedSeconds;

        AnomalyDetected = anomalyDetected;
        AnomalyReason   = anomalyDetected ? anomalyReason : string.Empty;

        OnMetricsUpdated?.Invoke();
    }
}
