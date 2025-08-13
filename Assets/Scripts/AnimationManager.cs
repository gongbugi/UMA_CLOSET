using UnityEngine;
using UMA.CharacterSystem;

public class AnimationManager : MonoBehaviour
{
    [Header("애니메이션 컨트롤러")]
    [Tooltip("기본 애니메이션 컨트롤러")]
    public RuntimeAnimatorController locomotionController;
    
    [Tooltip("걷기 애니메이션 컨트롤러")]
    public RuntimeAnimatorController walkingController;
    
    [Tooltip("뛰기 애니메이션 컨트롤러")]
    public RuntimeAnimatorController runningController;

    [Header("설정")]
    [Tooltip("UMA 아바타")]
    public DynamicCharacterAvatar avatar;
    
    private Animator avatarAnimator;
    private int currentAnimationIndex = 0; // 0: 기본, 1: 걷기, 2: 뛰기
    
    // 싱글톤 패턴
    public static AnimationManager Instance { get; private set; }
    
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
        // UMA 빌드 완료를 기다린 후 초기화
        StartCoroutine(WaitForUMAAndInitialize());
    }
    
    /// <summary>
    /// UMA 아바타 빌드 완료를 기다린 후 초기화
    /// </summary>
    System.Collections.IEnumerator WaitForUMAAndInitialize()
    {
        // UMA 아바타 찾기
        while (avatar == null)
        {
            avatar = FindFirstObjectByType<DynamicCharacterAvatar>();
            yield return new WaitForSeconds(0.1f);
        }
        
        // Animator 컴포넌트가 생성될 때까지 대기
        while (avatar.GetComponent<Animator>() == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // 초기화 실행
        InitializeAnimation();
    }
    
    /// <summary>
    /// 애니메이션 시스템 초기화
    /// </summary>
    void InitializeAnimation()
    {
        // UMA 아바타 찾기
        if (avatar == null)
        {
            avatar = FindFirstObjectByType<DynamicCharacterAvatar>();
            if (avatar == null)
            {
                Debug.LogWarning("씬에서 DynamicCharacterAvatar를 찾을 수 없습니다!");
                return;
            }
        }
        
        // Animator 컴포넌트 가져오기
        avatarAnimator = avatar.GetComponent<Animator>();
        if (avatarAnimator == null)
        {
            Debug.LogWarning("DynamicCharacterAvatar에 Animator 컴포넌트가 없습니다! UMA 빌드가 완료되지 않았을 수 있습니다.");
            return;
        }
        
        // Resources에서 애니메이션 컨트롤러 로드
        LoadAnimationControllers();
        
        Debug.Log("AnimationManager 초기화 완료");
    }
    
    /// <summary>
    /// Resources 폴더에서 애니메이션 컨트롤러들을 로드
    /// </summary>
    void LoadAnimationControllers()
    {
        if (locomotionController == null)
        {
            locomotionController = Resources.Load<RuntimeAnimatorController>("Locomotion");
            if (locomotionController == null)
                Debug.LogWarning("Locomotion.controller를 찾을 수 없습니다!");
        }
        
        if (walkingController == null)
        {
            walkingController = Resources.Load<RuntimeAnimatorController>("Walking");
            if (walkingController == null)
                Debug.LogWarning("Walking.controller를 찾을 수 없습니다!");
        }
        
        if (runningController == null)
        {
            runningController = Resources.Load<RuntimeAnimatorController>("Running");
            if (runningController == null)
                Debug.LogWarning("Running.controller를 찾을 수 없습니다!");
        }
    }
    
    /// <summary>
    /// 애니메이션 변경
    /// </summary>
    /// <param name="animationIndex">0: 기본, 1: 걷기, 2: 뛰기</param>
    public void SetAnimation(int animationIndex)
    {
        // Animator가 아직 초기화되지 않았다면 다시 초기화 시도
        if (avatarAnimator == null)
        {
            InitializeAnimation();
            if (avatarAnimator == null)
            {
                Debug.LogError("Animator가 초기화되지 않았습니다!");
                return;
            }
        }
        
        RuntimeAnimatorController targetController = null;
        string animationName = "";
        
        switch (animationIndex)
        {
            case 0:
                targetController = locomotionController;
                animationName = "기본 (Locomotion)";
                break;
            case 1:
                targetController = walkingController;
                animationName = "걷기 (Walking)";
                break;
            case 2:
                targetController = runningController;
                animationName = "뛰기 (Running)";
                break;
            default:
                Debug.LogWarning($"잘못된 애니메이션 인덱스: {animationIndex}");
                return;
        }
        
        if (targetController == null)
        {
            Debug.LogError($"애니메이션 컨트롤러가 로드되지 않았습니다: {animationName}");
            return;
        }
        
        // 애니메이션 컨트롤러 변경
        avatarAnimator.runtimeAnimatorController = targetController;
        currentAnimationIndex = animationIndex;
        
        Debug.Log($"애니메이션 변경 완료: {animationName}");
    }
    
    /// <summary>
    /// 기본 애니메이션으로 설정
    /// </summary>
    public void SetLocomotionAnimation()
    {
        SetAnimation(0);
    }
    
    /// <summary>
    /// 걷기 애니메이션으로 설정
    /// </summary>
    public void SetWalkingAnimation()
    {
        SetAnimation(1);
    }
    
    /// <summary>
    /// 뛰기 애니메이션으로 설정
    /// </summary>
    public void SetRunningAnimation()
    {
        SetAnimation(2);
    }
    
    /// <summary>
    /// 다음 애니메이션으로 순환 전환
    /// </summary>
    public void SwitchToNextAnimation()
    {
        int nextIndex = (currentAnimationIndex + 1) % 3;
        SetAnimation(nextIndex);
    }
    
    /// <summary>
    /// 현재 애니메이션 인덱스 반환
    /// </summary>
    public int GetCurrentAnimationIndex()
    {
        return currentAnimationIndex;
    }
    
    /// <summary>
    /// 현재 애니메이션 이름 반환
    /// </summary>
    public string GetCurrentAnimationName()
    {
        switch (currentAnimationIndex)
        {
            case 0: return "기본 (Locomotion)";
            case 1: return "걷기 (Walking)";
            case 2: return "뛰기 (Running)";
            default: return "알 수 없음";
        }
    }
}