using UnityEngine;

// 기본 분류: 상의, 하의
public enum ClothingType
{
    Top,
    Bottom
}

// 상의 상세 분류
public enum TopCategory
{
    Tshirt, // 티셔츠 (반팔)
    Shirt   // 셔츠 (긴팔)
}

// 하의 상세 분류
public enum BottomCategory
{
    Pants,  // 긴바지
    Shorts  // 반바지
}


[CreateAssetMenu(fileName = "NewOutfitData", menuName = "UMA Closet/Outfit Data")]
public class OutfitData : ScriptableObject
{
    [Header("Common")]
    public string outfitName; // 의상 이름 (UI에 표시될 이름)
    public ClothingType type; // 의상 타입 (상의/하의)
    public Sprite thumbnail; // 의상 썸네일 이미지

    [Header("UMA Recipe")]
    public string texturePath; // Resources 폴더 기준 텍스처 경로
    public string overlayName; // 텍스처를 적용할 오버레이 이름
    public string slotName; // 예: "Torso", "Legs"
    public string raceName; // 예: "HumanMale", "HumanFemale" (필요한 경우)


    [Header("Categories")]
    // 상의일 경우 사용
    [Tooltip("의상 타입이 Top일 경우에만 설정하세요.")]
    public TopCategory topCategory;

    // 하의일 경우 사용
    [Tooltip("의상 타입이 Bottom일 경우에만 설정하세요.")]
    public BottomCategory bottomCategory;
}
