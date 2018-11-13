// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33348,y:32422,varname:node_3138,prsc:2|emission-4727-OUT,clip-3536-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31944,y:32337,ptovrint:False,ptlb:Tint Color,ptin:_TintColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:262,x:31591,y:32523,varname:node_262,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:8558,x:32291,y:32523,varname:node_8558,prsc:2,tex:9f64a0b1148e82c4cbe4daddc9a6e9ef,ntxv:0,isnm:False|UVIN-3931-OUT,TEX-4860-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:4860,x:31944,y:32523,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4860,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9f64a0b1148e82c4cbe4daddc9a6e9ef,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:5973,x:31598,y:32841,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:777,x:31971,y:32719,ptovrint:False,ptlb:Rim Power,ptin:_RimPower,varname:node_777,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:16;n:type:ShaderForge.SFN_Add,id:7222,x:32819,y:32523,varname:node_7222,prsc:2|A-3226-OUT,B-9372-OUT,C-4844-OUT;n:type:ShaderForge.SFN_VertexColor,id:766,x:31588,y:32699,varname:node_766,prsc:2;n:type:ShaderForge.SFN_Color,id:4164,x:32291,y:32682,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:node_4164,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:8037,x:32474,y:32841,varname:node_8037,prsc:2|A-3674-OUT,B-4164-RGB;n:type:ShaderForge.SFN_Clamp01,id:9372,x:32642,y:32841,varname:node_9372,prsc:2|IN-8037-OUT;n:type:ShaderForge.SFN_Vector1,id:6420,x:32119,y:32266,varname:node_6420,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:2569,x:32291,y:32337,varname:node_2569,prsc:2|A-2896-OUT,B-6420-OUT;n:type:ShaderForge.SFN_Subtract,id:2896,x:32119,y:32337,varname:node_2896,prsc:2|A-7241-RGB,B-4771-OUT;n:type:ShaderForge.SFN_Vector1,id:4771,x:31944,y:32266,varname:node_4771,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:5887,x:32474,y:32523,varname:node_5887,prsc:2|A-8558-RGB,B-2569-OUT;n:type:ShaderForge.SFN_Clamp01,id:4727,x:33164,y:32523,varname:node_4727,prsc:2|IN-798-OUT;n:type:ShaderForge.SFN_Clamp01,id:3226,x:32642,y:32523,varname:node_3226,prsc:2|IN-5887-OUT;n:type:ShaderForge.SFN_Multiply,id:798,x:32993,y:32523,varname:node_798,prsc:2|A-7222-OUT,B-6839-OUT;n:type:ShaderForge.SFN_Dot,id:1733,x:31944,y:32984,varname:node_1733,prsc:2,dt:3|A-2734-OUT,B-6591-OUT;n:type:ShaderForge.SFN_ViewVector,id:2734,x:31773,y:32984,varname:node_2734,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6071,x:32128,y:32984,varname:node_6071,prsc:2|A-1733-OUT,B-1733-OUT,C-805-RGB;n:type:ShaderForge.SFN_Step,id:3536,x:33164,y:32699,varname:node_3536,prsc:2|A-1187-OUT,B-3018-OUT;n:type:ShaderForge.SFN_Clamp01,id:4844,x:32291,y:32984,varname:node_4844,prsc:2|IN-6071-OUT;n:type:ShaderForge.SFN_Color,id:805,x:31944,y:33145,ptovrint:False,ptlb:Light Color,ptin:_LightColor,varname:node_805,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:8818,x:31245,y:33132,ptovrint:False,ptlb:Normalmap,ptin:_Normalmap,varname:node_8818,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1bdd997b59cc43648840d40f0551a28a,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Set,id:6081,x:31742,y:32523,varname:UVCoord,prsc:2|IN-262-UVOUT;n:type:ShaderForge.SFN_Get,id:3931,x:32098,y:32523,varname:node_3931,prsc:2|IN-6081-OUT;n:type:ShaderForge.SFN_Tex2d,id:3101,x:31417,y:32984,varname:node_3101,prsc:2,tex:1bdd997b59cc43648840d40f0551a28a,ntxv:0,isnm:False|UVIN-7783-OUT,TEX-8818-TEX;n:type:ShaderForge.SFN_Get,id:7783,x:31224,y:32984,varname:node_7783,prsc:2|IN-6081-OUT;n:type:ShaderForge.SFN_Multiply,id:9559,x:31598,y:32984,varname:node_9559,prsc:2|A-3101-RGB,B-3255-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3255,x:31417,y:33132,ptovrint:False,ptlb:Normal Power,ptin:_NormalPower,varname:node_3255,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:6591,x:31773,y:32841,varname:node_6591,prsc:2|A-5973-OUT,B-9559-OUT;n:type:ShaderForge.SFN_OneMinus,id:1187,x:32996,y:32699,varname:node_1187,prsc:2|IN-140-OUT;n:type:ShaderForge.SFN_Set,id:6866,x:31739,y:32699,varname:VertexRGB,prsc:2|IN-766-RGB;n:type:ShaderForge.SFN_Set,id:7995,x:31739,y:32735,varname:VertexAlpha,prsc:2|IN-766-A;n:type:ShaderForge.SFN_Get,id:6839,x:32798,y:32476,varname:node_6839,prsc:2|IN-6866-OUT;n:type:ShaderForge.SFN_Multiply,id:3018,x:32992,y:32841,varname:node_3018,prsc:2|A-2790-OUT,B-3855-OUT;n:type:ShaderForge.SFN_Get,id:3855,x:32791,y:32877,varname:node_3855,prsc:2|IN-7995-OUT;n:type:ShaderForge.SFN_Set,id:5909,x:32098,y:32476,varname:TintAlpha,prsc:2|IN-7241-A;n:type:ShaderForge.SFN_Get,id:2790,x:32791,y:32841,varname:node_2790,prsc:2|IN-5909-OUT;n:type:ShaderForge.SFN_Set,id:7315,x:32453,y:32476,varname:TextureAlpha,prsc:2|IN-8558-A;n:type:ShaderForge.SFN_Get,id:140,x:32795,y:32699,varname:node_140,prsc:2|IN-7315-OUT;n:type:ShaderForge.SFN_OneMinus,id:3698,x:32128,y:32841,varname:node_3698,prsc:2|IN-1733-OUT;n:type:ShaderForge.SFN_Power,id:3674,x:32291,y:32841,varname:node_3674,prsc:2|VAL-3698-OUT,EXP-777-OUT;proporder:7241-4860-8818-777-4164-805-3255;pass:END;sub:END;*/

