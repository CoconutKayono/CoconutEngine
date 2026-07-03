using Launcher;
using TEngine;
using YooAsset;
using ProcedureOwner = TEngine.IFsm<TEngine.IProcedureModule>;

namespace Procedure
{
    /// <summary>
    /// 流程 => 启动器。
    /// <para>【流程位置】入口流程（ProcedureSetting.entranceProcedureTypeName）。</para>
    /// <para>【职责】初始化 Launcher UI 框架；读取 PlayerPrefs 恢复语言与音效配置。</para>
    /// <para>【下一流程】运行一帧后 → ProcedureSplash。</para>
    /// <para>【术语】LauncherMgr：启动阶段 UI 管理器；Localization：多语言模块。</para>
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        public override bool UseNativeDialog => true;
        
        private IAudioModule _audioModule;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            _audioModule = ModuleSystem.GetModule<IAudioModule>();
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            //热更新UI初始化
            LauncherMgr.Initialize();

            // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言
            InitLanguageSettings();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // 运行一帧即切换到 Splash 展示流程
            ChangeState<ProcedureSplash>(procedureOwner);
        }

        private void InitLanguageSettings()
        {
            if (_resourceModule.PlayMode == EPlayMode.EditorSimulateMode && RootModule.Instance.EditorLanguage == Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }
            
            ILocalizationModule localizationModule = ModuleSystem.GetModule<ILocalizationModule>();
            Language language = localizationModule.Language;
            if (Utility.PlayerPrefs.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    string languageString = Utility.PlayerPrefs.GetString(Constant.Setting.Language);
                    if (!string.IsNullOrEmpty(languageString)
                        && System.Enum.TryParse(typeof(Language), languageString, out var parsed)
                        && parsed is Language parsedLanguage
                        && parsedLanguage != Language.Unspecified)
                    {
                        language = parsedLanguage;
                    }
                    else
                    {
                        var index = Utility.PlayerPrefs.GetInt(Constant.Setting.Language, 0);
                        language = ResolveLanguageFromIndex(index);
                    }
                }
                catch(System.Exception exception)
                {
                    Log.Error("Init language error, reason {0}",exception.ToString());
                }
            }
            
            if (language != Language.English
                && language != Language.ChineseSimplified
                && language != Language.ChineseTraditional)
            {
                language = Language.English;
                Utility.PlayerPrefs.SetInt(Constant.Setting.Language, 1);
                Utility.PlayerPrefs.Save();
            }
            
            localizationModule.Language = language;
            Log.Info("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        /// <summary>
        /// 与 TbSystemConfig 语言下拉顺序一致：0=简体中文，1=英文。
        /// </summary>
        private static Language ResolveLanguageFromIndex(int index)
        {
            return index switch
            {
                0 => Language.ChineseSimplified,
                1 => Language.English,
                _ => Language.English
            };
        }

        private void InitSoundSettings()
        {
            const int volumeStepDefault = 10;

            _audioModule.MusicEnable = !Utility.PlayerPrefs.GetBool(Constant.Setting.MusicMuted, false);
            _audioModule.MusicVolume = LoadVolumeFromPlayerPrefs(Constant.Setting.MusicVolume, volumeStepDefault);
            _audioModule.SoundEnable = !Utility.PlayerPrefs.GetBool(Constant.Setting.SoundMuted, false);
            _audioModule.SoundVolume = LoadVolumeFromPlayerPrefs(Constant.Setting.SoundVolume, volumeStepDefault);
            _audioModule.UISoundEnable = !Utility.PlayerPrefs.GetBool(Constant.Setting.UISoundMuted, false);
            _audioModule.UISoundVolume = LoadVolumeFromPlayerPrefs(Constant.Setting.UISoundVolume, volumeStepDefault);
            _audioModule.VoiceEnable = !Utility.PlayerPrefs.GetBool(Constant.Setting.VoiceMuted, false);
            _audioModule.VoiceVolume = LoadVolumeFromPlayerPrefs(Constant.Setting.VoiceVolume, volumeStepDefault);
            Log.Info("Init sound settings complete.");
        }

        private static float LoadVolumeFromPlayerPrefs(string key, int defaultStep)
        {
            var step = Utility.PlayerPrefs.GetInt(key, defaultStep);
            step = UnityEngine.Mathf.Clamp(step, 0, 10);
            return step / 10f;
        }
    }
}
