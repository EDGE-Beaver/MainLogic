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
    public float minSpeedFactor = 0.3f; // 🎯 최소 속도 비율 (기본 30%)
    public float maxSpeedFactor = 1.5f; // 🎯 최대 속도 비율 (기본 150%)
    public float speedChangeDuration = 2f; // 🎯 속도 변화 지속 시간 (기본 2초)
    public float speedChangeIntervalMin = 3f; // 🎯 속도 변경 최소 대기 시간 (기본 3초)
    public float speedChangeIntervalMax = 6f; // 🎯 속도 변경 최대 대기 시간 (기본 6초)

    private Vector3 originalPosition;
    private float currentSpeed;
    private bool isChangingSpeed = false; // 🎯 속도 변경 중인지 확인
    private bool isBreathingActive = true; // 🎯 현재 숨쉬기 효과가 활성화되었는지 확인

    void Start()
    {
        if (targetObject == null)
        {
            targetObject = transform;
        }

        originalPosition = targetObject.localPosition;
        currentSpeed = baseSpeed;
        StartCoroutine(SpeedVariationCycle());
    }

    void Update()
    {
        if (targetObject == null || !isBreathingActive) return; // 🎯 숨쉬기 비활성화 시 중단

        float newY = originalPosition.y + (Mathf.Sin(Time.time * currentSpeed) * floatStrength);
        targetObject.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }

    IEnumerator SpeedVariationCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(speedChangeIntervalMin, speedChangeIntervalMax));

            if (!isChangingSpeed && isBreathingActive)
            {
                float targetSpeedFactor = Random.Range(minSpeedFactor, maxSpeedFactor);
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

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentSpeed = Mathf.Lerp(targetSpeed, baseSpeed, elapsedTime / duration);
            yield return null;
        }

        isChangingSpeed = false;
    }

    // 🎯 숨쉬기 효과 멈추기 ( `()` 을 만나면 실행 )
    public void StopBreathing()
    {
        isBreathingActive = false;
    }

    // 🎯 숨쉬기 효과 다시 시작 ( `(!)` 을 만나면 실행 )
    public void StartBreathing()
    {
        isBreathingActive = true;
    }
}
