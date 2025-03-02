using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//카메라 이동 -> UI 이동으로 변경
public class CameraWalker : MonoBehaviour
{
    /*인스펙터 변수 기본 세팅
     * 카메라 이동 속도(Cam/Zoom Speed) : 5, 5
     * 페이드 아웃 시간(Fade Time) : 1
     * */
    private void Start()
    {


        //===== 카메라 이동 효과 =====
        //카메라 이동 목적지 받아오기@@@
        titlePos_Scale = camPos_Title.transform.localScale;
        lobbyPos_Scale = camPos_MainLobby.transform.localScale;
        bookPos_Scale = camPos_Book.transform.localScale;

        titlePos = new Vector3(camPos_Title.transform.localPosition.x / -titlePos_Scale.x, camPos_Title.transform.localPosition.y / -titlePos_Scale.y, UI.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x / -lobbyPos_Scale.x, camPos_MainLobby.transform.localPosition.y / -lobbyPos_Scale.y, UI.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x / -bookPos_Scale.x, camPos_Book.transform.localPosition.y / -bookPos_Scale.y, UI.transform.localPosition.z);

        targetPos = UI.transform.localPosition;
        targetScale = UI.transform.localScale;

        //===== 페이드 효과 =====
        can = fadeoutBlack.GetComponent<CanvasGroup>();
        canq = fadeoutBlack_QUIT.GetComponent<CanvasGroup>();
        fadeoutBlack_QUIT.SetActive(false);
        TitleStart();

        //===== 게임 종료 & 메인 메뉴 경고창 =====
        WarnBox_Q.SetActive(false);
        WarnBox_QtextM.SetActive(false);
        WarnBox_QtextQ.SetActive(false);
    }

    private void Update()
    {
        //===== 카메라 이동 효과 =====
        UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, targetScale, zoomSpeed * Time.deltaTime);

        //Lerp 무한지속 방지턱(혹시 모를 오류를 대비해 넣어둠)
        alphaCheck1 = can.alpha;
        if (can.alpha <= 0.01f && alphaCheck1 < alphaCheck2)
        {
            can.alpha = 0f;
        }
        if (can.alpha >= 0.95f && alphaCheck1 > alphaCheck2)
        {
            can.alpha = 1f;
        }
        alphaCheck2 = alphaCheck1;

