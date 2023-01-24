using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using UnityEngine;

namespace smart3tene
{

    public class SetTomato : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private List<Transform> _tomatoLocations = new List<Transform>();
        [SerializeField] private List<GameObject> _tomatoPrefabs = new List<GameObject>();
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            var tomatoNum = DecideTomatoNum(_tomatoLocations.Count);

            RandomSetTomato(tomatoNum);
        }
        #endregion

        #region Private method
        private int DecideTomatoNum(int maxNum)
        {
            var random = new System.Random();
            return random.Next(0, maxNum + 1);

            //今はランダムに決めているが、
            //将来的には木の成長具合や時期などから、トマトの数を決定したい
            //例えば未熟な木ならトマトを少なくしたり、夏なら多くしたりなど
        }

        private void RandomSetTomato(int tomatoNum)
        {
            //トマトを実らせる順番をランダムにしているが、実際は実る法則などあるのだろうか...?
            var randomLocList = _tomatoLocations
                                .OrderBy(x => Guid.NewGuid())
                                .ToList();

            for(int i = 0; i < tomatoNum; i++)
            {
                var tomatoIndex = DecideTomatoIndex(_tomatoPrefabs.Count);
                Instantiate(_tomatoPrefabs[tomatoIndex], randomLocList[i]);
            }            
        }

        private int DecideTomatoIndex(int prefabCount)
        {
            var random = new System.Random();
            return random.Next(0, prefabCount);

            //今はランダムに決めているが、
            //将来的には木の成長具合や時期などから、生るトマトに偏りを持たせたい
            //例えば未熟な木なら青いトマトを多くしたり、夏なら赤いトマトを多くしたりなど
        }
        #endregion
    }
}