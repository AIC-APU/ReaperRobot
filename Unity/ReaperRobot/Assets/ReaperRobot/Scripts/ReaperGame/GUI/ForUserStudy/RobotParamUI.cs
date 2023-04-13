using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class RobotParamUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Robot Player")]
        [SerializeField] private RobotReaperPlayer _robotPlayer;

        [Header("UI")]
        [SerializeField] private GameObject _robotPlayerPanel;
        [SerializeField] private GameObject _csvDataPanel;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _inputHText;
        [SerializeField] private TMP_Text _inputVText;
        [SerializeField] private TMP_Text _liftText;
        [SerializeField] private TMP_Text _cutterText;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //パネルの表示・非表示
            _robotPlayerPanel
                .ObserveEveryValueChanged(x => x.activeSelf)
                .Subscribe(x => _csvDataPanel.SetActive(x))
                .AddTo(this);

            //以下テキストの更新
            _robotPlayer.InputH
                .Subscribe(x => _inputHText.text = x.ToString())
                .AddTo(this);

            _robotPlayer.InputV
                .Subscribe(x => _inputVText.text = x.ToString())
                .AddTo(this);

            _robotPlayer.Lift
                .Subscribe(x => _liftText.text = x.ToString())
                .AddTo(this);

            _robotPlayer.Cutter
                .Subscribe(x => _cutterText.text = x.ToString())
                .AddTo(this);
        }

        private void Update()
        {
            //_timeText.text = GameTimer.ConvertSecondsToString(_robotPlayer.PlayTime);
        }
        #endregion
    }
}