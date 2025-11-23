using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InsightPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimulationStateProvider stateProvider;
    [SerializeField] private MetricsBus metricsBus;

    [Header("UI Components - Assign from CurrentInsightPanel")]
    [SerializeField] private Image insightIcon;                    
    [SerializeField] private TextMeshProUGUI insightTitle;        
    [SerializeField] private TextMeshProUGUI insightBody;          // InsightBody
    [SerializeField] private TextMeshProUGUI insightRecommendation; // InsightRecommendation

    [Header("Colors")]
    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f);      // Green
    [SerializeField] private Color cautionColor = new Color(1f, 0.78f, 0f); // Yellow/Orange
    [SerializeField] private Color alertColor = new Color(1f, 0.2f, 0.2f);  // Red

    [Header("Settings")]
    [SerializeField] private bool showAnomalyCount = true;

    [Header("Anomaly Insight Behavior")]
    [SerializeField] private float anomalyInsightHoldSeconds = 2f;


private float lastAnomalyDetectedTime = -999f;


    private bool lastAnomalyState = false;
    private int anomalyCount = 0;

    private void Awake()
    {
        // Initialize with default text
        SetInsight(
            "Ready", 
            "Generate a session to begin", 
            "Press Generate button, then Play to start visualization.", 
            goodColor
        );
    }

    private void OnEnable()
    {
        if (metricsBus != null)
            metricsBus.OnMetricsUpdated += OnMetricsUpdated;

        if (stateProvider != null)
            stateProvider.OnStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        if (metricsBus != null)
            metricsBus.OnMetricsUpdated -= OnMetricsUpdated;

        if (stateProvider != null)
            stateProvider.OnStateChanged -= OnStateChanged;
    }

    private void OnMetricsUpdated()
    {
        if (metricsBus == null)
            return;

        bool currentAnomalyState = metricsBus.AnomalyDetected;
        if (currentAnomalyState && !lastAnomalyState)
        {
            anomalyCount++;
            lastAnomalyDetectedTime = Time.time;
        }
        lastAnomalyState = currentAnomalyState;

        // Update insight display based on current state
        UpdateInsight();
    }


    private void OnStateChanged(SimState newState)
    {
        // Update text for non-playing states
        if (newState != SimState.Playing)
            UpdateInsight();
    }

    private void UpdateInsight()
    {
        if (stateProvider == null || metricsBus == null)
            return;

        switch (stateProvider.Current)
        {
            case SimState.Generating:
                SetInsight(
                    "Generating Session",
                    "Creating synthetic rehabilitation data...",
                    "Preparing 9,000 frames of patient metrics.",
                    cautionColor
                );
                break;

            case SimState.Playing:
                UpdatePlayingInsight();
                break;

            case SimState.Paused:
                SetInsight(
                    "Playback Paused",
                    "Metrics held for review.",
                    $"Total anomalies detected: {anomalyCount}",
                    cautionColor
                );
                break;

            case SimState.Stopped:
            default:
                SetInsight(
                    "Ready",
                    "Generate a session to begin",
                    "Press Generate button, then Play to start visualization.",
                    goodColor
                );
                break;
        }
    }

        private void UpdatePlayingInsight()
    {

        bool anomalyActive =
            metricsBus.AnomalyDetected ||
            (Time.time - lastAnomalyDetectedTime) < anomalyInsightHoldSeconds;

        if (anomalyActive)
        {
            string body = $"Anomaly Type: {metricsBus.AnomalyReason}";
            string recommendation = showAnomalyCount
                ? $"Monitor patient closely. Total anomalies: {anomalyCount}"
                : "Monitor patient closely.";

            SetInsight("🚨 Anomaly Detected", body, recommendation, alertColor);
            return;
        }

        if (metricsBus.Fatigue > 85f)
        {
            SetInsight(
                "🚨 Critical Fatigue",
                $"Fatigue level extremely high: {metricsBus.Fatigue:F1}%",
                "Recommend immediate rest break. Patient at risk of overexertion.",
                alertColor
            );
            return;
        }

        if (metricsBus.Accuracy < 70f)
        {
            SetInsight(
                "🚨 Low Accuracy",
                $"Performance accuracy critically low: {metricsBus.Accuracy:F1}%",
                "Reduce exercise intensity. Focus on proper form.",
                alertColor
            );
            return;
        }

        if (metricsBus.Fatigue > 65f)
        {
            SetInsight(
                "🚨 Fatigue Increasing",
                $"Fatigue level rising: {metricsBus.Fatigue:F1}%",
                "Monitor closely. Prepare for potential rest break.",
                cautionColor
            );
            return;
        }

        if (metricsBus.Accuracy < 80f)
        {
            SetInsight(
                "🚨 Accuracy Declining",
                $"Performance accuracy dropping: {metricsBus.Accuracy:F1}%",
                "Encourage patient to maintain focus and proper technique.",
                cautionColor
            );
            return;
        }

        if (metricsBus.Velocity < 0.5f)
        {
            SetInsight(
                "🚨 Velocity Low",
                $"Movement velocity below optimal: {metricsBus.Velocity:F2} m/s",
                "Check patient comfort and range of motion.",
                cautionColor
            );
            return;
        }

        string statusBody = $"Accuracy: {metricsBus.Accuracy:F1}% | Velocity: {metricsBus.Velocity:F2} m/s | Fatigue: {metricsBus.Fatigue:F1}%";
        string statusRec = showAnomalyCount 
            ? $"Patient performance is stable and within expected parameters. Anomalies: {anomalyCount}"
            : "Patient performance is stable and within expected parameters.";
        
        SetInsight(" All Metrics Within Safe Range", statusBody, statusRec, goodColor);
    }

    private void SetInsight(string title, string body, string recommendation, Color iconColor)
    {
        if (insightTitle != null)
            insightTitle.text = title;

        if (insightBody != null)
            insightBody.text = body;

        if (insightRecommendation != null)
            insightRecommendation.text = recommendation;

        if (insightIcon != null)
            insightIcon.color = iconColor;
    }

    public void ResetAnomalyCount()
    {
        anomalyCount = 0;
        lastAnomalyState = false;
        lastAnomalyDetectedTime = -999f;
        UpdateInsight();
    }

    public void RefreshInsight()
    {
        UpdateInsight();
    }
}