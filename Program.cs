using System;
using HnTRgine;
using HnTRgine.EntityManagement;
using HnTRgine.Vectors;


namespace TheGame
{
    using HnTRgine.Components;
    using HnTRgine.Systems;
    using HnTRgine.InputManager;

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

            Entity cam = new Entity();
            Camera c = cam.AddComponent(new Camera());
            cam.AddComponent(new PlayerController());
            cam.transform.position = new Vector3D(0, 1, -3);

            Game.CurrentScene.AddEntity(cam);
            Game.CurrentScene.AddSystem(new CameraRendererSystem());
            Game.CurrentScene.AddSystem(new ScriptManagementSystem());

            game.GameLoop();
        }
    }

    class PlayerController : Script
    {
        public override void Update()
        {
            if (Input.IsKeyDown(Key.W))
                owner.transform.position += Vector3D.forward * Time.deltaTime * 5;
            if (Input.IsKeyDown(Key.S))
                owner.transform.position += Vector3D.back * Time.deltaTime * 5;
            if (Input.IsKeyDown(Key.A))
                owner.transform.position -= Vector3D.left * Time.deltaTime * 5;
            if (Input.IsKeyDown(Key.D))
                owner.transform.position -= Vector3D.right * Time.deltaTime * 5;
            if (Input.IsKeyDown(Key.E))
                owner.transform.position += Vector3D.up * Time.deltaTime * 5;
            if (Input.IsKeyDown(Key.Q))
                owner.transform.position += Vector3D.down * Time.deltaTime * 5;
        }
    }
}
