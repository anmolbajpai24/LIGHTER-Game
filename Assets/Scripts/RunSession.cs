using System;

// Rules are independent of scenes, frame rate, UI, and persistence.
public sealed class RunSession
{
    public enum Phase { Ready, Playing, Paused, Finished }
    public Phase State { get; private set; } = Phase.Ready;
    public float Seconds { get; private set; }
    public int Score => (int)Math.Floor(Seconds);
    public int Heat => Math.Min(6, 1 + Score / 15);
    public bool Start() { if (State != Phase.Ready) return false; State = Phase.Playing; return true; }
    public void Tick(float delta) { if (State == Phase.Playing && delta > 0 && !float.IsInfinity(delta)) Seconds += delta; }
    public bool Pause() { if (State != Phase.Playing) return false; State = Phase.Paused; return true; }
    public bool Resume() { if (State != Phase.Paused) return false; State = Phase.Playing; return true; }
    public bool Finish() { if (State == Phase.Ready || State == Phase.Finished) return false; State = Phase.Finished; return true; }
}
