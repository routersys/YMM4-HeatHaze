cbuffer Constants : register(b0)
{
    float strength;
    float speed;
    float scale;
    float time;
    float angle;
    float chromaticAberration;
    int preventEdgeStretch;
    int debugMode;
};

Texture2D<float4> InputTexture : register(t0);
SamplerState InputSampler : register(s0);

float2 perp(float2 v)
{
    return float2(-v.y, v.x);
}

float4 main(float4 pos : SV_POSITION, float4 posScene : SCENE_POSITION, float4 uv0 : TEXCOORD0) : SV_TARGET
{
    if (debugMode == 1)
    {
        return float4(0.5f, 0.5f, 0.5f, 1.0f);
    }
    
    float2 uv = uv0.xy;

    if (preventEdgeStretch == 1)
    {
        float max_offset = strength * 1.2f;
        uv = uv * (1.0f - max_offset) + (max_offset / 2.0f);
    }

    float2 mainDir = float2(cos(angle), sin(angle));

    float wave1 = sin(uv.y * scale * 2.0f + time * speed * 1.0f);
    float wave2 = sin(uv.x * scale * 3.0f + time * speed * 0.7f);
    float wave3 = sin((uv.x * 0.6f + uv.y * 1.4f) * scale + time * speed * 2.5f);

    float totalWave = (wave1 + wave2 + wave3) * 0.33f;
    float2 distortion = perp(mainDir) * totalWave * strength;

    float2 distortedUV = uv + distortion;

    float4 sourceColor = InputTexture.Sample(InputSampler, distortedUV);
    
    float2 chromaOffset = mainDir * chromaticAberration;
    float r = InputTexture.Sample(InputSampler, distortedUV - chromaOffset).r;
    float g = sourceColor.g;
    float b = InputTexture.Sample(InputSampler, distortedUV + chromaOffset).b;

    return float4(r, g, b, sourceColor.a);
}