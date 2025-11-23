using UnityEngine;
using UnityEngine.UI;
using TMPro; 

// ConsolidatedDashboard drives the main UI components: charts, insight
// panel and metric cards.  It reads data from the MetricsBus each
// frame and updates the visual representations accordingly.  This class
// contains simple heuristic logic to generate insight messages based on
// current metric values.
public class ConsolidatedDashboard : MonoBehaviour
{
    [Header("Chart Components")]
    [SerializeField] private EnhancedLineChart accuracyChart;
    [SerializeField] private EnhancedLineChart velocityChart;
    [SerializeField] private EnhancedLineChart fatigueChart;
    [SerializeField] private ChartFeederUI chartFeeder;

    [Header("Insight Components")]
    [SerializeField] private TMP_Text  insightTitleText;
    [SerializeField] private TMP_Text  insightBodyText;
    [SerializeField] private TMP_Text  insightRecommendationText;
    [SerializeField] private Image insightIcon;

    [Header("Metric Card Components")]
    [SerializeField] private TMP_Text  accuracyValueText;
    [SerializeField] private Image accuracyStatusDot;
    [SerializeField] private Image accuracyCardBackground;  // NEW: Card background
    [SerializeField] private TMP_Text  velocityValueText;
    [SerializeField] private Image velocityStatusDot;
    [SerializeField] private Image velocityCardBackground;  // NEW: Card background
    [SerializeField] private TMP_Text  fatigueValueText;
    [SerializeField] private Image fatigueStatusDot;
    [SerializeField] private Image fatigueCardBackground;   // NEW: Card background
    [SerializeField] private TMP_Text  confidenceValueText;
    [SerializeField] private Image confidenceStatusDot;
    [SerializeField] private Image confidenceCardBackground; // NEW: Card background

