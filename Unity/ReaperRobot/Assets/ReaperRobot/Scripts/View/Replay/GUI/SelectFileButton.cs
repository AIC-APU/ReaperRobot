using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class SelectFileButton : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TMP_Text _fileNameText;
        #endregion

        #region Readonly Field
        readonly string defaultFileDirectory = Path.GetFullPath(Application.streamingAssetsPath + "/../../../CSVData");
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _selectButton.onClick.AddListener(SelectButton);

            _fileNameText.text = "select csv file";
        }

        void OnDestroy()
        {
            _selectButton.onClick.RemoveListener(SelectButton);
        }
        #endregion

        #region private method
        private void SelectButton()
        {
            var path = OpenDialog.OpenCSVFile("select csv file", defaultFileDirectory);

            if(path != "")
            {
                _replayManager.InitializeData(path);
                _fileNameText.text = Path.GetFileName(path);
            }
            else
            {
                Debug.LogWarning("ファイルが選択されていません");
            }
        }
        #endregion
    }
}
