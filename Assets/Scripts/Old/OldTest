using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class TestScripts : MonoBehaviour
{
    public GameObject testObj;
    public FileManager test;
    // Start is called before the first frame update
    void Start()
    {
        test = testObj.GetComponent<FileManager>();
        string[] testVariable = test.GetAllDialogFileNameItHave();

        foreach(var testStr in testVariable){
            Debug.Log(testStr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
