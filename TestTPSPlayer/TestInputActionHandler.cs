using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hamster
{
    [RequireComponent(typeof(PlayerInput))]
    public class TestInputActionHandler : MonoBehaviour
    {
        public Action<InputAction.CallbackContext> MoveAct;
        public Action<InputAction.CallbackContext> JumpAct;

        [SerializeField]
        private InputActionReference _moveRef;

        [SerializeField]
        private InputActionReference _jumpRef;

        private PlayerInput _playerInput;

        private bool _isValidInputCallback;

        private enum InputType
        {
            Move,
            Jump,
        }

        /// <summary>
        /// 入力のコールバックを有効化・無効化する
        /// </summary>
        /// <param name="value"></param>
        public void ValidInputCallback(bool value)
        {
            _isValidInputCallback = value;
        }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            RegisterCalback();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDestroy()
        {
            RegisterCalback(true);
        }

        private void RegisterCalback(bool isUnregister = false)
        {
            if (isUnregister)
            {
                _playerInput.actions[_moveRef.action.name].started -= OnMove;
                _playerInput.actions[_moveRef.action.name].performed -= OnMove;
                _playerInput.actions[_moveRef.action.name].canceled -= OnMove;

                _playerInput.actions[_jumpRef.action.name].started -= OnJump;
                _playerInput.actions[_jumpRef.action.name].performed -= OnJump;
                _playerInput.actions[_jumpRef.action.name].canceled -= OnJump;

                MoveAct = null;
                JumpAct = null;
                return;
            }

            _playerInput.actions[_moveRef.action.name].started += OnMove;
            _playerInput.actions[_moveRef.action.name].performed += OnMove;
            _playerInput.actions[_moveRef.action.name].canceled += OnMove;

            _playerInput.actions[_jumpRef.action.name].started += OnJump;
            _playerInput.actions[_jumpRef.action.name].performed += OnJump;
            _playerInput.actions[_jumpRef.action.name].canceled += OnJump;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            InputCallback(InputType.Move, context);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            InputCallback(InputType.Jump, context);
        }

        private void InputCallback(InputType inputType, InputAction.CallbackContext context)
        {
            if (!_isValidInputCallback)
            {
                return;
            }

            switch (inputType)
            {
                case InputType.Move:
                    MoveAct?.Invoke(context);
                    break;

                case InputType.Jump:
                    JumpAct?.Invoke(context);
                    break;
            }
        }
    }
}