using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationIcon : MonoBehaviour {

    public FormationIconCell[] Cells;

    /// <summary>
    /// positionを赤色表示で陣形表示を行う
    /// </summary>
    /// <param name="formation">Formation.</param>
    public void Init(Formation formation, int position = -1)
    {
        // 一旦、全ての表示を落とす。
        System.Array.ForEach (Cells, x => x.SetVisble (false, false));

        for (int i = 1; i <= Party.PartyCardMax; ++i) {
            int column = formation.GetPostionColumn (i);
            int row = formation.GetPostionRow (i);

            var cell = GetIconCell (row, column);
            cell.SetVisble (true, position == i);
        }
    }

    private FormationIconCell GetIconCell(int row, int column)
    {
        int index = row * 3 + column;
        return Cells[index];
    }
}
