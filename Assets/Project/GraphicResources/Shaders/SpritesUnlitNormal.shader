// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33635,y:32682,varname:node_1873,prsc:2|emission-4113-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32551,y:32612,varname:_MainTex_copy,prsc:2,tex:fd9fde9ce6505d546952963a514b1909,ntxv:0,isnm:False|UVIN-7888-OUT,TEX-9154-TEX;n:type:ShaderForge.SFN_Color,id:5983,x:32036,y:33140,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32036,y:32852,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33088,y:32778,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-5001-OUT,B-4448-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:33088,y:32944,cmnt:A,varname:node_603,prsc:2|A-4284-OUT,B-958-OUT,C-5139-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9154,x:32375,y:32691,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9154,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fd9fde9ce6505d546952963a514b1909,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:1691,x:32551,y:33441,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_1691,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e2e48048625d63c41a7219b81562326d,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Dot,id:9717,x:32906,y:33441,varname:node_9717,prsc:2,dt:0|A-2790-XYZ,B-7447-RGB;n:type:ShaderForge.SFN_Tex2d,id:7447,x:32721,y:33597,varname:node_7447,prsc:2,tex:e2e48048625d63c41a7219b81562326d,ntxv:0,isnm:False|UVIN-3977-OUT,TEX-1691-TEX;n:type:ShaderForge.SFN_Vector4Property,id:2790,x:32721,y:33441,ptovrint:False,ptlb:LightVector,ptin:_LightVector,varname:node_2790,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:0;n:type:ShaderForge.SFN_TexCoord,id:7109,x:32189,y:32481,varname:node_7109,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Set,id:9573,x:32551,y:32852,varname:VertexRGB,prsc:2|IN-9103-OUT;n:type:ShaderForge.SFN_Set,id:192,x:32723,y:32984,varname:VertexA,prsc:2|IN-5724-OUT;n:type:ShaderForge.SFN_Get,id:5139,x:32906,y:33025,varname:node_5139,prsc:2|IN-192-OUT;n:type:ShaderForge.SFN_Set,id:4639,x:32375,y:32481,varname:UVC,prsc:2|IN-7109-UVOUT;n:type:ShaderForge.SFN_Get,id:3977,x:32551,y:33597,varname:node_3977,prsc:2|IN-4639-OUT;n:type:ShaderForge.SFN_Get,id:7888,x:32375,y:32612,varname:node_7888,prsc:2|IN-4639-OUT;n:type:ShaderForge.SFN_Set,id:1644,x:33780,y:33140,varname:Light,prsc:2|IN-2281-OUT;n:type:ShaderForge.SFN_Set,id:6616,x:32723,y:32612,varname:TexRGB,prsc:2|IN-4805-RGB;n:type:ShaderForge.SFN_Set,id:220,x:32723,y:32648,varname:TexA,prsc:2|IN-4805-A;n:type:ShaderForge.SFN_Get,id:5001,x:32906,y:32778,varname:node_5001,prsc:2|IN-6616-OUT;n:type:ShaderForge.SFN_Get,id:4284,x:32906,y:32944,varname:node_4284,prsc:2|IN-220-OUT;n:type:ShaderForge.SFN_Subtract,id:5280,x:32209,y:32852,varname:node_5280,prsc:2|A-5376-RGB,B-605-OUT;n:type:ShaderForge.SFN_Vector1,id:605,x:32036,y:32984,varname:node_605,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:6466,x:32209,y:32984,varname:node_6466,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:9103,x:32375,y:32852,varname:node_9103,prsc:2|A-5280-OUT,B-6466-OUT;n:type:ShaderForge.SFN_Multiply,id:4348,x:32375,y:32984,varname:node_4348,prsc:2|A-6466-OUT,B-5376-A;n:type:ShaderForge.SFN_Set,id:6792,x:32721,y:33283,varname:TintA,prsc:2|IN-7988-OUT;n:type:ShaderForge.SFN_Set,id:808,x:32551,y:33140,varname:TintRGB,prsc:2|IN-5256-OUT;n:type:ShaderForge.SFN_Get,id:958,x:32906,y:32980,varname:node_958,prsc:2|IN-6792-OUT;n:type:ShaderForge.SFN_Add,id:4732,x:33269,y:32778,varname:node_4732,prsc:2|A-1749-OUT,B-3042-OUT,C-8907-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4435,x:33088,y:33140,varname:node_4435,prsc:2,min:0.5,max:1|IN-9717-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:2808,x:33088,y:33441,varname:node_2808,prsc:2,min:0,max:0.5|IN-9717-OUT;n:type:ShaderForge.SFN_Color,id:2177,x:33440,y:33283,ptovrint:False,ptlb:AmbientColor,ptin:_AmbientColor,varname:node_2177,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Set,id:6491,x:33780,y:33283,varname:Ambient,prsc:2|IN-5384-OUT;n:type:ShaderForge.SFN_Vector1,id:8341,x:33270,y:33082,varname:node_8341,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:2925,x:33088,y:33283,varname:node_2925,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:3462,x:33269,y:33140,varname:node_3462,prsc:2|A-4435-OUT,B-2925-OUT;n:type:ShaderForge.SFN_Multiply,id:2281,x:33437,y:33140,varname:node_2281,prsc:2|A-3462-OUT,B-6665-RGB,C-8341-OUT;n:type:ShaderForge.SFN_Get,id:8907,x:33067,y:32894,varname:node_8907,prsc:2|IN-1644-OUT;n:type:ShaderForge.SFN_Multiply,id:9195,x:33270,y:33441,varname:node_9195,prsc:2|A-2808-OUT,B-9629-OUT;n:type:ShaderForge.SFN_Vector1,id:9629,x:33088,y:33597,varname:node_9629,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:4476,x:33440,y:33441,varname:node_4476,prsc:2|IN-9195-OUT;n:type:ShaderForge.SFN_Get,id:4448,x:32906,y:32814,varname:node_4448,prsc:2|IN-6491-OUT;n:type:ShaderForge.SFN_Lerp,id:5384,x:33616,y:33283,varname:node_5384,prsc:2|A-2177-RGB,B-5293-OUT,T-4476-OUT;n:type:ShaderForge.SFN_Vector1,id:5293,x:33440,y:33590,varname:node_5293,prsc:2,v1:1;n:type:ShaderForge.SFN_Color,id:6665,x:33270,y:33283,ptovrint:False,ptlb:LightColor,ptin:_LightColor,varname:node_6665,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Get,id:3042,x:33067,y:32707,varname:node_3042,prsc:2|IN-9573-OUT;n:type:ShaderForge.SFN_Clamp01,id:5724,x:32551,y:32984,varname:node_5724,prsc:2|IN-4348-OUT;n:type:ShaderForge.SFN_Subtract,id:7834,x:32209,y:33140,varname:node_7834,prsc:2|A-5983-RGB,B-605-OUT;n:type:ShaderForge.SFN_Multiply,id:5256,x:32375,y:33140,varname:node_5256,prsc:2|A-7834-OUT,B-6466-OUT;n:type:ShaderForge.SFN_Multiply,id:2786,x:32375,y:33283,varname:node_2786,prsc:2|A-5983-A,B-6466-OUT;n:type:ShaderForge.SFN_Clamp01,id:7988,x:32551,y:33283,varname:node_7988,prsc:2|IN-2786-OUT;n:type:ShaderForge.SFN_Multiply,id:4113,x:33437,y:32778,varname:node_4113,prsc:2|A-4732-OUT,B-603-OUT;proporder:5983-9154-1691-2790-2177-6665;pass:END;sub:END;*/

