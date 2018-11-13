using UnityEngine;
using System.Collections;

using SmileLab;

/// <summary>
/// 各種カメラのヘルパー.カメラにアクセスしたいときはここから.
/// </summary>
public class CameraHelper : ViewBase
{
    
    /// <summary>
    /// 共通インスタンス.
    /// </summary>
    public static CameraHelper SharedInstance { get; private set; }
    
    /// <summary>2D用カメラ.</summary>
    public Camera Camera2D { get { return this.GetScript<Camera>("Camera2D"); } }
    /// <summary>3D用カメラ.</summary>
    public Camera BattleCamera { get { return this.GetScript<Camera>("BattleCamera"); } }

    public Transform BattleCameraShake { get { return this.GetScript<Transform>("BattleCameraShake"); } }
	/// <summary>フェード用カメラ.</summary>
    public Camera FadeCamera { get { return this.GetScript<Camera>("FadeCamera"); } }
	/// <summary>クエストマップ用カメラ.</summary>
	public Camera CameraQuestMap { get { return this.GetScript<Camera>("CameraQuestMap"); } }

    /// <summary>バトルカメラ入力操作機能のOn/Off.</summary>
    public bool IsEnableBattleCameraInput 
    { 
        get {
            return BattleCamera.GetComponent<Camera2DTouchModule>().IsEnable;
        }
        set {
            BattleCamera.GetComponent<Camera2DTouchModule>().IsEnable = value;
        }
    }


