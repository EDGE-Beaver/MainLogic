using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//분기 변화 등을 괸리하는 스크립트. 
//해당 기능들을 다 구현해서 넣어주면 됨.
public interface BranchChangedManager 
{
    //변수 접근 로직(뱐수 이름과 기타등등 명시)
    public void SelectBranch(){ 
        //상속받은 브랜치에서는 이거 일일히 다 작성해주기.
    }

    public void ChangeBranch(){

    }

    public void ChangeText(){

    }
}
