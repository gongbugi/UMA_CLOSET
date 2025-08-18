using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

[Serializable]
public class BodyMeasurementData
{
    public float uma_height;
    public float uma_belly;
    public float uma_waist;
    public float uma_width;
    public float uma_fore_arm;
    public float uma_arm;
    public float uma_legs;
}

public class BodyMeasurements : MonoBehaviour
{
    [Header("설정")]
    public string jsonFileName = "body_measurements.json";
    public bool useLocalFile = true;
    public string serverUrl = "https://yourserver.com/api/measurements";

    public static BodyMeasurements Instance { get; private set; }
    
    private BodyMeasurementData currentMeasurements;
    private string authToken = "";
    
    public event Action<BodyMeasurementData> OnMeasurementsLoaded;

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
                Debug.Log("BodyMeasurements: LocalStorage에서 토큰 추출 성공");
                
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
                    Debug.Log("BodyMeasurements: PlayerPrefs에서 저장된 토큰 사용");
                }
                else
                {
                    Debug.LogWarning("BodyMeasurements: 토큰을 찾을 수 없습니다.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("BodyMeasurements: LocalStorage에서 토큰 추출 중 오류: " + e.Message);
            // 오류 시 저장된 토큰 시도
            authToken = PlayerPrefs.GetString("AuthToken", "");
        }
#else
        // 에디터나 다른 플랫폼에서는 저장된 토큰 사용
        authToken = PlayerPrefs.GetString("AuthToken", "");
        Debug.Log("BodyMeasurements: 에디터 모드: 저장된 토큰 사용");
#endif
    }

    public void LoadMeasurements()
    {
        if (useLocalFile)
        {
            StartCoroutine(LoadMeasurementsFromLocalFile());
        }
        else
        {
            StartCoroutine(LoadMeasurementsFromServer());
        }
    }

    private IEnumerator LoadMeasurementsFromLocalFile()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, jsonFileName);
        
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonContent = request.downloadHandler.text;
            ProcessMeasurementData(jsonContent);
        }
        else
        {
            Debug.LogError($"신체 측정 JSON 파일 로드 실패: {request.error}");
            Debug.LogError($"파일 경로: {filePath}");
        }

        request.Dispose();
    }

    private IEnumerator LoadMeasurementsFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        
        // 토큰이 있으면 Authorization 헤더에 추가
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
            Debug.Log("BodyMeasurements: Authorization 헤더 추가됨");
        }
        else
        {
            Debug.LogWarning("BodyMeasurements: 토큰이 없습니다. 401 오류가 발생할 수 있습니다.");
        }
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonContent = request.downloadHandler.text;
            Debug.Log("BodyMeasurements: 서버에서 데이터 로드 성공");
            ProcessMeasurementData(jsonContent);
        }
        else
        {
            string error = $"서버에서 신체 측정 데이터 로드 실패: {request.error}";
            
            // 401 오류인 경우 특별 처리
            if (request.responseCode == 401)
            {
                error += "\n토큰이 유효하지 않거나 만료되었습니다.";
            }
            
            Debug.LogError(error);
            Debug.LogError($"BodyMeasurements: 응답 코드: {request.responseCode}");
        }

        request.Dispose();
    }

    private void ProcessMeasurementData(string jsonContent)
    {
        try
        {
            currentMeasurements = JsonUtility.FromJson<BodyMeasurementData>(jsonContent);
            Debug.Log("신체 측정 데이터 로드 완료");
            Debug.Log($"Height: {currentMeasurements.uma_height}, Belly: {currentMeasurements.uma_belly}, Waist: {currentMeasurements.uma_waist}");
            
            OnMeasurementsLoaded?.Invoke(currentMeasurements);
        }
        catch (Exception e)
        {
            Debug.LogError($"신체 측정 JSON 파싱 오류: {e.Message}");
        }
    }

    /// <summary>
    /// 현재 로드된 신체 측정 데이터를 UMA DNA 값들로 변환해서 반환
    /// JSON 값들이 이미 0-1 범위라고 가정
    /// </summary>
    public Dictionary<string, float> GetUMADNAValues()
    {
        if (currentMeasurements == null)
        {
            Debug.LogWarning("신체 측정 데이터가 로드되지 않았습니다.");
            return null;
        }

        Dictionary<string, float> dnaValues = new Dictionary<string, float>();
        
        // uma_height -> other의 Height
        dnaValues["height"] = currentMeasurements.uma_height;
        
        // uma_width -> Arm의 Width (기존 shoulder_width)
        dnaValues["ArmWidth"] = currentMeasurements.uma_width;
        
        // uma_waist -> other의 Waist
        dnaValues["Waist"] = currentMeasurements.uma_waist;
        
        // uma_belly -> other의 Belly
        dnaValues["Belly"] = currentMeasurements.uma_belly;
        
        // uma_fore_arm -> Forearm의 Length
        dnaValues["ForearmLength"] = currentMeasurements.uma_fore_arm;
        
        // uma_arm -> Arm의 Length
        dnaValues["ArmLength"] = currentMeasurements.uma_arm;
        
        // uma_legs -> Legs의 Size
        dnaValues["LegsSize"] = currentMeasurements.uma_legs;

        Debug.Log("UMA DNA 값 변환 완료:");
        foreach (var kvp in dnaValues)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value:F3}");
        }

        return dnaValues;
    }

    public BodyMeasurementData GetCurrentMeasurements()
    {
        return currentMeasurements;
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
        
        Debug.Log("BodyMeasurements: 토큰이 수동으로 설정되었습니다.");
    }
}