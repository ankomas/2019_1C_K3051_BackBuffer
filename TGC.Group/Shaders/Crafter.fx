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
	float4 Color : COLOR0;
	float4 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
	float4 TexCoord : TEXCOORD0;
	float4 PositionForPixelShader : TEXCOORD1;
};

float4 lightPos = float4(-3000, 3000, -1000, 1);

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;

	float k = 0.9 + input.Position.y / 50;
	output.Color = float4(k, k, k, 1);
	output.Position = mul(input.Position, matWorldViewProj);
	output.TexCoord = input.TexCoord;
	output.PositionForPixelShader = output.Position;

	return output;
}

float4 main_pixel(VertexOutput input) : COLOR
{
	float4 texColor = tex2D(diffuseMap, input.TexCoord);
	return input.Color * texColor;
}

technique CrafterTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
};