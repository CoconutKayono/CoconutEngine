using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TEngine;
using UnityEngine;
#if ENABLE_OBFUZ
using Obfuz;
#endif

namespace GameLogic
{
    /// <summary>
    /// UI基类。
    /// </summary>
#if ENABLE_OBFUZ
    [ObfuzIgnore(ObfuzScope.TypeName, ApplyToChildTypes = true)]
#endif
    public class UIBase
    {
        /// <summary>
        /// 外部注入回调，允许外部为每一个 UIBase 实例委托，
        /// 在 UI 初始化或创建时由框架调用注入的方法/属性
        /// </summary>
        public static Action<UIBase>? Injector;

        /// <summary>
        /// UI类型。
        /// </summary>
        public enum UIType
        {
            /// <summary>
            /// 无类型。
            /// </summary>
            None,

            /// <summary>
            /// 窗口Windows。
            /// </summary>
            Window,

            /// <summary>
            /// 控件Widget。
            /// </summary>
            Widget,
        }

        /// <summary>
        /// 父级UI节点。
        /// </summary>
        protected UIBase _parent = null;

        /// <summary>
        /// UI父节点。
        /// </summary>
        public UIBase Parent => _parent;

        /// <summary>
        /// 自定义数据集合
        /// </summary>
        protected System.Object[] _userDatas;

