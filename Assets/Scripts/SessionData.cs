using System;

[Serializable]
public class SessionData
{
    public string modelName;          
    public int totalFrames;           
    public float sessionDuration;      
    public float[] accuracyData;       
    public float[] velocityData;      
    public float[] fatigueData;        
    public float[] confidenceData;     
    public DateTime timestamp;         


    public int[] anomalyFrames;

    public string[] anomalyReasons;
}