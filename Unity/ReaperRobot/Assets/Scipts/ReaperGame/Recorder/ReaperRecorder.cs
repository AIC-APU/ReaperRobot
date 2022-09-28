using smart3tene.Reaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEngine.Localization.Settings;
using Newtonsoft.Json.Schema;
using smart3tene;
using System;
using System.IO;

public class ReaperRecorder : MonoBehaviour
{
    #region Serialized Private Fields
    [SerializeField] private ReaperManager _reaperManager;
    [SerializeField] private Transform _reaperTransform;
    [SerializeField] private string _fileName = "InputLog";

    [Header("Popup Text")]
    [SerializeField, TextArea(1, 4)] private string _ja = "を保存しました";
    [SerializeField, TextArea(1, 4)] private string _en = "was saved.";
    #endregion

    #region Private Fields
    public bool IsRecording => _isRecording;
    private bool _isRecording = false;

    public float RecordingTime => _RecordingTime;
    private float _RecordingTime = 0f;

    private string _csvData = "";
    #endregion

    #region Readonly Field
    readonly string saveDirectory = Application.streamingAssetsPath + "/../../../InputLog";
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        //名前が無ければ付ける
        if (_fileName == "") _fileName = "InputLog";

        //拡張子は外しておく（Export時に付ける）
        if (_fileName.EndsWith(".csv")) _fileName = _fileName.Remove(_fileName.Length - 4, 4);

        //csvDataの1行目にラベルを設定
        _csvData += "Time,input_horizontal,input_vertical,Lift,Cutter,PosX,PosY,PosZ,AngleY" + "\n";
    }

    private void Update()
    {
        if (!_isRecording) return;

        //データを揃える
        //必要あればここで桁数やら形式やら指定する
        var time   = GameTimer.ConvertSecondsToString(_RecordingTime);

        var inputX = _reaperManager.NowInput.x;
        var inputY = _reaperManager.NowInput.y;

        var posX   = RoundF(_reaperTransform.position.x, 8);
        var posY   = RoundF(_reaperTransform.position.y, 8);
        var posZ   = RoundF(_reaperTransform.position.z, 8);

        var angleY = _reaperTransform.eulerAngles.y < 180 ? _reaperTransform.eulerAngles.y : _reaperTransform.transform.eulerAngles.y - 360f;
            angleY = RoundF(angleY, 8);

        var lift   = _reaperManager.IsLiftDown.Value ? 1 : 0;
        var cutter = _reaperManager.IsCutting.Value ? 1 : 0;

        // input.x, input.y, pos.x, pos.y, pos.z, angle.y のような形式でstringを保存
        _csvData += $"{time},{inputX},{inputY},{lift},{cutter},{posX},{posY},{posZ},{angleY}\n";

        _RecordingTime += Time.deltaTime;
    }
    #endregion

    #region Public method
    public void StartRecording()
    {
        _RecordingTime = 0f;

        _isRecording = true;
    }

    public void StopRecording()
    {
        _isRecording = false;

        ExportCSV();
    }
    #endregion

    #region Private method
    private void ExportCSV()
    {
        //ファイル名作成
        var now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var filePath = $"{saveDirectory}/{_fileName}_{now}.csv";

        //csvファイルに書き出し
        CSVUtility.Write(filePath, _csvData);

        //UIの表示
        var popupText = GetText(LocalizationSettings.SelectedLocale.Identifier.Code, Path.GetFileName(filePath));
        ReaperEventManager.InvokeTextPopupEvent(popupText);
    }

    private string GetText(string localeCode, string fileName)
    {
        var text = fileName + " ";
        switch (localeCode)
        {
            case "ja":
                text += _ja;
                break;

            case "en":
                text += _en;
                break;

            default:
                break;
        }
        return text;
    }

    /// <summary>
    /// 指定した小数点以下桁数で四捨五入
    /// </summary>
    /// <param name="num">四捨五入したい数</param>
    /// <param name="digits">小数点以下の桁数</param>
    private float RoundF(float num, int digits)
    {
        var multipile = (float)Math.Pow(10, digits);
        return Mathf.Round(num * multipile) / multipile;
    }
    #endregion
}
