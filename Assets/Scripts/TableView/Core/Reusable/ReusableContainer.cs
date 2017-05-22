using UnityEngine;

namespace Assets.Scripts.TableView
{
    public class ReusableContainer
    {
        #region Property

        public readonly RectTransform Container;
        private static string reusableContainerIdentifier = "Reusable Cells Container";

        #endregion

        #region Public

        public ReusableContainer(RectTransform transform)
        {
            Container = new GameObject(reusableContainerIdentifier, typeof(RectTransform)).GetComponent<RectTransform>();
            Container.SetParent(transform, false);
            Container.gameObject.SetActive(false);
        }

        #endregion
    }
}