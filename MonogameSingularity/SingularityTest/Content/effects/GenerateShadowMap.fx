
// DEFINES IN CASE WE USE GL OR DX
#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


// DEFINE GLOBAL VARIABLES BELOW
matrix World;
matrix View;
matrix Projection;

matrix LightView;
matrix LightProjection;

float1 MaxClippingDistance;

Texture2D ShadowMap;

sampler2D ShadowMapSampler = sampler_state
{
    texture = <ShadowMap>;
    //magfilter = LINEAR;
    //minfilter = LINEAR;
    //mipfilter = LINEAR;
    //AddressU = clamp;
    //AddressV = clamp;
};


///
/// SHADOW MAP METHODS
///

// VECTOR SHADER INPUT
struct VS_Map_Input
{
    float4 Position : SV_POSITION;
};

// VECTOR SHADER OUTPUT / PIXEL SHADER INPUT
struct VS_Map_Output
{
    float4 position : SV_POSITION;
    float4 position2D : TEXCOORD0;
};

// VECTOR SHADER METHOD
VS_Map_Output VSShadowMap(in VS_Map_Input input)
{
    VS_Map_Output output;

    output.position = mul(input.Position, World);
    output.position = mul(output.position, LightView);
    output.position = mul(output.position, LightProjection);
    output.position2D = output.position;

    return output;
}

// PIXEL SHADER METHOD
float4 PSShadowMap(VS_Map_Output input) : COLOR
{
    float4 temp = saturate(input.position2D.z / (input.position2D.w));
    temp.w = 1.0f;	// set visibility to 100%
    return temp;
}

///
/// SHADOW SCENE METHODS
///

struct VS_Scene_Input
{
    float4 Position : SV_POSITION;
    float4 Normal : NORMAL;
};

struct VS_Scene_Output
{
    float4 position : SV_POSITION;
    float4 lightPosition : TEXCOORD1;
    float4 position2D : TEXCOORD0;
    float4 Normal : COLOR;
};

// VECTOR SHADER METHOD
VS_Scene_Output VSShadowScene(in VS_Scene_Input input)
{
    VS_Scene_Output output;

    output.position = mul(input.Position, World);
    output.position = mul(output.position, View);
    output.position = mul(output.position, Projection);
    output.position2D = output.position;
	
    output.lightPosition = mul(input.Position, World);
    output.lightPosition = mul(output.lightPosition, LightView);
    output.lightPosition = mul(output.lightPosition, LightProjection);

    output.Normal = input.Normal;

    return output;
}

float4 PSShadowScene(VS_Scene_Output input) : COLOR
{
    float visibility = 1.0f;

    float depth = input.lightPosition.z / input.lightPosition.w;
    float4 shadowMapColor = tex2D(ShadowMapSampler, input.lightPosition.xy * float2(0.5f, -0.5f) + float2(0.5f, 0.5f));
	
    float4 color = float4(0.5f, 0.5f, 0.5f, 1.0f);

    if (depth > shadowMapColor.z + 0.05f)
    {
		// we are in the shadows
        visibility = 0.5f;
        color.r = 1.0f;	// debugging!
        color.bg = 0.0f;
    }


	// recalculate color with new values!

    color.rgb = color.rgb * visibility;

    color.w = 1.0f;

    return color;

}


// DON'T CHANGE ANYTHING BELOW HERE!
technique GenerateShadowMap
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL VSShadowMap();
        PixelShader = compile PS_SHADERMODEL PSShadowMap();
    }
}

technique ShadowScene
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL VSShadowScene();
        PixelShader = compile PS_SHADERMODEL PSShadowScene();
    }
}

// EOF MARKER