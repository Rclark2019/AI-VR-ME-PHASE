using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI-VR-ME/Data Generator/Model C (High Performer)")]
public class DataGenerator_ModelC : DataGenerator
{
    private void Reset() { modelName = "High Performer Model"; }

    public override SessionData GenerateSessionData()
    {
        var data = CreateSessionData();

        // High performer: stable, slight improvement, low fatigue
        for (int i = 0; i < data.totalFrames; i++)
        {
            float t = (float)i / (data.totalFrames - 1);

            data.accuracyData[i]   = Mathf.Lerp(94f, 97f, t);
            data.velocityData[i]   = Mathf.Lerp(1.20f, 1.35f, t);
            data.fatigueData[i]    = Mathf.Lerp(8f, 30f, t);
            data.confidenceData[i] = Mathf.Lerp(94f, 98f, t);
        }

        // Exactly 2 anomalies, late & mild (high performers rarely have issues)
        List<int> frames = new List<int>();
        List<string> reasons = new List<string>();

        int minFrame = 60 * 30;  // after 60s
        int maxFrame = data.totalFrames - 180;

        // First anomaly between 60-80 seconds
        int frame1 = Random.Range(minFrame, minFrame + (20 * 30));
        frames.Add(frame1);
        reasons.Add("Minor technique drift");
        
        // Second anomaly well after the first (at least 20 seconds later)
        int frame2 = Random.Range(frame1 + (20 * 30), maxFrame);
        frames.Add(frame2);
        reasons.Add("Slight posture adjustment needed");

        data.anomalyFrames = frames.ToArray();
        data.anomalyReasons = reasons.ToArray();

        foreach (int f in data.anomalyFrames)
            ApplyAnomalyImpact(data, f, 0.30f);

        return data;
    }

    private void ApplyAnomalyImpact(SessionData data, int frame, float severity)
    {
        int window = 15;
        for (int i = frame - window; i <= frame + window; i++)
        {
            if (i < 0 || i >= data.totalFrames) continue;

            float w = 1f - Mathf.Abs(i - frame) / (float)window;

            data.accuracyData[i]   = Mathf.Clamp(data.accuracyData[i] - (5f * severity * w), 0f, 100f);
            data.velocityData[i]   = Mathf.Clamp(data.velocityData[i] - (0.10f * severity * w), 0.2f, 3f);
            data.fatigueData[i]    = Mathf.Clamp(data.fatigueData[i] + (6f * severity * w), 0f, 100f);
            data.confidenceData[i] = Mathf.Clamp(data.confidenceData[i] - (4f * severity * w), 0f, 100f);
        }
    }
}