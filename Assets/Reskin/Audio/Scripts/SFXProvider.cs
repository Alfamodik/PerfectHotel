using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class SFXProvider : MonoBehaviour
{
    private const string ButtonClickSoundName = "button-click";
    private const float ButtonScanInterval = 0.25f;

    private static SFXProvider _instance;
    
    [SerializeField, Range(0, 1)] private float _globalMusicVolume = 0.5f;
    [SerializeField, Range(0, 1)] private float _globalEffectsVolume = 0.5f;
    [SerializeField] private List<Sound> _sounds;

    private readonly HashSet<Button> _buttonsWithClickSound = new HashSet<Button>();
    private float _globalMusicVolumeBeforeMute;
    private float _globalEffectsVolumeBeforeMute;
    private float _buttonScanTimer;
    private bool _isMute;

    public static float GlobalMusicVolume
    {
        get => _instance._globalMusicVolume;
        set
        {
            _instance._globalMusicVolume = Mathf.Clamp01(value);
            _instance.UpdateSourcesVolume();
        }
    }

    public static float GlobalEffectsVolume
    {
        get => _instance._globalMusicVolume;
        set
        {
            _instance._globalMusicVolume = Mathf.Clamp01(value);
            _instance.UpdateSourcesVolume();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        PlaySounds();

        YG2.onOpenAnyAdv += MuteAll;
        YG2.onCloseAnyAdv += UnmuteAll;
    }

    private void Update()
    {
        _buttonScanTimer -= Time.unscaledDeltaTime;

        if (_buttonScanTimer > 0f)
            return;

        _buttonScanTimer = ButtonScanInterval;
        RegisterButtonClickSounds();
    }

    private void OnDestroy()
    {
        if (_instance != this)
            return;

        YG2.onOpenAnyAdv -= MuteAll;
        YG2.onCloseAnyAdv -= UnmuteAll;
        _instance = null;
    }

    public static void Play(string soundName)
        => _instance.PlayBySoundName(soundName);

    public static void PlayOnce(string soundName)
        => _instance.PlayOnceBySoundName(soundName);

    public static void Stop(string soundName)
        => _instance.StopBySoundName(soundName);

    private void PlayBySoundName(string soundName)
    {
        Sound sound = GetSound(soundName);

        if (sound == null)
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
            return;
        }

        if (sound.Source == null)
        {
            InitializeSound(sound);
        }

        sound.AlreadyPlaying = true;
        sound.Source.Stop();
        sound.Source.Play();
    }
    private void PlayOnceBySoundName(string soundName)
    {
        Sound sound = GetSound(soundName);

        if (sound == null)
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
            return;
        }

        if (sound.AlreadyPlaying)
        {
            return;
        }

        if (sound.Source == null)
        {
            InitializeSound(sound);
        }

        sound.AlreadyPlaying = true;
        sound.Source.Stop();
        sound.Source.Play();
    }

    private void StopBySoundName(string soundName)
    {
        Sound sound = GetSound(soundName);

        if (sound == null)
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
            return;
        }

        sound.Source.Stop();
        sound.AlreadyPlaying = false;
    }

    private void UpdateSourcesVolume()
    {
        foreach (var item in _sounds)
        {
            if (item.Source == null)
                continue;

            if (item.AudioType == AudioType.Music)
                item.Source.volume = item.Volume * _globalMusicVolume;
            else
                item.Source.volume = item.Volume * _globalEffectsVolume;
        }
    }

    private void PlaySounds()
    {
        foreach (Sound sound in _sounds)
        {
            if (sound.PlayOnAwake && !sound.AlreadyPlaying)
                Play(sound.Name);
        }
    }

    private void RegisterButtonClickSounds()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var button in buttons)
        {
            if (button == null || _buttonsWithClickSound.Contains(button))
                continue;

            button.onClick.AddListener(PlayButtonClick);
            _buttonsWithClickSound.Add(button);
        }
    }

    private void PlayButtonClick()
    {
        Play(ButtonClickSoundName);
    }

    private void InitializeSound(Sound sound)
    {
        if (sound.DontDestroyOnLoad)
            sound.Source = gameObject.AddComponent<AudioSource>();
        else
            sound.Source = new GameObject(name = $"{sound.Name}").AddComponent<AudioSource>();

        if (sound.AudioType == AudioType.Music)
            sound.Source.volume = sound.Volume * GlobalMusicVolume;
        else
            sound.Source.volume = sound.Volume * GlobalEffectsVolume;

        sound.Source.clip = sound.Clip;
        sound.Source.loop = sound.Loop;
        sound.AlreadyPlaying = true;
    }

    private Sound GetSound(string name)
        => _sounds.Find(s => s.Name == name);

    private void UnmuteAll()
    {
        if (_isMute == false)
            return;

        _isMute = false;
        GlobalMusicVolume = _globalMusicVolumeBeforeMute;
        GlobalEffectsVolume = _globalEffectsVolumeBeforeMute;
    }

    private void MuteAll()
    {
        if (_isMute)
            return;

        _isMute = true;
        _globalMusicVolumeBeforeMute = _globalMusicVolume;
        _globalEffectsVolumeBeforeMute = _globalEffectsVolume;

        GlobalMusicVolume = 0;
        GlobalEffectsVolume = 0;
    }

    [Serializable]
    public class Sound
    {
        public string Name;
        public bool Loop = false;
        public bool PlayOnAwake = false;
        public bool DontDestroyOnLoad = false;
        public AudioType AudioType;

        public AudioClip Clip;
        [Range(0, 1)] public float Volume = 1f;

        [HideInInspector] public bool AlreadyPlaying;
        [HideInInspector] public AudioSource Source;
    }

    public enum AudioType
    {
        Effects,
        Music
    }
}
