using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.TableView
{
    public class VisibleCells
    {
        #region Property

        public Range Range;
        private Dictionary<int, TableViewCell> cells;

        public int Count
        {
            get { return cells.Count; }
        }

        #endregion

        #region Public

        public VisibleCells()
        {
            Range = new Range(0, 0);
            cells = new Dictionary<int, TableViewCell>();
        }

        public TableViewCell GetCellAtIndex(int index)
        {
            TableViewCell cell = null;
            cells.TryGetValue(index, out cell);
            return cell;
        }

        public void SetCellAtIndex(int index, TableViewCell cell)
        {
            cells[index] = cell;
        }

        public void RemoveCellAtIndex(int index)
        {
            cells.Remove(index);
        }

        #endregion
    }
}