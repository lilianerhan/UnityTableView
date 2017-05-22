using Assets.Scripts.TableView;
using UnityEngine;

namespace Assets.Scripts.Example
{
    public class ExampleViewController : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public TableView.TableView tableView;

        void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;

            GameObject prefab = Resources.Load("ExampleTableViewCell") as GameObject;
            tableView.RegisterPrefabForCellReuseIdentifier(prefab, "ExampleTableViewCellReuseIdentifier");
        }

        public int NumberOfRowsInTableView(TableView.TableView tableView)
        {
            return 100;
        }

        public float SizeForRowInTableView(TableView.TableView tableView, int row)
        {
            return Random.Range(50.0f, 200.0f);
        }

        public TableViewCell CellForRowInTableView(TableView.TableView tableView, int row)
        {
            TableViewCell cell = tableView.ReusableCellForRow("ExampleTableViewCellReuseIdentifier", row);
            cell.name = "Cell " + row;
            return cell;
        }

        public void TableViewDidHighlightCellForRow(TableView.TableView tableView, int row)
        {
            print("TableViewDidHighlightCellForRow : " + row);
        }

        public void TableViewDidSelectCellForRow(TableView.TableView tableView, int row)
        {
            print("TableViewDidSelectCellForRow : " + row);
        }
    }
}