﻿using System;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Battleship.Implementations;
using Battleship.Interfaces;
using System.Drawing;
using System.Windows;
using Ninject;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Size = System.Drawing.Size;

namespace TestGraphicUserInterface
{
    public partial class MainWindow
    {
        private readonly IKernel kernel;
        private IGameController controller;

        private readonly Button[,] selfFieldGridLabels;
        private readonly Button[,] opponentFieldGridLabels;

        private readonly Size FieldSize = new Size(10, 10);

        public MainWindow()
        {
            kernel = InitKernel();
            InitializeComponent();

            selfFieldGridLabels = SetUpField(SelfGrid);
            opponentFieldGridLabels = SetUpField(OpponentGrid);
        }

        private Button[,] SetUpField(Grid grid)
        {
            var result = new Button[FieldSize.Height, FieldSize.Width];

            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            for (var row = 0; row < FieldSize.Height; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition { MinHeight = 10, Height = new GridLength(30) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 10, Width = new GridLength(30) });
                for (var column = 0; column < FieldSize.Width; column++)
                {
                    var label = result[row, column] = new Button
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Content = "   ",
                        Margin = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        MinWidth = 50,
                        MinHeight = 50
                    };
                    grid.Children.Add(label);
                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, column);

                    var row0 = row;
                    var column0 = column;
                    label.Click += (sender, args) =>
                    {
                        controller.Shoot(new CellPosition(row0, column0));
                        while (!controller.GameFinished && !controller.FirstPlayerTurns)
                            controller.Shoot(controller.CurrentPlayer.NextTarget);
                        UpdatePlayerGrids(controller.FirstPlayer);
                        if (controller.GameFinished)
                            MessageBox.Show(controller.FirstPlayerTurns ? "You win!" : "You lost =(");
                    };
                }
            }

            return result;
        }

        private static void InitGrid<T>(Button[,] gridLabels, IRectangularReadonlyField<T> field, Func<T, Brush> getColor)
        {
            foreach (var position in field.EnumerateCellPositions())
                gridLabels[position.Row, position.Column].Background = getColor(field[position]);
        }

        private void CreateNewGameHandle(object sender, RoutedEventArgs e)
        {
            controller = kernel.Get<IGameController>();
            UpdatePlayerGrids(controller.FirstPlayer);
        }

        private void UpdatePlayerGrids(IPlayer player)
        {
            InitGrid(selfFieldGridLabels, player.SelfField, SelfFieldColorer);
            InitGrid(opponentFieldGridLabels, player.OpponentFieldKnowledge, OpponentFieldColorer);
        }

        private static readonly Func<IGameCell, Brush> SelfFieldColorer = cell =>
        {
            var shipCell = cell as ShipCell;
            if (shipCell != null)
            {
//                return shipCell.Ship.Killed
//                    ? Brushes.Red
//                    : shipCell.Damaged
//                        ? Brushes.Yellow
//                        : Brushes.Green;
                return shipCell.Damaged
                        ? Brushes.Yellow
                        : Brushes.Green;
            }
            return cell.Damaged ? Brushes.Gray : Brushes.LightGray;
        };
        private static readonly Func<bool?, Brush> OpponentFieldColorer = cell =>
        {
            return cell == null
                ? Brushes.Black
                : cell == true
                    ? Brushes.Green
                    : Brushes.Red;
        };

        private static IKernel InitKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IGameFieldBuilder>().To<GameFieldBuilder>();
            kernel.Bind<IGameField>().ToMethod(context => context.Kernel.Get<IGameFieldBuilder>().GenerateRandomField());
            kernel.Bind<IPlayerFactory>().To<PlayerFactory>();

            kernel.Bind<IGameController>().To<GameController>()
                .WithConstructorArgument("firstPlayer",
                    context => context.Kernel.Get<IPlayerFactory>().CreateConsolePlayer(kernel.Get<IGameField>()))
                .WithConstructorArgument("secondPlayer",
                    context => context.Kernel.Get<IPlayerFactory>().CreateRandomPlayer(kernel.Get<IGameField>()));

            return kernel;
        }
    }
}