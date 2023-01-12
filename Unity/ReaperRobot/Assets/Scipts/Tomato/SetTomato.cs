using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace smart3tene
{

    public class SetTomato : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private List<Transform> _tomatoLocations = new List<Transform>();
        [SerializeField] private List<GameObject> _tomatoPrefabs = new List<GameObject>();
        [SerializeField, Range(0, 10)] private int _tomatoNum = 5;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            RandomSetTomato(_tomatoNum);
        }
        #endregion

        #region Private method
        private void RandomSetTomato(int tomatoNum)
        {
            var randomLocList = _tomatoLocations
                                .OrderBy(x => Guid.NewGuid())
                                .ToList();

            var random = new System.Random();

            for(int i = 0; i < tomatoNum; i++)
            {
                var tomatoIndex = random.Next(0, _tomatoPrefabs.Count);
                Instantiate(_tomatoPrefabs[tomatoIndex], randomLocList[i]);
            }
        }
        #endregion
    }
}