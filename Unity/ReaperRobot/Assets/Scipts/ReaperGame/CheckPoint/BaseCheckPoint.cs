using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public abstract class BaseCheckPoint : MonoBehaviour
    {
        public abstract string Introduction { get; }
        
        public IReadOnlyReactiveProperty<bool> IsChecked  => _isChecked;
        protected ReactiveProperty<bool> _isChecked = new(false);

        public abstract void SetUp();

        public abstract void OnChecked();
    }

}
