using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene
{
    public class TrainingButtonForDebug : MonoBehaviour
    {
        [SerializeReference] private Button _trainingButton;

        private void Update()
        {
            if(GameData.NowGameMode == GameData.GameMode.SOLO)
            {
                _trainingButton.interactable = true;
            }
            else
            {
                _trainingButton.interactable = false;
            }
        }
    }

}
