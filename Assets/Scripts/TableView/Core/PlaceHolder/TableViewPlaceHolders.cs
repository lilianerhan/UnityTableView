using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TableView
{
    public class TableViewPlaceHolders
    {
        #region Property

        private LayoutElement startPlaceHolder;
        private LayoutElement endPlaceholder;

        private static string startPlaceHolderIdentifier = "Start Place Holder";
        private static string endPlaceHolderIdentifier = "End Place Holder";
        private TableViewLayoutOrientation layoutOrientation;

        #endregion

        #region Public

        public TableViewPlaceHolders(Transform transform)
        {
            CreateStartPlaceHolderWithTransform(transform);
            CreateEndPlaceHolderWithTransform(transform);
        }

        public void SetLayoutOrientation(TableViewLayoutOrientation layoutOrientation)
        {
            this.layoutOrientation = layoutOrientation;
        }

        public void UpdatePlaceHoldersWithSize(float startSize, float endSize)
        {
            if (layoutOrientation == TableViewLayoutOrientation.Horizontal)
            {
                startPlaceHolder.preferredWidth = startSize;
                endPlaceholder.preferredWidth = endSize;
            }
            else
            {
                startPlaceHolder.preferredHeight = startSize;
                endPlaceholder.preferredHeight = endSize;
            }

            startPlaceHolder.gameObject.SetActive(startSize > 0);
            endPlaceholder.gameObject.SetActive(endSize > 0);
        }

        #endregion

        #region Private

        private void CreateStartPlaceHolderWithTransform(Transform transform)
        {
            startPlaceHolder = CreatePlaceHolderWithIdentifier(startPlaceHolderIdentifier);
            startPlaceHolder.transform.SetParent(transform, false);
            startPlaceHolder.gameObject.SetActive(false);
        }

        private void CreateEndPlaceHolderWithTransform(Transform transform)
        {
            endPlaceholder = CreatePlaceHolderWithIdentifier(endPlaceHolderIdentifier);
            endPlaceholder.transform.SetParent(transform, false);
            endPlaceholder.gameObject.SetActive(false);
        }

        private LayoutElement CreatePlaceHolderWithIdentifier(string name)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            return gameObject.GetComponent<LayoutElement>();
        }

        #endregion
    }
}