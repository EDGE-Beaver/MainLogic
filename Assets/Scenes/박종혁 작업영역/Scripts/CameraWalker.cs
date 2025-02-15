using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraWalker : MonoBehaviour
{
    [Header("===== ī�޶� �̵� ȿ�� =====")]

    //�ν����� ����
    [Header("ī�޶�")]
    public Camera cam;

    [Header("ī�޶� �̵� ������")]
    public GameObject camPos_Title;
    public GameObject camPos_MainLobby;
    public GameObject camPos_Book;

    [Header("ī�޶� �̵� �ӵ�(��� �̵�, Ȯ��/���)")]
    public float camSpeed;
    public float zoomSpeed;

    //ī�޶� �̵� ������ ��ǥ
    private Vector3 titlePos;
    private Vector3 lobbyPos;
    private Vector3 bookPos;

    //ī�޶� �̵� ������
    private Vector3 targetPos;
    private float targetSize;

    //���� � �޴��� �ִ��� Ȯ��
    private bool inTitle;
    private bool inLobby;
    private bool inBook;

    private void Start()
    {
        //ī�޶� �̵� ������ �޾ƿ���
        titlePos = new Vector3(camPos_Title.transform.localPosition.x, camPos_Title.transform.localPosition.y, cam.transform.localPosition.z);
        lobbyPos = new Vector3(camPos_MainLobby.transform.localPosition.x, camPos_MainLobby.transform.localPosition.y, cam.transform.localPosition.z);
        bookPos = new Vector3(camPos_Book.transform.localPosition.x, camPos_Book.transform.localPosition.y, cam.transform.localPosition.z);

        //��ǥ Ȯ�ο�
        Debug.Log(titlePos + ", " + lobbyPos + ", " + bookPos);

        //ī�޶� �̵� ������ �⺻��
        targetPos = cam.transform.localPosition;
        targetSize = cam.orthographicSize;

        //ó���� Ÿ��Ʋ���� ����
        inTitle = true;
        inLobby = false;
        inBook = false;

        //å ������ ���� ����
        targetRotation = bookCover.transform.rotation;
    }

    private void Update()
    {
        //ī�޶� �̵�
        cam.transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, camSpeed * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        //Esc ������ Ÿ��Ʋ�� Ż��
        if(!inTitle && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc �Է�");
            ToTitle();
        }

        //å ������ ȸ��
        if (isFliping)
        {
            bookCover.transform.rotation = Quaternion.Slerp(bookCover.transform.rotation, targetRotation, flipSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(bookCover.transform.rotation, targetRotation) < 1f)
            {
                isFliping = false;
            }
        }
    }

    //ī�޶� �̵� ������ ����
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

    [Header("===== å �������� ȿ�� =====")]

    //�ν����� ����
    [Header("å ǥ��")]
    public GameObject bookCover;
    [Header("�ѱ�� �ӵ�")]
    public float flipSpeed;
    [Header("���� ȿ��?")]
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
