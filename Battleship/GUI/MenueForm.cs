using System;
using System.Drawing;
using System.Windows.Forms;

namespace Battleship.GUI
{
    public class MenueForm : Form
    {
        private const int ButtonsCount = 3;

        public MenueForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            var table = new TableLayoutPanel();

            const int columnCount = 3;
            for (var i = 0; i < columnCount; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columnCount));
            }

            for (var i = 0; i < ButtonsCount + 2; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / ButtonsCount));
            }

            var button = new Button { Text = "Start new game" };
            button.Dock = DockStyle.Fill;
            button.Click += CreateBattleshipFiled;
            table.Controls.Add(button, 1, 1);

            table.Dock = DockStyle.None;
            Controls.Add(table);
        }

        private void CreateBattleshipFiled(object sender, EventArgs e)
        {
            Hide();
            var fieldForm = new CreateBattleshipFieldForm();
            fieldForm.Closed += (s, args) => Close();
            fieldForm.Show();
        }
    }
}