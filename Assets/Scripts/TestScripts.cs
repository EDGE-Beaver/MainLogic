
using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.Windows;

public class TestScripts : MonoBehaviour
{
    public GameObject testObj;
    public FileManager test;
    // Start is called before the first frame update
    void Start()
    { 
        //이 파일은 대체 어디서 오는가. 
        // test = testObj.GetComponent<FileManager>();
        // string[] testVariable = test.GetAllDialogFileNameItHave();

        // foreach(var testStr in testVariable){
        //     Debug.Log(testStr);
        // }

        // test.SetCurrentFile(test.textFilePaths[0]);
        // for(int i = 0; i< test.GetCurrentFileLength(); i++){
        //     string[] testVariable2 = test.GetRowByIndex(i);
        //     foreach(var str1 in testVariable2){
        //         Debug.Log(str1);
        //     }


        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}