using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BattleLogic {

    public static class SkillExecutionResultExtention
    {
        public static T Cast<T>(SkillEffectResultBase self) where T : SkillEffectResultBase
        {
            return self as T;
        }

        public static IEnumerable<T> Cast<T>(this IEnumerable<SkillEffectResultBase> self) where T : SkillEffectResultBase
        {
            foreach (var result in self) {
                if (result is T) {
                    yield return (T)result;
                }
            }
        }
    }

    /// <summary>
    /// スキル効果の結果
    /// </summary>
    public abstract class SkillEffectResultBase
    {
        /// <summary>
        /// スキル効果の成功可否
        /// 攻撃が当たったや味方を回復したなどスキルの発動が成功した場合にtrueとなる。
        /// 詳細はwikiのスキル関連を参照
        /// </summary>
        public bool IsSuccess {
            get;
            private set;
        }

        public SkillEffectLogicEnum LogicEnum {
            get;
            private set;
        }

        public SkillEffectResultBase(bool success, SkillEffectLogicEnum logic)
        {
            IsSuccess = success;
            LogicEnum = logic; 
        }
    }

    /// <summary>
    /// スキルの実行結果
    /// </summary>
    public class SkillExecutionResult
    {
        private Dictionary<Parameter, List<SkillEffectResultBase>> Results = new Dictionary<Parameter, List<SkillEffectResultBase>> ();

        public List<SkillEffectResultBase> GetResults(Parameter parameter)
        {
            List<SkillEffectResultBase> ret = null;
            Results.TryGetValue (parameter, out ret);
            return ret;
        }

        public List<SkillEffectResultBase> GetResults()
        {
            return Results.SelectMany (x => x.Value).ToList();
        }

        public IEnumerable<Parameter> GetReceivers()
        {
            return Results.Keys;
        }

        public bool HasEffectResult(params SkillEffectLogicEnum[] logic)
        {
            return Results.Values.Any (x => x.Any (result => logic.Contains (result.LogicEnum)));
        }

        public IEnumerable<KeyValuePair<Parameter, SkillEffectResultBase[]>> GetResults(params SkillEffectLogicEnum[] logic)
        {
            foreach (var result in Results) {
                if(result.Value.Any(x => logic.Contains(x.LogicEnum))) {
                    yield return new KeyValuePair<Parameter, SkillEffectResultBase[]> (
                        result.Key,
                        result.Value.Where (x => logic.Contains (x.LogicEnum)).ToArray ()
                    );
                }
            }
        }

        public IEnumerable<SkillEffectResultBase> GetEffectResults(params SkillEffectLogicEnum[] logic)
        {
            foreach (var result in Results) {
                if(result.Value.Any(x => logic.Contains(x.LogicEnum))) {
                    foreach(var effectResult in result.Value) {
                        if (logic.Contains (effectResult.LogicEnum)) {
                            yield return effectResult;
                        }
                    }
                }
            }
        }

        public void AddResult(Parameter target, SkillEffectResultBase result)
        {
            List<SkillEffectResultBase> ret = null;
            Debug.Log (target.Name + ": " + Results.Count);
            if (!Results.TryGetValue (target, out ret)) {
                ret = new List<SkillEffectResultBase> ();
                Results.Add (target, ret);
            }
            ret.Add (result);
        }

        public bool IsSuccess {
            get {
                return Results.Count > 0 && Results.Values.Any (x => x.Any (result => result.IsSuccess));
            }
        }

        public bool IsDamageCritical {
            get {
                var results = Results.Values.SelectMany(x => x).Cast<DamageSkillEffectResult> ();
                return results.Count () > 0 && results.Any (x => x.DamageInfos.IsCritical ());
            }
        }

        public bool IsDamageHit {
            get {
                var results = Results.Values.SelectMany(x => x).Cast<DamageSkillEffectResult> ();
                return results.Count () > 0 && results.Any(x => x.DamageInfos.IsHit());
            }
        }
    }
}
