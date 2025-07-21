Shader "Custom/HoleInBody Mobile"
{
  Properties
  {
    _MainTex ("Base (ARGB)", 2D) = "white" {}
    _WidthMainTex ("Main Width", float) = 0
    _HeightMainTex ("Main Height", float) = 0
    [NoScaleOffset] _BloodTex ("Blood (ARGB)", 2D) = "white" {}
    _WidthBloodTex ("Blood Width", float) = 1
    _HeightBloodTex ("Blood Height", float) = 1
    [NoScaleOffset] _HoleTex ("Hole (ARGB)", 2D) = "white" {}
    _WidthHoleTex ("Hole Width", float) = 1
    _HeightHoleTex ("Hole Height", float) = 1
    PositionHole1 ("Position Hole1", Vector) = (0,0,0,1)
    PositionHole2 ("Position Hole2", Vector) = (0,0,0,1)
    PositionHole3 ("Position Hole3", Vector) = (0,0,0,1)
    PositionHole4 ("Position Hole4", Vector) = (0,0,0,1)
    PositionHole5 ("Position Hole5", Vector) = (0,0,0,1)
    CurrentHole ("CurrentHole", float) = 1
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 100
      ZWrite Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform sampler2D _MainTex;
      uniform sampler2D _HoleTex;
      uniform sampler2D _BloodTex;
      uniform float _WidthMainTex;
      uniform float _HeightMainTex;
      uniform float _WidthBloodTex;
      uniform float _HeightBloodTex;
      uniform float _WidthHoleTex;
      uniform float _HeightHoleTex;
      uniform float4 PositionHole1;
      uniform float4 PositionHole2;
      uniform float4 PositionHole3;
      uniform float4 PositionHole4;
      uniform float4 PositionHole5;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2.w = 1;
          tmpvar_2.xyz = in_v.vertex.xyz;
          tmpvar_1 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_2));
          out_v.xlv_COLOR = in_v.color;
          out_v.xlv_TEXCOORD0 = tmpvar_1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float xlat_mutablealphaDecal;
      float xlat_mutablex;
      float xlat_mutabley;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float tmpvar_1;
          tmpvar_1 = max(4, _WidthMainTex);
          float tmpvar_2;
          tmpvar_2 = max(4, _HeightMainTex);
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float4 colorTex_4;
          colorTex_4.xyz = tmpvar_3.xyz;
          float2 coordOfColor_5;
          xlat_mutablex = (_WidthHoleTex / tmpvar_1);
          xlat_mutabley = (_HeightHoleTex / tmpvar_2);
          coordOfColor_5.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole1.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_5.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole1.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          float tmpvar_6;
          tmpvar_6 = abs(PositionHole1.x);
          float tmpvar_7;
          tmpvar_7 = abs(PositionHole1.y);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_6 + tmpvar_7)), 0, 1) * clamp((coordOfColor_5.x * 100), 0, 1)) * clamp(((1 - coordOfColor_5.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_5.y * 100), 0, 1)) * clamp(((1 - coordOfColor_5.y) * 100), 0, 1));
          colorTex_4.w = (floor(((tmpvar_3.w + (((tmpvar_3.w * tex2D(_HoleTex, coordOfColor_5).x) - tmpvar_3.w) * xlat_mutablealphaDecal)) * 100)) / 100);
          float4 colorTex_8;
          colorTex_8.xyz = colorTex_4.xyz;
          float2 coordOfColor_9;
          xlat_mutablex = (_WidthHoleTex / tmpvar_1);
          xlat_mutabley = (_HeightHoleTex / tmpvar_2);
          coordOfColor_9.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole2.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_9.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole2.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          float tmpvar_10;
          tmpvar_10 = abs(PositionHole2.x);
          float tmpvar_11;
          tmpvar_11 = abs(PositionHole2.y);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_10 + tmpvar_11)), 0, 1) * clamp((coordOfColor_9.x * 100), 0, 1)) * clamp(((1 - coordOfColor_9.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_9.y * 100), 0, 1)) * clamp(((1 - coordOfColor_9.y) * 100), 0, 1));
          colorTex_8.w = (floor(((colorTex_4.w + (((colorTex_4.w * tex2D(_HoleTex, coordOfColor_9).x) - colorTex_4.w) * xlat_mutablealphaDecal)) * 100)) / 100);
          float4 colorTex_12;
          colorTex_12.xyz = colorTex_8.xyz;
          float2 coordOfColor_13;
          xlat_mutablex = (_WidthHoleTex / tmpvar_1);
          xlat_mutabley = (_HeightHoleTex / tmpvar_2);
          coordOfColor_13.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole3.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_13.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole3.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          float tmpvar_14;
          tmpvar_14 = abs(PositionHole3.x);
          float tmpvar_15;
          tmpvar_15 = abs(PositionHole3.y);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_14 + tmpvar_15)), 0, 1) * clamp((coordOfColor_13.x * 100), 0, 1)) * clamp(((1 - coordOfColor_13.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_13.y * 100), 0, 1)) * clamp(((1 - coordOfColor_13.y) * 100), 0, 1));
          colorTex_12.w = (floor(((colorTex_8.w + (((colorTex_8.w * tex2D(_HoleTex, coordOfColor_13).x) - colorTex_8.w) * xlat_mutablealphaDecal)) * 100)) / 100);
          float4 colorTex_16;
          colorTex_16.xyz = colorTex_12.xyz;
          float2 coordOfColor_17;
          xlat_mutablex = (_WidthHoleTex / tmpvar_1);
          xlat_mutabley = (_HeightHoleTex / tmpvar_2);
          coordOfColor_17.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole4.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_17.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole4.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          float tmpvar_18;
          tmpvar_18 = abs(PositionHole4.x);
          float tmpvar_19;
          tmpvar_19 = abs(PositionHole4.y);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_18 + tmpvar_19)), 0, 1) * clamp((coordOfColor_17.x * 100), 0, 1)) * clamp(((1 - coordOfColor_17.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_17.y * 100), 0, 1)) * clamp(((1 - coordOfColor_17.y) * 100), 0, 1));
          colorTex_16.w = (floor(((colorTex_12.w + (((colorTex_12.w * tex2D(_HoleTex, coordOfColor_17).x) - colorTex_12.w) * xlat_mutablealphaDecal)) * 100)) / 100);
          float4 colorTex_20;
          colorTex_20.xyz = colorTex_16.xyz;
          float2 coordOfColor_21;
          xlat_mutablex = (_WidthHoleTex / tmpvar_1);
          xlat_mutabley = (_HeightHoleTex / tmpvar_2);
          coordOfColor_21.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole5.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_21.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole5.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          float tmpvar_22;
          tmpvar_22 = abs(PositionHole5.x);
          float tmpvar_23;
          tmpvar_23 = abs(PositionHole5.y);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_22 + tmpvar_23)), 0, 1) * clamp((coordOfColor_21.x * 100), 0, 1)) * clamp(((1 - coordOfColor_21.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_21.y * 100), 0, 1)) * clamp(((1 - coordOfColor_21.y) * 100), 0, 1));
          colorTex_20.w = (floor(((colorTex_16.w + (((colorTex_16.w * tex2D(_HoleTex, coordOfColor_21).x) - colorTex_16.w) * xlat_mutablealphaDecal)) * 100)) / 100);
          float4 colorTex_24;
          colorTex_24.w = colorTex_20.w;
          float2 coordOfColor_25;
          xlat_mutablex = (_WidthBloodTex / tmpvar_1);
          xlat_mutabley = (_HeightBloodTex / tmpvar_2);
          coordOfColor_25.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole1.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_25.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole1.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_6 + tmpvar_7)), 0, 1) * clamp((coordOfColor_25.x * 100), 0, 1)) * clamp(((1 - coordOfColor_25.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_25.y * 100), 0, 1)) * clamp(((1 - coordOfColor_25.y) * 100), 0, 1));
          float4 tmpvar_26;
          tmpvar_26 = tex2D(_BloodTex, coordOfColor_25);
          colorTex_24.x = (floor(((tmpvar_3.x + ((tmpvar_26.x - tmpvar_3.x) * (tmpvar_26.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_24.y = (floor(((tmpvar_3.y + ((tmpvar_26.y - tmpvar_3.y) * (tmpvar_26.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_24.z = (floor(((tmpvar_3.z + ((tmpvar_26.z - tmpvar_3.z) * (tmpvar_26.w * xlat_mutablealphaDecal))) * 100)) / 100);
          float4 colorTex_27;
          colorTex_27.w = colorTex_24.w;
          float2 coordOfColor_28;
          xlat_mutablex = (_WidthBloodTex / tmpvar_1);
          xlat_mutabley = (_HeightBloodTex / tmpvar_2);
          coordOfColor_28.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole2.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_28.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole2.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_10 + tmpvar_11)), 0, 1) * clamp((coordOfColor_28.x * 100), 0, 1)) * clamp(((1 - coordOfColor_28.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_28.y * 100), 0, 1)) * clamp(((1 - coordOfColor_28.y) * 100), 0, 1));
          float4 tmpvar_29;
          tmpvar_29 = tex2D(_BloodTex, coordOfColor_28);
          colorTex_27.x = (floor(((colorTex_24.x + ((tmpvar_29.x - colorTex_24.x) * (tmpvar_29.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_27.y = (floor(((colorTex_24.y + ((tmpvar_29.y - colorTex_24.y) * (tmpvar_29.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_27.z = (floor(((colorTex_24.z + ((tmpvar_29.z - colorTex_24.z) * (tmpvar_29.w * xlat_mutablealphaDecal))) * 100)) / 100);
          float4 colorTex_30;
          colorTex_30.w = colorTex_27.w;
          float2 coordOfColor_31;
          xlat_mutablex = (_WidthBloodTex / tmpvar_1);
          xlat_mutabley = (_HeightBloodTex / tmpvar_2);
          coordOfColor_31.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole3.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_31.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole3.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_14 + tmpvar_15)), 0, 1) * clamp((coordOfColor_31.x * 100), 0, 1)) * clamp(((1 - coordOfColor_31.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_31.y * 100), 0, 1)) * clamp(((1 - coordOfColor_31.y) * 100), 0, 1));
          float4 tmpvar_32;
          tmpvar_32 = tex2D(_BloodTex, coordOfColor_31);
          colorTex_30.x = (floor(((colorTex_27.x + ((tmpvar_32.x - colorTex_27.x) * (tmpvar_32.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_30.y = (floor(((colorTex_27.y + ((tmpvar_32.y - colorTex_27.y) * (tmpvar_32.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_30.z = (floor(((colorTex_27.z + ((tmpvar_32.z - colorTex_27.z) * (tmpvar_32.w * xlat_mutablealphaDecal))) * 100)) / 100);
          float4 colorTex_33;
          colorTex_33.w = colorTex_30.w;
          float2 coordOfColor_34;
          xlat_mutablex = (_WidthBloodTex / tmpvar_1);
          xlat_mutabley = (_HeightBloodTex / tmpvar_2);
          coordOfColor_34.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole4.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_34.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole4.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_18 + tmpvar_19)), 0, 1) * clamp((coordOfColor_34.x * 100), 0, 1)) * clamp(((1 - coordOfColor_34.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_34.y * 100), 0, 1)) * clamp(((1 - coordOfColor_34.y) * 100), 0, 1));
          float4 tmpvar_35;
          tmpvar_35 = tex2D(_BloodTex, coordOfColor_34);
          colorTex_33.x = (floor(((colorTex_30.x + ((tmpvar_35.x - colorTex_30.x) * (tmpvar_35.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_33.y = (floor(((colorTex_30.y + ((tmpvar_35.y - colorTex_30.y) * (tmpvar_35.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_33.z = (floor(((colorTex_30.z + ((tmpvar_35.z - colorTex_30.z) * (tmpvar_35.w * xlat_mutablealphaDecal))) * 100)) / 100);
          float4 colorTex_36;
          colorTex_36.w = colorTex_33.w;
          float2 coordOfColor_37;
          xlat_mutablex = (_WidthBloodTex / tmpvar_1);
          xlat_mutabley = (_HeightBloodTex / tmpvar_2);
          coordOfColor_37.x = ((in_f.xlv_TEXCOORD0.x - (PositionHole5.x - (xlat_mutablex * 0.5))) / xlat_mutablex);
          coordOfColor_37.y = ((in_f.xlv_TEXCOORD0.y - (PositionHole5.y - (xlat_mutabley * 0.5))) / xlat_mutabley);
          xlat_mutablealphaDecal = ((clamp((100 * (tmpvar_22 + tmpvar_23)), 0, 1) * clamp((coordOfColor_37.x * 100), 0, 1)) * clamp(((1 - coordOfColor_37.x) * 100), 0, 1));
          xlat_mutablealphaDecal = ((xlat_mutablealphaDecal * clamp((coordOfColor_37.y * 100), 0, 1)) * clamp(((1 - coordOfColor_37.y) * 100), 0, 1));
          float4 tmpvar_38;
          tmpvar_38 = tex2D(_BloodTex, coordOfColor_37);
          colorTex_36.x = (floor(((colorTex_33.x + ((tmpvar_38.x - colorTex_33.x) * (tmpvar_38.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_36.y = (floor(((colorTex_33.y + ((tmpvar_38.y - colorTex_33.y) * (tmpvar_38.w * xlat_mutablealphaDecal))) * 100)) / 100);
          colorTex_36.z = (floor(((colorTex_33.z + ((tmpvar_38.z - colorTex_33.z) * (tmpvar_38.w * xlat_mutablealphaDecal))) * 100)) / 100);
          float4 tmpvar_39;
          tmpvar_39 = (colorTex_36 * in_f.xlv_COLOR);
          out_f.color = tmpvar_39;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
