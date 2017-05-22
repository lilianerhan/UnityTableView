using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Assets.Scripts.TableView
{
    public class TableView : MonoBehaviour, ITableView
    {
        #region Property

        [SerializeField] private TableViewLayoutOrientation layoutOrientation = TableViewLayoutOrientation.Vertical;
        [SerializeField] private float interItemSpacing = 10.0f;
        [SerializeField] private RectOffset padding;

        [SerializeField] private bool inertia = true;
        [SerializeField] private ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic;
        [SerializeField] private float elasticity = 0.1f;
        [SerializeField] private float scrollSensitivity = 1.0f;
        [SerializeField] private float decelerationRate = 0.135f;

        [SerializeField] private bool scrollToHighlighted = true;
        [SerializeField] private float scrollingSpeed = 0.2f;

        public ITableViewDataSource DataSource
        {
            get { return dataSource; }
            set { dataSource = value; requiresReload = true; }
        }

        public ITableViewDelegate Delegate
        {
            get { return tableViewDelegate; }
            set { tableViewDelegate = value; }
        }

        public Range VisibleRange
        {
            get { return visibleCells.Range; }
        }

        public float ContentSize
        {
            get { return tableViewScroll.Size - tableViewSize; }
        }

        public float Position
        {
            get { return position; }
        }

        private CellSizes cellSizes;
        private PrefabCells prefabCells;
        private VisibleCells visibleCells;
        private ReusableCells reusableCells;

        private TableViewScroll tableViewScroll;
        private TableViewLayout tableViewLayout;
        private TableViewContent tableViewContent;
        private TableViewPlaceHolders tableViewPlaceHolders;

        private ITableViewDataSource dataSource;
        private ITableViewDelegate tableViewDelegate;

        private float position;

        private bool isEmpty;
        private bool requiresReload;
        private bool requiresRefresh;

        private bool isVertical
        {
            get { return (tableViewLayout.Orientation == TableViewLayoutOrientation.Vertical); }
        }

        private float tableViewSize
        {
            get
            {
                Rect rect = (this.transform as RectTransform).rect;
                return isVertical ? rect.height : rect.width;
            }
        }

        #endregion


        #region Lifecycle

        void Awake()
        {
            isEmpty = true;

            cellSizes = new CellSizes();
            prefabCells = new PrefabCells();
            visibleCells = new VisibleCells();

            reusableCells = new ReusableCells();
            reusableCells.AddToParentTransform(this.transform as RectTransform);

            tableViewContent = new TableViewContent(this.transform as RectTransform);
            tableViewContent.SetLayoutOrientation(layoutOrientation);

            tableViewScroll = new TableViewScroll(this.gameObject, tableViewContent.Container);
            tableViewScroll.SetLayoutOrientation(layoutOrientation);
            tableViewScroll.Elasticity = elasticity;
            tableViewScroll.MovementType = movementType;
            tableViewScroll.Inertia = inertia;
            tableViewScroll.DecelerationRate = decelerationRate;
            tableViewScroll.ScrollSensitivity = scrollSensitivity;

            tableViewPlaceHolders = new TableViewPlaceHolders(tableViewScroll.Content);
            tableViewPlaceHolders.SetLayoutOrientation(layoutOrientation);

            tableViewLayout = new TableViewLayout(layoutOrientation);
            tableViewLayout.AddToParent(tableViewScroll.Content.gameObject);
            tableViewLayout.Spacing = interItemSpacing;
            tableViewLayout.Padding = padding;

            this.gameObject.AddComponent<RectMask2D>();
            this.gameObject.AddComponent<CanvasRenderer>();
        }

        void Update()
        {
            if (requiresReload)
            {
                ReloadData();
            }
        }

        void LateUpdate()
        {
            if (requiresRefresh)
            {
                RefreshVisibleCells();
            }
        }

        void OnEnable()
        {
            tableViewScroll.OnValueChanged.AddListener(ScrollViewValueChanged);
        }

        void OnDisable()
        {
            tableViewScroll.OnValueChanged.RemoveListener(ScrollViewValueChanged);
        }

        #endregion


        #region Public

        public TableViewCell ReusableCellForRow(string reuseIdentifier, int row)
        {
            TableViewCell cell = reusableCells.GetReusableCell(reuseIdentifier);

            if (cell == null)
            {
                cell = CreateCellFromPrefab(reuseIdentifier, row);
            }

            return cell;
        }

        public TableViewCell CellForRow(int row)
        {
            return visibleCells.GetCellAtIndex(row);
        }

        public float PositionForRow(int row)
        {
            return cellSizes.GetCumulativeRowSize(row) - cellSizes.SizeForRow(row) + tableViewLayout.StartPadding;
        }

        public void ReloadData()
        {
            int numberOfRows = dataSource.NumberOfRowsInTableView(this);
            cellSizes.SetRowsCount(numberOfRows);
            isEmpty = (numberOfRows == 0);
            RemoveAllCells();

            if (isEmpty) return;

            for (int i = 0; i < numberOfRows; i++)
            {
                float rowSize = dataSource.SizeForRowInTableView(this, i) + tableViewLayout.Spacing;
                cellSizes.SetSizeForRow(rowSize, i);
            }

            tableViewScroll.SizeDelta = cellSizes.GetCumulativeRowSize(numberOfRows - 1) + tableViewLayout.EndPadding;

            CreateCells();
            requiresReload = false;
        }

        public void RegisterPrefabForCellReuseIdentifier(GameObject prefab, string cellReuseIdentifier)
        {
            prefabCells.RegisterPrefabForCellReuseIdentifier(prefab, cellReuseIdentifier);
        }

        public void SetPosition(float newPosition)
        {
            if (isEmpty) { return; }

            newPosition = Mathf.Clamp(newPosition, 0, PositionForRow(cellSizes.RowsCount - 1));

            if (position != newPosition)
            {
                position = newPosition;
                requiresRefresh = true;
                float normalizedPosition = newPosition / ContentSize;
                float relativeScroll = 0;

                if (isVertical)
                {
                    relativeScroll = 1 - normalizedPosition;
                }
                else
                {
                    relativeScroll = normalizedPosition;
                }

                tableViewScroll.SetNormalizedPosition(relativeScroll);
            }
        }

        public void SetPosition(float newPosition, float time)
        {
            StartCoroutine(AnimateToPosition(newPosition, time));
        }

        #endregion


        #region Private

        private TableViewCell CreateCellFromPrefab(string reuseIdentifier, int row)
        {
            if (!prefabCells.IsRegisteredCellReuseIdentifier(reuseIdentifier)) return null;

            GameObject cellPrefab = prefabCells.PrefabForCellReuseIdentifier(reuseIdentifier);
            TableViewCell cell = Instantiate(cellPrefab).GetComponent<TableViewCell>();
            cell.SetRowNumber(row);

            return ConfigureCellWithRowAtEnd(cell, row, true);
        }

        private void ScrollViewValueChanged(Vector2 newScrollValue)
        {
            float relativeScroll = 0;

            if (isVertical)
            {
                relativeScroll = 1 - newScrollValue.y;
            }
            else
            {
                relativeScroll = newScrollValue.x;
            }

            position = relativeScroll * ContentSize;
            requiresRefresh = true;
        }

        private void CreateCells()
        {
            RemoveAllCells();
            SetInitialVisibleCells();
        }

        private void RemoveAllCells()
        {
            while (visibleCells.Count > 0)
            {
                RemoveCell(false);
            }

            visibleCells.Range = new Range(0, 0);
        }

        private Range CurrentVisibleCellsRange()
        {
            float startPosition = Math.Max(position - tableViewLayout.StartPadding - tableViewSize, 0);
            float endPosition = position + (tableViewSize * 2.0f);

            if (endPosition > tableViewScroll.Size)
            {
                endPosition = tableViewScroll.Size;
            }

            int startIndex = cellSizes.FindIndexOfRowAtPosition(startPosition);
            int endIndex = cellSizes.FindIndexOfRowAtPosition(endPosition);

            if (endIndex == cellSizes.RowsCount - 1)
            {
                endIndex = cellSizes.RowsCount;
            }

            int cellsCount = endIndex - startIndex;

            return new Range(startIndex, cellsCount);
        }

        private void SetInitialVisibleCells()
        {
            Range currentRange = CurrentVisibleCellsRange();

            for (int i = 0; i < currentRange.count; i++)
            {
                CreateCell(currentRange.from + i, true);
            }

            visibleCells.Range = currentRange;
            UpdatePaddingElements();
        }

        private void RefreshVisibleCells()
        {
            requiresRefresh = false;

            if (isEmpty) return;

            Range previousRange = visibleCells.Range;
            Range currentRange = CurrentVisibleCellsRange();

            if (currentRange.from > previousRange.Last() || currentRange.Last() < previousRange.from)
            {
                CreateCells();
                return;
            }

            RemoveCellsIfNeededWithRanges(previousRange, currentRange);
            CreateCellsIfNeededWithRanges(previousRange, currentRange);

            visibleCells.Range = currentRange;

            UpdatePaddingElements();
        }

        private void RemoveCellsIfNeededWithRanges(Range previousRange, Range currentRange)
        {
            for (int i = previousRange.from; i < currentRange.from; i++)
            {
                RemoveCell(false);
            }

            for (int i = currentRange.Last(); i < previousRange.Last(); i++)
            {
                RemoveCell(true);
            }
        }

        private void CreateCellsIfNeededWithRanges(Range previousRange, Range currentRange)
        {
            for (int i = previousRange.from - 1; i >= currentRange.from; i--)
            {
                CreateCell(i, false);
            }

            for (int i = previousRange.Last() + 1; i <= currentRange.Last(); i++)
            {
                CreateCell(i, true);
            }
        }

        private void CreateCell(int row, bool atEnd)
        {
            TableViewCell cell = dataSource.CellForRowInTableView(this, row);
            cell = ConfigureCellWithRowAtEnd(cell, row, atEnd);
        }

        private TableViewCell ConfigureCellWithRowAtEnd(TableViewCell cell, int row, bool atEnd)
        {
            cell.SetRowNumber(row);
            cell.transform.SetParent(tableViewScroll.Content, false);

            float cellSize = cellSizes.SizeForRow(row) - ((row > 0) ? tableViewLayout.Spacing : 0);
            CreateLayoutIfNeededForCellWithSize(cell, cellSize);

            cell.DidHighlightEvent.RemoveListener(CellDidHighlight);
            cell.DidHighlightEvent.AddListener(CellDidHighlight);

            cell.DidSelectEvent.RemoveListener(CellDidSelect);
            cell.DidSelectEvent.AddListener(CellDidSelect);

            visibleCells.SetCellAtIndex(row, cell);

            if (atEnd)
            {
                cell.transform.SetSiblingIndex(tableViewScroll.Content.childCount - 2);
            }
            else
            {
                cell.transform.SetSiblingIndex(1);
            }

            return cell;
        }

        private void RemoveCell(bool last)
        {
            int row = last ? visibleCells.Range.Last() : visibleCells.Range.from;
            TableViewCell removedCell = visibleCells.GetCellAtIndex(row);
            removedCell.DidHighlightEvent.RemoveListener(CellDidHighlight);
            reusableCells.AddReusableCell(removedCell);
            visibleCells.RemoveCellAtIndex(row);
            visibleCells.Range.count -= 1;

            if (!last)
            {
                visibleCells.Range.from += 1;
            }
        }

        private void CreateLayoutIfNeededForCellWithSize(TableViewCell cell, float size)
        {
            LayoutElement layoutElement = cell.GetComponent<LayoutElement>();

            if (layoutElement == null)
            {
                layoutElement = cell.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.preferredHeight = 0;
            layoutElement.preferredWidth = 0;

            if (isVertical)
            {
                layoutElement.preferredHeight = size;
            }
            else
            {
                layoutElement.preferredWidth = size;
            }
        }

        private void UpdatePaddingElements()
        {
            Range startRange = new Range(0, visibleCells.Range.from);
            float startSize = cellSizes.SumWithRange(startRange);

            Range endRange = new Range(visibleCells.Range.from, visibleCells.Range.Last() + 1);
            float hiddenElementsSizeSum = startSize + cellSizes.SumWithRange(endRange);
            float endSize = tableViewScroll.Size - hiddenElementsSizeSum;
            endSize -= tableViewLayout.StartPadding;
            endSize -= tableViewLayout.EndPadding;
            endSize -= tableViewLayout.Spacing;

            tableViewPlaceHolders.UpdatePlaceHoldersWithSize(startSize, endSize);
        }

        private IEnumerator AnimateToPosition(float newPosition, float time)
        {
            float startTime = Time.time;
            float initialPosition = position;
            float endTime = startTime + time;

            while (Time.time < endTime)
            {
                float relativeProgress = Mathf.InverseLerp(startTime, endTime, Time.time);
                SetPosition(Mathf.Lerp(initialPosition, newPosition, relativeProgress));
                yield return new WaitForEndOfFrame();
            }

            SetPosition(newPosition);
        }

        private void CellDidHighlight(int row)
        {
            if (tableViewDelegate != null)
            {
                tableViewDelegate.TableViewDidHighlightCellForRow(this, row);
            }

            if (scrollToHighlighted)
            {
                MakeVisibleRowIfNeeded(row);
            }
        }

        private void CellDidSelect(int row)
        {
            if (tableViewDelegate != null)
            {
                tableViewDelegate.TableViewDidSelectCellForRow(this, row);
            }
        }

        private void MakeVisibleRowIfNeeded(int row)
        {
            ScrollToStartIfNeededWithRow(row);
            ScrollToEndIfNeededWithRow(row);
        }

        private void ScrollToStartIfNeededWithRow(int row)
        {
            float rowStart = PositionForRow(row);

            if (row == 0)
            {
                rowStart -= tableViewLayout.StartPadding;
            }

            if (position > rowStart)
            {
                SetPosition(rowStart, scrollingSpeed);
            }
        }

        private void ScrollToEndIfNeededWithRow(int row)
        {
            float rowStart = PositionForRow(row);
            float rowSize = cellSizes.SizeForRow(row);
            float rowEnd = rowStart + rowSize + tableViewLayout.Spacing;
            float contentEndPosition = position + tableViewSize;
            int rowsCount = cellSizes.RowsCount - 1;

            if (row == rowsCount)
            {
                rowEnd += tableViewLayout.EndPadding;
            }

            if (rowEnd > tableViewScroll.Size)
            {
                rowEnd = tableViewScroll.Size;
            }

            if (rowEnd > contentEndPosition)
            {
                float rowEndPosition = rowEnd - tableViewSize;
                SetPosition(rowEndPosition, scrollingSpeed);
            }
        }

        #endregion
    }
}