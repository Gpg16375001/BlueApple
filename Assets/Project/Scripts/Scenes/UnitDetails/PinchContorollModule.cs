using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GraphicCast))]
public class PinchContorollModule : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void PinchContoroll(float scale);

    public event PinchContoroll PinchCallbackEvent;

    private int? FirstPointerID = null;
    private int? SceondPointerID = null;

    private Vector2 FirstPointerPosition;
    private Vector2 ScecondPointerPosition;

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!FirstPointerID.HasValue) {
            FirstPointerID = eventData.pointerId;
            FirstPointerPosition = eventData.position;
        } else if(FirstPointerID.Value != eventData.pointerId && !SceondPointerID.HasValue) {
            SceondPointerID = eventData.pointerId;
            ScecondPointerPosition = eventData.position;

            MobileControl (FirstPointerPosition, ScecondPointerPosition);
        }
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        if (FirstPointerID.HasValue && FirstPointerID.Value == eventData.pointerId) {
            if (SceondPointerID.HasValue) {
                FirstPointerID = SceondPointerID;
                FirstPointerPosition = ScecondPointerPosition;
                SceondPointerID = null;
            } else {
                FirstPointerID = null;
            }
            isPinch = false;
        } else if(SceondPointerID.HasValue && SceondPointerID.Value == eventData.pointerId) {
            SceondPointerID = null;
            isPinch = false;
        }
    }

    public void OnDrag (PointerEventData eventData)
    {
        if (FirstPointerID.HasValue && FirstPointerID.Value == eventData.pointerId) {
            FirstPointerPosition = eventData.position;
        } else if (SceondPointerID.HasValue && SceondPointerID.Value == eventData.pointerId) {
            ScecondPointerPosition = eventData.position;
        }
        if (FirstPointerID.HasValue && SceondPointerID.HasValue) {
            MobileControl (FirstPointerPosition, ScecondPointerPosition);
        }
    }

    #if UNITY_EDITOR
    void Update()
    {
        EditorControl ();
    }
    #endif

    private void EditorControl(){
        //タッチ中の処理
        if (isPinch) {
            //タッチ終了を感知し、終了処理をする
            if (Input.GetAxisRaw ("Vertical") == 0) {
                isPinch = false;
                return;
            }
            scale += Input.GetAxisRaw ("Vertical") * 1f * Time.deltaTime;
            SetNewScale (scale);
            UpdateScaling ();
            return;
        }
        //タッチ開始時を感知し、初期化処理をする
        if (Input.GetAxisRaw ("Vertical") != 0) {
            isPinch = true;
        }
    }
        
    private float max_distance = 0;
    private void MobileControl(Vector2 pos1, Vector2 pos2){
        //タッチ中の処理
        if (isPinch) {
            float distance = Vector2.Distance (pos1, pos2);
            SetNewScale (distance / max_distance);
            UpdateScaling ();
            return;
        }
        else {
            isPinch = true;
            float distance = Vector2.Distance (pos1, pos2);
            max_distance = distance / scale;
        }
    }

    private void SetNewScale(float new_scale){
        // min < 新しい拡大率 < max に設定する
        scale = Mathf.Clamp (new_scale, RangeScale.min, RangeScale.max);
    }

    private void UpdateScaling()
    {
        if (PinchCallbackEvent != null) {
            PinchCallbackEvent (scale);
        }
    }

    [SerializeField]
    private float scale;
    [System.Serializable]
    struct RangeClass
    {
        public float min, max;
    }
    [SerializeField]
    private RangeClass RangeScale;


    private bool isPinch = false;
}
