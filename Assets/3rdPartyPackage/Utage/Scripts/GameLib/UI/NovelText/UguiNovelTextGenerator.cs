// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UtageExtensions;
using UnityEngine.Profiling;


namespace Utage
{

	/// <summary>
	/// ノベル用に、禁則処理などを含めたテキスト表示
	/// </summary>
	[AddComponentMenu("Utage/Lib/UI/Internal/NovelTextGenerator")]
	public class UguiNovelTextGenerator : MonoBehaviour
	{
		public UguiNovelText NovelText { get { return novelText ?? (novelText = GetComponent<UguiNovelText>()); } }
		UguiNovelText novelText;

		TextData textData;

		public TextData TextData
		{
			get { return textData; }
		}

		/// <summary>
		/// テキスト表示の最大幅（0以下は無限）
		/// </summary>
		public float MaxWidth { get; private set; }

		/// <summary>
		/// テキスト表示の最大高さ（0以下は無限）
		/// </summary>
		public float MaxHeight { get; private set; }

		/// <summary>
		/// 実際に表示される高さ
		/// </summary>
		public float PreferredHeight
		{
			get
			{
				CalcPreferredHeight();
				return preferredHeight;
			}
			private set { preferredHeight = value; }
		}
		float preferredHeight;

		/// <summary>
		/// 実際に表示される幅(幅が決まっていなくて自動改行がない前提)
		/// </summary>
		public float PreferredWidth
		{
			get
			{
				CalcPreferredWidth();
				return preferredWidth;
			}
			private set { preferredWidth = value; }
		}
		float preferredWidth;

		float Height
		{
			get { return height; }
		}
		float height;

		/// <summary>
		/// スペースの幅(px)
		/// </summary>
		public float Space
		{
			get { return space; }
			set { space = value; SetAllDirty(); }
		}
		[SerializeField]
		float space = -1;

		/// <summary>
		/// 文字間(px)
		/// </summary>
		public float LetterSpaceSize
		{
			get { return letterSpaceSize; }
			set { letterSpaceSize = value; SetAllDirty(); }
		}
		[SerializeField]
		float letterSpaceSize = 1;


		/// <summary>
		/// 禁則処理の仕方
		/// </summary>
		[System.Flags]
		public enum WordWrap
		{
			/// <summary>デフォルト（半角のみ）</summary>
			Default = 0x1,
			/// <summary>日本語の禁則処理</summary>
			JapaneseKinsoku = 0x2,
		};
		/// <summary>
		/// 禁則処理の仕方
		/// </summary>
		public WordWrap WordWrapType
		{
			get { return wordWrap; }
			set { wordWrap = value; SetAllDirty(); }
		}
		[SerializeField]
		[EnumFlagsAttribute]
		WordWrap wordWrap = WordWrap.Default | WordWrap.JapaneseKinsoku;

		/// <summary>表示する文字の長さ(-1なら全部表示)</summary>
		public int LengthOfView
		{
			get { return lengthOfView; }
			set
			{
				if (lengthOfView != value)
				{
					lengthOfView = value;
					NovelText.SetVerticesOnlyDirty();
				}
			}
		}
		[SerializeField]
		int lengthOfView = -1;

		/// <summary>現在の表示する文字の長さ</summary>
		int CurrentLengthOfView
		{
			get { return (LengthOfView < 0) ? textData.Length : LengthOfView; }
		}

		/// <summary>
		/// //テキスト設定
		/// </summary>
		public UguiNovelTextSettings TextSettings
		{
			get { return textSettings; }
			set { textSettings = value; SetAllDirty(); }
		}
		[SerializeField]
		UguiNovelTextSettings textSettings;

		/// <summary>
		/// ルビのフォントサイズのスケール（ルビ対象の文字サイズに対しての倍率）
		/// </summary>
		public float RubySizeScale
		{
			get { return rubySizeScale; }
			set { rubySizeScale = value; SetAllDirty(); }
		}
		[SerializeField]
		float rubySizeScale = 0.5f;

		/// <summary>
		/// 上付き、下付き文字のサイズのスケール（上付き、下付き対象の文字サイズに対しての倍率）
		/// </summary>
		public float SupOrSubSizeScale
		{
			get { return supOrSubSizeScale; }
			set { supOrSubSizeScale = value; SetAllDirty(); }
		}
		[SerializeField]
		float supOrSubSizeScale = 0.5f;

