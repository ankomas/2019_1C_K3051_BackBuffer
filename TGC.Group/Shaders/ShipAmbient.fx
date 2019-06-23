#define c  0.9238795325112867f // cos(PI / 8)
#define s  0.3826834323650898f // sin(PI / 8)
#define MaxFarness 3000

extern uniform float4x4 matWorld;
extern uniform float4x4 matWorldView;
extern uniform float4x4 matWorldViewProj;
extern uniform float4x4 matInverseTransposeWorld;

const uniform float4x4 Rotate_PI_Over_8_On_Z =
{
    c, s, 0, 0,
   -s, c, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1
};
uniform float4x4 rotation =
{
    1, 0, 0, 0,
    0, 1, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1
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

extern uniform float4 lightPosition;
extern uniform float normalDirection = 1;
extern uniform float4 cameraPosition;
extern uniform float isBelt = 0;

extern uniform float time = 0;
extern uniform texture perlinNoise;
sampler2D perlinNoiseMap = sampler_state
{
    Texture = (perlinNoise);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

struct VertexInput
{
	float4 Color : COLOR;
	float4 Position : POSITION;
	float3 Normal : NORMAL;
	float4 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
	float4 Color : COLOR;
	float4 Position : POSITION;
	float3 Normal : NORMAL;
	float4 TexCoord : TEXCOORD0;
    float4 PositionForPixelShader : TEXCOORD1;
    float4 RotatedInitialPosition : TEXCOORD2;
};

float tolerance = 0.3;

bool eq(float a, float b)
{
	return abs(a - b) < tolerance;
}

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;
    output.Normal = mul(float4(input.Normal, 1), rotation).xyz;
    //output.Normal = input.Normal;
	output.Position = mul(input.Position, matWorldViewProj);
    output.PositionForPixelShader = output.RotatedInitialPosition = mul(input.Position, matWorld);
	return output;
}

VertexOutput main_vertex_crafted(VertexInput input)
{
    VertexOutput output;
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    output.Normal = mul(float4(input.Normal, 1), rotation).xyz;

    output.Position = mul(input.Position, mul(Rotate_PI_Over_8_On_Z, matWorldViewProj));
    output.PositionForPixelShader = mul(input.Position, mul(Rotate_PI_Over_8_On_Z, matWorld));
    output.RotatedInitialPosition = mul(input.Position, Rotate_PI_Over_8_On_Z);
    return output;
}

float4 main_pixel(VertexOutput input) : COLOR
{
	float4 pos = input.PositionForPixelShader;
	float ambient = 0.2;
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

    float3 blueBulb = float3(0.8, 0.8, 1);

    float4 realColor = float4((ambientColor + diffuseColor + specularColor) * blueBulb * tex2D(diffuseMap, input.TexCoord.xy).xyz, 1);

    float farness = length(pos - cameraPosition);

    float3 waterColor = float3(0, 0.4, 0.7);

    //float4 finalColor = lerp(realColor, duColor, saturate(farness / MaxFarness));
    float4 depthWaterColor = float4(waterColor * (farness / MaxFarness), 1);
    float4 finalColor = lerp(realColor, depthWaterColor, saturate(0.4 + farness / MaxFarness));

    return finalColor;
}

float4 main_pixel_crafted(VertexOutput input) : COLOR
{
    float4 pos = input.PositionForPixelShader;
    float ambient = 0.2;
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

    float3 blueBulb = float3(0.8, 0.8, 1);

    float noise = tex2D(perlinNoiseMap, input.TexCoord.xy).r;

    float4 ret;
    float t = time * 300;

    tolerance = 0.15;

    if (input.RotatedInitialPosition.y > t - 40
        && input.RotatedInitialPosition.y < t
        && eq((t - input.RotatedInitialPosition.y) / 40, tex2D(perlinNoiseMap, float2(0.5, input.RotatedInitialPosition.x / 200)).y))
        ret = float4(0.5, 1, 1, 1);
    else if ((t - input.RotatedInitialPosition.y) / 40 > tex2D(perlinNoiseMap, float2(0.5, input.RotatedInitialPosition.x / 20)).y)
        ret = float4(
        (ambientColor + diffuseColor + specularColor)
        * blueBulb
        * tex2D(diffuseMap, input.TexCoord.xy).xyz
        , input.RotatedInitialPosition.y < t);

    return ret;
}

technique ShipAmbient
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
};

technique CraftedItems
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 main_vertex_crafted();
        PixelShader = compile ps_3_0 main_pixel_crafted();
    }
};