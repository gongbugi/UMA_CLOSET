using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;

public class WardrobeManager : MonoBehaviour
{

    private List<OutfitData> allOutfits; // 모든 의상 데이터 리스트

    public GameObject outfitButtonPrefab;
    public GameObject unequipButtonPrefab;
    public Transform outfitListContent;

    [Header("Category UI")]
    public Button topsButton;
    public Button bottomsButton;
    public GameObject topSubCategoryPanel;
    public GameObject bottomSubCategoryPanel;

    [Header("Sub Category 'ALL' Buttons")]
    public Button allTopsButton;
    public Button allBottomsButton;

    [Header("Action Buttons")]
    public Button unequipAllButton; // 옷 전체 해제 버튼

    [Header("Background UI")]
    public Button backgroundButton; // 배경 카테고리 버튼
    public GameObject backgroundPanel; // 배경 버튼들을 담는 패널
    public Button[] backgroundSelectButtons = new Button[3]; // 배경 선택 버튼들

    [Header("Animation UI")]
    public GameObject animationPanel; // 애니메이션 버튼들을 담는 패널
    public Button[] animationSelectButtons = new Button[3]; // 애니메이션 선택 버튼들 (기본, 걷기, 뛰기)


    private Button currentSelectedMainButton;
    private Button currentSelectedSubButton;
    private List<OutfitData> currentFilteredList;
    private UMA.CharacterSystem.DynamicCharacterAvatar avatar;
    
    // HTTP 텍스처 캐싱
    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private string authToken = "";

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetTokenFromLocalStorage();
#endif

    void Awake()
    {
        allOutfits = new List<OutfitData>();
        
        // OutfitDataLoader의 이벤트에 구독
        if (OutfitDataLoader.Instance != null)
        {
            OutfitDataLoader.Instance.OnOutfitsLoaded += OnOutfitsLoadedFromJson;
        }
        
        // BodyMeasurements의 이벤트에 구독
        if (BodyMeasurements.Instance != null)
        {
            BodyMeasurements.Instance.OnMeasurementsLoaded += OnBodyMeasurementsLoaded;
        }
    }

    void Start()
    {
        // 씬에서 UMA 아바타를 찾습니다. (최신 API 사용)
        avatar = FindFirstObjectByType<UMA.CharacterSystem.DynamicCharacterAvatar>();
        if (avatar == null)
        {
            Debug.LogError("씬에서 DynamicCharacterAvatar를 찾을 수 없습니다!");
        }

        topSubCategoryPanel.SetActive(false);
        bottomSubCategoryPanel.SetActive(false);

        topsButton.onClick.AddListener(OnTopsCategorySelected);
        bottomsButton.onClick.AddListener(OnBottomsCategorySelected);
        unequipAllButton.onClick.AddListener(UnequipAllOutfits);

        // 배경 관련 초기화
        InitializeBackgroundUI();

        // LocalStorage에서 토큰 추출
        ExtractTokenFromLocalStorage();
        
        // JSON 데이터 로드 시작
        if (OutfitDataLoader.Instance != null)
        {
            OutfitDataLoader.Instance.LoadOutfits();
        }
        else
        {
            Debug.LogError("OutfitDataLoader Instance가 없습니다. OutfitDataLoader를 씬에 추가해주세요.");
        }
        
        // 신체 측정 데이터 로드 시작
        if (BodyMeasurements.Instance != null)
        {
            BodyMeasurements.Instance.LoadMeasurements();
        }
        else
        {
            Debug.LogError("BodyMeasurements Instance가 없습니다. BodyMeasurements를 씬에 추가해주세요.");
        }
    }


