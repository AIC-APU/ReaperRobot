using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.Data;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Replay.GUI
{
    public class SelectFileButton : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private List<BaseReplay> _replaySystems = new List<BaseReplay>();
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
                foreach (var replay in _replaySystems)
                {
                    replay.InitializeReplay(path);
                    _text.text = Path.GetFileName(path);
                }
            }
            else
            {
                Debug.LogWarning("ファイルが選択されていません");
            }
        }
        #endregion
    }
}