    /// <summary>位置的にフォーカス終了してる？</summary>
    public bool IsEndFocus 
    {
        get {
            if(m_targetObj == null){
                return false;
            }
            var targetPos = m_forcusBasePos + m_targetObj.transform.position;
            var maxX = Mathf.Max( Mathf.Abs(targetPos.x), Mathf.Abs(this.BattleCamera.transform.position.x) );
            var minX = Mathf.Min( Mathf.Abs(targetPos.x), Mathf.Abs(this.BattleCamera.transform.position.x) );
            var maxY = Mathf.Max( Mathf.Abs(targetPos.y), Mathf.Abs(this.BattleCamera.transform.position.y) );
            var minY = Mathf.Min( Mathf.Abs(targetPos.y), Mathf.Abs(this.BattleCamera.transform.position.y) );
            return (maxX - minX) < 1f && (maxY - minY) < 1f;
        }
    }
    
    
    void Awake()
    {
        if(SharedInstance != null){
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }
    
	void Start()
    {
        m_forcusBasePos = this.BattleCamera.transform.position;
        m_forcusBaseRotate = this.BattleCamera.transform.rotation;
        m_baseOrthographicSize = this.BattleCamera.orthographicSize;
    }

    /// <summary>
    /// 指定ターゲットに時間をかけてフォーカスする.
    /// </summary>
    public void ForcusTargetDelay2D(GameObject target, float forcusSpeed)
    {
        this.StopCoroutine("ResetPosition");
        this.StopCoroutine("UpdatePosition");
        this.StopCoroutine("UpdatePositionDelay");
        
        m_targetObj = target;
        m_forcusSpeed = forcusSpeed > 0f ? forcusSpeed : 1f;
        this.StartCoroutine("UpdatePositionDelay");
    }

    private IEnumerator UpdatePositionDelay()
    {
        var targetPos = this.BattleCamera.transform.position;
        while(m_targetObj != null){
            var x = m_forcusBasePos.x + m_targetObj.transform.position.x;
            var y = m_forcusBasePos.y + m_targetObj.transform.position.y;
            targetPos = new Vector3(x, y, m_forcusBasePos.z);
            this.BattleCamera.transform.position = Vector3.Lerp(this.BattleCamera.transform.position, targetPos, Time.deltaTime * m_forcusSpeed);
            yield return null;
        }
    }
    
    /// <summary>
    /// 指定ターゲットにカメラをフォーカスする.
    /// </summary>
    public void ForcusTarget(GameObject target, float timeLeft = 0f)
    {
        this.StopCoroutine("ResetPosition");
        this.StopCoroutine("UpdatePosition");
        this.StopCoroutine("UpdatePositionDelay");
        
        m_targetObj = target;
        this.StartCoroutine("UpdatePosition", timeLeft);
    }
    private IEnumerator UpdatePosition(float timeLeft)
    {
        // フォーカス時間の制限があればその時間だけフォーカス.
        var startTime = Time.time;
        var x = m_forcusBasePos.x + m_targetObj.transform.position.x;
        var y = m_forcusBasePos.y + m_targetObj.transform.position.y;
        this.BattleCamera.transform.position = new Vector3( x, y, m_forcusBasePos.z );
        yield return new WaitForSeconds(timeLeft);
        this.EndForcus();
    }
    
    /// <summary>
    /// フォーカス終了.
    /// </summary>
    public void EndForcus(bool bImmediate = false)
    {
        if(m_targetObj == null){
            Debug.LogWarning("[CameraController2D] EndForcus Warning : Not set target.");
            return;
        }
        this.StopCoroutine("UpdatePosition");
        if(bImmediate){
            this.BattleCamera.transform.position = m_forcusBasePos;
            this.BattleCamera.transform.rotation = m_forcusBaseRotate;
        }else{
            this.StartCoroutine("ResetPosition");
        }
        m_targetObj = null;
    }
    private IEnumerator ResetPosition()
    {
        var pos = this.transform.position;
        while(pos != m_forcusBasePos){
            this.BattleCamera.transform.position = Vector3.Lerp(this.BattleCamera.transform.position, m_forcusBasePos, Time.deltaTime * m_forcusSpeed);
            this.BattleCamera.transform.rotation = Quaternion.Lerp(this.BattleCamera.transform.rotation, m_forcusBaseRotate, Time.deltaTime * m_forcusSpeed);
            yield return null;
        }
    }
    
    /// <summary>
    /// カメラ位置一時停止.
    /// </summary>
    public void StopPosition()
    {
        this.StopCoroutine("UpdatePosition");
    }

    /// <summary>
    /// 2Dカメラの画面揺れ対応.
    /// </summary>
    public void ShakeCamera(float time = 0.5f)
    {
        var hash = iTween.Hash("x", 0.04f, "y", 0.04f, "time", time);
        iTween.ShakePosition(Camera2D.gameObject, hash);
        iTween.ShakeRotation(Camera2D.gameObject, hash);
        hash = iTween.Hash("x", 1f, "y", 1f, "time", time);
        iTween.ShakePosition(BattleCameraShake.gameObject, hash);
        iTween.ShakeRotation(BattleCameraShake.gameObject, hash);
    }

    public void ShakeBattleCamera(float max_x = 1.0f, float max_y = 1.0f, float time = 0.5f)
    {
        var hash = iTween.Hash("x", max_x, "y", max_y, "time", time);
        iTween.ShakePosition(BattleCameraShake.gameObject, hash);
        iTween.ShakeRotation(BattleCameraShake.gameObject, hash);
    }

    public void ShakeStopBattleCamera()
    {
        iTween.Stop (BattleCameraShake.gameObject);

        BattleCameraShake.localPosition = Vector3.zero;
        BattleCameraShake.localRotation = Quaternion.identity;
    }

    #region Camera Zoom Logic

    public void ForcusTargetForZoom(Vector3 target, float orthographicSize, float time)
    {
        iTween.Stop (BattleCamera.gameObject);
        iTween.MoveTo (BattleCamera.gameObject, target, time);
        Hashtable hash = new Hashtable(){
            {"from", BattleCamera.orthographicSize},
            {"to", orthographicSize},
            {"time", time},
            {"delay", 0f},
            {"easeType",iTween.EaseType.linear},
            {"loopType",iTween.LoopType.none},
            {"onupdate", "UpdateOrthographicSize"},
            {"onupdatetarget", gameObject},
        };
        iTween.ValueTo (BattleCamera.gameObject, hash);
    }

    public void ResetForcusForZoom(float time)
    {
        iTween.Stop (BattleCamera.gameObject);
        iTween.MoveTo (BattleCamera.gameObject, m_forcusBasePos, time);
        Hashtable hash = new Hashtable(){
            {"from", BattleCamera.orthographicSize},
            {"to", m_baseOrthographicSize},
            {"time", time},
            {"delay", 0f},
            {"easeType",iTween.EaseType.linear},
            {"loopType",iTween.LoopType.none},
            {"onupdate", "UpdateOrthographicSize"},
            {"onupdatetarget", gameObject},
        };
        iTween.ValueTo (BattleCamera.gameObject, hash);
    }

    private void UpdateOrthographicSize(float value)
    {
        BattleCamera.orthographicSize = value;
    }

    #endregion

    #region Camera Switching Logic

    public float SwitchinCameraOffsetX = 2.0f;
    /// <summary>
    /// カメラの位置を設定する
    /// </summary>
    /// <param name="target">指定位置</param>
    public void ForcusTarget(Vector3 target)
    {
        float offsetX = 0.0f;
        if (target.x > 0.0f) {
            offsetX = -1.0f * SwitchinCameraOffsetX;
        } else if(target.x < 0.0f) {
            offsetX = SwitchinCameraOffsetX;
        }
        iTween.Stop (BattleCamera.gameObject);
        this.BattleCamera.transform.position = new Vector3(
            target.x + offsetX,
            target.y,
            m_forcusBasePos.z);
    }

    /// <summary>
    /// カメラを初期位置に戻す
    /// </summary>
    public void ResetCameraPostion()
    {
        iTween.Stop (BattleCamera.gameObject);
        this.BattleCamera.transform.position = m_forcusBasePos;
    }

    /// <summary>
    /// カメラの正射影サイズを指定する。ズーム表現に使用
    /// </summary>
    /// <param name="orthographicSize">Orthographic size.</param>
    public void SetOrthographicSize(float orthographicSize)
    {
        iTween.Stop (BattleCamera.gameObject);
        this.BattleCamera.orthographicSize = orthographicSize;
    }

    /// <summary>
    /// カメラの正射影サイズを初期値に戻す。
    /// </summary>
    public void ResetOrthographicSize()
    {
        iTween.Stop (BattleCamera.gameObject);
        this.BattleCamera.orthographicSize = m_baseOrthographicSize;
    }
    #endregion

    private GameObject m_targetObj;
    private float m_forcusSpeed = 1f;
    private Vector3 m_forcusBasePos;
    private Quaternion m_forcusBaseRotate;
    private float m_baseFov;
    private float m_baseOrthographicSize;
}
