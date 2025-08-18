using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

[Serializable]
public class OutfitJsonData
{
    public string id;
    public string outfitName;
    public string type; // "Top" or "Bottom"
    public string topCategory; // "Tshirt" or "Shirt"
    public string bottomCategory; // "Pants" or "Shorts"
    public string slotName;
    public string overlayName;
    public string thumbnailUrl;
    public string raceName;
    public string texturePath; // 텍스처 경로
}

[Serializable]
public class OutfitListWrapper
{
    public List<OutfitJsonData> outfits;
}

public class OutfitDataLoader : MonoBehaviour
{
    [Header("설정")]
    public string jsonFileName = "outfits.json";
    public bool useLocalFile = true; // 로컬 파일 사용 여부
    public string serverUrl = "https://yourserver.com/api/outfits"; // 서버 URL (나중에 사용)

    public static OutfitDataLoader Instance { get; private set; }
    
    private List<OutfitData> loadedOutfits = new List<OutfitData>();
    private Dictionary<string, Sprite> thumbnailCache = new Dictionary<string, Sprite>();
    private string authToken = "";

    public event Action<List<OutfitData>> OnOutfitsLoaded;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetTokenFromLocalStorage();
    
    [DllImport("__Internal")]
    private static extern void SetTokenToLocalStorage(string token);
    
    [DllImport("__Internal")]
    private static extern void RemoveTokenFromLocalStorage();
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // LocalStorage에서 토큰 추출
        ExtractTokenFromLocalStorage();
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
                Debug.Log("LocalStorage에서 토큰 추출 성공");
                
