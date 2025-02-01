using UnityEngine;
using System.Collections;

public class BreathingFloatEffect : MonoBehaviour
{
    [Header("흔들림 대상")]
    public Transform targetObject; // 🎯 흔들릴 대상 (인스펙터에서 지정 가능)

    [Header("흔들림 설정")]
    public float floatStrength = 0.2f; // 🎯 위아래 이동 거리 (기본 0.2)
    public float baseSpeed = 2f; // 🎯 기본 속도 (기본 2)

    [Header("속도 변화 설정")]
    public float minSpeedFactor = 0.3f; // 🎯 속도가 최소한으로 느려지는 비율 (기본 30%)
    public float maxSpeedFactor = 1.5f; // 🎯 속도가 최대한으로 빨라지는 비율 (기본 150%)
    public float speedChangeDuration = 2f; // 🎯 속도 변화에 걸리는 시간 (기본 2초)
    public float speedChangeIntervalMin = 3f; // 🎯 속도가 변하기까지 최소 대기 시간 (기본 3초)
    public float speedChangeIntervalMax = 6f; // 🎯 속도가 변하기까지 최대 대기 시간 (기본 6초)

    private Vector3 originalPosition;
    private float currentSpeed;
    private bool isChangingSpeed = false; // 🎯 현재 속도를 변화시키는 중인지 확인

    void Start()
    {
        // 🎯 대상이 설정되지 않았다면 자기 자신을 대상으로 설정
        if (targetObject == null)
        {
            targetObject = transform;
        }

        originalPosition = targetObject.localPosition; // 🎯 초기 위치 저장
        currentSpeed = baseSpeed;
        StartCoroutine(SpeedVariationCycle());
    }

    void Update()
    {
        if (targetObject == null) return;

        // 🎯 부드러운 상하 이동 (Mathf.Sin() 사용, 속도 변화 적용)
        float newY = originalPosition.y + (Mathf.Sin(Time.time * currentSpeed) * floatStrength);
        targetObject.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }

    IEnumerator SpeedVariationCycle()
    {
        while (true)
        {
            // 🎯 랜덤한 시간 후 속도를 서서히 변경
            yield return new WaitForSeconds(Random.Range(speedChangeIntervalMin, speedChangeIntervalMax));

            if (!isChangingSpeed)
            {
                float targetSpeedFactor = Random.Range(minSpeedFactor, maxSpeedFactor); // 🎯 최소 ~ 최대 사이 랜덤 속도
                StartCoroutine(AdjustSpeed(targetSpeedFactor, speedChangeDuration));
            }
        }
    }

    IEnumerator AdjustSpeed(float targetSpeedFactor, float duration)
    {
        isChangingSpeed = true;
        float startSpeed = currentSpeed;
        float targetSpeed = baseSpeed * targetSpeedFactor;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / duration);
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(speedChangeIntervalMin, speedChangeIntervalMax));

        // 🎯 원래 속도로 복귀
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentSpeed = Mathf.Lerp(targetSpeed, baseSpeed, elapsedTime / duration);
            yield return null;
        }

        isChangingSpeed = false;
    }
}
