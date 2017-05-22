using UnityEngine;

namespace Assets.Scripts.TableView
{
    public class TableViewContent
    {
        #region Property

        public RectTransform Container { get; private set; }

        private static string containerIdentifier = "Table View Content";

        #endregion

        #region Public

        public TableViewContent(RectTransform transform)
        {
            Container = new GameObject(containerIdentifier, typeof(RectTransform)).GetComponent<RectTransform>();
            Container.SetParent(transform, false);
            Container.gameObject.SetActive(true);
            Container.offsetMin = Vector2.zero;
            Container.offsetMax = Vector2.zero;
        }

        public void SetLayoutOrientation(TableViewLayoutOrientation layoutOrientation)
        {
            if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
            {
                Container.anchorMin = new Vector2(0f, 0f);
                Container.anchorMax = new Vector2(0f, 1f);
                Container.pivot = new Vector2(0.0f, 0.5f);
            }
            else
            {
                Container.anchorMin = new Vector2(0f, 1f);
                Container.anchorMax = new Vector2(1f, 1f);
                Container.pivot = new Vector2(0.5f, 1.0f);
            }
        }

        #endregion
    }
}