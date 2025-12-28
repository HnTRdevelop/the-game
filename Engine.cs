namespace HnTRgine
{
    using Raylib_cs;
    using HnTRgine.Vectors;
    using HnTRgine.SceneManagement;

    class Game
    {
        public static Scene CurrentScene { get; private set; } = new Scene();
        public static Vector2I screenSize { get => new Vector2I(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); }
        public static bool IsRunning { get; private set; }

        public Game(Vector2I resolution, string? initalScene=null)
        {
            Raylib.InitWindow(resolution.x, resolution.y, "TheGame");
            if (!string.IsNullOrEmpty(initalScene))
                CurrentScene = Scene.LoadScene(initalScene);
        }

        public void GameLoop()
        {
            Init();
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Update();
                Raylib.DrawFPS(0, 0);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }

        public void Init()
        {
            CurrentScene.InitSystems();
            IsRunning = true;
        }

        public void Update()
        {
            CurrentScene.UpdateSystems();
            Time.UpdateTime();
        }
    }

    class Time
    {
        public static double deltaTime { get; private set; }
        public static double time { get; private set; }
        public static void UpdateTime()
        {
            time += Raylib.GetFrameTime();
            deltaTime = Raylib.GetFrameTime();
        }
    }
}
