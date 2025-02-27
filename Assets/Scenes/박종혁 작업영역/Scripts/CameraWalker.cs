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
        //===== 카메라 이동 효과 =====
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

        //처음엔 타이틀에서 시작
        inTitle = true;
        inLobby = false;
        inBook = false;

        //===== 시작할때 페이드 연출 & 카메라 효과 =====
        can = fadeoutBlack.GetComponent<CanvasGroup>();
        canq = fadeoutBlack_QUIT.GetComponent<CanvasGroup>();
        fadeoutBlack_QUIT.SetActive(false);
        TitleStart();
    }

    private void Update()
    {
        //===== 카메라 이동 효과 =====
        //카메라 이동@@@
        UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, targetScale, zoomSpeed * Time.deltaTime);

        //Lerp 무한지속 방지턱(딱히 의미 없어짐)@@@(UI 버전은 값이 수렴하는 형식으로 무한히 지속되지 않는 것으로 보임 & 구현하기 번거로움 이슈로 이동속도 방지턱은 제거함)
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
            ToTitle();
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
        UI.transform.localPosition = new Vector3(titlePos.x, titlePos.y + 150f, titlePos.z);
        titleIntro = true;
        ToTitleCam();
    }

    [Header("===== 2. 페이드 아웃 연출 =====")]

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
    IEnumerator FadeInEffect() //공사중@@@@@@@@@@@@@@@@@@@@
    {
        fadeoutBlack.SetActive(true);
        can.interactable = false;
        can.blocksRaycasts = false;
        can.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime * 500f)
        {
            can.alpha = Mathf.Lerp(can.alpha, 0f, elapsedTime / (fadeTime * 500f));
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
        while (elapsedTime < (fadeTime * 1000f))
        {
            can.alpha = Mathf.Lerp(can.alpha, 0f, elapsedTime / (fadeTime * 1000f));
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
    IEnumerator FadeOutEffect() //공사중@@@@@@@@@@@@@@@@@@@@@
    {
        fadeoutBlack.SetActive(true);
        can.interactable = true;
        can.blocksRaycasts = true;
        can.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            can.alpha = Mathf.Lerp(can.alpha, 1f, elapsedTime / fadeTime * 100);
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
        while (elapsedTime < fadeTime / 4f)
        {
            canq.alpha = Mathf.MoveTowards(canq.alpha, 1f, elapsedTime / (fadeTime / 4f));
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
        await Task.Delay(500);
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

    /*[Header("===== 4. 타이틀 메뉴별 화면 전환 =====")]

    public GameObject selectedPhoto;
    
    public void ShowPhoto(GameObject sel)
    {

    }*/
}
