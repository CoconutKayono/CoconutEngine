using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 队伍模块（全局单例）
    /// 职责：管理当前战斗中所有角色的队伍数据，包括成员增删、切换选中、交换位置等。
    /// 同时管理队伍级别的决策服务（如 SwitchInService），因为这类服务控制的是队伍切换逻辑。
    /// </summary>
    public class TeamModule : Singleton<TeamModule>
    {
        #region States

        private List<CharacterStore> team;
        private int maxMembers;
        private int currentIndex;

        private readonly List<DecisionServiceBase> _decisionServices = new();

        #endregion

        #region 生命周期

        protected override void OnInit()
        {
            base.OnInit();

            // 注册队伍级别的决策服务（Chain 意图相关）
            _decisionServices.Add(new SwitchInService());
        }

        protected override void OnRelease()
        {
            foreach (var service in _decisionServices)
            {
                service?.Dispose();
            }

            _decisionServices.Clear();

            base.OnRelease();
        }

        #endregion

        #region Event                   
        public event Action<CharacterStore> OnMemberAdded;
        public event Action<int> OnMemberRemoved;
        public event Action<int, int, CharacterStore> OnSelectMember;
        public event Action<int, int> OnSwapMembers;
        #endregion

        #region 数据查询（Getter）

        /// <summary>
        /// 根据索引获取成员
        /// </summary>
        public CharacterStore GetMember(int index)
        {
            return IsValidIndex(index, out var result) ? result : null;
        }

        /// <summary>
        /// 当前选中的角色
        /// </summary>
        public CharacterStore Current => GetMember(currentIndex);

        /// <summary>
        /// 队伍最大人数
        /// </summary>
        public int MaxMembers => maxMembers;

        /// <summary>
        /// 当前队伍人数
        /// </summary>
        public int CurrentCount => team?.Count ?? 0;

        /// <summary>
        /// 队伍是否已满
        /// </summary>
        public bool IsFull => CurrentCount >= maxMembers;

        /// <summary>
        /// 获取下一个队伍成员（循环）
        /// </summary>
        public CharacterStore GetNextMember()
        {
            if (team == null || team.Count == 0) return null;
            if (team.Count == 1) return team[0];

            int nextIndex = (currentIndex + 1) % team.Count;
            return team[nextIndex];
        }

        /// <summary>
        /// 获取上一个队伍成员（循环）
        /// </summary>
        public CharacterStore GetPreviousMember()
        {
            if (team == null || team.Count == 0) return null;
            if (team.Count == 1) return team[0];

            int prevIndex = (currentIndex - 1 + team.Count) % team.Count;
            return team[prevIndex];
        }

        #endregion

        #region 数据修改（Action）

        /// <summary>
        /// 初始化队伍
        /// </summary>
        public void InitTeam(List<CharacterStore> members)
        {
            if (members == null || members.Count == 0)
            {
                Log.Error("初始化队伍失败：成员列表为空！");
                return;
            }
            team = new List<CharacterStore>(members);
            currentIndex = 0;
        }

        /// <summary>
        /// 设置指定索引的成员
        /// </summary>
        public void SetMember(int index, CharacterStore character)
        {
            if (!IsValidIndex(index, out _)) return;
            team[index] = character;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        public bool AddMember(CharacterStore character)
        {
            if (character == null)
            {
                Log.Error("添加成员失败：角色为空！");
                return false;
            }

            if (IsFull)
            {
                Log.Warning($"添加成员失败：队伍已满 ({maxMembers}/{maxMembers})");
                return false;
            }

            team.Add(character);
            OnMemberAdded?.Invoke(character);
            return true;
        }

        /// <summary>
        /// 移除成员
        /// </summary>
        public bool RemoveMember(int index)
        {
            if (!IsValidIndex(index, out _))
            {
                Log.Warning($"移除角色失败：索引 {index} 无效");
                return false;
            }

            bool isCurrent = (index == currentIndex);
            team.RemoveAt(index);

            if (team.Count == 0)
            {
                currentIndex = 0;
            }
            else if (isCurrent)
            {
                currentIndex = Mathf.Clamp(currentIndex, 0, team.Count - 1);
            }

            OnMemberRemoved?.Invoke(index);
            return true;
        }

        /// <summary>
        /// 选择指定索引的成员作为当前角色
        /// </summary>
        public void SelectMember(int index)
        {
            if (currentIndex == index)
            {
                Log.Warning($"切换角色被忽略：索引 {index} 已是当前选中");
                return;
            }

            if (!IsValidIndex(index, out CharacterStore character))
            {
                Log.Warning($"切换角色失败：索引 {index} 无效");
                return;
            }

            int previousIndex = currentIndex;
            currentIndex = index;

            OnSelectMember?.Invoke(previousIndex, currentIndex, character);
        }

        /// <summary>
        /// 切换到下一个队员（循环）
        /// </summary>
        public void SelectNextMember()
        {
            var next = GetNextMember();
            if (next != null && next != Current)
            {
                int index = team.FindIndex(m => m.InstanceId == next.InstanceId);
                if (index >= 0)
                {
                    SelectMember(index);
                }
            }
        }

        /// <summary>
        /// 切换到上一个队员（循环）
        /// </summary>
        public void SelectPreviousMember()
        {
            var prev = GetPreviousMember();
            if (prev != null && prev != Current)
            {
                int index = team.FindIndex(m => m.InstanceId == prev.InstanceId);
                if (index >= 0)
                {
                    SelectMember(index);
                }
            }
        }

        /// <summary>
        /// 交换两个成员的位置
        /// </summary>
        public void SwapMember(int indexA, int indexB)
        {
            if (!IsValidIndex(indexA, out var memberA)) return;
            if (!IsValidIndex(indexB, out var memberB)) return;
            if (indexA == indexB)
            {
                Log.Warning($"交换成员被忽略：索引 {indexA} 与自身交换");
                return;
            }

            team[indexA] = memberB;
            team[indexB] = memberA;

            if (currentIndex == indexA)
            {
                currentIndex = indexB;
            }
            else if (currentIndex == indexB)
            {
                currentIndex = indexA;
            }

            OnSwapMembers?.Invoke(indexA, indexB);
        }

        /// <summary>
        /// 清空队伍
        /// </summary>
        public void Clear()
        {
            team.Clear();
            currentIndex = 0;
        }

        #endregion

        #region Utils

        private bool IsValidIndex(int index, out CharacterStore result)
        {
            result = null;
            if (team == null)
            {
                Log.Error("队伍列表未初始化！");
                return false;
            }
            if (index < 0 || index >= team.Count)
            {
                Log.Warning($"索引 {index} 超出队伍范围 (0~{team.Count - 1})");
                return false;
            }
            result = team[index];
            return true;
        }

        #endregion
    }
}