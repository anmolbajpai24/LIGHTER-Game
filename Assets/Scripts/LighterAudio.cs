using UnityEngine;

public class LighterAudio : MonoBehaviour
{
    private static LighterAudio instance;
    private AudioSource source;
    public static bool Muted => PlayerPrefs.GetInt("lighter.muted", 0) == 1;
    private void Awake() { instance = this; source = gameObject.AddComponent<AudioSource>(); source.playOnAwake = false; }
    public static void Toggle() { PlayerPrefs.SetInt("lighter.muted", Muted ? 0 : 1); PlayerPrefs.Save(); }
    public static void Play(float frequency, float duration, float volume = .2f)
    {
        if (instance == null || Muted) return;
        const int rate = 22050;
        var samples = new float[Mathf.CeilToInt(rate * duration)];
        for (int i = 0; i < samples.Length; i++)
        {
            float envelope = Mathf.Sin(Mathf.PI * i / samples.Length);
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / rate) * envelope * envelope;
        }
        var clip = AudioClip.Create("Lighter tone", samples.Length, 1, rate, false);
        clip.SetData(samples, 0);
        instance.source.PlayOneShot(clip, volume);
        Destroy(clip, duration + .2f);
    }
    private void OnDestroy() { if (instance == this) instance = null; }
}
