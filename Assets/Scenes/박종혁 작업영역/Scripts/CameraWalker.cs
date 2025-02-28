using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//카메라 이동 -> UI 이동으로 변경, @@@이 붙어있는 주석은 수정된 부분
public class CameraWalker : MonoBehaviour
{
    /*인스펙터 변수 기본 세팅
     * 카메라 이동 속도 : 5, 5
     * 페이드 아웃 시간 : 1
     * */
    private void Start()
    {
        //===== 1. 카메라 이동 효과 =====
        //카메라 이동 목적지 받아오기@@@
        titlePos_Scale = camPos_Title.transform.localScale;
        lobbyPos_Scale = camPos_MainLobby.transform.localScale;
        bookPos_Scale = camPos_Book.transform.localScale;

        titlePos = new Vector3(camPos_Title.transform.localPosition.x / -titlePos_Scale.x, camPos_Title.transform.localPosition.y / -titlePos_Scale.y, UI.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x / -lobbyPos_Scale.x, camPos_MainLobby.transform.localPosition.y / -lobbyPos_Scale.y, UI.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x / -bookPos_Scale.x, camPos_Book.transform.localPosition.y / -bookPos_Scale.y, UI.transform.localPosition.z);

        //(디버그)좌표 확인용
        //Debug.Log(titlePos + ", " + lobbyPos + ", " + bookPos);
        //Debug.Log(titlePos_Scale + ", " + lobbyPos_Scale + ", " + bookPos_Scale);
        //Debug.Log(UI.transform.localPosition);

        //카메라 이동 목적지 기본값@@@
        targetPos = UI.transform.localPosition;
        targetScale = UI.transform.localScale;

        //===== 2. 페이드 효과 =====
        //시작할때 페이드 연출 & 카메라 효과
        can = fadeoutBlack.GetComponent<CanvasGroup>();
        canq = fadeoutBlack_QUIT.GetComponent<CanvasGroup>();
        fadeoutBlack_QUIT.SetActive(false);
        TitleStart();

        //===== 4. 게임 종료 & 메인 메뉴 경고창 =====
        WarnBox_Q.SetActive(false);
        WarnBox_QtextM.SetActive(false);
        WarnBox_QtextQ.SetActive(false);
    }

    private void Update()
    {
        //===== 1. 카메라 이동 효과 =====
        //카메라 이동@@@
        UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, targetScale, zoomSpeed * Time.deltaTime);

        //Lerp 무한지속 방지턱(딱히 의미 없어짐)@@@(이동속도 방지턱은 제거함)
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

    //인스펙터 지정@@@(카메라 빼고 UI 추가)
    [Header("카메라(카메라 대신 이동할 UI)")]
    public GameObject UI;

    [Header("카메라 이동 목적지")]
    public GameObject camPos_Title;
    public GameObject camPos_MainLobby;
    public GameObject camPos_Book;

    [Header("카메라 이동 속도(평면 이동, 확대/축소)")]
    public float camSpeed;
    public float zoomSpeed;

    //카메라 이동 목적지 좌표@@@(위치별 Scale 추가)
    private Vector3 titlePos_Scale;
    private Vector3 lobbyPos_Scale;
    private Vector3 bookPos_Scale;

    private Vector3 titlePos;
    private Vector3 lobbyPos;
    private Vector3 bookPos;

    //카메라 이동 목적지@@@(targetSize 제거, targetScale로 대체)
    private Vector3 targetPos;
    private Vector3 targetScale;

    //현재 어떤 메뉴에 있는지 확인
    private bool inTitle;
    private bool inLobby;
    private bool inBook;

    //카메라 이동 목적지 지정@@@(각 함수마다 targetSize -> targetScale 한 줄씩만 수정)
    public void ToTitleCam()
    {
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
        camSpeed = 5f;
        targetPos = lobbyPos;
        targetScale = new Vector3(3.2f / lobbyPos_Scale.x, 3.2f / lobbyPos_Scale.y, 3.2f / lobbyPos_Scale.z);
        inTitle = false;
        inLobby = true;
        inBook = false;
    }

    public void ToBookCam()
    {
        camSpeed = 5f;
        targetPos = bookPos;
        targetScale = new Vector3(3.2f / bookPos_Scale.x, 3.2f / bookPos_Scale.y, 3.2f / bookPos_Scale.z);
        inTitle = false;
        inLobby = false;
        inBook = true;
    }

    //게임 켰을때 타이틀 카메라 이동@@@(이동속도 Lerp 무한지속 방지턱용 float인 speedCheck1, 2 제거)
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

    //인스펙터 지정
    [Header("페이드 아웃용 검정색 판떼기")]
    public GameObject fadeoutBlack;
    public GameObject fadeoutBlack_QUIT;

    [Header("페이드 아웃 시간")]
    public float fadeTime;

    //효과 구현을 위한 CanvasGroup 지정
    private CanvasGroup can;
    private CanvasGroup canq;

    //페이드 인
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
    IEnumerator FadeInEffect_Title() //게임 처음 시작할때용 페이드인
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

    //페이드 아웃
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

    [Header("===== 3. 타이틀 메뉴별 화면 전환 =====")]

    //인스펙터 지정
    [Header("책 페이지(표지 포함)")]
    public GameObject book_whole;
    public GameObject book_title;
    public GameObject book_load;
    public GameObject book_credit;
    public GameObject book_setting;

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
    public void ToSetting()
    {
        book_whole.SetActive(true);
        ToBookCam();
        book_title.SetActive(false);
        book_load.SetActive(false);
        book_credit.SetActive(false);
        book_setting.SetActive(true);
    }
    public void ToTitle()
    {
        ToTitleCam();
        book_title.SetActive(true);
        book_load.SetActive(false);
        book_credit.SetActive(false);
        book_setting.SetActive(false);
        book_whole.SetActive(false);
    }

    [Header("===== 4. 게임 종료 & 메인 메뉴 경고창 =====")]

    //인스펙터 지정
    [Header("경고창")]
    public GameObject WarnBox_Q;
    [Header("메인 메뉴 텍스트")]
    public GameObject WarnBox_QtextM;
    [Header("게임 종료 텍스트")]
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
