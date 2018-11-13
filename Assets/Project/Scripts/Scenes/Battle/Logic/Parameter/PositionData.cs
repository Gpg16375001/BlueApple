using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic {
    /// <summary>
    /// バトルのポジションデータ
    /// </summary>
    [Serializable]
    public class PositionData {
        public const int MAX_ROW = 3;
        public const int MAX_COLUMN = 3;

        [SerializeField]
        public bool isPlayer;
        [SerializeField]
        public int row;

        public int MaxRow {
            get {
                if (UnitSize == null) {
                    return row;
                }

                var maxRow = UnitSize.SizeList.Max (x => x.row);
                return Mathf.Clamp(row + maxRow, 0, MAX_ROW - 1);
            }
        }

        public int MinRow {
            get {
                if (UnitSize == null) {
                    return row;
                }
                var minRow = UnitSize.SizeList.Min (x => x.row);
                return Mathf.Clamp(row + minRow, 0, MAX_ROW - 1);
            }
        }


        [SerializeField]
        public int column;

        public int MaxColumn {
            get {
                if (UnitSize == null) {
                    return column;
                }

                var maxColumn = UnitSize.SizeList.Max (x => x.column);
                return Mathf.Clamp(column + maxColumn, 0, MAX_COLUMN - 1);
            }
        }

        public int MinColumn {
            get {
                if (UnitSize == null) {
                    return column;
                }
                var minColumn = UnitSize.SizeList.Min (x => x.column);
                return Mathf.Clamp(column + minColumn, 0, MAX_COLUMN - 1);
            }
        }

        private EnemyUnitSizeDefine _unitSize;
        public EnemyUnitSizeDefine UnitSize {
            get {
                if (UnitSizeID <= 0) {
                    return null;
                }
                if (_unitSize == null) {
                    _unitSize = MasterDataTable.enemy_unit_size_define[UnitSizeID];
                }
                return _unitSize;
            }
        }

        [SerializeField]
        private int _unitSizeID;
        public int UnitSizeID{
            get {
                return _unitSizeID;
            }
            set {
                if (_unitSizeID != value) {
                    // 値が変更になった場合はサイズ情報を削除
                    _unitSizeID = value;
                    _unitSize = null;
                }
            }
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

        public override bool Equals(object obj)
        {
            var pos = obj as PositionData;
            if (pos == null) {
                return false;
            }
            return pos != null && 
                pos.isPlayer == this.isPlayer && 
                pos.row == this.row &&
                pos.column == this.column &&
                pos.UnitSizeID == this.UnitSizeID;
        }

        public bool Equals(bool isPlayer, int row, int column)
        {
            return isPlayer == this.isPlayer &&
                row == this.row &&
                column == this.column;
        }

        public bool InArea(bool isPlayer, int row, int column)
        {
            if (UnitSize != null) {
                // サイズないであれば同一とみなす。
                if (isPlayer == this.isPlayer) {
                    return UnitSize.SizeList.Any (x => x.row + this.row == row && x.column + this.column == column);
                }
                return false;
            }
            return isPlayer == this.isPlayer &&
                row == this.row &&
                column == this.column;
        }

        public override string ToString ()
        {
            return string.Format ("[BattleLogic.PositionData] ({0}, {1}, {2})",
                isPlayer, row, column);
        }

        public static bool operator ==(PositionData a, PositionData b)
        {
            // 同一のインスタンスを参照している場合は true
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // どちらか片方でも null なら false
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Equals(b);
        }
        public static bool operator !=(PositionData a, PositionData b)
        {
            return !(a == b);
        }
    }

    public class PositionDataEquality : IEqualityComparer<PositionData>
    {
        public bool Equals(PositionData p1, PositionData p2)
        {
            return p1 == p2;
        }

        public int GetHashCode(PositionData p)
        {
            return p.GetHashCode ();
        }
    }
}