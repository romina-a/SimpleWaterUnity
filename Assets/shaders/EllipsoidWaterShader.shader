Shader "Romina/EllipsoidWaterShader"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", float) = 0.5 // TODO: what can it be?
		_WaterNormal ("Water Normal", vector) = (0,1,0,0)
		_Scale("Scale", vector) = (1, 1, 1, 0) // The scale of the cylinder in x,y,z directions
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

		 //Ztest LEqual //ztest does not run on the current object, so if culling is off and ztest is LEqual, the back facing tirangles are renderd on top(?)

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
			float fillEdge: TEXCOORD2;
		};

        float4 _TopColor, _Color, _WaterNormal, _Scale;

		float get_ellipsoid_value(float4 newvertex){
			float x = 1/(_Scale.x*_Scale.x)*newvertex.x*newvertex.x
				+1/(_Scale.y*_Scale.y)*newvertex.y*newvertex.y
				+1/(_Scale.z*_Scale.z)*newvertex.z*newvertex.z;
			return x;
        }

		bool is_inside(float4 newvertex){
			float x = get_ellipsoid_value(newvertex);
			if(x>1){
				return false;
            } else {
				return true;
            }
		}

        v2f vert (appdata v)
        {
			v2f o;
			
			o.vertex = UnityObjectToClipPos(v.vertex); //equals to o.vertex = mul(mul(mul(UNITY_MATRIX_P, UNITY_MATRIX_V), unity_ObjectToWorld),v.vertex); but use this because weird stuff happens in AR, prbably
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.normal = v.normal;

			float4 ObjectCenter = float4(0,0,0,1); // object coordinates
			ObjectCenter = mul(unity_ObjectToWorld, ObjectCenter); // world coordinates
			float3 wn = normalize(_WaterNormal.xyz); // world coordinates
			float3 pointOnWaterPlane = ObjectCenter; // world coordinates // TODO --> where should the line be in each direction?
			float4 vertex_world = mul(unity_ObjectToWorld, v.vertex); // world coordinates
			float pic = dot((vertex_world.xyz-pointOnWaterPlane.xyz), wn); // if vertex is above the plane pic is positive
			o.fillEdge = pic;
			
			// collapsing the above points on the water plane
			if (pic >= 0)
			{
				float4 newvertex;
				newvertex = float4(vertex_world-(pic*wn),1); // world coordinates

				newvertex = mul (unity_WorldToObject, newvertex); // object coordinates

				if (is_inside(newvertex) == false){
					newvertex = float4(newvertex.xyz/sqrt(get_ellipsoid_value(newvertex)),1);
					o.fillEdge = -100;
				}
				else
				{
					o.fillEdge = 0; 
                }
				o.vertex = UnityObjectToClipPos(newvertex);
			}

			return o;
        }

        fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
        {

			//float result = step(i.fillEdge, 0);

			if (facing>0 || true){
				return i.fillEdge == -100? float4(3, 0, 0, 1) :i.fillEdge < 0 ?  _Color: i.fillEdge < 0.05? _TopColor : float4(0, 0, 0, 0);  
				return i.fillEdge < 0 ?  _Color: float4(0, 0, 0, 0);  

			} 
			return i.fillEdge < 0 ?  _TopColor: float4(0, 0, 0, 0);
        }
        ENDCG
        }
    }
}

