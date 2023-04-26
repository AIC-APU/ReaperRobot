using UnityEngine;
using UniRx;
using UnityEngine.Localization.Settings;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint
{
    public abstract class BaseCheckPoint : MonoBehaviour
    {
        #region Public Properties
        public IReadOnlyReactiveProperty<bool> IsChecked => _isChecked;
        public string Introduction => GetIntroduction();
        #endregion

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] protected string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] protected string _en = "Explanation in ENG";
        #endregion

        #region protected fields
        protected ReactiveProperty<bool> _isChecked = new(false);
        #endregion

        #region public Methods
        public abstract void InitializeCheckPoint();
        public abstract void FinalizeCheckPoint();
        #endregion

        #region Private Methods
        private string GetIntroduction()
        {
            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "ja":
                    return _ja;
                case "en":
                    return _en;
                default:
                    return "";
            }
        }
        #endregion
    }
}