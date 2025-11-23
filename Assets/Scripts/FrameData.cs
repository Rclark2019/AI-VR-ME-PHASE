using UnityEngine;

[System.Serializable]
public class FrameData
{
    public int frameIndex;
    public float time;            // seconds
    public float accuracy;        // [0,1]
    public float velocity;        // [0,2]
    public float fatigue;         // [0,1]
    public float confidence;      // [0,1]
    public float performanceDeviation; // ~sigma units
    public bool anomalyFlag;

    // Positions omitted this phase
    public Vector3 leftArmPos;
    public Vector3 rightArmPos;
    public Vector3 torsoPos;
}
