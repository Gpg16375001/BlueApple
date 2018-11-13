using PhotoshopFile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using SmileLab.UI;
using SmileLab;

public class PSDEditorWindow : EditorWindow
{
    private const string vers = "ver. Smile-Lab 0.1 based ver. 1.9";
    private Font font;
    private TMP_FontAsset fontForTmp;
    private Texture2D image;
    private string imageAssetPath;
    private Vector2 scrollPos;
    private PsdFile psd;
    private int atlassize = 4096;
    private float pixelsToUnitSize = 100.0f;

    private string fileName;
    private string SearchBasePath;
    [SerializeField]
    private List<string> SearchFolders = new List<string> ();
    private List<string> LayerList = new List<string> ();
    private bool ShowAtlas;
    private GameObject CanvasObj;
    private string PackingTag;

    private Vector2 defaultPivot;

    private LayerTreeView objectTreeView;


    private int psdWidth {
        get {
            return (int)psd.BaseLayer.Rect.width;
        }
    }

    private int psdHeight {
        get {
            return (int)psd.BaseLayer.Rect.height;
        }
    }

    #region LayerPrefix

    // Layer名
    // フォルダにオブジェクトを示す接頭子が付いていた場合はフォルダ全体を一つのオブジェクトとして扱う。
    const string Prefix_Button = "bt";
    const string Prefix_TextMeshPro = "txtp";
    const string Prefix_Text = "txt";
    const string Prefix_Image = "img";

    const string Prefix_Exclude = "#";

    private static readonly string[] PrefixKeywords = new string[] {
        Prefix_Button,
        Prefix_TextMeshPro,
        Prefix_Text,
        Prefix_Image,
        Prefix_Exclude,
    };
    #endregion

    const char Split_ObjectPath = '/';
    const char Split_LayeraName = '_';

    #region LayerSuffix

    // anchor設定用
    const string Suffix_AnchorLeft = "left";
    const string Suffix_AnchorRight = "right";
    const string Suffix_AnchorTop = "top";
    const string Suffix_AnchorBottom = "bottom";

    // ボタンオブジェクト
    const string Suffix_Highlight = "highlight";
    const string Suffix_Disable = "disable";
    const string Suffix_Touched = "pressed";

    private static readonly string[] SuffixSelectableKeywords = new string[] {
        Suffix_Highlight,
        Suffix_Disable,
        Suffix_Touched
    };

    private static readonly string[] SuffixAnchorKeywords = new string[] {
        Suffix_AnchorLeft,
        Suffix_AnchorRight,
        Suffix_AnchorTop,
        Suffix_AnchorBottom,        
    };

    private static readonly string[] SuffixKeywords = new string[] {
        Suffix_AnchorLeft,
        Suffix_AnchorRight,
        Suffix_AnchorTop,
        Suffix_AnchorBottom,
        Suffix_Highlight,
        Suffix_Disable,
        Suffix_Touched,
    };
    #endregion

    #region MenuItems

    [MenuItem ("Window/uGUI/PSD Converter")]
    public static void ShowWindow ()
    {
        var wnd = GetWindow<PSDEditorWindow> ();

        wnd.minSize = new Vector2 (400, 300);
        wnd.Show ();
    }

    [MenuItem ("Assets/Convert to uGUI", true, 20000)]
    private static bool saveLayersEnabled ()
    {
        for (var i = 0; i < Selection.objects.Length; i++) {
            var obj = Selection.objects [i];
            var filePath = AssetDatabase.GetAssetPath (obj);
            if (filePath.EndsWith (".psd", StringComparison.CurrentCultureIgnoreCase)) {
                return true;
            }
        }

        return false;
    }

    [MenuItem ("Assets/Convert to uGUI", false, 20000)]
    private static void saveLayers ()
    {
        var obj = Selection.objects [0];

        var window = GetWindow<PSDEditorWindow> (true, "PSD to uGUI " + vers);
        window.minSize = new Vector2 (400, 300);
        window.image = (Texture2D)obj;
        window.LoadInformation (window.image);
        window.Show ();
    }

    #endregion

    public void OnEnable ()
    {
        var temppath = AssetDatabase.GetAssetPath (MonoScript.FromScriptableObject (this));
        var temp = temppath.Split (Split_ObjectPath).ToList ();

        temp.Remove (temp [temp.Count - 1]);
        temp.Remove (temp [temp.Count - 1]);
        temppath = "";
        foreach (var item in temp) {
            temppath += (item + "/");
        }
        titleContent.image = AssetDatabase.LoadAssetAtPath<Texture> (temppath + "Logo/logo.png");
        titleContent.text = "Parser";
        defaultPivot = new Vector2 (0.5f, 0.5f);

        objectTreeView = new LayerTreeView ();

        if (font == null) {
            var gui = EditorUserSettings.GetConfigValue ("PsdToUGUI_Font_Setting");
            if (!string.IsNullOrEmpty (gui)) {
                var path = AssetDatabase.GUIDToAssetPath (gui);
                font = AssetDatabase.LoadAssetAtPath<Font> (path);
            }
        }
        if (fontForTmp == null) {
            var gui = EditorUserSettings.GetConfigValue ("PsdToUGUI_FontForTMP_Setting");
            if (!string.IsNullOrEmpty (gui)) {
                var path = AssetDatabase.GUIDToAssetPath (gui);
                fontForTmp = AssetDatabase.LoadAssetAtPath<TMP_FontAsset> (path);
            }
        }

        SearchBasePath = EditorUserSettings.GetConfigValue ("PsdToUGUI_SearchBasePath");
        SearchFolders = EditorUserSettings.GetConfigValue ("PsdToUGUI_SearchFolders").Split(',').ToList();

        if (image != null && psd == null) {
            LoadInformation (image);
        }
    }

