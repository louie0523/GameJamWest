using UnityEngine;
using UnityEngine.EventSystems;

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

        // �ؽ�ó�� �б� �����ϵ��� ����
        if (cursorTexture != null)
        {
            cursorWidth = cursorTexture.width;
            cursorHeight = cursorTexture.height;

            // �ؽ�ó�� �б� �������� Ȯ��
            try
            {
                cursorPixels = cursorTexture.GetPixels();
            }
            catch (UnityException)
            {
                Debug.LogError("Ŀ�� �ؽ�ó�� �б� �Ұ����մϴ�. Import Settings���� Read/Write Enabled�� üũ���ּ���.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI ��� ���� ������ �ȼ� üũ ���� Ŭ�� ���
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI ��� Ŭ����");
                return; // UI �ý����� �˾Ƽ� ó��
            }

            if (IsCursorPixelOpaque())
            {
                Debug.Log("Ŀ�� ������ ���� Ŭ����");
                // Ŭ�� ó�� �ڵ� ���⿡ �ۼ�
                HandleClick();
            }
            else
            {
                Debug.Log("���� ���� Ŭ��, ����");
            }
        }
    }

    bool IsCursorPixelOpaque()
    {
        if (cursorPixels == null || cursorTexture == null)
            return true; // �ؽ�ó ������ ������ �⺻������ Ŭ�� ���

        Vector2 mousePos = Input.mousePosition;

        // ���콺 ��ġ���� �ֽ��� �������� ����
        float cursorX = mousePos.x - hotspot.x;
        float cursorY = mousePos.y - hotspot.y;

        // Ŀ�� �ؽ�ó ���� �ȼ� ��ǥ�� ��ȯ
        int texX = Mathf.FloorToInt(cursorX) % cursorWidth;
        int texY = Mathf.FloorToInt(cursorY) % cursorHeight;

        // ���� ó��
        if (texX < 0) texX += cursorWidth;
        if (texY < 0) texY += cursorHeight;

        // ���� üũ
        if (texX < 0 || texX >= cursorWidth || texY < 0 || texY >= cursorHeight)
            return false;

        // Unity �ؽ�ó�� ���� �ϴ��� (0,0)�̹Ƿ� Y ��ǥ ������
        texY = cursorHeight - 1 - texY;

        Color pixel = cursorPixels[texY * cursorWidth + texX];

        // ���İ� ����� ������ Ŭ�� ���
        return pixel.a > 0.1f;
    }

    void HandleClick()
    {
        // ���� Ŭ�� ó�� ����
        // ��: ����ĳ��Ʈ�� ������Ʈ ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Ŭ���� ������Ʈ: " + hit.collider.name);
            // �߰� Ŭ�� ó��
        }
    }
}