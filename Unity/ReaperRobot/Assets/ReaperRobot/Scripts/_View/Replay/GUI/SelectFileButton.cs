using System.IO;
using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class SelectFileButton : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private TMP_Text _text;
        #endregion

        #region Readonly Field
        readonly string defaultFileDirectory = Path.GetFullPath(Application.streamingAssetsPath + "/../../../CSVData");
        #endregion

        #region Public method
        public void SelectButton()
        {
            var path = OpenDialog.OpenCSVFile("select csv file", defaultFileDirectory);

            if(path != "")
            {
                _replayManager.InitializeData(path);
                _text.text = Path.GetFileName(path);
            }
            else
            {
                Debug.LogWarning("ファイルが選択されていません");
            }
        }
        #endregion
    }
}
