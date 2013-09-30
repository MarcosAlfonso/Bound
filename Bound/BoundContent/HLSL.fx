float4x4 xWorldViewProjection;
float4x4 xProjection;
float4x4 xView;
float4x4 xWorld;
float3 xLightDir;
float xLightPower;
float xAmbient;
float3 xColor;

float3 cameraPosition;

float xFogStart;
float xFogEnd;
float3 xFogColor;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//Data that is transferred from Vertex Shader to Pixel Shader
struct VertexToPixel
{
    float4 Position     : POSITION;    
    float2 TexCoords    : TEXCOORD0;
    float3 Normal        : TEXCOORD1;
    float3 Position3D    : TEXCOORD2;
	float Depth			: TEXCOORD3;
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
	Output.Depth = Output.Position.z;

    return Output;
}

//PixelShader, sens pixel data to frame
PixelToFrame SimplestPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    

    float diffuseLightingFactor = dot(xLightDir, PSIn.Normal);
    diffuseLightingFactor = saturate(diffuseLightingFactor);
    diffuseLightingFactor *= xLightPower;

	float l = saturate((PSIn.Depth - xFogStart) / (xFogEnd - xFogStart));

    PSIn.TexCoords.y--;
	float3 baseColor = xColor;
	Output.Color = float4(lerp(baseColor,xFogColor, l)*(diffuseLightingFactor + xAmbient), 1);

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

//------- Technique: SkyDome --------
 struct SDVertexToPixel
 {    
     float4 Position         : POSITION;
     float2 TextureCoords    : TEXCOORD0;
     float4 ObjectPosition    : TEXCOORD1;
 };
 
 struct SDPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 SDVertexToPixel SkyDomeVS( float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
 {    
     SDVertexToPixel Output = (SDVertexToPixel)0;
	      
     Output.Position = mul(inPos, xWorldViewProjection );
     Output.ObjectPosition = inPos;
     
     return Output;    
 }
 
 SDPixelToFrame SkyDomePS(SDVertexToPixel PSIn)
 {
     SDPixelToFrame Output = (SDPixelToFrame)0;        

     float4 topColor = float4(0.34f, 0.62f, .85f, 1);    
     float4 bottomColor = float4(1.0f, 0.6f, .09f, 1);    
     
     float4 baseColor = lerp(bottomColor, topColor, saturate((PSIn.ObjectPosition.y)/0.55f));
     float4 cloudValue = tex2D(TextureSampler, PSIn.TextureCoords).r;
     
     Output.Color = lerp(baseColor,1, cloudValue);        
 
     return Output;
 }
 
 technique SkyDome
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 SkyDomeVS();
         PixelShader = compile ps_2_0 SkyDomePS();
     }
 }

