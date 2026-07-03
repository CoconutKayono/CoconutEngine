using System.ComponentModel;
using UnityEngine;

namespace KayanoAction.Runtime
{
    [DisplayName("粒子")]
    public sealed class PlayParticleNotifyState : ActionNotifyState
    {
        public GameObject prefab;
        public Vector3 localPosition;
        public Quaternion localRotation = Quaternion.identity;
        public Vector3 localScale = Vector3.one;
    }
}
