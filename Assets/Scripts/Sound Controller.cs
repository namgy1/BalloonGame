using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    [Header("Slider References")]
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Audio Sources")]
    public AudioSource musicSource, musicSource2;  // StartScene, PlayScene BGM용
    public AudioSource soundSource;  // 효과음 재생용
    
    [Header("Audio Clips")]
    public List<AudioClip> soundClips;  // 효과음 목록
    public List<BGMClip> bgmClips;      // 씬 별 BGM 목록

    private Dictionary<string, AudioClip> soundDictionary;
    private Dictionary<string, AudioClip> bgmDictionary;

    [System.Serializable]
    public class BGMClip
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    private void InitializeSliders()
    {
        // 현재 씬에서 Slider 찾기
        FindSlidersInScene();

        // 기존 슬라이더 값 초기화
        if (musicSlider != null)
        {
            musicSlider.value = musicSource.volume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (soundSlider != null)
        {
            soundSlider.value = soundSource.volume;
            soundSlider.onValueChanged.AddListener(SetSoundVolume);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    

    private void Start()
    {
        // 효과음 및 BGM 초기화
        InitializeAudioDictionaries();

        // 슬라이더 초기화
        InitializeSliders();

        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬 BGM 재생
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 씬이 로드될 때 호출 (BGM 변경 및 슬라이더 동기화)
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬별 BGM 재생
        PlayBGMForScene(scene.name);

        // 새 씬에서 슬라이더 찾기 및 동기화
        FindSlidersInScene();
    }

    /// <summary>
    /// 씬에서 Slider 찾아 동기화
    /// </summary>
    private void FindSlidersInScene()
    {
        // 씬 내 모든 Slider 탐색
        Slider[] sliders = FindObjectsOfType<Slider>();

        foreach (Slider slider in sliders)
        {
            if (slider.CompareTag("MusicSlider"))  // 배경음악 슬라이더
            {
                musicSlider = slider;
                musicSlider.value = musicSource.volume;
                musicSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            else if (slider.CompareTag("SoundSlider"))  // 효과음 슬라이더
            {
                soundSlider = slider;
                soundSlider.value = soundSource.volume;
                soundSlider.onValueChanged.AddListener(SetSoundVolume);
            }
        }
    }

    /// <summary>
    /// 오디오 클립 초기화
    /// </summary>
    private void InitializeAudioDictionaries()
    {
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in soundClips)
        {
            soundDictionary[clip.name] = clip;
        }

        bgmDictionary = new Dictionary<string, AudioClip>();
        foreach (var bgm in bgmClips)
        {
            bgmDictionary[bgm.sceneName] = bgm.bgmClip;
        }
    }

    /// <summary>
    /// 씬 이름에 맞는 BGM 재생
    /// </summary>
    public void PlayBGMForScene(string sceneName)
{
    // 씬 전환 전에 모든 BGM을 중지
    StopAllBGM();

    if (bgmDictionary.ContainsKey(sceneName))
    {
        AudioClip bgmClip = bgmDictionary[sceneName];

        if (sceneName == "Start Scene")
        {
            PlayBGM(musicSource, bgmClip);  // StartScene에서 BGM 재생
        }
        else if (sceneName == "Play Scene")
        {
            PlayBGM(musicSource2, bgmClip);  // PlayScene에서 BGM 재생
        }
    }
}

/// <summary>
/// 모든 BGM 중지
/// </summary>


    /// <summary>
    /// 특정 AudioSource로 BGM 재생
    /// </summary>
    public void PlayBGM(AudioSource source, AudioClip bgmClip)
    {
        if (source.clip == bgmClip && source.isPlaying)
            return;

        source.clip = bgmClip;
        source.loop = true;
        source.Play();
    }

    /// <summary>
    /// 특정 AudioSource의 BGM 정지
    /// </summary>
    public void StopBGM(AudioSource source)
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
    }
    public void StopAllBGM()
    {
        StopBGM(musicSource);
        StopBGM(musicSource2);
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    public void PlaySoundEffect(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundSource.PlayOneShot(soundDictionary[soundName]);
        }
    }

    /// <summary>
    /// 배경음악 볼륨 설정
    /// </summary>
    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }
        if (musicSource2 != null)
        {
            musicSource2.volume = value;
        }
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void SetSoundVolume(float value)
    {
        if (soundSource != null)
        {
            soundSource.volume = value;
        }
    }
}
