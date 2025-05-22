using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    [SerializeField] private Transform CameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    private float camCurXRot;
    [SerializeField] private float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
    public Camera firstPerson;   // 1��Ī �� ī�޶� ��ġ 
    public Camera thirdPerson;   // 3��Ī �� ī�޶� ��ġ

    public bool isFirstPerson = true;

    [Header("Jump")]
    [SerializeField] float _jumpPower = 5f;
    private Rigidbody _rb;

    [Header("Dash & DoubleJump")]
    [SerializeField] float _dashPower = 50f;
    private int jumpCount = 0;
    private int maxJumpCount = 2;

    [Header("Climb")]
    private bool _wallCheck;
    [SerializeField] float _climbSpeed = 3f;
    [SerializeField] float _climbCheckDistance = 0.6f;
    [SerializeField] LayerMask wallLayer;

    
    public event Action OnInventory;
    public event Action OnInformation;
    private PlayerCondition condition;
    private bool isDash;
    private bool isDoubleJump;
    private bool isSliding;
    private bool isJumping;
    public bool canMove = true;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        condition = CharcterManager.Instance.GetComponent<PlayerCondition>() ?? CharcterManager.Instance.player.GetComponent<PlayerCondition>();
    }

    private void Update()
    {
        Move();
    }
    public void EnableMove()
    {
        canMove = true;
    }
    private void LateUpdate()
    {
        if (canLook)
        {
            if (isFirstPerson)
                FirstPersonCameraLook();
            else
                ThirdPersonCameraLook();
        }

    }

    private void Move()
    {
        if (!canMove) return;
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rb.velocity.y;

        _rb.velocity = dir;
    }

    // 1��Ī ī�޶��
    void FirstPersonCameraLook()
    {
        isFirstPerson = true;
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        CameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // 3��Ī ī�޶��
    void ThirdPersonCameraLook()
    {
        isFirstPerson = false;
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        CameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void onSwitchCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {

            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                firstPerson.gameObject.SetActive(true);
                thirdPerson.gameObject.SetActive(false);
            }
            else
            {
                thirdPerson.gameObject.SetActive(true);
                firstPerson.gameObject.SetActive(false);
            }
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;   
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if(IsGrounded()) // ����
        {
             _rb.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
        }
        else if(CheckWall() && !IsGrounded()) // ��Ÿ��
        {
            _rb.AddForce(Vector3.up * _jumpPower * 5f, ForceMode.Impulse);
        }
    }

    public bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * 0.4f; // ray ������
        Ray[] rays = new Ray[5]
        {
            new Ray(origin, Vector3.down), // �߽ɿ��� �Ʒ�
            new Ray(origin + transform.forward * 0.3f, Vector3.down), // ���ʿ��� �Ʒ�
            new Ray(origin - transform.forward * 0.3f, Vector3.down), // ���ʿ��� �Ʒ�
            new Ray(origin + transform.right * 0.3f, Vector3.down), // �����ʿ��� �Ʒ�
            new Ray(origin - transform.right * 0.3f, Vector3.down) // ���ʿ��� �Ʒ�
        };
        for(int i = 0; i< rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.5f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    public void onInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            OnInventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void onInformation(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnInformation?.Invoke();
            ToggleCursor();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (SkillManager.Instance.CheckUnLockSkill(SkillType.Dash) && !isDash)
            {
                SkillManager.Instance.TriggerCooldown(SkillType.Dash);
                condition.Dash(20f);
                StartCoroutine(Dash(_dashPower));
            }
        }
    }

    
    public void OnDoubleJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded() && !isDoubleJump)
            {
                jumpCount = 0;
                isDoubleJump = true;
                SkillManager.Instance.TriggerCooldown(SkillType.DoubleJump);
                condition.DoubleJump(10f);
            }
            if (SkillManager.Instance.CheckUnLockSkill(SkillType.DoubleJump) && jumpCount < maxJumpCount && isDoubleJump)
            {
                jumpCount++;
                _rb.velocity = new Vector2(_rb.velocity.x, 0f); 
                _rb.AddForce(Vector2.up * _jumpPower, ForceMode.Impulse);
                if(jumpCount == maxJumpCount) isDoubleJump = false;
            }
        }
    }

    public void OnInvincible(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (SkillManager.Instance.CheckUnLockSkill(SkillType.Invincible))
            {
                SkillManager.Instance.TriggerCooldown(SkillType.Invincible);
                condition.Invincibility(40f, SkillManager.Instance.GetCoolTime(SkillType.Invincible));
            }
        }    
    }

    private IEnumerator Dash(float dashPower)
    {
        isDash = true;

        Vector2 dir = new Vector2(transform.localScale.x, 0f); // ���� �÷��̾ �ٶ󺸴� ����
        _rb.AddForce(dir * dashPower, ForceMode.Impulse);

        yield return new WaitForSeconds(SkillManager.Instance.GetCoolTime(SkillType.Dash));

        isDash = false;
    }

    private bool CheckWall()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        _wallCheck = Physics.Raycast(ray, _climbCheckDistance, wallLayer);
        if (_wallCheck) return true;
        return false;
    }
}
