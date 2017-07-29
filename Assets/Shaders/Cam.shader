// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Cam" {
		Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Lightness("Lightness", Float) = 0.5
		[MaterialToggle] DoClip("Clipping enabled", Float) = 0
		ClipArea("Clipping area", Vector) = (0,0,0,0)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		LightPos("Mouse position", Vector) = (0,0,0,0)
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DUMMY PIXELSNAP_ON
#include "UnityCG.cginc"

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		half2 texcoord  : TEXCOORD0;
		float4 vertexS  : TEXCOORD2;
	};

	fixed4 _Color;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.vertexS = ComputeScreenPos(OUT.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}

	sampler2D _MainTex;
	float _Lightness;
	float DoClip;
	float4 ClipArea;
	half4 LightPos;

#define PI 3.14159265358979323844

	fixed4 frag(v2f IN) : COLOR
	{
		if (DoClip > 0)
		{
			float2 wcoord = (IN.vertexS.xy / IN.vertexS.w * _ScreenParams.xy);
			wcoord.y = _ScreenParams.y - wcoord.y;
			if (wcoord.x < ClipArea[0] ||
				wcoord.y < ClipArea[1] ||
				wcoord.x >= ClipArea[0] + ClipArea[2] ||
				wcoord.y >= ClipArea[1] + ClipArea[3]) discard;
		}
	
		half2 c = half2(0.5, 0);
		/*
		half ratio = _ScreenParams.x / _ScreenParams.y;

		half2 uv = IN.texcoord;
		uv.y /= ratio;
		//uv.y += 0.5;
		half2 x = uv - half2(0.5, 0);
		float radius = length(x);
		//radius = pow(radius, 10);
		float angle = atan2(x.y, x.x);
		angle = (angle * 0.5 / PI) - 0.25;

		//angle /= 2;
		//angle *= 6;
		angle += 0.5;
		angle = 1.0 - angle;

		half4 texcol = tex2D(_MainTex, half2(angle, radius)) * IN.color * _Lightness * 2;*/
		half2 uv = IN.texcoord;
		/*half dst = abs(uv.x - 0.5);
		half dst2 = (uv.x - 0.5);
		half dsty = (1.0-uv.y);
		half shifty = (((sin(dst) + 1.0) / 2.0) * (dst + 1.0))/8;
		half shiftx = sin(dst2);
		uv.y += shifty * dst;
		uv.x += shiftx * dst;
		uv.x = (uv.x - 0.5) * (1.0-dst) + 0.5;*/
		half4 texcol = tex2D(_MainTex, uv) * IN.color * _Lightness * 2;

		// do light
		half lightpower = 256;
		half2 lightpowers = half2(lightpower / _ScreenParams.x, lightpower / _ScreenParams.y);
		half2 locallight = half2(LightPos.x - uv.x, (1.0 - LightPos.y) - uv.y);
		half dst = 1.0 - clamp(length(locallight / lightpowers), 0.0, 1.0);

		// now trace.
		// gather offsets.
		half ang = atan2(locallight.x, locallight.y);
		half dstreal = length(locallight);
		half2 deltapower = half2(dstreal, dstreal) / lightpowers;
		half2 delta = (half2(sin(ang), cos(ang)) / _ScreenParams.xy) * deltapower;
		
		for (half i = 0; i < lightpower; i++)
		{
			half4 texcol2 = tex2D(_MainTex, uv + delta*i);
			dst -= (texcol2.a*deltapower)/4;
		}
		dst = clamp(dst, 0.0, 1.0);

		texcol += half4(dst, dst, dst, dst);

		return texcol;
	}
		ENDCG
	}
	}
		Fallback "Sprites/Default"
}