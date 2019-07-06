#define MaxFarness 20000

struct VertexData
{
    float4 Position : POSITION;
    float2 UV : TEXCOORD0;
    float4 PositionForPixelShader : TEXCOORD1;
    float3 Normal : NORMAL;
};

texture tex;
sampler2D mapper = sampler_state
{
    Texture = (tex);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

float4x4 transform;

VertexData main_vertex(VertexData input)
{
    float4 worldPos = mul(input.Position, transform);
    VertexData output = { worldPos, input.UV, worldPos, input.Normal };
    return output;
}

float4 main_pixel(VertexData input) : COLOR
{
    float4 color = tex2D(mapper, input.UV);

    return color;
}

float4 lightPosition = float4(0, 0, 0, 1);
float normalDirection = 1;
extern uniform float4 cameraPosition;
float isBelt = 0;

uniform extern float skyboxFarness;

float4 main_pixel_underwater(VertexData input) : COLOR
{
    float4 realColor = tex2D(mapper, input.UV);

    float3 waterColor = float3(0, 0.7, 1);
    
    float4 depthWaterColor = float4(waterColor * (skyboxFarness / MaxFarness), 1);
    float4 finalColor = lerp(realColor, depthWaterColor, saturate(0.4 + skyboxFarness / MaxFarness));

    return finalColor;
}

technique Outside
{
    pass unique
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader = compile ps_3_0 main_pixel();
    }
};

technique Underwater
{
    pass unique
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader = compile ps_3_0 main_pixel_underwater();
    }
};