using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using UnityEngine;


/// <summary>
/// 존재하지 않는 파일에 접근했을 시 생성되는 사용자 정의 예외입니다.
/// </summary>
public class FileDoesNotExistError : Exception{
    //에러에 대한 내용을 담고 있습니다
    public string Errorcontents;//에러가 구체적으로 무슨 내용인지
    public string ErrorSource;//에러가 발생한 곳이 어디인지
    const string ErrorMessage = "잘못된 이름을 가진 파일에 접근해서 생긴 예외입니다";//기본 에러 메세지

    

    /// <summary>
    /// 변수 두개짜리 생성자입니다 
    /// </summary>
    /// <param name="source">어디 스크립트에서 발생한 에러인지 작성하십시오</param>
    /// <param name="contents" 내용을 적을 수 있으면 적어주십시오</param>
    public FileDoesNotExistError(string source, string contents) : base(string.Format("{0} : {1}에서 발생", ErrorMessage, source)){
        this.Errorcontents = contents; 
        this.ErrorSource = source;
    }
    

}
