using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 输入模块（全局单例）— 组装 Input System、实时快照与输入缓冲。
    /// </summary>
    public class InputModule : Singleton<InputModule>, IUpdate
    {
        #region States
        private InputSystem_Actions actions;
        private InputSystem_Actions.PlayerActions PlayerActions;
        private InputSystem_Actions.UIActions UIActions;

        private InputStateModel _inputState;
        private InputStateService _stateService;
        private InputBufferService _bufferService;
        #endregion

        #region Unity LifeCycle
        protected override void OnInit()
        {
            base.OnInit();
            actions = new InputSystem_Actions();
            PlayerActions = actions.Player;
            UIActions = actions.UI;
            actions.Enable();

            _stateService = new InputStateService();
            _bufferService = new InputBufferService();
        }

        public void OnUpdate()
        {
            if (actions == null)
            {
                Log.Warning("Action列表为空，跳过执行");
                return;
            }

            _stateService.Tick(PlayerActions, ref _inputState);
            _bufferService.Tick(PlayerActions, _inputState);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            if (actions != null)
            {
                actions.Disable();
                actions.Dispose();
                actions = null;
            }

            _bufferService?.Buffer.Clear();
        }
        #endregion

        #region Getter,Setter
        public InputSystem_Actions Actions => actions;
        public InputSystem_Actions.PlayerActions Player => PlayerActions;
        public InputSystem_Actions.UIActions UI => UIActions;
        public InputStateModel Input => _inputState;
        public InputBufferModel Buffer => _bufferService.Buffer;
        #endregion
    }
}
