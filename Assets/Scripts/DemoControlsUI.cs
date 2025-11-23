using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class DemoControlsUI : MonoBehaviour
{
    [SerializeField] private Button generateButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button exportButton;
    [SerializeField] private Button switchModelButton;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TMP_Text  speedText;
    [SerializeField] private DigitalTwinManager digitalTwinManager;

    private void Start()
    {
        // Initialize slider and attach listeners programmatically if not
        // configured in the inspector
        if (speedSlider != null)
        {
            speedSlider.minValue = 0.1f;
            speedSlider.maxValue = 2.0f;
            speedSlider.value = 1.0f;
            speedSlider.onValueChanged.AddListener(OnSpeedChanged);
            UpdateSpeedText(speedSlider.value);
        }
        if (generateButton != null) generateButton.onClick.AddListener(OnGenerateClicked);
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        if (exportButton != null) exportButton.onClick.AddListener(OnExportClicked);
        if (switchModelButton != null) switchModelButton.onClick.AddListener(OnSwitchModelClicked);
    }

    public void OnGenerateClicked()
    {
        digitalTwinManager?.OnGenerateButtonClicked();
    }

    public void OnPlayClicked()
    {
        digitalTwinManager?.OnPlayButtonClicked();
    }

    public void OnPauseClicked()
    {
        digitalTwinManager?.OnPauseButtonClicked();
    }

    public void OnResetClicked()
    {
        digitalTwinManager?.OnStopButtonClicked();
    }

    public void OnExportClicked()
    {
        digitalTwinManager?.OnExportButtonClicked();
    }

    public void OnSwitchModelClicked()
    {
        digitalTwinManager?.OnSwitchModelButtonClicked();
    }

    public void OnSpeedChanged(float value)
    {
        digitalTwinManager?.SetPlaybackSpeed(value);
        UpdateSpeedText(value);
    }

    private void UpdateSpeedText(float value)
    {
        if (speedText != null) speedText.text = $"Speed: {value:0.00}x";
    }
}