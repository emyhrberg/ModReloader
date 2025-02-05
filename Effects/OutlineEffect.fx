sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float4 uShaderSpecificData;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 col = tex2D(uImage0, coords);
	if (!any(col))
	{
		float w = 2.0 / uImageSize0.x;
		float h = 2.0 / uImageSize0.y;
		
		// Expand the border outline and enhance the glow
		if (any(tex2D(uImage0, coords + float2(w, 0))) || any(tex2D(uImage0, coords - float2(w, 0))) ||
			any(tex2D(uImage0, coords + float2(0, h))) || any(tex2D(uImage0, coords - float2(0, h))))
		{
			return float4(1.0, 0.8, 0.3, 1.0) * 0.8; // Bright gold glow
		}
		else
		{
			w = w * 2;
			h = h * 2;
			if (any(tex2D(uImage0, coords + float2(w, 0))) || any(tex2D(uImage0, coords - float2(w, 0))) ||
				any(tex2D(uImage0, coords + float2(0, h))) || any(tex2D(uImage0, coords - float2(0, h))))
			{
				return float4(1.0, 0.7, 0.2, 1.0) * 0.6; // Slightly dimmer gold
			}
			else
			{
				w = w * 2;
				h = h * 2;
				if (any(tex2D(uImage0, coords + float2(w, 0))) || any(tex2D(uImage0, coords - float2(w, 0))) ||
					any(tex2D(uImage0, coords + float2(0, h))) || any(tex2D(uImage0, coords - float2(0, h))))
				{
					return float4(1.0, 0.6, 0.1, 1.0) * 0.4; // Soft golden fade-out
				}
			}
		}
		
		return float4(0, 0, 0, 0);
	}

	// Boost brightness and gold tint
	float3 goldTint = float3(1.2, 1.0, 0.6); // Enhanced gold tone
	return sampleColor * float4(goldTint, 1.0) * 1.2; // Brighter, golden effect
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