		/// <summary>
		/// 絵文字のデータ
		/// </summary>
		public UguiNovelTextEmojiData EmojiData
		{
			get { return emojiData; }
			set { emojiData = value; SetAllDirty(); }
		}
		[SerializeField]
		UguiNovelTextEmojiData emojiData;

		public char DashChar { get { return (dashChar == 0) ? CharData.Dash : dashChar; } }
		[SerializeField]
		char dashChar = '—';

		public int BmpFontSize
		{
			get
			{
				if (NovelText.font != null && !NovelText.font.dynamic)
				{
					if (bmpFontSize <= 0)
					{
						Debug.LogError("bmpFontSize is zero", this);
						return 1;
					}
				}
				return bmpFontSize;
			}
		}
		[SerializeField]
		int bmpFontSize = 1;

		public bool IsUnicodeFont { get { return isUnicodeFont; } }
		[SerializeField]
		bool isUnicodeFont = false;


		RectTransform cachedRectTransform;
		public RectTransform CachedRectTransform { get { if (this.cachedRectTransform == null) cachedRectTransform = GetComponent<RectTransform>(); return cachedRectTransform; } }


		//文字のデータ
		List<UguiNovelTextCharacter> characterDataList;
		List<UguiNovelTextCharacter> CharacterDataList
		{
			get { return characterDataList; }
		}

		//行のデータ
		List<UguiNovelTextLine> lineDataList = new List<UguiNovelTextLine>();
		public List<UguiNovelTextLine> LineDataList
		{
			get { return lineDataList; }
		}

		//最後の文字の座標（右下頂点座標）
		public Vector3 EndPosition { get { return endPosition; } }
		Vector3 endPosition;

		//ルビやアンダーラインなどの不可的な描画情報
		public UguiNovelTextGeneratorAdditional Additional
		{
			get { return additional; }
		}
		UguiNovelTextGeneratorAdditional additional;


		//絵文字などのグラフィックオブジェクト
		class GraphicObject
		{
			public UguiNovelTextCharacter character;
			public RectTransform graphic;

			public GraphicObject(UguiNovelTextCharacter character, RectTransform graphic)
			{
				this.character = character;
				this.graphic = graphic;
			}
		};
		List<GraphicObject> graphicObjectList = null;
		bool isInitGraphicObjectList = false;

		//当たり判定
		public List<UguiNovelTextHitArea> HitGroupLists { get { return hitGroupLists; } }
		List<UguiNovelTextHitArea> hitGroupLists = new List<UguiNovelTextHitArea>();


		bool isDebugLog = false;

#if UNITY_EDITOR
		protected void OnValidate()
		{
			SetAllDirty();
		}
#endif
		public void SetAllDirty()
		{
			NovelText.SetAllDirty();
		}

		void OnEnable()
		{
			//これやらないとLateUpdateが間に合わないときがある
			ForceUpdate();
		}

		//内容が変化しているか
		enum ChagneType
		{
			None,
			VertexOnly,
			All,
		};
		ChagneType CurrentChangeType { get; set; }
		bool IsChangedAll { get { return CurrentChangeType == ChagneType.All; } }
		bool IsChangedVertexOnly { get { return CurrentChangeType == ChagneType.VertexOnly; } }

		public void ChangeAll()
		{
			this.preferredWidth = 0;
			CurrentChangeType = ChagneType.All;
		}

		public void SetVerticesOnlyDirty()
		{
			ChagneType last = CurrentChangeType;
			NovelText.SetVerticesDirty();
			if (last != ChagneType.All)
			{
				CurrentChangeType = ChagneType.VertexOnly;
			}
		}
		void ClearCurrentChangeType()
		{
			CurrentChangeType = ChagneType.None;
		}

		public bool IsRebuidFont { get; set; }

		//フォントの文字画像を作成リクエスト中か
		bool isRequestingCharactersInTexture = false;
		public bool IsRequestingCharactersInTexture
		{
			get { return isRequestingCharactersInTexture; }
		}

		//頂点情報を作成
		void LateUpdate()
		{
			ForceUpdate();
		}

		void ForceUpdate()
		{
			if (IsChangedAll)
			{
				ClearGraphicObjectList();
				Refresh();
				ClearCurrentChangeType();
			}

			//絵文字など子オブジェクトとして表示するグラフィックを更新
			UpdateGraphicObjectList(lineDataList);
		}

