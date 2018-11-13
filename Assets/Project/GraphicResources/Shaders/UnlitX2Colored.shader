// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33665,y:32497,varname:node_3138,prsc:2|emission-6577-OUT,alpha-5053-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32471,y:32402,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:5334,x:32272,y:32594,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6715,x:32471,y:32594,varname:_node_6715,prsc:2,ntxv:0,isnm:False|UVIN-5941-UVOUT,MIP-4287-OUT,TEX-5334-TEX;n:type:ShaderForge.SFN_Vector1,id:4287,x:32272,y:32331,varname:node_4287,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:5941,x:32272,y:32402,varname:node_5941,prsc:2,uv:0;n:type:ShaderForge.SFN_Subtract,id:9722,x:32641,y:32402,varname:node_9722,prsc:2|A-7241-RGB,B-7670-OUT;n:type:ShaderForge.SFN_Vector1,id:7670,x:32471,y:32331,varname:node_7670,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:9709,x:32809,y:32402,varname:node_9709,prsc:2|A-9722-OUT,B-5734-OUT;n:type:ShaderForge.SFN_Vector1,id:5734,x:32641,y:32331,varname:node_5734,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:7635,x:33322,y:32597,varname:node_7635,prsc:2|A-6715-RGB,B-9339-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:9339,x:32980,y:32402,varname:node_9339,prsc:2,min:-1,max:1|IN-9709-OUT;n:type:ShaderForge.SFN_Clamp01,id:6577,x:33490,y:32597,varname:node_6577,prsc:2|IN-7635-OUT;n:type:ShaderForge.SFN_Multiply,id:897,x:32641,y:32775,varname:node_897,prsc:2|A-6715-A,B-7241-A;n:type:ShaderForge.SFN_Vector1,id:8543,x:32641,y:32949,varname:node_8543,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:237,x:32809,y:32775,varname:node_237,prsc:2|A-897-OUT,B-8543-OUT;n:type:ShaderForge.SFN_Clamp01,id:5053,x:32980,y:32775,varname:node_5053,prsc:2|IN-237-OUT;proporder:7241-5334;pass:END;sub:END;*/

Shader "Unlit/X2 Colored" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("MainTex", 2D) = "gray" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _node_6715 = tex2Dlod(_MainTex,float4(i.uv0,0.0,0.0));
                float3 node_7635 = (_node_6715.rgb+clamp(((_Color.rgb-0.5)*2.0),-1,1));
                float3 emissive = saturate(node_7635);
                float3 finalColor = emissive;
                float node_5053 = saturate(((_node_6715.a*_Color.a)*2.0));
                return fixed4(finalColor,node_5053);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
