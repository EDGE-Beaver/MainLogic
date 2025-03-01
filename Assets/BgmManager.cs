using UnityEngine;
using UnityEngine.UI;

public class BGMManager : MonoBehaviour
{
    public AudioClip bgmClip; 
    private AudioSource audioSource;

    public Slider volumeSlider; // 볼륨 조절을 위한 슬라이더 (인스펙터에서 설정)

    void Awake()
    {
        // 오디오 소스가 있는지 확인하고 없으면 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 오디오 소스 기본 설정
        audioSource.clip = bgmClip;
        audioSource.loop = true; // 반복 재생
        audioSource.playOnAwake = true; // 게임 시작 시 자동 재생

        // 슬라이더 값으로 초기 볼륨 설정 (슬라이더가 없으면 기본값 1)
        audioSource.volume = volumeSlider ? volumeSlider.value : 1f;

        // BGM 재생
        PlayBGM();

        // 슬라이더 값 변경 시 볼륨 조정
        if (volumeSlider)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // BGM 재생
    public void PlayBGM()
    {
        if (!audioSource.isPlaying && bgmClip != null)
        {
            audioSource.Play();
        }
    }

    // BGM 정지
    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // 슬라이더 값에 따라 볼륨 조절
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    private void OnDestroy()
    {
        // 씬 변경 또는 오브젝트 제거 시 이벤트 리스너 해제 (메모리 누수 방지임)
        if (volumeSlider)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }
}
