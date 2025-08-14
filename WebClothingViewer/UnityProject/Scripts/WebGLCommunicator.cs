using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLCommunicator : MonoBehaviour
{
    [Header("옷 모델 컨트롤러")]
    public ClothingModel clothingModel;
    
    [Header("카메라 설정")]
    public Camera mainCamera;
    public Transform cameraTarget; // 카메라가 바라볼 타겟
    
    public static WebGLCommunicator Instance { get; private set; }
    
    // WebGL에서 JavaScript로 메시지 전송을 위한 외부 함수
    [DllImport("__Internal")]
    private static extern void SendMessageToWeb(string message);
    
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
        // 초기 설정
        SetupCamera();
        
        // 웹에 Unity 준비 완료 신호 전송
        SendToWeb("unityReady");
    }
    
    /// <summary>
    /// 웹에서 호출할 옷 변경 메서드 (JavaScript에서 직접 호출)
    /// </summary>
    public void ChangeClothingFromWeb(string clothingId)
    {
        Debug.Log($"웹에서 옷 변경 요청 받음: {clothingId}");
        
        if (clothingModel != null)
        {
            clothingModel.ChangeClothing(clothingId);
            
            // 옷 변경 완료를 웹에 알림
            SendToWeb($"clothingChanged:{clothingId}");
        }
        else
        {
            Debug.LogError("ClothingModel이 설정되지 않았습니다!");
            SendToWeb("error:ClothingModel not found");
        }
    }
    
    /// <summary>
    /// 웹에서 호출할 모델 숨기기 메서드
    /// </summary>
    public void HideModelFromWeb()
    {
        if (clothingModel != null)
        {
            clothingModel.HideModel();
            SendToWeb("modelHidden");
        }
    }
    
    /// <summary>
    /// 웹에서 호출할 모델 보이기 메서드
    /// </summary>
    public void ShowModelFromWeb()
    {
        if (clothingModel != null)
        {
            clothingModel.ShowModel();
            SendToWeb("modelShown");
        }
    }
    
    /// <summary>
    /// 웹에서 호출할 현재 옷 정보 요청 메서드
    /// </summary>
    public void GetCurrentClothingInfo()
    {
        if (clothingModel != null)
        {
            string info = clothingModel.GetCurrentClothingInfo();
            SendToWeb($"currentClothingInfo:{info}");
        }
    }
    
    /// <summary>
    /// 카메라 설정
    /// </summary>
    private void SetupCamera()
    {
        if (mainCamera != null && cameraTarget != null)
        {
            // 카메라가 타겟을 바라보도록 설정
            mainCamera.transform.LookAt(cameraTarget);
            
            // 적절한 거리로 카메라 위치 조정
            Vector3 direction = (mainCamera.transform.position - cameraTarget.position).normalized;
            mainCamera.transform.position = cameraTarget.position + direction * 3f;
        }
    }
    
    /// <summary>
    /// 웹으로 메시지 전송
    /// </summary>
    private void SendToWeb(string message)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            SendMessageToWeb(message);
            Debug.Log($"웹으로 메시지 전송: {message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"웹 메시지 전송 실패: {e.Message}");
        }
        #else
        Debug.Log($"[에디터 모드] 웹으로 전송할 메시지: {message}");
        #endif
    }
    
    /// <summary>
    /// 카메라 각도 조정 (웹에서 호출 가능)
    /// </summary>
    public void RotateCamera(float x, float y)
    {
        if (mainCamera != null && cameraTarget != null)
        {
            // 타겟 주변으로 카메라 회전
            mainCamera.transform.RotateAround(cameraTarget.position, Vector3.up, x);
            mainCamera.transform.RotateAround(cameraTarget.position, mainCamera.transform.right, y);
            
            SendToWeb($"cameraRotated:{x},{y}");
        }
    }
    
    /// <summary>
    /// 카메라 줌 조정 (웹에서 호출 가능)
    /// </summary>
    public void ZoomCamera(float delta)
    {
        if (mainCamera != null && cameraTarget != null)
        {
            Vector3 direction = (mainCamera.transform.position - cameraTarget.position).normalized;
            float newDistance = Vector3.Distance(mainCamera.transform.position, cameraTarget.position) - delta;
            
            // 줌 범위 제한
            newDistance = Mathf.Clamp(newDistance, 1f, 10f);
            
            mainCamera.transform.position = cameraTarget.position + direction * newDistance;
            
            SendToWeb($"cameraZoomed:{newDistance}");
        }
    }
}