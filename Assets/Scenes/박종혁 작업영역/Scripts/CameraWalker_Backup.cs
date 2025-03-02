using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//카메라 이동 -> UI 이동으로 변경, @@@이 붙어있는 주석은 수정된 부분
public class CameraWalker_Backup : MonoBehaviour
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

        targetPos = UI.transform.localPosition;
        targetScale = UI.transform.localScale;

        inTitle = true;
        inLobby = false;
        inBook = false;

        can = fadeoutBlack.GetComponent<CanvasGroup>();
        canq = fadeoutBlack_QUIT.GetComponent<CanvasGroup>();
        fadeoutBlack_QUIT.SetActive(false);
        TitleStart();
    }

    private void Update()
    {
        //===== 카메라 이동 효과 =====
        UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, targetScale, zoomSpeed * Time.deltaTime);

        //Esc 누르면 타이틀로 탈출@@@
        if (!inTitle && Input.GetKeyDown(KeyCode.Escape))
        {
            ToTitle();
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

    private bool titleIntro = false;
    public void TitleStart()
    {
        UI.transform.localPosition = new Vector3(titlePos.x, titlePos.y + 150f, titlePos.z);
        titleIntro = true;
        ToTitleCam();
    }

    [Header("===== 2. 페이드 아웃 연출 =====")]
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
    private void FadeOut()
    {
        StartCoroutine(FadeOutEffect());
    }
    private IEnumerator FadeOutEffect()
    {
        fadeoutBlack.SetActive(true);
        can = fadeoutBlack.GetComponent<CanvasGroup>();
        can.alpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            can.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        can.alpha = 1f;
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
        can.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            can.alpha = Mathf.Lerp(can.alpha, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeoutBlack.SetActive(false);
    }

    IEnumerator FadeInEffect_Title()
    {
        fadeoutBlack.SetActive(true);
        can.alpha = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            can.alpha = Mathf.Lerp(can.alpha, 0f, elapsedTime / fadeTime);
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
}
