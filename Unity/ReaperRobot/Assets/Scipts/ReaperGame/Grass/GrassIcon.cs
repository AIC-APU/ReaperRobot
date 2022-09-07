using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(Grass))]
    public class GrassIcon : MonoBehaviour
    {
        private Grass _grass;
        private GameObject _grassIcon;

        void Awake()
        {
            _grass = GetComponent<Grass>();

            var iconPrefab = (GameObject)Resources.Load("GrassIcon");
            var iconPos = new Vector3(transform.position.x, iconPrefab.transform.position.y, transform.position.z);
            _grassIcon = Instantiate(iconPrefab, iconPos, iconPrefab.transform.rotation);

            _grass.IsCut.Subscribe(isCut =>
            {
                if (isCut)
                {
                    _grassIcon.GetComponent<SpriteRenderer>().color = Color.gray;
                }
                else
                {
                    _grassIcon.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }).AddTo(this);
        }
    }
}

