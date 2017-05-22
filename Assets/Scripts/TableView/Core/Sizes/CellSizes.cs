using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.TableView
{
    public class CellSizes
    {
        #region Property

        private float[] rowSize;
        private float[] cumulativeRowSize;
        public int CumulativeIndex = -1;

        public int RowsCount
        {
            get { return rowSize.Length; }
        }

        public int CumulativeRowsCount
        {
            get { return cumulativeRowSize.Length; }
        }

        #endregion

        #region Public

        public void SetRowsCount(int count)
        {
            CumulativeIndex = -1;
            rowSize = new float[count];
            cumulativeRowSize = new float[count];
        }

        public void SetSizeForRow(float size, int row)
        {
            if (size <= 0) return;
            if (row >= RowsCount) return;

            rowSize[row] = size;
        }

        public float SizeForRow(int row)
        {
            return rowSize[row];
        }

        public float SumWithRange(Range range)
        {
            float sum = 0;

            for (int i = range.from; i < range.count; i++)
            {
                sum += SizeForRow(i);
            }

            return sum;
        }

        public float GetCumulativeRowSize(int row)
        {
            while (CumulativeIndex < row)
            {
                CumulativeIndex++;
                cumulativeRowSize[CumulativeIndex] = rowSize[CumulativeIndex];

                if (CumulativeIndex > 0)
                {
                    cumulativeRowSize[CumulativeIndex] += cumulativeRowSize[CumulativeIndex - 1];
                }
            }
            return cumulativeRowSize[row];
        }

        public int FindIndexOfRowAtPosition(float position)
        {
            return FindIndexOfRowAtPosition(position, 0, cumulativeRowSize.Length - 1);
        }

        public int FindIndexOfRowAtPosition(float position, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex) return startIndex;

            int midIndex = (startIndex + endIndex) / 2;

            if (GetCumulativeRowSize(midIndex) > position)
            {
                return FindIndexOfRowAtPosition(position, startIndex, midIndex);
            }
            else
            {
                return FindIndexOfRowAtPosition(position, midIndex + 1, endIndex);
            }
        }

        #endregion
    }
}