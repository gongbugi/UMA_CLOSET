using UnityEngine;

[System.Serializable]
public class ClothingData
{
    public string id;
    public string name;
    public string type; // "shirt", "pants", "shoes" 등
    public string modelPath; // Resources 폴더 내 모델 경로
    public string texturePath; // Resources 폴더 내 텍스처 경로
}

public class ClothingModel : MonoBehaviour
{
    [Header("현재 표시중인 옷 정보")]
    public ClothingData currentClothing;
    
    [Header("3D 모델 설정")]
    public Transform modelParent; // 3D 모델이 생성될 부모 Transform
    public Vector3 modelRotation = Vector3.zero;
    public Vector3 modelScale = Vector3.one;
    
    private GameObject currentModelObject;
    private Renderer currentRenderer;
    
    void Start()
    {
        // 초기 상태에서는 아무 옷도 표시하지 않음
        HideModel();
    }
    
    /// <summary>
    /// 웹에서 호출될 옷 변경 메서드
    /// </summary>
    public void ChangeClothing(string clothingId)
    {
        Debug.Log($"옷 변경 요청: {clothingId}");
        
        // 기존 모델 제거
        if (currentModelObject != null)
        {
            DestroyImmediate(currentModelObject);
        }
        
        // 새 옷 데이터 로드
        ClothingData clothingData = LoadClothingData(clothingId);
        if (clothingData == null)
        {
            Debug.LogError($"옷 데이터를 찾을 수 없습니다: {clothingId}");
            return;
        }
        
        currentClothing = clothingData;
        
        // 3D 모델 로드 및 표시
        LoadAndDisplayModel();
    }
    
    /// <summary>
    /// 옷 데이터 로드 (실제로는 JSON이나 ScriptableObject에서 로드)
    /// </summary>
    private ClothingData LoadClothingData(string clothingId)
    {
        // 임시 하드코딩된 데이터 (나중에 JSON 파일이나 DB로 교체)
        switch (clothingId)
        {
            case "tshirt_001":
                return new ClothingData
                {
                    id = "tshirt_001",
                    name = "기본 티셔츠",
                    type = "shirt",
                    modelPath = "Models/Tshirt_Model",
                    texturePath = "Textures/Tshirt_001"
                };
            case "pants_001":
                return new ClothingData
                {
                    id = "pants_001",
                    name = "청바지",
                    type = "pants",
                    modelPath = "Models/Pants_Model",
                    texturePath = "Textures/Pants_001"
                };
            case "shoes_001":
                return new ClothingData
                {
                    id = "shoes_001",
                    name = "운동화",
                    type = "shoes",
                    modelPath = "Models/Shoes_Model",
                    texturePath = "Textures/Shoes_001"
                };
            default:
                return null;
        }
    }
    
    /// <summary>
    /// 3D 모델 로드 및 표시
    /// </summary>
    private void LoadAndDisplayModel()
    {
        if (currentClothing == null) return;
        
        // 3D 모델 로드
        GameObject modelPrefab = Resources.Load<GameObject>(currentClothing.modelPath);
        if (modelPrefab == null)
        {
            Debug.LogError($"3D 모델을 찾을 수 없습니다: {currentClothing.modelPath}");
            return;
        }
        
        // 모델 인스턴스 생성
        currentModelObject = Instantiate(modelPrefab, modelParent);
        currentModelObject.transform.localPosition = Vector3.zero;
        currentModelObject.transform.localRotation = Quaternion.Euler(modelRotation);
        currentModelObject.transform.localScale = modelScale;
        
        // 렌더러 설정
        currentRenderer = currentModelObject.GetComponentInChildren<Renderer>();
        
        // 텍스처 적용
        ApplyTexture();
        
        Debug.Log($"옷 모델 표시 완료: {currentClothing.name}");
    }
    
    /// <summary>
    /// 텍스처 적용
    /// </summary>
    private void ApplyTexture()
    {
        if (currentRenderer == null || string.IsNullOrEmpty(currentClothing.texturePath)) return;
        
        Texture2D texture = Resources.Load<Texture2D>(currentClothing.texturePath);
        if (texture != null)
        {
            currentRenderer.material.mainTexture = texture;
            Debug.Log($"텍스처 적용 완료: {currentClothing.texturePath}");
        }
        else
        {
            Debug.LogWarning($"텍스처를 찾을 수 없습니다: {currentClothing.texturePath}");
        }
    }
    
    /// <summary>
    /// 모델 숨기기
    /// </summary>
    public void HideModel()
    {
        if (currentModelObject != null)
        {
            currentModelObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 모델 보이기
    /// </summary>
    public void ShowModel()
    {
        if (currentModelObject != null)
        {
            currentModelObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 현재 표시중인 옷 정보 반환
    /// </summary>
    public string GetCurrentClothingInfo()
    {
        if (currentClothing != null)
        {
            return $"{currentClothing.name} ({currentClothing.id})";
        }
        return "표시중인 옷이 없습니다.";
    }
}