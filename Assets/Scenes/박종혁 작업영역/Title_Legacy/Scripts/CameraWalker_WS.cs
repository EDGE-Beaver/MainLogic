/*
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraWalker : MonoBehaviour
{
    //인스펙터 변수 기본 세팅
    //카메라 이동 속도 : 5, 5
    //페이드 아웃 시간 : 1

    private void Start()
    {
        //===== 카메라 이동 효과 =====
        //카메라 이동 목적지 받아오기
        titlePos = new Vector3(camPos_Title.transform.localPosition.x, camPos_Title.transform.localPosition.y, cam.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x, camPos_MainLobby.transform.localPosition.y, cam.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x, camPos_Book.transform.localPosition.y, cam.transform.localPosition.z);

        //(디버그)좌표 확인용
        //Debug.Log(titlePos + ", " + lobbyPos + ", " + bookPos);

        //카메라 이동 목적지 기본값
        targetPos = cam.transform.localPosition;
        targetSize = cam.orthographicSize;

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
        //카메라 이동
        cam.transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        //Lerp 무한지속 방지턱(딱히 의미 없어짐)
        speedCheck1 = cam.velocity.sqrMagnitude;
        if (cam.velocity.sqrMagnitude <= 0.00001 && speedCheck1 < speedCheck2)
        {
            cam.transform.localPosition = targetPos;
        }
        speedCheck2 = speedCheck1;

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

    //인스펙터 지정
    [Header("카메라")]
    public Camera cam;

    [Header("카메라 이동 목적지")]
    public GameObject camPos_Title;
    public GameObject camPos_MainLobby;
    public GameObject camPos_Book;

    [Header("카메라 이동 속도(평면 이동, 확대/축소)")]
    public float camSpeed;
    public float zoomSpeed;

    //카메라 이동 목적지 좌표
    private Vector3 titlePos;
    private Vector3 lobbyPos;
    private Vector3 bookPos;

    //카메라 이동 목적지
    private Vector3 targetPos;
    private float targetSize;

    //현재 어떤 메뉴에 있는지 확인
    private bool inTitle;
    private bool inLobby;
    private bool inBook;

    //카메라 이동 목적지 지정
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
        targetSize = 5f;
        inTitle = true;
        inLobby = false;
        inBook = false;
    }

    public void ToLobbyCam()
    {
        camSpeed = 5f;
        targetPos = lobbyPos;
        targetSize = 16f;
        inTitle = false;
        inLobby = true;
        inBook = false;
    }

    public void ToBookCam()
    {
        camSpeed = 5f;
        targetPos = bookPos;
        targetSize = 16f;
        inTitle = false;
        inLobby = false;
        inBook = true;
    }

    //게임 켰을때 타이틀 카메라 이동
    private float speedCheck1 = 0f, speedCheck2 = 0f;
    private float alphaCheck1 = 0f, alphaCheck2 = 0f;
    private bool titleIntro = false;
    public void TitleStart()
    {
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y - 150f, cam.transform.localPosition.z);
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
    IEnumerator FadeInEffect() //여기 고쳐야됨@@@@@@@@@@@@@@@@@@@@@
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
    IEnumerator FadeOutEffect() //여기 고쳐야됨@@@@@@@@@@@@@@@@@@@@@
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
}
*/