using System;
using System.Drawing;
using System.Windows.Forms;

namespace Battleship.GUI
{
    public class MenueForm : Form
    {
        private const int ButtonsCount = 1;
        private const int SideFill =100;

        public MenueForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            var startButton = new Button
            {
                Text = "Start new game",
                Dock = DockStyle.Fill
            };
            startButton.Click += CreateBattleshipFiled;

            var table = new TableLayoutPanel();

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, SideFill));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, SideFill));
            
            table.Controls.Add(new Panel(), 0, 0);
            table.Controls.Add(startButton, 0, 1);
            table.Controls.Add(new Panel(), 0, 2);

            table.Dock = DockStyle.Fill;
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