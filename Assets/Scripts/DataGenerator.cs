using System;
using System.Collections.Generic;
using UnityEngine;

// DataGenerator is an abstract base class for creating synthetic
// rehabilitation session data.  Subclasses override GenerateSessionData
// to produce curves for accuracy, velocity, fatigue and confidence as
// well as a schedule of anomalies.  Unity's ScriptableObject is used
// here so that multiple instances can be created in the editor and
// easily swapped by the ModelSwitcher at runtime.
public abstract class DataGenerator : ScriptableObject
{
    [SerializeField]
    protected string modelName = "Unnamed Model";

    // Expose the model name for runtime queries.  The string returned
    // corresponds to the human‑readable name associated with this
    // generator, e.g. "Baseline Model".
    public string ModelName => modelName;

    // Subclasses must implement this method to populate all metrics and
    // anomaly information.  The generated SessionData will contain
    // arrays sized to totalFrames (default 9000) and may include
    // anomalyFrames and anomalyReasons for scheduled anomalies.
    public abstract SessionData GenerateSessionData();

    // Helper to create a pre‑initialised SessionData with arrays
    // allocated.  Subclasses can call this before filling in the data.
    protected SessionData CreateSessionData()
    {
        var data = new SessionData();
        data.modelName = modelName;
        data.totalFrames = 9000;
        data.sessionDuration = 300f;
        data.accuracyData = new float[data.totalFrames];
        data.velocityData = new float[data.totalFrames];
        data.fatigueData = new float[data.totalFrames];
        data.confidenceData = new float[data.totalFrames];
        data.timestamp = DateTime.Now;
        return data;
    }
}