using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("배경 설정")]
    [Tooltip("배경 GameObject들을 순서대로 배치하세요")]
    public GameObject[] backgrounds = new GameObject[3]; // 기본, 배경1, 배경2
    
    [Header("배경 정보")]
    [Tooltip("각 배경의 이름 (UI 표시용)")]
    public string[] backgroundNames = { "기본 배경", "배경 1", "배경 2" };
    
    private int currentBackgroundIndex = 0; // 현재 활성화된 배경 인덱스
    
    // 싱글톤 패턴
    public static BackgroundManager Instance { get; private set; }
    
    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeBackgrounds();
    }
    
    /// <summary>
    /// 배경 초기화 - 첫 번째 배경만 활성화, 나머지는 비활성화
    /// </summary>
    void InitializeBackgrounds()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null)
            {
                backgrounds[i].SetActive(i == 0); // 첫 번째 배경만 활성화
            }
        }
        
        currentBackgroundIndex = 0;
        Debug.Log($"배경 초기화 완료. 현재 배경: {backgroundNames[currentBackgroundIndex]}");
    }
    
    /// <summary>
    /// 배경 전환 (즉시 전환 방식)
    /// </summary>
    /// <param name="newBackgroundIndex">전환할 배경 인덱스 (0~2)</param>
    public void SwitchBackground(int newBackgroundIndex)
    {
        // 유효성 검사
        if (newBackgroundIndex < 0 || newBackgroundIndex >= backgrounds.Length)
        {
            Debug.LogWarning($"잘못된 배경 인덱스: {newBackgroundIndex}");
            return;
        }
        
        if (backgrounds[newBackgroundIndex] == null)
        {
            Debug.LogWarning($"배경 {newBackgroundIndex}가 할당되지 않았습니다.");
            return;
        }
        
        if (newBackgroundIndex == currentBackgroundIndex)
        {
            Debug.Log("이미 선택된 배경입니다.");
            return;
        }
        
        // 현재 배경 비활성화
        if (backgrounds[currentBackgroundIndex] != null)
        {
            backgrounds[currentBackgroundIndex].SetActive(false);
        }
        
        // 새 배경 활성화
        backgrounds[newBackgroundIndex].SetActive(true);
        
        // 현재 배경 인덱스 업데이트
        currentBackgroundIndex = newBackgroundIndex;
        
        Debug.Log($"배경 전환 완료: {backgroundNames[currentBackgroundIndex]}");
    }
    
    /// <summary>
    /// 현재 배경 인덱스 반환
    /// </summary>
    public int GetCurrentBackgroundIndex()
    {
        return currentBackgroundIndex;
    }
    
    /// <summary>
    /// 현재 배경 이름 반환
    /// </summary>
    public string GetCurrentBackgroundName()
    {
        return backgroundNames[currentBackgroundIndex];
    }
    
    /// <summary>
    /// 다음 배경으로 순환 전환
    /// </summary>
    public void SwitchToNextBackground()
    {
        int nextIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
        SwitchBackground(nextIndex);
    }
    
    /// <summary>
    /// 이전 배경으로 순환 전환
    /// </summary>
    public void SwitchToPreviousBackground()
    {
        int previousIndex = (currentBackgroundIndex - 1 + backgrounds.Length) % backgrounds.Length;
        SwitchBackground(previousIndex);
    }
}