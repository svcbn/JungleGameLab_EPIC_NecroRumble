// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteGradient" {
Properties {
     [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
     _ColorC ("Center Color", Color) = (1,1,1,1)
     _ColorS ("Side Color", Color) = (0,1,1,1)
}
SubShader {
     Tags {"Queue"="Transparent"  "IgnoreProjector"="True"}
     LOD 100
     ZWrite Off
     Pass {
          Blend SrcAlpha OneMinusSrcAlpha
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
         fixed4 _ColorC;
         fixed4 _ColorS;
         struct v2f {
             float4 pos : SV_POSITION;
             fixed4 col : COLOR;
         };
         v2f vert (appdata_full v)
         {
             v2f o;
             o.pos = UnityObjectToClipPos (v.vertex);
			 if(v.texcoord.x < 0.5)
             	o.col = lerp(_ColorS,_ColorC, v.texcoord.x / 0.5);
			else
             	o.col = lerp(_ColorC,_ColorS, (v.texcoord.x - 0.5) / 0.5 );
             return o;
         }
   
         float4 frag (v2f i) : COLOR {
             float4 c = i.col;
             return c;
         }
             ENDCG
         }
     }
}