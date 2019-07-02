struct VertexData
{
    float4 Position : POSITION;
    float4 WorldPos : TEXCOORD0;
    float4 Color : COLOR;
    float Y : TEXCOORD1;
};

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
float tolerance = 0.005;
extern uniform float time;

bool eq(float a, float b)
{
    return abs(a - b) < tolerance;
}

VertexData main_vertex(VertexData input)
{
    float f = 0.05;
    float amplitude = 10;
    input.Position.y = (sin(input.Position.x * f - time) + sin(input.Position.z * f - time)) * amplitude;

    float4 worldPos = mul(input.Position, transform);
    VertexData output = { worldPos, worldPos, input.Color, input.Position.y };
    return output;
}

float4 main_pixel(VertexData input) : COLOR
{
    return float4(0.2, 0.5, 0.75, 0.5) + float4(1, 1, 1, 1) * (input.Y / 200);
}

technique Plane
{
    pass unique
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader = compile ps_3_0 main_pixel();
    }
};