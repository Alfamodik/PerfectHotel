using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    [SerializeField] private List<AudioClip> playlist = new List<AudioClip>();
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool loopCurrentTrack;

    private int currentTrackIndex;
    private AudioSource audioSource;
    private bool isPaused;
    private bool isStoppedManually;

    public int CurrentTrackIndex => currentTrackIndex;
    public AudioClip CurrentTrack => HasTracks() ? playlist[currentTrackIndex] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = loopCurrentTrack;
    }

    private void Start()
    {
        if (playOnStart == false)
            return;

        PlayCurrent();
    }

    private void Update()
    {
        if (ShouldPlayNextTrackAutomatically() == false)
            return;

        PlayNext();
    }

    public void PlayCurrent()
    {
        int playableTrackIndex = GetNextPlayableTrackIndex(currentTrackIndex, 1);

        if (playableTrackIndex < 0)
            return;

        currentTrackIndex = playableTrackIndex;
        PlayClip(playlist[currentTrackIndex]);
    }

    [ContextMenu("PlayNext")]
    public void PlayNext()
    {
        if (HasTracks() == false)
            return;

        int playableTrackIndex = GetNextPlayableTrackIndex(currentTrackIndex + 1, 1);

        if (playableTrackIndex < 0)
            return;

        currentTrackIndex = playableTrackIndex;
        PlayClip(playlist[currentTrackIndex]);
    }

    [ContextMenu("PlayPrevious")]
    public void PlayPrevious()
    {
        if (HasTracks() == false)
            return;

        int playableTrackIndex = GetNextPlayableTrackIndex(currentTrackIndex - 1, -1);

        if (playableTrackIndex < 0)
            return;

        currentTrackIndex = playableTrackIndex;
        PlayClip(playlist[currentTrackIndex]);
    }

    public void PlayByIndex(int trackIndex)
    {
        if (HasTracks() == false)
            return;

        if (trackIndex < 0 || trackIndex >= playlist.Count)
            return;

        int playableTrackIndex = GetNextPlayableTrackIndex(trackIndex, 1);

        if (playableTrackIndex < 0)
            return;

        currentTrackIndex = playableTrackIndex;
        PlayClip(playlist[currentTrackIndex]);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        if (audioSource.isPlaying == false)
            return;

        isPaused = true;
        audioSource.Pause();
    }

    [ContextMenu("Resume")]
    public void Resume()
    {
        if (audioSource.clip == null)
            return;

        isPaused = false;
        isStoppedManually = false;
        audioSource.UnPause();
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        isPaused = false;
        isStoppedManually = true;
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void SetPlaylist(List<AudioClip> newPlaylist, bool playFirstTrackImmediately = false)
    {
        playlist = newPlaylist ?? new List<AudioClip>();
        currentTrackIndex = 0;
        isPaused = false;
        isStoppedManually = true;
        audioSource.Stop();

        if (playFirstTrackImmediately == false)
            return;

        PlayCurrent();
    }

    private void PlayClip(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        isPaused = false;
        isStoppedManually = false;

        audioSource.loop = loopCurrentTrack;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private bool ShouldPlayNextTrackAutomatically()
    {
        if (HasTracks() == false)
            return false;

        if (loopCurrentTrack)
            return false;

        if (isPaused)
            return false;

        if (isStoppedManually)
            return false;

        if (audioSource.clip == null)
            return false;

        if (audioSource.isPlaying)
            return false;

        return audioSource.timeSamples > 0;
    }

    private int GetNextPlayableTrackIndex(int startIndex, int step)
    {
        if (HasTracks() == false)
            return -1;

        int checkedTracksCount = 0;
        int trackIndex = WrapTrackIndex(startIndex);

        while (checkedTracksCount < playlist.Count)
        {
            if (playlist[trackIndex] != null)
                return trackIndex;

            trackIndex = WrapTrackIndex(trackIndex + step);
            checkedTracksCount++;
        }

        return -1;
    }

    private int WrapTrackIndex(int trackIndex)
    {
        if (trackIndex < 0)
            return playlist.Count - 1;

        if (trackIndex >= playlist.Count)
            return 0;

        return trackIndex;
    }

    private bool HasTracks()
    {
        return playlist != null && playlist.Count > 0;
    }
}