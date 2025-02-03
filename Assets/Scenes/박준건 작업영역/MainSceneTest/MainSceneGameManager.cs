using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneGameManager : MonoBehaviour
{
    [Header("어떤 씬을 들렀는지 여부를 체크하는 변수")]
    public bool DoEnterScene1 = false;
    [Header("씬 내부 오브젝트 들어가는 변수")]
    [Tooltip("텍스트가 전부 다 여기로 들어감")]
    public GameObject TextManager;
    [Tooltip("버튼이 전부 다 여기로 들어감")]
    public GameObject BtnManager;
    
    // 싱글톤 패턴
    private static MainSceneGameManager instance = null;
    void Awake()
    {
    	// SoundManager 인스턴스가 이미 있는지 확인, 이 상태로 설정
        if (instance == null){
            instance = this;
        }
         
        // 인스턴스가 이미 있는 경우 오브젝트 제거
        else if (instance != this) 
            Destroy(gameObject);
            
        // 이렇게 하면 다음 scene으로 넘어가도 오브젝트가 사라지지 않습니다.
        DontDestroyOnLoad(gameObject); 
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DoEnterScene1){
            //씬 1을 방문했는지 테스트하기 위함
            TextManager textManager = TextManager.GetComponent<TextManager>();
            textManager.ScenarioText.text = "씬 1을 방문했음";
        }
    }
}
