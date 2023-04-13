using UnityEngine;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Grass
{
    [RequireComponent(typeof(Grass))]
    public class GrassIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _grassIconPrefab;
        private Grass _grass;
        private GameObject _grassIcon;

        void Awake()
        {
            _grass = GetComponent<Grass>();

            var iconHeight = _grassIconPrefab.transform.position.y;
            var iconPos = new Vector3(transform.position.x, iconHeight, transform.position.z);
            _grassIcon = Instantiate(_grassIconPrefab, iconPos, _grassIconPrefab.transform.rotation);

            _grass
                .IsCut
                .Subscribe(isCut =>
                {
                    if (isCut)
                    {
                        _grassIcon.GetComponent<SpriteRenderer>().color = Color.gray;
                    }
                    else
                    {
                        _grassIcon.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                })
                .AddTo(this);
        }
    }
}

