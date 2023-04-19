using UnityEngine;
using Plusplus.ReaperRobot.Scripts.UnityComponent.Grass;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CheckPoint.AboutGrass
{
    public class AllCutCheckPoint : BaseCheckPoint
    {
        #region Public Fields
        [SerializeField] private GrassCounter _grassCounter;
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _grassCounter.OnAllGrassCut.AddListener(OnAllGrassCut);
        }

        public override void FinalizeCheckPoint()
        {
            _grassCounter.OnAllGrassCut.RemoveListener(OnAllGrassCut);
        }
        #endregion

        #region Private method
        private void OnAllGrassCut()
        {
            _isChecked.Value = true;
        }
        #endregion
    }
}