    public void OnGUI ()
    {
        GUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        GUILayout.Label ("PSD to UGUI " + vers);
        GUILayout.FlexibleSpace ();
        GUILayout.EndHorizontal ();

        CanvasObj = (GameObject)EditorGUILayout.ObjectField ("Root Object", CanvasObj, typeof(GameObject), true);
        EditorGUILayout.Space ();

        GUILayout.Label ("Select default font for UI");
        EditorGUILayout.Space ();
        Font prevFont = font;
        font = (Font)EditorGUILayout.ObjectField ("Font", font, typeof(Font), true);
        if (font != prevFont) {
            var path = AssetDatabase.GetAssetPath (font);
            var gui = AssetDatabase.AssetPathToGUID (path);
            EditorUserSettings.SetConfigValue ("PsdToUGUI_Font_Setting", gui);
        }
        TMP_FontAsset prevFontForTmp = fontForTmp;
        fontForTmp = (TMP_FontAsset)EditorGUILayout.ObjectField ("TextMeshPro Font", fontForTmp, typeof(TMP_FontAsset), true);
        if (fontForTmp != prevFontForTmp) {
            var path = AssetDatabase.GetAssetPath (fontForTmp);
            var gui = AssetDatabase.AssetPathToGUID (path);
            EditorUserSettings.SetConfigValue ("PsdToUGUI_FontForTMP_Setting", gui);
        }
        EditorGUILayout.Space ();


        GUILayout.Label ("Packing Tag");
        PackingTag = EditorGUILayout.TextArea (PackingTag);
        EditorGUILayout.Space ();

        defaultPivot = EditorGUILayout.Vector2Field ("Set default pivot", defaultPivot);

        EditorGUILayout.Space ();

        GUILayout.Label ("Search Texture Folders");
        EditorGUI.BeginChangeCheck ();
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Search Base Path");
        SearchBasePath = EditorGUILayout.TextArea (SearchBasePath);
        EditorGUILayout.EndHorizontal ();
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        // 上のプロパティを文字列で指定する.
        SerializedProperty stringsProperty = so.FindProperty("SearchFolders");
        // trueは子の表示.
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
        var searchSettingChanged = EditorGUI.EndChangeCheck ();
        if (searchSettingChanged) {
            EditorUserSettings.SetConfigValue ("PsdToUGUI_SearchBasePath", SearchBasePath);
            EditorUserSettings.SetConfigValue ("PsdToUGUI_SearchFolders", string.Join(",", SearchFolders.ToArray()));
        }

        EditorGUILayout.Space ();

        EditorGUI.BeginChangeCheck ();
        image = (Texture2D)EditorGUILayout.ObjectField ("PSD File", image, typeof(Texture2D), true);
        EditorGUILayout.Space ();
        var changed = EditorGUI.EndChangeCheck ();

        if (image != null) {
            if (changed) {
                imageAssetPath = AssetDatabase.GetAssetPath (image);
                if (imageAssetPath.ToUpper ().EndsWith (".PSD", StringComparison.CurrentCultureIgnoreCase)) {
                    LoadInformation (image);
                } else {
                    psd = null;
                }
            }

            if (font == null) {
                EditorGUILayout.HelpBox ("Choose the font of text layers.", MessageType.Error);
            }
            if (fontForTmp == null) {
                EditorGUILayout.HelpBox ("Choose the TextMeshPro font of text layers.", MessageType.Error);
            }
            if (psd != null) {
                scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

                objectTreeView.ShowGUI ();

                EditorGUILayout.BeginHorizontal ();
                if (GUILayout.Button ("Select All", GUILayout.Width (200))) {
                    foreach (Layer layer in psd.Layers) {
                        if (!layer.IsExclude) {
                            layer.IsOutput = true;
                        }

                    }

                }
                if (GUILayout.Button ("Select None", GUILayout.Width (200))) {
                    foreach (Layer layer in psd.Layers) {
                        if (!layer.IsExclude) {
                            layer.IsOutput = false;
                        }
                    }

                }
                EditorGUILayout.EndHorizontal ();
                EditorGUILayout.EndScrollView ();

                if(GUILayout.Button ("Create PngFile")) {
                    ProcessToPng(imageAssetPath);
                }
/*
                if (GUILayout.Button ("Create atlas && GUI ")) {
                    ShowAtlas = !ShowAtlas;
                }
                if (ShowAtlas) {
                    atlassize = EditorGUILayout.IntField ("Max. atlas size", atlassize);

                    if (!((atlassize != 0) && ((atlassize & (atlassize - 1)) == 0))) {
                        EditorGUILayout.HelpBox ("Atlas size should be a power of 2", MessageType.Warning);
                    }

                    pixelsToUnitSize = EditorGUILayout.FloatField ("Pixels To Unit Size", pixelsToUnitSize);

                    if (pixelsToUnitSize <= 0) {
                        EditorGUILayout.HelpBox ("Pixels To Unit Size should be greater than 0.", MessageType.Warning);
                    }
                    if (GUILayout.Button ("Start")) {
                        CreateAtlas();
                    }
                }
*/
                if (GUILayout.Button ("Create Import Sprites && GUI")) {
                    ExportLayers();
                }
            } else {
                EditorGUILayout.HelpBox ("This texture is not a PSD file.", MessageType.Error);
            }
        }
    }

    // 指定レイヤーに対応するデータをロードしTexture2Dとして返す。
    private Texture2D LoadTexture(Layer layer)
    {
        var path = AssetDatabase.GetAssetPath (image);
        var dirPath = Path.Combine (Path.GetDirectoryName (path), Path.GetFileNameWithoutExtension (path));

        var filePath = Path.Combine (dirPath, string.Format ("{0}.png", GetTextureName(layer)));

        Texture2D texture = new Texture2D (2, 2);
        texture.LoadImage (File.ReadAllBytes(filePath));
        texture.Apply (false, false);
        return texture;
    }

    // atlasを用いての作成
    private void CreateAtlas ()
    {
        var textures = new List<Texture2D> ();
        var spriteRenderers = new List<SpriteRenderer> ();
        LayerList = new List<string> ();
        int zOrder = 0;
        var root = new GameObject (fileName);
        foreach (var layer in psd.Layers) {
            if (HasTexture(layer)) {
                if (LayerList.IndexOf (layer.Name) == -1) {
                    LayerList.Add (GetObjectName(layer));
                    var tex = LoadTexture (layer);
                    textures.Add (tex);
                    var go = new GameObject (GetObjectName(layer));
                    var sr = go.AddComponent<SpriteRenderer> ();
                    go.transform.localPosition = new Vector3 ((layer.Rect.width / 2 + layer.Rect.x) / pixelsToUnitSize,
                        (-layer.Rect.height / 2 - layer.Rect.y) / pixelsToUnitSize, 0);
                    spriteRenderers.Add (sr);
                    sr.sortingOrder = zOrder++;
                    go.transform.parent = root.transform;
                }
            }
        }
        Rect[] rects;
        var atlas = new Texture2D (atlassize, atlassize);
        var textureArray = textures.ToArray ();
        rects = atlas.PackTextures (textureArray, 2, atlassize);
        var Sprites = new List<SpriteMetaData> ();
        for (int i = 0; i < rects.Length; i++) {
            var smd = new SpriteMetaData ();
            smd.name = spriteRenderers [i].name;
            smd.rect = new Rect (rects [i].xMin * atlas.width,
                rects [i].yMin * atlas.height,
                rects [i].width * atlas.width,
                rects [i].height * atlas.height);
            smd.pivot = new Vector2 (0.5f, 0.5f);
            smd.alignment = (int)SpriteAlignment.Center;
            Sprites.Add (smd);
        }

        // Need to load the image first
        var assetPath = AssetDatabase.GetAssetPath (image);
        var path = Path.Combine (Path.GetDirectoryName (assetPath), string.Format ("{0}_atlas.png", Path.GetFileNameWithoutExtension (assetPath)));

        var buf = atlas.EncodeToPNG ();
        File.WriteAllBytes (path, buf);
        AssetDatabase.Refresh ();

        // Get our texture that we loaded
        atlas = (Texture2D)AssetDatabase.LoadAssetAtPath (path, typeof(Texture2D));
        var textureImporter = AssetImporter.GetAtPath (path) as TextureImporter;
        // Make sure the size is the same as our atlas then create the spritesheet
        textureImporter.maxTextureSize = atlassize;
        textureImporter.spritesheet = Sprites.ToArray ();
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePivot = new Vector2 (0.5f, 0.5f);
        textureImporter.spritePixelsPerUnit = pixelsToUnitSize;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed; // obsolute form "textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;"
        AssetDatabase.ImportAsset (path, ImportAssetOptions.Default);
        foreach (Texture2D tex in textureArray) {
            DestroyImmediate (tex);
        }
        AssetDatabase.Refresh ();
        DestroyImmediate (root);

        var atlases = AssetDatabase.LoadAllAssetsAtPath (path).Select (x => x as Sprite).Where (x => x != null).ToArray ();
        CreateGUI (atlases);
    }

