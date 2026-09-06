using UnityEngine;
using UnityEngine.InputSystem;

// Gerak FPS sederhana di atas CharacterController. WASD + panah untuk
// gerak, mouse untuk lihat. Bergerak hanya saat misi berjalan sehingga
// pemain terkunci saat MISSION COMPLETE atau FAILED.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float lookSensitivity = 2f;
    public float gravity = -9.81f;

    [Header("Refs (diisi builder)")]
    public Camera playerCamera;

    CharacterController controller;
    float pitch;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        try
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        catch (System.Exception)
        {
            // Abaikan bila kursor tak bisa dikunci (misal editor headless).
        }
    }

    void Update()
    {
        if (MissionManager.Instance != null && MissionManager.Instance.Phase != MissionManager.MissionPhase.Running)
        {
            UnlockCursor();
            return;
        }
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;
        Vector2 delta = mouse.delta.ReadValue();
        float yaw = delta.x * lookSensitivity * 0.05f;
        pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity * 0.05f, -80f, 80f);
        transform.Rotate(Vector3.up * yaw);
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMove()
    {
        var kb = Keyboard.current;
        Vector2 input = Vector2.zero;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
        }
        Vector3 move = (transform.forward * input.y + transform.right * input.x);
        if (move.sqrMagnitude > 1f)
            move.Normalize();
        move *= moveSpeed;
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    void UnlockCursor()
    {
        try
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        catch (System.Exception)
        {
            // Abaikan, tidak kritis.
        }
    }
}
