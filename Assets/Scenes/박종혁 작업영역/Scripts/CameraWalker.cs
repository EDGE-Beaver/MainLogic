using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//ī�޶� �̵� -> UI �̵����� ����, @@@�� �پ��ִ� �ּ��� ������ �κ�
public class CameraWalker : MonoBehaviour
{
    /*�ν����� ���� �⺻ ����
     * ī�޶� �̵� �ӵ� : 5, 5
     * ���̵� �ƿ� �ð� : 1
     * */
    private void Start()
    {
        //===== ī�޶� �̵� ȿ�� =====
        //ī�޶� �̵� ������ �޾ƿ���@@@
        titlePos_Scale = camPos_Title.transform.localScale;
        lobbyPos_Scale = camPos_MainLobby.transform.localScale;
        bookPos_Scale = camPos_Book.transform.localScale;

        titlePos = new Vector3(camPos_Title.transform.localPosition.x / -titlePos_Scale.x, camPos_Title.transform.localPosition.y / -titlePos_Scale.y, UI.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x / -lobbyPos_Scale.x, camPos_MainLobby.transform.localPosition.y / -lobbyPos_Scale.y, UI.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x / -bookPos_Scale.x, camPos_Book.transform.localPosition.y / -bookPos_Scale.y, UI.transform.localPosition.z);

        //(�����)��ǥ Ȯ�ο�
        //Debug.Log(titlePos + ", " + lobbyPos + ", " + bookPos);
        //Debug.Log(titlePos_Scale + ", " + lobbyPos_Scale + ", " + bookPos_Scale);
        //Debug.Log(UI.transform.localPosition);

        //ī�޶� �̵� ������ �⺻��@@@
        targetPos = UI.transform.localPosition;
        targetScale = UI.transform.localScale;

        //ó���� Ÿ��Ʋ���� ����
        inTitle = true;
        inLobby = false;
        inBook = false;

        //===== �����Ҷ� ���̵� ���� & ī�޶� ȿ�� =====
        can = fadeoutBlack.GetComponent<CanvasGroup>();
        canq = fadeoutBlack_QUIT.GetComponent<CanvasGroup>();
        fadeoutBlack_QUIT.SetActive(false);
        TitleStart();
    }

    private void Update()
    {
        //===== ī�޶� �̵� ȿ�� =====
        //ī�޶� �̵�@@@
        UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, targetScale, zoomSpeed * Time.deltaTime);

        //Lerp �������� ������(���� �ǹ� ������)@@@(UI ������ ���� �����ϴ� �������� ������ ���ӵ��� �ʴ� ������ ���� & �����ϱ� ���ŷο� �̽��� �̵��ӵ� �������� ������)
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

        //Esc ������ Ÿ��Ʋ�� Ż��
        if (!inTitle && Input.GetKeyDown(KeyCode.Escape))
        {
            ToTitle();
        }
    }

    [Header("===== 1. ī�޶� �̵� ȿ�� =====")]

    //�ν����� ����@@@(ī�޶� ���� UI �߰�)
    [Header("ī�޶�(ī�޶� ��� �̵��� UI)")]
    public GameObject UI;

    [Header("ī�޶� �̵� ������")]
    public GameObject camPos_Title;
    public GameObject camPos_MainLobby;
    public GameObject camPos_Book;

    [Header("ī�޶� �̵� �ӵ�(��� �̵�, Ȯ��/���)")]
    public float camSpeed;
    public float zoomSpeed;

    //ī�޶� �̵� ������ ��ǥ@@@(��ġ�� Scale �߰�)
    private Vector3 titlePos_Scale;
    private Vector3 lobbyPos_Scale;
    private Vector3 bookPos_Scale;

    private Vector3 titlePos;
    private Vector3 lobbyPos;
    private Vector3 bookPos;

    //ī�޶� �̵� ������@@@(targetSize ����, targetScale�� ��ü)
    private Vector3 targetPos;
    private Vector3 targetScale;

    //���� � �޴��� �ִ��� Ȯ��
    private bool inTitle;
    private bool inLobby;
    private bool inBook;

    //ī�޶� �̵� ������ ����@@@(�� �Լ����� targetSize -> targetScale �� �پ��� ����)
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

    //���� ������ Ÿ��Ʋ ī�޶� �̵�@@@(�̵��ӵ� Lerp �������� �����ο� float�� speedCheck1, 2 ����)
    private float alphaCheck1 = 0f, alphaCheck2 = 0f;
    private bool titleIntro = false;
    public void TitleStart()
    {
        UI.transform.localPosition = new Vector3(titlePos.x, titlePos.y + 150f, titlePos.z);
        titleIntro = true;
        ToTitleCam();
    }

    [Header("===== 2. ���̵� �ƿ� ���� =====")]

    //�ν����� ����
    [Header("���̵� �ƿ��� ������ �Ƕ���")]
    public GameObject fadeoutBlack;
    public GameObject fadeoutBlack_QUIT;

    [Header("���̵� �ƿ� �ð�")]
    public float fadeTime;

    //ȿ�� ������ ���� CanvasGroup ����
    private CanvasGroup can;
    private CanvasGroup canq;

    //���̵� ��
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
    IEnumerator FadeInEffect() //������@@@@@@@@@@@@@@@@@@@@
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
    IEnumerator FadeInEffect_Title() //���� ó�� �����Ҷ��� ���̵���
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

    //���̵� �ƿ�
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
    IEnumerator FadeOutEffect() //������@@@@@@@@@@@@@@@@@@@@@
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
    IEnumerator FadeOutEffect_Quit() //���� ����� ���̵�ƿ�
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

    //���� ����
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

    [Header("===== 3. Ÿ��Ʋ �޴��� ȭ�� ��ȯ =====")]

    //�ν����� ����
    [Header("å ������(ǥ�� ����)")]
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

    /*[Header("===== 4. Ÿ��Ʋ �޴��� ȭ�� ��ȯ =====")]

    public GameObject selectedPhoto;
    
    public void ShowPhoto(GameObject sel)
    {

    }*/
}
