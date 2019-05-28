float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matInverseTransposeWorld;

extern uniform float screen_dx = 1024;
extern uniform float screen_dy = 768;

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
	float4 Position : POSITION0;
	float4 Normal : NORMAL;
};

struct VertexOutput
{
	float4 Position : POSITION0;
	float4 PositionForPixel : TEXCOORD1;
	float4 Normal : NORMAL;
	float4 Color : COLOR;
};

float4 lightPos = float4(-3000, 3000, -1000, 1);

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;

	input.Position = mul(input.Position, matWorld);

	output.PositionForPixel = input.Position;

	input.Position = mul(input.Position, matInverseTransposeWorld);
	input.Position = mul(input.Position, matWorldViewProj);

	output.Position = input.Position;
	output.Normal = input.Normal;
	output.Color = normalize(input.Normal);

	return output;
}

float4 main_pixel(VertexOutput input) : COLOR
{
	//float k = dot(normalize(input.Normal.xyz), normalize(lightPos.xyz - input.PositionForPixel.xyz));

	//return float4(k, k, k, 1);
	return input.Color;
}

technique CrafterTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
};