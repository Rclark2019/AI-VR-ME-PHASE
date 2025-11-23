using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeaderBarUI : MonoBehaviour
{
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI modelText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI progressFrameText;   

    [Header("Progress Bar")]
    [SerializeField] private Image progressFillImage;             

    [Header("State (optional)")]
    [SerializeField] private SimulationStateProvider stateProvider;
    [SerializeField] private SimulationController simulationController;

    private void Awake()
    {
        // Initialise with safe defaults
        SetTitle("AI-VR-ME — Your AI Digital Twin Dashboard");
        SetModel("A");
        SetTime(0f);
        SetStatus("STOPPED");
        SetProgress(0f);
        SetProgressFrames(0, 0);
    }

    private void Update()
    {

        if (stateProvider != null)
        {
            string status = stateProvider.Current.ToString().ToUpper();
            SetStatus(status);
        }
    }

    // Set the main header title.
    public void SetTitle(string title)
    {
        if (titleText != null)
            titleText.text = title;
    }

    // Set the current model (A/B/C, etc).
    public void SetModel(string modelKey)
    {
        if (modelText != null)
            modelText.text = $"Model: {modelKey}";
    }

    // Set elapsed time in seconds.
    public void SetTime(float seconds)
    {
        if (timeText == null)
            return;

        int totalSeconds = Mathf.Max(0, Mathf.RoundToInt(seconds));
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;
        timeText.text = $"Time: {mins:00}:{secs:00}";
    }

    // Set playback status text (PLAYING / PAUSED / STOPPED).
    public void SetStatus(string status)
    {
        if (statusText != null)
            statusText.text = $"Status: {status}";
    }

    // Set the progress bar fill (0–1).
    public void SetProgress(float progress)
    {
        if (progressFillImage != null)
            progressFillImage.fillAmount = Mathf.Clamp01(progress);
    }

    // Set the numeric frame counter (e.g. "123 / 9000").
    public void SetProgressFrames(int currentFrame, int totalFrames)
    {
        if (progressFrameText == null)
            return;

        if (totalFrames > 0 && currentFrame >= 0)
            progressFrameText.text = $"{currentFrame}/{totalFrames}";
        else
            progressFrameText.text = "0/0";
    }
}
