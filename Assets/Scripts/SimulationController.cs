using UnityEngine;


public class SimulationController : MonoBehaviour
{

    public SessionData Session { get; private set; }

    [Range(0.1f, 2.0f)]
    public float playbackSpeed = 1f;


    public bool IsPlaying { get; private set; }
    public int CurrentFrame { get; private set; }
    private float playbackTime;


    public void LoadSession(SessionData data)
    {
        Session = data;
        playbackTime = 0f;
        CurrentFrame = 0;
        IsPlaying = false;
    }


    public void StartPlayback()
    {
        if (Session == null) return;
        IsPlaying = true;
    }


    public void PausePlayback()
    {
        IsPlaying = false;
    }

    // Stop playback and reset to the beginning of the session.
    public void StopPlayback()
    {
        IsPlaying = false;
        playbackTime = 0f;
        CurrentFrame = 0;
    }

    private void Update()
    {
        if (!IsPlaying || Session == null) return;
        // Advance internal time by the product of deltaTime and playbackSpeed
        playbackTime += Time.deltaTime * playbackSpeed;

        int frame = Mathf.FloorToInt(playbackTime * 30f);
        // Clamp to the valid range
        CurrentFrame = Mathf.Clamp(frame, 0, Session.totalFrames - 1);
        // If we've reached the end, stop automatically
        if (CurrentFrame >= Session.totalFrames - 1)
        {
            IsPlaying = false;
        }
    }
}