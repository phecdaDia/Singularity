#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VSInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
};

struct VSOutput
{
    float4 position : SV_POSITION;
    float4 position2D : TEXCOORD0;
    float4 Normal : COLOR0;
};

VSOutput VSShadowMap(in VSInput input)
{
    VSOutput output;
	
    output.position = mul(input.Position, WorldViewProjection);
    output.position2D = output.position;
    output.Normal = input.Normal;
    return output;
}

float4 PSShadowMap(VSOutput input) : COLOR
{
    return abs(input.Normal);

}

technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL VSShadowMap();
        PixelShader = compile PS_SHADERMODEL PSShadowMap();
    }
}