Shader "Sprites/Unlit Normal" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "gray" {}
        _Normal ("Normal", 2D) = "bump" {}
        _LightVector ("LightVector", Vector) = (0.5,0.5,0.5,0)
        _AmbientColor ("AmbientColor", Color) = (0,0,0,1)
        _LightColor ("LightColor", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
            "PreviewType"="Plane"
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float4 _LightVector;
            uniform float4 _AmbientColor;
            uniform float4 _LightColor;
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
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float2 UVC = i.uv0;
                float2 node_7888 = UVC;
                float4 _MainTex_copy = tex2D(_MainTex,TRANSFORM_TEX(node_7888, _MainTex));
                float3 TexRGB = _MainTex_copy.rgb;
                float node_5293 = 1.0;
                float2 node_3977 = UVC;
                float3 node_7447 = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_3977, _Normal)));
                float node_9717 = dot(_LightVector.rgb,node_7447.rgb);
                float3 Ambient = lerp(_AmbientColor.rgb,float3(node_5293,node_5293,node_5293),saturate((clamp(node_9717,0,0.5)*2.0)));
                float node_605 = 0.5;
                float node_6466 = 2.0;
                float3 VertexRGB = ((i.vertexColor.rgb-node_605)*node_6466);
                float3 Light = ((clamp(node_9717,0.5,1)-0.5)*_LightColor.rgb*2.0);
                float TexA = _MainTex_copy.a;
                float TintA = saturate((_Color.a*node_6466));
                float VertexA = saturate((node_6466*i.vertexColor.a));
                float node_603 = (TexA*TintA*VertexA); // A
                float3 emissive = (((TexRGB*Ambient)+VertexRGB+Light)*node_603);
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
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
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
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
