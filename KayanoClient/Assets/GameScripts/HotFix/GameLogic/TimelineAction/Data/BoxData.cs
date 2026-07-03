using KayanoAction.Runtime;
using UnityEngine;

namespace GameLogic
{
    public struct BoxData
    {
        public Vector3 Center;
        public float Radius;
        public Vector3 Size;
        public EBoxShape Shape;
        public EHitStrength HitStrength;
        public int HitId;
        public float HitGatherDist;
        public string HitAudio;
        public float HitstopDuration;
        public float HitstopSpeed;
    }
}
