using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �Ŵ������� ��ӹ޾Ƽ� ������ Ŭ����
/// �̱����� �����ؼ� Ŭ������ ������� �ʵ��� ��. 
/// </summary>
public class FileManagerLJH: FileManager
{
    // Start is called before the first frame update
    private static FileManagerLJH instance = null;
    void Awake()
    {
        // SoundManager �ν��Ͻ��� �̹� �ִ��� Ȯ��, �� ���·� ����
        if (instance == null)
        {
            instance = this;
        }

        // �ν��Ͻ��� �̹� �ִ� ��� ������Ʈ ����
        //gameObject�����ε� �� ��ũ��Ʈ�� ������Ʈ�μ� �پ��ִ� Hierarchy���� ���ӿ�����Ʈ��� ��������, 
        //���� �򰥸� ������ ���� this�� �ٿ��ֱ⵵ �Ѵ�.
        else if (instance != this)
            Destroy(this.gameObject);

        // �̷��� �ϸ� ���� scene���� �Ѿ�� ������Ʈ�� ������� �ʽ��ϴ�.
        DontDestroyOnLoad(this.gameObject);
        LoadAllTextFiles();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
