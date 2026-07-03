#if UNITY_EDITOR

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GameConfig.Main;
using GameLogic;
using KayanoAction.Runtime;
using Luban;
using TEngine.Editor.ActionModule;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor.TimelineAction
{
    public static class ActionBakePipeline
    {
        public static TimelineActionSO Bake(
            ActionTimeline timeline,
            TimelineCatalogSO catalog,
            string outputFolder = null)
        {
            if (timeline == null)
            {
                return null;
            }

            var actionName = ResolveActionName(timeline.name, catalog);

            var runtime = FindOrCreateRuntimeAsset(timeline, outputFolder);
            CopyMetadata(timeline, runtime, actionName);
            ValidateRuntime(timeline, runtime);
            BakeSchedule(timeline, runtime);
            ExportServerActionJson(timeline, runtime);
            EditorUtility.SetDirty(runtime);
            AssetDatabase.SaveAssets();
            return runtime;
        }

        public static int BakeTimelines(
            IEnumerable<ActionTimeline> timelines,
            TimelineCatalogSO catalog,
            string outputFolder)
        {
            if (timelines == null)
            {
                return 0;
            }

            var baked = new List<TimelineActionSO>();
            foreach (var timeline in timelines)
            {
                if (timeline == null)
                {
                    continue;
                }

                var runtime = Bake(timeline, catalog, outputFolder);
                if (runtime != null)
                {
                    baked.Add(runtime);
                }
            }

            if (catalog != null && baked.Count > 0)
            {
                catalog.actions.Clear();
                catalog.actions.AddRange(baked);
                EditorUtility.SetDirty(catalog);
                KayanoActionCatalogSOEditor.ValidateCatalogOrLogError(catalog);
            }

            AssetDatabase.SaveAssets();
            return baked.Count;
        }

        private static TimelineActionSO FindOrCreateRuntimeAsset(
            ActionTimeline timeline,
            string outputFolder)
        {
            var timelinePath = AssetDatabase.GetAssetPath(timeline);
            var folder = string.IsNullOrEmpty(outputFolder)
                ? Path.GetDirectoryName(timelinePath)
                : outputFolder;
            if (string.IsNullOrEmpty(folder))
            {
                folder = "Assets";
            }

            var fileName = timeline.name + "_Runtime.asset";
            var assetPath = Path.Combine(folder, fileName).Replace('\\', '/');

            var existing = AssetDatabase.LoadAssetAtPath<TimelineActionSO>(assetPath);
            if (existing != null)
            {
                return existing;
            }

            var runtime = ScriptableObject.CreateInstance<TimelineActionSO>();
            AssetDatabase.CreateAsset(runtime, assetPath);
            return runtime;
        }

        private static void CopyMetadata(
            ActionTimeline timeline,
            TimelineActionSO runtime,
            string actionName)
        {
            runtime.actionId = timeline.actionId;
            runtime.actionName = actionName;
            runtime.primaryIntent = timeline.primaryIntent;
            runtime.isLoop = timeline.isLoop;
            runtime.enableRotation = timeline.enableRotation;
            runtime.enableRecenter = timeline.enableRecenter;
            runtime.enableLookAtMonster = timeline.enableLookAtMonster;
            ApplyActionArgDefaults(runtime);
            runtime.inheritActionName = timeline.inheritActionTransition;
            runtime.finishTransition = timeline.finishTransition;
            runtime.commandTransitions = timeline.commandTransitions != null
                ? new List<CommandTransitionInfo>(timeline.commandTransitions)
                : new List<CommandTransitionInfo>();
            runtime.signalTransitions = timeline.signalTransitions != null
                ? new List<SignalTransitionInfo>(timeline.signalTransitions)
                : new List<SignalTransitionInfo>();
        }

        private static void ValidateRuntime(ActionTimeline timeline, TimelineActionSO runtime)
        {
            if (!runtime.isLoop)
            {
                var finish = runtime.finishTransition;
                if (finish == null || string.IsNullOrEmpty(finish.actionName))
                {
                    Debug.LogError(
                        $"[ActionBakePipeline] {timeline.name}（{runtime.actionName}）为非 loop 动作，但未配置 finishTransition.actionName。请在 Timeline 配置收尾转移。");
                }
            }

            ValidateActionNumeric(timeline, runtime);
        }

        private static void ValidateActionNumeric(ActionTimeline timeline, TimelineActionSO runtime)
        {
            if (!RequiresActionNumeric(timeline))
            {
                return;
            }

            if (timeline.actionId <= 0)
            {
                Debug.LogError(
                    $"[ActionBakePipeline] {timeline.name}（{runtime.actionName}）含 HitBox 或战斗主意图，但未配置 actionId。"
                    + "请在 Timeline 填写「动作 ID」，并在 TbChActionConfig 登记 buff_tags。");
                return;
            }

            if (!TryLoadBuffTagsInEditor(timeline.actionId, out var buffTags))
            {
                Debug.LogError(
                    $"[ActionBakePipeline] actionId={timeline.actionId}（{runtime.actionName}）未在 TbChActionConfig 登记。");
                return;
            }

            if (buffTags == EActionBuffTag.None)
            {
                Debug.LogError(
                    $"[ActionBakePipeline] actionId={timeline.actionId}（{runtime.actionName}）的 buff_tags 为 None，"
                    + "战斗招式请在 TbChActionConfig 配置 buff_tags。");
            }
        }

        private static bool TryLoadBuffTagsInEditor(int actionId, out EActionBuffTag buffTags)
        {
            buffTags = EActionBuffTag.None;
            if (actionId <= 0)
            {
                return false;
            }

            const string bytesPath = "Assets/AssetRaw/Configs/bytes/main_tbchactionconfig.bytes";
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(bytesPath);
            if (asset == null)
            {
                Debug.LogWarning(
                    $"[ActionBakePipeline] 未找到 {bytesPath}，跳过 TbChActionConfig 校验。请先转表。");
                return true;
            }

            var table = new TbChActionConfig(new ByteBuf(asset.bytes));
            if (!table.DataMap.TryGetValue(actionId, out var config) || config == null)
            {
                return false;
            }

            buffTags = config.BuffTags;
            return true;
        }

        private static bool RequiresActionNumeric(ActionTimeline timeline)
        {
            if (IsCombatPrimaryIntent(timeline.primaryIntent))
            {
                return true;
            }

            return TimelineHasHitBox(timeline);
        }

        private static bool IsCombatPrimaryIntent(EIntentAction intent)
        {
            return intent is EIntentAction.Attack
                or EIntentAction.SpecialAttack
                or EIntentAction.Ultimate
                or EIntentAction.Dodge
                or EIntentAction.Chain;
        }

        private static bool TimelineHasHitBox(ActionTimeline timeline)
        {
            var enterStates = new List<ActionNotifyState>();
            var notifyStates = new List<ActionNotifyState>();
            var notifies = new List<ActionNotify>();
            ActionUnpacker.Unpack(timeline, out _, enterStates, notifyStates, notifies);

            return ContainsHitBox(enterStates) || ContainsHitBox(notifyStates);
        }

        private static bool ContainsHitBox(List<ActionNotifyState> states)
        {
            for (var i = 0; i < states.Count; i++)
            {
                if (states[i] is BoxNotifyState box && box.boxType == EBoxType.HitBox)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ApplyActionArgDefaults(TimelineActionSO runtime)
        {
            if (!runtime.enableRotation && !runtime.enableRecenter)
            {
                var name = runtime.actionName;
                if (!string.IsNullOrEmpty(name))
                {
                    if (name.Contains("Walk") || name.Contains("Run"))
                    {
                        runtime.enableRotation = true;
                        runtime.enableRecenter = true;
                    }
                    else if (name.Contains("Idle") || name.Contains("Dodge"))
                    {
                        runtime.enableRotation = true;
                    }
                }
            }

            ApplyLookAtMonsterDefault(runtime);
        }

        private static void ApplyLookAtMonsterDefault(TimelineActionSO runtime)
        {
            if (runtime.enableLookAtMonster)
            {
                return;
            }

            var name = runtime.actionName;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (name.Contains("Attack"))
            {
                runtime.enableLookAtMonster = true;
            }
        }

        private static void BakeSchedule(ActionTimeline timeline, TimelineActionSO runtime)
        {
            ClearBakedSubAssets(runtime);

            var enterStates = new List<ActionNotifyState>();
            var notifyStates = new List<ActionNotifyState>();
            var notifies = new List<ActionNotify>();

            ActionUnpacker.Unpack(timeline, out var clip, enterStates, notifyStates, notifies);

            runtime.clip = clip;
            runtime.notifies.Clear();
            runtime.notifyStates.Clear();

            for (var i = 0; i < notifies.Count; i++)
            {
                notifies[i].scheduleTime = (float)notifies[i].time;
                var baked = BakeNotifySO(notifies[i], runtime);
                if (baked != null)
                {
                    runtime.notifies.Add(baked);
                }
            }

            runtime.notifies.Sort((l, r) => l.scheduleTime.CompareTo(r.scheduleTime));

            for (var i = 0; i < notifyStates.Count; i++)
            {
                var baked = BakeNotifyStateSO(notifyStates[i], runtime);
                if (baked != null)
                {
                    runtime.notifyStates.Add(baked);
                }
            }

            runtime.notifyStates.Sort((l, r) => l.start.CompareTo(r.start));
        }

        private static void ClearBakedSubAssets(TimelineActionSO runtime)
        {
            var path = AssetDatabase.GetAssetPath(runtime);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (var i = subAssets.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(subAssets[i], true);
            }
        }

        private static T CreateSubAsset<T>(TimelineActionSO runtime, string name)
            where T : ScriptableObject
        {
            var entry = ScriptableObject.CreateInstance<T>();
            entry.name = name;
            AssetDatabase.AddObjectToAsset(entry, runtime);
            return entry;
        }

        private static float GetMarkerTime(ActionNotify notify)
        {
            return notify != null ? notify.scheduleTime : 0f;
        }

        /// <summary>
        /// Timeline 资产名（如 Anbi_Idle）→ RuntimeSO.actionName（如 Idle）。
        /// </summary>
        private static string ResolveActionName(string timelineName, TimelineCatalogSO catalog)
        {
            if (string.IsNullOrEmpty(timelineName))
            {
                return timelineName;
            }

            if (catalog != null)
            {
                return catalog.RemoveNamePrefix(timelineName);
            }

            var underscore = timelineName.IndexOf('_');
            if (underscore >= 0 && underscore < timelineName.Length - 1)
            {
                return timelineName[(underscore + 1)..];
            }

            return timelineName;
        }

        private static NotifySO BakeNotifySO(ActionNotify source, TimelineActionSO runtime)
        {
            if (source == null)
            {
                return null;
            }

            NotifySO baked = source switch
            {
                ConsumeStaminaNotify => CreateSubAsset<ConsumeStaminaNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                RestoreSwitchChargeNotify => CreateSubAsset<RestoreSwitchChargeNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                PlayAudioNotify => CreateSubAsset<PlayAudioNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                PlayVoiceNotify => CreateSubAsset<PlayVoiceNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                PlayParticleNotify => CreateSubAsset<PlayParticleNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                ControlAssistCameraNotify => CreateSubAsset<ControlAssistCameraNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                AttackTipNotify => CreateSubAsset<AttackTipNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                CameraShakeNotify => CreateSubAsset<CameraShakeNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                HitstopNotify => CreateSubAsset<HitstopNotifySO>(
                    runtime, $"Notify_{source.GetType().Name}_{source.scheduleTime:F3}"),
                _ => null,
            };

            if (baked == null)
            {
                return null;
            }

            baked.scheduleTime = source.scheduleTime;
            baked.hideFlags = HideFlags.HideInHierarchy;

            switch (baked)
            {
                case ConsumeStaminaNotifySO stamina when source is ConsumeStaminaNotify staminaSource:
                    stamina.amount = staminaSource.amount;
                    break;
                case RestoreSwitchChargeNotifySO restore when source is RestoreSwitchChargeNotify restoreSource:
                    restore.amount = restoreSource.amount;
                    break;
                case PlayAudioNotifySO audio when source is PlayAudioNotify audioSource:
                    audio.paths = ClipsToPaths(audioSource.EffectiveClips);
                    audio.dontPlayProbability = audioSource.dontPlayProbability;
                    break;
                case PlayVoiceNotifySO voice when source is PlayVoiceNotify voiceSource:
                    voice.paths = ClipsToPaths(voiceSource.EffectiveClips);
                    voice.dontPlayProbability = voiceSource.dontPlayProbability;
                    break;
                case PlayParticleNotifySO particle when source is PlayParticleNotify particleSource:
                    particle.prefab = particleSource.prefab;
                    particle.localPosition = particleSource.localPosition;
                    particle.localRotation = particleSource.localRotation;
                    particle.localScale = particleSource.localScale;
                    break;
                case ControlAssistCameraNotifySO assist when source is ControlAssistCameraNotify assistSource:
                    assist.enter = assistSource.enter;
                    break;
                case AttackTipNotifySO tip when source is AttackTipNotify tipSource:
                    tip.canParry = tipSource.canParry;
                    break;
                case CameraShakeNotifySO shake when source is CameraShakeNotify shakeSource:
                    shake.angle = shakeSource.angle;
                    shake.speed = shakeSource.speed;
                    break;
                case HitstopNotifySO hitstop when source is HitstopNotify hitstopSource:
                    hitstop.duration = hitstopSource.duration;
                    hitstop.animationSpeed = hitstopSource.animationSpeed;
                    break;
            }

            return baked;
        }

        private static NotifyStateSO BakeNotifyStateSO(
            ActionNotifyState source,
            TimelineActionSO runtime)
        {
            if (source == null || source is CommandTransitionNotifyState)
            {
                return null;
            }

            NotifyStateSO baked = source switch
            {
                PlayAudioNotifyState => CreateSubAsset<PlayAudioNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                PlayVoiceNotifyState => CreateSubAsset<PlayVoiceNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                PlayParticleNotifyState => CreateSubAsset<PlayParticleNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                BoxNotifyState => CreateSubAsset<BoxNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                AttackingNotifyState => CreateSubAsset<AttackingNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                HoldCameraFollowNotifyState => CreateSubAsset<HoldCameraFollowNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                AssistCameraNotifyState => CreateSubAsset<AssistCameraNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                ControlParryAidNotifyState => CreateSubAsset<ControlParryAidNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                ControlDodgeNotifyState => CreateSubAsset<ControlDodgeNotifyStateSO>(
                    runtime, $"NotifyState_{source.GetType().Name}_{source.start:F3}_{source.length:F3}"),
                _ => null,
            };

            if (baked == null)
            {
                return null;
            }

            baked.start = source.start;
            baked.length = source.length;
            baked.hideFlags = HideFlags.HideInHierarchy;

            switch (baked)
            {
                case PlayAudioNotifyStateSO audio when source is PlayAudioNotifyState audioSource:
                    audio.paths = ClipsToPaths(audioSource.EffectiveClips);
                    audio.dontPlayProbability = audioSource.dontPlayProbability;
                    break;
                case PlayVoiceNotifyStateSO voice when source is PlayVoiceNotifyState voiceSource:
                    voice.paths = ClipsToPaths(voiceSource.EffectiveClips);
                    voice.dontPlayProbability = voiceSource.dontPlayProbability;
                    break;
                case PlayParticleNotifyStateSO particle when source is PlayParticleNotifyState particleSource:
                    particle.prefab = particleSource.prefab;
                    particle.localPosition = particleSource.localPosition;
                    particle.localRotation = particleSource.localRotation;
                    particle.localScale = particleSource.localScale;
                    break;
                case BoxNotifyStateSO box when source is BoxNotifyState boxSource:
                    box.boxType = boxSource.boxType;
                    box.boxShape = boxSource.boxShape;
                    box.center = boxSource.center;
                    box.radius = boxSource.radius;
                    box.size = boxSource.size;
                    box.hitStrength = boxSource.hitStrength;
                    box.hitId = boxSource.hitId;
                    box.particlePrefab = boxSource.particlePrefab;
                    box.setParticleRot = boxSource.setParticleRot;
                    box.rotValue = boxSource.rotValue;
                    box.rotMaxValue = boxSource.rotMaxValue;
                    box.hitGatherDist = boxSource.hitGatherDist;
                    box.hitAudioPath = boxSource.hitAudio != null
                        ? AssetDatabase.GetAssetPath(boxSource.hitAudio)
                        : null;
                    box.hitstopDuration = boxSource.hitstopDuration;
                    box.hitstopSpeed = boxSource.hitstopSpeed;
                    break;
            }

            return baked;
        }

        private static string[] ClipsToPaths(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            var paths = new string[clips.Length];
            for (var i = 0; i < clips.Length; i++)
            {
                paths[i] = clips[i] != null ? AssetDatabase.GetAssetPath(clips[i]) : null;
            }

            return paths;
        }

        private static void ExportServerActionJson(ActionTimeline timeline, TimelineActionSO runtime)
        {
            if (runtime == null || string.IsNullOrEmpty(runtime.actionName))
            {
                return;
            }

            var folder = Path.GetFullPath(Path.Combine(Application.dataPath, ActionBakePaths.ServerActionRelativeFolder));
            Directory.CreateDirectory(folder);

            var duration = runtime.clip != null ? runtime.clip.length : 1f;
            var moveSpeed = ResolveServerMoveSpeed(runtime.actionName);
            var json = BuildServerActionJson(runtime, duration, moveSpeed);
            var path = Path.Combine(folder, $"{runtime.actionName}.json");
            File.WriteAllText(path, json, Encoding.UTF8);
            Debug.Log($"[ActionBakePipeline] ServerAction exported: {path}");
        }

        private static float ResolveServerMoveSpeed(string actionName)
        {
            return actionName == "Run" ? 3.5f : 0f;
        }

        private static string BuildServerActionJson(
            TimelineActionSO runtime,
            float duration,
            float moveSpeed)
        {
            var culture = CultureInfo.InvariantCulture;
            var sb = new StringBuilder(256);
            sb.AppendLine("{");
            sb.AppendLine($"  \"actionName\": \"{runtime.actionName}\",");
            sb.AppendLine($"  \"duration\": {duration.ToString(culture)},");
            sb.AppendLine($"  \"isLoop\": {(runtime.isLoop ? "true" : "false")},");
            sb.AppendLine($"  \"moveSpeed\": {moveSpeed.ToString(culture)},");
            sb.AppendLine("  \"hitBoxes\": [");

            var hitBoxes = CollectHitBoxes(runtime);
            for (var i = 0; i < hitBoxes.Count; i++)
            {
                var box = hitBoxes[i];
                sb.AppendLine("    {");
                sb.AppendLine($"      \"hitId\": {box.hitId},");
                sb.AppendLine($"      \"start\": {box.start.ToString(culture)},");
                sb.AppendLine($"      \"end\": {(box.start + box.length).ToString(culture)},");
                sb.AppendLine("      \"shape\": \"Sphere\",");
                sb.AppendLine($"      \"centerX\": {box.center.x.ToString(culture)},");
                sb.AppendLine($"      \"centerY\": {box.center.y.ToString(culture)},");
                sb.AppendLine($"      \"centerZ\": {box.center.z.ToString(culture)},");
                sb.Append("      \"radius\": ");
                sb.Append(box.radius.ToString(culture));
                sb.AppendLine();
                sb.Append("    }");
                sb.AppendLine(i < hitBoxes.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine("  ]");
            sb.Append('}');
            return sb.ToString();
        }

        private static List<BoxNotifyStateSO> CollectHitBoxes(TimelineActionSO runtime)
        {
            var result = new List<BoxNotifyStateSO>(4);
            if (runtime.notifyStates == null)
            {
                return result;
            }

            for (var i = 0; i < runtime.notifyStates.Count; i++)
            {
                if (runtime.notifyStates[i] is BoxNotifyStateSO box
                    && box.boxType == EBoxType.HitBox)
                {
                    result.Add(box);
                }
            }

            return result;
        }
    }
}

#endif
