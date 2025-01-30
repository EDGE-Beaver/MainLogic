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
        Debug.Log("적 A가 B를 제거했습니다.");
    }

    public void RealEnding()
    {
        Debug.Log("리얼 엔딩 호출!");
    }
}
