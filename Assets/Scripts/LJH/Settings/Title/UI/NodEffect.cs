using UnityEngine;
using System.Collections;

public class NodEffect : MonoBehaviour
{
    [Header("끄덕임 설정")]
    public float nodDistance = 10f; // 🎯 아래로 내려가는 거리 (기본 10)
    public float nodDuration = 0.3f; // 🎯 내려가는 시간 (기본 0.3초)

    private Vector3 originalPosition; // 원래 위치

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void StartNod()
    {
        StopAllCoroutines();
        StartCoroutine(NodAnimation());
    }

    IEnumerator NodAnimation()
    {
        Vector3 targetPosition = originalPosition + new Vector3(0, -nodDistance, 0);

        // 🎯 아래로 부드럽게 이동
        float elapsedTime = 0;
        while (elapsedTime < nodDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / nodDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;

        // 🎯 원래 위치로 복귀
        elapsedTime = 0;
        while (elapsedTime < nodDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / nodDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
