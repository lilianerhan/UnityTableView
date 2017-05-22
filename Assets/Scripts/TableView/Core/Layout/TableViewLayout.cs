using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TableView
{
    public enum TableViewLayoutOrientation
    {
        Vertical,
        Horizontal
    }

    public class TableViewLayout
    {
        #region Property

        public float Spacing
        {
            get { return layout.spacing; }
            set { layout.spacing = value; }
        }

        public RectOffset Padding
        {
            get { return layout.padding; }
            set { layout.padding = value; }
        }

        public int StartPadding
        {
            get
            {
                if (Orientation == TableViewLayoutOrientation.Horizontal)
                {
                    return layout.padding.left;
                }
                else
                {
                    return layout.padding.top;
                }
            }
            set
            {
                if (Orientation == TableViewLayoutOrientation.Horizontal)
                {
                    layout.padding.left = value;
                }
                else
                {
                    layout.padding.top = value;
                }
            }
        }

        public int EndPadding
        {
            get
            {
                if (Orientation == TableViewLayoutOrientation.Horizontal)
                {
                    return layout.padding.right;
                }
                else
                {
                    return layout.padding.bottom;
                }
            }
            set
            {
                if (Orientation == TableViewLayoutOrientation.Horizontal)
                {
                    layout.padding.right = value;
                }
                else
                {
                    layout.padding.bottom = value;
                }
            }
        }

        public TableViewLayoutOrientation Orientation { get; private set; }

        private HorizontalOrVerticalLayoutGroup layout;

        #endregion

        #region Public

        public TableViewLayout(TableViewLayoutOrientation orientation)
        {
            Orientation = orientation;
        }

        public void AddToParent(GameObject gameObject)
        {
            if (Orientation == TableViewLayoutOrientation.Horizontal)
            {
                layout = gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childForceExpandHeight = true;
                layout.childForceExpandWidth = false;
            }
            else
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();
                layout.childAlignment = TextAnchor.UpperCenter;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
            }
        }

        #endregion
    }
}