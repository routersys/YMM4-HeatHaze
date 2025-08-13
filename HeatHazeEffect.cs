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
        [Display(Name = "自動")]
        Automatic,
        [Display(Name = "手動")]
        Manual
    }

    public enum DebugViewMode
    {
        [Display(Name = "最終結果")]
        Result,
        [Display(Name = "ノイズ表示")]
        NoiseView
    }

    [VideoEffect("熱揺らぎ", ["歪み"], ["heat shimmer", "distortion", "熱揺らぎ", "陽炎"], IsAviUtlSupported = false)]
    public class HeatShimmerEffect : VideoEffectBase
    {
        public override string Label => "熱揺らぎ";

        [Display(GroupName = "操作モード", Name = "モード切替", Description = "操作モードを、直感的な「自動」と、詳細な「手動」で切り替えます。", Order = 0)]
        [EnumComboBox]
        public ControlMode ControlMode { get => controlMode; set => Set(ref controlMode, value); }
        private ControlMode controlMode = ControlMode.Automatic;

        [Display(GroupName = "自動設定", Name = "温度", Description = "現実の温度を模倣して、揺らぎの強さや速さを自動調整します。", Order = 1)]
        [AnimationSlider("F1", "℃", -10, 50)]
        public Animation Temperature { get; } = new(35, -10, 50);

        [Display(GroupName = "自動設定", Name = "湿度", Description = "現実の湿度を模倣して、揺らぎの質感を調整します。", Order = 2)]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Humidity { get; } = new(60, 0, 100);

        [Display(GroupName = "手動設定", Name = "揺らぎの強さ", Description = "映像の歪みの大きさを調整します。", Order = 3)]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Strength { get; } = new(50, 0, 100);

        [Display(GroupName = "手動設定", Name = "揺らぎの細かさ", Description = "歪みのパターンの大きさを調整します。", Order = 4)]
        [AnimationSlider("F1", "%", 1, 500)]
        public Animation Scale { get; } = new(100, 1, 500);

        [Display(GroupName = "手動設定", Name = "流れの速さ", Description = "歪みパターン全体が移動する速度を制御します。", Order = 5)]
        [AnimationSlider("F1", "", -100, 100)]
        public Animation FlowSpeed { get; } = new(10, -100, 100);

        [Display(GroupName = "手動設定", Name = "うねりの速さ", Description = "歪みパターンがその場で変化する速度を制御します。", Order = 6)]
        [AnimationSlider("F1", "", 0, 100)]
        public Animation BoilSpeed { get; } = new(10, 0, 100);

        [Display(GroupName = "共通設定", Name = "揺らぎの角度", Description = "歪みが発生する方向を調整します。", Order = 7)]
        [AnimationSlider("F1", "°", 0, 360)]
        public Animation Angle { get; } = new(90, 0, 360);

        [Display(GroupName = "共通設定", Name = "色収差の強さ", Description = "歪みに伴うRGBの色ズレの強さを調整します。", Order = 8)]
        [AnimationSlider("F1", "px", 0, 10)]
        public Animation ChromaticAberration { get; } = new(0, 0, 10);

        [Display(GroupName = "共通設定", Name = "連動ブラー", Description = "オンにすると、歪みの強さに応じて映像が僅かにぼやけます。", Order = 9)]
        [ToggleSlider]
        public bool EnableBlur { get => enableBlur; set => Set(ref enableBlur, value); }
        private bool enableBlur = false;

        [Display(GroupName = "共通設定", Name = "ブラーの強さ", Description = "歪みに連動するぼかしの強さを調整します。", Order = 10)]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation BlurStrength { get; } = new(50, 0, 100);

        [Display(GroupName = "共通設定", Name = "端の伸長を防止", Description = "オンにすると、歪みによる映像の端の引き伸ばしを防ぎます。", Order = 11)]
        [ToggleSlider]
        public bool PreventEdgeStretch { get => preventEdgeStretch; set => Set(ref preventEdgeStretch, value); }
        private bool preventEdgeStretch = true;

        [Display(GroupName = "デバッグ", Name = "表示モード", Description = "デバッグ用に、内部で生成されているノイズパターンを表示します。", Order = 12)]
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
