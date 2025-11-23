using System;
using System.IO;
using System.Text;
using UnityEngine;

// DataExporter is a utility class that converts SessionData into a CSV
// file.  The exported file includes a header row and all per‑frame
// metrics.  Files are written to Application.persistentDataPath with
// names that include the model key and a timestamp.
public class DataExporter : MonoBehaviour
{
    public void ExportSession(SessionData session)
    {
        if (session == null)
        {
            Debug.LogWarning("No session to export");
            return;
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Frame,Time,Accuracy,Velocity,Fatigue,Confidence");
        for (int i = 0; i < session.totalFrames; i++)
        {
            float time = i / 30f;
            sb.Append(i).Append(',');
            sb.Append(time.ToString("F3")).Append(',');
            sb.Append(session.accuracyData[i].ToString("F2")).Append(',');
            sb.Append(session.velocityData[i].ToString("F2")).Append(',');
            sb.Append(session.fatigueData[i].ToString("F2")).Append(',');
            sb.Append(session.confidenceData[i].ToString("F2")).Append('\n');
        }
        string modelKey = session.modelName.Replace(" ", "_");
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"Session_{modelKey}_{timestamp}.csv";
        string path = Path.Combine(Application.persistentDataPath, filename);
        try
        {
            File.WriteAllText(path, sb.ToString());
            Debug.Log($"Session exported to {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to export session: {ex.Message}");
        }
    }
}