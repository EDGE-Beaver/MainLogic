using UnityEngine;
using System.Collections.Generic;

/*
 * 📌 **VariableManager**
 * 
 *
 * 📂 아래는 스크립트 설명
 * - 게임 내 변수(상태 값)를 관리하는 클래스입니다.
 * - 전역적으로 변수 값을 저장하고, 변경 및 조회할 수 있도록 지원합니다.
 * - 싱글톤(Singleton) 패턴을 사용하여 모든 씬에서 변수를 공유할 수 있도록 설계되었습니다.
 *
 * 🛠️ 주요 기능:
 * 1️⃣ **변수 저장 및 관리**
 *    - 변수는 Dictionary<string, int> 자료구조에 저장됩니다.
 *    - 키(key) 값으로 변수를 식별하며, 해당 변수 값을 저장 및 수정할 수 있습니다.
 *
 * 2️⃣ **변수 조작 기능**
 *    - SetVariable(key, value) → 변수를 설정합니다. (존재하면 덮어쓰기)
 *    - GetVariable(key) → 특정 변수의 현재 값을 반환합니다. (없으면 기본값 0 반환)
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

    private Dictionary<string, int> variables = new Dictionary<string, int>(); // 🔹 변수 저장소

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVariable(string key, int value)
    {
        if (variables.ContainsKey(key))
            variables[key] = value;
        else
            variables.Add(key, value);
    }

    public int GetVariable(string key)
    {
        return variables.ContainsKey(key) ? variables[key] : 0;
    }

    public void ModifyVariable(string key, int change)
    {
        if (variables.ContainsKey(key))
            variables[key] += change;
        else
            variables[key] = change;
    }
}
