//----------------------------------------------------------------------------------------------------------------------
// Visual Studio Tools for Unity のプロジェクトファイルの生成方法を操作するクラス。
//
// ■参考URL
// https://msdn.microsoft.com/ja-jp/dn833197.aspx
//----------------------------------------------------------------------------------------------------------------------
// memo: 環境によって動いたり動かなかったりしたので、問題が特定できるまで一旦無効化
#if false
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using UnityEditor;

using SyntaxTree.VisualStudio.Unity.Bridge;

[InitializeOnLoad]
public class ProjectFileHook
{
    // necessary for XLinq to save the xml project file in utf8 
    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    static ProjectFileHook()
    {
        ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
        {
            var document = XDocument.Parse(content);
            //document.Root.Add(new XComment("FIX ME"));
            _RemoveLangVersion(document);

            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        };
    }

    /// <summary>
    /// LangVersionノードを削除する(VisualStudioでC#6.0を利用できるようにする)
    /// </summary>
    /// <param name="document">XDocumentオブジェクト</param>
    static void _RemoveLangVersion(XDocument document)
    {
        // TODO: UnityバージョンアップでC#6.0に正式対応したらここを削除する
        document.Root.Elements().FirstOrDefault(x => x.Name.LocalName == "PropertyGroup").Elements().FirstOrDefault(x => x.Name.LocalName == "LangVersion").Remove();
    }
}
#endif