using System.Collections;
using UnityEngine;

public class DigitalTwinManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ModelSwitcher modelSwitcher;
    [SerializeField] private SimulationController simulationController;
    [SerializeField] private MetricsBus metricsBus;
    [SerializeField] private SimulationStateProvider stateProvider;
    [SerializeField] private ConsolidatedDashboard dashboard;
    [SerializeField] private HeaderBarUI headerBar;
    [SerializeField] private AnomalyPanelUI anomalyPanel;
    [SerializeField] private InsightPanelUI insightPanel;  
    [SerializeField] private DataExporter dataExporter;
    [SerializeField] private ToastManager toastManager;
    [SerializeField] private ChartFeederUI chartFeeder;


    // Current session data and anomaly scheduling
    private SessionData currentSession;
    private int nextAnomalyIndex;

    // Reference to the running metrics poller coroutine
    private Coroutine metricsCoroutine;

    // PUBLIC BUTTON HANDLERS -------------------------------------------------

    public void OnGenerateButtonClicked()
    {
        if (stateProvider.Current == SimState.Generating) return;
        // Generate a new session via the active model
        stateProvider.SetGenerating();
        var generator = modelSwitcher.GetActiveGenerator();
        if (generator == null)
        {
            Debug.LogError("No DataGenerator is assigned to ModelSwitcher");
            toastManager?.Show("No model assigned", 2f);
            stateProvider.SetStopped();
            return;
        }
        currentSession = generator.GenerateSessionData();
        // Reset anomaly index
        nextAnomalyIndex = 0;
        // Load session into simulation controller
        simulationController.LoadSession(currentSession);
        // Reset dashboard and other UI
        dashboard.ResetDashboard();
        
        // // Reset panels
        if (anomalyPanel != null) anomalyPanel.ResetAnomalyCount();
        if (insightPanel != null) insightPanel.ResetAnomalyCount();
      
        // Clear charts via ChartFeederUI
        if (chartFeeder != null) chartFeeder.ClearAll();
      
        // Reset header bar time + progress
        headerBar.SetTime(0f);
        headerBar.SetProgress(0f);
        headerBar.SetProgressFrames(0, currentSession.totalFrames);

        // Stop any running metric coroutine
        if (metricsCoroutine != null) StopCoroutine(metricsCoroutine);
        // Transition back to stopped state
        stateProvider.SetStopped();
        toastManager?.Show("Session generated successfully!", 2f);
    }

    // Called by DemoControlsUI when the Play button is clicked.
    public void OnPlayButtonClicked()
    {
        if (currentSession == null)
        {
            toastManager?.Show("Please generate a session first", 2f);
            return;
        }
        if (stateProvider.Current == SimState.Playing) return;
        simulationController.StartPlayback();
        stateProvider.SetPlaying();
        // Start polling metrics every frame to update MetricsBus
        if (metricsCoroutine != null) StopCoroutine(metricsCoroutine);
        metricsCoroutine = StartCoroutine(MetricsPoller());
        toastManager?.Show("Playback started", 1.5f);
    }

    // Called by DemoControlsUI when the Pause button is clicked.
    public void OnPauseButtonClicked()
    {
        if (stateProvider.Current != SimState.Playing) return;
        simulationController.PausePlayback();
        stateProvider.SetPaused();
        toastManager?.Show("Playback paused", 1.5f);
    }


    public void OnStopButtonClicked()
    {
        simulationController.StopPlayback();
        stateProvider.SetStopped();
        if (metricsCoroutine != null)
        {
            StopCoroutine(metricsCoroutine);
            metricsCoroutine = null;
        }
        // Clear dashboard and metrics
        dashboard.ResetDashboard();
        
        // // Reset panels
        if (anomalyPanel != null) anomalyPanel.ResetAnomalyCount();
        
        // Clear charts via ChartFeederUI
        if (chartFeeder != null) chartFeeder.ClearAll();
        
        headerBar.SetTime(0f);
        if (currentSession != null)
            headerBar.SetProgressFrames(0, currentSession.totalFrames);
        else
            headerBar.SetProgressFrames(0, 0);

        headerBar.SetProgress(0f);
        metricsBus.UpdateFromFrame(0f, 0f, 0f, 0f, 0, 0f, false, string.Empty);
        toastManager?.Show("Session reset", 1.5f);
    }

    public void OnExportButtonClicked()
    {
        if (currentSession == null)
        {
            toastManager?.Show("No session to export", 2f);
            return;
        }
        dataExporter.ExportSession(currentSession);
        toastManager?.Show("Session exported successfully!", 2f);
    }


    public void OnSwitchModelButtonClicked()
    {
        modelSwitcher.SwitchToNextModel();
        string modelKey = modelSwitcher.GetActiveModelKey();
        string modelName = modelSwitcher.GetCurrentModelName();
        headerBar.SetModel(modelKey);
        toastManager?.Show($"Switched to {modelName}", 1.5f);
    }


    public void SetPlaybackSpeed(float speed)
    {
        simulationController.playbackSpeed = speed;
    }

    private IEnumerator MetricsPoller()
    {
        while (stateProvider.Current == SimState.Playing)
        {
            if (currentSession == null)
                yield break;

            int frame = simulationController.CurrentFrame;

            if (frame < 0 || frame >= currentSession.totalFrames)
                break;

            float elapsed = frame / 30f;

            float accuracy   = currentSession.accuracyData[frame];
            float velocity   = currentSession.velocityData[frame];
            float fatigue    = currentSession.fatigueData[frame];
            float confidence = currentSession.confidenceData[frame];


            bool   anomalyNow = false;
            string reason     = string.Empty;

            const int anomalyWindowFrames = 10;

            if (currentSession.anomalyFrames != null &&
                currentSession.anomalyReasons != null &&
                nextAnomalyIndex < currentSession.anomalyFrames.Length)
            {
                int anomalyFrame = currentSession.anomalyFrames[nextAnomalyIndex];
                int delta        = frame - anomalyFrame;

                if (Mathf.Abs(delta) <= anomalyWindowFrames)
                {
                    anomalyNow = true;
                    reason     = currentSession.anomalyReasons[nextAnomalyIndex];
                }

                if (delta > anomalyWindowFrames)
                {
                    nextAnomalyIndex++;
                }
            }


            metricsBus.UpdateFromFrame(
                accuracy,
                velocity,
                fatigue,
                confidence,
                frame,
                elapsed,
                anomalyNow,
                reason
            );

            // Update dashboard UI
            dashboard.UpdateFromMetrics();

            headerBar.SetTime(elapsed);
            float progress = (float)frame / (currentSession.totalFrames - 1);
            headerBar.SetProgress(progress);
            headerBar.SetProgressFrames(frame, currentSession.totalFrames);

            yield return new WaitForSeconds(0.033f);

            if (!simulationController.IsPlaying)
            {
                stateProvider.SetStopped();
                break;
            }
        }

        metricsCoroutine = null;
    }
}