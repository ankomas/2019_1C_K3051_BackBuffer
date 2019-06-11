extern uniform float4x4 transform;

struct VertexInput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

struct VertexOutput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
    float4 PositionForPixelShader : TEXCOORD0;
    float4 FinalPosition : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
    output.PositionForPixelShader = input.Position;
    output.Position = output.FinalPosition = mul(input.Position, transform);
    output.WorldPosition = input.Position;
    output.Color = input.Color;

	/*output.Color = 1 - output.Position.z * 2;*/
	return output;
}

bool eq(float a, float b)
{
    return abs(a - b) < 0.05;
}

float4 main_pixel(VertexOutput input) : COLOR
{
    float i = 0.6 + (eq(input.Color.r, 0) || eq(input.Color.g, 0) || eq(input.Color.b, 0)) * 0.4;
    float4 color = float4(0, i, i, 0.95);

    float k = 0.5 + 0.5 * input.WorldPosition.z / 0.1;
    float4 light = float4(k, k, k, 1);
    return color * light;
}

technique OrientationArrow
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader  = compile ps_3_0 main_pixel();
	}
};