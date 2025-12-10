using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // 인스펙터에서 연결 필수:
    // cameraTransform: 플레이어의 Main Camera 오브젝트 연결
    public float moveSpeed = 5f;
    public float verticalSpeed = 3f; // 위아래 이동 속도 추가
    public float rotationSpeed = 2f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalRotation = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        // 1. 캐릭터 이동 (W, A, S, D)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. 위아래 이동 입력 확인 (Space Bar = 위, Left Control = 아래)
        float yMovement = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            yMovement = verticalSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            yMovement = -verticalSpeed;
        }

        // 이동 방향 벡터 계산 및 적용
        // yMovement를 포함하여 위아래 이동을 허용합니다.
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal + transform.up * yMovement;

        if (controller != null)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }

        // 3. 시점 회전 (마우스 Look - Pitch/Yaw)
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Player 본체는 좌우만 회전합니다.
        transform.Rotate(Vector3.up * mouseX); 

        // 카메라만 위아래로 회전합니다.
        if (cameraTransform != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}