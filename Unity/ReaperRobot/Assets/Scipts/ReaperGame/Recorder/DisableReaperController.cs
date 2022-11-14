using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    public class DisableReaperController : MonoBehaviour
    {
        #region Serilaized Private Fields
        [SerializeField] private RobotPlayerUI _robotPlayerUI;
        [SerializeField] private CSVRecorderUI _csvRecorderUI;
        [SerializeField] private ShadowPlayerUI _shadowPlayerUI;

        [Header("Controller")]
        [SerializeField] private ReaperController _controller;

        [Header("Reaper Button")]
        [SerializeField] private Button _downLiftButton;
        [SerializeField] private Button _upLiftButton;
        [SerializeField] private Button _rotateCutterButton;
        [SerializeField] private Button _stopCutterButton;
        #endregion

        #region Private Fields

        #endregion

        #region Monobehaviour Callbacks
        private void Update()
        {
            var controllable = _robotPlayerUI.ControllableRobot && _csvRecorderUI.ControllableRobot && _shadowPlayerUI.ControllableRobot;

            _controller.enabled = controllable;
            LiftAndCutterButton(controllable);
        }
        #endregion

        #region Private method
        private void LiftAndCutterButton(bool interactable)
        {
            _downLiftButton.interactable = interactable;
            _upLiftButton.interactable = interactable;
            _stopCutterButton.interactable = interactable;
            _rotateCutterButton.interactable = interactable;
        }
        #endregion
    }

}