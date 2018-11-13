using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// タップエフェクト.
/// </summary>
[RequireComponent(typeof(Camera))]
public class TapEffect : MonoBehaviour 
{
    [SerializeField]
    private GameObject effect;


    void Awake()
    {
        m_cam = this.GetComponent<Camera>();
        m_effectInstance = Instantiate(effect) as GameObject;
        m_effectInstance.transform.SetParent(this.transform);
        m_effectInstance.SetActive(false);
    }

	void Update()
    {
        if(!Input.GetMouseButtonDown(0)){
            return;
        }
        m_effectInstance.transform.position = m_cam.ScreenToWorldPoint(Input.mousePosition + m_cam.transform.forward * 10);
        m_effectInstance.SetActive(false);
        m_effectInstance.SetActive(true);
    }

    void OnDestroy()
    {
        foreach(var p in m_effectInstance.GetComponentsInChildren<ParticleSystem>(true)){
            p.Stop();
        }
        Destroy(m_effectInstance);
    }

    private Camera m_cam;
    private GameObject  m_effectInstance;
}
