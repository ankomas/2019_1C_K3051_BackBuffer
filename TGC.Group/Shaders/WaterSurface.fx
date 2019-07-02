#define MaxFarness 20000

struct VertexData
{
    float4 Position : POSITION;
    float4 WorldPos : TEXCOORD0;
    float4 Color : COLOR;
    float Y : TEXCOORD1;
    float Normal : NORMAL;
};

float4 camPos;
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

extern uniform float4x4 transform;
float tolerance = 0.3;
extern uniform float time;
extern uniform float4 cameraPosition;

float4 minColor(float4 c1, float4 c2)
{
    float4 ret = (c1.r < c2.r && c1.g < c2.g && c1.g < c2.g) ? c1: c2;

    return ret;
}

bool eq(float a, float b)
{
    return abs(a - b) < tolerance;
}

VertexData main_vertex(VertexData input)
{
    float f = 0.05;
    float amplitude = 10;
    input.Position.y = (sin((camPos.x/3 - input.Position.x) * f - time) + sin((camPos.z/3 - input.Position.z) * f - time)) * amplitude;

    float4 worldPos = mul(input.Position, transform);
    VertexData output = { worldPos, worldPos, input.Color, input.Position.y, input.Normal };
    return output;
}

float4 lightPosition = float4(0, 0, 0, 1);
float normalDirection = 1;
float isBelt = 0;

float3 texColor = float3(0.2, 0.5, 0.75);

float4 applyFogAndLight(VertexData input) : COLOR
{
    lightPosition = cameraPosition + float4(0, 5000, 0, 1);
    float4 pos = input.WorldPos;
    float ambient = 0.5;
    float3 ambientColor = float3(ambient, ambient, ambient);

    float3 normal = normalize(input.Normal) * normalDirection;
    float4 posToLight = normalize(lightPosition - pos);
    float dotP = dot(normal, posToLight.xyz);
    float diffuseK = saturate(dotP);
    float absDiffuseK = abs(dotP);
    float2 chooseK = { diffuseK, absDiffuseK };
    float k = chooseK[isBelt];
    float3 diffuseColor = float3(k, k, k);
	
    float4 lightToPos = posToLight * (-1);
    float4 posToCamera = normalize(cameraPosition - pos);
    float3 reflectedRay = normalize(reflect(lightToPos.xyz, normal));
    float specularK = pow(max(dot(reflectedRay, posToCamera.xyz), 0), 100);

    float3 specularColor = float3(specularK, specularK, specularK);

    float4 realColor = float4((ambientColor + diffuseColor + specularColor) * texColor, 1);

    float farness = length(pos - cameraPosition);

    float3 waterColor = float3(0, 0.7, 1);
    
    float4 depthWaterColor = float4(waterColor * (farness / MaxFarness), 0.5);
    float4 finalColor = lerp(realColor, depthWaterColor, saturate(0.6 + pow(farness / MaxFarness, 2)));

    return finalColor;
}

float4 maxWaterColor = float4(0.1, 0.8, 1, 1);

float4 main_pixel_outside(VertexData input) : COLOR
{
    return float4(texColor, 0.5) + float4(1, 1, 1, 1) * (input.Y / 200);
}
float4 main_pixel_underwater(VertexData input) : COLOR
{
    return minColor(maxWaterColor, applyFogAndLight(input) + float4(1, 1, 1, 1) * (input.Y / 200));
}

technique Plane
{
    pass unique
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader = compile ps_3_0 main_pixel_underwater();
    }
};