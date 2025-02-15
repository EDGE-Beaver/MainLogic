using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnScripts : MonoBehaviour
{
    [Header("메인 게임매니저를 여기애")]
    public MainSceneGameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnterScene1BtnClick(){
        Debug.Log("버튼은 클릭되는중");
        manager.DoEnterScene1 = true;
        SceneManager.LoadScene("SceneTestScene1");

    }
}
