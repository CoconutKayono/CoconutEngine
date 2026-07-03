using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 队伍模块（全局单例）
    /// 职责：管理当前战斗中所有角色的队伍数据，包括成员增删、切换选中、交换位置等。
    /// 队伍数据是战斗系统的核心数据源，所有与"当前上场角色"相关的操作都通过此模块进行。
    /// </summary>
    public class TeamModule : Singleton<TeamModule>
    {
        #region States
        private List<CharacterModule> team;
        private int maxMembers;
        private int currentIndex;
        #endregion
        #region Event                   
        public event Action<CharacterModule> OnMemberAdded;
        public event Action<int> OnMemberRemoved;               
        public event Action<int, int, CharacterModule> OnSelectMember;
        public event Action<int, int> OnSwapMembers;
        #endregion
        #region Getter,Setter
        public void InitTeam(List<CharacterModule> members)
        {
            if (members == null || members.Count == 0)
            {
                Log.Error("初始化队伍失败：成员列表为空！");
                return;
            }
            team = new List<CharacterModule>(members);
            currentIndex = 0;
        }

        public CharacterModule GetMember(int index)
        {
            return IsValidIndex(index, out var result) ? result : null;
        }

        public void SetMember(int index, CharacterModule character)
        {
            if (!IsValidIndex(index, out _)) return;
            team[index] = character;
        }

        public bool AddMember(CharacterModule character)
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

        public bool RemoveMember(int index)
        {
            if (!IsValidIndex(index, out _))
            {
                Log.Warning($"移除角色失败：索引 {index} 无效");
                return false;
            }

            // 如果要移除的是当前角色，自动切换到下一个
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

        public void SelectMember(int index)
        {
            if (currentIndex == index)
            {
                Log.Warning($"切换角色被忽略：索引 {index} 已是当前选中");
                return;
            }

            if (!IsValidIndex(index, out CharacterModule character))
            {
                Log.Warning($"切换角色失败：索引 {index} 无效");
                return;
            }

            int previousIndex = currentIndex;
            currentIndex = index;

            OnSelectMember?.Invoke(previousIndex, currentIndex, character);
        }

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

        public void Clear()
        {
            team.Clear();
            currentIndex = 0;
        }

        public CharacterModule Current => GetMember(currentIndex);
        public int MaxMembers => maxMembers;
        public int CurrentCount => team?.Count ?? 0;
        public bool IsFull => CurrentCount >= maxMembers;
        #endregion

        #region Utils
        private bool IsValidIndex(int index, out CharacterModule result)
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
