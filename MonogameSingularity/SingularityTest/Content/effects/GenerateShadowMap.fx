
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

float3 LightDirection;
float3 CameraPosition;

float4 AmbientLightColor;
float1 AmbientLightIntensity;

float4 DiffuseLightColor;
float1 DiffuseLightIntensity;

int UseTexture;
float4 DefaultColor;

Texture2D ShadowMap;

sampler2D ShadowMapSampler = sampler_state
{
    texture = <ShadowMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    //AddressU = clamp;
    //AddressV = clamp;
};

Texture2D Texture;

sampler2D TextureSampler = sampler_state {
	texture = <Texture>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
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
	float2 TexCoords: TEXCOORD0;
};

struct VS_Scene_Output
{
    float4 position : SV_POSITION;
    float4 lightPosition : TEXCOORD1;
    float4 position2D : TEXCOORD0;
    float4 Normal : COLOR;
	float2 TexCoords: TEXCOORD2;
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
	output.TexCoords = input.TexCoords;

    return output;
}

float4 PSShadowScene(VS_Scene_Output input) : COLOR
{
    float4 color = DefaultColor;
	
    if (UseTexture)
        color = tex2D(TextureSampler, input.TexCoords);

    float visibility = 1.0f;

    float depth = input.lightPosition.z / input.lightPosition.w;
    float4 shadowMapColor = tex2D(ShadowMapSampler, input.lightPosition.xy * float2(0.5f, -0.5f) + 0.5f);

    float angle = abs(acos(dot(input.Normal, (LightDirection * float3(-1, 1, -1)) / (length(input.Normal) * length(LightDirection)))));
    
    //if (angle >= 1.5707f && angle <= 3 * 1.5707f)
    //    visibility = 0.5f;

    if (depth > shadowMapColor.z + 0.0025f && shadowMapColor.w > 0.0f)
        visibility = 0.5f;
		

	// now get the dot product. We want this to be fully illuminated if the light shines directly at it.
	// and only partially if it is at an angle!
    float diffuseLighting = saturate(dot(input.Normal, (LightDirection * float3(-1, 1, -1))));

    //diffuseLighting *= ((length(LightDirection) * length(LightDirection)) / dot(light.Position - Input.WorldPosition, light.Position - Input.WorldPosition));
    float3 h = normalize(normalize(CameraPosition - input.position2D) + (LightDirection * float3(-1, 1, -1)));
    float specLighting = pow(saturate(dot(h, input.Normal)), 2.0f);

    color.rgb *= saturate(AmbientLightColor * AmbientLightIntensity + (DiffuseLightColor * diffuseLighting * DiffuseLightIntensity) + (specLighting * 0.5f)) * visibility;
	
    //color.rgb = abs(input.Normal) * visibility;

    //color.w = 1.0f;

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