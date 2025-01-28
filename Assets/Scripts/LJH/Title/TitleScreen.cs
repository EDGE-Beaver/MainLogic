using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class TitleScreen : MonoBehaviour
{
    public string nextSceneName = "Main"; // 넘어갈 씬 이름

    private bool isKeyPressed = false; // 키 입력 확인 플래그

    void Update()
    {
        // 아무 키나 눌렸을 때
        if (Input.anyKeyDown && !isKeyPressed)
        {
            isKeyPressed = true; // 중복 입력 방지
            StartGame(); // 게임 시작
        }
    }

    void StartGame()
    {
        Debug.Log("굿");

        // 다음 씬으로 이동
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("씬 이름 수정했냐?");
        }
    }
}
