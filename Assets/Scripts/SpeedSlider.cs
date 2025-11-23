using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedSliderController : MonoBehaviour
{
    public Slider speedSlider;
    public TextMeshProUGUI speedValueText;
    public DigitalTwinManager twinManager; // Optional
    
    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.AddListener(OnSpeedChanged);
            UpdateSpeedText(speedSlider.value);
        }
    }
    
    private void OnSpeedChanged(float value)
    {
        UpdateSpeedText(value);
        
        // Optional: Update playback speed
        if (twinManager != null)
            twinManager.SetPlaybackSpeed(value);
    }
    
    private void UpdateSpeedText(float value)
    {
        if (speedValueText != null)
            speedValueText.text = value.ToString("F2") + "x";
    }
}