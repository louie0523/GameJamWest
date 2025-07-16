using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;  // Ŀ�� �̹��� (���� ���� ����)
    public Vector2 hotspot = Vector2.zero;  // Ŀ�� ������

    private Color[] cursorPixels;
    private int cursorWidth;
    private int cursorHeight;

    void Start()
    {
        // Ŀ�� ����
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

        // �ؽ�ó �ȼ� ���� �̸� ��������
        cursorWidth = cursorTexture.width;
        cursorHeight = cursorTexture.height;
        cursorPixels = cursorTexture.GetPixels();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsCursorPixelOpaque())
            {
                Debug.Log("Ŀ�� ������ ���� Ŭ����");
                // Ŭ�� ó�� �ڵ� ���⿡ �ۼ�
            }
            else
            {
                Debug.Log("���� ���� Ŭ��, ����");
            }
        }
    }

    bool IsCursorPixelOpaque()
    {
        Vector2 mousePos = Input.mousePosition;

        // ȭ�� ��ǥ�� �ؽ�ó �ȼ� ��ǥ�� ��ȯ
        // ���콺 ��ġ���� �ֽ��� ��ġ�� ��
        int texX = (int)(mousePos.x - hotspot.x);
        int texY = (int)(mousePos.y - hotspot.y);

        // �ؽ�ó�� ��ũ���� y�� �ݴ� �����̱� ������ ��ȯ �ʿ�
        texY = Screen.height - texY;

        // ���� ���̸� ������ �������� ����
        if (texX < 0 || texX >= cursorWidth || texY < 0 || texY >= cursorHeight)
            return false;

        Color pixel = cursorPixels[texY * cursorWidth + texX];

        // ���İ� ����� ������ Ŭ�� ���
        return pixel.a > 0.1f;
    }
}
