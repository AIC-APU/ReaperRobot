using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class GraphTest : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] private float _multipile = 3;
        [SerializeField] private bool _runScriptMachine = true;

        private ScriptMachine _scriptMachine;

        private void Start()
        {
            _scriptMachine = GetComponent<ScriptMachine>();
        }

        private void Update()
        {
            //オブジェクト変数の変更
            Variables.Object(this).Set("moveRadius", _multipile);

            //ScriptMachineのon/off
            _scriptMachine.enabled = _runScriptMachine;
        }
    }

}
