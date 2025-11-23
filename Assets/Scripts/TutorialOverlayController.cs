using UnityEngine;
using TMPro;

public class TutorialOverlayController : MonoBehaviour
{
    [Header("References")]
    public TMP_Text body;     
    public GameObject root;    

    [Header("Behavior")]
    [Tooltip("If true, tutorial shows every time scene starts. If false, only first run until completed.")]
    public bool alwaysShowEveryRun = false; 

    [Header("Tutorial Pages (Optional Override)")]
    [TextArea(3, 6)]
    public string[] customPages;

    // Default AI-VR-ME pages if customPages is empty
    readonly string[] defaultPages = {
        "This dashboard simulates a rehabilitation session using a digital twin.\n" +
        "You’ll see a 3D avatar, time-series charts (accuracy, velocity, fatigue), and live insights.",

        "2) Pick a Digital Twin model\n\n" +
        "Use [SWITCH MODEL] to cycle between:\n" +
        "• Model A – Baseline patient\n" +
        "• Model B – Fatigued patient\n" +
        "• Model C – High performer\n\n" +
        "Each model generates different synthetic data and anomaly patterns.",

        "3) Generate and play a session\n\n" +
        "• Click [GENERATE] to create a new synthetic session.\n" +
        "• Click [> PLAY] to start playback.\n" +
        "• Use [PAUSE] and [RESET] to control the timeline.\n\n" +
        "The progress bar in the header will fill as the session runs.",

        "4) Read the charts & insights\n\n" +
        "Left: Accuracy, Velocity, and Fatigue charts over time.\n" +
        "Right: Current Insight and Metrics Cards (Accuracy, Velocity, Fatigue, Confidence).\n\n" +
        "Watch for anomaly warnings and avatar color changes ((green) good, (yellow) caution, (red) alert).",

        "5) Export and use the data\n\n" +
        "• Click [EXPORT] to save the full session as CSV.\n" +
        "  (Includes all frames, metrics, and model info.) "
    };

    int step = 0;
    string[] Pages => (customPages != null && customPages.Length > 0) ? customPages : defaultPages;

    void Start()
    {
        if (!root || !body)
        {
            Debug.LogWarning("[TutorialOverlay] Missing UI references.");
            return;
        }

        // Respect PlayerPrefs unless alwaysShowEveryRun is true
        if (!alwaysShowEveryRun && PlayerPrefs.GetInt("AI_VR_ME_TutorialDone", 0) == 1)
        {
            root.SetActive(false);
            return;
        }

        root.SetActive(true);
        step = 0;
        body.text = Pages[0];
    }

    public void Next()
    {
        if (!root || !body) return;

        step++;
        if (step >= Pages.Length)
        {
            FinishTutorial();
            return;
        }
        body.text = Pages[step];
    }

    public void Skip()
    {
        FinishTutorial();
    }

    void FinishTutorial()
    {
        if (!alwaysShowEveryRun)
        {
            PlayerPrefs.SetInt("AI_VR_ME_TutorialDone", 1);
            PlayerPrefs.Save();
        }
        if (root) root.SetActive(false);
    }

    public static void ResetTutorialProgress()
    {
        PlayerPrefs.DeleteKey("AI_VR_ME_TutorialDone");
        PlayerPrefs.Save();
    }
}
