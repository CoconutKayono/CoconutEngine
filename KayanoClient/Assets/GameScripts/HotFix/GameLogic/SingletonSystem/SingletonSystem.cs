using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameLogic
{
    /// <summary>
    /// 单例生命周期接口。
    /// </summary>
    public interface ISingleton
    {
        /// <summary>
        /// 激活接口，通常在单例实例化后调用。
        /// </summary>
        void Active();

        /// <summary>
        /// 释放接口。
        /// </summary>
        void Release();
    }

    /// <summary>
    /// 每帧更新接口。
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// 每帧更新回调。
        /// </summary>
        void OnUpdate();
    }

    /// <summary>
    /// 固定时间步长更新接口（物理更新）。
    /// </summary>
    public interface IFixedUpdate
    {
        /// <summary>
        /// 固定时间步长更新回调。
        /// </summary>
        void OnFixedUpdate();
    }

    /// <summary>
    /// 延迟更新接口（LateUpdate）。
    /// </summary>
    public interface ILateUpdate
    {
        /// <summary>
        /// 延迟更新回调。
        /// </summary>
        void OnLateUpdate();
    }

    /// <summary>
    /// Gizmos 绘制接口。
    /// </summary>
    public interface IDrawGizmos
    {
        void OnDrawGizmos();
    }

    /// <summary>
    /// Gizmos 选中绘制接口。
    /// </summary>
    public interface IDrawGizmosSelected
    {
        void OnDrawGizmosSelected();
    }

    /// <summary>
    /// 全局单例系统，统一管理所有单例的生命周期、更新回调及游戏对象。
    /// </summary>
    public static class SingletonSystem
    {
        private static IUpdateDriver _updateDriver;
        private static readonly List<ISingleton> _singletons = new List<ISingleton>();
        private static readonly List<IUpdate> _updates = new List<IUpdate>();
        private static readonly List<IFixedUpdate> _fixedUpdates = new List<IFixedUpdate>();
        private static readonly List<ILateUpdate> _lateUpdates = new List<ILateUpdate>();
#if UNITY_EDITOR
        private static readonly List<IDrawGizmos> _drawGizmos = new List<IDrawGizmos>();
        private static readonly List<IDrawGizmosSelected> _drawGizmosSelecteds = new List<IDrawGizmosSelected>();
#endif

        private static readonly Dictionary<string, GameObject> _gameObjects = new Dictionary<string, GameObject>();

        public static void Retain(ISingleton singleton)
        {
            CheckInit();

            _singletons.Add(singleton);
            BuildLifeCycle(singleton);
        }

        public static void Retain(GameObject go, object singleton)
        {
            CheckInit();

            if (go == null)
            {
                Debug.LogError($"[SingletonSystem] Retain 失败：GameObject 为 null，单例类型 {singleton?.GetType()}");
                return;
            }

            if (_gameObjects.TryAdd(go.name, go))
            {
                if (Application.isPlaying)
                {
                    Object.DontDestroyOnLoad(go);
                }

                BuildLifeCycle(singleton);
            }
            else
            {
                Debug.LogWarning($"[SingletonSystem] 已存在同名 GameObject：{go.name}，跳过保留");
            }
        }

        private static void BuildLifeCycle(object singleton)
        {
            if (singleton == null) return;

            Type type = singleton.GetType();

            if (singleton is IUpdate update)
            {
                _updates.Add(update);
            }

            if (singleton is IFixedUpdate fixedUpdate)
            {
                _fixedUpdates.Add(fixedUpdate);
            }

            if (singleton is ILateUpdate lateUpdate)
            {
                _lateUpdates.Add(lateUpdate);
            }

#if UNITY_EDITOR
            if (singleton is IDrawGizmos drawGizmos)
            {
                _drawGizmos.Add(drawGizmos);
            }

            if (singleton is IDrawGizmosSelected drawGizmosSelected)
            {
                _drawGizmosSelecteds.Add(drawGizmosSelected);
            }
#endif
        }

        public static void Release(GameObject go, object singleton)
        {
            if (go != null && _gameObjects != null && _gameObjects.ContainsKey(go.name))
            {
                _gameObjects.Remove(go.name);
                Object.Destroy(go);
                ReleaseLifeCycle(singleton);
            }
        }

        public static void Release(ISingleton singleton)
        {
            if (_singletons != null && _singletons.Contains(singleton))
            {
                _singletons.Remove(singleton);
                ReleaseLifeCycle(singleton);
            }
        }

        private static void ReleaseLifeCycle(object singleton)
        {
            if (singleton == null) return;

            if (singleton is IUpdate update)
            {
                _updates.Remove(update);
            }

            if (singleton is IFixedUpdate fixedUpdate)
            {
                _fixedUpdates.Remove(fixedUpdate);
            }

            if (singleton is ILateUpdate lateUpdate)
            {
                _lateUpdates.Remove(lateUpdate);
            }

#if UNITY_EDITOR
            if (singleton is IDrawGizmos drawGizmos)
            {
                _drawGizmos.Remove(drawGizmos);
            }

            if (singleton is IDrawGizmosSelected drawGizmosSelected)
            {
                _drawGizmosSelecteds.Remove(drawGizmosSelected);
            }
#endif
        }

        public static void Release()
        {
            // 销毁所有管理的 GameObject
            if (_gameObjects != null)
            {
                var gameObjectSnapshot = new List<GameObject>(_gameObjects.Values);
                foreach (var gameObject in gameObjectSnapshot)
                {
                    if (gameObject != null)
                    {
                        Object.Destroy(gameObject);
                    }
                }

                _gameObjects.Clear();
            }

            // 释放所有单例
            if (_singletons != null)
            {
                var singletonSnapshot = new List<ISingleton>(_singletons);
                for (int i = singletonSnapshot.Count - 1; i >= 0; i--)
                {
                    singletonSnapshot[i]?.Release();
                }

                _singletons.Clear();
            }

            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();

#if UNITY_EDITOR
            _drawGizmos.Clear();
            _drawGizmosSelecteds.Clear();
#endif

            DeInit();
            Resources.UnloadUnusedAssets();
        }

        public static GameObject GetGameObject(string name)
        {
            if (string.IsNullOrEmpty(name) || _gameObjects == null)
                return null;

            _gameObjects.TryGetValue(name, out var go);
            return go;
        }

        internal static bool ContainsKey(string name)
        {
            return _gameObjects != null && _gameObjects.ContainsKey(name);
        }

        public static void Restart()
        {
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            Release();
            SceneManager.LoadScene(0);
        }

        internal static ISingleton GetSingleton(string name)
        {
            if (string.IsNullOrEmpty(name) || _singletons == null)
                return null;

            for (int i = 0; i < _singletons.Count; ++i)
            {
                // 通过类型名称或 ToString 匹配
                var singleton = _singletons[i];
                if (singleton != null && singleton.ToString() == name)
                {
                    return singleton;
                }
            }

            return null;
        }

        #region 生命周期驱动管理

        private static bool _isInit = false;

        private static void CheckInit()
        {
            if (_isInit) return;

            _isInit = true;

            _updateDriver ??= ModuleSystem.GetModule<IUpdateDriver>();
            if (_updateDriver == null)
            {
                Debug.LogError("[SingletonSystem] 无法获取 IUpdateDriver，更新回调将不会执行");
                return;
            }

            _updateDriver.AddUpdateListener(OnUpdate);
            _updateDriver.AddFixedUpdateListener(OnFixedUpdate);
            _updateDriver.AddLateUpdateListener(OnLateUpdate);
#if UNITY_EDITOR
            _updateDriver.AddOnDrawGizmosListener(OnDrawGizmos);
            _updateDriver.AddOnDrawGizmosSelectedListener(OnDrawGizmosSelected);
#endif
        }

        private static void DeInit()
        {
            if (!_isInit) return;

            _isInit = false;

            if (_updateDriver == null) return;

            _updateDriver.RemoveUpdateListener(OnUpdate);
            _updateDriver.RemoveFixedUpdateListener(OnFixedUpdate);
            _updateDriver.RemoveLateUpdateListener(OnLateUpdate);
#if UNITY_EDITOR
            _updateDriver.RemoveOnDrawGizmosListener(OnDrawGizmos);
            _updateDriver.RemoveOnDrawGizmosSelectedListener(OnDrawGizmosSelected);
#endif
        }

        private static void OnUpdate()
        {
            foreach (var update in _updates)
            {
                update?.OnUpdate();
            }
        }

        private static void OnFixedUpdate()
        {
            foreach (var fixedUpdate in _fixedUpdates)
            {
                fixedUpdate?.OnFixedUpdate();
            }
        }

        private static void OnLateUpdate()
        {
            foreach (var lateUpdate in _lateUpdates)
            {
                lateUpdate?.OnLateUpdate();
            }
        }

#if UNITY_EDITOR
        private static void OnDrawGizmos()
        {
            foreach (var drawGizmo in _drawGizmos)
            {
                drawGizmo?.OnDrawGizmos();
            }
        }

        private static void OnDrawGizmosSelected()
        {
            foreach (var drawGizmosSelected in _drawGizmosSelecteds)
            {
                drawGizmosSelected?.OnDrawGizmosSelected();
            }
        }
#endif

        #endregion
    }
}