using System;
using UnityEngine;


public enum SimState
{
    Stopped,      // Initial or reset state; nothing is running
    Generating,   // Session generation is in progress
    Playing,      // Playback is currently underway
    Paused        // Playback is paused
}


public class SimulationStateProvider : MonoBehaviour
{
    public SimState Current { get; private set; } = SimState.Stopped;
    public event Action<SimState> OnStateChanged;

    public void SetStopped() => SetState(SimState.Stopped);
    public void SetGenerating() => SetState(SimState.Generating);
    public void SetPlaying() => SetState(SimState.Playing);
    public void SetPaused() => SetState(SimState.Paused);

    private void SetState(SimState newState)
    {
        if (Current == newState) return;
        Current = newState;
        OnStateChanged?.Invoke(newState);
    }
}