using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.Grass;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutGrass
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

            if (_grassCounter.CutGrassRatio == 1)
            {
                OnAllGrassCut();
            }
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
