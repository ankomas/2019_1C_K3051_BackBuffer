#define MaxFarness 20000
extern uniform float elapsedTime;
extern uniform float time;
extern uniform float4x4 matWorld;
extern uniform float4x4 matWorldView;
extern uniform float4x4 matWorldViewProj;
extern uniform float4x4 matInverseTransposeWorld;
extern uniform float4 cameraPosition;

float tolerance = 0.3;
float4 lightPosition = float4(0, 0, 0, 1);
float normalDirection = 1;
float isBelt = 0;

struct VertexData
{
    float4 Position : POSITION;
    float2 UV : TEXCOORD0;
    float4 PositionForPixelShader : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
    float3 Normal : NORMAL;
};

texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

VertexData main_vertex(VertexData input)
{
    float4 worldPos = mul(input.Position, matWorld);
    float4 projectedPos = mul(input.Position, matWorldViewProj);
    float3 transformedNormal = mul(float4(input.Normal, 0), matWorld).xyz;

    VertexData output = { projectedPos, input.UV, projectedPos, worldPos, transformedNormal };

    return output;
}

bool eq(float a, float b)
{
    return abs(a - b) < tolerance;
}

float4 applyFogAndLight(VertexData input) : COLOR
{
    lightPosition = cameraPosition + float4(0, 5000, 0, 1);
    float4 pos = input.PositionForPixelShader;
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

    float4 realColor = float4((ambientColor + diffuseColor + specularColor) * tex2D(diffuseMap, input.UV.xy).xyz, 1);

    float farness = length(pos - cameraPosition);

    float3 waterColor = float3(0, 0.7, 1);
    
    float4 depthWaterColor = float4(waterColor * (farness / MaxFarness), 1);
    float4 finalColor = lerp(realColor, depthWaterColor, saturate(0.4 + farness / MaxFarness));

    return finalColor;
}

float4 main_pixel(VertexData Input) : COLOR0
{
    if ((frac(Input.PositionForPixelShader.x * elapsedTime) + 0.3) * elapsedTime > 1 || (frac(Input.PositionForPixelShader.y * elapsedTime) + 0.2) * elapsedTime > 1)
        discard;
    return applyFogAndLight(Input);
}

technique Fish
{
    pass unique
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader = compile ps_3_0 main_pixel();
    }
};