Shader "Effects/SimpRim Unlit" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _Normalmap ("Normalmap", 2D) = "bump" {}
        _RimPower ("Rim Power", Range(0, 16)) = 1
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _LightColor ("Light Color", Color) = (0,0,0,1)
        _NormalPower ("Normal Power", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TintColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform float4 _LightColor;
            uniform sampler2D _Normalmap; uniform float4 _Normalmap_ST;
            uniform float _NormalPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 UVCoord = i.uv0;
                float2 node_3931 = UVCoord;
                float4 node_8558 = tex2D(_MainTex,TRANSFORM_TEX(node_3931, _MainTex));
                float TextureAlpha = node_8558.a;
                float TintAlpha = _TintColor.a;
                float VertexAlpha = i.vertexColor.a;
                clip(step((1.0 - TextureAlpha),(TintAlpha*VertexAlpha)) - 0.5);
////// Lighting:
////// Emissive:
                float2 node_7783 = UVCoord;
                float4 node_3101 = tex2D(_Normalmap,TRANSFORM_TEX(node_7783, _Normalmap));
                float node_1733 = abs(dot(viewDirection,(i.normalDir*(node_3101.rgb*_NormalPower))));
                float3 VertexRGB = i.vertexColor.rgb;
                float3 emissive = saturate(((saturate((node_8558.rgb+((_TintColor.rgb-0.5)*2.0)))+saturate((pow((1.0 - node_1733),_RimPower)*_RimColor.rgb))+saturate((node_1733*node_1733*_LightColor.rgb)))*VertexRGB));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
            uniform float4 _TintColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
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
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float2 UVCoord = i.uv0;
                float2 node_3931 = UVCoord;
                float4 node_8558 = tex2D(_MainTex,TRANSFORM_TEX(node_3931, _MainTex));
                float TextureAlpha = node_8558.a;
                float TintAlpha = _TintColor.a;
                float VertexAlpha = i.vertexColor.a;
                clip(step((1.0 - TextureAlpha),(TintAlpha*VertexAlpha)) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
