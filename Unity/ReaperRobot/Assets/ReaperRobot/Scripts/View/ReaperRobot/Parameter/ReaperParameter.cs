using UnityEngine;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    [CreateAssetMenu(menuName = "ReaperRobot/ReaperParameter")]
    public class ReaperParameter : ScriptableObject
    {
        public ReactiveProperty<float> MoveTorque = new(110f);
        public ReactiveProperty<float> BrakeTorque = new(500f);
        public ReactiveProperty<float> TorqueRateAtCutting = new(0.5f);
        public ReactiveProperty<Vector3> CenterOfGravity = new(new Vector3(0, 0, -0.2f));
        public ReactiveProperty<float> DampingRate = new(50f);
        public ReactiveProperty<float> RobotMath = new(1100f);
        public ReactiveProperty<float> ForwardFriction = new(1f);
        public ReactiveProperty<float> SidewaysFriction = new(1f);
    }
}
