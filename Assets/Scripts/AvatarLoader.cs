using UnityEngine;
using UnityEngine.InputSystem;
using UMA.CharacterSystem;

public class AvatarLoader : MonoBehaviour
{
    public string avatarPrefabName = "DynamicCharacterAvatar";
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on this GameObject. Please add one.");
        }
    }

    void Start()
    {
        GameObject avatarPrefab = Resources.Load<GameObject>(avatarPrefabName);

        if (avatarPrefab != null)
        {
            GameObject avatarInstance = Instantiate(avatarPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0));
            DynamicCharacterAvatar umaAvatar = avatarInstance.GetComponent<DynamicCharacterAvatar>();
            AvatarRotator avatarRotator = avatarInstance.GetComponent<AvatarRotator>();

            if (umaAvatar == null)
            {
                Debug.LogError("DynamicCharacterAvatar component not found on instantiated avatar!");
            }

            if (playerInput != null && avatarRotator != null)
            {
                // The rotation logic is now handled directly in AvatarRotator's Update method,
                // so we no longer need to subscribe to the Input System events here.
            }
            else
            {
                Debug.LogError("PlayerInput or AvatarRotator component not found!");
            }
        }
        else
        {
            Debug.LogError($"Avatar prefab '{avatarPrefabName}' not found in a 'Resources' folder. Please check the path and name.");
        }
    }
}
