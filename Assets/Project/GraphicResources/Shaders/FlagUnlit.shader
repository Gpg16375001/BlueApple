// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:35076,y:31929,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:34204,y:32011,varname:_MainTex_copy,prsc:2,tex:70a0601c59b745d4288eb39ee069ab21,ntxv:0,isnm:False|UVIN-2948-OUT,TEX-3269-TEX;n:type:ShaderForge.SFN_Multiply,id:1086,x:34736,y:32011,cmnt:RGB,varname:node_1086,prsc:2|A-2354-OUT,B-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:34019,y:32215,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:0.5;n:type:ShaderForge.SFN_VertexColor,id:5376,x:34556,y:31890,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:34909,y:32011,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:34736,y:32215,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5522-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:3269,x:34019,y:32011,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:70a0601c59b745d4288eb39ee069ab21,ntxv:2,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:5079,x:33272,y:31871,varname:node_5079,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:7648,x:33613,y:31818,ptovrint:False,ptlb:NoiseTex,ptin:_NoiseTex,varname:_NoiseTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Time,id:6979,x:33072,y:31682,varname:node_6979,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:5764,x:33787,y:31578,varname:node_5764,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7500-OUT,TEX-7648-TEX;n:type:ShaderForge.SFN_Append,id:7500,x:33613,y:31578,varname:node_7500,prsc:2|A-6588-OUT,B-6376-OUT;n:type:ShaderForge.SFN_Add,id:6588,x:33447,y:31578,varname:node_6588,prsc:2|A-6707-OUT,B-5079-U;n:type:ShaderForge.SFN_Add,id:6376,x:33447,y:31739,varname:node_6376,prsc:2|A-1974-OUT,B-5079-V;n:type:ShaderForge.SFN_Add,id:5714,x:33443,y:32388,varname:node_5714,prsc:2|A-8360-OUT,B-4035-OUT;n:type:ShaderForge.SFN_Multiply,id:9828,x:33270,y:32226,varname:node_9828,prsc:2|A-9424-OUT,B-7360-OUT,C-1743-OUT;n:type:ShaderForge.SFN_Slider,id:9424,x:32935,y:32226,ptovrint:False,ptlb:NoisePower U,ptin:_NoisePowerU,varname:_NoisePowerU,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.05,max:1;n:type:ShaderForge.SFN_Multiply,id:6707,x:33272,y:31578,varname:node_6707,prsc:2|A-6979-T,B-8105-OUT;n:type:ShaderForge.SFN_Slider,id:8105,x:32928,y:31574,ptovrint:False,ptlb:NoiseSpeed U,ptin:_NoiseSpeedU,varname:_NoiseSpeedU,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.1,max:1;n:type:ShaderForge.SFN_Slider,id:6496,x:32915,y:31847,ptovrint:False,ptlb:NoiseSpeed V,ptin:_NoiseSpeedV,varname:_NoiseSpeedV,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.1,max:1;n:type:ShaderForge.SFN_Multiply,id:1974,x:33272,y:31739,varname:node_1974,prsc:2|A-6979-T,B-6496-OUT;n:type:ShaderForge.SFN_Add,id:8543,x:33443,y:32226,varname:node_8543,prsc:2|A-9828-OUT,B-9328-OUT;n:type:ShaderForge.SFN_Append,id:2127,x:33610,y:32226,varname:node_2127,prsc:2|A-8543-OUT,B-5714-OUT;n:type:ShaderForge.SFN_Slider,id:3995,x:32929,y:32388,ptovrint:False,ptlb:NoisePower V,ptin:_NoisePowerV,varname:_NoisePowerV,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.05,max:1;n:type:ShaderForge.SFN_Multiply,id:8360,x:33270,y:32388,varname:node_8360,prsc:2|A-3995-OUT,B-7360-OUT,C-1743-OUT;n:type:ShaderForge.SFN_Subtract,id:7061,x:34204,y:32215,varname:node_7061,prsc:2|A-5983-RGB,B-2183-OUT;n:type:ShaderForge.SFN_Vector1,id:2183,x:34019,y:32407,varname:node_2183,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:5817,x:34381,y:32215,varname:node_5817,prsc:2|A-7061-OUT,B-8389-OUT;n:type:ShaderForge.SFN_Vector1,id:8389,x:34204,y:32407,varname:node_8389,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:2354,x:34556,y:32011,varname:node_2354,prsc:2|A-5442-OUT,B-5817-OUT,C-7815-OUT;n:type:ShaderForge.SFN_Multiply,id:5522,x:34381,y:32407,varname:node_5522,prsc:2|A-8389-OUT,B-5983-A;n:type:ShaderForge.SFN_Multiply,id:6817,x:33270,y:32616,varname:node_6817,prsc:2|A-4032-OUT,B-8981-OUT;n:type:ShaderForge.SFN_Set,id:1215,x:33950,y:32616,varname:Light,prsc:2|IN-857-OUT;n:type:ShaderForge.SFN_Get,id:7815,x:34360,y:31890,varname:node_7815,prsc:2|IN-1215-OUT;n:type:ShaderForge.SFN_Set,id:4383,x:33765,y:32226,varname:uvDistort,prsc:2|IN-2127-OUT;n:type:ShaderForge.SFN_Get,id:2948,x:33998,y:31890,varname:node_2948,prsc:2|IN-4383-OUT;n:type:ShaderForge.SFN_Set,id:7217,x:34715,y:31890,varname:VertexAlpha,prsc:2|IN-5376-A;n:type:ShaderForge.SFN_Get,id:1743,x:33068,y:32519,varname:node_1743,prsc:2|IN-7217-OUT;n:type:ShaderForge.SFN_Set,id:9109,x:33447,y:31871,varname:U,prsc:2|IN-5079-U;n:type:ShaderForge.SFN_Set,id:1340,x:33447,y:31966,varname:V,prsc:2|IN-5079-V;n:type:ShaderForge.SFN_Get,id:9328,x:33249,y:32165,varname:node_9328,prsc:2|IN-9109-OUT;n:type:ShaderForge.SFN_Get,id:4035,x:33249,y:32519,varname:node_4035,prsc:2|IN-1340-OUT;n:type:ShaderForge.SFN_Color,id:7931,x:33610,y:32388,ptovrint:False,ptlb:LightColor,ptin:_LightColor,varname:_LightColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:4032,x:33089,y:32616,varname:node_4032,prsc:2,v1:2;n:type:ShaderForge.SFN_Subtract,id:4400,x:33443,y:32616,varname:node_4400,prsc:2|A-6817-OUT,B-9787-OUT;n:type:ShaderForge.SFN_Vector1,id:9787,x:33270,y:32736,varname:node_9787,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:8644,x:33610,y:32616,varname:node_8644,prsc:2|IN-4400-OUT;n:type:ShaderForge.SFN_Multiply,id:857,x:33786,y:32616,varname:node_857,prsc:2|A-8644-OUT,B-7931-RGB,C-5148-OUT;n:type:ShaderForge.SFN_Get,id:5148,x:33589,y:32736,varname:node_5148,prsc:2|IN-7217-OUT;n:type:ShaderForge.SFN_Get,id:8981,x:33068,y:32736,varname:node_8981,prsc:2|IN-5153-OUT;n:type:ShaderForge.SFN_Set,id:5153,x:33961,y:31663,varname:uvNoise,prsc:2|IN-5764-R;n:type:ShaderForge.SFN_Get,id:7360,x:33071,y:32103,varname:node_7360,prsc:2|IN-5153-OUT;n:type:ShaderForge.SFN_Color,id:5443,x:33270,y:32805,ptovrint:False,ptlb:ShadowColor,ptin:_ShadowColor,varname:_ShadowColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:1313,x:33443,y:32805,varname:node_1313,prsc:2|A-5443-RGB,B-6817-OUT,C-3226-OUT;n:type:ShaderForge.SFN_Clamp01,id:6874,x:33610,y:32805,varname:node_6874,prsc:2|IN-1313-OUT;n:type:ShaderForge.SFN_Set,id:3060,x:33765,y:32805,varname:Shadow,prsc:2|IN-6874-OUT;n:type:ShaderForge.SFN_Get,id:2922,x:34183,y:31890,varname:node_2922,prsc:2|IN-3060-OUT;n:type:ShaderForge.SFN_Get,id:9129,x:33068,y:32999,varname:node_9129,prsc:2|IN-7217-OUT;n:type:ShaderForge.SFN_OneMinus,id:3226,x:33270,y:32999,varname:node_3226,prsc:2|IN-9129-OUT;n:type:ShaderForge.SFN_Multiply,id:5442,x:34381,y:32011,varname:node_5442,prsc:2|A-4805-RGB,B-2922-OUT;proporder:5983-7931-5443-3269-7648-9424-8105-3995-6496;pass:END;sub:END;*/