    [Header("Colors")]
    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f);
    [SerializeField] private Color cautionColor = new Color(1f, 0.78f, 0f);
    [SerializeField] private Color alertColor = new Color(1f, 0.2f, 0.2f);

    [Header("Card Background Colors")]
    [Tooltip("Card background tint for Good status (default: light green)")]
    [SerializeField] private Color goodCardColor = new Color(0.8f, 1f, 0.8f, 1f);
    [Tooltip("Card background tint for Caution status (default: light yellow)")]
    [SerializeField] private Color cautionCardColor = new Color(1f, 0.95f, 0.7f, 1f);
    [Tooltip("Card background tint for Alert status (default: light red)")]
    [SerializeField] private Color alertCardColor = new Color(1f, 0.8f, 0.8f, 1f);

    private MetricsBus metricsBus;

    private void Awake()
    {
        metricsBus = FindAnyObjectByType<MetricsBus>();
    }

    // Called by DigitalTwinManager's poller.  Pulls latest metrics from
    // MetricsBus and updates charts, insight panel and metric cards.
    public void UpdateFromMetrics()
    {
        if (metricsBus == null) return;
        float acc = metricsBus.Accuracy;
        float vel = metricsBus.Velocity;
        float fat = metricsBus.Fatigue;
        float conf = metricsBus.Confidence;
        // Update metric cards
        UpdateMetricCards(acc, vel, fat, conf);
        // Generate insights
        UpdateInsightPanel(acc, vel, fat, conf);
    }

    // Updates the metric cards with current values and status dots.
    private void UpdateMetricCards(float accuracy, float velocity, float fatigue, float confidence)
    {
        if (accuracyValueText != null) accuracyValueText.text = accuracy.ToString("F1") + "%";
        if (velocityValueText != null) velocityValueText.text = velocity.ToString("F2") + " m/s";
        if (fatigueValueText != null) fatigueValueText.text = fatigue.ToString("F1") + "%";
        if (confidenceValueText != null) confidenceValueText.text = confidence.ToString("F1") + "%";
        
        // Get status for each metric
        string accuracyStatus = GetStatusFromAccuracyFatigue(accuracy, fatigue);
        string velocityStatus = GetStatusFromVelocity(velocity);
        string fatigueStatus = GetStatusFromAccuracyFatigue(accuracy, fatigue);
        string confidenceStatus = GetStatusFromConfidence(confidence);
        
        // Update status dots (original functionality)
        SetStatusDot(accuracyStatusDot, accuracyStatus);
        SetStatusDot(velocityStatusDot, velocityStatus);
        SetStatusDot(fatigueStatusDot, fatigueStatus);
        SetStatusDot(confidenceStatusDot, confidenceStatus);
        
        // NEW: Update card backgrounds to match status
        SetCardBackground(accuracyCardBackground, accuracyStatus);
        SetCardBackground(velocityCardBackground, velocityStatus);
        SetCardBackground(fatigueCardBackground, fatigueStatus);
        SetCardBackground(confidenceCardBackground, confidenceStatus);
    }

    // Generates an insight message based on heuristics.  This example uses
    // simple thresholds; real systems could incorporate more advanced
    // analytics.  Updates the insight title, body and recommendation.
    private void UpdateInsightPanel(float accuracy, float velocity, float fatigue, float confidence)
{
    if (insightTitleText == null || insightBodyText == null || insightRecommendationText == null) return;

    // 1) If an anomaly is active, this always takes priority.
    if (metricsBus != null && metricsBus.AnomalyDetected)
    {
        insightTitleText.text = "🚨 Anomaly Detected";

        insightBodyText.text = string.IsNullOrEmpty(metricsBus.AnomalyReason)
            ? "The system detected an irregular performance pattern."
            : metricsBus.AnomalyReason;

        insightRecommendationText.text =
            "Pause or slow the session and assess the patient's condition.";

        if (insightIcon != null) insightIcon.color = alertColor;
        return;
    }

    // 2) Otherwise, fall back to normal status logic (Good / Caution / Alert)
    string status = GetStatusFromAccuracyFatigue(accuracy, fatigue);
    switch (status)
    {
        case "Good":
            insightTitleText.text = "✅ All Metrics Within Safe Range";
            insightBodyText.text = "Patient performance is stable and within expected parameters.";
            insightRecommendationText.text = "Keep current pace.";
            if (insightIcon != null) insightIcon.color = goodColor;
            break;

        case "Caution":
            insightTitleText.text = "🚨 Performance Deteriorating";
            insightBodyText.text = "Accuracy or fatigue levels are approaching unsafe thresholds.";
            insightRecommendationText.text = "Consider reducing intensity or taking a short rest.";
            if (insightIcon != null) insightIcon.color = cautionColor;
            break;

        default: // Alert
            insightTitleText.text = "⛔ Alert: Unsafe Performance";
            insightBodyText.text = "Metrics indicate a high-risk state (low accuracy and/or high fatigue).";
            insightRecommendationText.text = "Stop the exercise and allow the patient to recover.";
            if (insightIcon != null) insightIcon.color = alertColor;
            break;
    }
}


    // Maps accuracy and fatigue values to Good/Caution/Alert categories.
    private string GetStatusFromAccuracyFatigue(float accuracy, float fatigue)
    {
        if (accuracy >= 70f && fatigue < 65f) return "Good";
        if (accuracy >= 50f && accuracy < 70f || (fatigue >= 65f && fatigue <= 85f)) return "Caution";
        return "Alert";
    }

    // Maps velocity to status categories.  Here we use example thresholds
    // where normal velocity range is 0.8–1.5 m/s.  Values outside this
    // range raise caution or alert states.
    private string GetStatusFromVelocity(float velocity)
    {
        if (velocity >= 0.8f && velocity <= 1.5f) return "Good";
        if (velocity >= 0.5f && velocity < 0.8f || velocity > 1.5f) return "Caution";
        return "Alert";
    }

    // Maps confidence to status categories.  This uses simple ranges
    // where high confidence is above 80%, moderate 60–80%, low below 60%.
    private string GetStatusFromConfidence(float confidence)
    {
        if (confidence >= 80f) return "Good";
        if (confidence >= 60f && confidence < 80f) return "Caution";
        return "Alert";
    }

    // Set the colour of a status indicator dot based on the status name.
    private void SetStatusDot(Image dot, string status)
    {
        if (dot == null) return;
        switch (status)
        {
            case "Good": dot.color = goodColor; break;
            case "Caution": dot.color = cautionColor; break;
            case "Alert": dot.color = alertColor; break;
        }
    }

    // NEW: Set the background color of a metric card based on status
    private void SetCardBackground(Image cardBackground, string status)
    {
        if (cardBackground == null) return;
        switch (status)
        {
            case "Good": cardBackground.color = goodCardColor; break;
            case "Caution": cardBackground.color = cautionCardColor; break;
            case "Alert": cardBackground.color = alertCardColor; break;
        }
    }

    // Clears all UI elements.  Called when resetting or generating a
    // session.  Also instructs the ChartFeederUI to clear its buffers.
    public void ResetDashboard()
    {
        // Clear charts via ChartFeederUI
        if (chartFeeder != null) chartFeeder.ClearAll();
        // Reset metric texts
        if (accuracyValueText != null) accuracyValueText.text = "--";
        if (velocityValueText != null) velocityValueText.text = "--";
        if (fatigueValueText != null) fatigueValueText.text = "--";
        if (confidenceValueText != null) confidenceValueText.text = "--";
        // Reset insight panel
        if (insightTitleText != null) insightTitleText.text = "No Data";
        if (insightBodyText != null) insightBodyText.text = "Generate and play a session to view insights.";
        if (insightRecommendationText != null) insightRecommendationText.text = string.Empty;
        
        // NEW: Reset card backgrounds to Good status
        SetCardBackground(accuracyCardBackground, "Good");
        SetCardBackground(velocityCardBackground, "Good");
        SetCardBackground(fatigueCardBackground, "Good");
        SetCardBackground(confidenceCardBackground, "Good");
        
        // Reset status dots
        SetStatusDot(accuracyStatusDot, "Good");
        SetStatusDot(velocityStatusDot, "Good");
        SetStatusDot(fatigueStatusDot, "Good");
        SetStatusDot(confidenceStatusDot, "Good");
    }
}