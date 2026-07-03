using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace KayanoAction.Runtime
{
    [DisplayName("粒子")]
    public sealed class PlayParticleNotify : ActionNotify, INotification
    {
        private static readonly PropertyName k_Id = new(nameof(PlayParticleNotify));

        public GameObject prefab;
        public Vector3 localPosition;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localScale = Vector3.one;

        PropertyName INotification.id => k_Id;
    }
}
