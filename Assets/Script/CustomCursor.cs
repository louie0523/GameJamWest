using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;  // 커서 이미지 (투명 영역 포함)
    public Vector2 hotspot = Vector2.zero;  // 커서 기준점

    private Color[] cursorPixels;
    private int cursorWidth;
    private int cursorHeight;

    void Start()
    {
        // 커서 설정
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

        // 텍스처 픽셀 정보 미리 가져오기
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
                Debug.Log("커서 불투명 영역 클릭됨");
                // 클릭 처리 코드 여기에 작성
            }
            else
            {
                Debug.Log("투명 영역 클릭, 무시");
            }
        }
    }

    bool IsCursorPixelOpaque()
    {
        Vector2 mousePos = Input.mousePosition;

        // 화면 좌표를 텍스처 픽셀 좌표로 변환
        // 마우스 위치에서 핫스팟 위치를 뺌
        int texX = (int)(mousePos.x - hotspot.x);
        int texY = (int)(mousePos.y - hotspot.y);

        // 텍스처가 스크린과 y축 반대 방향이기 때문에 변환 필요
        texY = Screen.height - texY;

        // 범위 밖이면 무조건 투명으로 간주
        if (texX < 0 || texX >= cursorWidth || texY < 0 || texY >= cursorHeight)
            return false;

        Color pixel = cursorPixels[texY * cursorWidth + texX];

        // 알파가 충분히 높으면 클릭 허용
        return pixel.a > 0.1f;
    }
}
