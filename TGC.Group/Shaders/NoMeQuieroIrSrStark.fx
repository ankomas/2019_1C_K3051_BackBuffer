
//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))
float elapsedTime = 1.0;

<<<<<<< HEAD
=======
float screen_dx = 1024;
float screen_dy = 768;
float elapsedTime = 1.0;
float frecuencia = 10;
>>>>>>> 0fd6cc10d9f901b1128c0675f07e707544f48b09


//Textura para DiffuseMap
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


//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;

    //if(frac(Input.Position.x * elapsedTime)  > 0.3)
<<<<<<< HEAD
      //  Input.Position.x -= abs(Input.Position.x - centerX) * time / 5 ;
=======
      //  Input.Position.x -= abs(Input.Position.x - centerX) * elapsedTime / 5 ;
>>>>>>> 0fd6cc10d9f901b1128c0675f07e707544f48b09

    Output.RealPos = Input.Position;

	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);
   
	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord; 

	//Propago el color x vertice
    Output.Color = Input.Color;

    return (Output);
}


//Pixel Shader
float4 ps_main(VS_OUTPUT Input) : COLOR0
{
<<<<<<< HEAD
    if((frac(Input.RealPos.x * elapsedTime) + 0.3 )* time > 1 || (frac(Input.RealPos.y * elapsedTime ) + 0.2 )* elapsedTime > 1)
=======
    if((frac(Input.RealPos.x * elapsedTime) + 0.3 )* elapsedTime > 1 || (frac(Input.RealPos.y * elapsedTime ) + 0.2 )* elapsedTime > 1)
>>>>>>> 0fd6cc10d9f901b1128c0675f07e707544f48b09
        discard;
    return tex2D(diffuseMap, Input.Texcoord);
}

technique RenderScene
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}

