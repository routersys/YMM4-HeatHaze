using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace YMM4HeatHaze.Effect.Video
{
    internal class HeatHazeCustomEffect : D2D1CustomShaderEffectBase
    {
        public float Strength { set => SetValue((int)EffectImpl.Properties.Strength, value); }
        public float Speed { set => SetValue((int)EffectImpl.Properties.Speed, value); }
        public float Scale { set => SetValue((int)EffectImpl.Properties.Scale, value); }
        public float Time { set => SetValue((int)EffectImpl.Properties.Time, value); }
        public float Angle { set => SetValue((int)EffectImpl.Properties.Angle, value); }
        public float ChromaticAberration { set => SetValue((int)EffectImpl.Properties.ChromaticAberration, value); }
        public int PreventEdgeStretch { set => SetValue((int)EffectImpl.Properties.PreventEdgeStretch, value); }
        public int DebugMode { set => SetValue((int)EffectImpl.Properties.DebugMode, value); }

        public HeatHazeCustomEffect(IGraphicsDevicesAndContext devices)
            : base(Create<EffectImpl>(devices))
        {
        }

        [CustomEffect(1)]
        private class EffectImpl() : D2D1CustomShaderEffectImplBase<EffectImpl>(LoadShaderFromEmbeddedResource("HeatHazeShader.cso"))
        {
            private ConstantBuffer _constants;

            private static byte[] LoadShaderFromEmbeddedResource(string resourceName)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fullResourceName = $"YMM4HeatHaze.Shaders.{resourceName}";
                using var stream = assembly.GetManifestResourceStream(fullResourceName) ?? throw new FileNotFoundException($"Shader resource '{fullResourceName}' not found.");
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }

            protected override void UpdateConstants() => drawInformation?.SetPixelShaderConstantBuffer(_constants);

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Strength;
                public float Speed;
                public float Scale;
                public float Time;
                public float Angle;
                public float ChromaticAberration;
                public int PreventEdgeStretch;
                public int DebugMode;
            }

            public enum Properties
            {
                Strength,
                Speed,
                Scale,
                Time,
                Angle,
                ChromaticAberration,
                PreventEdgeStretch,
                DebugMode
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Strength)]
            public float Strength { get => _constants.Strength; set { _constants.Strength = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Speed)]
            public float Speed { get => _constants.Speed; set { _constants.Speed = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Scale)]
            public float Scale { get => _constants.Scale; set { _constants.Scale = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Time)]
            public float Time { get => _constants.Time; set { _constants.Time = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.Angle)]
            public float Angle { get => _constants.Angle; set { _constants.Angle = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Float, (int)Properties.ChromaticAberration)]
            public float ChromaticAberration { get => _constants.ChromaticAberration; set { _constants.ChromaticAberration = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Int32, (int)Properties.PreventEdgeStretch)]
            public int PreventEdgeStretch { get => _constants.PreventEdgeStretch; set { _constants.PreventEdgeStretch = value; UpdateConstants(); } }
            [CustomEffectProperty(PropertyType.Int32, (int)Properties.DebugMode)]
            public int DebugMode { get => _constants.DebugMode; set { _constants.DebugMode = value; UpdateConstants(); } }
        }
    }
}