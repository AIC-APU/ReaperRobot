using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionMapSwitcher : MonoBehaviour
    {

        #region Struct
        [Serializable]
        struct ActionMapPair
        {
            //カメラマネージャーのターゲットがこのオブジェクトになったら、ActinMapをEnableにする
            public GameObject Target;
            public string ActionMapName;
            [NonSerialized] public InputActionMap ActionMap;
        }
        #endregion

        #region Serialized Private Fields
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private List<ActionMapPair> _pairs = new List<ActionMapPair>();
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();

            //ActionMapPairのActionMapを取得
            for (int i = 0; i < _pairs.Count; i++)
            {
                var pair = _pairs[i];
                pair.ActionMap = _playerInput.actions.FindActionMap(pair.ActionMapName);
                _pairs[i] = pair;
            }

            //カメラのターゲットが変更されたら、そのターゲットに対応するアクションマップをEnableにする
            _cameraManager
                .ActiveCamera
                .Select(x => x.Target)
                .Subscribe(x => EnableActionMap(x))
                .AddTo(this);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// ターゲットオブジェクトに対応するアクションマップをEnableにし、それ以外をDisableにする
        /// </summary>
        public void EnableActionMap(GameObject target)
        {
            foreach (var pair in _pairs)
            {

                if (pair.Target == target)
                {
                    pair.ActionMap.Enable();
                }
                else
                {
                    pair.ActionMap.Disable();
                }
            }
        }
        #endregion
    }
}