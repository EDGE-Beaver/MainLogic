using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectingManager : MonoBehaviour
{
    [Header("분기 변화와 관련된 스크립트 작성되어 있는 파일들 여기에 넣기")]
    public List<BranchChangedManager> BranchManagers = new List<BranchChangedManager>();
    //얘내 상속받은 애들 넣어도 괜찮음


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //변수에 따라 다른 작용을 확인하는 장면. 
}
