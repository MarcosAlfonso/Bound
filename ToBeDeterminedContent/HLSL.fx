float4x4 xWorldViewProjection;
float4x4 xWorld;
float3 xLightPos;
float xLightPower;
float xAmbient;
float3 xColor;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//Data that is transferred from Vertex Shader to Pixel Shader
struct VertexToPixel
{
    float4 Position     : POSITION;    
    float2 TexCoords    : TEXCOORD0;
    float3 Normal        : TEXCOORD1;
    float3 Position3D    : TEXCOORD2;
};

//Data that is sent to screen from Pixel Shader
struct PixelToFrame
{
    float4 Color        : COLOR0;
};

//Dot Product for lighting
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}

//Vertex Shader, sends data to Pixel Shader
VertexToPixel SimplestVertexShader( float4 inPos : POSITION0, float3 inNormal: NORMAL0, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;
    
    Output.Position =mul(inPos, xWorldViewProjection);
    Output.TexCoords = inTexCoords;
    Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));    
    Output.Position3D = mul(inPos, xWorld);

    return Output;
}

//PixelShader, sens pixel data to frame
PixelToFrame SimplestPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    

    float diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
    diffuseLightingFactor = saturate(diffuseLightingFactor);
    diffuseLightingFactor *= xLightPower;

    PSIn.TexCoords.y--;
	float3 baseColor = xColor;
    Output.Color.rgb = baseColor*(diffuseLightingFactor + xAmbient);

    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 SimplestPixelShader();
    }
}

