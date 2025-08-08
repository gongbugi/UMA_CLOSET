using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject wardrobePanel; // 접고 펼 UI 패널
    public GameObject categoryButtonsPanel; // 상의/하의 버튼을 포함하는 패널
    public GameObject topSubCategoryPanel; // 티셔츠, 셔츠 등 버튼을 담는 패널
    public GameObject bottomSubCategoryPanel; // 긴바지, 반바지 등 버튼을 담는 패널
    public GameObject foldButton;    // '>>' 접기 버튼
    public GameObject unfoldButton;  // '<<' 펴기 버튼

    public Transform cameraTransform; // 제어할 카메라의 Transform
    public Vector3 cameraCenterPosition = new Vector3(0f, 1, -10); // 아바타가 중앙에 보일 때의 카메라 위치
    public float animationDuration = 0.5f; // 애니메이션 지속 시간

    private Vector3 cameraOriginalPosition;
    private bool isAnimating = false;
    private bool wasTopPanelActive;
    private bool wasBottomPanelActive;

    private RectTransform panelRectTransform;
    private Vector2 panelOnScreenPosition;
    private Vector2 panelOffScreenPosition;

    void Start()
    {
        // Main Camera를 찾아서 Transform을 할당합니다.
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cameraOriginalPosition = cameraTransform.position;
        }
        else
        {
            Debug.LogError("Main Camera not found in the scene! Please make sure your camera is tagged as 'MainCamera'.");
        }

        // UI 패널 RectTransform 설정
        panelRectTransform = wardrobePanel.GetComponent<RectTransform>();
        panelOnScreenPosition = panelRectTransform.anchoredPosition;
        // 화면 너비만큼 오른쪽으로 이동한 위치를 화면 밖 위치로 설정
        panelOffScreenPosition = new Vector2(panelOnScreenPosition.x + panelRectTransform.rect.width, panelOnScreenPosition.y);


        // 초기 UI 상태 설정
        unfoldButton.SetActive(false);
        foldButton.SetActive(true);
        wardrobePanel.SetActive(true);
        if(categoryButtonsPanel != null) categoryButtonsPanel.SetActive(true);
        if(topSubCategoryPanel != null) wasTopPanelActive = topSubCategoryPanel.activeSelf;
        if(bottomSubCategoryPanel != null) wasBottomPanelActive = bottomSubCategoryPanel.activeSelf;
    }

    // '>>' 접기 버튼을 눌렀을 때 호출
    public void FoldPanel()
    {
        if (isAnimating || cameraTransform == null) return;
        // 현재 하위 패널들의 활성화 상태 저장
        if (topSubCategoryPanel != null) wasTopPanelActive = topSubCategoryPanel.activeSelf;
        if (bottomSubCategoryPanel != null) wasBottomPanelActive = bottomSubCategoryPanel.activeSelf;
        StartCoroutine(AnimateUI(panelOffScreenPosition, cameraCenterPosition, false));
    }

    // '<<' 펴기 버튼을 눌렀을 때 호출
    public void UnfoldPanel()
    {
        if (isAnimating || cameraTransform == null) return;
        StartCoroutine(AnimateUI(panelOnScreenPosition, cameraOriginalPosition, true));
    }

    private IEnumerator AnimateUI(Vector2 panelTargetPosition, Vector3 cameraTargetPosition, bool showPanelAtEnd)
    {
        isAnimating = true;

        // 애니메이션 시작과 함께 버튼 상태를 즉시 변경하여 중복 클릭 방지
        foldButton.SetActive(false);
        unfoldButton.SetActive(false);

        // 카테고리 버튼 패널을 즉시 비활성화 (접힐 때)
        if (!showPanelAtEnd && categoryButtonsPanel != null)
        {
            categoryButtonsPanel.SetActive(false);
            if (topSubCategoryPanel != null) topSubCategoryPanel.SetActive(false);
            if (bottomSubCategoryPanel != null) bottomSubCategoryPanel.SetActive(false);
        }

        // 펼쳐질 때는 애니메이션 시작 시 옷장 패널만 활성화
        if (showPanelAtEnd)
        {
            wardrobePanel.SetActive(true);
        }

        Vector2 panelStartPosition = panelRectTransform.anchoredPosition;
        Vector3 cameraStartPosition = cameraTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            // 부드러운 움직임을 위한 Ease-in-out 효과
            t = t * t * (3f - 2f * t);

            panelRectTransform.anchoredPosition = Vector2.Lerp(panelStartPosition, panelTargetPosition, t);
            cameraTransform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, t);

            yield return null;
        }

        // 애니메이션 종료 후 정확한 위치 설정
        panelRectTransform.anchoredPosition = panelTargetPosition;
        cameraTransform.position = cameraTargetPosition;

        // 애니메이션이 끝난 후 패널 비활성화 (필요한 경우)
        if (!showPanelAtEnd)
        {
            wardrobePanel.SetActive(false);
        }
        else
        {
            if (topSubCategoryPanel != null) topSubCategoryPanel.SetActive(wasTopPanelActive);
            if (bottomSubCategoryPanel != null) bottomSubCategoryPanel.SetActive(wasBottomPanelActive);
        }
        // 최종 버튼 상태 설정
        foldButton.SetActive(showPanelAtEnd);
        unfoldButton.SetActive(!showPanelAtEnd);
        if (categoryButtonsPanel != null) 
        {
            categoryButtonsPanel.SetActive(showPanelAtEnd);
        }

        isAnimating = false;
    }
}
