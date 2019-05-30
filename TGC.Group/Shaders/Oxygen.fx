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

float isGreaterEqThanZero(float num)
{
	return sign(sign(num) + 1);
}

float isLowerEqThanZero(float num)
{
	return isGreaterEqThanZero(-num);
}

float isGreaterThanZero(float num)
{
	return !isLowerEqThanZero(num);
}

float isLowerThanZero(float num)
{
	return !isGreaterEqThanZero(num);
}

float isLowerEqThan(float a, float b)
{
	return isLowerEqThanZero(a - b);
}

float isGreaterEqThan(float a, float b)
{
	return isGreaterEqThanZero(a - b);
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

uniform float stripeRadialStart = 0.32;
uniform float stripeRadialEnd = 0.5;

float edgeTransparency(float2 absolutePos, float2 circleOrigin, float circleRadius)
{
	float2 center = float2(0.5, 0.5);
	float2 distFromCircleOrigin = length(absolutePos - circleOrigin);

	float2 pos = absolutePos - center;
	float radialDist = length(pos);

	float transparency = pow(cos(distFromCircleOrigin * (PI / (stripeRadialEnd - stripeRadialStart))) + 0.8, 10) * isLowerEqThan(distFromCircleOrigin, circleRadius);

	return transparency;
}

float4 new_pixel(PixelInput input) : COLOR
{
	float2 absolutePos = input.Color.rg;
	float2 center = float2(0.5, 0.5);
	float2 pos = absolutePos - center;
	float radialDist = length(pos);

	float angleForColor = (PI * isGreaterEqThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float primitiveAngle = (PI * isGreaterEqThanZero(pos.y) - signOf(pos.y) * asin(abs(pos.x) / radialDist));
	float realAngle = modulus(primitiveAngle - isLowerEqThanZero(pos.x) * 2 * primitiveAngle, 2 * PI);
	float blueIntensity = angleForColor / PI;

	float transparency =
		isGreaterEqThan(radialDist, stripeRadialStart)
		* isLowerEqThan(radialDist, stripeRadialEnd)
		* isLowerEqThan(realAngle, oxygen * 2 * PI)
		* pow(sin((radialDist - stripeRadialStart) * (PI / (stripeRadialEnd - stripeRadialStart))) + 0.8, 10);

	float circleDistanceFromOrigin = stripeRadialStart + (stripeRadialEnd - stripeRadialStart) / 2;
	float2 circleOrigin = float2(0.5, 0.5 - circleDistanceFromOrigin);
	float2 o2AngleUnitVector = float2(sin(oxygen * 2 * PI), -cos(oxygen * 2 * PI));
	float2 secondCircleOrigin = float2(0.5, 0.5) + o2AngleUnitVector * circleDistanceFromOrigin;

	float circleRadius = (stripeRadialEnd - stripeRadialStart) / 2;

	float4 lowCircleTransparency = edgeTransparency(absolutePos, circleOrigin, circleRadius);
	float4 highCircleTransparency = edgeTransparency(absolutePos, secondCircleOrigin, circleRadius);

	float finalTransparency = max(transparency, max(lowCircleTransparency.a, highCircleTransparency.a));

	return float4(1 - oxygen, oxygen, blueIntensity, isGreaterThanZero(oxygen) * finalTransparency);
}

float4 main_pixel2(PixelInput input) : COLOR
{
	float2 absolutePos = input.Color.rg;
	float2 center = float2(0.5, 0.5);
	float2 pos = absolutePos - center;
	float radialDist = sqrt(pow(pos.x, 2) + pow(pos.y, 2));
	float k = radialDist * 2.5;
	return float4(k, k, k, isLowerEqThanZero(radialDist - 0.3) * (120.0 / 255.0));
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
		PixelShader = compile ps_3_0 new_pixel();
	}
	pass Pass_1
	{
		VertexShader = compile vs_3_0 main_vertex();
		PixelShader = compile ps_3_0 main_pixel2();
	}
};