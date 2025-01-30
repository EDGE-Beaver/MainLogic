using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyAIndex = 0;
    public int bIndex = 5;
    public bool bExists = true;
    public void MoveEnemyA(int newIndex)
    {

        enemyAIndex = newIndex;
        CheckEnemyInteraction();
    }

    private void CheckEnemyInteraction()
    {
        if (bExists && enemyAIndex == bIndex)
        {
            RemoveB();
        }
    }

    private void RemoveB()
    {
        bExists = false;
        Debug.Log("�� A�� B�� �����߽��ϴ�.");
    }

    public void RealEnding()
    {
        Debug.Log("���� ���� ȣ��!");
    }
}
