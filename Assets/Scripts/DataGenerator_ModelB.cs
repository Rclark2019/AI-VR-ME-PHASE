using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI-VR-ME/Data Generator/Model B (Fatigued)")]
public class DataGenerator_ModelB : DataGenerator
{
    private void Reset() { modelName = "Fatigued Model"; }

    public override SessionData GenerateSessionData()
    {
        var data = CreateSessionData();

        // Fatigued patient: fast decline + early fatigue
        for (int i = 0; i < data.totalFrames; i++)
        {
            float t = (float)i / (data.totalFrames - 1);

            data.accuracyData[i]   = Mathf.Lerp(88f, 70f, t);
            data.velocityData[i]   = Mathf.Lerp(1.05f, 0.65f, t);
            data.fatigueData[i]    = Mathf.Lerp(25f, 90f, t);
            data.confidenceData[i] = Mathf.Lerp(88f, 60f, t);
        }

        // 6-10 anomalies inside first 5 seconds (more realistic clustering)
        List<int> frames = new List<int>();
        List<string> reasons = new List<string>();

        int anomalyCount = Random.Range(6, 11);
        int minFrame = 0 * 30;  // 0s (can start immediately)
        int maxFrame = 5 * 30;  // 5s

        // Generate clustered anomalies in first 5 seconds
        for (int n = 0; n < anomalyCount; n++)
        {
            // Space them roughly 15-20 frames apart (0.5-0.67s)
            int frame = minFrame + (n * Random.Range(15, 21));
            if (frame >= maxFrame) break;
            
            frames.Add(frame);
            
            // Vary the anomaly reasons for fatigued patient
            string[] possibleReasons = {
                "Severe fatigue spike + accuracy drop",
                "Energy depletion detected",
                "Form breakdown due to exhaustion",
                "Critical fatigue threshold reached"
            };
            reasons.Add(possibleReasons[Random.Range(0, possibleReasons.Length)]);
        }

        data.anomalyFrames = frames.ToArray();
        data.anomalyReasons = reasons.ToArray();

        // Strong anomaly impacts (visibly jagged early session)
        foreach (int f in data.anomalyFrames)
            ApplyAnomalyImpact(data, f, 0.85f);

        return data;
    }

    private void ApplyAnomalyImpact(SessionData data, int frame, float severity)
    {
        int window = 12; // slightly tighter spike
        for (int i = frame - window; i <= frame + window; i++)
        {
            if (i < 0 || i >= data.totalFrames) continue;

            float w = 1f - Mathf.Abs(i - frame) / (float)window;

            data.accuracyData[i]   = Mathf.Clamp(data.accuracyData[i] - (14f * severity * w), 0f, 100f);
            data.velocityData[i]   = Mathf.Clamp(data.velocityData[i] - (0.30f * severity * w), 0.2f, 3f);
            data.fatigueData[i]    = Mathf.Clamp(data.fatigueData[i] + (18f * severity * w), 0f, 100f);
            data.confidenceData[i] = Mathf.Clamp(data.confidenceData[i] - (10f * severity * w), 0f, 100f);
        }
    }
}