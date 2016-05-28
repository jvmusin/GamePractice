using System;
using Battleship.Implementations;
using Battleship.Interfaces;
using Ninject;

namespace GraphicInterface
{
    public class Program
    {
        private readonly IGameController controller;
        private readonly IGraphicUserInterface gui;

        public Program(IGameController controller, IGraphicUserInterface gui)
        {
            this.controller = controller;
            this.gui = gui;
        }

        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IGameFieldBuilder>().To<GameFieldBuilder>();
            kernel.Bind<IGameField>().ToMethod(context => context.Kernel.Get<IGameFieldBuilder>().GenerateRandomField());
            kernel.Bind<IPlayerFactory>().To<PlayerFactory>();
            
            var me = kernel.Get<IPlayerFactory>().CreateConsolePlayer(kernel.Get<IGameField>());
            var opponent = kernel.Get<IPlayerFactory>().CreateRandomPlayer(kernel.Get<IGameField>());
            kernel.Bind<IGameController>().ToConstant(new GameController(me, opponent));

            kernel.Bind<IGraphicUserInterface>().To<TextUI>()
                .WithConstructorArgument("writer", Console.Out);

            kernel.Get<Program>().Run();
        }

        public void Run()
        {
            gui.Update();
            while (!controller.GameFinished)
            {
                var target = controller.CurrentPlayer.NextTarget;
                if (controller.Shoot(target) != null)
                    gui.Update();
            }
        }
    }
}
