using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraWalker : MonoBehaviour
{
    [Header("===== 카메라 이동 효과 =====")]

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

    private void Start()
    {
        //카메라 이동 목적지 받아오기
        titlePos = new Vector3(camPos_Title.transform.localPosition.x, camPos_Title.transform.localPosition.y, cam.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x, camPos_MainLobby.transform.localPosition.y, cam.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x, camPos_Book.transform.localPosition.y, cam.transform.localPosition.z);

        //좌표 확인용
        Debug.Log(titlePos + ", " + lobbyPos + ", " + bookPos);

        //카메라 이동 목적지 기본값
        targetPos = cam.transform.localPosition;
        targetSize = cam.orthographicSize;

        //처음엔 타이틀에서 시작
        inTitle = true;
        inLobby = false;
        inBook = false;

        //책 기울어진 각도 저장
        targetRotation = bookCover.transform.rotation;
    }

    private void Update()
    {
        //카메라 이동
        cam.transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        //Esc 누르면 타이틀로 탈출
        if(!inTitle && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc 입력");
            ToTitle();
        }

        //책 페이지 회전
        if (isFliping)
        {
            bookCover.transform.rotation = Quaternion.Slerp(bookCover.transform.rotation, targetRotation, flipSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(bookCover.transform.rotation, targetRotation) < 1f)
            {
                isFliping = false;
            }
        }
    }

    //카메라 이동 목적지 지정
    public void ToTitle()
    {
        targetPos = titlePos;
        targetSize = 5f;
        inTitle = true;
        inLobby = false;
        inBook = false;
    }

    public void ToLobby()
    {
        targetPos = lobbyPos;
        targetSize = 16f;
        inTitle = false;
        inLobby = true;
        inBook = false;
    }

    public void ToBook()
    {
        targetPos = bookPos;
        targetSize = 16f;
        inTitle = false;
        inLobby = false;
        inBook = true;
    }

    [Header("===== 책 펼쳐지는 효과 =====")]

    //인스펙터 지정
    [Header("책 표지")]
    public GameObject bookCover;
    [Header("넘기는 속도")]
    public float flipSpeed;
    [Header("감속 효과?")]
    public float smoothTime;

    private bool isFliping = false;
    private Quaternion targetRotation;
    private float velocity = 0f;

    public void FlipPage()
    {
        if (!isFliping)
        {
            isFliping = true;
            Vector3 pivotPoint = bookCover.transform.position + bookCover.transform.right * -0.5f;
            float newYAngle = (Mathf.Abs(bookCover.transform.localEulerAngles.y - 180) < 1f) ? 0 : 180;
            targetRotation = Quaternion.Euler(bookCover.transform.localEulerAngles.x, newYAngle, bookCover.transform.localEulerAngles.z);
            bookCover.transform.RotateAround(pivotPoint, bookCover.transform.up, (newYAngle - bookCover.transform.localEulerAngles.y));
        }
    }
}
