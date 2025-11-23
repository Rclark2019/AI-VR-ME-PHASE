using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnalyticsDashboard : MonoBehaviour
{
    [Header("Text Fields")]
    public TextMeshProUGUI timeStampText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI fatigueText;
    public TextMeshProUGUI confidenceText;
    public TextMeshProUGUI anomalyCountText;

    [Header("Slider Fields")]
    public Slider accuracySlider;
    public Slider velocitySlider;
    public Slider fatigueSlider;
    public Slider confidenceSlider;

    [Header("Chart Renderers")]
    public LineChartRenderer accuracyChart;
    public LineChartRenderer velocityChart;
    public LineChartRenderer fatigueChart;

    [Header("Threshold Settings")]
    public float goodThreshold = 0.7f;
    public float warningThreshold = 0.4f;
    public Color goodColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color alertColor = Color.red;

    public void ResetDashboard()
    {
        if (timeStampText) timeStampText.text = "Time: 00:00";
        if (accuracyText) accuracyText.text = "Accuracy: --";
        if (velocityText) velocityText.text = "Velocity: --";
        if (fatigueText) fatigueText.text = "Fatigue: --";
        if (confidenceText) confidenceText.text = "Confidence: --";
        if (anomalyCountText) anomalyCountText.text = "Anomalies: 0";

        if (accuracySlider) accuracySlider.value = 0;
        if (velocitySlider) velocitySlider.value = 0;
        if (fatigueSlider) fatigueSlider.value = 0;
        if (confidenceSlider) confidenceSlider.value = 0;

        if (accuracyChart) accuracyChart.ClearData();
        if (velocityChart) velocityChart.ClearData();
        if (fatigueChart) fatigueChart.ClearData();

        if (accuracyText) accuracyText.color = Color.white;
        if (velocityText) velocityText.color = Color.white;
        if (fatigueText) fatigueText.color = Color.white;
        if (confidenceText) confidenceText.color = Color.white;
    }

    public void UpdateDashboard(FrameData frame, float currentTime, int anomalyCount)
    {
        if (timeStampText)
        {
            int minutes = (int)currentTime / 60;
            int seconds = (int)currentTime % 60;
            timeStampText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        if (accuracyText)
        {
            int percent = Mathf.RoundToInt(frame.accuracy * 100f);
            accuracyText.text = $"Accuracy: {percent}%";
            accuracyText.color = (frame.accuracy >= goodThreshold) ? goodColor :
                                  (frame.accuracy >= warningThreshold) ? warningColor : alertColor;
        }
        if (accuracySlider) accuracySlider.value = frame.accuracy;

        if (velocityText) velocityText.text = $"Velocity: {frame.velocity:F1}";
        if (velocitySlider) velocitySlider.value = Mathf.Clamp01(frame.velocity / 2.0f);

        if (fatigueText)
        {
            int percentFatigue = Mathf.RoundToInt(frame.fatigue * 100f);
            fatigueText.text = $"Fatigue: {percentFatigue}%";
            fatigueText.color = (frame.fatigue <= warningThreshold) ? goodColor :
                                 (frame.fatigue <= goodThreshold) ? warningColor : alertColor;
        }
        if (fatigueSlider) fatigueSlider.value = frame.fatigue;

        if (confidenceText)
        {
            int percentConf = Mathf.RoundToInt(frame.confidence * 100f);
            confidenceText.text = $"Confidence: {percentConf}%";
            confidenceText.color = (frame.confidence >= goodThreshold) ? goodColor :
                                    (frame.confidence >= warningThreshold) ? warningColor : alertColor;
        }
        if (confidenceSlider) confidenceSlider.value = frame.confidence;

        if (anomalyCountText) anomalyCountText.text = $"Anomalies: {anomalyCount}";

        if (accuracyChart) accuracyChart.AddDataPoint(frame.accuracy);
        if (velocityChart) velocityChart.AddDataPoint(frame.velocity / 2.0f);
        if (fatigueChart) fatigueChart.AddDataPoint(frame.fatigue);
    }
}