Shader "Effects/FlagUnlit" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,0.5)
        _LightColor ("LightColor", Color) = (0.5,0.5,0.5,1)
        _ShadowColor ("ShadowColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _NoiseTex ("NoiseTex", 2D) = "black" {}
        _NoisePowerU ("NoisePower U", Range(0, 1)) = 0.05
        _NoiseSpeedU ("NoiseSpeed U", Range(-1, 1)) = 0.1
        _NoisePowerV ("NoisePower V", Range(0, 1)) = 0.05
        _NoiseSpeedV ("NoiseSpeed V", Range(-1, 1)) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
            uniform float _NoisePowerU;
            uniform float _NoiseSpeedU;
            uniform float _NoiseSpeedV;
            uniform float _NoisePowerV;
            uniform float4 _LightColor;
            uniform float4 _ShadowColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_6979 = _Time;
                float2 node_7500 = float2(((node_6979.g*_NoiseSpeedU)+i.uv0.r),((node_6979.g*_NoiseSpeedV)+i.uv0.g));
                float4 node_5764 = tex2D(_NoiseTex,TRANSFORM_TEX(node_7500, _NoiseTex));
                float uvNoise = node_5764.r;
                float node_7360 = uvNoise;
                float VertexAlpha = i.vertexColor.a;
                float node_1743 = VertexAlpha;
                float U = i.uv0.r;
                float V = i.uv0.g;
                float2 uvDistort = float2(((_NoisePowerU*node_7360*node_1743)+U),((_NoisePowerV*node_7360*node_1743)+V));
                float2 node_2948 = uvDistort;
                float4 _MainTex_copy = tex2D(_MainTex,TRANSFORM_TEX(node_2948, _MainTex));
                float node_4032 = 2.0;
                float node_6817 = (node_4032*uvNoise);
                float3 Shadow = saturate((_ShadowColor.rgb+node_6817+(1.0 - VertexAlpha)));
                float node_8389 = 2.0;
                float3 Light = (saturate((node_6817-1.0))*_LightColor.rgb*VertexAlpha);
                float3 node_1086 = (((_MainTex_copy.rgb*Shadow)+((_Color.rgb-0.5)*node_8389)+Light)*i.vertexColor.rgb); // RGB
                float node_603 = (_MainTex_copy.a*(node_8389*_Color.a)); // A
                float3 emissive = (node_1086*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
