using System.Collections.Generic;
using System.IO;
using UnityEngine;




public abstract class FileManager : MonoBehaviour
{

    /*
     * 
     * ȭ�� �̸�|��ȭ ����|SE ���� �̸�|�̹��� ���� �̸�|�� ������|������1,������2,������3
     * ����:
    NPC1|�ȳ��ϼ���! ����� �׽�Ʈ ���Դϴ�.|se_greeting|npc1_image|Scene1|
    NPC2|������ ���͵帱���?|se_help|npc2_image|Scene2|���� ��û�ϱ�,�׳� ������
     * 
     */


    // �ε�� �����͸� �����ϴ� ��ųʸ�: ���� �̸��� Ű��, �� ���� �Ľ��� ������ ����Ʈ�� ������ ����
    protected Dictionary<string, List<string[]>> loadedData = new Dictionary<string, List<string[]>>();

    [Header("�ؽ�Ʈ ���� ��� ��� (Resources ���� ����)")]
    public List<string> textFilePaths;

    [Header("������ ������")]
    public char delimiter = '|';

    /// <summary>
    /// ��� �ؽ�Ʈ ���� �ε�
    /// </summary>
    protected void LoadAllTextFiles()
    {
        foreach (var filePath in textFilePaths)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            if (textAsset == null)
            {
                Debug.LogError($"�ؽ�Ʈ ������ ã�� �� �����ϴ�: {filePath}");
                continue;
            }

            var lines = textAsset.text.Split('\n');
            var dataList = new List<string[]>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var tokens = line.Split(delimiter);
                    dataList.Add(tokens);
                }
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            loadedData[fileName] = dataList; // �ε�� �����͸� ��ųʸ��� ����

            Debug.Log($"�ؽ�Ʈ ���� �ε� �Ϸ�: {fileName}");
        }
    }

    /// <summary>
    /// Ư�� ������ Ư�� �ε��� ������ ��������
    /// </summary>
    /// <param name="fileName">���� �̸�</param>
    /// <param name="index">������ �ε���</param>
    /// <returns>������ �迭 (null ��ȯ ����)</returns>
    public string[] GetRowByIndex(string fileName, int index)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogError($"���� �̸� '{fileName}'�� �ش��ϴ� �����Ͱ� �����ϴ�.");
            return null;
        }

        var dataList = loadedData[fileName];
        if (index >= 0 && index < dataList.Count)
            return dataList[index];

        Debug.LogWarning($"�߸��� �ε��� ��û: {index} (����: {fileName})");
        return null;
    }

    /// <summary>
    /// Ư�� ������ �� �� �� ��ȯ
    /// </summary>
    /// <param name="fileName">���� �̸�</param>
    /// <returns>�� �� ��</returns>
    public int GetRowCount(string fileName)
    {
        if (!loadedData.ContainsKey(fileName))
        {
            Debug.LogWarning($"���� �̸� '{fileName}'�� �ش��ϴ� �����Ͱ� �����ϴ�.");
            return 0;
        }

        return loadedData[fileName].Count;
    }

    /// <summary>
    /// �ε�� ���� �̸� ��� ��ȯ
    /// </summary>
    /// <returns>���� �̸� ����Ʈ</returns>
    public List<string> GetLoadedFileNames()
    {
        return new List<string>(loadedData.Keys);
    }
}