    // レイヤーに対応したspriteをロードしインポート設定をし直す。
    private Sprite LoadSprite(Layer layer)
    {
        // PSDと同名のフォルダ内を探す
        var path = AssetDatabase.GetAssetPath (image);
        Debug.Log (path);
        var dirPath = Path.Combine (Path.GetDirectoryName (path), Path.GetFileNameWithoutExtension (path));
        var filePath = Path.Combine (dirPath, string.Format ("{0}.png", GetTextureName(layer)));
        TextureImporter textureImporter = AssetImporter.GetAtPath (filePath) as TextureImporter;
        if (textureImporter == null) {
            // 見つからなかったら検索フォルダ内を検索する。
            foreach (var folderName in SearchFolders) {
                dirPath = Path.Combine (SearchBasePath, folderName);
                filePath = Path.Combine (dirPath, string.Format ("{0}.png", GetTextureName(layer)));
                textureImporter = AssetImporter.GetAtPath (filePath) as TextureImporter;
                if(textureImporter != null) break;
            }
            if(textureImporter == null) {
                Debug.LogError (string.Format ("レイヤー:{1} {0}が見つかりませんでした。", filePath, layer.Name));
                throw new FileNotFoundException ();
            }
        }
        if (textureImporter.textureType != TextureImporterType.Sprite) {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.maxTextureSize = atlassize;
            if (!System.String.IsNullOrEmpty (PackingTag)) {
                textureImporter.spritePackingTag = PackingTag;
            }

            // obsolute form "textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;"
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset (filePath, ImportAssetOptions.ForceUpdate);
        }
        return (Sprite)AssetDatabase.LoadAssetAtPath (filePath, typeof(Sprite));
    }

    // borderが設定されているものは9スライスとして扱う
    private bool Is9SliceObject(Sprite sprite)
    {
        return sprite.border != Vector4.zero;
    }

    // 個別スプライトでのデータ出力
    private void ExportLayers ()
    {
        createdObjectPath.Clear ();
        cachedFolderRect.Clear ();
        LayerList = new List<string> ();
        var atlas = new List<Sprite> ();
        foreach (var layer in psd.Layers) {
            if (HasTexture(layer)) {
                var tex = LoadSprite (layer);
                if (tex == null) {
                    continue;
                }
                atlas.Add (tex);
            }
        }
        CreateGUI (atlas.ToArray ());
    }

