using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AvatarRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // 회전 속도를 더 직관적으로 조절할 수 있도록 값을 조정했습니다.
    private bool isRotating = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 입력을 확인합니다.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI 요소 위에서 클릭이 시작되었는지 확인합니다.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI 위라면 회전을 시작하지 않습니다.
            }

            // 마우스 클릭 위치에서 Ray를 쏩니다.
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // Ray가 이 게임 오브젝트의 Collider와 충돌했는지 확인합니다.
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isRotating = true;
            }
        }

        // 마우스 왼쪽 버튼을 떼면 회전을 멈춥니다.
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isRotating = false;
        }

        // 회전 상태일 때만 아바타를 회전시킵니다.
        if (isRotating)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            // Y축을 기준으로 회전합니다. 마우스 X축 움직임에 따라 회전하도록 delta.x를 사용합니다.
            transform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime);
        }
    }
}
