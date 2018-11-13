// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33658,y:32342,varname:node_3138,prsc:2|emission-92-OUT,clip-8546-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32777,y:32269,ptovrint:False,ptlb:Tint Color,ptin:_TintColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:4652,x:31547,y:32598,varname:node_4652,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Set,id:3113,x:31708,y:32598,varname:UVCoord,prsc:2|IN-4652-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9238,x:31903,y:32598,ptovrint:False,ptlb:DistortTex,ptin:_DistortTex,varname:node_9238,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4692,x:32071,y:32752,varname:node_4692,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7625-OUT,TEX-9238-TEX;n:type:ShaderForge.SFN_Slider,id:4353,x:31914,y:32917,ptovrint:False,ptlb:Distort Power,ptin:_DistortPower,varname:node_4353,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2509,x:32250,y:32752,varname:node_2509,prsc:2|A-4353-OUT,B-4692-R;n:type:ShaderForge.SFN_Add,id:1840,x:32600,y:32442,varname:node_1840,prsc:2|A-8462-OUT,B-7786-OUT;n:type:ShaderForge.SFN_Tex2d,id:9036,x:32777,y:32578,varname:node_9036,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-1840-OUT,TEX-2262-TEX;n:type:ShaderForge.SFN_Tex2d,id:3773,x:32250,y:32442,varname:node_3773,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8675-UVOUT,TEX-9238-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:2262,x:32600,y:32578,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2262,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Time,id:7504,x:31360,y:32442,varname:node_7504,prsc:2;n:type:ShaderForge.SFN_Set,id:6314,x:31708,y:32688,varname:VCoord,prsc:2|IN-4652-V;n:type:ShaderForge.SFN_Set,id:7168,x:31708,y:32442,varname:Time,prsc:2|IN-993-OUT;n:type:ShaderForge.SFN_Set,id:6287,x:31708,y:32644,varname:UCoord,prsc:2|IN-4652-U;n:type:ShaderForge.SFN_Get,id:5771,x:31339,y:32870,varname:node_5771,prsc:2|IN-7168-OUT;n:type:ShaderForge.SFN_Get,id:5701,x:31526,y:33035,varname:node_5701,prsc:2|IN-6314-OUT;n:type:ShaderForge.SFN_Add,id:8964,x:31729,y:32917,varname:node_8964,prsc:2|A-2382-OUT,B-5701-OUT;n:type:ShaderForge.SFN_Append,id:7625,x:31903,y:32752,varname:node_7625,prsc:2|A-8074-OUT,B-8964-OUT;n:type:ShaderForge.SFN_Get,id:3379,x:31526,y:32870,varname:node_3379,prsc:2|IN-6287-OUT;n:type:ShaderForge.SFN_Multiply,id:3292,x:31547,y:32752,varname:node_3292,prsc:2|A-4687-OUT,B-5771-OUT;n:type:ShaderForge.SFN_Vector1,id:4687,x:31360,y:32752,varname:node_4687,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:2382,x:31547,y:32917,varname:node_2382,prsc:2|A-4694-OUT,B-5771-OUT;n:type:ShaderForge.SFN_Vector1,id:4694,x:31360,y:32917,varname:node_4694,prsc:2,v1:-1;n:type:ShaderForge.SFN_Add,id:8074,x:31729,y:32752,varname:node_8074,prsc:2|A-3292-OUT,B-3379-OUT;n:type:ShaderForge.SFN_Multiply,id:993,x:31547,y:32442,varname:node_993,prsc:2|A-7504-TSL,B-7158-OUT;n:type:ShaderForge.SFN_Slider,id:7158,x:31203,y:32598,ptovrint:False,ptlb:Distort Speed,ptin:_DistortSpeed,varname:node_7158,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:109,x:32981,y:32442,varname:node_109,prsc:2|A-7241-RGB,B-9046-RGB,T-9036-R;n:type:ShaderForge.SFN_Color,id:9046,x:32777,y:32442,ptovrint:False,ptlb:Light Color,ptin:_LightColor,varname:node_9046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Get,id:2950,x:31882,y:32366,varname:node_2950,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Get,id:7786,x:32399,y:32611,varname:node_7786,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Blend,id:8462,x:32420,y:32442,varname:node_8462,prsc:2,blmd:10,clmp:True|SRC-3773-R,DST-2509-OUT;n:type:ShaderForge.SFN_ValueProperty,id:317,x:32600,y:33206,ptovrint:False,ptlb:Clip,ptin:_Clip,varname:node_317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_VertexColor,id:2251,x:32600,y:32917,varname:node_2251,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6938,x:32777,y:32752,varname:node_6938,prsc:2|A-9831-A,B-2251-A;n:type:ShaderForge.SFN_Step,id:7868,x:32978,y:32752,varname:node_7868,prsc:2|A-317-OUT,B-6938-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:8282,x:32978,y:33059,varname:node_8282,prsc:2|IN-6938-OUT,IMIN-317-OUT,IMAX-7447-OUT,OMIN-1798-OUT,OMAX-2036-OUT;n:type:ShaderForge.SFN_ValueProperty,id:780,x:32600,y:33059,ptovrint:False,ptlb:Inner Range,ptin:_InnerRange,varname:node_780,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_Add,id:7447,x:32777,y:33059,varname:node_7447,prsc:2|A-780-OUT,B-317-OUT;n:type:ShaderForge.SFN_Vector1,id:2036,x:32777,y:33268,varname:node_2036,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1798,x:32777,y:33206,varname:node_1798,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:5577,x:33145,y:32442,varname:node_5577,prsc:2|A-109-OUT,B-9864-OUT;n:type:ShaderForge.SFN_Clamp01,id:9477,x:33316,y:33059,varname:node_9477,prsc:2|IN-46-OUT;n:type:ShaderForge.SFN_Multiply,id:46,x:33145,y:33059,varname:node_46,prsc:2|A-8282-OUT,B-8354-RGB;n:type:ShaderForge.SFN_Color,id:8354,x:32978,y:33206,ptovrint:False,ptlb:Inner Color,ptin:_InnerColor,varname:node_8354,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Set,id:5972,x:33469,y:33059,varname:InnerColor,prsc:2|IN-9477-OUT;n:type:ShaderForge.SFN_Get,id:9864,x:32957,y:32578,varname:node_9864,prsc:2|IN-5972-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5275,x:32600,y:33345,ptovrint:False,ptlb:Outer Range,ptin:_OuterRange,varname:node_5275,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_Subtract,id:418,x:32777,y:33345,varname:node_418,prsc:2|A-317-OUT,B-5275-OUT;n:type:ShaderForge.SFN_Clamp01,id:8118,x:33318,y:32752,varname:node_8118,prsc:2|IN-9122-OUT;n:type:ShaderForge.SFN_Add,id:9122,x:33145,y:32752,varname:node_9122,prsc:2|A-7868-OUT,B-9466-RGB;n:type:ShaderForge.SFN_Color,id:9466,x:32978,y:32917,ptovrint:False,ptlb:Outer Color,ptin:_OuterColor,varname:node_9466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Set,id:8288,x:33469,y:32752,varname:OuterColor,prsc:2|IN-8118-OUT;n:type:ShaderForge.SFN_Get,id:2532,x:33124,y:32660,varname:node_2532,prsc:2|IN-8288-OUT;n:type:ShaderForge.SFN_Step,id:7833,x:32978,y:33345,varname:node_7833,prsc:2|A-418-OUT,B-6938-OUT;n:type:ShaderForge.SFN_Set,id:8825,x:33124,y:33345,varname:OuterColorBase,prsc:2|IN-7833-OUT;n:type:ShaderForge.SFN_Clamp01,id:92,x:33490,y:32442,varname:node_92,prsc:2|IN-9741-OUT;n:type:ShaderForge.SFN_Multiply,id:9741,x:33318,y:32442,varname:node_9741,prsc:2|A-5577-OUT,B-4066-OUT,C-2532-OUT;n:type:ShaderForge.SFN_Get,id:4066,x:33124,y:32578,varname:node_4066,prsc:2|IN-8825-OUT;n:type:ShaderForge.SFN_Tex2d,id:9831,x:32600,y:32752,varname:node_9831,prsc:2,tex:6e921f7e23589ab42a7c3745e2f10ca0,ntxv:0,isnm:False|TEX-6455-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:6455,x:32432,y:32752,ptovrint:False,ptlb:CrackTex,ptin:_CrackTex,varname:node_6455,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6e921f7e23589ab42a7c3745e2f10ca0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Rotator,id:8675,x:32071,y:32442,varname:node_8675,prsc:2|UVIN-2950-OUT,PIV-3540-OUT,ANG-8957-OUT;n:type:ShaderForge.SFN_Get,id:8546,x:33469,y:32578,varname:node_8546,prsc:2|IN-8825-OUT;n:type:ShaderForge.SFN_Get,id:8957,x:31882,y:32532,varname:node_8957,prsc:2|IN-7168-OUT;n:type:ShaderForge.SFN_Vector2,id:3540,x:31903,y:32442,varname:node_3540,prsc:2,v1:0.5,v2:0.5;proporder:7241-9046-8354-9466-2262-9238-317-4353-7158-780-5275-6455;pass:END;sub:END;*/

