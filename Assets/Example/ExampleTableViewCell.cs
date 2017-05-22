using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace Assets.Scripts.Example
{
    public class ExampleTableViewCell : TableViewCell
    {
        public Text text;

        public override string ReuseIdentifier
        {
            get { return "ExampleTableViewCellReuseIdentifier"; }
        }

        public override void SetHighlighted()
        {
            print("CellSetHighlighted : " + RowNumber);
        }

        public override void SetSelected()
        {
            print("CellSetSelected : " + RowNumber);
        }

        public override void Display()
        {
            text.text = "Row " + RowNumber;
        }
    }
}