using TMPro;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class MeasuringDistanceUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private RobotReaperPlayer _robotReaperPlayer;
        [SerializeField] private ShadowReaperPlayer _shadowReaperPlayer;
        [SerializeField] private TMP_Text _nowDistanceText;
        [SerializeField] private TMP_Text _maxDistanceText;
        #endregion

        #region Private Fields
        private ReactiveProperty<float> _distance = new(0f);
        private ReactiveProperty<float> _maxDistance = new(0f);
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            //distanceが更新されたらUIテキストの変更
            _distance.Subscribe(x => _nowDistanceText.text = x.ToString("F2"));
            _maxDistance.Subscribe(x => _maxDistanceText.text = x.ToString("F2"));

            //PlayTimeが0の時初期値に戻す
            _shadowReaperPlayer
                .ObserveEveryValueChanged(x => x.PlayTime)
                .Where(x => x == 0)
                .Subscribe(_ =>
                {
                    _distance.Value = 0f;
                    _maxDistance.Value = 0f;
                })
                .AddTo(this);
        }

        void Update()
        {
            if (_shadowReaperPlayer.IsPlaying.Value)
            {
                //影モード使用時には常にdistanceを更新
                var robotPos = _robotReaperPlayer.ReaperTransform.position;
                var shadowPos = _shadowReaperPlayer.ShadowTransform.position;
                _distance.Value = Vector3.Distance(robotPos, shadowPos);
                _maxDistance.Value = Mathf.Max(_maxDistance.Value, _distance.Value);
            }
        }
        #endregion
    }

}