		//頂点情報を作成
		public void CreateVertex(List<UIVertex> verts)
		{
			Profiler.BeginSample("CreateVertex");

			Profiler.BeginSample("CreateVertex Init");

			//本当ならhasChangedをチェックすべきだが、
			//Textクラスのフォント変更のコールバックが継承先のクラスでは制御不能なので
			//頂点作成ごとにテキストデータを作成しなおす
			if (IsChangedAll || IsRebuidFont)
			{
				if (isDebugLog && IsRebuidFont) Debug.LogError("Refresh on CreateVertex");
				Refresh();
			}
			else
			{
				UpdateGraphicObjectList(this.lineDataList);
			}
			ClearCurrentChangeType();
			Profiler.EndSample();

			//各頂点データを構築
			if (isDebugLog) Debug.LogFormat("PosX={0}",this.lineDataList[0].Characters[0].PositionX);

			Profiler.BeginSample("MakeVerts");
			MakeVerts(verts, this.lineDataList);
			Profiler.EndSample();


			//描画用頂点データリストを作成・文字の表示長さを変更
			Profiler.BeginSample("CreateVertexList");
			CreateVertexList(verts, this.lineDataList, CurrentLengthOfView);
			Profiler.EndSample();

			//当たり判定を更新
			Profiler.BeginSample("RefreshHitArea");
			RefreshHitArea();
			Profiler.EndSample();

			Profiler.EndSample();
		}

		//頂点情報を作成
		void Refresh()
		{
			if (CheckRequestingCharactersInTextureError())
			{
				//テクスチャ内の文字をリクエストしている最中にここに来るのはエラー
				return;
			}

			Profiler.BeginSample("Refresh");

			//TextDataの更新
			//文字の大きさなどのデータを取得
			bool updateText = TryUpdateTextData(NovelText.text);
			//行データの再作成
			TryUpdateLineData(updateText);

			if (isDebugLog) Debug.Log("Refresh()");
			//テキストのアンカーを適用する
			ApplyTextAnchor(this.LineDataList, NovelText.alignment, this.MaxWidth, this.MaxHeight);
			//Offsetを適用する
			ApplyOffset(this.LineDataList, this.MaxWidth, this.MaxHeight);
			//拡張的な情報の表示位置を初期化
			Additional.InitPosition(this);
			//当たり判定の情報を作成
			MakeHitGroups(this.CharacterDataList);

			isInitGraphicObjectList = false;
			IsRebuidFont = false;
			Profiler.EndSample();
		}

		//頂点情報を作成
		void CalcPreferredWidth()
		{
			if (CheckRequestingCharactersInTextureError())
			{
				//テクスチャ内の文字をリクエストしている最中にここに来るのはエラー
				return;
			}

			Profiler.BeginSample("Refresh");

			//TextDataの更新
			//文字の大きさなどのデータを取得
			bool updateText = TryUpdateTextData(NovelText.text);
			if (updateText || this.preferredWidth == 0)
			{
				//描画範囲のサイズに合わせて自動改行
				this.preferredWidth = CalcPreferredWidth(this.CharacterDataList);
			}
			if (isDebugLog) Debug.Log("CalcPreferredWidth()");

			Profiler.EndSample();
		}

		//頂点情報を作成
		void CalcPreferredHeight()
		{
			if (CheckRequestingCharactersInTextureError())
			{
				//テクスチャ内の文字をリクエストしている最中にここに来るのはエラー
				return;
			}
			if (isDebugLog) Debug.Log("CalcPreferredHeight()");

			Profiler.BeginSample("Refresh");

			//TextDataの更新
			//文字の大きさなどのデータを取得
			bool updateText = TryUpdateTextData(NovelText.text);
			//行データの再作成
			bool updateLine = TryUpdateLineData(updateText);

			if (updateLine)
			{
				if (isDebugLog) Debug.Log("updateLine");
				//テキストのアンカーを適用する
				ApplyTextAnchor(this.LineDataList, NovelText.alignment, this.MaxWidth, this.MaxHeight);
				//Offsetを適用する
				ApplyOffset(this.LineDataList, this.MaxWidth, this.MaxHeight);
				//拡張的な情報の表示位置を初期化
				Additional.InitPosition(this);
				//当たり判定の情報を作成
				MakeHitGroups(this.CharacterDataList);
			}

			Profiler.EndSample();
		}

