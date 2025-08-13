using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace YMM4HeatHaze.Effect.Video
{
    public enum DebugViewMode
    {
        [Display(Name = "最終結果")]
        Result,
        [Display(Name = "ノイズ表示")]
        NoiseView
    }

    [VideoEffect("陽炎", ["加工"], ["heat haze", "distortion", "陽炎", "かげろう"], IsAviUtlSupported = false)]
    public class HeatHazeEffect : VideoEffectBase
    {
        public override string Label => "陽炎";

        [Display(GroupName = "設定", Name = "ゆらぎの強さ", Description = "映像の歪みの大きさを調整します。", Order = 1)]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Strength { get; } = new(5, 0, 100);

        [Display(GroupName = "設定", Name = "ゆらぎの速さ", Description = "陽炎が動くスピードを調整します。", Order = 2)]
        [AnimationSlider("F1", "", 0, 100)]
        public Animation Speed { get; } = new(10, 0, 100);

        [Display(GroupName = "設定", Name = "ゆらぎの細かさ", Description = "歪みのパターンの大きさを調整します。", Order = 3)]
        [AnimationSlider("F1", "%", 1, 500)]
        public Animation Scale { get; } = new(100, 1, 500);

        [Display(GroupName = "設定", Name = "ゆらぎの角度", Description = "歪みが発生する方向を調整します。", Order = 4)]
        [AnimationSlider("F1", "°", 0, 360)]
        public Animation Angle { get; } = new(90, 0, 360);

        [Display(GroupName = "設定", Name = "色収差の強さ", Description = "歪みに伴うRGBの色ズレの強さを調整します。", Order = 5)]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation ChromaticAberration { get; } = new(0, 0, 100);

        [Display(GroupName = "設定", Name = "端の伸長を防止", Description = "オンにすると、歪みによる映像の端の引き伸ばしを防ぎます。代わりに映像が僅かに拡大されます。", Order = 6)]
        [ToggleSlider]
        public bool PreventEdgeStretch { get => preventEdgeStretch; set => Set(ref preventEdgeStretch, value); }
        private bool preventEdgeStretch = true;

        [Display(GroupName = "デバッグ", Name = "表示モード", Description = "デバッグ用に、内部で生成されているノイズパターンを表示します。", Order = 7)]
        [EnumComboBox]
        public DebugViewMode DebugMode { get => debugMode; set => Set(ref debugMode, value); }
        private DebugViewMode debugMode = DebugViewMode.Result;


        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new HeatHazeEffectProcessor(devices, this);
        }

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Strength, Speed, Scale, Angle, ChromaticAberration];
    }
}