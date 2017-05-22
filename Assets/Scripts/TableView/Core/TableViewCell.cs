using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.TableView
{
    public abstract class TableViewCell : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
    {
        #region Property

        public int RowNumber
        {
            get { return rowNumber; }
        }

        public abstract string ReuseIdentifier { get; }

        public TableViewCellDidSelectEvent DidSelectEvent;
        public TableViewCellDidHighlightEvent DidHighlightEvent;

        private int rowNumber;

        #endregion

        #region Lifecycle

        void Awake()
        {
            this.gameObject.AddComponent<Selectable>();
        }

        #endregion

        #region Public

        public abstract void SetHighlighted();
        public abstract void SetSelected();
        public abstract void Display();

        public void SetRowNumber(int number)
        {
            rowNumber = number;

            Display();
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetHighlighted();

            if (DidHighlightEvent == null) return;

            DidHighlightEvent.Invoke(rowNumber);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            SetSelected();

            if (DidSelectEvent == null) return;

            DidSelectEvent.Invoke(rowNumber);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetSelected();

            if (DidSelectEvent == null) return;

            DidSelectEvent.Invoke(rowNumber);
        }

        #endregion
    }
}