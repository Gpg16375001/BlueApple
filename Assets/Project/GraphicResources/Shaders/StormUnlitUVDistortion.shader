// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33791,y:32470,varname:node_3138,prsc:2|emission-2108-OUT,clip-7868-OUT,voffset-4781-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32777,y:32269,ptovrint:False,ptlb:Tint Color,ptin:_TintColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:4652,x:31357,y:32385,varname:node_4652,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Set,id:3113,x:31564,y:32385,varname:UVCoord,prsc:2|IN-4652-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9238,x:31895,y:32409,ptovrint:False,ptlb:DistortTex,ptin:_DistortTex,varname:node_9238,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4692,x:32065,y:32409,varname:node_4692,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7625-OUT,TEX-9238-TEX;n:type:ShaderForge.SFN_Slider,id:4353,x:31908,y:32312,ptovrint:False,ptlb:Distort Power,ptin:_DistortPower,varname:node_4353,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2509,x:32244,y:32409,varname:node_2509,prsc:2|A-4353-OUT,B-4692-R;n:type:ShaderForge.SFN_Add,id:1840,x:32600,y:32418,varname:node_1840,prsc:2|A-7786-OUT,B-3361-OUT;n:type:ShaderForge.SFN_Tex2d,id:9036,x:32777,y:32586,varname:node_9036,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-1840-OUT,TEX-2262-TEX;n:type:ShaderForge.SFN_Multiply,id:6792,x:32958,y:32825,varname:node_6792,prsc:2|A-7583-OUT,B-4217-OUT;n:type:ShaderForge.SFN_Slider,id:4217,x:32620,y:32899,ptovrint:False,ptlb:Additive,ptin:_Additive,varname:node_4217,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:3773,x:32065,y:32586,varname:node_3773,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-2950-OUT,TEX-9238-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:2262,x:32419,y:32772,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2262,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:7675,x:31895,y:32772,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:7552,x:32065,y:32772,varname:node_7552,prsc:2,dt:3|A-7675-OUT,B-9363-OUT;n:type:ShaderForge.SFN_Time,id:7504,x:31153,y:32244,varname:node_7504,prsc:2;n:type:ShaderForge.SFN_Set,id:6314,x:31564,y:32475,varname:VCoord,prsc:2|IN-4652-V;n:type:ShaderForge.SFN_Set,id:7168,x:31564,y:32248,varname:Time,prsc:2|IN-993-OUT;n:type:ShaderForge.SFN_Set,id:6287,x:31564,y:32431,varname:UCoord,prsc:2|IN-4652-U;n:type:ShaderForge.SFN_Get,id:5771,x:31149,y:32738,varname:node_5771,prsc:2|IN-7168-OUT;n:type:ShaderForge.SFN_Get,id:5701,x:31336,y:32738,varname:node_5701,prsc:2|IN-6314-OUT;n:type:ShaderForge.SFN_Add,id:8964,x:31539,y:32809,varname:node_8964,prsc:2|A-2382-OUT,B-5701-OUT;n:type:ShaderForge.SFN_Append,id:7625,x:31713,y:32600,varname:node_7625,prsc:2|A-8074-OUT,B-8964-OUT;n:type:ShaderForge.SFN_Get,id:3379,x:31336,y:32540,varname:node_3379,prsc:2|IN-6287-OUT;n:type:ShaderForge.SFN_Multiply,id:3292,x:31357,y:32600,varname:node_3292,prsc:2|A-4687-OUT,B-5771-OUT;n:type:ShaderForge.SFN_Vector1,id:4687,x:31170,y:32600,varname:node_4687,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:2382,x:31357,y:32809,varname:node_2382,prsc:2|A-4694-OUT,B-5771-OUT;n:type:ShaderForge.SFN_Vector1,id:4694,x:31170,y:32809,varname:node_4694,prsc:2,v1:-1;n:type:ShaderForge.SFN_Add,id:8074,x:31539,y:32600,varname:node_8074,prsc:2|A-3292-OUT,B-3379-OUT;n:type:ShaderForge.SFN_Multiply,id:993,x:31357,y:32248,varname:node_993,prsc:2|A-7504-TSL,B-7158-OUT;n:type:ShaderForge.SFN_Slider,id:7158,x:30996,y:32169,ptovrint:False,ptlb:Distort Speed,ptin:_DistortSpeed,varname:node_7158,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:1,max:10;n:type:ShaderForge.SFN_ViewVector,id:9363,x:31894,y:32916,varname:node_9363,prsc:2;n:type:ShaderForge.SFN_Lerp,id:109,x:32981,y:32442,varname:node_109,prsc:2|A-7241-RGB,B-9046-RGB,T-9036-R;n:type:ShaderForge.SFN_Color,id:9046,x:32777,y:32442,ptovrint:False,ptlb:Light Color,ptin:_LightColor,varname:node_9046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:0.5,c4:1;n:type:ShaderForge.SFN_Get,id:2950,x:31874,y:32586,varname:node_2950,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Get,id:7786,x:32398,y:32418,varname:node_7786,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Blend,id:8462,x:32419,y:32586,varname:node_8462,prsc:2,blmd:6,clmp:True|SRC-3773-R,DST-2509-OUT;n:type:ShaderForge.SFN_Multiply,id:4781,x:33536,y:33162,varname:node_4781,prsc:2|A-1691-OUT,B-6002-OUT,C-9669-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9669,x:33344,y:33300,ptovrint:False,ptlb:Vertex Offset,ptin:_VertexOffset,varname:node_9669,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:317,x:33332,y:32658,ptovrint:False,ptlb:Clip,ptin:_Clip,varname:node_317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Get,id:7583,x:32756,y:32825,varname:node_7583,prsc:2|IN-544-OUT;n:type:ShaderForge.SFN_Set,id:544,x:32222,y:32772,varname:Dot,prsc:2|IN-7552-OUT;n:type:ShaderForge.SFN_Get,id:1691,x:33323,y:33162,varname:node_1691,prsc:2|IN-8169-OUT;n:type:ShaderForge.SFN_Get,id:9414,x:32222,y:33028,varname:node_9414,prsc:2|IN-544-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5819,x:32419,y:33185,ptovrint:False,ptlb:Rim Power,ptin:_RimPower,varname:node_5819,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:16;n:type:ShaderForge.SFN_Color,id:9698,x:32617,y:33185,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:node_9698,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6591,x:32810,y:33028,varname:node_6591,prsc:2|A-8175-OUT,B-9698-RGB;n:type:ShaderForge.SFN_Clamp01,id:8011,x:32981,y:33028,varname:node_8011,prsc:2|IN-6591-OUT;n:type:ShaderForge.SFN_Add,id:2255,x:33149,y:32442,varname:node_2255,prsc:2|A-109-OUT,B-6792-OUT,C-8011-OUT;n:type:ShaderForge.SFN_VertexColor,id:2251,x:32981,y:32586,varname:node_2251,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6938,x:33332,y:32785,varname:node_6938,prsc:2|A-962-OUT,B-7253-OUT;n:type:ShaderForge.SFN_Step,id:7868,x:33524,y:32658,varname:node_7868,prsc:2|A-317-OUT,B-6938-OUT;n:type:ShaderForge.SFN_Set,id:1706,x:32789,y:33185,varname:RimColor,prsc:2|IN-9698-RGB;n:type:ShaderForge.SFN_Set,id:4080,x:32600,y:32586,varname:DistortUV,prsc:2|IN-8462-OUT;n:type:ShaderForge.SFN_Get,id:3361,x:32398,y:32470,varname:node_3361,prsc:2|IN-4080-OUT;n:type:ShaderForge.SFN_Get,id:962,x:33128,y:32785,varname:node_962,prsc:2|IN-4080-OUT;n:type:ShaderForge.SFN_Get,id:6002,x:33323,y:33213,varname:node_6002,prsc:2|IN-4080-OUT;n:type:ShaderForge.SFN_Multiply,id:2108,x:33332,y:32442,varname:node_2108,prsc:2|A-2255-OUT,B-2251-RGB;n:type:ShaderForge.SFN_Add,id:8086,x:33159,y:33028,varname:node_8086,prsc:2|A-2251-A,B-8115-OUT;n:type:ShaderForge.SFN_Slider,id:8115,x:32789,y:33259,ptovrint:False,ptlb:Vertex Alpha Add,ptin:_VertexAlphaAdd,varname:node_8115,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:7253,x:33323,y:33028,varname:node_7253,prsc:2|IN-8086-OUT;n:type:ShaderForge.SFN_OneMinus,id:2178,x:32419,y:33028,varname:node_2178,prsc:2|IN-9414-OUT;n:type:ShaderForge.SFN_Power,id:8175,x:32617,y:33028,varname:node_8175,prsc:2|VAL-2178-OUT,EXP-5819-OUT;n:type:ShaderForge.SFN_Set,id:8169,x:32044,y:32916,varname:NormalDir,prsc:2|IN-7675-OUT;proporder:7241-9046-9698-2262-9238-5819-317-4353-7158-4217-9669-8115;pass:END;sub:END;*/

