using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class UITheme
{
    [Header("텍스트 색상")]
    public Color textColor = new Color(50f/255f, 50f/255f, 50f/255f, 1f);
    
    [Header("패널 색상")]
    public Color panelColor = new Color(0f, 0f, 0f, 75f/255f);
    
    [Header("버튼 색상")]
    public Color buttonNormalColor = Color.white;
    public Color buttonHighlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color buttonPressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color buttonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    [Header("카테고리 버튼 선택 색상")]
    public Color categorySelectedColor = new Color(1f, 0.9f, 0.6f, 1f); // Light Orange
}

public class UIThemeManager : MonoBehaviour
{
    [Header("테마 설정")]
    [Tooltip("밝은 배경용 테마 (배경0)")]
    public UITheme lightTheme = new UITheme
    {
        textColor = new Color(50f/255f, 50f/255f, 50f/255f, 1f),
        panelColor = new Color(0f, 0f, 0f, 75f/255f),
        buttonNormalColor = Color.white,
        buttonHighlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f),
        buttonPressedColor = new Color(0.8f, 0.8f, 0.8f, 1f),
        buttonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1f),
        categorySelectedColor = new Color(1f, 0.9f, 0.6f, 1f)
    };
    
    [Tooltip("어두운 배경용 테마 (배경1, 배경2)")]
    public UITheme darkTheme = new UITheme
    {
        textColor = new Color(1f, 1f, 1f, 1f), // 흰색 텍스트
        panelColor = new Color(1f, 1f, 1f, 0.8f), // 반투명 흰색 패널
        buttonNormalColor = new Color(1f, 1f, 1f, 0.9f),
        buttonHighlightedColor = new Color(1f, 1f, 1f, 1f),
        buttonPressedColor = new Color(0.9f, 0.9f, 0.9f, 1f),
        buttonSelectedColor = new Color(0.8f, 0.8f, 0.8f, 1f),
        categorySelectedColor = new Color(1f, 0.9f, 0.6f, 1f) // 어두운 테마에서도 동일한 선택 색상 사용
    };
    
    [Header("UI 요소들")]
    [Tooltip("테마가 적용될 텍스트 컴포넌트들")]
    public List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>();
    
    [Tooltip("테마가 적용될 패널 이미지 컴포넌트들")]
    public List<Image> panelImages = new List<Image>();
    
    [Tooltip("테마 적용에서 제외할 패널 이름들")]
    public List<string> excludedPanelNames = new List<string> { "AnimationPanel", "BackgroundPanel" };
    
    [Tooltip("테마가 적용될 버튼 컴포넌트들")]
    public List<Button> buttonComponents = new List<Button>();
    
    // 싱글톤 패턴
    public static UIThemeManager Instance { get; private set; }
    
    private UITheme currentTheme;
    
    // 선택된 버튼들 추적
    private Button currentSelectedMainCategoryButton;
    private Button currentSelectedSubCategoryButton;
    private List<Button> selectedBackgroundButtons = new List<Button>();
    private List<Button> selectedAnimationButtons = new List<Button>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentTheme = lightTheme; // 기본값은 밝은 테마
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // BackgroundManager의 배경 변경 이벤트 구독
        if (BackgroundManager.Instance != null)
        {
            // 초기 테마 적용
            ApplyThemeForBackground(BackgroundManager.Instance.GetCurrentBackgroundIndex());
        }
        
        // UI 요소들 자동 수집 (Inspector에서 수동으로 할당하지 않은 경우)
        if (textComponents.Count == 0 || panelImages.Count == 0 || buttonComponents.Count == 0)
        {
            CollectUIElements();
        }
    }
    
    /// <summary>
    /// 씬의 모든 UI 요소들을 자동으로 수집
    /// </summary>
    void CollectUIElements()
    {
        // 모든 TextMeshProUGUI 컴포넌트 수집
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (var text in allTexts)
        {
            if (!textComponents.Contains(text))
            {
                textComponents.Add(text);
            }
        }
        
        // 패널 역할을 하는 Image 컴포넌트들 수집 (투명도가 있는 것들, 제외 목록 확인)
        Image[] allImages = FindObjectsOfType<Image>();
        foreach (var image in allImages)
        {
            // 제외 목록에 있는 패널은 건너뛰기
            bool isExcluded = false;
            foreach (string excludedName in excludedPanelNames)
            {
                if (image.name.Contains(excludedName))
                {
                    isExcluded = true;
                    break;
                }
            }
            
            if (isExcluded) continue;
            
            // 패널로 보이는 이미지들만 수집 (알파값이 1보다 작거나 특정 이름 패턴)
            if (image.color.a < 1f || image.name.ToLower().Contains("panel") || 
                image.name.ToLower().Contains("background"))
            {
                if (!panelImages.Contains(image))
                {
                    panelImages.Add(image);
                }
            }
        }
        
        // 모든 Button 컴포넌트 수집
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (var button in allButtons)
        {
            if (!buttonComponents.Contains(button))
            {
                buttonComponents.Add(button);
            }
        }
        
        Debug.Log($"UI 요소 자동 수집 완료 - 텍스트: {textComponents.Count}, 패널: {panelImages.Count}, 버튼: {buttonComponents.Count}");
    }
    
    /// <summary>
    /// 배경 인덱스에 따라 적절한 테마 적용
    /// </summary>
    /// <param name="backgroundIndex">배경 인덱스 (0: 밝은 배경, 1,2: 어두운 배경)</param>
    public void ApplyThemeForBackground(int backgroundIndex)
    {
        UITheme targetTheme;
        
        if (backgroundIndex == 0)
        {
            targetTheme = lightTheme;
            Debug.Log("밝은 테마 적용");
        }
        else
        {
            targetTheme = darkTheme;
            Debug.Log("어두운 테마 적용");
        }
        
        ApplyTheme(targetTheme);
    }
    
    /// <summary>
    /// 지정된 테마를 모든 UI 요소에 적용
    /// </summary>
    /// <param name="theme">적용할 테마</param>
    public void ApplyTheme(UITheme theme)
    {
        currentTheme = theme;
        
        // 텍스트 색상 적용
        foreach (var textComponent in textComponents)
        {
            if (textComponent != null)
            {
                textComponent.color = theme.textColor;
            }
        }
        
        // 패널 색상 적용
        foreach (var panelImage in panelImages)
        {
            if (panelImage != null)
            {
                panelImage.color = theme.panelColor;
            }
        }
        
        // 버튼 색상 적용
        foreach (var button in buttonComponents)
        {
            if (button != null)
            {
                ColorBlock colorBlock = button.colors;
                colorBlock.normalColor = theme.buttonNormalColor;
                colorBlock.highlightedColor = theme.buttonHighlightedColor;
                colorBlock.pressedColor = theme.buttonPressedColor;
                colorBlock.selectedColor = theme.buttonSelectedColor;
                button.colors = colorBlock;
            }
        }
        
        // 선택된 버튼들의 색상도 새 테마에 맞게 업데이트
        UpdateSelectedButtonsForNewTheme();
        
        Debug.Log("테마 적용 완료");
    }
    
    /// <summary>
    /// 현재 적용된 테마 반환
    /// </summary>
    public UITheme GetCurrentTheme()
    {
        return currentTheme;
    }
    
    /// <summary>
    /// 새로운 UI 요소를 테마 시스템에 등록
    /// </summary>
    public void RegisterTextComponent(TextMeshProUGUI textComponent)
    {
        if (textComponent != null && !textComponents.Contains(textComponent))
        {
            textComponents.Add(textComponent);
            textComponent.color = currentTheme.textColor;
        }
    }
    
    public void RegisterPanelImage(Image panelImage)
    {
        if (panelImage != null && !panelImages.Contains(panelImage))
        {
            // 제외 목록 확인
            bool isExcluded = false;
            foreach (string excludedName in excludedPanelNames)
            {
                if (panelImage.name.Contains(excludedName))
                {
                    isExcluded = true;
                    break;
                }
            }
            
            if (!isExcluded)
            {
                panelImages.Add(panelImage);
                panelImage.color = currentTheme.panelColor;
            }
        }
    }
    
    public void RegisterButton(Button button)
    {
        if (button != null && !buttonComponents.Contains(button))
        {
            buttonComponents.Add(button);
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = currentTheme.buttonNormalColor;
            colorBlock.highlightedColor = currentTheme.buttonHighlightedColor;
            colorBlock.pressedColor = currentTheme.buttonPressedColor;
            colorBlock.selectedColor = currentTheme.buttonSelectedColor;
            button.colors = colorBlock;
        }
    }
    
    /// <summary>
    /// 메인 카테고리 버튼 선택 관리
    /// </summary>
    public void SetMainCategoryButtonSelected(Button selectedButton)
    {
        // 이전 선택된 버튼을 일반 상태로 복원
        if (currentSelectedMainCategoryButton != null)
        {
            SetButtonToNormalState(currentSelectedMainCategoryButton);
        }
        
        // 새 버튼을 선택 상태로 설정
        if (selectedButton != null)
        {
            SetButtonToSelectedState(selectedButton);
            currentSelectedMainCategoryButton = selectedButton;
        }
    }
    
    /// <summary>
    /// 서브 카테고리 버튼 선택 관리
    /// </summary>
    public void SetSubCategoryButtonSelected(Button selectedButton)
    {
        // 이전 선택된 버튼을 일반 상태로 복원
        if (currentSelectedSubCategoryButton != null)
        {
            SetButtonToNormalState(currentSelectedSubCategoryButton);
        }
        
        // 새 버튼을 선택 상태로 설정
        if (selectedButton != null)
        {
            SetButtonToSelectedState(selectedButton);
            currentSelectedSubCategoryButton = selectedButton;
        }
    }
    
    /// <summary>
    /// 배경 버튼 선택 관리 (단일 선택)
    /// </summary>
    public void SetBackgroundButtonSelected(Button selectedButton, Button[] allBackgroundButtons)
    {
        // 모든 배경 버튼을 일반 상태로 복원
        foreach (var button in allBackgroundButtons)
        {
            if (button != null)
            {
                SetButtonToNormalState(button);
            }
        }
        
        // 선택된 버튼을 선택 상태로 설정
        if (selectedButton != null)
        {
            SetButtonToSelectedState(selectedButton);
        }
    }
    
    /// <summary>
    /// 애니메이션 버튼 선택 관리 (단일 선택)
    /// </summary>
    public void SetAnimationButtonSelected(Button selectedButton, Button[] allAnimationButtons)
    {
        // 모든 애니메이션 버튼을 일반 상태로 복원
        foreach (var button in allAnimationButtons)
        {
            if (button != null)
            {
                SetButtonToNormalState(button);
            }
        }
        
        // 선택된 버튼을 선택 상태로 설정
        if (selectedButton != null)
        {
            SetButtonToSelectedState(selectedButton);
        }
    }
    
    /// <summary>
    /// 버튼을 일반 상태로 설정
    /// </summary>
    private void SetButtonToNormalState(Button button)
    {
        if (button != null)
        {
            // Image 컴포넌트가 있는 경우 (메인/서브 카테고리 버튼)
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = currentTheme.buttonNormalColor;
            }
            
            // ColorBlock을 사용하는 버튼들 (배경/애니메이션 버튼)
            var colorBlock = button.colors;
            colorBlock.normalColor = currentTheme.buttonNormalColor;
            colorBlock.highlightedColor = currentTheme.buttonHighlightedColor;
            colorBlock.pressedColor = currentTheme.buttonPressedColor;
            colorBlock.selectedColor = currentTheme.buttonSelectedColor;
            button.colors = colorBlock;
        }
    }
    
    /// <summary>
    /// 버튼을 선택 상태로 설정
    /// </summary>
    private void SetButtonToSelectedState(Button button)
    {
        if (button != null)
        {
            // Image 컴포넌트가 있는 경우 (메인/서브 카테고리 버튼)
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = currentTheme.categorySelectedColor;
            }
            
            // ColorBlock을 사용하는 버튼들 (배경/애니메이션 버튼)
            var colorBlock = button.colors;
            colorBlock.normalColor = currentTheme.categorySelectedColor;
            button.colors = colorBlock;
        }
    }
    
    /// <summary>
    /// 현재 선택된 모든 버튼들의 색상을 새 테마에 맞게 업데이트
    /// </summary>
    public void UpdateSelectedButtonsForNewTheme()
    {
        if (currentSelectedMainCategoryButton != null)
        {
            SetButtonToSelectedState(currentSelectedMainCategoryButton);
        }
        
        if (currentSelectedSubCategoryButton != null)
        {
            SetButtonToSelectedState(currentSelectedSubCategoryButton);
        }
    }
}