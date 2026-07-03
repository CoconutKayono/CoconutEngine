using System;
using TEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace GameLogic
{
    /// <summary>
    /// 重绑定服务 — 负责按键重绑定的逻辑
    /// </summary>
    public class RebindService : Singleton<RebindService>
    {
        #region Ref
        private InputSystem_Actions actions;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
        #endregion
        #region Event
        public event Action OnRebindStarted;
        public event Action<string> OnRebindCompleted;
        public event Action OnRebindCanceled;
        #endregion

        protected override void OnInit()
        {
            base.OnInit();
            actions = InputModule.Instance.Actions;
        }

        #region Actions
        /// <summary>
        /// 开始重绑定某个 Action 的指定绑定
        /// </summary>
        /// <param name="actionName">Action 名称（如 "Attack"）</param>
        /// <param name="bindingIndex">绑定索引（0 = keyboard, 1 = gamepad）</param>
        public void StartRebind(string actionName, int bindingIndex = 0)
        {
            var action = actions.FindAction(actionName);
            if (action == null)
            {
                Log.Error($"未找到 Action: {actionName}");
                return;
            }

            rebindingOperation?.Dispose();
            OnRebindStarted?.Invoke();

            rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithExpectedControlType<ButtonControl>()
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(OnRebindComplete)
                .OnCancel(OnRebindCancel)
                .Start();
        }

        private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            operation.Dispose();
            rebindingOperation = null;

            string bindingPath = operation.selectedControl?.path ?? "None";
            Log.Info($"重绑定完成: {bindingPath}");

            // 保存到 PlayerPrefs（或本地文件）
            SaveBindings();
            OnRebindCompleted?.Invoke(bindingPath);
        }

        private void OnRebindCancel(InputActionRebindingExtensions.RebindingOperation operation)
        {
            operation.Dispose();
            rebindingOperation = null;
            Log.Info("重绑定已取消");
            OnRebindCanceled?.Invoke();
        }

        /// <summary>
        /// 保存所有绑定覆盖到 PlayerPrefs
        /// </summary>
        public void SaveBindings()
        {
            string json = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("InputBindings", json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 从 PlayerPrefs 加载绑定覆盖
        /// </summary>
        public void LoadBindings()
        {
            if (PlayerPrefs.HasKey("InputBindings"))
            {
                string json = PlayerPrefs.GetString("InputBindings");
                actions.LoadBindingOverridesFromJson(json);
                Log.Info("绑定加载成功");
            }
        }

        /// <summary>
        /// 重置所有绑定到默认
        /// </summary>
        public void ResetAllBindings()
        {
            actions.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("InputBindings");
            Log.Info("所有绑定已重置");
        }

        /// <summary>
        /// 获取某个 Action 当前绑定的显示名称
        /// </summary>
        public string GetBindingDisplayString(string actionName, int bindingIndex = 0)
        {
            var action = actions.FindAction(actionName);
            if (action == null) return "未绑定";
            return action.GetBindingDisplayString(bindingIndex);
        }
        #endregion
    }
}
