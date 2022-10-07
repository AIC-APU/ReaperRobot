using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ShadowParamUi : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Shadow Player")]
        [SerializeField] private ShadowReaperPlayer _shadowPlayer;

        [Header("UI")]
        [SerializeField] private GameObject _shadowPlayerPanel;
        [SerializeField] private GameObject _csvDataPanel;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _inputHText;
        [SerializeField] private TMP_Text _inputVText;
        [SerializeField] private TMP_Text _liftText;
        [SerializeField] private TMP_Text _cutterText;
        [SerializeField] private TMP_Text _posXText;
        [SerializeField] private TMP_Text _posYText;
        [SerializeField] private TMP_Text _posZText;
        [SerializeField] private TMP_Text _angleText;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //パネルの表示・非表示
            _shadowPlayerPanel
                .ObserveEveryValueChanged(x => x.activeSelf)
                .Subscribe(x => _csvDataPanel.SetActive(x))
                .AddTo(this);

            //以下テキストの更新
            _shadowPlayer.InputH
                .Subscribe(x => _inputHText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.InputV
                .Subscribe(x => _inputVText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.Lift
                .Subscribe(x => _liftText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.Cutter
                .Subscribe(x => _cutterText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.PosX
                .Subscribe(x => _posXText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.PosY
                .Subscribe(x => _posYText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.PosZ
                .Subscribe(x => _posZText.text = x.ToString())
                .AddTo(this);

            _shadowPlayer.Angle
                .Subscribe(x => _angleText.text = x.ToString())
                .AddTo(this);
        }

        private void Update()
        {
            _timeText.text = GameTimer.ConvertSecondsToString(_shadowPlayer.PlayTime);
        }
        #endregion
    }

}