Shader "Romina/WaterShader"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _TopColor ("Top Color", Color) = (1,1,1,1)
        [HideInInspector] _FillAmount ("Fill Amount", Range(0,1)) = 0.5
		//[HideInInspector] _WobbleX ("WobbleX", Range(-1,1)) = 0.0
		//[HideInInspector] _WobbleZ ("WobbleZ", Range(-1,1)) = 0.0
		
		_WobbleX ("WobbleX", Range(-1,1)) = 0.0
		_WobbleZ ("WobbleZ", Range(-1,1)) = 0.0
		//_FoamColor ("Foam Line Color", Color) = (1,1,1,1)
		//_Rim ("Foam Line Width", Range(0,0.1)) = 0.0
		//_RimColor ("Rim Color", Color) = (1,1,1,1)
	    //_RimPower ("Rim Power", Range(0,10)) = 0.0
    }
 
    SubShader
    {
		/***********************************
		Tags:
		https://docs.unity3d.com/Manual/SL-SubShaderTags.html
		you can access these in the c# file:
		   Renderer myRenderer = GetComponent<Renderer>();
           string tagValue = myRenderer.material.GetTag(ExampleTagName, true, "Tag not found");

		can have custom Tags

		about these tags:

		"Queue": the order of rendering
		https://docs.unity3d.com/ScriptReference/Rendering.RenderQueue.html

		"DisableBatching" is true because the shader uses object space operations
		************************************/

        Tags {"Queue"="Transparent"  "DisableBatching" = "True" }

        Pass
        {
		 Zwrite off // ? sets whether the depth buffer contents are updated during rendering normally ZWrite is enabled for opaque objects and disabled for semi-transparent ones
		 Ztest LEqual //ztest does not run on the current object, so if culling is off and ztest is LEqual, the back facing tirangles are renderd on top(?)
		 Cull off // we want the front and back faces, Culling is an optimization that does not render polygons facing away from the viewer. All polygons have a front and a back side.
		 //AlphaToMask off // transparency on top (?)
		 Blend SrcAlpha OneMinusSrcAlpha

         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
 
         struct appdata
         {
           float4 vertex : POSITION;
		   float3 normal : NORMAL;
         };
 
         struct v2f
         {
            float4 vertex : SV_POSITION;
			float3 viewDir : COLOR;
		    float3 normal : COLOR2;
			float fillEdge : TEXCOORD0;
         };
 
         float _FillAmount, _WobbleX, _WobbleZ;
         float4 _TopColor, _RimColor, _FoamColor, _Color;
         float _Rim, _RimPower;
           

         v2f vert (appdata v)
         {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            float3 worldPos = mul (unity_ObjectToWorld, v.vertex.xyz);
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
            o.normal = v.normal;

			float y = worldPos.y + (v.vertex.x  * _WobbleX) + (v.vertex.z * _WobbleZ); 
			o.fillEdge = y;
            return o;
         }

         fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
         {
		   // rim light
		   //float dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
           //float4 RimResult = smoothstep(0.5, 1.0, dotProduct);
           //RimResult *= _RimColor;
		   // foam edge
		   //float4 foam = ( step(i.fillEdge, 0.5) - step(i.fillEdge, (0.5 - _Rim)));
           //float4 foamColored = foam * (_FoamColor * 0.75);
		   // rest of the liquid
		   //float4 result = step(i.fillEdge, 0.1) ;//- foam;
		   float l = (_FillAmount*2-1)* length(unity_ObjectToWorld._m01_m11_m21);
		   float result = step(i.fillEdge, l);
           //float4 resultColored = result * _Color;
		   // both together
           //float4 finalResult = resultColored;// + foamColored;
		   //finalResult.rgb += RimResult;
		   // color of backfaces/ top
		   //float4 topColor = _TopColor * result;//(foam + result);
		   //VFACE returns positive for front facing, negative for backfacing
		   //return facing > 0 ? finalResult : topColor;
		   return facing > 0 ? result* _Color: result* _TopColor;    
         }
         ENDCG
        }
    }
}