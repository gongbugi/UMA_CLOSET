using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;
using UMA;
using UMA.CharacterSystem;

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

    [Header("Button Colors")]
    public Color selectedColor = new Color(1f, 0.9f, 0.6f); // Light Orange
    public Color normalColor = Color.white;

    private Button currentSelectedMainButton;
    private Button currentSelectedSubButton;
    private List<OutfitData> currentFilteredList;
    private UMA.CharacterSystem.DynamicCharacterAvatar avatar;

    void Awake()
    {
        allOutfits = new List<OutfitData>();
        
        // OutfitDataLoader의 이벤트에 구독
        if (OutfitDataLoader.Instance != null)
        {
            OutfitDataLoader.Instance.OnOutfitsLoaded += OnOutfitsLoadedFromJson;
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

        // JSON 데이터 로드 시작
        if (OutfitDataLoader.Instance != null)
        {
            OutfitDataLoader.Instance.LoadOutfits();
        }
        else
        {
            Debug.LogError("OutfitDataLoader Instance가 없습니다. OutfitDataLoader를 씬에 추가해주세요.");
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
    }

    private void SelectMainCategory(Button selectedButton)
    {
        if (currentSelectedMainButton == selectedButton) return;

        if (currentSelectedMainButton != null)
        {
            currentSelectedMainButton.GetComponent<Image>().color = normalColor;
        }
        selectedButton.GetComponent<Image>().color = selectedColor;
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

        if (currentSelectedSubButton != null)
        {
            currentSelectedSubButton.GetComponent<Image>().color = normalColor;
        }

        if (selectedButton != null)
        {
            selectedButton.GetComponent<Image>().color = selectedColor;
            currentSelectedSubButton = selectedButton;
        }
        else
        {
            currentSelectedSubButton = null;
        }
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

        // 3. 최종적으로 수정이 완료된 레시피를 아바타에 적용하고 빌드
        avatar.BuildCharacter();
        Debug.Log($"'{selectedOutfit.outfitName}' 착용 완료!");
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

        // 여기에 다른 의상 종류에 대한 규칙을 추가할 수 있습니다.
        // 예: if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Pants)
        //     return "Custom/Pants_Recipe";

        return null; // 규칙에 맞는 레시피가 없는 경우
    }

    private string GetOverlayPathByConvention(OutfitData outfit)
    {
        if (outfit.type == ClothingType.Top && outfit.topCategory == TopCategory.Tshirt)
        {
            return "Custom/Tshirt_Overlay";
        }

        // 여기에 다른 의상 종류에 대한 규칙을 추가할 수 있습니다.
        // 예: if (outfit.type == ClothingType.Bottom && outfit.bottomCategory == BottomCategory.Pants)
        //     return "Custom/Pants_Overlay";

        return null; // 규칙에 맞는 레시피가 없는 경우
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

}
