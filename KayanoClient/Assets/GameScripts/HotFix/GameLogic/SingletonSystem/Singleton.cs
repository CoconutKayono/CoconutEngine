using System.Diagnostics;

namespace GameLogic
{
    /// <summary>
    /// 全局单例基类。
    /// </summary>
    /// <typeparam name="T">单例类型。</typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        protected static T _instance = default(T);

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new T();
                    _instance.OnInit();
                    SingletonSystem.Retain(_instance);
                }

                return _instance;
            }
        }

        public static bool IsValid => _instance != null;

        protected Singleton()
        {
#if UNITY_EDITOR
            string st = new StackTrace().ToString();
            // 使用常量字符串进行比较
            if (!st.Contains("GameLogic.Singleton`1[T].get_Instance"))
            {
                UnityEngine.Debug.LogError($"请通过 Instance 属性获取实例 {typeof(T).FullName}");
            }
#endif
        }

        protected virtual void OnInit()
        {
        }

        public virtual void Active()
        {
        }

        public virtual void Release()
        {
            OnRelease();
            if (_instance != null)
            {
                SingletonSystem.Release(_instance);
                _instance = null;
            }
        }

        protected virtual void OnRelease()
        {
        }
    }
}