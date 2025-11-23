using System.Collections;
using UnityEngine;

public class ChartFeederUI : MonoBehaviour
{
    [SerializeField] private EnhancedLineChart accuracyChart;
    [SerializeField] private EnhancedLineChart velocityChart;
    [SerializeField] private EnhancedLineChart fatigueChart;
    [SerializeField] private MetricsBus metricsBus;
    [SerializeField] private SimulationStateProvider stateProvider;

    [Tooltip("Number of updates per second to push data to the charts.")]
    [SerializeField] private float feedHz = 30f; 


    [Tooltip("Normalize data to 0-1 range for charts.")]
    [SerializeField] private bool normalize01 = false;  
    
    [Tooltip("Maximum velocity used for normalization.")]
    [SerializeField] private float velocityMax = 2.0f;

    private Coroutine feedCoroutine;

    private void OnEnable()
    {
        StartFeeding();
    }

    private void OnDisable()
    {
        StopFeeding();
    }

    // Start the feeding coroutine if it isn't already running.
    public void StartFeeding()
    {
        if (feedCoroutine == null)
        {
            feedCoroutine = StartCoroutine(FeedLoop());
        }
    }

    // Stop the feeding coroutine.
    public void StopFeeding()
    {
        if (feedCoroutine != null)
        {
            StopCoroutine(feedCoroutine);
            feedCoroutine = null;
        }
    }

    // Clear all charts and reset buffers.
    public void ClearAll()
    {
        if (accuracyChart != null) accuracyChart.ClearChart();
        if (velocityChart != null) velocityChart.ClearChart();
        if (fatigueChart != null) fatigueChart.ClearChart();
    }

    private IEnumerator FeedLoop()
    {
        var wait = new WaitForSeconds(1f / feedHz);
        while (true)
        {
            // Only feed data when playing
            if (stateProvider != null && stateProvider.Current == SimState.Playing && metricsBus != null)
            {
                float x = metricsBus.ElapsedSeconds;
                
                float acc = normalize01 ? metricsBus.Accuracy / 100f : metricsBus.Accuracy;
                float vel = normalize01 ? metricsBus.Velocity / velocityMax : metricsBus.Velocity;
                float fat = normalize01 ? metricsBus.Fatigue / 100f : metricsBus.Fatigue;
                
                // Append to charts
                if (accuracyChart != null) accuracyChart.AddDataPoint(x, acc);
                if (velocityChart != null) velocityChart.AddDataPoint(x, vel);
                if (fatigueChart != null) fatigueChart.AddDataPoint(x, fat);
                
            }
            yield return wait;
        }
    }
}