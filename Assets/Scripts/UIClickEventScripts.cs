using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[스크립트 설명]
 - UI 쪽에서 클릭 이벤트를 처리하는 함수입니다.
*/
public class UIClickEventScripts : MonoBehaviour, IPointerClickHandler
{
    [Header("클릭을 감지할 오브젝트들 모음")]
    public GameObject DialogueBox;

    [Header("필요한 매니저들 연결")]
    public GameObject dialogueManagerObj;
    public DialogueManager dialogueManager;

    void Awake()
    {
        GetAllManagerComponents();
    }

    /// <summary>
    /// 컴포넌트 추가
    /// </summary>
    private void GetAllManagerComponents()
    {      
        CheckManagerAndAssignComp(dialogueManagerObj, out dialogueManager, "DialogManager");
    }
    /// <summary>
    /// 전체 매니저를 체크하고 컴포넌트와 이어주는 역할. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="manager"></param>
    /// <param name="managerName"></param>
    private void CheckManagerAndAssignComp<T>(GameObject obj, out T manager, string managerName) where T : Component
    {
        if (obj != null && obj.TryGetComponent(out manager)) return;

        manager = null;
        Debug.LogError($"[GetAllManagerComponents] {managerName}가 null입니다! ({obj?.name ?? "NULL"})");
    }
    public void OnPointerClick(PointerEventData eventData)
    {   
          if(eventData.button == PointerEventData.InputButton.Left){
            Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
            if(eventData.pointerCurrentRaycast.gameObject == DialogueBox){
                dialogueManager.isClickDialogueBox = true;
            }
        }
    }
}
