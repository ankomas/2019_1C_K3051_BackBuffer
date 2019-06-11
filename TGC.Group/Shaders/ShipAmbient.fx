float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matInverseTransposeWorld;

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
};

bool eq(float a, float b)
{
	return abs(a - b) < 0.001;
}

extern uniform float4 lightPosition;
extern uniform float normalDirection = 1;
extern uniform float4 cameraPosition;

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;
	output.Normal = input.Normal;
	output.Position = mul(input.Position, matWorldViewProj);
	output.PositionForPixelShader = mul(input.Position, matWorld);
	return output;
}

float4 main_pixel(VertexOutput input) : COLOR
{
	float4 pos = input.PositionForPixelShader;
	float ambient = 0.2;
	float3 ambientColor = float3(ambient, ambient, ambient);

	float3 normal = normalize(input.Normal) * normalDirection;
	float4 posToLight = normalize(lightPosition - pos);
    float diffuseK = abs(dot(normal, posToLight.xyz));
    float3 diffuseColor = float3(diffuseK, diffuseK, diffuseK);
	
	float4 lightToPos = posToLight * (-1);
	float4 posToCamera = normalize(cameraPosition - pos);
	float3 reflectedRay = normalize(reflect(lightToPos.xyz, normal));
	float specularK = pow(max(dot(reflectedRay, posToCamera.xyz), 0), 1);

    float3 specularColor = float3(specularK, specularK, specularK);

    float3 blueBulb = float3(0.8, 0.8, 1);

    return float4((ambientColor + diffuseColor) * blueBulb, 1) * tex2D(diffuseMap, input.TexCoord.xy);
}

technique ShipAmbient
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
};