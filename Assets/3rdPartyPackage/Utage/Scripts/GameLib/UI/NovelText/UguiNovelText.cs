// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// ノベル用に、禁則処理などを含めたテキスト表示
	/// </summary>
	[RequireComponent(typeof(UguiNovelTextGenerator))]
	[AddComponentMenu("Utage/Lib/UI/NovelText")]
	public class UguiNovelText : Text
	{
		public int LengthOfView
		{
			get { return TextGenerator.LengthOfView; }
			set { TextGenerator.LengthOfView = value; }
		}

		public UguiNovelTextGenerator TextGenerator { get { return textGenerator ?? (textGenerator = GetComponent<UguiNovelTextGenerator>()); } }
		UguiNovelTextGenerator textGenerator;

		//文字送りをしない場合の文字の最後の座標
		public Vector3 EndPosition { get { return TextGenerator.EndPosition; } }

		//文字送りをする場合の文字の最後の座標
		public Vector3 CurrentEndPosition { get { TextGenerator.RefreshEndPosition(); return TextGenerator.EndPosition; } }


        //頂点情報を作成
        /// <summary>
        /// Draw the Text.
        /// </summary>
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
        protected override void OnFillVBO(List<UIVertex> vbo)
		{
            if (font == null)
                return;

            if (TextGenerator.IsRequestingCharactersInTexture)
            {
                return;
            }

            //フォントの再作成によるものであればその旨を通知
            if (!isDirtyVerts)
            {
                TextGenerator.IsRebuidFont = true;
            }

            IList<UIVertex> verts = TextGenerator.CreateVertex();
            vbo.AddRange(verts);
            isDirtyVerts = false;
        }
#elif UNITY_5_2_0
        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(Mesh toFill)
        {
            if (font == null)
                return;

            if (TextGenerator.IsRequestingCharactersInTexture)
            {
                return;
            }

            //フォントの再作成によるものであればその旨を通知
            if (!isDirtyVerts)
            {
                TextGenerator.IsRebuidFont = true;
            }

            IList<UIVertex> verts = TextGenerator.CreateVertex();
            using (var vh = new VertexHelper())
            {
                for (int i = 0; i < verts.Count; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    if (tempVertsIndex == 3)
                        vh.AddUIVertexQuad(m_TempVerts);
                }
                vh.FillMesh(toFill);
            }
            isDirtyVerts = false;
        }
#else
        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (font == null)
                return;

            if (TextGenerator.IsRequestingCharactersInTexture)
            {
                return;
            }

            //フォントの再作成によるものであればその旨を通知
            if (!isDirtyVerts)
            {
                TextGenerator.IsRebuidFont = true;
            }

			var verts = ListPool<UIVertex>.Get();
			TextGenerator.CreateVertex(verts);
            vh.Clear();
            for (int i = 0; i < verts.Count; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                if (tempVertsIndex == 3)
                    vh.AddUIVertexQuad(m_TempVerts);
            }
			ListPool<UIVertex>.Release(verts);
			isDirtyVerts = false;
        }
#endif

        protected override void Awake()
		{
			base.Awake();
			UnityAction onDirtyVertsCallback = OnDirtyVerts;
			m_OnDirtyVertsCallback += onDirtyVertsCallback;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			TextGenerator.ChangeAll();
			base.OnDisable();
		}

		public override void SetAllDirty()
		{
			TextGenerator.ChangeAll();
			base.SetAllDirty();
		}

		void OnDirtyVerts()
		{
			TextGenerator.ChangeAll();
			isDirtyVerts = true;
		}
		bool isDirtyVerts = false;

		//頂点情報のみ変化している（文字送りやエフェクトカラーの変更などにつかう）
		internal void SetVerticesOnlyDirty()
		{
			TextGenerator.SetVerticesOnlyDirty();
		}

		//行間を含んだ高さを取得
		public int GetTotalLineHeight( int fontSize )
		{
			//uGUIは行間の基本値1=1.2の模様
			return Mathf.CeilToInt(fontSize * (lineSpacing + 0.2f));
		}

		public override float preferredHeight
		{
			get
			{
				return TextGenerator.PreferredHeight;
			}
		}

		public override float preferredWidth
		{
			get
			{
				return TextGenerator.PreferredWidth;
			}
		}
	}
}

