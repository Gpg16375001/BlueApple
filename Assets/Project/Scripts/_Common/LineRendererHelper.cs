using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// Lineを引く際のヘルパー.
/// </summary>
public class LineRendererHelper : ViewBase
{

    /// <summary>
    /// 行動順リストの次の位置へラインを引く.
    /// </summary>
    public static void DrawActiveTimeLineNext(Transform start, Transform target, Canvas canvas)
    {
        if (instance == null) {
            var go = GameObjectEx.LoadAndCreateObject("_Common/LineRendererHelper");
            instance = go.GetOrAddComponent<LineRendererHelper>();
        }
        instance.transform.SetParent(canvas.transform, false);
        CoroutineAgent.Execute(DelayDrawActiveTime(start, target, canvas));
    }

    static float subY = 0.05f;
    // Gridに対するSetSiblingなど座標が固まるまで時間がかかることがあるので基本的にディレイをかける.
    static IEnumerator DelayDrawActiveTime(Transform start, Transform target, Canvas canvas)
    {
        yield return new WaitForSeconds(0.1f);

        var line = instance.GetScript<LineRenderer>("LineRenderer");

        line.sortingLayerID = canvas.sortingLayerID;
        line.sortingOrder = canvas.sortingOrder;

        // リスト外の順番の時は後ろに続く矢印を表示する.targetObjがnullかどうかで判断.
        Vector3 endPos = Vector3.zero;
        if (target == null) {
            endPos = new Vector3(GetCanvasLeftEndX(canvas), (start.position.y + subY));
        } else {
            endPos = new Vector3(target.position.x, target.position.y);
        }
        var positionList = new List<Vector3>();
        line.gameObject.SetActive(true);
        positionList.Add(new Vector3(start.position.x, start.position.y));
        positionList.Add(new Vector3(start.position.x, (start.position.y + subY)));
        if(target != null){
            positionList.Add(new Vector3(endPos.x, endPos.y + subY));
        }
        positionList.Add(endPos);
        line.positionCount = positionList.Count;
        line.SetPositions(positionList.ToArray());

        // TODO : 矢印ヘッダー表示.リスト外の場合も対応する必要あり.
        var arrowName = target != null ? "LineRendererArrowHead" : "LineRendererArrowHeadOut";
        var arrowMesh = instance.GetScript<MeshRenderer>(arrowName);
        // 枠外矢印の場合は矢印とラインの間にちょっと隙間を開けたい...
        if(arrowName == "LineRendererArrowHeadOut"){
            endPos.x -= (instance.GetScript<MeshRenderer>("LineRendererArrowHeadOut").bounds.size.x/2f);
        }
        arrowMesh.transform.position = endPos;
        arrowMesh.gameObject.SetActive(true);
    }
    // 指定したキャンバスの左端の座標Xを求めて返す.
    static float GetCanvasLeftEndX(Canvas canvas)
    {
        var canvasRect = canvas.GetComponent<RectTransform>();
        var leftEndX = canvasRect.rect.xMin;

        // 左端表示用の矢印アイコンを考慮した位置に調整.(枠外矢印の場合は矢印とラインの間にちょっと隙間を開けたい...)
        var arrowMesh = instance.GetScript<MeshRenderer>("LineRendererArrowHeadOut");
        var worldPos = canvas.transform.localToWorldMatrix.MultiplyPoint3x4 (new Vector3 (leftEndX, 0.0f));

        return worldPos.x + arrowMesh.bounds.size.x;
    }

    /// <summary>
    /// ライン削除.
    /// </summary>
    public static void DestroyLine()
    {
        if(instance == null){
            return;
        }
        GameObject.Destroy(instance.gameObject);
        instance = null;
    }

    private static LineRendererHelper instance;
}