    private void OnOutfitsLoadedFromJson(List<OutfitData> outfits)
    {
        allOutfits = outfits;
        Debug.Log($"의상 데이터 로드 완료: {allOutfits.Count}개");
        
        // 초기 상태를 Tops -> All로 설정
        SelectMainCategory(topsButton);
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (OutfitDataLoader.Instance != null)
        {
            OutfitDataLoader.Instance.OnOutfitsLoaded -= OnOutfitsLoadedFromJson;
        }
        
        if (BodyMeasurements.Instance != null)
        {
            BodyMeasurements.Instance.OnMeasurementsLoaded -= OnBodyMeasurementsLoaded;
        }
    }

    private void OnBodyMeasurementsLoaded(BodyMeasurementData measurements)
    {
        Debug.Log("신체 측정 데이터가 로드되었습니다. UMA 아바타에 적용합니다.");
        ApplyBodyMeasurementsToAvatar();
    }

    /// <summary>
    /// 로드된 신체 측정 데이터를 UMA 아바타의 DNA에 적용
    /// </summary>
    public void ApplyBodyMeasurementsToAvatar()
    {
        if (avatar == null)
        {
            Debug.LogError("UMA 아바타를 찾을 수 없습니다!");
            return;
        }

        if (BodyMeasurements.Instance == null)
        {
            Debug.LogError("BodyMeasurements Instance가 없습니다!");
            return;
        }

        var dnaValues = BodyMeasurements.Instance.GetUMADNAValues();
        if (dnaValues == null)
        {
            Debug.LogError("UMA DNA 값을 가져올 수 없습니다!");
            return;
        }

        // UMA 아바타의 DNA 설정
        try
        {
            // uma_height -> other의 Height (소문자 'height'가 정확한 DNA 이름)
            if (dnaValues.ContainsKey("height"))
                avatar.SetDNA("height", dnaValues["height"]);

            // uma_width -> Arm의 Width (기존 shoulder_width)
            if (dnaValues.ContainsKey("ArmWidth"))
                avatar.SetDNA("armWidth", dnaValues["ArmWidth"]);

            // uma_waist -> other의 Waist
            if (dnaValues.ContainsKey("Waist"))
                avatar.SetDNA("waist", dnaValues["Waist"]);

            // uma_belly -> other의 Belly
            if (dnaValues.ContainsKey("Belly"))
                avatar.SetDNA("belly", dnaValues["Belly"]);

            // uma_fore_arm -> Forearm의 Length
            if (dnaValues.ContainsKey("ForearmLength"))
                avatar.SetDNA("forearmLength", dnaValues["ForearmLength"]);

            // uma_arm -> Arm의 Length
            if (dnaValues.ContainsKey("ArmLength"))
                avatar.SetDNA("armLength", dnaValues["ArmLength"]);

            // uma_legs -> Legs의 Size
            if (dnaValues.ContainsKey("LegsSize"))
                avatar.SetDNA("legsSize", dnaValues["LegsSize"]);

            // 아바타 리빌드
            avatar.BuildCharacter();
            Debug.Log("UMA 아바타에 신체 측정 데이터 적용 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UMA DNA 적용 중 오류 발생: {e.Message}");
        }
    }

    private void SelectMainCategory(Button selectedButton)
    {
        if (currentSelectedMainButton == selectedButton) return;

        // UIThemeManager를 통해 버튼 선택 상태 관리
        if (UIThemeManager.Instance != null)
        {
            UIThemeManager.Instance.SetMainCategoryButtonSelected(selectedButton);
        }
        
        currentSelectedMainButton = selectedButton;

        if (selectedButton == topsButton)
        {
            topSubCategoryPanel.SetActive(true);
            bottomSubCategoryPanel.SetActive(false);
            ShowAllTops();
        }
        else
        {
            topSubCategoryPanel.SetActive(false);
            bottomSubCategoryPanel.SetActive(true);
            ShowAllBottoms();
        }
    }

    private void SelectSubCategory(Button selectedButton)
    {
        if (currentSelectedSubButton == selectedButton) return;

        // UIThemeManager를 통해 버튼 선택 상태 관리
        if (UIThemeManager.Instance != null)
        {
            UIThemeManager.Instance.SetSubCategoryButtonSelected(selectedButton);
        }
        
        currentSelectedSubButton = selectedButton;
    }

