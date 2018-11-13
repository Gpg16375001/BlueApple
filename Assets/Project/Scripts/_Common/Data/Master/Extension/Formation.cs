using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class Formation {
    public bool SatisfyTheCondition(BattleLogic.Parameter parameter)
    {
        var conditions = MasterDataTable.formation_invocation_condition.DataList.
            Where (x => x.formation_id == id && x.position == parameter.PositionIndex);

        // 条件がない場合は条件を満たしたと判定する。
        if (conditions.Count () <= 0) {
            return true;
        }

        // 条件判定
        // 同じ条件はorで違う条件はandとして処理する。
        if (!conditions.GroupBy (x => x.condition).
            All (same_conditions => same_conditions.Any (condition => condition.CheckValue (parameter)))
        ) {
            // 条件を満たさない場合は不可
            return false;
        }

        return true;
    }

    public FormationInvocationCondition[] GetInvocationConditions(int position) {
        return MasterDataTable.formation_invocation_condition.DataList.
            Where (x => x.formation_id == id && x.position == position).ToArray();
    }

    public int GetPostionRow(int index)
    {
        if (A1 == index) {
            return 0;
        }
        else if(A2 == index) {
            return 1;
        }
        else if(A3 == index) {
            return 2;
        }
        else if(B1 == index) {
            return 0;
        }
        else if(B2 == index) {
            return 1;
        }
        else if(B3 == index) {
            return 2;
        }
        else if(C1 == index) {
            return 0;
        }
        else if(C2 == index) {
            return 1;
        }
        else if(C3 == index) {
            return 2;
        }
        return -1;
    }

    public int GetPostionColumn(int index)
    {
        if (A1 == index) {
            return 0;
        }
        else if(A2 == index) {
            return 0;
        }
        else if(A3 == index) {
            return 0;
        }
        else if(B1 == index) {
            return 1;
        }
        else if(B2 == index) {
            return 1;
        }
        else if(B3 == index) {
            return 1;
        }
        else if(C1 == index) {
            return 2;
        }
        else if(C2 == index) {
            return 2;
        }
        else if(C3 == index) {
            return 2;
        }
        return -1;
    }

    public int GetPositionIndex(int row, int column)
    {
        if (row == 0) {
            if (column == 0) {
                return A1;
            } else if (column == 1) {
                return B1;
            } else if (column == 2) {
                return C1;
            }
        } else if (row == 1) {
            if (column == 0) {
                return A2;
            } else if (column == 1) {
                return B2;
            } else if (column == 2) {
                return C2;
            }
        } else if (row == 2) {
            if (column == 0) {
                return A3;
            } else if (column == 1) {
                return B3;
            } else if (column == 2) {
                return C3;
            }
        }
        return -1;
    }

    public bool HasPositionSkill(int index) {
        if (A1 == index) {
            return A1_skill_id.HasValue;
        }
        else if(A2 == index) {
            return A2_skill_id.HasValue;
        }
        else if(A3 == index) {
            return A3_skill_id.HasValue;
        }
        else if(B1 == index) {
            return B1_skill_id.HasValue;
        }
        else if(B2 == index) {
            return B2_skill_id.HasValue;
        }
        else if(B3 == index) {
            return B3_skill_id.HasValue;
        }
        else if(C1 == index) {
            return C1_skill_id.HasValue;
        }
        else if(C2 == index) {
            return C2_skill_id.HasValue;
        }
        else if(C3 == index) {
            return C3_skill_id.HasValue;
        }
        return false;
    }

	public Skill GetPositionSkill(int index) {
		if (A1_skill_id.HasValue && A1 == index) {
			return MasterDataTable.skill[A1_skill_id.Value];
		}
		else if(A2_skill_id.HasValue && A2 == index) {
			return MasterDataTable.skill[A2_skill_id.Value];
		}
		else if(A3_skill_id.HasValue && A3 == index) {
			return MasterDataTable.skill[A3_skill_id.Value];
		}
		else if(B1_skill_id.HasValue && B1 == index) {
			return MasterDataTable.skill[B1_skill_id.Value];
		}
		else if(B2_skill_id.HasValue && B2 == index) {
			return MasterDataTable.skill[B2_skill_id.Value];
		}
		else if(B3_skill_id.HasValue && B3 == index) {
			return MasterDataTable.skill[B3_skill_id.Value];
		}
		else if(C1_skill_id.HasValue && C1 == index) {
			return MasterDataTable.skill[C1_skill_id.Value];
		}
		else if(C2_skill_id.HasValue && C2 == index) {
			return MasterDataTable.skill[C2_skill_id.Value];
		}
		else if(C3_skill_id.HasValue && C3 == index) {
			return MasterDataTable.skill[C3_skill_id.Value];
		}
		return null;
	}
}
