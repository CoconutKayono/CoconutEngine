using GameLogic;
using TEngine;
using UnityEngine;

namespace KayanoAction.Runtime
{
    public sealed class BoxNotifyStateSO : NotifyStateSO
    {
        public EBoxType boxType = EBoxType.HitBox;
        public EBoxShape boxShape = EBoxShape.Sphere;
        public Vector3 center = new(0f, 1f, 1.5f);
        public float radius = 1f;
        public Vector3 size = new(2f, 2f, 2f);
        public EHitStrength hitStrength;
        public int hitId;
        public GameObject particlePrefab;
        public bool setParticleRot;
        public float rotValue;
        public float rotMaxValue;
        public float hitGatherDist;
        public string hitAudioPath;
        public float hitstopDuration = 0.05f;
        public float hitstopSpeed;

        public BoxData ToBoxData()
        {
            return new BoxData
            {
                Center = center,
                Radius = radius,
                Size = size,
                Shape = boxShape,
                HitStrength = hitStrength,
                HitId = hitId,
                HitGatherDist = hitGatherDist,
                HitAudio = hitAudioPath,
                HitstopDuration = hitstopDuration,
                HitstopSpeed = hitstopSpeed,
            };
        }

        public override void Enter(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnBoxNotifyBegin(new BoxNotifyBeginEvent
            {
                Context = ctx,
                Box = ToBoxData(),
                StateInstanceId = instanceId,
            });
        }

        public override void Exit(in ActionTimelineContext ctx, int instanceId)
        {
            GameEvent.Get<IActionTimelineEvents>().OnBoxNotifyEnd(new BoxNotifyEndEvent
            {
                Context = ctx,
                Box = ToBoxData(),
                StateInstanceId = instanceId,
            });
        }
    }
}
