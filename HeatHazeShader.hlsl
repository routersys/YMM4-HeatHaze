cbuffer Constants : register(b0)
{
    float strength;
    float scale;
    float angle;
    float flowSpeed;
    float boilSpeed;
    float chromaticAberration;
    int enableBlur;
    float blurStrength;
    int preventEdgeStretch;
    int debugMode;
    float time;
    float _padding;
};

Texture2D<float4> InputTexture : register(t0);
SamplerState InputSampler : register(s0);

float3 hash(float3 p)
{
    p = float3(dot(p, float3(127.1, 311.7, 271.9)),
               dot(p, float3(269.5, 183.3, 323.1)),
               dot(p, float3(113.5, 271.9, 124.6)));
    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

float noise(float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
    float3 u = f * f * (3.0 - 2.0 * f);

    return lerp(lerp(lerp(dot(hash(i + float3(0, 0, 0)), f - float3(0, 0, 0)),
                          dot(hash(i + float3(1, 0, 0)), f - float3(1, 0, 0)), u.x),
                     lerp(dot(hash(i + float3(0, 1, 0)), f - float3(0, 1, 0)),
                          dot(hash(i + float3(1, 1, 0)), f - float3(1, 1, 0)), u.x), u.y),
                lerp(lerp(dot(hash(i + float3(0, 0, 1)), f - float3(0, 0, 1)),
                          dot(hash(i + float3(1, 0, 1)), f - float3(1, 0, 1)), u.x),
                     lerp(dot(hash(i + float3(0, 1, 1)), f - float3(0, 1, 1)),
                          dot(hash(i + float3(1, 1, 1)), f - float3(1, 1, 1)), u.x), u.y), u.z);
}

float fbm(float3 p)
{
    float value = 0.0;
    float amplitude = 0.5;
    
    [unroll]
    for (int i = 0; i < 6; i++)
    {
        value += amplitude * noise(p);
        p *= 2.0;
        amplitude *= 0.5;
    }
    return value;
}

float4 main(float4 pos : SV_POSITION, float4 posScene : SCENE_POSITION, float4 uv0 : TEXCOORD0) : SV_TARGET
{
    float4 originalColor = InputTexture.Sample(InputSampler, uv0.xy);
    if (originalColor.a < 0.01f)
    {
        return float4(0, 0, 0, 0);
    }
    
    float2 dir = float2(cos(angle), sin(angle));
    float2 flow = dir * time * flowSpeed;
    float3 noiseCoord = float3((uv0.xy + flow) * scale, time * boilSpeed);
    float noiseValue = fbm(noiseCoord);

    if (debugMode == 1)
    {
        return float4(noiseValue * 0.5f + 0.5f, noiseValue * 0.5f + 0.5f, noiseValue * 0.5f + 0.5f, 1.0f);
    }
    
    float2 uv = uv0.xy;
    if (preventEdgeStretch == 1)
    {
        float max_offset = strength * 0.1f;
        uv = uv * (1.0f - max_offset) + (max_offset / 2.0f);
    }

    float2 offset = dir * noiseValue * strength * 0.1f;
    float2 distortedUV = uv + offset;
    
    float3 finalRgb;
    float4 distortedColor = InputTexture.Sample(InputSampler, distortedUV);

    if (enableBlur == 1 && blurStrength > 0.0f)
    {
        float blurSize = length(offset) * blurStrength * 0.2f;
        float4 blurredColor = 0;
        int sampleCount = 0;
        [unroll]
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                blurredColor += InputTexture.Sample(InputSampler, distortedUV + float2(i, j) * blurSize);
                sampleCount++;
            }
        }
        finalRgb = blurredColor.rgb / sampleCount;
    }
    else
    {
        finalRgb = distortedColor.rgb;
    }

    if (chromaticAberration > 0.0f)
    {
        float r = InputTexture.Sample(InputSampler, distortedUV - chromaticAberration * dir).r;
        float b = InputTexture.Sample(InputSampler, distortedUV + chromaticAberration * dir).b;
        finalRgb.r = r;
        finalRgb.b = b;
    }
    
    float3 blendedRgb = lerp(originalColor.rgb, finalRgb, distortedColor.a);

    return float4(blendedRgb, originalColor.a);
}
