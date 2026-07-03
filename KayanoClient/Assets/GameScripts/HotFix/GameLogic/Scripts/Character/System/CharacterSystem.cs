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
    public class CharacterSystem : MonoBehaviour, IDamageable
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
        private CharacterModule _characterModule;
        private IActionInputHandler _inputHandler;
        private ActionArbitrationService _arbitrationService;
        private TimelineActionService _timelineAction;
        private RootMotionService _rootMotionService;
        private RotationService _rotationService;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _animator ??= GetComponent<Animator>();

            if (_owner == null || _actionList == null || _actionList.Count == 0 || string.IsNullOrEmpty(_defaultAction))
            {
                Log.Error($"[CharacterSystem] {gameObject.name} 初始化失败：缺少必要引用");
                enabled = false;
                return;
            }

            _characterModule = new CharacterModule(
                _unitType,
                _configId,
                _actionList,
                _defaultAction,
                _animator,
                _owner
            );

            _timelineAction = new TimelineActionService(_characterModule);
            _inputHandler = new PlayerInputHandler(_characterModule);
            _arbitrationService = new ActionArbitrationService(_characterModule);
        }

        private void Update()
        {
            // 1. 执行层：推进 Timeline
            _timelineAction.Tick(Time.deltaTime);

            // 2. 输入处理：输入 → 意图事件
            _inputHandler.ProcessInput();

            // 3. 仲裁层：从候选中选出最终意图
            _arbitrationService.Tick();

        }

        private void LateUpdate()
        {
            
        }

        private void OnAnimatorMove()
        {

        }

        private void OnDestroy()
        {
            _inputHandler?.Dispose();
            _arbitrationService?.Dispose();

            _inputHandler = null;
            _arbitrationService = null;
            _timelineAction = null;
            _characterModule = null;
        }
        #endregion

        #region Getter
        /// <summary>角色模块（包含属性、动作状态）</summary>
        public CharacterModule CharacterModule => _characterModule;

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
        public bool IsInitialized => _characterModule != null;

        /// <summary>是否存活（由 CharacterModule 判断）</summary>
        public bool IsAlive => _characterModule?.CharacterAttributes?.IsAlive ?? false;

        /// <summary>当前动作名称</summary>
        public string CurrentActionName => _timelineAction?.CurrentAction?.actionName ?? "None";

        /// <summary>当前动作 ID</summary>
        public int CurrentActionId => _timelineAction?.CurrentAction?.actionId ?? 0;

        public int InstanceId => _characterModule?.InstanceId ?? 0;
        #endregion


        #region IDamageable
        public void TakeDamage(DamageContext context)
        {
            // 由 DamageService 处理，此处留空或转发
            Log.Warning($"[CharacterSystem] TakeDamage called on {gameObject.name}，由 DamageService 处理");
        }
        #endregion
    }
}