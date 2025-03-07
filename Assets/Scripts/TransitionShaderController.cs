using UnityEngine;





public class TransitionShaderController : MonoBehaviour
{
    public Material transitionMaterial; // 적용할 Shader Material
    public string lerpPropertyName = "_Lerp"; // Shader의 Lerp 변수 이름
    public float transitionDuration = 1.5f; // 트랜지션 지속 시간 (초)
    private float elapsedTime = 0f; // 경과 시간
    private float startValue = 0f; // 초기 Lerp 값
    private float targetValue = 1f; // 목표 Lerp 값
    private bool isTransitioning = false; // 트랜지션 실행 여부
    private bool isFadingOut = false; // 페이드 아웃 여부

    void Start()
    {
        // // 게임 시작 시 자동으로 화면 열리는 효과 페이드 아웃
        // StartFadeOut();
    }

    void Update()
    {
        //테스트용이겠지?
        // 1번 키를 누르면 페이드 인 (화면 닫힘)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartFadeIn();
        }

        // 2번 키를 누르면 페이드 아웃 (화면 열림)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartFadeOut();
        }

        if (isTransitioning)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // 🔻 페이드 아웃 (0 → 1): 원이 커졌다가 줄어든 후 확 커지며 사라짐
            if (isFadingOut)
            {
                float easeOut = Mathf.SmoothStep(0, 1, t); // 부드러운 증가
                float overshoot = 1.1f - 0.1f * Mathf.Cos(t * Mathf.PI * 2f); // 처음 커졌다가 다시 줄어듦
                float elastic = Mathf.Lerp(startValue, targetValue, easeOut) * overshoot; // 탄력 적용

                transitionMaterial.SetFloat(lerpPropertyName, Mathf.Clamp01(elastic));
            }
            // 🔻 페이드 인 (1 → 0): 원이 바깥쪽에서 줄어들다가 살짝 커지고 다시 닫히며 검정이 됨
            else
            {
                float easeIn = 1 - Mathf.Pow(1 - t, 3); // 부드러운 감속 (Ease-In Cubic)
                float bounce = 0.08f * Mathf.Sin(t * Mathf.PI * 3f) * (1 - t); // 점점 약해지는 반동 효과
                float elastic = Mathf.Lerp(startValue, targetValue, easeIn) - bounce; // 원이 확 줄어들다가 살짝 커지고 다시 닫힘

                transitionMaterial.SetFloat(lerpPropertyName, Mathf.Clamp01(elastic));
            }

            // 트랜지션 종료 조건
            if (elapsedTime >= transitionDuration)
            {
                isTransitioning = false;
                transitionMaterial.SetFloat(lerpPropertyName, targetValue);
                Debug.Log("Transition Complete!");
            }
        }
    }

    //  페이드 인 (1 → 0) - 화면이 바깥에서부터 닫히며 완전히 검정됨
    public void StartFadeIn()
    {
        if (!isTransitioning)
        {
            startValue = 1f; // 시작 상태: 투명 (보이는 상태)
            targetValue = 0f; // 최종 상태: 검정 (닫힘)
            elapsedTime = 0f;
            isTransitioning = true;
            isFadingOut = false; // 페이드 인 모드
        }
    }

    //  페이드 아웃 (0 → 1) - 화면이 열리면서 원이 커졌다가 줄어들고 마지막에 확 커짐
    public void StartFadeOut()
    {
        if (!isTransitioning)
        {
            startValue = 0f; // 시작 상태: 검정 (닫힘)
            targetValue = 1f; // 최종 상태: 투명 (열림)
            elapsedTime = 0f;
            isTransitioning = true;
            isFadingOut = true; // 페이드 아웃 모드
        }
    }
}