		bool CheckRequestingCharactersInTextureError()
		{
			//テクスチャ内の文字をリクエストしている最中にここに来るのはエラー
			if (isRequestingCharactersInTexture)
			{
				if (isDebugLog) Debug.LogError("RequestingCharactersInTexture on Refresh");
				return true;
			}
			return false;
		}

		//テキストデータの再作成
		bool TryUpdateTextData(string text)
		{
			if (CurrentChangeType != ChagneType.All && this.TextData != null && this.TextData.OriginalText == text)
			{
				return false;
			}
			else
			{
				//TextData作成
				this.textData = new TextData(text);
				if (isDebugLog) Debug.Log(this.TextData.ParsedText.OriginalText);

				//文字データを作成
				this.characterDataList = CreateCharacterDataList(this.TextData);
				//拡張的な情報を作成
				this.additional = new UguiNovelTextGeneratorAdditional(this.CharacterDataList, this);
				//フォントの文字画像を準備・設定
				InitFontCharactes(NovelText.font, this.CharacterDataList);
				//拡張的な情報の初期化
				Additional.InitAfterCharacterInfo(this);
				return true;
			}
		}

		//行データの再作成の必要性を検証
		bool TryUpdateLineData( bool updateText )
		{
			//描画範囲のサイズに合わせて自動改行
			Rect rect = CachedRectTransform.rect;
			float maxW = Mathf.Abs(rect.width);
			float maxH = Mathf.Abs(rect.height);
			if (updateText || this.lineDataList == null)
			{
				//行データを必ず再作成する必要がある
			}
			else
			{ 
				//前回と同じなら再作成必要ない
				if (Mathf.Approximately(maxW, this.MaxWidth) && Mathf.Approximately(maxH, this.MaxHeight))
				{
					return false;
				}
			}

			//文字のX座標を計算（自動改行処理も行う）
			ApplyXPosition(this.CharacterDataList, maxW);
			//行ごとの文字データを作成
			this.lineDataList = CreateLineList(this.CharacterDataList, maxH);

			//今の描画範囲を更新
			this.MaxWidth = maxW;
			this.MaxHeight = maxH;

			return true;
		}
		
		//文字データを作成
		List<UguiNovelTextCharacter> CreateCharacterDataList(TextData data)
		{
			List<UguiNovelTextCharacter> characterDataList = new List<UguiNovelTextCharacter>();
			if (data == null) return characterDataList;

			for (int i = 0; i < data.Length; i++)
			{
				UguiNovelTextCharacter character = new UguiNovelTextCharacter(data.CharList[i], this );
				characterDataList.Add(character);
			}
			return characterDataList;
		}

		//フォントの文字画像を準備・設定
		void InitFontCharactes( Font font, List<UguiNovelTextCharacter> characterDataList)
		{
			bool isComplete = false;
			//再試行回数
			int retryCount = 5;
			for (int i = 0; i < retryCount; ++i )
			{
				if (TryeSetFontCharcters(font, characterDataList))
				{
					isComplete = true;
					break;
				}
				else					
				{
					RequestCharactersInTexture(font, characterDataList);
					if (i == retryCount-1)
					{
						SetFontCharcters(font, characterDataList);
					}
				}
			}
			if (isDebugLog)
			{
				if (!isComplete)
				{
					if (isDebugLog) Debug.LogError("InitFontCharactes Error");
					TryeSetFontCharcters(font, characterDataList);
				}
			}
		}

		//フォントの文字画像の設定を試行
		bool TryeSetFontCharcters(Font font, List<UguiNovelTextCharacter> characterDataList)
		{
			foreach (UguiNovelTextCharacter character in characterDataList)
			{
				if (!character.TrySetCharacterInfo(font))
				{
					return false;
				}
			}
			return Additional.TrySetFontCharcters(font);
		}

		//フォントの文字画像を設定・エラーの場合もそのまま
		void SetFontCharcters(Font font, List<UguiNovelTextCharacter> characterDataList)
		{
			foreach (UguiNovelTextCharacter character in characterDataList)
			{
				character.SetCharacterInfo(font);
			}
		}

