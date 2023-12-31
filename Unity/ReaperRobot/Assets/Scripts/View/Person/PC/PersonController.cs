﻿using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(PlayerInput))]
    public class PersonController : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private PersonManager _personManager;
        [SerializeField] private Camera _personCamera;
        #endregion

        #region private Fields
        private PlayerInput _playerInput;
        private InputActionMap _personActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _personActionMap = _playerInput.actions.FindActionMap("Person");

            //操作対象がPersonでなくなったら歩くのを止める
            _personActionMap
                .ObserveEveryValueChanged(x => x.enabled)
                .Skip(1)
                .Where(x => !x)
                .Subscribe(_ => _personManager.StopMove())
                .AddTo(this);
        }

        private void FixedUpdate()
        {
            if (!_playerInput.enabled || !_personActionMap.enabled) return;
            
            //移動
            if (_personActionMap["Move"].IsPressed())
            {
                var move = _personActionMap["Move"].ReadValue<Vector2>();
                _personManager.Move(move.x, move.y, _personCamera.transform);
            }
        }
        #endregion
    }
}