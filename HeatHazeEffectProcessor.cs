using System;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace YMM4HeatHaze.Effect.Video
{
    internal class HeatHazeEffectProcessor : VideoEffectProcessorBase
    {
        private readonly HeatHazeEffect _item;
        private HeatHazeCustomEffect? _effect;

        public HeatHazeEffectProcessor(IGraphicsDevicesAndContext devices, HeatHazeEffect item) : base(devices)
        {
            _item = item;
        }

        public override DrawDescription Update(EffectDescription effectDescription)
        {
            if (IsPassThroughEffect || _effect is null)
                return effectDescription.DrawDescription;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var totalSeconds = (double)frame / fps;

            _effect.Strength = (float)(_item.Strength.GetValue(frame, length, fps) / 1000.0);
            _effect.Speed = (float)(_item.Speed.GetValue(frame, length, fps) / 100.0);
            _effect.Scale = (float)(_item.Scale.GetValue(frame, length, fps) / 100.0);
            _effect.Time = (float)totalSeconds;
            _effect.Angle = (float)(_item.Angle.GetValue(frame, length, fps) * Math.PI / 180.0);
            _effect.ChromaticAberration = (float)(_item.ChromaticAberration.GetValue(frame, length, fps) / 5000.0);
            _effect.PreventEdgeStretch = _item.PreventEdgeStretch ? 1 : 0;
            _effect.DebugMode = (int)_item.DebugMode;

            return effectDescription.DrawDescription;
        }

        protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
        {
            _effect = new HeatHazeCustomEffect(devices);
            if (!_effect.IsEnabled)
            {
                _effect.Dispose();
                _effect = null;
                return null;
            }
            disposer.Collect(_effect);

            var output = _effect.Output;
            disposer.Collect(output);
            return output;
        }

        protected override void setInput(ID2D1Image? input)
        {
            _effect?.SetInput(0, input, true);
        }

        protected override void ClearEffectChain()
        {
            _effect?.SetInput(0, null, true);
        }
    }
}