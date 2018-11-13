// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33718,y:32341,varname:node_3138,prsc:2|emission-4454-OUT,voffset-4781-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32777,y:32108,ptovrint:False,ptlb:Tint Color,ptin:_TintColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_TexCoord,id:4652,x:32065,y:32409,varname:node_4652,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Set,id:3113,x:32231,y:32418,varname:UVCoord,prsc:2|IN-4652-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9238,x:31895,y:32711,ptovrint:False,ptlb:DistortTex,ptin:_DistortTex,varname:node_9238,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:641a3a3985976d04fadba3e1c6bdfcad,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9036,x:32777,y:32442,varname:node_9036,prsc:2,tex:a16602472734173489887fa3bfd3c05a,ntxv:0,isnm:False|UVIN-7786-OUT,TEX-2262-TEX;n:type:ShaderForge.SFN_Multiply,id:6792,x:32969,y:32584,varname:node_6792,prsc:2|A-4217-OUT,B-1322-OUT,C-7552-OUT,D-211-OUT;n:type:ShaderForge.SFN_Slider,id:4217,x:32620,y:32584,ptovrint:False,ptlb:Additive,ptin:_Additive,varname:node_4217,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:3773,x:32065,y:32584,varname:node_3773,prsc:2,tex:641a3a3985976d04fadba3e1c6bdfcad,ntxv:0,isnm:False|UVIN-2950-OUT,TEX-9238-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:2262,x:32607,y:32280,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2262,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a16602472734173489887fa3bfd3c05a,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:7675,x:31895,y:32993,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:7552,x:32777,y:32713,varname:node_7552,prsc:2,dt:0|A-9363-OUT,B-7583-OUT;n:type:ShaderForge.SFN_Set,id:6314,x:32231,y:32508,varname:VCoord,prsc:2|IN-4652-V;n:type:ShaderForge.SFN_Set,id:6287,x:32231,y:32464,varname:UCoord,prsc:2|IN-4652-U;n:type:ShaderForge.SFN_ViewVector,id:9363,x:32607,y:32711,varname:node_9363,prsc:2;n:type:ShaderForge.SFN_Lerp,id:109,x:32969,y:32442,varname:node_109,prsc:2|A-7241-RGB,B-9046-RGB,T-9036-R;n:type:ShaderForge.SFN_Color,id:9046,x:32777,y:32280,ptovrint:False,ptlb:Light Color,ptin:_LightColor,varname:node_9046,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Get,id:2950,x:31874,y:32584,varname:node_2950,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Get,id:7786,x:32586,y:32442,varname:node_7786,prsc:2|IN-3113-OUT;n:type:ShaderForge.SFN_Multiply,id:4781,x:33487,y:33137,varname:node_4781,prsc:2|A-1691-OUT,B-9887-OUT,C-7469-OUT,D-9669-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9669,x:33316,y:33289,ptovrint:False,ptlb:Vertex Offset,ptin:_VertexOffset,varname:node_9669,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:317,x:33487,y:32915,ptovrint:False,ptlb:Clip,ptin:_Clip,varname:node_317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Get,id:7583,x:32584,y:32860,varname:node_7583,prsc:2|IN-544-OUT;n:type:ShaderForge.SFN_Set,id:544,x:32231,y:32993,varname:Dot,prsc:2|IN-6818-OUT;n:type:ShaderForge.SFN_Get,id:1691,x:33295,y:33137,varname:node_1691,prsc:2|IN-3672-OUT;n:type:ShaderForge.SFN_Get,id:9414,x:32231,y:33070,varname:node_9414,prsc:2|IN-544-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5819,x:32430,y:33070,ptovrint:False,ptlb:Rim Power,ptin:_RimPower,varname:node_5819,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:16;n:type:ShaderForge.SFN_Color,id:9698,x:32605,y:33070,ptovrint:False,ptlb:Rim Color,ptin:_RimColor,varname:node_9698,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6591,x:32777,y:32915,varname:node_6591,prsc:2|A-6796-OUT,B-9698-RGB;n:type:ShaderForge.SFN_Clamp01,id:8011,x:32969,y:32915,varname:node_8011,prsc:2|IN-6591-OUT;n:type:ShaderForge.SFN_Add,id:2255,x:33149,y:32442,varname:node_2255,prsc:2|A-109-OUT,B-6792-OUT,C-8011-OUT;n:type:ShaderForge.SFN_VertexColor,id:2251,x:32969,y:32713,varname:node_2251,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6938,x:33487,y:33000,varname:node_6938,prsc:2|A-3706-OUT,B-9036-R;n:type:ShaderForge.SFN_Multiply,id:4237,x:33316,y:32442,varname:node_4237,prsc:2|A-2255-OUT,B-2251-RGB,C-1939-OUT,D-531-OUT;n:type:ShaderForge.SFN_Step,id:7779,x:33655,y:32915,varname:node_7779,prsc:2|A-317-OUT,B-6938-OUT;n:type:ShaderForge.SFN_Clamp01,id:4454,x:33487,y:32442,varname:node_4454,prsc:2|IN-4237-OUT;n:type:ShaderForge.SFN_Set,id:506,x:32413,y:32584,varname:DistortUV,prsc:2|IN-3820-OUT;n:type:ShaderForge.SFN_Get,id:9887,x:33295,y:33173,varname:node_9887,prsc:2|IN-506-OUT;n:type:ShaderForge.SFN_Add,id:7401,x:33149,y:33000,varname:node_7401,prsc:2|A-2251-A,B-227-OUT;n:type:ShaderForge.SFN_Slider,id:227,x:32812,y:33066,ptovrint:False,ptlb:Vertex Alpha Add,ptin:_VertexAlphaAdd,varname:node_227,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:3706,x:33316,y:33000,varname:node_3706,prsc:2|IN-7401-OUT;n:type:ShaderForge.SFN_Get,id:2056,x:31874,y:32860,varname:node_2056,prsc:2|IN-6314-OUT;n:type:ShaderForge.SFN_Get,id:1322,x:32756,y:32655,varname:node_1322,prsc:2|IN-506-OUT;n:type:ShaderForge.SFN_Clamp01,id:3820,x:32252,y:32584,varname:node_3820,prsc:2|IN-3773-R;n:type:ShaderForge.SFN_Clamp01,id:5258,x:32252,y:32860,varname:node_5258,prsc:2|IN-8519-OUT;n:type:ShaderForge.SFN_Set,id:6853,x:32409,y:32860,varname:DropMask,prsc:2|IN-5258-OUT;n:type:ShaderForge.SFN_Get,id:7469,x:33295,y:33217,varname:node_7469,prsc:2|IN-6853-OUT;n:type:ShaderForge.SFN_Get,id:211,x:32756,y:32860,varname:node_211,prsc:2|IN-6853-OUT;n:type:ShaderForge.SFN_Multiply,id:8519,x:32065,y:32860,varname:node_8519,prsc:2|A-2056-OUT,B-6016-OUT;n:type:ShaderForge.SFN_Vector1,id:6016,x:31895,y:32918,varname:node_6016,prsc:2,v1:4;n:type:ShaderForge.SFN_Set,id:9252,x:33827,y:32915,varname:Cutout,prsc:2|IN-7779-OUT;n:type:ShaderForge.SFN_Get,id:1939,x:33128,y:32584,varname:node_1939,prsc:2|IN-9252-OUT;n:type:ShaderForge.SFN_Get,id:531,x:33128,y:32633,varname:node_531,prsc:2|IN-6853-OUT;n:type:ShaderForge.SFN_Dot,id:6818,x:32065,y:32993,varname:node_6818,prsc:2,dt:3|A-7675-OUT,B-724-OUT;n:type:ShaderForge.SFN_ViewVector,id:724,x:31895,y:33141,varname:node_724,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:7939,x:32430,y:32915,varname:node_7939,prsc:2|IN-9414-OUT;n:type:ShaderForge.SFN_Power,id:6796,x:32605,y:32915,varname:node_6796,prsc:2|VAL-7939-OUT,EXP-5819-OUT;n:type:ShaderForge.SFN_Set,id:3672,x:32044,y:33141,varname:NomalDir,prsc:2|IN-7675-OUT;proporder:7241-9046-9698-2262-9238-5819-317-4217-9669-227;pass:END;sub:END;*/

