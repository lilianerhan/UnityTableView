using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TableView
{
    public class TableViewScroll
    {
        #region Property

        public RectTransform Content
        {
            get { return scrollRect.content; }
        }

        public float Size
        {
            get
            {
                if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
                {
                    return Content.rect.width;
                }
                else
                {
                    return Content.rect.height;
                }
            }
        }

        public float SizeDelta
        {
            get
            {
                if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
                {
                    return Content.sizeDelta.y;
                }
                else
                {
                    return Content.sizeDelta.x;
                }
            }
            set
            {
                if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
                {
                    Content.sizeDelta = new Vector2(value, Content.sizeDelta.y);
                }
                else
                {
                    Content.sizeDelta = new Vector2(Content.sizeDelta.x, value);
                }
            }
        }

        public ScrollRect.ScrollRectEvent OnValueChanged
        {
            get { return scrollRect.onValueChanged; }
        }

        public ScrollRect.MovementType MovementType
        {
            get { return scrollRect.movementType; }
            set { scrollRect.movementType = value; }
        }

        public float Elasticity
        {
            get { return scrollRect.elasticity; }
            set { scrollRect.elasticity = value; }
        }

        public bool Inertia
        {
            get { return scrollRect.inertia; }
            set { scrollRect.inertia = value; }
        }

        public float DecelerationRate
        {
            get { return scrollRect.decelerationRate; }
            set { scrollRect.decelerationRate = value; }
        }

        public float ScrollSensitivity
        {
            get { return scrollRect.scrollSensitivity; }
            set { scrollRect.scrollSensitivity = value; }
        }

        private ScrollRect scrollRect;
        private TableViewLayoutOrientation layoutOrientation;

        #endregion

        #region Public

        public TableViewScroll(GameObject gameObject, RectTransform transform)
        {
            scrollRect = gameObject.AddComponent<ScrollRect>();
            scrollRect.content = transform;
            scrollRect.gameObject.SetActive(true);
            scrollRect.movementType = MovementType;
            scrollRect.elasticity = Elasticity;
            scrollRect.inertia = Inertia;
            scrollRect.decelerationRate = DecelerationRate;
            scrollRect.scrollSensitivity = ScrollSensitivity;
        }

        public void SetLayoutOrientation(TableViewLayoutOrientation layoutOrientation)
        {
            this.layoutOrientation = layoutOrientation;

            if (this.layoutOrientation == TableViewLayoutOrientation.Horizontal)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
            else
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
            }
        }

        public void SetNormalizedPosition(float normalizedPosition)
        {
            if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
            {
                scrollRect.horizontalNormalizedPosition = normalizedPosition;
            }
            else
            {
                scrollRect.verticalNormalizedPosition = normalizedPosition;
            }
        }

        #endregion
    }
}