        //Esc 누르면 타이틀로 탈출
        if (!inTitle && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEscWarning)
            {
                EscWarning_NO();
            }
            else
            {
                if (inLobby)
                {
                    ToBookCam();
                }
                else
                {
                    ToTitle();
                }
            }
        }
    }

    [Header("===== 1. 카메라 이동 효과 =====")]

    [Header("카메라(카메라 대신 이동할 UI)")]
    public GameObject UI;

    [Header("카메라 이동 목적지")]
    public GameObject camPos_Title;
    public GameObject camPos_MainLobby;
    public GameObject camPos_Book;

    [Header("카메라 이동 속도(평면 이동, 확대/축소)")]
    public float camSpeed;
    public float zoomSpeed;

    private Vector3 titlePos_Scale;
    private Vector3 lobbyPos_Scale;
    private Vector3 bookPos_Scale;

    private Vector3 titlePos;
    private Vector3 lobbyPos;
    private Vector3 bookPos;

    private Vector3 targetPos;
    private Vector3 targetScale;

    private bool inTitle;
    private bool inLobby;
    private bool inBook;

    public void ToTitleCam()
    {
        PlaySFX(); //@@@ 효과음 적용
        if (titleIntro)
        {
            camSpeed = 1f;
            FadeIn();
        }
        else
        {
            camSpeed = 5f;
        }
        targetPos = titlePos;
        targetScale = new Vector3(3.2f / titlePos_Scale.x, 3.2f / titlePos_Scale.y, 3.2f / titlePos_Scale.z);
        inTitle = true;
        inLobby = false;
        inBook = false;
    }

    public void ToLobbyCam()
    {
        PlaySFX(); //@@@ 효과음 적용
        camSpeed = 5f;
        targetPos = lobbyPos;
        targetScale = new Vector3(3.2f / lobbyPos_Scale.x, 3.2f / lobbyPos_Scale.y, 3.2f / lobbyPos_Scale.z);
        inTitle = false;
        inLobby = true;
        inBook = false;
    }

    public void ToBookCam()
    {
        PlaySFX(); //@@@ 효과음 적용
        camSpeed = 5f;
        targetPos = bookPos;
        targetScale = new Vector3(3.2f / bookPos_Scale.x, 3.2f / bookPos_Scale.y, 3.2f / bookPos_Scale.z);
        inTitle = false;
        inLobby = false;
        inBook = true;
    }

    private void PlaySFX()
    {
        if (sfxPlayer && menuChangeSFX)
        {
            sfxPlayer.volume = sfxVolumeSlider ? sfxVolumeSlider.value : 1.0f;
            sfxPlayer.PlayOneShot(menuChangeSFX);
        }
    }

    private float alphaCheck1 = 0f, alphaCheck2 = 0f;
    private bool titleIntro = false;

    public void TitleStart()
    {
        inTitle = true;
        inLobby = false;
        inBook = false;

        UI.transform.localPosition = new Vector3(titlePos.x, titlePos.y + 150f, titlePos.z);
        UI.transform.localScale = new Vector3(3.2f / titlePos_Scale.x, 3.2f / titlePos_Scale.y, 3.2f / titlePos_Scale.z);
        titleIntro = true;
        ToTitle();
        ToTitleCam();
    }

    [Header("===== 2. 페이드 효과 =====")]
    public GameObject fadeoutBlack;
    public GameObject fadeoutBlack_QUIT;
    public float fadeTime;

    private CanvasGroup can;
    private CanvasGroup canq;

    [Header("===== 3. 효과음 설정 =====")]
    public AudioSource sfxPlayer;
    public AudioClip menuChangeSFX;
    public Slider sfxVolumeSlider;

    [Header("===== 4. 게임 시작 설정 =====")]
    public string startSceneName = "MainMenu"; //@@@ 인스펙터에서 씬 지정

    public void ToTitle()
    {
        ToTitleCam();
        book_title.SetActive(true);
        book_load.SetActive(false);
        book_credit.SetActive(false);
        book_setting.SetActive(false);
        book_whole.SetActive(false);
    }

    [Header("===== 5. 타이틀 메뉴별 화면 전환 =====")]

    public GameObject book_whole;
    public GameObject book_title;
    public GameObject book_load;
    public GameObject book_credit;
    public GameObject book_setting;

    // 게임 시작 버튼을 눌렀을 때 실행
    public void ToGameStart()
    {
        PlaySFX(); // 효과음 재생
        StartCoroutine(GameStartSequence());
    }
    public void FadeOut()
    {
        if (onGameQuit)
        {
            StopCoroutine(FadeOutEffect_Quit());
            StartCoroutine(FadeOutEffect_Quit());
        }
        else
        {
            onGameQuit = false;
            StopCoroutine(FadeOutEffect());
            StartCoroutine(FadeOutEffect());
        }
    }
    IEnumerator FadeOutEffect()
    {
        fadeoutBlack.SetActive(true);
        can.interactable = true;
        can.blocksRaycasts = true;
        can.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            if (elapsedTime >= fadeTime)
            {
                can.alpha = 1f;
                break;
            }
            can.alpha = Mathf.SmoothStep(0f, 1f, elapsedTime / (fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FadeOutEffect_Quit() //게임 종료용 페이드아웃
    {
        fadeoutBlack_QUIT.SetActive(true);
        canq.interactable = true;
        canq.blocksRaycasts = true;
        canq.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            if (elapsedTime >= fadeTime)
            {
                canq.alpha = 1f;
                break;
            }
            canq.alpha = Mathf.SmoothStep(0f, 1f, elapsedTime / (fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // 씬 변경 전 연출 실행 후 이동
    private IEnumerator GameStartSequence()
    {
        // 페이드 아웃 효과
        FadeOut();

        // 페이드 아웃이 끝날 때까지 대기
        yield return new WaitForSeconds(fadeTime);

        // 씬 이동
        if (!string.IsNullOrEmpty(startSceneName))
        {
            SceneManager.LoadScene(startSceneName);
        }
    }

    //게임 종료
    private bool onGameQuit = false;
    public async void QuitGame()
    {
        StopAllCoroutines();
        onGameQuit = true;
        FadeOut();
        await Task.Delay((int)fadeTime * 1000);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void ToLoad()
    {
        book_whole.SetActive(true);
        ToBookCam();
        book_title.SetActive(false);
        book_load.SetActive(true);
        book_credit.SetActive(false);
        book_setting.SetActive(false);
    }

    public void ToCredit()
    {
        book_whole.SetActive(true);
        ToBookCam();
        book_title.SetActive(false);
        book_load.SetActive(false);
        book_credit.SetActive(true);
        book_setting.SetActive(false);
    }

    public void FadeIn()
    {
        if (titleIntro)
        {
            titleIntro = false;
            StopCoroutine(FadeInEffect_Title());
            StartCoroutine(FadeInEffect_Title());
        }
        else
        {
            StopCoroutine(FadeInEffect());
            StartCoroutine(FadeInEffect());
        }
    }

    IEnumerator FadeInEffect()
    {
        fadeoutBlack.SetActive(true);
        can.interactable = false;
        can.blocksRaycasts = false;
        can.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            if (elapsedTime >= fadeTime)
            {
                can.alpha = 0f;
                break;
            }
            can.alpha = Mathf.SmoothStep(1f, 0f, elapsedTime / (fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeoutBlack.SetActive(false);
    }

    IEnumerator FadeInEffect_Title()
    {
        fadeoutBlack.SetActive(true);
        can.interactable = false;
        can.blocksRaycasts = false;
        can.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime * 4)
        {
            if (elapsedTime >= fadeTime * 4)
            {
                can.alpha = 0f;
                break;
            }
            can.alpha = Mathf.SmoothStep(1f, 0f, elapsedTime / (fadeTime * 4));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeoutBlack.SetActive(false);
    }

    public void ToSetting()
    {
        book_whole.SetActive(true);
        ToBookCam();
        book_title.SetActive(false);
        book_load.SetActive(false);
        book_credit.SetActive(false);
        book_setting.SetActive(true);
    }

    [Header("===== 6. 게임 종료 & 메인 메뉴 경고창 =====")]

    //경고창
    public GameObject WarnBox_Q;
    //메인 메뉴 텍스트
    public GameObject WarnBox_QtextM;
    //게임 종료 텍스트
    public GameObject WarnBox_QtextQ;

    //경고창이 띄워져있는지, 어떤 경고창인지 확인
    private bool isEscWarning = false;
    private bool isEscWarning_Q = false;

    public void Debug_EscWarning_setQtrue()
    {
        isEscWarning_Q = true;
    }

    public void EscWarning()
    {
        WarnBox_Q.SetActive(true);
        isEscWarning = true;
        if (isEscWarning_Q)
        {
            WarnBox_QtextQ.SetActive(true);
        }
        else
        {
            WarnBox_QtextM.SetActive(true);
        }
    }

    public void EscWarning_YES()
    {
        if (isEscWarning_Q)
        {
            EscWarning_NO();
            QuitGame();
        }
        else
        {
            EscWarning_NO();
            TitleStart();
        }
    }

    public void EscWarning_NO()
    {
        isEscWarning = false;
        isEscWarning_Q = false;
        WarnBox_Q.SetActive(false);
        WarnBox_QtextM.SetActive(false);
        WarnBox_QtextQ.SetActive(false);
    }
}
