using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Battleship.Implementations;
using Battleship.Interfaces;
using Ninject;
using Brush = System.Windows.Media.Brush;
using Size = System.Drawing.Size;

namespace BattleshipUserInterface
{
    public partial class MainWindow
    {
        private readonly IKernel container;
        private IGameController controller;

        private IGameFieldBuilder builder;

        private readonly Rectangle[,] selfFieldCells;
        private readonly Rectangle[,] opponentFieldCells;

        private readonly Size fieldSize = new Size(10, 10);

        public MainWindow()
        {
            InitializeComponent();
            container = InitKernel();

            selfFieldCells = SetUpField(SelfGrid, false);
            opponentFieldCells = SetUpField(OpponentGrid, true);

            builder = container.Get<IGameFieldBuilder>();
            InitShipImages();
            UpdateShipsLeftCount();
        }

        private void InitShipImages()
        {
            var imagesToFill = new[]
            {
                SizeOneShipImage,
                SizeTwoShipImage,
                SizeThreeShipImage,
                SizeFourShipImage
            };

            for (var size = 1; size <= imagesToFill.Length; size++)
                for (var i = 0; i < size; i++)
                {
                    var cell = DefaultGridCell;
                    cell.Fill = SelfFieldUndamagedShipCellColor;
                    imagesToFill[size - 1].Children.Add(cell);
                }
        }

        private void UpdateShipsLeftCount()
        {
            var labels = new[]
            {
                SizeOneShipsLeft,
                SizeTwoShipsLeft,
                SizeThreeShipsLeft,
                SizeFourShipsLeft
            };

            for (var size = 1; size <= labels.Length; size++)
                labels[size - 1].Text = builder.ShipsLeft[(ShipType)size].ToString();
        }

