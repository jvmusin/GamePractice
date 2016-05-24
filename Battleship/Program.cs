using System.Drawing;
using System.Windows.Forms;
using Battleship.GUI;

namespace Battleship
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application.Run(new MenueForm { ClientSize = new Size(300, 300) });
        }
    }
}
