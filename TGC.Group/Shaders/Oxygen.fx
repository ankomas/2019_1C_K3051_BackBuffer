struct VertexOutput
{
	float4 Position : POSITION;
	float Color : COLOR;
};
struct VertexInput
{
	float4 Position : POSITION;
	float4 Color: COLOR;
};

struct PixelInput
{
	float4 Position : POSITIONT;
	float4 Color: COLOR;
};

VertexOutput main_vertex(VertexInput input)
{
	VertexOutput output;
	output.Position = input.Position;
	output.Color = input.Color;
	return output;
}

float PI = 3.1415;

float signOf(float num)
{
	return sign(num) + !num;
}

float isGreaterThanZero(float num)
{
	return sign(sign(num) + 1);
}

float isLowerThanZero(float num)
{
	return isGreaterThanZero(-num);
}

bool eq(float4 a, float4 b)
{
	return abs(a - b) < 0.05;
}

float modulus(float num, float base)
{
	return (num % base + base) % base;
}

extern uniform float oxygen;

float edgeTransparency(float2 absolutePos, float2 circleOrigin)
{
	float2 center = float2(0.5, 0.5);
	float circleRadius = 0.1;
	float2 distFromCircleOrigin = length(absolutePos - circleOrigin);

	float2 pos = absolutePos - center;
	float radialDist = length(pos);

	float transparency = (0.8 + cos((length(circleOrigin - center) + length(absolutePos - circleOrigin)) * 25 + 2.5)) * isLowerThanZero(distFromCircleOrigin - circleRadius);

	return transparency;
}

float4 main_pixel(PixelInput input) : COLOR
{
	float2 absolutePos = input.Color.rg;
	float2 center = float2(0.5, 0.5);
	float2 pos = absolutePos - center;
	float radialDist = sqrt(pow(pos.x, 2) + pow(pos.y, 2));

	float angleForColor = (PI * isGreaterThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float primitiveAngle = (PI * isGreaterThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float realAngle = modulus(primitiveAngle - isLowerThanZero(pos.x) * 2 * primitiveAngle, 2 * PI);
	float blueIntensity = angleForColor / PI;

	float transparency = ((0.8 + cos(radialDist * 25 + 2.5)) * (isGreaterThanZero(radialDist * 25 + 2.5 - (3.5) * PI + 2) && isLowerThanZero(radialDist * 10 - 1.7 * PI))) * isLowerThanZero(realAngle - oxygen * 2 * PI);

	float circleDistanceFromOrigin = 0.403;
	float2 circleOrigin = float2(0.5, 0.5 - circleDistanceFromOrigin);
	float2 o2AngleUnitVector = float2(sin(oxygen * 2 * PI), -cos(oxygen * 2 * PI));
	float2 secondCircleOrigin = float2(0.5, 0.5) + o2AngleUnitVector * circleDistanceFromOrigin;

	float4 lowCircleTransparency = edgeTransparency(absolutePos, circleOrigin);
	float4 highCircleTransparency = edgeTransparency(absolutePos, secondCircleOrigin);

	float finalTransparency = max(transparency, max(lowCircleTransparency.a, highCircleTransparency.a));

	return float4(1 - oxygen, oxygen, blueIntensity, finalTransparency);
}

float4 main_pixel2(PixelInput input) : COLOR
{
	float2 absolutePos = input.Color.rg;
	float2 center = float2(0.5, 0.5);
	float2 pos = absolutePos - center;
	float radialDist = sqrt(pow(pos.x, 2) + pow(pos.y, 2));
	float k = 1 - radialDist * 2.5;
	return float4(k, k, k, isLowerThanZero(radialDist - 0.3) * (120.0 / 255.0));
}

float4 main_pixel_debug_coords(PixelInput input) : COLOR
{
	float2 xy = input.Color.rg;
	return float4(1, 0, 1, (eq(xy.x,0) || eq(xy.y, 0)));
}

technique OxygenTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel();
	}
	pass Pass_1
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel2();
	}
};