Shader "Effects/WaterDropAdd" {
    Properties {
        _TintColor ("Tint Color", Color) = (0,0,0,1)
        _LightColor ("Light Color", Color) = (0.5,0.5,0.5,1)
        _RimColor ("Rim Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _DistortTex ("DistortTex", 2D) = "white" {}
        _RimPower ("Rim Power", Float ) = 16
        _Clip ("Clip", Float ) = 0.5
        _Additive ("Additive", Range(-1, 1)) = 0
        _VertexOffset ("Vertex Offset", Float ) = 0.5
        _VertexAlphaAdd ("Vertex Alpha Add", Range(-1, 1)) = 0
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
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform float4 _TintColor;
            uniform sampler2D _DistortTex; uniform float4 _DistortTex_ST;
            uniform float _Additive;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
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
                float3 NomalDir = v.normal;
                float2 UVCoord = o.uv0;
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_2950, _DistortTex),0.0,0));
                float DistortUV = saturate(node_3773.r);
                float VCoord = o.uv0.g;
                float DropMask = saturate((VCoord*4.0));
                v.vertex.xyz += (NomalDir*DistortUV*DropMask*_VertexOffset);
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
////// Lighting:
////// Emissive:
                float2 UVCoord = i.uv0;
                float2 node_7786 = UVCoord;
                float4 node_9036 = tex2D(_MainTex,TRANSFORM_TEX(node_7786, _MainTex));
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2D(_DistortTex,TRANSFORM_TEX(node_2950, _DistortTex));
                float DistortUV = saturate(node_3773.r);
                float Dot = abs(dot(i.normalDir,viewDirection));
                float VCoord = i.uv0.g;
                float DropMask = saturate((VCoord*4.0));
                float Cutout = step(_Clip,(saturate((i.vertexColor.a+_VertexAlphaAdd))*node_9036.r));
                float3 emissive = saturate(((lerp(_TintColor.rgb,_LightColor.rgb,node_9036.r)+(_Additive*DistortUV*dot(viewDirection,Dot)*DropMask)+saturate((pow((1.0 - Dot),_RimPower)*_RimColor.rgb)))*i.vertexColor.rgb*Cutout*DropMask));
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
            uniform float _VertexOffset;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float3 NomalDir = v.normal;
                float2 UVCoord = o.uv0;
                float2 node_2950 = UVCoord;
                float4 node_3773 = tex2Dlod(_DistortTex,float4(TRANSFORM_TEX(node_2950, _DistortTex),0.0,0));
                float DistortUV = saturate(node_3773.r);
                float VCoord = o.uv0.g;
                float DropMask = saturate((VCoord*4.0));
                v.vertex.xyz += (NomalDir*DistortUV*DropMask*_VertexOffset);
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
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
