using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Image = UnityEngine.UI.Image;

public class BackGroundManager : MonoBehaviour
{
    [Header("필요한 UI 요소들")]
    public GameObject BackGroundPanel;
    public Image PanelImage;

    [Header("이 씬에 필요한 백그라운드 파일들")]
    public List<string> BackGroundImagePath = new List<string>();

    [Header("실제로 백그라운드 파일들이 저장되는 곳.")]
    public Dictionary<string, Sprite> BackGroundImage = new Dictionary<string, Sprite>();

    public HashSet<string> BackGroundImageSet = new HashSet<string>();

    // Start is called before the first frame update
    void Awake()
    {
        PanelImage = BackGroundPanel.GetComponent<Image>();
        LoadAllBackGroundImage();
    }

    private void LoadAllBackGroundImage()
    {
        BackGroundImageSet.Clear();
        BackGroundImage.Clear();
        //읽어야 하는 파일들마다 반복
        foreach (var filePath in BackGroundImagePath)
        {
            Sprite imageAsset = Resources.Load<Sprite>(filePath);
            if (imageAsset == null)
            {
                Debug.LogError($"🚨 이미지 파일을 찾을 수 없습니다 : {filePath}");
                continue;
            }

    
            
            string fileName = Path.GetFileNameWithoutExtension(filePath); 

            BackGroundImage[fileName] = imageAsset;
            BackGroundImageSet.Add(fileName);

            Debug.Log($"📂백그라운드 이미지 파일 로드 완료: {fileName}");
       }
    }

    public void SetCurrentBackground(string name){
        if(BackGroundImage.ContainsKey(name)) PanelImage.sprite = BackGroundImage[name];
        else Debug.LogError("그런 백그라운드 이미지는 존재하지 않습니다.");
    }

}