		//フォントの文字画像の作成リクエスト
		void RequestCharactersInTexture(Font font, List<UguiNovelTextCharacter> characterDataList)
		{
			List<RequestCharactersInfo> infoList = MakeRequestCharactersInfoList(characterDataList);
			isRequestingCharactersInTexture = true;

#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
			font.textureRebuildCallback += FontTextureRebuildCallback;
			foreach (RequestCharactersInfo info in infoList)
			{
				font.RequestCharactersInTexture(info.characters, info.size, info.style);
			}
			font.textureRebuildCallback -= FontTextureRebuildCallback;
#else
			Font.textureRebuilt += FontTextureRebuildCallback;
			foreach (RequestCharactersInfo info in infoList)
			{
				font.RequestCharactersInTexture(info.characters, info.size, info.style);
			}
			Font.textureRebuilt -= FontTextureRebuildCallback;
#endif
			isRequestingCharactersInTexture = false;
		}

		void FontTextureRebuildCallback()
		{
			if (isDebugLog) Debug.LogError("FontTextureRebuildCallback");
		}

		void FontTextureRebuildCallback( Font font )
		{
			FontTextureRebuildCallback();
		}

		//フォントの文字画像の作成のための情報を作成
		List<RequestCharactersInfo> MakeRequestCharactersInfoList(List<UguiNovelTextCharacter> characterDataList)
		{
			List<RequestCharactersInfo> infoList = new List<RequestCharactersInfo>();
			foreach( UguiNovelTextCharacter characterData in characterDataList )
			{
				AddToRequestCharactersInfoList(characterData,infoList);
			}
			List<UguiNovelTextCharacter> additionalCharacterList = Additional.MakeCharacterList();
			foreach (UguiNovelTextCharacter characterData in additionalCharacterList)
			{
				AddToRequestCharactersInfoList(characterData, infoList);
			}
			return infoList;
		}

		//フォントの文字画像の作成のための情報を作成
		void AddToRequestCharactersInfoList(UguiNovelTextCharacter characterData, List<RequestCharactersInfo> infoList)
		{
			if (characterData.IsNoFontData) return;

			foreach (RequestCharactersInfo info in infoList)
			{
				if (info.TryAddData(characterData))
				{
					return;
				}
			}
			infoList.Add(new RequestCharactersInfo(characterData));
		}

		internal class RequestCharactersInfo
		{
			public string characters;
			public readonly int size;
			public readonly FontStyle style;

