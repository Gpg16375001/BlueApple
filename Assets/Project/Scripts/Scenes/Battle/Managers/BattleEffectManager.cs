using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEffectManager : MonoBehaviour {
    internal class BattleEffectItemCollection
    {
        public BattleEffectItem m_Item = null;
        public string m_Tag = string.Empty;
        public bool IsRemove {
            get {
                return m_Item == null;
            }
        }

        public BattleEffectItemCollection(BattleEffectItem item, string tag)
        {
            m_Item = item;
            m_Tag = tag;
        }
    }

    /// <summary>共通インスタンス.</summary>
    private static BattleEffectManager m_Instance;
    public static BattleEffectManager Instance {
        get {
            return m_Instance;
        }
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    public void CreateDamageLabel(int number, bool isHit, bool isCritical, bool week, bool resist, int index, GameObject parent, Vector3 startPos, System.Action endCallback=null)
    {
        var go = GameObjectEx.LoadAndCreateObject ("Battle/Damage", parent);
        go.transform.position = startPos;
        var effectItem = go.GetOrAddComponent<BattleEffectItem> ();
        // 再生開始待ちさせる
        effectItem.WaitPlay ();
        if (endCallback != null) {
            effectItem.SetNextCallback ((effGo) => {
                if (endCallback != null) {
                    endCallback ();
                }
            });
        }
        go.GetOrAddComponent<BattleDamageItem> ().Init (number, isHit, isCritical, week, resist, (float)index * 1.0f,
            () => { 
                effectItem.Play();
            }
        );

        m_ItemCollection.Add (new BattleEffectItemCollection (effectItem, "Damage"));
    }

    public void CreateHealLabel(int number, int index, GameObject parent, Vector3 startPos)
    {
        var go = GameObjectEx.LoadAndCreateObject ("Battle/Heal", parent);
        go.transform.position = startPos;
        var effectItem = go.GetOrAddComponent<BattleEffectItem> ();
        // 再生開始待ちさせる
        effectItem.WaitPlay ();

        go.GetOrAddComponent<BattleHealItem> ().Init (number, (float)index * 1.0f,
            () => {
                effectItem.Play();
            }
        );

        m_ItemCollection.Add (new BattleEffectItemCollection (effectItem, "Heal"));
    }

	public static event System.Action<GameObject/*EffectItemObj*/> DidCreateEffectObject;
    public GameObject CreateEffectItem(string tag, string prefabName, float waitPlay, GameObject parent = null, System.Action<GameObject> didCreate = null, System.Action<GameObject> didNext = null, System.Action<GameObject> didDestroy = null)
    {
		Debug.Log("CreateEffectItem : DidCreateEffectObject="+(DidCreateEffectObject));
        bool loadAssetBundle = false;
        GameObject go = null;
        var prefab = BattleResourceManager.Shared.GetBattleSkillEffectPrefab (prefabName);
        if (prefab == null) {
            go = GameObjectEx.LoadAndCreateObject (string.Format ("Battle/BattleEffect/{0}", prefabName), parent);
        } else {
            go = GameObject.Instantiate (prefab);
            if (parent != null) {
                parent.AddInChild (go);
            }
            loadAssetBundle = true;
        }

        var effectItem = go.GetOrAddComponent<BattleEffectItem> ();
        if (loadAssetBundle) {
            effectItem.SetRendererSortSetting ("BattleCharacter");
        }
        if (didNext != null) {
            effectItem.SetNextCallback (didNext);
        }
        if (didDestroy != null) {
            effectItem.SetNextCallback (didDestroy);
        }

        if (didCreate != null) {
            didCreate (go);
        }
		if(DidCreateEffectObject != null){
			DidCreateEffectObject(go);  // 外部からの干渉用.
		}

        if (waitPlay > 0.0f) {
            effectItem.SetWait (waitPlay);
        }

        m_ItemCollection.Add (new BattleEffectItemCollection (effectItem, tag));



        return go;
    }

    public bool ExistTag(string tag)
    {
        return m_ItemCollection.Any (x => x.m_Tag == tag);
    }

    public bool IsNextAllWithTag(string tag)
    {
        return !ExistTag(tag) || m_ItemCollection.Where(x => x.m_Tag == tag).All (x => x.m_Item.IsNext);
    }

    void LateUpdate()
    {
        int count = m_ItemCollection.Count;
        for (int i = 0; i < count; ++i) {
            var item = m_ItemCollection [i].m_Item;

            item.UpdateAnimationState ();
            if (item.IsNext) {
                item.CallNextCallback ();
            }
            if (item.MayDestory) {
                item.CallDestoryCallback ();

                if (item.gameObject != null) {
                    Destroy (item.gameObject);
                }
                m_ItemCollection [i].m_Item = null;
            }
        }
        m_ItemCollection.RemoveAll (x => x.IsRemove);
    }

    private List<BattleEffectItemCollection> m_ItemCollection = new List<BattleEffectItemCollection> ();
}