                // 토큰을 PlayerPrefs에도 저장 (백업용)
                PlayerPrefs.SetString("AuthToken", authToken);
                PlayerPrefs.Save();
            }
            else
            {
                // LocalStorage에 토큰이 없으면 PlayerPrefs에서 시도
                authToken = PlayerPrefs.GetString("AuthToken", "");
                if (!string.IsNullOrEmpty(authToken))
                {
                    Debug.Log("PlayerPrefs에서 저장된 토큰 사용");
                }
                else
                {
                    Debug.LogWarning("토큰을 찾을 수 없습니다. 로그인 페이지에서 토큰이 LocalStorage에 저장되었는지 확인해주세요.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("LocalStorage에서 토큰 추출 중 오류: " + e.Message);
            // 오류 시 저장된 토큰 시도
            authToken = PlayerPrefs.GetString("AuthToken", "");
        }
#else
        // 에디터나 다른 플랫폼에서는 저장된 토큰 사용
        authToken = PlayerPrefs.GetString("AuthToken", "");
        Debug.Log("에디터 모드: 저장된 토큰 사용");
#endif
    }

    public void LoadOutfits()
    {
        if (useLocalFile)
        {
            StartCoroutine(LoadOutfitsFromLocalFile());
        }
        else
        {
            StartCoroutine(LoadOutfitsFromServer());
        }
    }

    private IEnumerator LoadOutfitsFromLocalFile()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, jsonFileName);
        
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonContent = request.downloadHandler.text;
            ProcessJsonData(jsonContent);
        }
        else
        {
            Debug.LogError($"로컬 JSON 파일 로드 실패: {request.error}");
            Debug.LogError($"파일 경로: {filePath}");
        }

        request.Dispose();
    }

    private IEnumerator LoadOutfitsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        
        // 토큰이 있으면 Authorization 헤더에 추가
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            Debug.Log("Authorization 헤더 추가됨");
        }
        else
        {
            Debug.LogWarning("토큰이 없습니다. 401 오류가 발생할 수 있습니다.");
        }
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonContent = request.downloadHandler.text;
            Debug.Log("서버에서 데이터 로드 성공");
            ProcessJsonData(jsonContent);
        }
        else
        {
            string error = $"서버에서 JSON 로드 실패: {request.error}";
            
            // 401 오류인 경우 특별 처리
            if (request.responseCode == 401)
            {
                error += "\n토큰이 유효하지 않거나 만료되었습니다.";
            }
            
            Debug.LogError(error);
            Debug.LogError($"응답 코드: {request.responseCode}");
        }

        request.Dispose();
    }

    private void ProcessJsonData(string jsonContent)
    {
        try
        {
            OutfitListWrapper wrapper = JsonUtility.FromJson<OutfitListWrapper>(jsonContent);
            loadedOutfits.Clear();

            foreach (OutfitJsonData jsonData in wrapper.outfits)
            {
                OutfitData outfitData = ConvertJsonToOutfitData(jsonData);
                loadedOutfits.Add(outfitData);
            }

            Debug.Log($"JSON에서 {loadedOutfits.Count}개의 의상 로드 완료");
            OnOutfitsLoaded?.Invoke(loadedOutfits);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파싱 오류: {e.Message}");
        }
    }

    private OutfitData ConvertJsonToOutfitData(OutfitJsonData jsonData)
    {
        OutfitData outfitData = ScriptableObject.CreateInstance<OutfitData>();
        
        outfitData.outfitName = jsonData.outfitName;
        outfitData.slotName = jsonData.slotName;
        outfitData.overlayName = jsonData.overlayName;
        outfitData.raceName = jsonData.raceName;
        outfitData.texturePath = jsonData.texturePath;
        
        // ClothingType enum 변환
        if (Enum.TryParse<ClothingType>(jsonData.type, out ClothingType clothingType))
        {
            outfitData.type = clothingType;
        }
        
        // TopCategory enum 변환
        if (!string.IsNullOrEmpty(jsonData.topCategory) && 
            Enum.TryParse<TopCategory>(jsonData.topCategory, out TopCategory topCategory))
        {
            outfitData.topCategory = topCategory;
        }
        
        // BottomCategory enum 변환
        if (!string.IsNullOrEmpty(jsonData.bottomCategory) && 
            Enum.TryParse<BottomCategory>(jsonData.bottomCategory, out BottomCategory bottomCategory))
        {
            outfitData.bottomCategory = bottomCategory;
        }

        // 썸네일 이미지 로드 (비동기)
        StartCoroutine(LoadThumbnail(jsonData.thumbnailUrl, outfitData));

        return outfitData;
    }

    private IEnumerator LoadThumbnail(string path, OutfitData outfitData)
    {
        // 이미 캐시된 이미지가 있는지 확인
        if (thumbnailCache.ContainsKey(path))
        {
            outfitData.thumbnail = thumbnailCache[path];
            yield break;
        }

        // 경로가 비어있거나 예시 URL인 경우 기본 처리
        if (string.IsNullOrEmpty(path) || path.StartsWith("https://example.com"))
        {
            outfitData.thumbnail = CreateDefaultThumbnail();
            yield break;
        }

        string fileUrl;
        // 절대 경로인지 확인하여 file:// 프로토콜 추가
        if (System.IO.Path.IsPathRooted(path))
        {
            fileUrl = "file://" + path.Replace("\\", "/");
        }
        else
        {
            fileUrl = path; // HTTP URL이거나 상대경로
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fileUrl);
        
        // HTTP URL인 경우 토큰이 있으면 Authorization 헤더에 추가
        if (!fileUrl.StartsWith("file://") && !string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            
            outfitData.thumbnail = sprite;
            thumbnailCache[path] = sprite;
            Debug.Log($"썸네일 로드 성공: {path}");
        }
        else
        {
            Debug.LogWarning($"썸네일 로드 실패: {path} - {request.error}");
            outfitData.thumbnail = CreateDefaultThumbnail();
        }

        request.Dispose();
    }

    private Sprite CreateDefaultThumbnail()
    {
        // 기본 썸네일 생성 (단색 정사각형)
        Texture2D texture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(0.7f, 0.7f, 0.7f, 1f); // 회색
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.one * 0.5f);
    }

    public List<OutfitData> GetLoadedOutfits()
    {
        return loadedOutfits;
    }
    
    /// <summary>
    /// 현재 토큰 상태 확인
    /// </summary>
    public string GetCurrentToken()
    {
        return authToken;
    }
    
    /// <summary>
    /// 외부에서 토큰 설정 (테스트용)
    /// </summary>
    public void SetAuthToken(string token)
    {
        authToken = token;
        PlayerPrefs.SetString("AuthToken", authToken);
        PlayerPrefs.Save();
        
#if UNITY_WEBGL && !UNITY_EDITOR
        // LocalStorage에도 저장
        SetTokenToLocalStorage(token);
#endif
        
        Debug.Log("토큰이 수동으로 설정되었습니다.");
    }
}