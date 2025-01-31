using UnityEngine;
using UnityEngine.EventSystems;

public class HoverChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer spriteRenderer; // 2D SpriteRenderer
    private Color originalColor;          // ���� ����
    public Color hoverColor = Color.green; // ȣ�� �� ���� (�ʷϻ��� �ϴ� ����Ʈ �÷���� �����ϰ� ����)

    void Start()
    {
        // SpriteRenderer ������Ʈ ��������
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // ���� ���� ����
        }
        else
        {
            Debug.LogError("�̰� �ȵǸ� �ν����� â ���� �������� �����ϼ�");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor; // ȣ�� �� �ʷϻ����� ����
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // ȣ�� ���� �� ���� �������� ����
        }
    }
}
