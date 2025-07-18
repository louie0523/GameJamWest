using UnityEngine;
using UnityEngine.EventSystems;

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

        // 텍스처를 읽기 가능하도록 설정
        if (cursorTexture != null)
        {
            cursorWidth = cursorTexture.width;
            cursorHeight = cursorTexture.height;

            // 텍스처가 읽기 가능한지 확인
            try
            {
                cursorPixels = cursorTexture.GetPixels();
            }
            catch (UnityException)
            {
                Debug.LogError("커서 텍스처가 읽기 불가능합니다. Import Settings에서 Read/Write Enabled를 체크해주세요.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위에 있으면 픽셀 체크 없이 클릭 허용
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI 요소 클릭됨");
                return; // UI 시스템이 알아서 처리
            }

            if (IsCursorPixelOpaque())
            {
                Debug.Log("커서 불투명 영역 클릭됨");
                // 클릭 처리 코드 여기에 작성
                HandleClick();
            }
            else
            {
                Debug.Log("투명 영역 클릭, 무시");
            }
        }
    }

    bool IsCursorPixelOpaque()
    {
        if (cursorPixels == null || cursorTexture == null)
            return true; // 텍스처 정보가 없으면 기본적으로 클릭 허용

        Vector2 mousePos = Input.mousePosition;

        // 마우스 위치에서 핫스팟 오프셋을 적용
        float cursorX = mousePos.x - hotspot.x;
        float cursorY = mousePos.y - hotspot.y;

        // 커서 텍스처 내의 픽셀 좌표로 변환
        int texX = Mathf.FloorToInt(cursorX) % cursorWidth;
        int texY = Mathf.FloorToInt(cursorY) % cursorHeight;

        // 음수 처리
        if (texX < 0) texX += cursorWidth;
        if (texY < 0) texY += cursorHeight;

        // 범위 체크
        if (texX < 0 || texX >= cursorWidth || texY < 0 || texY >= cursorHeight)
            return false;

        // Unity 텍스처는 왼쪽 하단이 (0,0)이므로 Y 좌표 뒤집기
        texY = cursorHeight - 1 - texY;

        Color pixel = cursorPixels[texY * cursorWidth + texX];

        // 알파가 충분히 높으면 클릭 허용
        return pixel.a > 0.1f;
    }

    void HandleClick()
    {
        // 실제 클릭 처리 로직
        // 예: 레이캐스트로 오브젝트 선택
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("클릭된 오브젝트: " + hit.collider.name);
            // 추가 클릭 처리
        }
    }
}