    private void CreateGUI (Sprite[] atlas)
    {
        int currentid = 0;
        int outputCount = psd.Layers.Count (x => IsCreateObject(x));
        _atlas = atlas;
        try {

            List<string> psdTextList = new List<string>();
            // CanvasObjの指定がない場合は作成する。
            if (CanvasObj == null) {
                CanvasObj = new GameObject ();
                CanvasObj.name = "Canvas";
                CanvasObj.AddComponent<Canvas> ();
                Debug.Log ("Created new Canvas");
            }

            // 
            var canvas = CanvasObj.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var canvasScaler = CanvasObj.GetOrAddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(psdWidth, psdHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            canvasScaler.enabled = false;

            CanvasObj.GetOrAddComponent<GraphicRaycaster>();

            // RectTransformを再定義しておく
            var rectTransform = CanvasObj.GetOrAddComponent<RectTransform> ();
            rectTransform.pivot = new Vector2 (0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2 (0.5f, 0.5f);
            rectTransform.localPosition = new Vector3 (0, 0, 0);
            rectTransform.sizeDelta = new Vector2 (psdWidth, psdHeight);
            rectTransform.localScale = Vector3.one;

            var layerCount = psd.Layers.Count;
            for (int i = 0; i < layerCount; i++) {
                var layer = psd.Layers [i];
                if (IsCreateObject(layer)) {
                    currentid++;
                    var Prefix = GetPrefixName(layer);
                    var Suffixs = GetSuffixNames(layer);
                    var RealLayerName = GetObjectName(layer);
                    var TextureName = GetTextureName(layer);

                    string infoString = string.Format ("Exporting {0} / {1} Layers", currentid, outputCount);
                    string fileString = string.Format ("Exporting PSD Layer: {0}", RealLayerName);
                    EditorUtility.DisplayProgressBar (fileString, infoString, currentid / outputCount);
                    {
                        var instant = CreatePanel (layer.ObjectPath);
                        instant.name = RealLayerName;
                        instant.SetActive (false);

                        var typeToolInfo = (LayerTypeToolInfo)layer.AdditionalInfo.SingleOrDefault (x => x is LayerTypeToolInfo);

                        var rect = GetTargetRect(layer);
                        if(IsFolderObject(layer)) {
                            rect = GetInclusionRect(layer);
                        }
                        if(rect.xMin < 0) rect.xMin = 0;
                        if(rect.yMin < 0) rect.yMin = 0;
                        if(rect.xMax > psdWidth) rect.xMax = psdWidth;
                        if(rect.yMax > psdHeight) rect.yMax = psdHeight;

                        // テキストの設定
                        if (typeToolInfo != null && (Prefix == Prefix_Text || Prefix == Prefix_TextMeshPro)) {
                            var bounds = (DescriptorStructure)typeToolInfo.TextData["bounds"];

                            //　デバッグ時用にコメントで残しておく
                            //System.IO.File.WriteAllText(string.Format("{0}.txt", RealLayerName), Encoding.UTF8.GetString(((RowData)typeToolInfo.TextData["EngineData"]).Data));

                            var dat = (Hashtable)EngineDataParser.Parse(((RowData)typeToolInfo.TextData["EngineData"]).Data);

                            ArrayList paragraphDataArray = ((ArrayList)((Hashtable)((Hashtable)dat["EngineDict"])["ParagraphRun"])["RunArray"]);
                            if(paragraphDataArray.Count > 1) {
                                Debug.LogWarning(string.Format("レイヤー名:{0}にて段落情報が複数見つかりました。レイアウトなどおかしい部分があった場合は正しい設定にした上でテキストの打ち直しをしてみてください。", RealLayerName));
                            }
                            var paragraphData = ((Hashtable)((Hashtable)((Hashtable)(paragraphDataArray[0]))["ParagraphSheet"])["Properties"]);
                            var alignmentSetting = (int)((double)(paragraphData["Justification"]));
                            ArrayList styleDataArray = ((ArrayList)((Hashtable)((Hashtable)dat["EngineDict"])["StyleRun"])["RunArray"]);
                            if(styleDataArray.Count > 1) {
                                Debug.LogWarning(string.Format("レイヤー名:{0}にてスタイル情報が複数見つかりました。レイアウトなどおかしい部分があった場合は正しい設定にした上でテキストの打ち直しをしてみてください。", RealLayerName));
                            }
                            var styleData = ((Hashtable)((Hashtable)((Hashtable)(styleDataArray[0]))["StyleSheet"])["StyleSheetData"]);

                            var fontSize = (float)(double)(styleData["FontSize"]);
                            ArrayList fillColor = null;
                            if(styleData.ContainsKey("FillColor")) {
                                fillColor = (ArrayList)(((Hashtable)(styleData["FillColor"]))["Values"]);
                            }
                            float? leading = null;
                            if(styleData.ContainsKey("Leading")) {
                                leading = (float)(double)(styleData["Leading"]);
                            }

                            string text_data = ((StringStructure)typeToolInfo.TextData["Txt "]).Text.Replace("\r\n", "\r").Replace('\r', '\n');
                            // お尻に終端コードが入っているようなので削除する
                            text_data = text_data.TrimEnd((char)0);
                            psdTextList.Add(text_data);
                            if (Prefix == Prefix_TextMeshPro) {
                                var text = instant.GetOrAddComponent<TextMeshProUGUI> ();
                                text.font = fontForTmp;
                                text.SetText (text_data);
                                if(fillColor != null) {
                                    text.color = ArrayListToColor(fillColor);
                                }
                                text.fontSize = fontSize;
                                if(leading.HasValue) {
                                    text.lineSpacing = (leading.Value - fontForTmp.fontInfo.LineHeight) * (fontSize / fontForTmp.fontInfo.PointSize) ;
                                }
                                if(alignmentSetting == 0) {
                                    text.alignment = TextAlignmentOptions.MidlineLeft;
                                } else if(alignmentSetting == 1) {
                                    text.alignment = TextAlignmentOptions.MidlineRight;
                                } else {
                                    text.alignment = TextAlignmentOptions.Midline;
                                }
                                instant.GetComponent<RectTransform> ().sizeDelta = BoundsToSize(bounds);
                                text.raycastTarget = false;
                            } else {
                                var text = instant.GetOrAddComponent<Text> ();
                                text.text = text_data;
                                if(fillColor != null) {
                                    text.color = ArrayListToColor(fillColor);
                                }
                                text.font = font;
                                text.fontSize = (int)fontSize;
                                if(leading.HasValue) {
                                    text.lineSpacing = -1.0f * (font.lineHeight - leading.Value);
                                }
                                if(alignmentSetting == 0) {
                                    text.alignment = TextAnchor.MiddleLeft;
                                } else if(alignmentSetting == 1) {
                                    text.alignment = TextAnchor.MiddleRight;
                                } else {
                                    text.alignment = TextAnchor.MiddleCenter;
                                }
                                instant.GetComponent<RectTransform> ().sizeDelta = BoundsToSize(bounds);
                                text.resizeTextForBestFit = true;
                                text.raycastTarget = false;
                            }
                        } else {
                            var img = instant.GetOrAddComponent<Image> ();
                            img.sprite = GetSprite(TextureName);
                            if(!Is9SliceObject(img.sprite)) {
                                img.SetNativeSize ();
                            } else {
                                img.type = Image.Type.Sliced;
                                var rectTrans = instant.GetComponent<RectTransform> ();
                                rectTrans.sizeDelta = new Vector2(rect.width, rect.height);
                            }
                            img.raycastTarget = false;
                        }

                        instant.SetActive (true);
                        instant.GetComponent<RectTransform> ().anchorMin = Vector2.zero;
                        instant.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
                        instant.GetComponent<RectTransform> ().pivot = new Vector2(0.5f, 0.5f);
                        instant.GetComponent<RectTransform> ().anchorMax = Vector2.zero;

                        instant.transform.position = new Vector3 (rect.center.x - psdWidth / 2, psdHeight / 2 - rect.center.y, 0);
						
                        if (Prefix == Prefix_Button) {
                            var img = instant.GetOrAddComponent<Image> ();
                            img.raycastTarget = true;
                            instant.name = RealLayerName;

                            bool isPlayClickSe = false;
                            SoundClipName clickSe = SoundClipName.select;

                            // ボタンサウンドを削除し設定されている情報をCustomButtonに引き継ぐ
                            var btnSound = instant.GetComponent<ButtonSound>();
                            if(btnSound != null) {
                                isPlayClickSe = true;
                                clickSe = btnSound.GetSoundClip();
                                GameObject.DestroyImmediate(btnSound);
                            }

                            bool isAddForceSelected = false;
                            var forceSelected = instant.GetComponent<ForceSelected>();
                            if(forceSelected != null) {
                                isAddForceSelected = true;
                                GameObject.DestroyImmediate(forceSelected);
                            }

                            // 古いボタンオブジェクトが付いていたら削除する。
                            bool isOldButton = false;
                            var btn = instant.GetComponent<Button>();
                            if(btn != null) {
                                isOldButton = true;
                                GameObject.DestroyImmediate(btn);
                            }
                            var customButton = instant.GetOrAddComponent<CustomButton>();
                            // enabled
                            customButton.enabled = true;
                            customButton.transition = Selectable.Transition.SpriteSwap;

                            if(isPlayClickSe) {
                                customButton.SetClickSe(clickSe);
                            }
                            if(isOldButton && isAddForceSelected) {
                                instant.GetOrAddComponent<ForceSelected>();
                            }

                            // ボタンステータスの設定
                            var State = customButton.spriteState;
                            var pressed = psd.Layers.SingleOrDefault(x => {
                                var name = GetObjectName(x);
                                if(x.ObjectPath.StartsWith(layer.ObjectPath) && name != RealLayerName && GetExcludeSuffixName(x) == RealLayerName) {
                                    var suffixs = GetSuffixNames(x);
                                    return  suffixs.Contains(Suffix_Touched);
                                }
                                return false;
                            });
                            if(pressed != null) {
                                var pressedObjectName = GetTextureName(pressed);
                                State.pressedSprite = GetSprite(pressedObjectName);
                            }

                            var highlight = psd.Layers.SingleOrDefault(x => {
                                var name = GetObjectName(x);
                                if(x.ObjectPath.StartsWith(layer.ObjectPath) && name != RealLayerName && GetExcludeSuffixName(x) == RealLayerName) {
                                    var suffixs = GetSuffixNames(x);
                                    return  suffixs.Contains(Suffix_Highlight);
                                }
                                return false;
                            });
                            if(highlight != null) {
                                var highlightObjectName = GetTextureName(highlight);
                                State.highlightedSprite = GetSprite(highlightObjectName);
                            }

                            var disable = psd.Layers.SingleOrDefault(x => {
                                var name = GetObjectName(x);
                                if(x.ObjectPath.StartsWith(layer.ObjectPath) && name != RealLayerName && GetExcludeSuffixName(x) == RealLayerName) {
                                    var suffixs = GetSuffixNames(x);
                                    return  suffixs.Contains(Suffix_Disable);
                                }
                                return false;
                            });
                            if(disable != null) {
                                var disableObjectName = GetTextureName(disable);
                                State.disabledSprite = GetSprite(disableObjectName);
                            }
                            instant.GetComponent<Selectable> ().spriteState = State;
                        }

                        bool isLeft = Suffixs.Contains(Suffix_AnchorLeft);
                        bool isRight = Suffixs.Contains(Suffix_AnchorRight);
                        bool isTop = Suffixs.Contains(Suffix_AnchorTop);
                        bool isBottom = Suffixs.Contains(Suffix_AnchorBottom);

                        SetPivot (instant.GetComponent<RectTransform> (), isLeft, isRight, isTop, isBottom);
                        SetAnchor (instant.GetComponent<RectTransform> (), isLeft, isRight, isTop, isBottom);
                    }
                }
            }

            // オブジェクトの並び替えを行う。
            // レイヤー構成が変わった時の対応
            // ただしレイヤーにないオブジェクトは変更をかけないようにする。
            SortChildren(CanvasObj.transform as RectTransform);

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvasScaler.enabled = true;

            // PSD内のテキスト情報を出力
            // TextMeshProのFont更新用に使う
            if(psdTextList.Count > 0) {
                var path = AssetDatabase.GetAssetPath (image);
                System.IO.File.WriteAllText(
                    Path.Combine(
                        Application.dataPath, 
                        string.Format(
                            "Font/IncludeCharacters/FromPSD/{0}.txt",
                            Path.GetFileNameWithoutExtension (path)
                        )
                    ),
                    string.Join("", psdTextList.ToArray())
                );
                AssetDatabase.Refresh();
            }        
        } catch (Exception e) {
            Debug.LogException (e);
        } finally {
            _atlas = null;
            EditorUtility.ClearProgressBar ();
        }
    }

    private Sprite[] _atlas;
    private Sprite GetSprite(string name)
    {
        if (_atlas == null)
            return null;
        
        int index = Array.FindIndex (_atlas, x => x.name == name);
        if (index < 0) {
            throw new System.IndexOutOfRangeException (string.Format("Not Found Atlas Name = {0}", name));
        }
        return _atlas[index];
    }

    // オブジェクトの並び替えを行う。
    // レイヤー構成が変わった時の対応
    // ただしレイヤーにないオブジェクトは変更をかけないようにする。
    private void SortChildren(RectTransform root)
    {
        if (root == null) {
            return;
        }
        var count = root.childCount;
        List<Transform> children = new List<Transform> ();
        for (int i = 0; i < count; ++i) {
            var trans = root.GetChild (i) as RectTransform;
            if (trans != null) {
                children.Add (trans);
                if (trans.childCount > 0) {
                    SortChildren (trans);
                }
            }
        }
            
        var layerIndecies = children.Select (x => {
            var objectPath = GetObjectPath(x.gameObject);

            return psd.Layers.FindIndex( layer => {
                return objectPath == layer.ObjectPath;
            });
        }).ToArray();

        bool change = false;
        var childrenCount = children.Count;
        do {
            change = false;
            for (int i = 0; i < childrenCount - 1; ++i) {
                // layerIndeciesが0の時は入れ替え対象外
                if (layerIndecies [i] >= 0 && layerIndecies [i + 1] >= 0) {
                    int j = i+1;
                    if (layerIndecies [i] > layerIndecies [j]) {
                        var tmpIndex = layerIndecies [j];
                        layerIndecies [j] = layerIndecies [i];
                        layerIndecies [i] = tmpIndex;

                        var tmpObject = children [j];
                        children [j] = children [i];
                        children [i] = tmpObject;

                        change = true;
                    }
                }
            }
        } while (change);

        for (int i = 0; i < childrenCount; ++i) {
            children [i].SetSiblingIndex (i);
        }
    }

    // psdのエンジンデータからColorを生成して返す。
    private static Color ArrayListToColor(ArrayList color)
    {
        double r = 0;
        double g = 0;
        double b = 0;
        double a = 0;
        if (color.Count >= 1) {
            a = (double)color[0];
        }
        if (color.Count >= 2) {
            r = (double)color[1];
        }
        if (color.Count >= 3) {
            g = (double)color[2];
        }
        if (color.Count >= 4) {
            b = (double)color[3];
        }
        return new Color ((float)r, (float)g, (float)b, (float)a);
    }

    // psdからロードしたデータからサイズを計算して返す。
    private static Vector2 BoundsToSize(DescriptorStructure bounds)
    {
        double left = ((UnitFloatStructure)bounds["Left"]).Value;
        double right = ((UnitFloatStructure)bounds["Rght"]).Value;
        double top = ((UnitFloatStructure)bounds["Top "]).Value;
        double bottom = ((UnitFloatStructure)bounds["Btom"]).Value;

        // UnityとPSDの配置ロジックの違いの問題があるためサイズは切り上げして大きめにしておく。
        return new Vector2 (Mathf.Ceil((float)(right - left)), Mathf.Ceil(((float)(bottom - top))));
    }

    List<string> createdObjectPath = new List<string> ();
    // ゲームオブジェクトを作成する。
    GameObject CreatePanel (string path)
    {
        var pathtemp = new List<string> ();
        pathtemp.Add ("Canvas");
        pathtemp.AddRange (path.Split(Split_ObjectPath));

        List<string> nowPaths = new List<string> ();

        var PathObj = new List<GameObject> ();
        PathObj.Add (CanvasObj);

        for (int i = 1; i < pathtemp.Count - 1; i++) {
            nowPaths.Add (pathtemp [i]);
            var transform = PathObj [i - 1].transform.Find (pathtemp [i]);
            string objectPath = string.Join (Split_ObjectPath.ToString(), nowPaths.ToArray());
            var layer = psd.Layers.FirstOrDefault (x => x.ObjectPath == objectPath);

            if (!createdObjectPath.Contains (layer.ObjectPath)) {
                if (transform == null) {
                    var temp = new GameObject ();
                    temp.SetActive (false);
                    temp.name = pathtemp [i];
                    temp.transform.SetParent (PathObj [i - 1].transform);
                    SetFolderObjectRectTransform (temp, layer);
                    PathObj.Add (temp);
                    temp.SetActive (true);
                } else {
                    var obj = transform.gameObject;
                    obj.SetActive (false);
                    SetFolderObjectRectTransform (obj, layer);
                    PathObj.Add (obj);
                    obj.SetActive (true);
                }
                createdObjectPath.Add (layer.ObjectPath);
            } else {
                PathObj.Add(transform.gameObject);
            }
        }
            
        var temp1Transform = PathObj.Last().transform.Find (pathtemp [pathtemp.Count - 1]);
        // オブジェクトが見つからない場合は作成
        GameObject temp1 = temp1Transform == null ? 
            new GameObject (pathtemp [pathtemp.Count - 1]) :
            temp1Transform.gameObject;

        PathObj.Add (temp1);
        // RectTransformが付いていない場合は初期化する。
        if (temp1.GetComponent<RectTransform>() == null) {
            temp1.SetActive (false);
            temp1.AddComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
            temp1.transform.GetComponent<RectTransform> ().SetParent (PathObj [pathtemp.Count - 2].transform);
            temp1.SetActive (true);
        }

        return PathObj.Last ();
    }

    // フォルダーレイヤーをオブジェクトとして生成する処理
    private void SetFolderObjectRectTransform(GameObject obj, Layer layer)
    {
        // 一個でも子オブジェクトにPSD管理外のオブジェクトがある場合でRectTransformをもともと持っている場合は何もしないで抜ける
        if (obj.transform is RectTransform) {
            for (int i = 0; i < obj.transform.childCount; ++i) {
                var rectTrans = obj.transform.GetChild (i) as RectTransform;
                if (rectTrans != null) {
                    var path = GetObjectPath (rectTrans.gameObject);
                    if (!psd.Layers.Any (x => x.ObjectPath == path)) {
                        return;
                    }
                }
            }
        }

        var rectTransform = obj.GetOrAddComponent<RectTransform> ();
        rectTransform.pivot = new Vector2 (0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2 (0.5f, 0.5f);
        if (!psd.Layers.Any(x => IsParent(layer, x))) {
            rectTransform.localPosition = new Vector3 (0, 0, 0);
            rectTransform.sizeDelta = new Vector2 (psdWidth, psdHeight);
        } else  {
            // 子供がいる場合は#SIZEレイヤーを探してあればそいつをベースにサイズをとる
            var rect = GetInclusionRect(layer);

            float xMin = rect.xMin;
            float yMin = rect.yMin;
            float xMax = rect.xMax;
            float yMax = rect.yMax;

            if(xMin < 0) xMin = 0;
            if(yMin < 0) yMin = 0;
            if(xMax > psdWidth) xMax = psdWidth;
            if(yMax > psdHeight) yMax = psdHeight;

            float sizeX = xMax - xMin;
            float sizeY = yMax - yMin;

            rectTransform.position = new Vector3 (rect.center.x - psdWidth / 2, psdHeight / 2 - rect.center.y, 0);
            rectTransform.sizeDelta = new Vector2 (sizeX, sizeY);
        }

        var Suffixs = GetSuffixNames (layer);
        bool isLeft = Suffixs.Contains(Suffix_AnchorLeft);
        bool isRight = Suffixs.Contains(Suffix_AnchorRight);
        bool isTop = Suffixs.Contains(Suffix_AnchorTop);
        bool isBottom = Suffixs.Contains(Suffix_AnchorBottom);
        SetPivot (rectTransform, isLeft, isRight, isTop, isBottom);
        SetAnchor (rectTransform, isLeft, isRight, isTop, isBottom);
    }

    // レイヤーの階層情報を作成する。
    private void ApplyLayerSections (List<Layer> layers)
    {
        var stack = new Stack<string> ();

        foreach (var layer in Enumerable.Reverse(layers)) {
            var sectionInfo = (LayerSectionInfo)layer.AdditionalInfo.SingleOrDefault (x => x is LayerSectionInfo);
            {
                var Reverstack = stack.ToArray ();
                if (Reverstack.Length > 0) {
                    Array.Reverse (Reverstack);
                    layer.ObjectPath = string.Join (Split_ObjectPath.ToString(), Reverstack) + Split_ObjectPath.ToString() + GetObjectName(layer);
                } else {
                    layer.ObjectPath = GetObjectName(layer);
                }
                layer.IsExclude = IsExcludeObject(layer);
                layer.IsOutput = layer.Visible;
            }
            if (sectionInfo != null) {
                switch (sectionInfo.SectionType) {
                case LayerSectionType.OpenFolder:
                    stack.Push (GetObjectName(layer));
                    break;
                case LayerSectionType.Layer:
                    stack.Push (GetObjectName(layer));
                    break;
                case LayerSectionType.ClosedFolder:
                    stack.Push (GetObjectName(layer));
                    break;
                case LayerSectionType.SectionDivider:
                    stack.Pop ();
                    break;
                }
            }
        }
    }

    // psdのロードとWindowの再構築
    public void LoadInformation (Texture2D Img)
    {
        string path = AssetDatabase.GetAssetPath (Img);

        psd = new PsdFile (path, Encoding.Default);
        fileName = Path.GetFileNameWithoutExtension (path);
        ApplyLayerSections (psd.Layers);

        objectTreeView.BuildView (psd.Layers.Where(x => !x.IsExclude));
    }

    // オブジェクトのboundsを返す
    // マスクレイヤーが指定されている場合そちらが優先される。
    private Rect GetTargetRect(Layer layer)
    {
        return GetImageRect(layer);
    }

    // 指定レイヤー配列のboundsを内包するboundsを返す
    private Rect GetInclusionRect(Layer parent, bool isFolderObject, IEnumerable<Layer> children)
    {
        Rect ret = new Rect();
        ret.yMax = ret.xMax = float.MinValue;
        ret.yMin = ret.xMin = float.MaxValue;
        if (children.Count() > 0) {
            var sizeObj = children.FirstOrDefault(x => x.Name == "#SIZE");
            if (sizeObj == null) {
                if (parent.Masks.LayerMask != null) {
                    ret = new Rect(parent.Masks.LayerMask.Rect);
                } else {
                    var folderLayers = children.Where (x => x.AdditionalInfo.Any (info => info is LayerSectionInfo));

                    // フォルダーオブジェクト内の非表示レイヤーとクリッピングレイヤーはサイズ計算に含めない
                    var imageLayers = children.Where (x => !x.AdditionalInfo.Any (info => info is LayerSectionInfo) && !x.Clipping &&
                        (!isFolderObject || (isFolderObject && x.Visible)));
                    /*
                    foreach (var layer in folderLayers) {
                        Debug.Log (layer.Name + " " + GetInclusionRect (layer, isFolderObject));
                        Debug.Log (layer.Clipping);
                        Debug.Log (string.Join (",", layer.AdditionalInfo.Select (x => x.Key).ToArray ()));
                    }
                    foreach (var layer in imageLayers) {
                        Debug.Log (layer.Name + " " + GetImageRect (layer));
                        Debug.Log (layer.Clipping);
                        Debug.Log (string.Join (",", layer.AdditionalInfo.Select (x => x.Key).ToArray ()));
                    }
                    */
                    List<Rect> rects = new List<Rect> ();
                    if (folderLayers.Count () > 0) {
                        rects.AddRange (folderLayers.Select (x => GetInclusionRect (x, isFolderObject)).Where(x => x.width > 0 && x.height > 0));
                    }
                    if (imageLayers.Count () > 0) {
                        rects.AddRange (imageLayers.Select (x => GetImageRect (x)).Where(x => x.width > 0 && x.height > 0));
                    }

                    if (rects.Count <= 0) {
                        if (!IsFolderObject(parent)) {
                            Debug.LogError (string.Format ("レイヤー名:{1} path:{0} にて有効なサイズを持つレイヤーが見つかりませんでした。", parent.ObjectPath, parent.Name));
                        }
                        if (isFolderObject) {
                            Debug.LogError (string.Format ("レイヤー名:{1} path:{0} にて有効なサイズを持つレイヤーが見つかりませんでした。\nフォルダーオブジェクトは内包するレイヤーでPhotoshop上で表示されているものだけをサイズ計算の対象とします。Photoshop上の表示状態を確認してください。", parent.ObjectPath, parent.Name));
                        }
                    } else {
                        ret.xMin = rects.Select (x => x.xMin).Min ();
                        ret.xMax = rects.Select (x => x.xMax).Max ();
                        ret.yMin = rects.Select (x => x.yMin).Min ();
                        ret.yMax = rects.Select (x => x.yMax).Max ();
                    }
                }
            } else {
                ret = new Rect(sizeObj.Rect);
            }
        }
        return ret;
    }

    public Rect GetImageRect(Layer layer)
    {
        Rect rect = new Rect (layer.Rect);
        if (layer.AdditionalInfo.Any (info => info is LayerTypeToolInfo)) {
            var typeToolInfo = (LayerTypeToolInfo)layer.AdditionalInfo.SingleOrDefault (x => x is LayerTypeToolInfo);
            var bounds = (DescriptorStructure)typeToolInfo.TextData["bounds"];
            var size = BoundsToSize (bounds);
            rect.width = size.x;
            rect.height = size.y;
            return rect;
        }

        try {
            var sprite = GetSprite (GetTextureName (layer));
            if (sprite != null && !Is9SliceObject(sprite)) {
                rect.width = sprite.rect.width;
                rect.height = sprite.rect.height;
                return rect;
            }
        } catch {
        }


        byte[] channel = null;
        if (layer.Masks.LayerMask != null) {
            channel = layer.Masks.LayerMask.ImageData;
        }

        if (layer.AlphaChannel != null) {
            channel = layer.AlphaChannel.ImageData;
        }

        if(channel == null)
        {
            return rect;
        }
         
        int height = (int)layer.Rect.height;
        int width = (int)layer.Rect.width;
        int right = width;
        int bottom = height;
        int left = -1;
        int top = -1;
        // alphaがある場合はalphaの０の部分を省いてサイズを求める
        for (int y = 0; y < height; ++y) {
            bool allZero = true;
            for (int x = 0; x < width; ++x) {
                byte a = channel [x + y * width];
                if (a != 0) {
                    allZero = false;
                    break;
                }
            }
            if (allZero) {
                top = y;
            } else {
                break;
            }
        }

        for (int y = height - 1; y >= 0; --y) {
            bool allZero = true;
            for (int x = 0; x < width; ++x) {
                byte a = channel [x + y * width];
                if (a != 0) {
                    allZero = false;
                    break;
                }
            }
            if (allZero) {
                bottom = y;
            } else {
                break;
            }
        }

        for (int x = 0; x < width; ++x) {
            bool allZero = true;
            for (int y = 0; y < height; ++y) {
                byte a = channel [x + y * width];
                if (a != 0) {
                    allZero = false;
                    break;
                }
            }
            if (allZero) {
                left = x;
            } else {
                break;
            }
        }
        for (int x = width - 1; x >= 0; --x) {
            bool allZero = true;
            for (int y = 0; y < height; ++y) {
                byte a = channel [x + y * width];
                if (a != 0) {
                    allZero = false;
                    break;
                }
            }
            if (allZero) {
                right = x;
            } else {
                break;
            }
        }

        rect.xMin += (float)(left + 1);
        rect.xMax += (float)(right - width);
        rect.yMin += (float)(top + 1);
        rect.yMax += (float)(bottom - height);

        return rect;
    }

    static public bool IsParent(Layer parent, Layer child)
    {
        var parentPathSplit = parent.ObjectPath.Split (Split_ObjectPath);
        var pathSplits = child.ObjectPath.Split(Split_ObjectPath);
        if(parentPathSplit.Length != pathSplits.Length - 1) {
            return false;
        }
        for(int i = 0; i < parentPathSplit.Length; ++i) {
            if(parentPathSplit[i] != pathSplits[i]) {
                return false;
            }
        }

        return true;
    }

    // 複数回やるのはめんどくさいのでキャッシュする。
    private Dictionary<Layer, Rect> cachedFolderRect = new Dictionary<Layer, Rect>();
    // フォルダー内のレイヤーのboundsを内包するboundsを返す
    // レイヤー指定バージョン
    private Rect GetInclusionRect(Layer parent, bool isFolderObject=false)
    {
        if (!cachedFolderRect.ContainsKey (parent)) {
            cachedFolderRect [parent] = GetInclusionRect (
                parent,
                isFolderObject || IsFolderObject(parent),
                psd.Layers.Where (x => 
                (
                    x.Name == "#SIZE" ||
                    (
                        !x.Name.StartsWith (Prefix_Exclude)
                    ) 
                ) &&
                IsParent (parent, x)
                )
            );
        } 
        return cachedFolderRect [parent];
    }

    // 指定レイヤーをオブジェクトとして作成するかの判定
    private bool IsCreateObject(Layer layer)
    {
        return layer.IsOutput && !layer.IsExclude &&
            (IsFolderObject (layer) || !layer.AdditionalInfo.Any (x => x is LayerSectionInfo));
    }

    private bool HasTexture(Layer layer)
    {
        var suffixs = GetSuffixNames (layer);
        var prefix = GetPrefixName (layer);
        return (IsCreateObject (layer) && !(layer.AdditionalInfo.Any (x => x is LayerTypeToolInfo) && (prefix == Prefix_Text || prefix == Prefix_TextMeshPro))) ||
            (Prefix_Button == prefix && (suffixs.Contains (Suffix_Disable) || suffixs.Contains (Suffix_Highlight) || suffixs.Contains (Suffix_Touched)));
    }

    // フォルダーを１オブジェクトとして扱う場合の判定
    private bool IsFolderObject(Layer layer)
    {
        return (layer.Name.StartsWith (Prefix_Image) || layer.Name.StartsWith (Prefix_Button)) &&
            layer.AdditionalInfo.Any(info => {
                LayerSectionInfo secInfo = info as LayerSectionInfo;
                return secInfo != null && (secInfo.SectionType == LayerSectionType.ClosedFolder || secInfo.SectionType == LayerSectionType.OpenFolder);
            });
    }

    // 除外レイヤー検出処理
    private bool IsExcludeObject(Layer layer)
    {
        // 除外キーワードを名前に持っている場合
        if (layer.Name.StartsWith (Prefix_Exclude)) {
            return true;
        }

        // ボタンの場合でかつボタンステート時の指定のものは除外指定する
        if (layer.Name.StartsWith (Prefix_Button)) {
            var suffixs = GetSuffixNames (layer);
            if (suffixs.Contains (Suffix_Disable) || suffixs.Contains (Suffix_Highlight) || suffixs.Contains (Suffix_Touched)) {
                return true;
            }
        }

        // レイヤーはClodedFolderのみを対象とする
        if (layer.AdditionalInfo.Any (info => {
            LayerSectionInfo secInfo = info as LayerSectionInfo;
            return secInfo != null && !(secInfo.SectionType == LayerSectionType.ClosedFolder || secInfo.SectionType == LayerSectionType.OpenFolder);
        })) {
            return true;
        }

        // レイヤーサイズが0のArtLayerは除外
        if (layer.Rect.width <= 0 && layer.Rect.height <= 0 && !layer.AdditionalInfo.Any(x => x is LayerSectionInfo)) {
            return true;
        }

        var paths = layer.ObjectPath.Split (Split_ObjectPath).ToList();
        // 自分自身は判定に含めない
        paths.RemoveAt (paths.Count - 1);

        // 親が除外オブジェクトなら除外する。
        if (paths.Any (x => x.StartsWith (Prefix_Exclude))) {
            return true;
        }

        // 親が統合オブジェクト指定されている場合は子は全て除外
        List<string> objects = new List<string>();
        foreach(var path in paths) {
            objects.Add (path);
            var objectPath = string.Join (Split_ObjectPath.ToString (), objects.ToArray ());
            Layer parentLayer = null;
            try {
                parentLayer = psd.Layers.SingleOrDefault (x => x.ObjectPath != null && x.ObjectPath == objectPath);
            } catch(Exception e) {
                Debug.LogError(string.Format("{0}と同じ階層に同名のオブジェクトが存在します。", objectPath));
                throw e;
            }
            if(parentLayer != null) {
                if(IsFolderObject(parentLayer)) {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetPivot (RectTransform rectTransform, bool isLeft, bool isRight, bool isTop, bool isBottom)
    {
        if (rectTransform == null)
            return;

        Vector2 pivot = new Vector2 (defaultPivot.x, defaultPivot.y);
        if (isLeft) {
            pivot.x = 0.0f;
        } else if (isRight) {
            pivot.x = 1.0f;
        }

        if (isTop) {
            pivot.y = 1.0f;
        } else if (isBottom) {
            pivot.y = 0.0f;
        }
        SetAnchor (rectTransform, pivot);
    }

    public void SetPivot (RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null)
            return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3 (deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    public void SetAnchor (RectTransform rectTransform, bool isLeft, bool isRight, bool isTop, bool isBottom)
    {
        Vector2 anchor = new Vector2 (0.5f, 0.5f);
        if (isLeft) {
            anchor.x = 0.0f;
        } else if (isRight) {
            anchor.x = 1.0f;
        }

        if (isTop) {
            anchor.y = 1.0f;
        } else if (isBottom) {
            anchor.y = 0.0f;
        }
        SetAnchor (rectTransform, anchor);
    }

    public void SetAnchor (RectTransform rectTransform)
    {
        SetAnchor (rectTransform, new Vector2 (0.5f, 0.5f));
    }

    public void SetAnchor (RectTransform rectTransform, Vector2 point)
    {
        if (rectTransform == null)
            return;

        var pos = rectTransform.localPosition;
        rectTransform.anchorMin = point;
        rectTransform.anchorMax = point;
        rectTransform.localPosition = pos;
    }

    // Droplet起動処理
    private void ProcessToPng(string psdPath)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process ();

#if UNITY_EDITOR_OSX
        process.StartInfo.FileName = Path.Combine(Application.dataPath, "../tool/to_png.app/Contents/MacOS/Droplet");
#elif UNITY_EDITOR_WIN
        process.StartInfo.FileName = Path.Combine(Application.dataPath, "../tool/to_png.exe");
#endif
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
        process.StartInfo.RedirectStandardError = true;
        process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ErrorOutputHanlder);
        process.StartInfo.RedirectStandardInput = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.Arguments = string.Format ("{0}/{1}", System.IO.Path.GetDirectoryName (Application.dataPath), psdPath);
        process.EnableRaisingEvents = true;
        process.Exited += new System.EventHandler(Process_Exit);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    // 標準出力時.
    private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            Debug.Log(args.Data);
        }
    }

    // エラー出力時.
    private void ErrorOutputHanlder(object sender, System.Diagnostics.DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            Debug.Log(args.Data);
        }
    }

    // プロセス終了時.
    private void Process_Exit(object sender, System.EventArgs e)
    {
        System.Diagnostics.Process proc = (System.Diagnostics.Process)sender;

        // プロセスを閉じる.
        proc.Kill();
    }


    // 
    private string GetObjectPath(GameObject obj)
    {
        Transform trans = obj.transform;
        StringBuilder builder = new StringBuilder();
        List<string> parentNames = new List<string> ();
        while (trans.parent != CanvasObj.transform) {
            trans = trans.parent;
            //builder.Append (trans.name + Split_ObjectPath.ToString());
            parentNames.Add (trans.name);
        }
        for (int i = parentNames.Count - 1; i >= 0; --i) {
            builder.Append(parentNames [i]);
            builder.Append(Split_ObjectPath.ToString());
        }
        builder.Append (obj.name);
        return builder.ToString();
    }

    public static string GetExcludeSuffixName(Layer layer)
    {
        var splitName = layer.Name.Split (Split_LayeraName);

        if (splitName.Length >= 2) {
            if (!PrefixKeywords.Contains (splitName [0])) {
                return splitName [0];
            } else {
                return splitName [0] + Split_LayeraName.ToString() + splitName [1];
            }
        }
        return layer.Name;
    }

    private static string RemoveNameDotLater(string name)
    {
        var ret = name;
        int dotPos = name.IndexOf ('.');
        if (dotPos >= 0) {
            ret = name.Remove (dotPos);
        }
        return ret;
    }

    public static string GetObjectName(Layer layer)
    {
        var layerName = RemoveNameDotLater(layer.Name);
        var splitName = layerName.Split (Split_LayeraName);
        List<string> nameElement = new List<string>();

        if (splitName.Length >= 2) {
            int startIndex = 1;
            nameElement.Add (splitName [0]);
            if (PrefixKeywords.Contains (splitName [0])) {
                startIndex = 2;
                nameElement.Add (splitName [1]);
            }

            for (int i = startIndex; i < splitName.Length; ++i) {
                if (!SuffixAnchorKeywords.Contains (splitName [i])) {
                    nameElement.Add (splitName [i]);
                }
            }

            return string.Join (Split_LayeraName.ToString (), nameElement.ToArray ());
        }
        return layerName;
    }

    public static string GetTextureName(Layer layer)
    {
        var name = RemoveNameDotLater(layer.Name);
        var splitName = name.Split (Split_LayeraName);
        List<string> nameElement = new List<string>();

        if (splitName.Length >= 2) {
            int startIndex = 1;
            nameElement.Add (splitName [0]);
            if (PrefixKeywords.Contains (splitName [0])) {
                startIndex = 2;
                nameElement.Add (splitName [1]);
            }

            for (int i = startIndex; i < splitName.Length; ++i) {
                if (SuffixSelectableKeywords.Contains (splitName [i])) {
                    nameElement.Add (splitName [i]);
                }
            }

            return string.Join (Split_LayeraName.ToString (), nameElement.ToArray ());
        }
        return name.Replace (' ', '-');
    }

    public static string GetPrefixName(Layer layer)
    {
        var splitName = layer.Name.Split (Split_LayeraName);

        if (splitName.Length >= 2) {
            if (PrefixKeywords.Contains (splitName [0])) {
                return splitName [0].ToLower();
            }
        }
        return string.Empty;
    }

    public static List<string> GetSuffixNames(Layer layer)
    {
        List<string> suffixs = new List<string> ();
        var splitName = layer.Name.Split (Split_LayeraName);

        if (splitName.Length >= 2) {
            int startIndex = 1;
            if (PrefixKeywords.Contains (splitName [0])) {
                startIndex = 2;
            }

            for (int i = startIndex; i < splitName.Length; ++i) {
                if (SuffixKeywords.Contains (splitName [i])) {
                    suffixs.Add (splitName [i].ToLower());
                }
            }
        }
        return suffixs;
    }
}