Shader "Effects/StormUnlit UV Distortion" {
    Properties {
        _TintColor ("Tint Color", Color) = (0,1,0,1)
        _LightColor ("Light Color", Color) = (1,1,0.5,1)
        _RimColor ("Rim Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _DistortTex ("DistortTex", 2D) = "white" {}
        _RimPower ("Rim Power", Float ) = 16
        _Clip ("Clip", Float ) = 0.5
        _DistortPower ("Distort Power", Range(0, 1)) = 1
        _DistortSpeed ("Distort Speed", Range(-10, 10)) = 1
        _Additive ("Additive", Range(-1, 1)) = 0
        _VertexOffset ("Vertex Offset", Float ) = 0.5
        _VertexAlphaAdd ("Vertex Alpha Add", Range(-1, 1)) = 0
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
            uniform float _Additive;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _DistortSpeed;
            uniform float4 _LightColor;
            uniform float _VertexOffset;
            uniform float _Clip;
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform float _VertexAlphaAdd;
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
                float3 NormalDir = v.normal;
                float2 UVCoord = o.uv0;
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_2950, _DistortTex),0.0,0));
                float4 node_7504 = _Time;
                float Time = (node_7504.r*_DistortSpeed);
                float node_5771 = Time;
                float UCoord = o.uv0.r;
                float VCoord = o.uv0.g;
                float2 node_7625 = float2(((0.5*node_5771)+UCoord),(((-1.0)*node_5771)+VCoord));
                float4 node_4692 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_7625, _DistortTex),0.0,0));
                float DistortUV = saturate((1.0-(1.0-node_3773.r)*(1.0-(_DistortPower*node_4692.r))));
                v.vertex.xyz += (NormalDir*DistortUV*_VertexOffset);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
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
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2D(_DistortTex,TRANSFORM_TEX(node_2950, _DistortTex));
                float4 node_7504 = _Time;
                float Time = (node_7504.r*_DistortSpeed);
                float node_5771 = Time;
                float UCoord = i.uv0.r;
                float VCoord = i.uv0.g;
                float2 node_7625 = float2(((0.5*node_5771)+UCoord),(((-1.0)*node_5771)+VCoord));
                float4 node_4692 = tex2D(_DistortTex,TRANSFORM_TEX(node_7625, _DistortTex));
                float DistortUV = saturate((1.0-(1.0-node_3773.r)*(1.0-(_DistortPower*node_4692.r))));
                clip(step(_Clip,(DistortUV*saturate((i.vertexColor.a+_VertexAlphaAdd)))) - 0.5);
