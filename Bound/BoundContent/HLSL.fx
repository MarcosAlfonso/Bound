float4x4 xWorldViewProjection;
float4x4 xProjection;
float4x4 xView;
float4x4 xWorld;
float3 xLightDirDown;
float3 xLightDirUp;
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

//Vertex Shader, sends data to Pixel Shader
VertexToPixel SimplestVertexShader( float4 inPos : POSITION0, float3 inNormal: NORMAL0, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;
    
    Output.Position =mul(inPos, xWorldViewProjection);
    Output.TexCoords = inTexCoords;
    Output.Normal = normalize(mul(inNormal, xWorld));    
    Output.Position3D = mul(inPos, xWorld);
	

	Output.Depth = distance(float3(Output.Position3D.xyz),float3(cameraPosition.xyz));

    return Output;
}

//PixelShader, sens pixel data to frame
PixelToFrame SimplestPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    

    float diffuseLightingFactorDown = dot(xLightDirDown, PSIn.Normal);
    diffuseLightingFactorDown = saturate(diffuseLightingFactorDown);
    diffuseLightingFactorDown *= xLightPower;

	float diffuseLightingFactorUp = dot(xLightDirUp, PSIn.Normal);
    diffuseLightingFactorUp = saturate(diffuseLightingFactorUp);
    diffuseLightingFactorUp *= xLightPower;

	float l;

	if (PSIn.Position3D.y > 400)
		l = saturate((PSIn.Depth - xFogStart) / (xFogEnd - xFogStart));
	else
		l = saturate((PSIn.Depth - xFogStart) / (xFogEnd - xFogStart) + (-PSIn.Position3D.y+400)/900);


    PSIn.TexCoords.y--;
	float3 lightColor = xColor*(diffuseLightingFactorDown + diffuseLightingFactorUp + xAmbient);
	//float3 fogColor = lerp(xFogColor, float3(0.18f, 0.33f, .54f), PSIn.Position3D.y);
	Output.Color = float4(lerp(lightColor,xFogColor, l),1);

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
	 float3 Normal			: TEXCOORD2;
 };
 
 struct SDPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 SDVertexToPixel SkyDomeVS( float4 inPos : POSITION, float3 inNormal: NORMAL0, float2 inTexCoords: TEXCOORD0)
 {    
     SDVertexToPixel Output = (SDVertexToPixel)0;
	      
     Output.Position = mul(inPos, xWorldViewProjection );
     Output.ObjectPosition = mul(inPos,xWorld);
	 Output.Normal = normalize(mul(inNormal, xWorld));
     
     return Output;    
 }

// -------------------------------------------------------------
// Miscellaneous Functions
// -------------------------------------------------------------
float3 interpolate(float3 origin, float3 destination, float startStep, float endStep, float step)
{
	#ifdef HERMITE_INTERPOLATION
		return lerp(origin, destination, smoothstep(startStep, endStep, step));
	#elif defined(LINEAR_INTERPOLATION)
		return lerp(origin, destination, saturate((step - startStep) / (endStep - startStep)));
	#else // Fallback so that it compiles...
		return 0.0f.rrr;
	#endif
}
 
 SDPixelToFrame SkyDomePS(SDVertexToPixel PSIn)
 {
     SDPixelToFrame Output = (SDPixelToFrame)0;
	 
	 float normalY = PSIn.Normal.y;     

     float4 topColor = float4(0.1f, 0.2f, .4f, 1);    
     float4 bottomColor = float4(1.0f, 1.0f, 1.0f, 1);
	     
     Output.Color = lerp(bottomColor,topColor,saturate(((normalY-.1f)/.7))); 
 
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

