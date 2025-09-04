using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace YMM4HeatShimmer.Effect.Video
{
    internal class HeatShimmerCustomEffect : D2D1CustomShaderEffectBase
    {
        public float Strength { set => SetValue((int)EffectImpl.Properties.Strength, value); }
        public float Scale { set => SetValue((int)EffectImpl.Properties.Scale, value); }
        public float Angle { set => SetValue((int)EffectImpl.Properties.Angle, value); }
        public float FlowSpeed { set => SetValue((int)EffectImpl.Properties.FlowSpeed, value); }
        public float BoilSpeed { set => SetValue((int)EffectImpl.Properties.BoilSpeed, value); }
        public float ChromaticAberration { set => SetValue((int)EffectImpl.Properties.ChromaticAberration, value); }
        public int EnableBlur { set => SetValue((int)EffectImpl.Properties.EnableBlur, value); }
        public float BlurStrength { set => SetValue((int)EffectImpl.Properties.BlurStrength, value); }
        public int PreventEdgeStretch { set => SetValue((int)EffectImpl.Properties.PreventEdgeStretch, value); }
        public int DebugMode { set => SetValue((int)EffectImpl.Properties.DebugMode, value); }
        public float Time { set => SetValue((int)EffectImpl.Properties.Time, value); }

        public HeatShimmerCustomEffect(IGraphicsDevicesAndContext devices)
            : base(Create<EffectImpl>(devices))
        {
        }

        [CustomEffect(1)]
        private class EffectImpl() : D2D1CustomShaderEffectImplBase<EffectImpl>(LoadShaderFromEmbeddedResource("HeatShimmerShader.cso"))
        {
            private ConstantBuffer _constants;

            private static byte[] LoadShaderFromEmbeddedResource(string resourceName)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fullResourceName = $"YMM4HeatShimmer.Shaders.{resourceName}";
                using var stream = assembly.GetManifestResourceStream(fullResourceName) ?? throw new FileNotFoundException($"Shader resource '{fullResourceName}' not found.");
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }

            protected override void UpdateConstants() => drawInformation?.SetPixelShaderConstantBuffer(_constants);

            public override void MapInputRectsToOutputRect(RawRect[] inputRects, RawRect[] inputOpaqueSubRects, out RawRect outputRect, out RawRect outputOpaqueSubRect)
            {
                if (inputRects.Length != 1)
                    throw new System.ArgumentException("InputRects must be length of 1", nameof(inputRects));

                var input = inputRects[0];
                int expansion = 2000;

                outputRect = new RawRect(
                    input.Left - expansion,
                    input.Top - expansion,
                    input.Right + expansion,
                    input.Bottom + expansion);

                outputOpaqueSubRect = default;
            }

            public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
            {
                if (inputRects.Length != 1)
                    throw new System.ArgumentException("InputRects must be length of 1", nameof(inputRects));

                int expansion = 2000;

                inputRects[0] = new RawRect(
                    outputRect.Left - expansion,
                    outputRect.Top - expansion,
                    outputRect.Right + expansion,
                    outputRect.Bottom + expansion);
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Strength;
                public float Scale;
                public float Angle;
                public float FlowSpeed;
                public float BoilSpeed;
                public float ChromaticAberration;
                public int EnableBlur;
                public float BlurStrength;
                public int PreventEdgeStretch;
                public int DebugMode;
                public float Time;
                private float _padding;
            }

            public enum Properties
            {
                Strength,
                Scale,
                Angle,
                FlowSpeed,
                BoilSpeed,
                ChromaticAberration,
                EnableBlur,
                BlurStrength,
                PreventEdgeStretch,
                DebugMode,
                Time
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Strength)]
            public float Strength { get => _constants.Strength; set { _constants.Strength = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Scale)]
            public float Scale { get => _constants.Scale; set { _constants.Scale = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Angle)]
            public float Angle { get => _constants.Angle; set { _constants.Angle = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.FlowSpeed)]
            public float FlowSpeed { get => _constants.FlowSpeed; set { _constants.FlowSpeed = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.BoilSpeed)]
            public float BoilSpeed { get => _constants.BoilSpeed; set { _constants.BoilSpeed = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.ChromaticAberration)]
            public float ChromaticAberration { get => _constants.ChromaticAberration; set { _constants.ChromaticAberration = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Int32, (int)Properties.EnableBlur)]
            public int EnableBlur { get => _constants.EnableBlur; set { _constants.EnableBlur = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.BlurStrength)]
            public float BlurStrength { get => _constants.BlurStrength; set { _constants.BlurStrength = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Int32, (int)Properties.PreventEdgeStretch)]
            public int PreventEdgeStretch { get => _constants.PreventEdgeStretch; set { _constants.PreventEdgeStretch = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Int32, (int)Properties.DebugMode)]
            public int DebugMode { get => _constants.DebugMode; set { _constants.DebugMode = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Time)]
            public float Time { get => _constants.Time; set { _constants.Time = value; UpdateConstants(); } }
        }
    }
}