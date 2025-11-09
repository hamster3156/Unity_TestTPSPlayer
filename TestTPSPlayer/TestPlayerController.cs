using Hamster;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerController : MonoBehaviour
{
    [SerializeField, Header("移動の速度")]
    private float _moveSpd = 5f;

    [SerializeField, Header("回転の速度")]
    private float _rotationSpd = 10f;

    [Header("ジャンプの設定")]
    [SerializeField, Tooltip("ジャンプ速度")]
    private float _jumpSpd = 5f;

    [SerializeField, Tooltip("クールタイム")]
    private float _jumpCoolTime = 0.5f;

    [Header("地面判定の設定")]
    [SerializeField, Tooltip("検知半径")]
    float _groundCheckRadius = 0.4f;

    [SerializeField, Tooltip("発射位置からずらす値")]
    float _groundCheckOffsetY = -0.76f;

    [SerializeField, Tooltip("長さ")]
    float _groundCheckDistance = 0.2f;

    [SerializeField]
    LayerMask groundLayer;

    private TestInputActionHandler _inputActionHandler;
    private Rigidbody _rigidbody;

    private Vector2 _moveInputDir;
    private (bool Input, bool Jumping) _isJumpFlag;

    private RaycastHit _rayHitObj;

    private bool CheckGroundStatus()
    {
        var groundOffset = transform.position + new Vector3(0, _groundCheckOffsetY + _groundCheckDistance, 0);
        return Physics.SphereCast(groundOffset, _groundCheckRadius, Vector3.down, out _rayHitObj, _groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }


    private void Start()
    {
        _inputActionHandler = GetComponent<TestInputActionHandler>();
        _rigidbody = GetComponent<Rigidbody>();

        _inputActionHandler.MoveAct += OnInputMove;
        _inputActionHandler.JumpAct += OnInputJump;
        _inputActionHandler.ValidInputCallback(true);
    }

    private void Update()
    {
        if (_isJumpFlag.Input && CheckGroundStatus())
        {
            AddFroceMoveY(isInit: false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - (_groundCheckOffsetY - _groundCheckDistance) * Vector3.down, _groundCheckRadius);
    }

    private void FixedUpdate()
    {
        VelocityMoveXZ(isInit: false);
    }

    private void OnInputMove(InputAction.CallbackContext context)
    {
        _moveInputDir = context.ReadValue<Vector2>();
    }

    private void OnInputJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isJumpFlag.Input = true;
            StartCoroutine(JumpCoolTime());
        }
    }

    private IEnumerator JumpCoolTime()
    {
        yield return new WaitForSeconds(_jumpCoolTime);
        _isJumpFlag.Input = false;
        _isJumpFlag.Jumping = false;
    }

    /// <summary>
    /// XZ軸の速度を直接変更する
    /// </summary>
    /// <param name="isInit">値を初期化する</param>
    private void VelocityMoveXZ(bool isInit)
    {
        if (isInit)
        {
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
            return;
        }

        var camForward = Camera.main.transform.forward;
        var camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        var moveInputDir = camForward * _moveInputDir.y + camRight * _moveInputDir.x;
        var moveDir = moveInputDir * _moveSpd;
        _rigidbody.linearVelocity = new Vector3(moveDir.x, _rigidbody.linearVelocity.y, moveDir.z);
        InputRotation(moveInputDir);
    }

    /// <summary>
    /// Y軸に瞬発的な力を加える
    /// </summary>
    /// <param name="isInit">値を初期化する</param>
    private void AddFroceMoveY(bool isInit)
    {
        if (_isJumpFlag.Jumping)
        {
            return;
        }

        _isJumpFlag.Jumping = true;

        if (isInit)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            return;
        }

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        _rigidbody.AddForce(Vector3.up * _jumpSpd, ForceMode.Impulse);
    }

    private void InputRotation(Vector3 inputDir)
    {
        if (inputDir == Vector3.zero)
        {
            return;
        }

        var toRotation = Quaternion.LookRotation(inputDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, _rotationSpd * Time.deltaTime);
    }
}
