using UnityEngine;

namespace GameLogic
{
    public interface IActionInputHandler
    {
        void ProcessInput();

        void Dispose();
    }
}
