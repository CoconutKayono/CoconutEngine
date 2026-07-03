using KayanoAction.Runtime;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 角色系统 — 胶水代码，组装并驱动单个角色的战斗行为
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterComponent : MonoBehaviour, IDamageable
    {
        #region Inspector
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _owner;
        [SerializeField] private int _configId;
        [SerializeField] private EUnitType _unitType = EUnitType.Player;
        [SerializeField] private List<TimelineActionSO> _actionList;
        [SerializeField] private string _defaultAction = "Idle";
        #endregion

        #region States
        private CharacterStore _characterStore;
        private IActionInputHandler _inputHandler;
        private ActionArbitrationService _arbitrationService;
        private TimelineActionService _timelineAction;
        private RootMotionService _rootMotionService;
        private RotationService _rotationService;
        private MoveDirectionService _moveDirectionService;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _animator ??= GetComponent<Animator>();

            if (_owner == null || _actionList == null || _actionList.Count == 0 || string.IsNullOrEmpty(_defaultAction))
            {
                Log.Error($"[CharacterComponent] {gameObject.name} 初始化失败：缺少必要引用");
                enabled = false;
                return;
            }

            // 1. 创建数据模块
            _characterStore = new CharacterStore(
                _unitType,
                _configId,
                _actionList,
                _defaultAction,
                _animator,
                _owner
            );

            CharacterModule.Instance.RegisterUnit(_characterStore, _unitType);

            // 2. 创建执行层服务
            _timelineAction = new TimelineActionService(_characterStore);
            _rootMotionService = new RootMotionService(_characterStore);

            // 3. 创建输入与仲裁服务
            _inputHandler = new PlayerInputHandler(_characterStore);
            _arbitrationService = new ActionArbitrationService(_characterStore);

            // 4. 创建方向与旋转服务
            _moveDirectionService = new MoveDirectionService(_characterStore);
            _rotationService = new RotationService(_characterStore);
        }

        private void Update()
        {
            // 1. 执行层：推进 Timeline
            _timelineAction.Tick(Time.deltaTime);

            // 2. 更新移动方向（将输入写入 ChStateModel）
            var input = InputModule.Instance.Input;
            _moveDirectionService.UpdateDirection(input.Move);

            // 3. 输入处理：输入 → 意图事件
            _inputHandler.ProcessInput();

            // 4. 仲裁层：从候选中选出最终意图
            _arbitrationService.Tick();

            // 5. 旋转处理：根据当前移动方向旋转角色
            _rotationService.RotateTowards();
        }

        private void OnAnimatorMove()
        {
            _rootMotionService?.OnAnimatorMove();
        }

        private void OnDestroy()
        {
            if (_characterStore != null)
            {
                CharacterModule.Instance.UnregisterUnit(_characterStore.InstanceId);
            }

            _inputHandler?.Dispose();
            _arbitrationService?.Dispose();
            _rootMotionService?.Dispose();
            _rotationService?.Dispose();
            _moveDirectionService?.Dispose();

            _inputHandler = null;
            _arbitrationService = null;
            _rootMotionService = null;
            _rotationService = null;
            _moveDirectionService = null;
            _timelineAction = null;
            _characterStore = null;
        }
        #endregion

        #region Getter
        /// <summary>角色模块（包含属性、动作状态）</summary>
        public CharacterStore CharacterStore => _characterStore;

        /// <summary>动作服务（推进 Timeline）</summary>
        public TimelineActionService TimelineAction => _timelineAction;

        /// <summary>动画控制器</summary>
        public Animator Animator => _animator;

        /// <summary>角色挂载根节点</summary>
        public Transform Owner => _owner;

        /// <summary>配置 ID</summary>
        public int ConfigId => _configId;

        /// <summary>单位类型</summary>
        public EUnitType UnitType => _unitType;

        /// <summary>是否已初始化（模块不为空）</summary>
        public bool IsInitialized => _characterStore != null;

        /// <summary>是否存活（由 CharacterModule 判断）</summary>
        public bool IsAlive => _characterStore?.ChAttribute?.IsAlive ?? false;

        /// <summary>当前动作名称</summary>
        public string CurrentActionName => _timelineAction?.CurrentAction?.actionName ?? "None";

        /// <summary>当前动作 ID</summary>
        public int CurrentActionId => _timelineAction?.CurrentAction?.actionId ?? 0;

        public int InstanceId => _characterStore?.InstanceId ?? 0;
        #endregion

        #region IDamageable
        public void TakeDamage(DamageContext context)
        {
            // 由 DamageService 处理，此处留空或转发
            Log.Warning($"[CharacterComponent] TakeDamage called on {gameObject.name}，由 DamageService 处理");
        }
        #endregion
    }
}
