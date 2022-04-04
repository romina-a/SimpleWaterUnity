Shader "Romina/WaterShader2"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 0.5
		_WaterNormal ("Water Normal", vector) = (0,1,0,0)
		[HideInInspector] _WaterNormalRotationX ("Water Normal Rotation X", Range(0,360)) = 90
		[HideInInspector] _WaterNormalRotationY ("Water Normal Rotation Y", Range(0,360)) = 0
		[HideInInspector] _WaterNormalRotationZ ("Water Normal Rotation Z", Range(0,360)) = 90
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

        Tags {"Queue"="Transparent" "RenderType"="Transparent" "DisableBatching" = "True" }

        Pass
        {
		 Zwrite off // ? sets whether the depth buffer contents are updated during rendering normally ZWrite is enabled for opaque objects and disabled for semi-transparent ones
		 Ztest LEqual //ztest does not run on the current object, so if culling is off and ztest is LEqual, the back facing tirangles are renderd on top(?)
		 Cull off // we want the front and back faces, Culling is an optimization that does not render polygons facing away from the viewer. All polygons have a front and a back side.
		 AlphaToMask on // transparency on top (?)
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
			float fillEdge: TEXCOORD1;
		};
 
         float _FillAmount, _WaterNormalRotationX, _WaterNormalRotationY, _WaterNormalRotationZ;
         float4 _TopColor, _Color, _WaterNormal;
		

         v2f vert (appdata v)
         {
            v2f o;
			
            o.vertex = UnityObjectToClipPos(v.vertex); //equals to o.vertex = mul(mul(mul(UNITY_MATRIX_P, UNITY_MATRIX_V), unity_ObjectToWorld),v.vertex); but use this because weird stuff happens in AR, prbably
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.normal = v.normal;

			float l = (_FillAmount*2-1)* length(unity_ObjectToWorld._m01_m11_m21);
			float3 localPointOnWaterPlane = mul(unity_ObjectToWorld,float4(0,l,0,1));//TODO
			float4 wn = normalize (_WaterNormal);
			//float3 wn = normalize( float3( cos(radians(_WaterNormalRotationX)),cos(radians(_WaterNormalRotationY)), cos(radians(_WaterNormalRotationZ)) ) );
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.fillEdge = dot((worldPos.xyz-localPointOnWaterPlane), wn.xyz);
            return o;
         }

         fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
         {
		   float result = step(i.fillEdge, 0);
		   return facing > 0 ? result* _Color: result* _TopColor;
		   //return i.fillEdge;
         }
         ENDCG
        }
    }
}