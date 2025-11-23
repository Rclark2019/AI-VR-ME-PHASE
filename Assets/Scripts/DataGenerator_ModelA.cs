using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI-VR-ME/Data Generator/Model A (Baseline)")]
public class DataGenerator_ModelA : DataGenerator
{
    private void Reset() { modelName = "Baseline Model"; }

    public override SessionData GenerateSessionData()
    {
        var data = CreateSessionData();

        // Baseline patient: mild decline across session
        for (int i = 0; i < data.totalFrames; i++)
        {
            float t = (float)i / (data.totalFrames - 1);

            data.accuracyData[i]   = Mathf.Lerp(92f, 84f, t);
            data.velocityData[i]   = Mathf.Lerp(1.25f, 1.0f, t);
            data.fatigueData[i]    = Mathf.Lerp(12f, 55f, t);
            data.confidenceData[i] = Mathf.Lerp(92f, 78f, t);
        }

        // 2-4 anomalies, starting after ~30s
        List<int> frames = new List<int>();
        List<string> reasons = new List<string>();

        int anomalyCount = Random.Range(2, 5);
        int minFrame = 30 * 30;                 // 30s @30fps
        int maxFrame = data.totalFrames - 120;  // leave room

        int lastFrame = minFrame;
        for (int n = 0; n < anomalyCount; n++)
        {
            int frame = Random.Range(lastFrame + 150, lastFrame + 300); // 5-10s apart
            if (frame >= maxFrame) break;
            frames.Add(frame);
            
            // Vary the anomaly reasons
            string[] possibleReasons = {
                "Accuracy dip + mild fatigue spike",
                "Slight form deviation detected",
                "Momentary concentration lapse"
            };
            reasons.Add(possibleReasons[Random.Range(0, possibleReasons.Length)]);
            lastFrame = frame;
        }

        data.anomalyFrames = frames.ToArray();
        data.anomalyReasons = reasons.ToArray();

        // Apply mild anomaly impact curves
        foreach (int f in data.anomalyFrames)
            ApplyAnomalyImpact(data, f, 0.5f);

        return data;
    }

    private void ApplyAnomalyImpact(SessionData data, int frame, float severity)
    {
        int window = 15; // 0.5s either side
        for (int i = frame - window; i <= frame + window; i++)
        {
            if (i < 0 || i >= data.totalFrames) continue;

            float w = 1f - Mathf.Abs(i - frame) / (float)window;

            data.accuracyData[i]   = Mathf.Clamp(data.accuracyData[i] - (8f * severity * w), 0f, 100f);
            data.velocityData[i]   = Mathf.Clamp(data.velocityData[i] - (0.18f * severity * w), 0.2f, 3f);
            data.fatigueData[i]    = Mathf.Clamp(data.fatigueData[i] + (10f * severity * w), 0f, 100f);
            data.confidenceData[i] = Mathf.Clamp(data.confidenceData[i] - (6f * severity * w), 0f, 100f);
        }
    }
}