using System;
using System.IO;
using Battleship.Implementations;
using Battleship.Interfaces;
using Ninject;

namespace GraphicInterface
{
    public class Program
    {
        private readonly IGameController controller;
        private readonly TextUI gui;

        public Program(IGameController controller, TextUI gui)
        {
            this.controller = controller;
            this.gui = gui;
        }

        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IGameFieldBuilder>().To<GameFieldBuilder>();
            kernel.Bind<IGameField>().ToMethod(context => context.Kernel.Get<IGameFieldBuilder>().GenerateRandomField());

            kernel.Bind<IGameController>().To<GameController>()
                .WithConstructorArgument("firstPlayer", context => context.Kernel.Get<ConsolePlayer>())
                .WithConstructorArgument("secondPlayer", context => context.Kernel.Get<RandomPlayer>());

            kernel.Bind<TextWriter>().ToConstant(Console.Out);

            kernel.Get<Program>().Run();
        }

        public void Run()
        {
            gui.DrawCurrentState();
            while (!controller.GameFinished)
            {
                var target = controller.CurrentPlayer.NextTarget;
                if (controller.Shoot(target) != null)
                    gui.DrawCurrentState();
            }
        }
    }
}
