using UnityEngine;
using System.Collections;

public class AvatarColorController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;
    
    [Header("Data Source")]
    [SerializeField] private MetricsBus metricsBus;
    
    [Header("Status Colors")]
    [SerializeField] private Color goodColor = new Color(0f, 1f, 0f);
    [SerializeField] private Color cautionColor = new Color(1f, 0.78f, 0f);
    [SerializeField] private Color alertColor = new Color(1f, 0f, 0f);
    
    [Header("Color Intensity")]
    [Tooltip("How strong the color tint is (0 = no tint, 1 = full color). Recommended: 0.2-0.4")]
    [Range(0f, 1f)]
    [SerializeField] private float colorIntensity = 0.3f;
    
    [Tooltip("Base color of the avatar (usually white or gray)")]
    [SerializeField] private Color baseColor = Color.white;
    
    [Header("Emission (Glow)")]
    [Tooltip("Add glowing outline effect")]
    [SerializeField] private bool useEmission = true;
    [Tooltip("How bright the glow is")]
    [Range(0f, 2f)]
    [SerializeField] private float emissionIntensity = 0.3f;

    [Header("Anomaly Alert")]
    [Tooltip("Duration to show red alert when anomaly detected (seconds)")]
    [SerializeField] private float anomalyAlertDuration = 3f;
    [Tooltip("Color intensity during anomaly alert (usually higher than normal)")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyColorIntensity = 0.6f;

    private MaterialPropertyBlock mpb;
    private Material instanceMaterial;
    private bool isInitialized = false;
    private Color originalColor;
    
    // Anomaly tracking
    private bool isShowingAnomalyAlert = false;
    private bool lastAnomalyFlag = false;
    private Coroutine anomalyAlertCoroutine;

    private void Awake()
    {
        // Find MetricsBus if not assigned
        if (metricsBus == null)
        {
            metricsBus = FindAnyObjectByType<MetricsBus>();
        }

        // Find Renderer if not assigned
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }
        }

        // Create material instance
        if (targetRenderer != null && targetRenderer.material != null)
        {
            instanceMaterial = targetRenderer.material;
            mpb = new MaterialPropertyBlock();
            
            // Store original color
            if (instanceMaterial.HasProperty("_Color"))
            {
                originalColor = instanceMaterial.GetColor("_Color");
            }
            else if (instanceMaterial.HasProperty("_BaseColor"))
            {
                originalColor = instanceMaterial.GetColor("_BaseColor");
            }
            else
            {
                originalColor = Color.white;
            }
            
            isInitialized = true;
        }
    }

    private void OnEnable()
    {
        if (metricsBus != null)
        {
            metricsBus.OnMetricsUpdated += OnMetricsUpdated;
        }
    }

    private void OnDisable()
    {
        if (metricsBus != null)
        {
            metricsBus.OnMetricsUpdated -= OnMetricsUpdated;
        }
        
        // Stop any running anomaly alert
        if (anomalyAlertCoroutine != null)
        {
            StopCoroutine(anomalyAlertCoroutine);
            anomalyAlertCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }

    private void OnMetricsUpdated()
    {
        if (!isInitialized || metricsBus == null) return;

        bool currentAnomalyFlag = metricsBus.AnomalyDetected;
        
        if (currentAnomalyFlag && !lastAnomalyFlag)
        {
            // Anomaly just started - trigger red alert
            TriggerAnomalyAlert();
        }
        
        lastAnomalyFlag = currentAnomalyFlag;

        // If currently showing anomaly alert, don't update normal color
        if (isShowingAnomalyAlert)
        {
            return;
        }

        // Normal status-based coloring
        string status = GetStatus(metricsBus.Accuracy, metricsBus.Fatigue);
        SetStatusColor(status);
    }

    private void TriggerAnomalyAlert()
    {
        // Stop any existing alert coroutine
        if (anomalyAlertCoroutine != null)
        {
            StopCoroutine(anomalyAlertCoroutine);
        }
        
        // Start new alert
        anomalyAlertCoroutine = StartCoroutine(ShowAnomalyAlert());
    }

    private IEnumerator ShowAnomalyAlert()
    {
        isShowingAnomalyAlert = true;
        
        // Show RED alert with higher intensity
        ApplyColorTint(alertColor, anomalyColorIntensity);
        
        // Wait for duration
        yield return new WaitForSeconds(anomalyAlertDuration);
        
        // Return to normal status-based coloring
        isShowingAnomalyAlert = false;
        
        // Update to current status
        if (metricsBus != null)
        {
            string status = GetStatus(metricsBus.Accuracy, metricsBus.Fatigue);
            SetStatusColor(status);
        }
        
        anomalyAlertCoroutine = null;
    }

    private string GetStatus(float accuracy, float fatigue)
    {
        if (accuracy >= 70f && fatigue < 65f) return "Good";
        if (accuracy >= 50f && accuracy < 70f || (fatigue >= 65f && fatigue <= 85f)) return "Caution";
        return "Alert";
    }

    public void SetStatusColor(string status)
    {
        Color targetColor;
        switch (status)
        {
            case "Good": targetColor = goodColor; break;
            case "Caution": targetColor = cautionColor; break;
            case "Alert": targetColor = alertColor; break;
            default: targetColor = Color.white; break;
        }
        
        ApplyColorTint(targetColor, colorIntensity);
    }

    private void ApplyColorTint(Color tintColor, float intensity)
    {
        if (!isInitialized || targetRenderer == null || instanceMaterial == null) return;

        // Blend the status color with the base color
        Color finalColor = Color.Lerp(baseColor, tintColor, intensity);

        // Apply to material
        if (instanceMaterial.HasProperty("_Color"))
        {
            instanceMaterial.SetColor("_Color", finalColor);
        }
        
        if (instanceMaterial.HasProperty("_BaseColor"))
        {
            instanceMaterial.SetColor("_BaseColor", finalColor);
        }

        // Apply emission glow
        if (useEmission)
        {
            if (instanceMaterial.HasProperty("_EmissionColor"))
            {
                Color emissionColor = tintColor * emissionIntensity;
                instanceMaterial.SetColor("_EmissionColor", emissionColor);
                instanceMaterial.EnableKeyword("_EMISSION");
            }
        }
    }

    // Overload for normal status coloring (uses default intensity)
    private void ApplyColorTint(Color tintColor)
    {
        ApplyColorTint(tintColor, colorIntensity);
    }

    public void ResetColor()
    {
        if (!isInitialized || instanceMaterial == null) return;

        // Stop any anomaly alert
        if (anomalyAlertCoroutine != null)
        {
            StopCoroutine(anomalyAlertCoroutine);
            anomalyAlertCoroutine = null;
        }
        isShowingAnomalyAlert = false;
        lastAnomalyFlag = false;

        if (instanceMaterial.HasProperty("_Color"))
        {
            instanceMaterial.SetColor("_Color", originalColor);
        }
        
        if (instanceMaterial.HasProperty("_BaseColor"))
        {
            instanceMaterial.SetColor("_BaseColor", originalColor);
        }
        
        if (instanceMaterial.HasProperty("_EmissionColor"))
        {
            instanceMaterial.SetColor("_EmissionColor", Color.black);
            instanceMaterial.DisableKeyword("_EMISSION");
        }
    }

    // Test methods
    [ContextMenu("Test Good Color")]
    public void TestGood() => SetStatusColor("Good");

    [ContextMenu("Test Caution Color")]
    public void TestCaution() => SetStatusColor("Caution");

    [ContextMenu("Test Alert Color")]
    public void TestAlert() => SetStatusColor("Alert");

    [ContextMenu("Test Anomaly Alert (3s Red)")]
    public void TestAnomalyAlert() => TriggerAnomalyAlert();

    [ContextMenu("Reset to Original")]
    public void TestReset() => ResetColor();
}