    private void ClearWardrobeUI()
    {
        foreach (Transform child in outfitListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateWardrobeUI(List<OutfitData> outfitsToShow, ClothingType category)
    {
        ClearWardrobeUI();
        currentFilteredList = outfitsToShow;

        // Add 'Unequip' button at the beginning
        GameObject unequipButtonGO = Instantiate(unequipButtonPrefab, outfitListContent);
        Button unequipButton = unequipButtonGO.GetComponent<Button>();
        unequipButton.onClick.AddListener(() => UnequipOutfitByCategory(category));


        for (int i = 0; i < currentFilteredList.Count; i++)
        {
            GameObject buttonGO = Instantiate(outfitButtonPrefab, outfitListContent);
            Image buttonImage = buttonGO.GetComponent<Image>();
            if (buttonImage != null && currentFilteredList[i].thumbnail != null)
            {
                buttonImage.sprite = currentFilteredList[i].thumbnail;
            }

            Button button = buttonGO.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(() => ChangeOutfitByIndex(index));
        }
    }

    public void OnTopsCategorySelected() => SelectMainCategory(topsButton);
    public void OnBottomsCategorySelected() => SelectMainCategory(bottomsButton);

    public void ShowAllTops()
    {
        SelectSubCategory(allTopsButton);
        var filtered = allOutfits.Where(o => o.type == ClothingType.Top).ToList();
        PopulateWardrobeUI(filtered, ClothingType.Top);
    }

    public void ShowAllBottoms()
    {
        SelectSubCategory(allBottomsButton);
        var filtered = allOutfits.Where(o => o.type == ClothingType.Bottom).ToList();
        PopulateWardrobeUI(filtered, ClothingType.Bottom);
    }

    public void ShowTopsBySubCategory(int topCategory)
    {
        Button clickedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        SelectSubCategory(clickedButton);
        var filtered = allOutfits.Where(o => o.type == ClothingType.Top && o.topCategory == (TopCategory)topCategory).ToList();
        PopulateWardrobeUI(filtered, ClothingType.Top);
    }

    public void ShowBottomsBySubCategory(int bottomCategory)
    {
        Button clickedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        SelectSubCategory(clickedButton);
        var filtered = allOutfits.Where(o => o.type == ClothingType.Bottom && o.bottomCategory == (BottomCategory)bottomCategory).ToList();
        PopulateWardrobeUI(filtered, ClothingType.Bottom);
    }


    public void ChangeOutfitByIndex(int index)
    {
        OutfitData selectedOutfit = currentFilteredList[index];
        string recipePath = GetRecipePathByConvention(selectedOutfit); // "Custom/Tshirt_Recipe"
        string overlayPath = GetOverlayPathByConvention(selectedOutfit); // "Custom/Tshirt_Overlay"

        // 1. Resources에서 레시피 에셋을 로드
        var recipe = Resources.Load<UMAWardrobeRecipe>(recipePath);

        avatar.SetSlot(recipe);

        // 2. 레시피의 텍스처를 교체
        if (!string.IsNullOrEmpty(selectedOutfit.texturePath))
        {
            // HTTP URL인지 확인
            if (selectedOutfit.texturePath.StartsWith("http"))
            {
                // HTTP에서 텍스처 로드
                StartCoroutine(LoadTextureFromHTTP(selectedOutfit.texturePath, recipe, overlayPath));
            }
            // 절대 경로인지 확인 (Windows: C:\, D:\ / Unix: /)
            else if (System.IO.Path.IsPathRooted(selectedOutfit.texturePath))
            {
                // 로컬 파일에서 텍스처 로드
                StartCoroutine(LoadTextureFromFile(selectedOutfit.texturePath, recipe, overlayPath));
            }
            else
            {
                // Resources에서 텍스처 로드 (기존 방식)
                var texture = Resources.Load<Texture2D>(selectedOutfit.texturePath);
                if (texture != null)
                {
                    ApplyTextureToRecipe(recipe, overlayPath, texture);
                }
                else
                {
                    Debug.LogError($"텍스처 로드 실패: {selectedOutfit.texturePath}");
                }
            }
        }

        // 3. Resources 텍스처인 경우에만 즉시 빌드 (HTTP/로컬 파일은 코루틴에서 빌드)
        if (!selectedOutfit.texturePath.StartsWith("http") && !System.IO.Path.IsPathRooted(selectedOutfit.texturePath))
        {
            avatar.BuildCharacter();
        }
        Debug.Log($"'{selectedOutfit.outfitName}' 착용 시작!");
    }


    // 오버레이의 텍스처를 교체하는 함수
    void ApplyTextureToRecipe(UMAWardrobeRecipe recipe, string overlayPath, Texture2D texture)
    {
        var overlayAsset = Resources.Load<OverlayDataAsset>(overlayPath);

        if (overlayAsset != null)
        {
            overlayAsset.textureList[0] = texture;
            Debug.Log($"'${overlayAsset.overlayName}' 오버레이의 텍스처를 성공적으로 교체했습니다.");
        }
        else
        {
            Debug.LogWarning($"레시피 '{recipe.name}'의 슬롯에서 오버레이를 찾지 못했습니다.");
        }
    }


    private string GetRecipePathByConvention(OutfitData outfit)
    {
        if (outfit.type == ClothingType.Top && outfit.topCategory == TopCategory.Tshirt)
        {
            return "Custom/Tshirt_Recipe";
        }
        if (outfit.type == ClothingType.Top && outfit.topCategory == TopCategory.Shirt)
        {
            return "Custom/Shirt_Recipe";
        }
        if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Pants)
        {
            return "Custom/Pants_Recipe";
        }
        if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Shorts)
        {
            return "Custom/Shorts_Recipe";
        }

        return null; // 규칙에 맞는 레시피가 없는 경우
    }

