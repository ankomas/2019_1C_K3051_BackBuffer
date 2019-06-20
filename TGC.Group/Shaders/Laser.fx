extern uniform float4x4 world;
extern uniform float4x4 view;
extern uniform float4x4 projection;

struct VertexInput
{
    float4 Position : POSITION;
};
struct VertexOutput
{
    float4 Position : POSITION;
    float4 ModelPosition : TEXCOORD;
};

VertexOutput main_vertex(VertexInput input)
{
    VertexOutput o;
    o.Position = o.ModelPosition = input.Position;
    o.Position = mul(o.Position, world);
    o.Position = mul(o.Position, view);
    o.Position = mul(o.Position, projection);
    return o;
}

float4 main_pixel(VertexOutput input) : COLOR
{
    float dist = saturate(abs(input.ModelPosition.x) / 0.1 - 0.25) * 2;
    return float4(dist == 0, 1, 1, 1 - dist);
}

technique Laser
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 main_vertex();
        PixelShader  = compile ps_3_0 main_pixel();
    }
}