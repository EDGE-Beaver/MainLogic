using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip bgmClip; // 인스펙터에서 지정할 BGM
    private AudioSource audioSource;

    void Awake()
    {
        // 오디오 소스 컴포넌트 확인 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BGM 설정
        audioSource.clip = bgmClip;
        audioSource.loop = true; // 반복 재생 활성화
        audioSource.playOnAwake = true; // 게임 시작 시 자동 재생

        // BGM 재생
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (!audioSource.isPlaying && bgmClip != null)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}