        /// <summary>
        /// 自定义数据。
        /// </summary>
        public System.Object UserData
        {
            get
            {
                if (_userDatas != null && _userDatas.Length >= 1)
                {
                    return _userDatas[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 自定义数据集合
        /// </summary>
        public System.Object[] UserDatas => _userDatas;

        /// <summary>
        /// 节点的实例资源对象
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public virtual GameObject gameObject { protected set; get; }

        /// <summary>
        /// 节点位置变换
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public virtual Transform transform { protected set; get; }

        /// <summary>
        /// 节点矩形位置变换
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public virtual RectTransform rectTransform { protected set; get; }

        /// <summary>
        /// UI类型。
        /// </summary>
        public virtual UIType Type => UIType.None;

        /// <summary>
        /// 资源是否准备完毕。
        /// </summary>
        public bool IsPrepare { protected set; get; }

        /// <summary>
        /// UI子控件列表
        /// </summary>
        internal readonly List<UIWidget> ListChild = new List<UIWidget>();

        /// <summary>
        /// 需要Update更新的UI子控件列表
        /// </summary>
        protected List<UIWidget> _listUpdateChild = null;

        /// <summary>
        /// 是否需要Update行为
        /// </summary>
        protected bool _updateListValid = false;

        /// <summary>
        /// 是否正在排序
        /// </summary>
        protected bool _isSortingOrderDirty = false;

        /// <summary>
        /// 依赖注入。
        /// </summary>
        protected void Inject()
        {
            Injector?.Invoke(this);
        }

        /// <summary>
        /// 进行自定义绑定。
        /// </summary>
        protected virtual void ScriptGenerator()
        {
        }

        /// <summary>
        /// 绑定UI成员元素。
        /// </summary>
        protected virtual void BindMemberProperty()
        {
        }

        /// <summary>
        /// 注册事件。
        /// </summary>
        protected virtual void RegisterEvent()
        {
        }

        /// <summary>
        /// 节点创建。
        /// </summary>
        protected virtual void OnCreate()
        {
        }

        /// <summary>
        /// 界面刷新。
        /// </summary>
        protected virtual void OnRefresh()
        {
        }

        /// <summary>
        /// 是否需要Update。
        /// </summary>
        protected bool _hasOverrideUpdate = true;

        /// <summary>
        /// 节点更新。
        /// </summary>
        protected virtual void OnUpdate()
        {
            _hasOverrideUpdate = false;
        }

        internal void CallDestroy()
        {
            OnDestroy();
        }

        /// <summary>
        /// 节点销毁。
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 更新子节点的层级排序
        /// </summary>
        protected void _OnSortDepth()
        {
            if (ListChild != null)
            {
                for (int i = 0; i < ListChild.Count; i++)
                {
                    ListChild[i].OnSortDepth();
                }
            }

            OnSortDepth();
        }


        /// <summary>
        /// 更新子节点的层级排序
        /// </summary>
        protected virtual void OnSortDepth()
        {
        }

        /// <summary>
        /// 根据父级节点的大小节点可见性递归子节点遍历
        /// </summary>
        protected virtual void OnSetVisible(bool visible)
        {
        }

        internal void SetUpdateDirty()
        {
            _updateListValid = false;
            if (Parent != null)
            {
                Parent.SetUpdateDirty();
            }
        }

        #region FindChildComponent

        public Transform FindChild(string path)
        {
            return FindChildImp(rectTransform, path);
        }

        public Transform FindChild(Transform trans, string path)
        {
            return FindChildImp(trans, path);
        }

        public T FindChildComponent<T>(string path) where T : Component
        {
            return FindChildComponentImp<T>(rectTransform, path);
        }

        public T FindChildComponent<T>(Transform trans, string path) where T : Component
        {
            return FindChildComponentImp<T>(trans, path);
        }

        private static Transform FindChildImp(Transform transform, string path)
        {
            var findTrans = transform.Find(path);
            return findTrans != null ? findTrans : null;
        }

        private static T FindChildComponentImp<T>(Transform transform, string path) where T : Component
        {
            var findTrans = transform.Find(path);
            if (findTrans != null)
            {
                return findTrans.gameObject.GetComponent<T>();
            }

            return null;
        }

        #endregion

        #region UIEvent

        private GameEventMgr _eventMgr;

        protected GameEventMgr EventMgr
        {
            get
            {
                if (_eventMgr == null)
                {
                    _eventMgr = MemoryPool.Acquire<GameEventMgr>();
                }

                return _eventMgr;
            }
        }

        public void AddUIEvent(int eventType, Action handler)
        {
            EventMgr.AddEvent(eventType, handler);
        }

        protected void AddUIEvent<T>(int eventType, Action<T> handler)
        {
            EventMgr.AddEvent(eventType, handler);
        }

        protected void AddUIEvent<T, U>(int eventType, Action<T, U> handler)
        {
            EventMgr.AddEvent(eventType, handler);
        }

        protected void AddUIEvent<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            EventMgr.AddEvent(eventType, handler);
        }

        protected void AddUIEvent<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            EventMgr.AddEvent(eventType, handler);
        }

        protected void RemoveAllUIEvent()
        {
            if (_eventMgr != null)
            {
                MemoryPool.Release(_eventMgr);
            }
        }

        #endregion

        #region UIWidget

        /// <summary>
        /// 创建UIWidget通过UI位置节点。
        /// <remarks>因为资源实例已经存在节点所以不需要异步</remarks>
        /// </summary>
        /// <param name="goPath">绑UI位置节点。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidget<T>(string goPath, bool visible = true) where T : UIWidget, new()
        {
            var goRootTrans = FindChild(goPath);

            if (goRootTrans != null)
            {
                return CreateWidget<T>(goRootTrans.gameObject, visible);
            }

            return null;
        }


        /// <summary>
        /// 创建UIWidget通过UI位置节点。
        /// <remarks>因为资源实例已经存在节点所以不需要异步</remarks>
        /// </summary>
        /// <param name="parentTrans"></param>
        /// <param name="goPath">绑UI位置节点。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidget<T>(Transform parentTrans, string goPath, bool visible = true) where T : UIWidget, new()
        {
            var goRootTrans = FindChild(parentTrans, goPath);
            if (goRootTrans != null)
            {
                return CreateWidget<T>(goRootTrans.gameObject, visible);
            }

            return null;
        }

        /// <summary>
        /// 创建UIWidget通过游戏物体。
        /// <remarks>因为资源实例已经存在节点所以不需要异步</remarks>
        /// </summary>
        /// <param name="goRoot">游戏物体。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidget<T>(GameObject goRoot, bool visible = true) where T : UIWidget, new()
        {
            var widget = new T();
            if (widget.Create(this, goRoot, visible))
            {
                return widget;
            }

            return null;
        }

        /// <summary>
        /// 创建UIWidget通过资源定位地址。
        /// </summary>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="assetLocation">资源定位地址。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidgetByPath<T>(Transform parentTrans, string assetLocation, bool visible = true) where T : UIWidget, new()
        {
            GameObject goInst = UIModule.Resource.LoadGameObject(assetLocation, parent: parentTrans);
            return CreateWidget<T>(goInst, visible);
        }

        /// <summary>
        /// 异步创建UIWidget通过资源定位地址。
        /// </summary>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="assetLocation">资源定位地址。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public async UniTask<T> CreateWidgetByPathAsync<T>(Transform parentTrans, string assetLocation, bool visible = true) where T : UIWidget, new()
        {
            GameObject goInst = await UIModule.Resource.LoadGameObjectAsync(assetLocation, parentTrans, gameObject.GetCancellationTokenOnDestroy());
            return CreateWidget<T>(goInst, visible);
        }

        /// <summary>
        /// 根据prefab资源模板来创建新的 widget。
        /// </summary>
        /// <param name="goPrefab">资源预制对象</param>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidgetByPrefab<T>(GameObject goPrefab, Transform parentTrans = null, bool visible = true) where T : UIWidget, new()
        {
            var widget = new T();
            if (!widget.CreateByPrefab(this, goPrefab, parentTrans, visible))
            {
                return null;
            }

            return widget;
        }

        /// <summary>
        /// 通过UI类型创建widget。
        /// </summary>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public T CreateWidgetByType<T>(Transform parentTrans, bool visible = true) where T : UIWidget, new()
        {
            return CreateWidgetByPath<T>(parentTrans, typeof(T).Name, visible);
        }

        /// <summary>
        /// 异步通过UI类型创建widget。
        /// </summary>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="visible">是否可见</param>
        /// <typeparam name="T">UIWidget类型</typeparam>
        /// <returns>UIWidget实例对象</returns>
        public async UniTask<T> CreateWidgetByTypeAsync<T>(Transform parentTrans, bool visible = true) where T : UIWidget, new()
        {
            return await CreateWidgetByPathAsync<T>(parentTrans, typeof(T).Name, visible);
        }

        /// <summary>
        /// 调整图标数量
        /// </summary>
        /// <remarks>给Icon专用</remarks>
        /// <param name="listIcon">待Icon的列表</param>
        /// <param name="number">调整数目</param>
        /// <param name="parentTrans">资源父节点。</param>
        /// <param name="prefab">资源预制体</param>
        /// <param name="assetPath">资源地址。</param>
        /// <typeparam name="T">图标类型。</typeparam>
        public void AdjustIconNum<T>(List<T> listIcon, int number, Transform parentTrans, GameObject prefab = null, string assetPath = "")
            where T : UIWidget, new()
        {
            if (listIcon == null)
            {
                listIcon = new List<T>();
            }

            if (listIcon.Count < number)
            {
                int needNum = number - listIcon.Count;
                for (int iconIdx = 0; iconIdx < needNum; iconIdx++)
                {
                    T tmpT = prefab == null ? CreateWidgetByType<T>(parentTrans) : CreateWidgetByPrefab<T>(prefab, parentTrans);
                    listIcon.Add(tmpT);
                }
            }
            else if (listIcon.Count > number)
            {
                RemoveUnUseItem<T>(listIcon, number);
            }
        }

        /// <summary>
        /// 异步调整图标数量
        /// </summary>
        /// <param name="listIcon"></param>
        /// <param name="tarNum"></param>
        /// <param name="parentTrans"></param>
        /// <param name="prefab"></param>
        /// <param name="assetPath"></param>
        /// <param name="maxNumPerFrame"></param>
        /// <param name="updateAction"></param>
        /// <typeparam name="T"></typeparam>
        public void AsyncAdjustIconNum<T>(List<T> listIcon, int tarNum, Transform parentTrans, GameObject prefab = null,
            string assetPath = "", int maxNumPerFrame = 5,
            Action<T, int> updateAction = null) where T : UIWidget, new()
        {
            AsyncAdjustIconNumInternal(listIcon, tarNum, parentTrans, maxNumPerFrame, updateAction, prefab, assetPath).Forget();
        }

        /// <summary>
        /// 异步接口。
        /// </summary>
        /// <param name="listIcon"></param>
        /// <param name="tarNum"></param>
        /// <param name="parentTrans"></param>
        /// <param name="maxNumPerFrame"></param>
        /// <param name="updateAction"></param>
        /// <param name="prefab"></param>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        private async UniTaskVoid AsyncAdjustIconNumInternal<T>(List<T> listIcon, int tarNum, Transform parentTrans, int maxNumPerFrame,
            Action<T, int> updateAction, GameObject prefab, string assetPath) where T : UIWidget, new()
        {
            if (listIcon == null)
            {
                listIcon = new List<T>();
            }

            int createCnt = 0;

            for (int i = 0; i < tarNum; i++)
            {
                T tmpT = null;
                if (i < listIcon.Count)
                {
                    tmpT = listIcon[i];
                }
                else
                {
                    if (prefab == null)
                    {
                        tmpT = await CreateWidgetByPathAsync<T>(parentTrans, assetPath);
                    }
                    else
                    {
                        tmpT = CreateWidgetByPrefab<T>(prefab, parentTrans);
                    }

                    listIcon.Add(tmpT);
                }

                int index = i;
                if (updateAction != null)
                {
                    updateAction(tmpT, index);
                }

                createCnt++;
                if (createCnt >= maxNumPerFrame)
                {
                    createCnt = 0;
                    await UniTask.Yield();
                }
            }

            if (listIcon.Count > tarNum)
            {
                RemoveUnUseItem(listIcon, tarNum);
            }
        }

        private void RemoveUnUseItem<T>(List<T> listIcon, int tarNum) where T : UIWidget
        {
            var removeIcon = new List<T>();
            for (int iconIdx = 0; iconIdx < listIcon.Count; iconIdx++)
            {
                var icon = listIcon[iconIdx];
                if (iconIdx >= tarNum)
                {
                    removeIcon.Add(icon);
                }
            }

            for (var index = 0; index < removeIcon.Count; index++)
            {
                var icon = removeIcon[index];
                listIcon.Remove(icon);
                icon.OnDestroy();
                icon.OnDestroyWidget();
                ListChild.Remove(icon);
                if (icon.gameObject != null)
                {
                    UnityEngine.Object.Destroy(icon.gameObject);
                }
            }
        }

        #endregion
    }
}