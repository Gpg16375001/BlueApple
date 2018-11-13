using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SmileLab
{
    /// <summary>
    /// JsonUtility拡張.
    /// </summary>
    public static class JsonUtilityEx
    {
        /// <summary>List型をシリアライズし指定のフィールド名としてJson文字列化する.</summary>
        public static string ToJsonUtilityJson<T>(this List<T> list, string fieldName)
        {
            var rtn = JsonUtility.ToJson(new Serialization<T>(list));
            return rtn.Replace("target", fieldName);
        }
        /// <summary>List型をシリアライズし指定のフィールド名としてJson文字列化する.</summary>
        public static string ToJsonUtilityJson<TKey, TValue>(this Dictionary<TKey, TValue> dict, string keyName, string valName)
        {
            var rtn = JsonUtility.ToJson(new Serialization<TKey, TValue>(dict));
            rtn = rtn.Replace("keys", keyName);
            return rtn.Replace("values", valName);
        }
    }

    /// <summary>
    /// List型のシリアライズ.
    /// </summary>
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        /*
         * ex. JsonUtilityにおけるシリアライズ例
         * // List<T> -> Json文字列 ( 例 : List<Enemy> )
         * string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies));
        */
        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }

    /// <summary>
    /// Dictionary型のシリアライズ.
    /// </summary>
    [Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }

        /*
         * ex. JsonUtilityにおけるシリアライズ例
         * // Dictionary<TKey,TValue> -> Json文字列 ( 例 : Dictionary<int, Enemy> )
        * string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies));
        */
        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for(var i = 0; i < count; ++i) {
                target.Add(keys[i], values[i]);
            }
        }
    }
}