////// Lighting:
////// Emissive:
                float2 node_1840 = (UVCoord+DistortUV);
                float4 node_9036 = tex2D(_MainTex,TRANSFORM_TEX(node_1840, _MainTex));
                float Dot = abs(dot(i.normalDir,viewDirection));
                float3 emissive = ((lerp(_TintColor.rgb,_LightColor.rgb,node_9036.r)+(Dot*_Additive)+saturate((pow((1.0 - Dot),_RimPower)*_RimColor.rgb)))*i.vertexColor.rgb);
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
            uniform sampler2D _DistortTex; uniform float4 _DistortTex_ST;
            uniform float _DistortPower;
            uniform float _DistortSpeed;
            uniform float _VertexOffset;
            uniform float _Clip;
            uniform float _VertexAlphaAdd;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float3 NormalDir = v.normal;
                float2 UVCoord = o.uv0;
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_2950, _DistortTex),0.0,0));
                float4 node_7504 = _Time;
                float Time = (node_7504.r*_DistortSpeed);
                float node_5771 = Time;
                float UCoord = o.uv0.r;
                float VCoord = o.uv0.g;
                float2 node_7625 = float2(((0.5*node_5771)+UCoord),(((-1.0)*node_5771)+VCoord));
                float4 node_4692 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_7625, _DistortTex),0.0,0));
                float DistortUV = saturate((1.0-(1.0-node_3773.r)*(1.0-(_DistortPower*node_4692.r))));
                v.vertex.xyz += (NormalDir*DistortUV*_VertexOffset);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
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
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2D(_DistortTex,TRANSFORM_TEX(node_2950, _DistortTex));
                float4 node_7504 = _Time;
                float Time = (node_7504.r*_DistortSpeed);
                float node_5771 = Time;
                float UCoord = i.uv0.r;
                float VCoord = i.uv0.g;
                float2 node_7625 = float2(((0.5*node_5771)+UCoord),(((-1.0)*node_5771)+VCoord));
                float4 node_4692 = tex2D(_DistortTex,TRANSFORM_TEX(node_7625, _DistortTex));
                float DistortUV = saturate((1.0-(1.0-node_3773.r)*(1.0-(_DistortPower*node_4692.r))));
                clip(step(_Clip,(DistortUV*saturate((i.vertexColor.a+_VertexAlphaAdd)))) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
