using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-1)]
    public class GUIManager : MonoBehaviour
    {
        #region Serialized private Fields
        [Header("MainScreen")]
        [SerializeField] private RawImage _mainScreen;

        [Header("ReapRate and Time")]
        [SerializeField] private TMP_Text _reaperRateNum;
        [SerializeField] private TMP_Text _timeNum;

        [Header("Delay")]
        [SerializeField] private CameraProjector _projector;
        [SerializeField] private Slider _delaySlider;
        [SerializeField] private TMP_Text _delayNumText;

        [Header("Camera")]
        [SerializeField] private Camera _personTPVCamera;
        [SerializeField] private Camera _personFPVCamera;
        [SerializeField] private Camera _robotFPVCamera;
        [SerializeField] private Camera _robotBirdViewCamera;
        [SerializeField] private Camera _robotAroundCamera;
        [SerializeField] private Camera _miniMapCamera;


        [Header("Reaper Camera Parameter")]
        [SerializeField] private GameObject _reaperFPVCameraPanel;
        [SerializeField] private TMP_Text _positonXNum;
        [SerializeField] private TMP_Text _positonYNum;
        [SerializeField] private TMP_Text _positonZNum;
        [SerializeField] private TMP_Text _rotationXNum;
        [SerializeField] private TMP_Text _rotationYNum;
        [SerializeField] private TMP_Text _rotationZNum;

        [Header("Lift and Cutter")]
        [SerializeField] private Image _liftLamp;
        [SerializeField] private Image _cutterLamp;

        [Header("Menu Panel")]
        [SerializeField] private GameObject _menu;

        [Header("ViewMode Panel")]
        [SerializeField] private TMP_Text _viewModeText;

        [Header("Save File Panel")]
        [SerializeField] private GameObject _saveFilePanel;
        [SerializeField] private TMP_Text _filePathText;
        #endregion

        #region Readonly Fields
        readonly int maxDelay = 3;
        #endregion

        #region Private Fields
        private Transform _reaperTransform;
        private ReaperManager _reaperManager;
        private FPVCameraManager _fpvCameraManager;
        private Camera _mainCamera;
        
        public Camera NowUsingCamera => _nowUsingCamera;
        private Camera _nowUsingCamera;

        private CancellationTokenSource _savePanelCancelTaken;
        private UniTask _saveFileTask;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _reaperManager = GameSystem.Instance.ReaperInstance.GetComponent<ReaperManager>();
            _reaperTransform = GameSystem.Instance.ReaperInstance.transform;
            _fpvCameraManager = GameSystem.Instance.ReaperInstance.GetComponent<FPVCameraManager>();

            //------以下各種GUIの挙動------
            //ReapRate
            GameSystem.Instance.CutGrassCount.Subscribe(x => _reaperRateNum.text = UpdateReapRate());

            //Time
            GameSystem.Instance.GameTime.Subscribe(x => _timeNum.text = UpdateGameTime(x));

            //DelaySlider
            InitializeDelaySlider();

            //メインカメラの取得
            _mainCamera = Camera.main;

            //robotFPVCameraをprojectorに設定
            _projector.recordingCamera = _robotFPVCamera;

            //各ロボットカメラの設定
            GameSystem.Instance.ReaperInstance.GetComponent<FPVCameraManager>().Camera = _robotFPVCamera;
            GameSystem.Instance.ReaperInstance.GetComponent<BirdViewCameraManager>().Camera = _robotBirdViewCamera;
            GameSystem.Instance.ReaperInstance.GetComponent<AroundViewCameraManager>().Camera = _robotAroundCamera;

            var personManager = GameSystem.Instance.PersonInstance.GetComponent<PersonManager>();
            personManager.FPVCameraTransform = _personFPVCamera.transform;
            personManager.TPVCameraTransform = _personTPVCamera.transform;

            //ReaperCameraの位置・角度テキスト
            _fpvCameraManager.CameraOffsetPos.Subscribe(vec =>
            {
                _positonXNum.text = vec.x.ToString("F1");
                _positonYNum.text = vec.y.ToString("F1");
                _positonZNum.text = vec.z.ToString("F1");
            });

            _fpvCameraManager.CameraOffsetRot.Subscribe(vec =>
            {
                _rotationXNum.text = ((int)vec.x).ToString();
                _rotationYNum.text = ((int)vec.y).ToString();
                _rotationZNum.text = ((int)vec.z).ToString();
            });


            //Liftのランプ
            _reaperManager.IsLiftDown.Subscribe(isDown =>
            {
                if (isDown)
                {
                    _liftLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {
                    _liftLamp.color = new Color32(196, 196, 196, 255);
                }
            });


            //Cutterのランプ
            _reaperManager.IsCutting.Subscribe(isCutting =>
            {
                if (isCutting)
                {
                    _cutterLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {       
                    _cutterLamp.color = new Color32(196, 196, 196, 255);
                }
            });

            //カメラの切り替え
            GameSystem.Instance.NowViewMode.Subscribe(mode =>
            {
                _viewModeText.text = mode.ToString();

                //画面切り替え、もっといい方法あればそうしたい
                switch (mode)
                {
                    case GameSystem.ViewMode.REAPER_FPV:
                        _mainCamera.enabled = true;
                        _personTPVCamera.enabled = false;
                        _personFPVCamera.enabled = false;
                        _robotBirdViewCamera.enabled = false;
                        _robotAroundCamera.enabled = false;

                        GetComponent<Canvas>().enabled = true;
                        _mainScreen.enabled = true;

                        _reaperFPVCameraPanel.SetActive(true);

                        _nowUsingCamera = _robotFPVCamera;
                        break;

                    case GameSystem.ViewMode.REAPER_BIRDVIEW:
                        _mainCamera.enabled = false;
                        _personTPVCamera.enabled = false;
                        _personFPVCamera.enabled = false;
                        _robotBirdViewCamera.enabled = true;
                        _robotAroundCamera.enabled = false;

                        GetComponent<Canvas>().enabled = true;
                        _mainScreen.enabled = false;

                        _reaperFPVCameraPanel.SetActive(false);

                        _nowUsingCamera = _robotBirdViewCamera;
                        break;

                    case GameSystem.ViewMode.REAPER_AROUND:
                        _mainCamera.enabled = false;
                        _personTPVCamera.enabled = false;
                        _personFPVCamera.enabled = false;
                        _robotBirdViewCamera.enabled = false;
                        _robotAroundCamera.enabled = true;

                        GetComponent<Canvas>().enabled = true;
                        _mainScreen.enabled = false;

                        _reaperFPVCameraPanel.SetActive(false);

                        _nowUsingCamera = _robotAroundCamera;
                        break;

                    case GameSystem.ViewMode.REAPER_FromPERSON:
                        _mainCamera.enabled = false;
                        _personTPVCamera.enabled = false;
                        _personFPVCamera.enabled = true;
                        _robotBirdViewCamera.enabled = false;
                        _robotAroundCamera.enabled = false;

                        GetComponent<Canvas>().enabled = false;
                        _mainScreen.enabled = false;

                        _reaperFPVCameraPanel.SetActive(false);

                        _nowUsingCamera = _personFPVCamera;
                        break;

                    case GameSystem.ViewMode.PERSON_TPV:
                        _mainCamera.enabled = false;
                        _personTPVCamera.enabled = true;
                        _personFPVCamera.enabled = false;
                        _robotBirdViewCamera.enabled = false;
                        _robotAroundCamera.enabled = false;

                        GetComponent<Canvas>().enabled = false;
                        _mainScreen.enabled = false;

                        _reaperFPVCameraPanel.SetActive(false);

                        _nowUsingCamera = _personTPVCamera;
                        break;

                    default:
                        break;
                }
            });

            this.LateUpdateAsObservable()
                .Where(_ => _miniMapCamera.transform != null)
                .Where(_ => _reaperTransform != null)
                .Subscribe(_ =>
                {
                    //ミニマップカメラの位置
                    _miniMapCamera.transform.position = new Vector3(_reaperTransform.position.x, _miniMapCamera.transform.position.y, _reaperTransform.position.z);
                    _miniMapCamera.transform.eulerAngles = new Vector3(_miniMapCamera.transform.eulerAngles.x, _reaperTransform.eulerAngles.y, _miniMapCamera.transform.eulerAngles.z);
                })
                .AddTo(this);

            _menu.SetActive(false);
            GameSystem.Instance.MenuEvent += ShowAndHideMenu;

            _savePanelCancelTaken = new CancellationTokenSource();
            GameSystem.Instance.SaveFileEvent += OnSaveFileEvent;
        }

        private void OnDisable()
        {
            GameSystem.Instance.MenuEvent -= ShowAndHideMenu;

            GameSystem.Instance.SaveFileEvent -= OnSaveFileEvent;
        }
        #endregion

        #region public method
        public void DelaySliderOnValueChaged()
        {
            _projector.delay = _delaySlider.value;
            _delayNumText.text = _delaySlider.value.ToString("F1");
        }

        public void DownButtonClick()
        {
            _reaperManager.MoveLift(true);
        }

        public void UpButtonClick()
        {
            _reaperManager.MoveLift(false);
        }

        public void RotateButtonClick()
        {
            _reaperManager.RotateCutter(true);
        }

        public void StopButtonClick()
        {
            _reaperManager.RotateCutter(false);
        }

        public void ResetButtonClick()
        {
            GameSystem.Instance.ResetGrasses();
        }

        public void EndGameButtonClick()
        {
            SceneTransitionManager.Instance.EndGame();
        }
        #endregion

        #region private method
        private void InitializeDelaySlider()
        {
            _delaySlider.maxValue = maxDelay;
            _delaySlider.minValue = 0;

            if (_projector.delay > maxDelay)
            {
                _projector.delay = maxDelay;
            }
            _delaySlider.value = _projector.delay;

            _delayNumText.text = _delaySlider.value.ToString("F1");
        }

        private string UpdateGameTime(float gameTime)
        {
            var span = new TimeSpan(0, 0, (int)gameTime);
            return span.ToString(@"hh\:mm\:ss");
        }

        private string UpdateReapRate()
        {
            float reapRate;
            if (GameSystem.Instance.AllGrassCount != 0)
            {
                reapRate = 100f * (float)GameSystem.Instance.CutGrassCount.Value / (float)GameSystem.Instance.AllGrassCount;
            }
            else
            {
                return "--";
            }

            return reapRate.ToString("F1");
        }
        private void ShowAndHideMenu()
        {
            _menu.SetActive(!_menu.activeSelf);
        }

        private async void OnSaveFileEvent(string fileName)
        {
            await _saveFileTask;

            _saveFileTask = ShowSaveFilePanel(fileName, _savePanelCancelTaken.Token);
        }

        private async UniTask ShowSaveFilePanel(string fileName, CancellationToken ct = default)
        {
            var canvasGroup = _saveFilePanel.GetComponent<CanvasGroup>();

            _filePathText.text = $"{fileName} was photographed";

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += 0.01f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2));

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= 0.01f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
        #endregion
    }
}