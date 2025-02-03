using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 파일 매니저에서 상속받아서 가져온 클래스
/// 싱글톤을 적용해서 클래스가 사라지지 않도록 함. 
/// </summary>
public class FileManagerMain : FileManager
{
    // Start is called before the first frame update
    private static FileManagerMain instance = null;
    void Awake()
    {
    	// SoundManager 인스턴스가 이미 있는지 확인, 이 상태로 설정
        if (instance == null){
            instance = this;
        }
         
        // 인스턴스가 이미 있는 경우 오브젝트 제거
        //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
        else if (instance != this) 
            Destroy(this.gameObject);
            
        // 이렇게 하면 다음 scene으로 넘어가도 오브젝트가 사라지지 않습니다.
        DontDestroyOnLoad(this.gameObject); 
        LoadAllTextFiles();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
