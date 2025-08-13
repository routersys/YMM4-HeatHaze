using System;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace YMM4HeatShimmer.Effect.Video
{
    internal class HeatShimmerEffectProcessor : VideoEffectProcessorBase
    {
        private readonly HeatShimmerEffect _item;
        private HeatShimmerCustomEffect? _effect;

        public HeatShimmerEffectProcessor(IGraphicsDevicesAndContext devices, HeatShimmerEffect item) : base(devices)
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

            float finalStrength, finalScale, finalFlowSpeed, finalBoilSpeed;

            if (_item.ControlMode == ControlMode.Automatic)
            {
                var temperature = (float)_item.Temperature.GetValue(frame, length, fps);
                var humidity = (float)_item.Humidity.GetValue(frame, length, fps) / 100f;

                float tempFactor = Math.Clamp((temperature - 15f) / 35f, 0f, 1.5f);
                float humidityFactor = 1f + humidity * 0.5f;

                finalStrength = tempFactor * humidityFactor * 0.5f;
                finalScale = (1f + tempFactor) * 1.5f;
                finalFlowSpeed = tempFactor * 0.2f;
                finalBoilSpeed = tempFactor * 0.3f;
            }
            else
            {
                finalStrength = (float)_item.Strength.GetValue(frame, length, fps) / 100f;
                finalScale = (float)_item.Scale.GetValue(frame, length, fps) / 100f;
                finalFlowSpeed = (float)_item.FlowSpeed.GetValue(frame, length, fps) / 100f;
                finalBoilSpeed = (float)_item.BoilSpeed.GetValue(frame, length, fps) / 100f;
            }

            _effect.Strength = finalStrength;
            _effect.Scale = finalScale;
            _effect.FlowSpeed = finalFlowSpeed;
            _effect.BoilSpeed = finalBoilSpeed;

            _effect.Angle = (float)(_item.Angle.GetValue(frame, length, fps) * Math.PI / 180.0);
            _effect.ChromaticAberration = (float)(_item.ChromaticAberration.GetValue(frame, length, fps) / 1000.0);
            _effect.EnableBlur = _item.EnableBlur ? 1 : 0;
            _effect.BlurStrength = (float)(_item.BlurStrength.GetValue(frame, length, fps) / 100.0);
            _effect.PreventEdgeStretch = _item.PreventEdgeStretch ? 1 : 0;
            _effect.DebugMode = (int)_item.DebugMode;
            _effect.Time = (float)totalSeconds;

            return effectDescription.DrawDescription;
        }

        protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
        {
            _effect = new HeatShimmerCustomEffect(devices);
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
