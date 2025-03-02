using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

/*
 * 📌 **VariableManager**
 * @author : 이지환
 *
 * @review : 박준건(2/10)
 
 [수정사항(2/14)]
 
 1. 함수 설명 추가
 2. int 부분을 nullable으로 변경. 이제 더는 null을 리턴하지 않습니다. 

 * 📂 [스크립트 설명]
 * -게임 내 변수(상태값)을 저장하고 관리하는 클래스입니다
 * - 전역적으로 변수 값을 저장하고, 변경 및 조회할 수 있도록 지원합니다.
 * - 싱글톤(Singleton) 패턴을 사용하여 모든 씬에서 변수를 공유할 수 있도록 설계되었습니다.

 * [함수 설명]
 
  - Awake()
    
    게임이 시작할 때 이 매니저가 포함된 오브젝트를 싱글톤으로 만듭니다. 

  - SetVariable(key, Value)
    주어진 String값에 대응하는 딕셔너리 내부 변수값을 Value값으로 설정합니다. 
    만일 변수가 존재하지 않는다면 새로 만듭니다. 

  - int GetVariable(string key)

    함수값을 읽어옵니다. 
    만일 존재하지 않을 시 debug.err 메세지와 함께 null을 리턴합니다. 

  - void ModifyVariable(string key, int change)

    함수값을 증가하거나 감소시킵니다. 
    만일 존재하지 않는 변수에 접근한다면 오류 메세지를 리턴합니다. 

 * [변수 설명]

  -  Dictionary<string, int> variables = new Dictionary<string, int>(); 
    변수가 들어가는 딕셔너리입니다. 
    String 타입의 키로 식별하고, int 타입의 값을 저장하고 있습니다. 
 *
 * 🛠️ [주요 기능]
 *
 * 1️⃣ **변수 저장 및 관리**
 *    - 변수는 Dictionary<string, int> 자료구조에 저장됩니다.
 *    - 키(key) 값으로 변수를 식별하며, 해당 변수 값을 저장 및 수정할 수 있습니다.
 *
 * 2️⃣ **변수 조작 기능**
 *    - SetVariable(key, value) → 변수를 설정합니다. (존재하면 덮어쓰기)
 *    - GetVariable(key) → 특정 변수의 현재 값을 반환합니다. 없으면 try-catch를 통해 에러를 감지합니다. 
 *    - ModifyVariable(key, change) → 변수 값을 증가 또는 감소시킵니다. (없으면 새로 추가)
 *
 * 3️⃣ **싱글톤 패턴 적용**
 *    - VariableManager는 게임 내에서 하나의 인스턴스만 존재하도록 설계되었습니다.
 *    - Awake()에서 이미 존재하는 인스턴스가 있으면 새로 생성하지 않고 기존 인스턴스를 유지합니다.
 *    - DontDestroyOnLoad(gameObject)를 사용하여 씬이 변경되더라도 변수가 초기화되지 않습니다.
 *  
 * ⚠️ **예외 처리**
 * - 존재하지 않는 변수를 GetVariable()로 호출하면 기본값 0을 반환합니다.
 * - ModifyVariable() 사용 시, 해당 변수가 없으면 기본값을 0으로 설정한 후 변경합니다.
 */


public class VariableManager : MonoBehaviour
{
    public static VariableManager Instance; // 싱글톤 인스턴스

/// <summary>
/// 🔹 변수를 저장하는 딕셔너리입니다. 
/// <para>
/// String은 변수명, int는 변수값에 대응한다고 생각하며 작업해주십시오. 
/// </para>
/// </summary>
    private Dictionary<string, int> variables = new Dictionary<string, int>(); // 

    void Awake()
    {
        //싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
        //
    }
    /// <summary>
    /// 1. 변수를 추가합니다.
    /// <para>
    /// 2. 이미 추가되어 있는 변수의 경우, 그 변수의 값을 변경합니다
    /// </para> <para>
    /// ex) SetVariable(P1, 5) = P1이라는 이름을 가진 변수에 접근합니다
    /// </para>
    /// 
    /// </summary>
    /// <param name="key"> 1.  변수를 추가할 경우 : 추가한 변수에 접근하기 위해 사용할 키입니다. 
    /// <para>
    ///                    2. 추가된 변수의 경우 : 변수에 접근하기 위해 사용할 키입니다
    ///                     </para>
    ///                    </param>
    /// <param name="value">1. 변수 추가의 경우 : 변수의 초기값입니다. 
    /// <para>
    /// 
    ///                     2. 추가된 변수의 경우 : 변수의 값을 바꿉니다.</para> </param>
    public void SetVariable(string key, int value)
    {
        if (variables.ContainsKey(key))
            variables[key] = value;
        else
            variables.Add(key, value);
    }
    /// <summary>
    /// 딕셔너리 내부 변수의 값을 확인합니다. 
    /// <para>
    /// [!!!] ContainsKey와 동반해서 사용해야 합니다. 
    /// </para>
    /// </summary>
    /// <param name="key"> 확인을 원하는 변수의 인덱스 번호를 기입하십시오</param>
    /// <returns>변수의 값을 리턴합니다. </returns>

    public int? GetVariable(string key)
    {
        //try - catch로 잘못된 곳에 접근할 시 에러를 표시하도록 했음. (2/10 수정사항)
        try{
            return variables[key];
        }catch(NullReferenceException err){
            Debug.LogError(this.name + "의 GetVariable에서 발생한 에러입니다.\n 딕셔너리 내부에 존재하지 않는 값에 접근했습니다"); 
            Debug.LogError("해당 연산의 결과로 null이 전달되었을 것입니다");
            Debug.LogError("다음은 에러 메세지입니다 : " + err.Message);
            return null;
        }
        
    }
    /// <summary>
    /// 변수의 값을 change에 있는 만큼 증/감시킵니다. 
    /// <para>
    /// [!!!] ContainsKey와 동반해서 사용해야 합니다. 만일 오류가 난다면, 존재하는 변수를 지정한 것인지 확인하십시오. 
    /// </para>
    /// </summary>
    /// <param name="key">증감시키길 원하는 변수의 인덱스입니다</param>
    /// <param name="change">증감량입니다</param>

    public void ModifyVariable(string key, int change)
    {
        // if (variables.ContainsKey(key))
        //     variables[key] = value;
        // else
        //     variables.Add(key, value);
        // try{
        //     variables[key] += change;
        // }catch(NullReferenceException err){
        //     Debug.LogError(this.name + "의 ModifyVariable에서 발생한 에러입니다.\n 딕셔너리 내부에 존재하지 않는 값에 접근했습니다"); 
        //     Debug.LogError("다음은 에러 메세지입니다 : " + err.Message);
        
        // }
        
        if (variables.ContainsKey(key))
            variables[key] += change;
        else
            SetVariable(key, change);
    }
}
