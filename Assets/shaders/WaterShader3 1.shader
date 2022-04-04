Shader "Romina/WaterShader3"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 0.5
		_WaterNormal ("Water Normal", vector) = (0,1,0,0)
		_XScale ("X Scale", float) = 1
		_YScale ("Y Scale", float) = 1
		_ZScale ("Z Scale", float) = 1

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
		 Zwrite on // ? sets whether the depth buffer contents are updated during rendering normally ZWrite is enabled for opaque objects and disabled for semi-transparent ones
		 Ztest LEqual //ztest does not run on the current object, so if culling is off and ztest is LEqual, the back facing tirangles are renderd on top(?)
		 Cull off // we want the front and back faces, Culling is an optimization that does not render polygons facing away from the viewer. All polygons have a front and a back side.
		 //AlphaToMask on // transparency on top (?)
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
			float fillEdge: TEXCOORD2;
		};
 
         float _FillAmount, _WaterNormalRotationX, _WaterNormalRotationY, _WaterNormalRotationZ, _XScale, _YScale, _ZScale;
         float4 _TopColor, _Color, _WaterNormal;
		

         v2f vert (appdata v)
         {
            v2f o;
			
            o.vertex = UnityObjectToClipPos(v.vertex); //equals to o.vertex = mul(mul(mul(UNITY_MATRIX_P, UNITY_MATRIX_V), unity_ObjectToWorld),v.vertex); but use this because weird stuff happens in AR, prbably
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.normal = v.normal;

			float l = (_FillAmount*2-1)* _YScale;
			float3 PointOnWaterPlane = float3(0,l,0);//TODO
			float3 wn = normalize(mul(unity_WorldToObject,_WaterNormal.xyz));
			//float3 wn = normalize( float3( cos(radians(_WaterNormalRotationX)),cos(radians(_WaterNormalRotationY)), cos(radians(_WaterNormalRotationZ)) ) );
			float pic = dot((v.vertex.xyz-PointOnWaterPlane), wn);
			o.fillEdge = pic;
			if (pic > 0)
			{
				// reflecting the points above the water plane on the water plane
				float4 newvertex = v.vertex;
				//if ( abs(wn.y) >= 0.5)
				//{
					float newy = (wn.y*l-wn.x*v.vertex.x-wn.z*v.vertex.z)/wn.y;
					newvertex = float4(v.vertex.x, newy, v.vertex.z, v.vertex.w);
				//}
				
				o.fillEdge = dot((newvertex.xyz-PointOnWaterPlane), wn);
				o.vertex = UnityObjectToClipPos(newvertex);

				//checking if the point is inside the the cylinder (//TODO this only works for cylinders!!!)

				/*
				float newy1 = (wn.y*l-wn.x*_XScale)/wn.y;
				float ellipse_a = sqrt(_XScale*_XScale + newy1* newy1);
				float newy2 = (wn.y*l-wn.z*_ZScale)/wn.y;
				float ellipse_b = sqrt(_ZScale*_ZScale + newy1* newy1);
				bool is_inside = //??? TODO: I don't have the newvertex coordinates on the plane where the ellipsis is defined
				*/

				//bool is_inside = length(newvertex.xyz-float3(0,l,0))<= _ZScale && length(newvertex.xyz-float3(0,l,0))<= _XScale;
				//if (!is_inside){
				//	o.fillEdge = 10;
				//}

			//	o.vertex.xyz = unityobjecttoclippos(v.vertex.xyz-d);
			//	o.top = 1;
			}
            return o;
         }

         fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
         {
		   //float result = step(i.fillEdge, 0);
		   return i.fillEdge < -0.005 ?  _Color: (i.fillEdge < 0.005? _TopColor : float4(0, 0, 0, 0));  

		  // return _Color;
         }
         ENDCG
        }
    }
}

/****************
TODO list:

[Water surface related]

-) might need to change later: collapsing points now in the object's y direction, might want to collapse them in the gravity direction]

1) do all the calculations in world space because object space is affected by the scale of the object

2) add limitation from the containter to the liquid (to limit the vertices to be inside the object)


[Direction manipulation]

1) read about showing roataion and dealing with rotation in 3d and change accordingly


[Realistic water render transparency and stuff]

1) understand initial micros

*****************/