			public RequestCharactersInfo(UguiNovelTextCharacter data)
			{
				characters = "" + data.Char;
				size = data.FontSize;
				style = data.FontStyle;
			}
			public bool TryAddData(UguiNovelTextCharacter data)
			{
				if (size == data.FontSize && style == data.FontStyle)
				{
					characters += data.Char;
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		//文字のX座標を計算（自動改行処理も行う）
		void ApplyXPosition(List<UguiNovelTextCharacter> characterDataList, float maxWidth)
		{
			ClacXPosition(characterDataList, true, true, maxWidth);
		}
		//自動改行なしでの幅を求める
		float CalcPreferredWidth(List<UguiNovelTextCharacter> characterDataList)
		{
			return ClacXPosition(characterDataList, false, false, float.MaxValue );
		}

		//文字のX座標を計算（自動改行処理も行う）
		float ClacXPosition(List<UguiNovelTextCharacter> characterDataList, bool autoLineBreak, bool applyX, float maxWidth )
		{
			float maxX = 0;
			float indentSize = 0;
			int index = 0;
			//infoListがテキストの文字数ぶんになるまでループ
			while (index < characterDataList.Count)
			{
				//行の開始インデックス
				int beginIndex = index;
				float currentLetterSpace = 0;	//文字間のサイズ
				float x = 0;	//現在のX位置
				//一行ぶん（改行までの）の処理をループ内で処理
				while (index < characterDataList.Count)
				{
					UguiNovelTextCharacter currentData = characterDataList[index];
					bool isAutoLineBreak = false;	//自動改行をするか

					//行の先頭で、先頭の文字スペースが必要場合があるので加算する
					if (x == 0)
					{
						currentLetterSpace = Additional.GetTopLetterSpace(currentData,this);
						x += indentSize;
						if(index==0 && IsAutoIndentation(currentData.Char))
						{
							indentSize = currentData.Width + LetterSpaceSize;
						}
					}

					//文字間の適用
					if (currentData.CustomInfo.IsRuby) currentLetterSpace += currentData.RubySpaceSize;
					x += currentLetterSpace;
					
					if (currentData.IsBlank)
					{
						//改行文字かスペース
					}
					else if(autoLineBreak)
					{
						//いったん改行データをクリア
						currentData.isAutoLineBreak = false;
						//横幅を越えるなら自動改行
						isAutoLineBreak = IsOverMaxWidth(x, Additional.GetMaxWidth(currentData), maxWidth);
						if (isAutoLineBreak)
						{
							//自動改行処理
							//改行すべき文字の位置まで戻る
							index = GetAutoLineBreakIndex(characterDataList, beginIndex, index);
							currentData = characterDataList[index];
							currentData.isAutoLineBreak = true;
						}
					}
					//1文字進める
					++index;

					bool br = (autoLineBreak && currentData.IsBrOrAutoBr) || currentData.IsBr;
					//改行処理
					if (br)
					{
						//改行なので行処理のループ終了
						break;
					}
					else
					{
						if (applyX)
						{
							currentData.InitPositionX(x);
						}
						//X位置を進める
						x += currentData.Width;
						if (currentData.RubySpaceSize != 0)
						{
							currentLetterSpace = currentData.RubySpaceSize;
						}
						else
						{
							currentLetterSpace = LetterSpaceSize;

							//文字間を無視する場合のチェック
							if (TextSettings)
							{
								if (index < characterDataList.Count)
								{
									if (TextSettings.IsIgonreLetterSpace(currentData, characterDataList[index]))
									{
										currentLetterSpace = 0;
									}
								}
							}
						}
					}
				}
				maxX = Mathf.Max(x, maxX);
			}
			return maxX;
		}


		//行データを作成する
		List<UguiNovelTextLine> CreateLineList(List<UguiNovelTextCharacter> characterDataList, float maxHeight)
		{
			//行データの作成＆Y位置の調整
			List<UguiNovelTextLine> lineList = new List<UguiNovelTextLine>();

			//行データを作成
			UguiNovelTextLine currentLine = new UguiNovelTextLine();
			foreach (UguiNovelTextCharacter character in characterDataList)
			{
				currentLine.AddCharaData(character);
				//改行処理
				if (character.IsBrOrAutoBr)
				{
					currentLine.EndCharaData(this);
					lineList.Add(currentLine);
					//次の行を追加
					currentLine = new UguiNovelTextLine();
				}
			}
			if (currentLine.Characters.Count > 0)
			{
				currentLine.EndCharaData(this);
				lineList.Add(currentLine);
			}

			if (lineList.Count <= 0) return lineList;

			float y = 0;
			//行間
			for(int i = 0; i < lineList.Count; ++i)
			{
				UguiNovelTextLine line = lineList[i];
				float y0 = y;
				y -= line.MaxFontSize;
				//縦幅のチェック
				line.IsOver = IsOverMaxHeight(-y, maxHeight);
				//表示する幅を取得
				if (!line.IsOver)
				{
					this.height = -y;
				}
				this.PreferredHeight = -y;
				//Y座標を設定
				line.SetLineY(y, this);
				//行間を更新
				y = y0 - line.TotalHeight;
			}
			return lineList;
		}

		//テキストのアンカーを適用する
		void ApplyTextAnchor( List<UguiNovelTextLine> lineList, TextAnchor anchor, float maxWidth, float maxHeight)
		{
			Vector2 pivot = Text.GetTextAnchorPivot(anchor);
			foreach (UguiNovelTextLine line in lineList)
			{
				line.ApplyTextAnchorX(pivot.x, maxWidth);
			}

			if (pivot.y == 1.0f) return;

			float offsetY = (maxHeight - Height) * (pivot.y-1.0f);
			foreach (UguiNovelTextLine line in lineList)
			{
				line.ApplyTextAnchorY(offsetY);
			}
		}

		//Offsetを適用する
		void ApplyOffset(List<UguiNovelTextLine> lineList, float maxWidth, float maxHeight)
		{
			Vector2 pivot = CachedRectTransform.pivot;
			Vector2 offset = new Vector2(-pivot.x * maxWidth, (1.0f - pivot.y) * maxHeight);
			foreach (UguiNovelTextLine line in lineList)
			{
				line.ApplyOffset(offset);
			}
			if (isDebugLog) Debug.LogFormat("PosX={0}",this.lineDataList[0].Characters[0].PositionX);
		}

		//当たり判定の情報を作成
		void MakeHitGroups(List<UguiNovelTextCharacter> characterDataList)
		{
			this.hitGroupLists = new List<UguiNovelTextHitArea>();
			int index = 0;
			//一行ぶん（改行までの）の処理をループ内で処理
			while (index < characterDataList.Count)
			{
				UguiNovelTextCharacter currentData = characterDataList[index];
				if (currentData.charData.CustomInfo.IsHitEventTop)
				{
					CharData.HitEventType type = currentData.CustomInfo.HitEventType;
					string arg = currentData.CustomInfo.HitEventArg;
					List<UguiNovelTextCharacter> characterList = new List<UguiNovelTextCharacter>();
					characterList.Add(currentData);
					++index;
					while (index < characterDataList.Count)
					{
						UguiNovelTextCharacter c = characterDataList[index];
						if (!c.CustomInfo.IsHitEvent || c.CustomInfo.IsHitEventTop) break;
						characterList.Add(c);
						++index;
					}
					hitGroupLists.Add( new UguiNovelTextHitArea( NovelText, type, arg, characterList) );
				}
				else
				{
					++index;
				}
			}
		}
		
		//当たり判定の情報を更新
		void RefreshHitArea()
		{
			foreach (var item in hitGroupLists)
			{
				item.RefreshHitAreaList();
			}
		}

		
		//各頂点データを構築
		void MakeVerts(List<UIVertex> verts, List<UguiNovelTextLine> lineList)
		{
			Color color = NovelText.color;
			foreach (UguiNovelTextLine line in lineList)
			{
				foreach (UguiNovelTextCharacter character in line.Characters)
				{
					character.MakeVerts(color,this);
				}
			}
			Additional.MakeVerts(color,this);
		}

		//描画用頂点データリストを作成
		void CreateVertexList(List<UIVertex> verts, List<UguiNovelTextLine> lineList, int max)
		{
			if (lineList == null || (max <= 0 && lineList.Count <= 0) )
			{
				return;
			}

			int count = 0;
			UguiNovelTextCharacter lastCaracter = null;
			foreach (UguiNovelTextLine line in lineList)
			{
				if (line.IsOver) break;

				for( int i = 0; i < line.Characters.Count; ++i)
				{
					UguiNovelTextCharacter character = line.Characters[i];
					character.IsVisible = (count < max);
					++count;
					if (character.IsBr) continue;
					if (character.IsVisible)
					{
						lastCaracter = character;
						if( !character.IsNoFontData )
						{
							verts.AddRange(character.Verts);
						}
						endPosition = new Vector3( lastCaracter.EndPositionX, line.Y0, 0 );
					}
				}
			}

			Additional.AddVerts(verts, endPosition,this);
		}

		//最後の座標を計算
		internal void RefreshEndPosition()
		{
			int max = CurrentLengthOfView;
			if (LineDataList == null || (max <= 0 && LineDataList.Count <= 0))
			{
				return;
			}

			int count = 0;
			UguiNovelTextCharacter lastCaracter = null;
			foreach (UguiNovelTextLine line in LineDataList)
			{
				if (line.IsOver) break;

				for (int i = 0; i < line.Characters.Count; ++i)
				{
					UguiNovelTextCharacter character = line.Characters[i];
					character.IsVisible = (count < max);
					++count;
					if (character.IsBr) continue;
					if (character.IsVisible)
					{
						lastCaracter = character;
						endPosition = new Vector3(lastCaracter.EndPositionX, line.Y0, 0);
					}
				}
			}
		}


		//絵文字などのグラフィックオブジェクトを全て削除
		void ClearGraphicObjectList()
		{
			if (graphicObjectList != null)
			{
				foreach (GraphicObject graphic in graphicObjectList)
				{
					if (Application.isPlaying)
					{
						GameObject.Destroy(graphic.graphic.gameObject);
					}
					else
					{
						GameObject.DestroyImmediate(graphic.graphic.gameObject);
					}
				}
				graphicObjectList.Clear();
				graphicObjectList = null;
				isInitGraphicObjectList = false;
			}
		}

		//絵文字など子オブジェクトとして表示するグラフィックを作成
		void UpdateGraphicObjectList(List<UguiNovelTextLine> lineList)
		{
			//絵文字など子オブジェクトとして表示するグラフィックを作成
			if (!isInitGraphicObjectList)
			{
				ClearGraphicObjectList();
				graphicObjectList = new List<GraphicObject>();

				foreach (UguiNovelTextLine line in lineList)
				{
					foreach (UguiNovelTextCharacter character in line.Characters)
					{
						RectTransform graphicObjecct = character.AddGraphicObject(CachedRectTransform, this);
						if (graphicObjecct)
						{
							graphicObjectList.Add(new GraphicObject(character, graphicObjecct));
						}
					}
				}
				isInitGraphicObjectList = true;
			}

			foreach (GraphicObject graphicObject in graphicObjectList)
			{
				graphicObject.graphic.gameObject.SetActive(graphicObject.character.IsVisible);
			}
		}

		//以下、自動改行に必要な細かい処理

		//自動改行
		//禁則などで送り出しされる文字がある場合は、適切な改行の文字インデックスを返す
		int GetAutoLineBreakIndex(List<UguiNovelTextCharacter> characterList, int beginIndex, int index)
		{
			if (index <= beginIndex) return index;

			UguiNovelTextCharacter current = characterList[index];	//はみ出た文字
			UguiNovelTextCharacter prev = characterList[index-1];	//一つ前の文字（改行文字候補）

			if (prev.IsBrOrAutoBr)
			{
				//前の文字が改行の場合、そのまま現在の文字を改行文字にする
				return index;
			}
			else if (CheckWordWrap(current, prev))
			{
				//禁則処理
				//改行可能な位置まで文字インデックスを戻す
				int i = ParseWordWrap(characterList, beginIndex, index-1);
				if (i != beginIndex)
				{
					return i;
				}
				else
				{
					//前の文字を自動改行
					return --index;
				}
			}
			else
			{
				//前の文字を自動改行
				return --index;
			}
		}


		//WordWrap処理
		int ParseWordWrap(List<UguiNovelTextCharacter> infoList, int beginIndex, int index)
		{
			if (index <= beginIndex) return beginIndex;

			UguiNovelTextCharacter current = infoList[index];	//改行させる文字
			UguiNovelTextCharacter prev = infoList[index - 1];	//一つ前の文字

			if (CheckWordWrap(current, prev))
			{	//禁則に引っかかるので、一文字前をチェック
				return ParseWordWrap(infoList, beginIndex, index - 1);
			}
			else
			{
				return index - 1;
			}
		}

		//禁則のチェック
		bool CheckWordWrap(UguiNovelTextCharacter current, UguiNovelTextCharacter prev)
		{
			//ルビは開始の文字以外は改行できない
			if (current.IsDisableAutoLineBreak)
			{
				return true;
			}

			if( TextSettings !=null )
			{
				return TextSettings.CheckWordWrap(this,current, prev);
			}
			else
			{
				return false;
			}
		}

		//最大横幅のチェック
		bool IsOverMaxWidth(float x, float width, float maxWidth)
		{
			return (x > 0) && (x + width) > maxWidth;
		}

		//最大縦幅のチェック
		bool IsOverMaxHeight(float height, float maxHeight)
		{
			return height > maxHeight;
		}

		bool IsAutoIndentation(char character)
		{
			if (TextSettings != null) {
				return  TextSettings.IsAutoIndentation(character);
			} else {
				return false;
			}
		}

#if UNITY_EDITOR
		//文字が範囲外かどうかのチェック
		public bool EditorCheckRect(string text, out int len, out string errorString)
		{
			this.NovelText.text = text;
			this.ChangeAll();
			ForceUpdate ();
			errorString = "";
			bool isOver = false;
			foreach (var item in this.lineDataList) 
			{
				if( item.IsOver)
				{
					isOver = true;
					break;
				}
			}

			if(isOver)
			{
				System.Text.StringBuilder normalText = new System.Text.StringBuilder();
				System.Text.StringBuilder overedText = new System.Text.StringBuilder();
				System.Text.StringBuilder builder = normalText;
				int overLineCount = 0;
				foreach (var line in this.lineDataList)
				{
					if(line.IsOver)
					{
						builder = overedText;
						++overLineCount;
						if (overLineCount > 10)
						{
							builder.AppendLine("...");
							break;
						}
					}
					foreach( var c in line.Characters )
					{
						builder.Append(c.Char);
						if(c.isAutoLineBreak)
						{
							builder.AppendLine();
						}
					}
				}
				errorString += normalText.ToString() + TextParser.AddTag(overedText.ToString(), "color", "red");
			}

			len = this.textData.Length;
			return !isOver;
		}
#endif

	}
}
