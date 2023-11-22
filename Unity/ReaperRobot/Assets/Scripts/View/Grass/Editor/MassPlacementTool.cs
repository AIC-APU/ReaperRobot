using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MassPlacementTool : EditorWindow
{
    private GameObject _selectedPrefab;
    private float _lastProcessTime = 0;
    private float _processInterval = 0.5f;
    private List<GameObject> _objectList = new();
    private int _maxListLength = 100;

    [MenuItem("Custom Tools/Mass Placemet Tool")]
    public static void ShowWindow()
    {
        MassPlacementTool window = GetWindow<MassPlacementTool>();
        window.titleContent = new GUIContent("Select Range");
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        _objectList.Clear();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (_selectedPrefab == null) return;

        Event e = Event.current;
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        // Left mouse button drag event
        if (e.isMouse && e.button == 0 && e.type == EventType.MouseDrag)
        {
            // Process every Interval seconds
            float currentTime = Time.realtimeSinceStartup;
            if (currentTime - _lastProcessTime >= _processInterval)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit dragHit, 100f, LayerMask.GetMask("Ground")))
                {
                    var pos = dragHit.point;
                    var rot = Quaternion.FromToRotation(Vector3.up, dragHit.normal);

                    var obj = Instantiate(_selectedPrefab, pos, rot);
                    _objectList.Add(obj);
                    if (_objectList.Count > _maxListLength) _objectList.RemoveAt(0);

                    _lastProcessTime = currentTime;
                }
            }
            e.Use();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag to select a range in the Scene view");

        _selectedPrefab = EditorGUILayout.ObjectField(_selectedPrefab, typeof(GameObject), false) as GameObject;
        _processInterval = EditorGUILayout.Slider("Process Interval (seconds)", _processInterval, 0.1f, 1.0f);

        if (_objectList.Count > 0)
        {
            if (GUILayout.Button("Undo", GUILayout.Height(30)))
            {
                var last = _objectList.Count - 1;
                var obj = _objectList[last];
                DestroyImmediate(obj);
                _objectList.RemoveAt(last);
            }
        }
        else
        {
            GUILayout.Label("No object to undo", GUILayout.Height(30));
        }
    }
}
