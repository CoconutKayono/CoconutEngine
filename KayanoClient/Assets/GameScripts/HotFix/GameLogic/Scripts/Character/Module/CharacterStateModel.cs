using GameConfig.Main;
using UnityEngine;

namespace GameLogic
{
    [System.Serializable]
    public class CharacterStateModel
    {
        private ChMotionConfig _chMotionConfig;

        public CharacterStateModel(ChMotionConfig chMotionConfig)
        {
            _chMotionConfig = chMotionConfig;
        }

        public ChMotionConfig ChMotionConfig => _chMotionConfig;

        public float RotationSpeed => _chMotionConfig.RotationSpeed;

        public float WalkToRunTime => _chMotionConfig.WalkToRunTime;

        public float TurnThreshold => _chMotionConfig.TurnThreshold;
    }
}