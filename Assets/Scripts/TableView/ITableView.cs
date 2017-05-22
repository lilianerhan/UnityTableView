using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.TableView
{
    public interface ITableView
    {
        ITableViewDataSource DataSource { get; set; }
        ITableViewDelegate Delegate { get; set; }

        Range VisibleRange { get; }
        float ContentSize { get; }
        float Position { get; }

        TableViewCell ReusableCellForRow(string reuseIdentifier, int row);
        TableViewCell CellForRow(int row);
        float PositionForRow(int row);
        void ReloadData();

        void SetPosition(float newPosition);
        void SetPosition(float newPosition, float time);

        void RegisterPrefabForCellReuseIdentifier(GameObject prefab, string cellReuseIdentifier);
    }
}