using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TableView
{
    public class ReusableCells
    {
        #region Property

        private ReusableContainer reusableContainer;
        private readonly Dictionary<string, LinkedList<TableViewCell>> reusableCells;

        #endregion

        #region Public

        public ReusableCells()
        {
            reusableCells = new Dictionary<string, LinkedList<TableViewCell>>();
        }

        public void AddToParentTransform(RectTransform transform)
        {
            reusableContainer = new ReusableContainer(transform);
        }

        public void AddReusableCell(TableViewCell cell)
        {
            if (!IsValidContainer()) return;
            if (string.IsNullOrEmpty(cell.ReuseIdentifier)) return;

            if (!reusableCells.ContainsKey(cell.ReuseIdentifier))
            {
                reusableCells.Add(cell.ReuseIdentifier, new LinkedList<TableViewCell>());
            }

            reusableCells[cell.ReuseIdentifier].AddLast(cell);
            cell.transform.SetParent(reusableContainer.Container, false);
        }

        public TableViewCell GetReusableCell(string reuseIdentifier)
        {
            LinkedList<TableViewCell> reusableCellsList;

            if (!reusableCells.TryGetValue(reuseIdentifier, out reusableCellsList)) return null;
            if (reusableCellsList.Count == 0) return null;

            TableViewCell reusableCell = reusableCellsList.First.Value;
            reusableCellsList.RemoveFirst();

            return reusableCell;
        }

        #endregion

        #region Private

        private bool IsValidContainer()
        {
            if (reusableContainer == null)
            {
                throw new Exception("Reusable Container can not be null !!!");
            }

            return true;
        }

        #endregion
    }
}