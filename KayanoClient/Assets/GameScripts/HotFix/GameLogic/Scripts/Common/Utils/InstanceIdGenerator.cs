using UnityEngine;

namespace GameLogic
{
    public static class InstanceIdGenerator
    {
        private static int _nextId = 1;

        public static int Create()
        {
            return _nextId++;
        }

        public static void Reset()
        {
            _nextId = 1;
        }
    }
}
