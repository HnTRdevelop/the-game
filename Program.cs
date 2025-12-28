using HnTRgine;
using HnTRgine.Components;
using HnTRgine.EntityManagement;
using HnTRgine.Systems;
using HnTRgine.Vectors;


namespace TheGame
{    
    internal class Program
    {
        public static void Main(string[] args)
        {
            Game game;
            Vector2I screenSize = new Vector2I(1280, 720);
            if (args.Length == 1)
                game = new Game(screenSize, args[0]);
            else if (args.Length == 3)
                game = new Game(new Vector2I(int.Parse(args[1]), int.Parse(args[2])), args[0]);
            else if (args.Length == 2)
                game = new Game(new Vector2I(int.Parse(args[0]), int.Parse(args[1])));
            else
                game = new Game(screenSize);

            game.GameLoop();
        }
    }
}
