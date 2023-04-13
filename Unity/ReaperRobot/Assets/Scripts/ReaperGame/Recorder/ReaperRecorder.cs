using smart3tene;
using smart3tene.Reaper;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UniRx;
using Cysharp.Threading.Tasks;

public class ReaperRecorder : MonoBehaviour
{
    #region 
    public bool ControllableRobot { get; private set; } = true;
    #endregion

    #region Serialized Private Fields
    //[SerializeField] private ReaperManager _reaperManager;
    [SerializeField] private Transform _reaperTransform;
    [SerializeField] private string _fileName = "InputLog";

    [Header("Popup Text")]
    [SerializeField, TextArea(1, 4)] private string _ja = "を保存しました";
    [SerializeField, TextArea(1, 4)] private string _en = "was saved.";
    #endregion

    #region Private Fields
    public IReadOnlyReactiveProperty<bool> IsRecording => _isRecording;
    private ReactiveProperty<bool> _isRecording = new(false);

    public IReadOnlyReactiveProperty<float> RecordingTime => _recordingTime;
    private ReactiveProperty<float> _recordingTime = new(0f);

    public IReadOnlyReactiveProperty<float> InputH => _inputH;
    private ReactiveProperty<float> _inputH = new(0);

    public IReadOnlyReactiveProperty<float> InputV => _inputV;
    private ReactiveProperty<float> _inputV = new(0);

    public IReadOnlyReactiveProperty<int> LiftInt => _liftInt;
    private ReactiveProperty<int> _liftInt = new(1);

    public IReadOnlyReactiveProperty<int> CutterInt => _cutterInt;
    private ReactiveProperty<int> _cutterInt = new(1);

    public IReadOnlyReactiveProperty<float> PosX => _posX;
    private ReactiveProperty<float> _posX = new(0);

    public IReadOnlyReactiveProperty<float> PosY => _posY;
    private ReactiveProperty<float> _posY = new(0);

    public IReadOnlyReactiveProperty<float> PosZ => _posZ;
    private ReactiveProperty<float> _posZ = new(0);

    public IReadOnlyReactiveProperty<float> Angle => _angleY;
    private ReactiveProperty<float> _angleY = new(0);

    private string _csvData = "";
    #endregion

    #region Readonly Field
    readonly string saveDirectory = Application.streamingAssetsPath + "/../../../InputLog";
    readonly Vector3 _repositionPos = new(0f, 0f, 0f);
    readonly Vector3 _repositionAng = new(0f, 0f, 0f);
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        //名前が無ければ付ける
        if (_fileName == "") _fileName = "InputLog";

        //拡張子は外しておく（Export時に付ける）
        if (_fileName.EndsWith(".csv")) _fileName = _fileName.Remove(_fileName.Length - 4, 4);
    }

    private void Update()
    {
        if (!_isRecording.Value) return;

        //データを揃える
        //必要あればここで桁数やら形式やら指定する

        //ここ時間を入れる
        var time   = "00:00:00";

        // _inputH.Value = _reaperManager.NowInput.x;
        // _inputV.Value = _reaperManager.NowInput.y;

        // _liftInt.Value   = _reaperManager.IsLiftDown.Value ? 1 : 0;
        // _cutterInt.Value = _reaperManager.IsCutting.Value ? 1 : 0;

        _posX.Value   = RoundF(_reaperTransform.position.x, 8);
        _posY.Value   = RoundF(_reaperTransform.position.y, 8);
        _posZ.Value   = RoundF(_reaperTransform.position.z, 8);

        var angleY    = _reaperTransform.eulerAngles.y < 180 ? _reaperTransform.eulerAngles.y : _reaperTransform.transform.eulerAngles.y - 360f;
        _angleY.Value = RoundF(angleY, 8);

        // input.x, input.y, pos.x, pos.y, pos.z, angle.y のような形式でstringを保存
        _csvData += $"{time},{_inputH.Value},{_inputV.Value},{_liftInt.Value},{_cutterInt.Value},{_posX.Value},{_posY.Value},{_posZ.Value},{_angleY.Value}\n";

        _recordingTime.Value += Time.deltaTime;
    }

    private void OnDestroy()
    {
        //記録の途中でゲームが終わったならそこでExportする
        if (_isRecording.Value)
        {
            _isRecording.Value = false;
            ExportCSV();
        }
    }
    #endregion

    #region Public method
    public void StartRecording()
    {
        _recordingTime.Value = 0f;

        //csvDataの1行目にラベルを設定
        _csvData = "Time,input_horizontal,input_vertical,Lift,Cutter,PosX,PosY,PosZ,AngleY" + "\n";

        _isRecording.Value = true;

        //コントローラの使用の許可
        ControllableRobot = true;
    }

    public void StopRecording()
    {
        if (_isRecording.Value)
        {
            _isRecording.Value = false;
            ExportCSV();
        }

        //コントローラの使用の許可
        ControllableRobot = true;
    }

    public async void Reposition()
    {
        // _reaperManager.Move(0, 0);

        // await UniTask.Yield();

        // _reaperTransform.SetPositionAndRotation(_repositionPos, Quaternion.Euler(_repositionAng));

        // _reaperManager.MoveLift(true);
        // _reaperManager.RotateCutter(true);

        //コントローラの使用の禁止
        ControllableRobot = false;
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
        //ReaperEventManager.InvokeTextPopupEvent(popupText);

        //csvDataの初期化
        _csvData = "";
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
