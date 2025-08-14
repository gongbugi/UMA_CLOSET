using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

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
    
    public event Action<BodyMeasurementData> OnMeasurementsLoaded;

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
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonContent = request.downloadHandler.text;
            ProcessMeasurementData(jsonContent);
        }
        else
        {
            Debug.LogError($"서버에서 신체 측정 데이터 로드 실패: {request.error}");
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
}