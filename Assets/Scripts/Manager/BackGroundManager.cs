using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class BackGroundManager : MonoBehaviour
{
    [Header("필요한 UI 요소들")]
    public GameObject BackGroundPanel;
    public Image PanelImage;

    [Header("이 씬에 필요한 백그라운드 파일들")]
    public List<string> NackGroundImageObj = new List<string>();

    public Dictionary<string, Sprite> characterPos = new Dictionary<string, Sprite>();


    // Start is called before the first frame update
    void Awake()
    {
        PanelImage = BackGroundPanel.GetComponent<Image>();
    }

    public void SetCurrentBackground(string name){
        PanelImage.sprite = characterPos[name];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
