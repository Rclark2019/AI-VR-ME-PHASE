using UnityEngine;
using TMPro;


public class AnomalyPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Data Source")]
    [SerializeField] private MetricsBus metricsBus;

    [Header("Display Settings")]
    [Tooltip("Seconds to keep the panel visible after an anomaly frame.")]
    [SerializeField] private float holdSeconds = 3f;

    private CanvasGroup canvasGroup;
    private int totalAnomalies = 0;
    private bool lastAnomalyFlag = false;
    private float lastAnomalyTime = -999f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        HideAnomaly();
    }

    private void OnEnable()
    {
        if (metricsBus == null)
        {
            metricsBus = FindAnyObjectByType<MetricsBus>();
        }

        if (metricsBus != null)
        {
            metricsBus.OnMetricsUpdated += HandleMetricsUpdated;
        }
        else
        {
            Debug.LogWarning("[AnomalyPanelUI] MetricsBus not found!");
        }
    }

    private void OnDisable()
    {
        if (metricsBus != null)
        {
            metricsBus.OnMetricsUpdated -= HandleMetricsUpdated;
        }
    }

    private void HandleMetricsUpdated()
    {
        if (metricsBus == null) return;

        bool currentFlag = metricsBus.AnomalyDetected;

        if (currentFlag && !lastAnomalyFlag)
        {
            totalAnomalies++;
            lastAnomalyTime = Time.time;

            string reason = string.IsNullOrEmpty(metricsBus.AnomalyReason)
                ? "An irregular pattern was detected."
                : metricsBus.AnomalyReason;

            ShowAnomaly(reason);
        }

        lastAnomalyFlag = currentFlag;

        // If no anomaly currently active and we've exceeded the hold time,
        // hide the panel.
        if (!currentFlag && canvasGroup.alpha > 0)
        {
            if (Time.time - lastAnomalyTime > holdSeconds)
            {
                HideAnomaly();
            }
        }
    }

    public void ShowAnomaly(string reason)
    {
        if (titleText != null)
            titleText.text = "⚠ Anomaly Detected";

        if (reasonText != null)
            reasonText.text = reason;

        if (countText != null)
            countText.text = $"Total anomalies: {totalAnomalies}";

        // Make panel visible and interactive
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideAnomaly()
    {
        // Make panel invisible and non-interactive
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // Optional: Reset method for when a new session is generated
    public void ResetAnomalyCount()
    {
        totalAnomalies = 0;
        lastAnomalyFlag = false;
        lastAnomalyTime = -999f;
        HideAnomaly();
    }
}