    private string GetOverlayPathByConvention(OutfitData outfit)
    {
        if (outfit.type == ClothingType.Top && outfit.topCategory == TopCategory.Tshirt)
        {
            return "Custom/Tshirt_Overlay";
        }
        if (outfit.type == ClothingType.Top && outfit.topCategory == TopCategory.Shirt)
        {
            return "Custom/Shirt_Overlay";
        }
        if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Pants)
        {
            return "custom/Pants_Overlay";
        }
        if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Shorts)
        {
            return "Custom/Shorts_Overlay";
        }
        return null; // 규칙에 맞는 레시피가 없는 경우
    }

    // HTTP에서 텍스처를 로드하는 코루틴
    private IEnumerator LoadTextureFromHTTP(string url, UMAWardrobeRecipe recipe, string overlayPath)
    {
        // 캐시 확인
        if (textureCache.ContainsKey(url))
        {
            ApplyTextureToRecipe(recipe, overlayPath, textureCache[url]);
            avatar.BuildCharacter();
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        
        // 토큰이 있으면 Authorization 헤더에 추가
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            textureCache[url] = texture; // 캐시에 저장
            ApplyTextureToRecipe(recipe, overlayPath, texture);
            avatar.BuildCharacter();
            Debug.Log($"HTTP 텍스처 로드 성공: {url}");
        }
        else
        {
            Debug.LogError($"HTTP 텍스처 로드 실패: {url} - {request.error}");
        }

        request.Dispose();
    }

    // 로컬 파일에서 텍스처를 로드하는 코루틴
    private IEnumerator LoadTextureFromFile(string filePath, UMAWardrobeRecipe recipe, string overlayPath)
    {
        // 캐시 확인
        if (textureCache.ContainsKey(filePath))
        {
            ApplyTextureToRecipe(recipe, overlayPath, textureCache[filePath]);
            avatar.BuildCharacter();
            yield break;
        }

        // file:// 프로토콜 사용하여 로컬 파일 로드
        string fileUrl = "file://" + filePath.Replace("\\", "/");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fileUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            textureCache[filePath] = texture; // 캐시에 저장
            ApplyTextureToRecipe(recipe, overlayPath, texture);
            avatar.BuildCharacter();
            Debug.Log($"로컬 텍스처 로드 성공: {filePath}");
        }
        else
        {
            Debug.LogError($"로컬 텍스처 로드 실패: {filePath} - {request.error}");
        }

        request.Dispose();
    }

    public void UnequipAllOutfits()
    {
        if (avatar == null) return;

        avatar.ClearSlot("Chest");
        avatar.ClearSlot("Legs");
        avatar.BuildCharacter();
        Debug.Log("모든 의상을 해제했습니다.");
    }

    public void UnequipOutfitByCategory(ClothingType type)
    {
        if (avatar == null) return;

        string slotToClear = "";
        switch (type)
        {
            case ClothingType.Top:
                slotToClear = "Chest";
                break;
            case ClothingType.Bottom:
                slotToClear = "Legs";
                break;
        }

        if (!string.IsNullOrEmpty(slotToClear))
        {
            avatar.ClearSlot(slotToClear);
            avatar.BuildCharacter();
            Debug.Log($"{type} 카테고리 의상을 해제했습니다.");
        }
    }

    #region 배경 관련 메서드들

    /// <summary>
    /// 배경 UI 초기화
    /// </summary>
    void InitializeBackgroundUI()
    {
        // 배경 패널 초기 비활성화
        if (backgroundPanel != null)
        {
            backgroundPanel.SetActive(false);
        }
        // 애니메이션 패널 초기 비활성화
        if (animationPanel != null)
        {
            animationPanel.SetActive(false);
        }

        // 배경 카테고리 버튼 이벤트 설정
        if (backgroundButton != null)
        {
            backgroundButton.onClick.AddListener(OnBackgroundCategorySelected);
        }

        // 각 배경 선택 버튼 이벤트 설정
        for (int i = 0; i < backgroundSelectButtons.Length; i++)
        {
            if (backgroundSelectButtons[i] != null)
            {
                int index = i; // 클로저 문제 방지
                backgroundSelectButtons[i].onClick.AddListener(() => OnBackgroundSelected(index));
            }
        }

        // 각 애니메이션 선택 버튼 이벤트 설정
        for (int i = 0; i < animationSelectButtons.Length; i++)
        {
            if (animationSelectButtons[i] != null)
            {
                int index = i; // 클로저 문제 방지
                animationSelectButtons[i].onClick.AddListener(() => OnAnimationSelected(index));
            }
        }

        // 현재 선택된 배경에 맞춰 버튼 색상 업데이트
        UpdateBackgroundButtonColors();
        // 현재 선택된 애니메이션에 맞춰 버튼 색상 업데이트
        UpdateAnimationButtonColors();
    }

    /// <summary>
    /// 배경 카테고리 버튼 클릭 시 호출
    /// </summary>
    void OnBackgroundCategorySelected()
    {   
        // 배경, 애니메이션 패널 활성화/비활성화 토글
        bool isActive = !backgroundPanel.activeSelf;
        backgroundPanel.SetActive(isActive);
        animationPanel.SetActive(isActive);

        // 메인 카테고리 버튼 색상 업데이트
        UpdateMainCategoryButtonColors(backgroundButton);

        Debug.Log($"배경 카테고리 선택됨. 패널 활성화: {isActive}");
    }

    /// <summary>
    /// 특정 배경 선택 시 호출
    /// </summary>
    void OnBackgroundSelected(int backgroundIndex)
    {
        if (BackgroundManager.Instance != null)
        {
            BackgroundManager.Instance.SwitchBackground(backgroundIndex);
            UpdateBackgroundButtonColors();
            Debug.Log($"배경 {backgroundIndex} 선택됨");
        }
        else
        {
            Debug.LogError("BackgroundManager Instance를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 배경 버튼 색상 업데이트
    /// </summary>
    void UpdateBackgroundButtonColors()
    {
        if (BackgroundManager.Instance == null || UIThemeManager.Instance == null) return;

        int currentBg = BackgroundManager.Instance.GetCurrentBackgroundIndex();
        Button selectedButton = (currentBg < backgroundSelectButtons.Length) ? backgroundSelectButtons[currentBg] : null;
        
        // UIThemeManager를 통해 배경 버튼 선택 상태 관리
        UIThemeManager.Instance.SetBackgroundButtonSelected(selectedButton, backgroundSelectButtons);
    }

    /// <summary>
    /// 메인 카테고리 버튼 색상 업데이트
    /// </summary>
    void UpdateMainCategoryButtonColors(Button selectedButton)
    {
        // UIThemeManager를 통해 버튼 선택 상태 관리
        if (UIThemeManager.Instance != null)
        {
            UIThemeManager.Instance.SetMainCategoryButtonSelected(selectedButton);
        }
        
        currentSelectedMainButton = selectedButton;
    }

    /// <summary>
    /// 특정 애니메이션 선택 시 호출
    /// </summary>
    void OnAnimationSelected(int animationIndex)
    {
        if (AnimationManager.Instance != null)
        {
            AnimationManager.Instance.SetAnimation(animationIndex);
            UpdateAnimationButtonColors();
            Debug.Log($"애니메이션 {animationIndex} 선택됨");
        }
        else
        {
            Debug.LogError("AnimationManager Instance를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 애니메이션 버튼 색상 업데이트
    /// </summary>
    void UpdateAnimationButtonColors()
    {
        if (AnimationManager.Instance == null || UIThemeManager.Instance == null) return;

        int currentAnimation = AnimationManager.Instance.GetCurrentAnimationIndex();
        Button selectedButton = (currentAnimation < animationSelectButtons.Length) ? animationSelectButtons[currentAnimation] : null;
        
        // UIThemeManager를 통해 애니메이션 버튼 선택 상태 관리
        UIThemeManager.Instance.SetAnimationButtonSelected(selectedButton, animationSelectButtons);
    }

    #endregion
    
    /// <summary>
    /// 배경/테마 변경 시 모든 버튼 색상을 업데이트
    /// </summary>
    public void UpdateAllButtonColorsForTheme()
    {
        UpdateBackgroundButtonColors();
        UpdateAnimationButtonColors();
        
        // UIThemeManager가 이미 선택된 버튼들의 색상을 업데이트하므로 
        // 여기서는 추가 작업이 필요없음
    }
    
    /// <summary>
    /// LocalStorage에서 토큰 추출
    /// </summary>
    private void ExtractTokenFromLocalStorage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            string token = GetTokenFromLocalStorage();
            
            if (!string.IsNullOrEmpty(token))
            {
                authToken = token;
                Debug.Log("WardrobeManager: LocalStorage에서 토큰 추출 성공");
            }
            else
            {
                // LocalStorage에 토큰이 없으면 PlayerPrefs에서 시도
                authToken = PlayerPrefs.GetString("AuthToken", "");
                if (!string.IsNullOrEmpty(authToken))
                {
                    Debug.Log("WardrobeManager: PlayerPrefs에서 저장된 토큰 사용");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("WardrobeManager: LocalStorage에서 토큰 추출 중 오류: " + e.Message);
            // 오류 시 저장된 토큰 시도
            authToken = PlayerPrefs.GetString("AuthToken", "");
        }
#else
        // 에디터나 다른 플랫폼에서는 저장된 토큰 사용
        authToken = PlayerPrefs.GetString("AuthToken", "");
        Debug.Log("WardrobeManager: 에디터 모드: 저장된 토큰 사용");
#endif
    }
    
    /// <summary>
    /// 현재 토큰 상태 확인
    /// </summary>
    public string GetCurrentToken()
    {
        return authToken;
    }

}
