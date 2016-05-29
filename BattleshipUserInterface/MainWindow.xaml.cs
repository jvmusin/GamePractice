using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Battleship.Implementations;
using Battleship.Interfaces;
using Ninject;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Drawing.Size;

namespace BattleshipUserInterface
{
    public partial class MainWindow
    {
        private readonly IKernel kernel;
        private IGameController controller;

        private readonly Rectangle[,] selfFieldCells;
        private readonly Rectangle[,] opponentFieldCells;

        private readonly Size fieldSize = new Size(10, 10);

        public MainWindow()
        {
            kernel = InitKernel();
            InitializeComponent();

            selfFieldCells = SetUpField(SelfGrid, false);
            opponentFieldCells = SetUpField(OpponentGrid, true);

            CreateNewGameHandle(null, null);
        }

        private Rectangle[,] SetUpField(Grid field, bool shouldBeClickable)
        {
            var result = new Rectangle[fieldSize.Height, fieldSize.Width];

            SetUpFieldSize(field);
            
            for (var row = 0; row < fieldSize.Height; row++)
                for (var column = 0; column < fieldSize.Width; column++)
                {
                    var cell = result[row, column] = DefaultGridCell;
                    field.Children.Add(cell);
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, column);

                    if (shouldBeClickable)
                        AddTurnOnClick(cell, row, column);
                }

            return result;
        }

        private static Rectangle DefaultGridCell => new Rectangle
        {
            Margin = new Thickness(1),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MinWidth = 50,
            MinHeight = 50
        };

        private static void SetUpFieldSize(Grid field)
        {
            field.HorizontalAlignment = HorizontalAlignment.Stretch;
            field.VerticalAlignment = VerticalAlignment.Stretch;
            for (var i = 0; i < 10; i++)
            {
                field.RowDefinitions.Add(new RowDefinition { MinHeight = 10, Height = new GridLength(30) });
                field.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 10, Width = new GridLength(30) });
            }
        }

        private Thread opponentThread;

        private void AddTurnOnClick(UIElement element, int row, int column)
        {
            element.MouseUp += (sender, args) =>
            {
                if ((opponentThread != null && opponentThread.IsAlive) || controller.GameFinished)
                    return;

                controller.Shoot(new CellPosition(row, column));
                UpdateFields();
                if (controller.GameFinished)
                {
                    SetGameStatus("Вы победили!");
                    MessageBox.Show("Вы победили!");
                    return;
                }

                GameStatus.Text = "Ход оппонента";
                opponentThread = new Thread(() =>
                {
                    while (!controller.GameFinished && !controller.FirstPlayerTurns)
                    {
                        var opponentTarget = controller.CurrentPlayer.NextTarget;
                        controller.Shoot(opponentTarget);
                        Thread.Sleep(300);
                        element.Dispatcher.Invoke(UpdateFields);
                    }
                    if (controller.GameFinished)
                    {
                        SetGameStatus("Вы проиграли =(");
                        MessageBox.Show("Вы проиграли =(");
                    }
                    else
                    {
                        SetGameStatus("Ваш ход");
                    }
                });
                opponentThread.Start();
            };
        }

        private void SetGameStatus(string newStatus)
        {
            GameStatus.Dispatcher.Invoke(() => GameStatus.Text = newStatus);
        }

        private static void ColorCells<T>(Rectangle[,] cells, IRectangularReadonlyField<T> field, Func<T, Color> getColor)
        {
            foreach (var position in field.EnumerateCellPositions())
                cells[position.Row, position.Column].Fill 
                    = new SolidColorBrush(getColor(field[position]));
        }

        private void CreateNewGameHandle(object sender, RoutedEventArgs e)
        {
            controller = kernel.Get<IGameController>();
            UpdateFields();
        }

        private void UpdateFields()
        {
            ColorCells(selfFieldCells, controller.FirstPlayer.SelfField, SelfFieldColorer);
            ColorCells(opponentFieldCells, controller.FirstPlayer.OpponentFieldKnowledge, OpponentFieldColorer);
        }

        private static readonly Func<IGameCell, Color> SelfFieldColorer = cell =>
        {
            var shipCell = cell as ShipCell;
            if (shipCell != null)
            {
                return shipCell.Damaged
                    ? FromHex("#750529")
                    : FromHex("#F12869");
            }
            return cell.Damaged
                ? FromHex("#CEBDC9")
                : FromHex("#FFDCE7");
        };

        private static readonly Func<bool?, Color> OpponentFieldColorer = cell =>
        {
            return cell == null
                ? FromHex("#C4FE90")
                : cell == true
                    ? FromHex("#3A7505")
                    : FromHex("#B7CBA8");
        };

        private static Color FromHex(string hexColor)
        {
            return (Color) ColorConverter.ConvertFromString(hexColor);
        }

        private static IKernel InitKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IGameFieldBuilder>().To<GameFieldBuilder>();
            kernel.Bind<IGameField>().ToMethod(context => context.Kernel.Get<IGameFieldBuilder>().GenerateRandomField());
            kernel.Bind<IPlayer>().To<RandomPlayer>();
            kernel.Bind<IGameController>().To<GameController>();

            return kernel;
        }
    }
}
