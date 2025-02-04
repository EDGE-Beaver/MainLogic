using UnityEngine;
using System.Collections.Generic;

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