Shader "Effects/LavaUnlit" {
    Properties {
        _TintColor ("Tint Color", Color) = (1,0,0,1)
        _LightColor ("Light Color", Color) = (1,1,0,1)
        _InnerColor ("Inner Color", Color) = (1,1,1,1)
        _OuterColor ("Outer Color", Color) = (0,0,0,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _DistortTex ("DistortTex", 2D) = "white" {}
        _Clip ("Clip", Float ) = 0.5
        _DistortPower ("Distort Power", Range(0, 1)) = 1
        _DistortSpeed ("Distort Speed", Range(-1, 1)) = 1
        _InnerRange ("Inner Range", Float ) = 0.3
        _OuterRange ("Outer Range", Float ) = 0.05
        _CrackTex ("CrackTex", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform float4 _TintColor;
            uniform sampler2D _DistortTex; uniform float4 _DistortTex_ST;
            uniform float _DistortPower;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _DistortSpeed;
            uniform float4 _LightColor;
            uniform float _Clip;
            uniform float _InnerRange;
            uniform float4 _InnerColor;
            uniform float _OuterRange;
            uniform float4 _OuterColor;
            uniform sampler2D _CrackTex; uniform float4 _CrackTex_ST;
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
                float4 node_9831 = tex2D(_CrackTex,TRANSFORM_TEX(i.uv0, _CrackTex));
                float node_6938 = (node_9831.a*i.vertexColor.a);
                float OuterColorBase = step((_Clip-_OuterRange),node_6938);
                clip(OuterColorBase - 0.5);
////// Lighting:
////// Emissive:
                float4 node_7504 = _Time;
                float Time = (node_7504.r*_DistortSpeed);
                float node_8675_ang = Time;
                float node_8675_spd = 1.0;
                float node_8675_cos = cos(node_8675_spd*node_8675_ang);
                float node_8675_sin = sin(node_8675_spd*node_8675_ang);
                float2 node_8675_piv = float2(0.5,0.5);
                float2 UVCoord = i.uv0;
                float2 node_8675 = (mul(UVCoord-node_8675_piv,float2x2( node_8675_cos, -node_8675_sin, node_8675_sin, node_8675_cos))+node_8675_piv);
                float4 node_3773 = tex2D(_DistortTex,TRANSFORM_TEX(node_8675, _DistortTex));
                float node_5771 = Time;
                float UCoord = i.uv0.r;
                float VCoord = i.uv0.g;
                float2 node_7625 = float2(((0.5*node_5771)+UCoord),(((-1.0)*node_5771)+VCoord));
                float4 node_4692 = tex2D(_DistortTex,TRANSFORM_TEX(node_7625, _DistortTex));
                float2 node_1840 = (saturate(( (_DistortPower*node_4692.r) > 0.5 ? (1.0-(1.0-2.0*((_DistortPower*node_4692.r)-0.5))*(1.0-node_3773.r)) : (2.0*(_DistortPower*node_4692.r)*node_3773.r) ))+UVCoord);
                float4 node_9036 = tex2D(_MainTex,TRANSFORM_TEX(node_1840, _MainTex));
                float node_1798 = 1.0;
                float3 InnerColor = saturate(((node_1798 + ( (node_6938 - _Clip) * (0.0 - node_1798) ) / ((_InnerRange+_Clip) - _Clip))*_InnerColor.rgb));
                float3 OuterColor = saturate((step(_Clip,node_6938)+_OuterColor.rgb));
                float3 emissive = saturate(((lerp(_TintColor.rgb,_LightColor.rgb,node_9036.r)+InnerColor)*OuterColorBase*OuterColor));
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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform float _Clip;
            uniform float _OuterRange;
            uniform sampler2D _CrackTex; uniform float4 _CrackTex_ST;
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
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 node_9831 = tex2D(_CrackTex,TRANSFORM_TEX(i.uv0, _CrackTex));
                float node_6938 = (node_9831.a*i.vertexColor.a);
                float OuterColorBase = step((_Clip-_OuterRange),node_6938);
                clip(OuterColorBase - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
