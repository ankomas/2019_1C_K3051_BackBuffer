float4x4 scaleHalf =
{
	0.5, 0  , 0  , 0  ,
	0  , 0.5, 0  , 0  ,
	0  , 0  , 1  , 0  ,
	0  , 0  , 0  , 1  
};
float4x4 translateLeft =
{
	1  , 0  , 0  ,-0.5,
	0  , 1  , 0  , 0  ,
	0  , 0  , 1  , 0  ,
	0  , 0  , 0  , 1
};
float4x4 rotate90CounterClockwise =
{
	0  ,-1  , 0  , 0  ,
	1  , 0  , 0  , 0  ,
	0  , 0  , 1  , 0  ,
	0  , 0  , 0  , 1
};
float4x4 scaleHalfX =
{
	0.5, 0  , 0  , 0  ,
	0  , 1  , 0  , 0  ,
	0  , 0  , 1  , 0  ,
	0  , 0  , 0  , 1
};

struct VertexInput
{
	float4 Color : COLOR;
	float4 Position : POSITION;
};

struct VertexOutput
{
	float4 Color : COLOR;
	float4 Position : POSITION;
	float4 PositionForPixelShader : TEXCOORD;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Color = input.Color;
	output.Position = input.Position;

	output.Position = mul(scaleHalf, output.Position);
	output.Position = mul(rotate90CounterClockwise, output.Position);
	output.Position = mul(scaleHalfX, output.Position);
	output.Position = mul(translateLeft, output.Position);
	output.PositionForPixelShader = output.Position;

	return output;
}

float4 main_pixel(VertexOutput input) : COLOR
{
	float k = abs(input.PositionForPixelShader.y) * 2;
	return float4(k, k, k, 1);
}

technique Fede
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader  = compile ps_3_0 main_pixel();
	}
};