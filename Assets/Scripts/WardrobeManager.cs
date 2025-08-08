using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

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
        // 옷 입히기 기능 제거됨
    }

    public void UnequipAllOutfits()
    {
        Debug.Log("전체 옷 해제 버튼이 눌렸습니다.");
    }

    public void UnequipOutfitByCategory(ClothingType type)
    {
        Debug.Log($"{type} 카테고리 옷 해제 버튼이 눌렸습니다.");
    }
}
