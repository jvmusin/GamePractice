using System;
using System.Windows.Forms;

namespace Battleship.GUI
{
    public class MenueForm : Form
    {
        private const int SideFill = 100;

        public MenueForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            var startButton = new Button
            {
                Text = "Start new game",
                Dock = DockStyle.Fill
            };
            startButton.Click += CreateBattleshipFiled;

            var buttonsTable = GetButtonsTableLayoutPanel(startButton);
            var table = GetMainTableLayoutPanel(buttonsTable);
            Controls.Add(table);
        }

        private void CreateBattleshipFiled(object sender, EventArgs e)
        {
            Hide();
            var fieldForm = new CreateBattleshipFieldForm();
            fieldForm.Closed += (s, args) => Close();
            fieldForm.Show();
        }

        private TableLayoutPanel GetButtonsTableLayoutPanel(params Button[] buttons)
        {
            var table = new TableLayoutPanel();

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, SideFill));
            table.Controls.Add(new Panel());

            foreach (var button in buttons)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / buttons.Length * 2));
                table.Controls.Add(button);
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / buttons.Length * 2));
                table.Controls.Add(new Panel());
            }

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, SideFill));

            table.Dock = DockStyle.Fill;

            return table;
        }

        private TableLayoutPanel GetMainTableLayoutPanel(TableLayoutPanel innerTable)
        {
            var table = new TableLayoutPanel();
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SideFill));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SideFill));
            

            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(innerTable, 1, 0);
            table.Controls.Add(new Panel(), 2, 0);
            table.Dock = DockStyle.Fill;

            return table;
        }
    }
}