        private Rectangle[,] SetUpField(Grid field, bool opponentField)
        {
            var result = new Rectangle[fieldSize.Height, fieldSize.Width];

            SetUpFieldSize(field);

            for (var row = 0; row < fieldSize.Height; row++)
                for (var column = 0; column < fieldSize.Width; column++)
                {
                    var cell = result[row, column] = DefaultGridCell;
                    cell.Fill = opponentField
                        ? OpponentFieldUnknownCellColor
                        : SelfFieldUndamagedEmptyCellColor;
                    field.Children.Add(cell);
                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, column);

                    if (opponentField)
                        AddTurnOnClick(cell, row, column);
                    else
                        AddEditShipOnClick(cell, row, column);
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
            for (var i = 0; i < 10; i++)
            {
                field.RowDefinitions.Add(new RowDefinition { MinHeight = 10, Height = new GridLength(30) });
                field.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 10, Width = new GridLength(30) });
            }
        }

        private Thread opponentThread;

        private void AddTurnOnClick(UIElement element, int row, int column)
        {
            element.MouseLeftButtonUp += (sender, args) =>
            {
                if ((opponentThread != null && opponentThread.IsAlive) || controller.GameFinished)
                    return;

                controller.Shoot(new CellPosition(row, column));
                UpdateFields();
                if (controller.GameFinished)
                {
                    ShowPlayerWonStatus();
                    return;
                }

                UpdateCurrentPlayerStatus();
                opponentThread = new Thread(() =>
                {
                    while (!controller.GameFinished && !controller.FirstPlayerTurns)
                    {
                        var opponentTarget = controller.CurrentPlayer.NextTarget;
                        controller.Shoot(opponentTarget);
                        Thread.Sleep(300);
                        element.Dispatcher.Invoke(UpdateFields);
                    }
                    UpdateCurrentPlayerStatus();
                    if (controller.GameFinished)
                        ShowPlayerLostStatus();
                });
                opponentThread.Start();
            };
        }

        private void AddEditShipOnClick(Shape cell, int row, int column)
        {
            cell.MouseLeftButtonUp += (sender, args) =>
            {
                if (builder == null)
                    return;
                if (builder.TryAddShipCell(new CellPosition(row, column)))
                    cell.Fill = SelfFieldUndamagedShipCellColor;
                UpdateShipsLeftCount();
            };

            cell.MouseRightButtonUp += (sender, args) =>
            {
                if (builder == null)
                    return;
                if (builder.TryRemoveShipCell(new CellPosition(row, column)))
                    cell.Fill = SelfFieldUndamagedEmptyCellColor;
                UpdateShipsLeftCount();
            };
        }

        private void ShowPlayerWonStatus()
        {
            var text = "Вы победили!";
            SetGameStatus(text);
            MessageBox.Show(text);
        }

        private void ShowPlayerLostStatus()
        {
            var text = "Вы проиграли =(";
            SetGameStatus(text);
            MessageBox.Show(text);
        }

        private void UpdateCurrentPlayerStatus()
        {
            var text = controller.FirstPlayerTurns ? "Ваш ход" : "Ход оппонента";
            SetGameStatus(text);
        }

        private void SetGameStatus(string newStatus)
        {
            GameStatus.Dispatcher.Invoke(() => GameStatus.Text = newStatus);
        }

        private static void ColorCells<T>(Rectangle[,] cells, IRectangularReadonlyField<T> field, Func<T, Brush> getBrush)
        {
            foreach (var position in field.EnumerateCellPositions())
                cells[position.Row, position.Column].Fill = getBrush(field[position]);
        }

        private void CreateFieldHandle(object sender, RoutedEventArgs e)
        {
            var me = builder.Build();
            if (me == null)
            {
                MessageBox.Show(this, "Поле заполнено некорректно!");
                return;
            }
            builder = null;

            //TODO Make with Ninject
            controller = new GameController(new RandomPlayer(me), container.Get<IPlayer>());
            HideGroup(BuilderElements);
            ShowGroup(GameFieldElements);
            UpdateFields();
            UpdateCurrentPlayerStatus();
        }

        private void ClearFieldHandle(object sender, MouseButtonEventArgs e)
        {
            builder = container.Get<IGameFieldBuilder>();
            FillSelfFieldUsingBuilder();
            UpdateShipsLeftCount();
        }

        private void GenerateRandomFieldHandle(object sender, MouseButtonEventArgs e)
        {
            builder = container.Get<IGameFieldBuilder>();
            builder.GenerateRandomField();
            FillSelfFieldUsingBuilder();
            UpdateShipsLeftCount();
        }

        private void FillSelfFieldUsingBuilder()
        {
            foreach (var row in Enumerable.Range(0, builder.FieldSize.Height))
                foreach (var column in Enumerable.Range(0, builder.FieldSize.Width))
                    selfFieldCells[row, column].Fill = builder[new CellPosition(row, column)]
                        ? SelfFieldUndamagedShipCellColor
                        : SelfFieldUndamagedEmptyCellColor;
        }

        private IEnumerable<UIElement> BuilderElements => new UIElement[]
        {
            CreateFieldButton,
            GenerateRandomFieldButton,
            ClearFieldButton,
            FieldBuilderCounter
        };

        private IEnumerable<UIElement> GameFieldElements => new UIElement[]
        {
            OpponentField,
            CreateNewGameButton
        };

        private static void HideGroup(IEnumerable<UIElement> elements)
        {
            foreach (var element in elements)
                element.Visibility = Visibility.Collapsed;
        }

        private static void ShowGroup(IEnumerable<UIElement> elements)
        {
            foreach (var element in elements)
                element.Visibility = Visibility.Visible;
        }

        private void UpdateFields()
        {
            ColorCells(selfFieldCells, controller.FirstPlayer.SelfField, SelfFieldColorer);
            ColorCells(opponentFieldCells, controller.FirstPlayer.OpponentFieldKnowledge, OpponentFieldColorer);
        }

        private static readonly Func<IGameCell, Brush> SelfFieldColorer = cell =>
        {
            var shipCell = cell as ShipCell;
            if (shipCell != null)
            {
                return shipCell.Damaged
                    ? SelfFieldDamagedShilCellColor
                    : SelfFieldUndamagedShipCellColor;
            }
            return cell.Damaged
                ? SelfFieldDamagedEmptyCellColor
                : SelfFieldUndamagedEmptyCellColor;
        };

        private static readonly Func<bool?, Brush> OpponentFieldColorer = cell =>
        {
            // ReSharper disable once ConvertToLambdaExpression
            return cell == null
                ? OpponentFieldUnknownCellColor
                : cell == true
                    ? OpponentFieldShipCellColor
                    : OpponentFieldEmptyCellColor;
        };
        
        private void CreateNewGameHandle(object sender, MouseButtonEventArgs e)
        {
            var result = controller == null || controller.GameFinished
                ? MessageBoxResult.OK
                : MessageBox.Show(
                    "Игра не доиграна. Вы действительно хотите начать новую игру?",
                    "Новая игра",
                    MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                controller = null;
                HideGroup(GameFieldElements);
                ShowGroup(BuilderElements);
                SetGameStatus("Расставьте корабли");
                ClearFieldHandle(null, null);
            }
        }

        private void ExitGameHandle(object sender, MouseButtonEventArgs e)
        {
            var result = controller == null || controller.GameFinished
                ? MessageBoxResult.OK
                : MessageBox.Show(
                    "Игра не доиграна. Вы действительно хотите выйти?",
                    "Выход из игры",
                    MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                Application.Current.Shutdown();
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

        private static Color FromHex(string hexColor)
        {
            // ReSharper disable once PossibleNullReferenceException
            return (Color)ColorConverter.ConvertFromString(hexColor);
        }

        #region Brushes

        public static readonly Brush SelfFieldUndamagedShipCellColor = new SolidColorBrush(FromHex("#F12869"));
        public static readonly Brush SelfFieldDamagedShilCellColor = new SolidColorBrush(FromHex("#750529"));
        public static readonly Brush SelfFieldUndamagedEmptyCellColor = new SolidColorBrush(FromHex("#FFDCE7"));
        public static readonly Brush SelfFieldDamagedEmptyCellColor = new SolidColorBrush(FromHex("#CEBDC9"));

        public static readonly Brush OpponentFieldUnknownCellColor = new SolidColorBrush(FromHex("#C4FE90"));
        public static readonly Brush OpponentFieldShipCellColor = new SolidColorBrush(FromHex("#3A7505"));
        public static readonly Brush OpponentFieldEmptyCellColor = new SolidColorBrush(FromHex("#B7CBA8"));

        public static readonly Brush BackgroundColor = new SolidColorBrush(FromHex("#EFFEE1"));

        public static readonly Brush StatusBorderColor = new SolidColorBrush(FromHex("#87F228"));

        public static readonly Brush ExitButtonBackgroundColor = new SolidColorBrush(FromHex("#FE90B3"));

        #endregion
    }
}
