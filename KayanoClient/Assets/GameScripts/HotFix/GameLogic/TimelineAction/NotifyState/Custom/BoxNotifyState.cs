using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace KayanoAction.Runtime
{
    [DisplayName("碰撞盒")]
    [NotifyStateDuration(0.25f)]
    public sealed class BoxNotifyState : ActionNotifyState
    {
        [NonSerialized] public GameObject owner;

        public EBoxType boxType = EBoxType.HitBox;
        public EBoxShape boxShape = EBoxShape.Sphere;
        public Vector3 center = new(0f, 1f, 1.5f);
        public float radius = 1f;
        public Vector3 size = new(2f, 2f, 2f);
        public EHitStrength hitStrength;
        public int hitId;
        public GameObject particlePrefab;

        [FormerlySerializedAs("setRot")]
        public bool setParticleRot;

        public float rotValue;
        public float rotMaxValue;
        public float hitGatherDist;
        public AudioClip hitAudio;

        [TimeField(60)]
        public float hitstopDuration = 0.05f;

        public float hitstopSpeed;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.owner = owner;
            return base.CreatePlayable(graph, owner);
        }
    }
}
