using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip bgmClip; // �ν����Ϳ��� ������ BGM
    private AudioSource audioSource;

    void Awake()
    {
        // ����� �ҽ� ������Ʈ Ȯ�� �Ǵ� �߰�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BGM ����
        audioSource.clip = bgmClip;
        audioSource.loop = true; // �ݺ� ��� Ȱ��ȭ
        audioSource.playOnAwake = true; // ���� ���� �� �ڵ� ���

        // BGM ���
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
