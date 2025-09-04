using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace YMM4HeatShimmer.Effect.Video
{
    public enum ControlMode
    {
        [Display(Name = nameof(Translate.ControlMode_Automatic), ResourceType = typeof(Translate))]
        Automatic,
        [Display(Name = nameof(Translate.ControlMode_Manual), ResourceType = typeof(Translate))]
        Manual
    }

    public enum DebugViewMode
    {
        [Display(Name = nameof(Translate.DebugViewMode_Result), ResourceType = typeof(Translate))]
        Result,
        [Display(Name = nameof(Translate.DebugViewMode_NoiseView), ResourceType = typeof(Translate))]
        NoiseView
    }

    [VideoEffect(nameof(Translate.EffectName), ["歪み"], ["heat shimmer", "distortion", "熱揺らぎ", "陽炎"], IsAviUtlSupported = false, ResourceType = typeof(Translate))]
    public class HeatShimmerEffect : VideoEffectBase
    {
        public override string Label => Translate.EffectName;

        [Display(GroupName = nameof(Translate.GroupName_ControlMode), Name = nameof(Translate.Name_ControlMode), Description = nameof(Translate.Description_ControlMode), Order = 0, ResourceType = typeof(Translate))]
        [EnumComboBox]
        public ControlMode ControlMode { get => controlMode; set => Set(ref controlMode, value); }
        private ControlMode controlMode = ControlMode.Automatic;

        [Display(GroupName = nameof(Translate.GroupName_AutoSettings), Name = nameof(Translate.Name_Temperature), Description = nameof(Translate.Description_Temperature), Order = 1, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "℃", -10, 50)]
        public Animation Temperature { get; } = new(35, -10, 50);

        [Display(GroupName = nameof(Translate.GroupName_AutoSettings), Name = nameof(Translate.Name_Humidity), Description = nameof(Translate.Description_Humidity), Order = 2, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Humidity { get; } = new(60, 0, 100);

        [Display(GroupName = nameof(Translate.GroupName_ManualSettings), Name = nameof(Translate.Name_Strength), Description = nameof(Translate.Description_Strength), Order = 3, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Strength { get; } = new(50, 0, 100);

        [Display(GroupName = nameof(Translate.GroupName_ManualSettings), Name = nameof(Translate.Name_Scale), Description = nameof(Translate.Description_Scale), Order = 4, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "%", 1, 500)]
        public Animation Scale { get; } = new(100, 1, 500);

        [Display(GroupName = nameof(Translate.GroupName_ManualSettings), Name = nameof(Translate.Name_FlowSpeed), Description = nameof(Translate.Description_FlowSpeed), Order = 5, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "", -100, 100)]
        public Animation FlowSpeed { get; } = new(10, -100, 100);

        [Display(GroupName = nameof(Translate.GroupName_ManualSettings), Name = nameof(Translate.Name_BoilSpeed), Description = nameof(Translate.Description_BoilSpeed), Order = 6, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "", 0, 100)]
        public Animation BoilSpeed { get; } = new(10, 0, 100);

        [Display(GroupName = nameof(Translate.GroupName_CommonSettings), Name = nameof(Translate.Name_Angle), Description = nameof(Translate.Description_Angle), Order = 7, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "°", 0, 360)]
        public Animation Angle { get; } = new(90, 0, 360);

        [Display(GroupName = nameof(Translate.GroupName_CommonSettings), Name = nameof(Translate.Name_ChromaticAberration), Description = nameof(Translate.Description_ChromaticAberration), Order = 8, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "px", 0, 10)]
        public Animation ChromaticAberration { get; } = new(0, 0, 10);

        [Display(GroupName = nameof(Translate.GroupName_CommonSettings), Name = nameof(Translate.Name_EnableBlur), Description = nameof(Translate.Description_EnableBlur), Order = 9, ResourceType = typeof(Translate))]
        [ToggleSlider]
        public bool EnableBlur { get => enableBlur; set => Set(ref enableBlur, value); }
        private bool enableBlur = false;

        [Display(GroupName = nameof(Translate.GroupName_CommonSettings), Name = nameof(Translate.Name_BlurStrength), Description = nameof(Translate.Description_BlurStrength), Order = 10, ResourceType = typeof(Translate))]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation BlurStrength { get; } = new(50, 0, 100);

        [Display(GroupName = nameof(Translate.GroupName_CommonSettings), Name = nameof(Translate.Name_PreventEdgeStretch), Description = nameof(Translate.Description_PreventEdgeStretch), Order = 11, ResourceType = typeof(Translate))]
        [ToggleSlider]
        public bool PreventEdgeStretch { get => preventEdgeStretch; set => Set(ref preventEdgeStretch, value); }
        private bool preventEdgeStretch = true;

        [Display(GroupName = nameof(Translate.GroupName_Debug), Name = nameof(Translate.Name_DebugMode), Description = nameof(Translate.Description_DebugMode), Order = 12, ResourceType = typeof(Translate))]
        [EnumComboBox]
        public DebugViewMode DebugMode { get => debugMode; set => Set(ref debugMode, value); }
        private DebugViewMode debugMode = DebugViewMode.Result;

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new HeatShimmerEffectProcessor(devices, this);
        }

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Temperature, Humidity, Strength, Scale, Angle, FlowSpeed, BoilSpeed, ChromaticAberration, BlurStrength];
    }
}