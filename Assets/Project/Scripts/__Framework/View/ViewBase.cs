using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using SmileLab.UI;

namespace SmileLab
{
    /// <summary>
    /// Viewベースクラス
    /// </summary>
    public class ViewBase : MonoBehaviour, IDisposable
    {

        /// <summary>
        /// プロパティ：ボタン有効設定
        /// </summary>
        public virtual bool IsEnableButton
        {
            set {
                foreach(var b in this.gameObject.GetComponentsInChildren<Button>()) {
                    b.interactable = value;
                }
				foreach (var b in this.gameObject.GetComponentsInChildren<CustomButton>()) {
                    b.interactable = value;
                }
                foreach(var b in this.gameObject.GetComponentsInChildren<Toggle>()) {
                    b.enabled = value;
                }
            }
        }

        /// <summary>
        /// プロパティ：Destroy された？
        /// </summary>
        public bool IsDestroyed { get; private set; }


        /// <summary>
        /// 使用後は必ず呼び出すこと！内部でGameObjectも破棄してる.アンマネージド系のリソースを使うことを考慮.
        /// </summary>
        public virtual void Dispose()
        {
            if(!this.IsDestroyed) {
                GameObject.Destroy(this.gameObject);
            }
        }
        private void OnDestroy()
        {
            this.IsDestroyed = true;
        }

        /// <summary>
        /// ボタンを押した時の処理設定. ※for uGUI Canvas
        /// </summary>
        public void SetCanvasButtonMsg(string btnName, UnityAction func)
        {
            this.GetScript<Button>(btnName).onClick.AddListener(func);
        }
        /// <summary>
        /// ボタンを押した時の処理設定. ※for uGUI Other
        /// </summary>
        public void SetButtonMsg(string btnName, UnityAction func)
        {
            this.GetScript<uGUIButtonBehaviour>(btnName).AddClickHandler(func);
        }
        /// <summary>
        /// ボタンを押した時の処理設定. ※for SmileLab.UI.CustomButton
        /// </summary>
        public void SetCanvasCustomButtonMsg(string btnName, UnityAction func, bool interactable = true)
        {
            var button = this.GetScript<CustomButton> (btnName);
            button.onClick.AddListener(func);
            button.interactable = interactable;
        }

        /// <summary>
        /// テキストオブジェクトに文字列を設定する
        /// </summary>
        /// <param name="objectName">設定対象のオブジェクト名</param>
        /// <param name="text">設定する文字列</param>
        public void SetText(string objectName, string text)
        {
            this.GetScript<Text>(objectName).text = text;
        }

        /// <summary>
        /// 指定タイプのコンポーネントを子階層から全て取得する
        /// </summary>
        public Dictionary<string, object> GetScriptList(Type type)
        {
            var tbl = new Dictionary<string, object>();

            foreach(var i in this.GetComponentsInChildren(type, true)) {
                tbl[i.name] = i;

                if(i.transform.parent != null) {
                    tbl[i.transform.parent.name + "/" + i.name] = i;
                }
            }
            return tbl;
        }
        /// <summary>
        /// 指定名を含む指定タイプのコンポーネントを取得する.親は含まない.
        /// </summary>
        public Dictionary<string, object> GetScriptList(string name, Type type)
        {
            var tbl = new Dictionary<string, object>();
            foreach(var i in this.GetComponentsInChildren(type, true)) {
                if(!i.name.Contains(name)) {
                    continue;
                }
                tbl[i.name] = i;
            }
            return tbl;
        }

        /// <summary>
        /// 該当名のコンポーネントがこ階層に存在しているか.
        /// </summary>
		public bool Exist<T>(string key) where T : Component
		{
			if (!this.Scripts.ContainsKey(typeof(T))) {
                this.UpdateScriptList<T>();
            }
            if (!this.Scripts.ContainsKey(typeof(T))) {
				return false;
            }
            var tbl = this.Scripts[typeof(T)];
            if (!tbl.ContainsKey(key)) {
				return false;
            }
            return tbl[key] is T;
		}

        /// <summary>
        /// 子階層にあるすべてのスクリプトから該当名のものを取得する.
        /// </summary>
        public T GetScript<T>(string key) where T : Component
        {
            if(!this.Scripts.ContainsKey(typeof(T))) {
                this.UpdateScriptList<T>();
            }
            if(!this.Scripts.ContainsKey(typeof(T))) {
                throw new KeyNotFoundException(string.Format("オブジェクト「" + gameObject.name + "」の子階層に「" + typeof(T).Name + "」型のオブジェクトが１つも見つかりませんでした。"));
            }
            var tbl = this.Scripts[typeof(T)];
            if(!tbl.ContainsKey(key)) {
                throw new KeyNotFoundException(string.Format("オブジェクト「" + gameObject.name + "」の子階層に「" + typeof(T).Name + "」型のオブジェクト「" + key + "」が見つかりませんでした。"));
            }
            return tbl[key] as T;
        }
        private void UpdateScriptList<T>() where T : Component
        {
            if(this.Scripts.ContainsKey(typeof(T))) {
                this.Scripts[typeof(T)].Clear();
            }
            this.Scripts[typeof(T)] = this.GetScriptList(typeof(T));
        }

        private Dictionary<Type, Dictionary<string, object>> Scripts
        {
            get {
                if(this.m_scripts == null) {
                    this.m_scripts = new Dictionary<Type, Dictionary<string, object>>();
                }
                return m_scripts;
            }
        }
        private Dictionary<Type, Dictionary<string, object>> m_scripts;

    }
}
