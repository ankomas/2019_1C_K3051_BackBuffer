extern uniform float4 color, borderColor;
extern uniform float2 screenPos, size;
extern uniform float borderThickness;
uniform float tolerance = 0.02;

struct TransformedVertexData
{
    float4 ScreenCoordPosition : POSITIONT;
    float4 ShaderCoordPosition : COLOR;
};

bool eqDebug(float a, float b)
{
    return abs(a - b) < tolerance;
}

bool eq(float a, float b)
{
    return abs(a - b) < borderThickness;
}

float4 main_interior(TransformedVertexData input) : COLOR
{
    float x = input.ShaderCoordPosition.x;
    float y = input.ShaderCoordPosition.y;
    return color;
}

float4 main_border(TransformedVertexData input) : COLOR
{
    float abstractX = input.ShaderCoordPosition.x;
    float abstractY = input.ShaderCoordPosition.y;

    float x = screenPos.x + abstractX * size.x;
    float y = screenPos.y + abstractY * size.y;

    if (!(eq(x, screenPos.x) || eq(x, screenPos.x + size.x) || eq(y, screenPos.y) || eq(y, screenPos.y + size.y)))
    {
        discard;
    }

    return float4(borderColor.xyz, 1);
}

technique DialogBox
{
    pass Interior
    {
        PixelShader = compile ps_3_0 main_interior();
    }
    pass Border
    {
        PixelShader = compile ps_3_0 main_border();
    }
};