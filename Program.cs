using System.Text.Json;
using System.Text.Json.Nodes;
using HnTRgine;


namespace TheGame
{    
    internal class Program
    {
        public static void Main(string[] args)
        {
            Game game;
            if (args.Length == 1)
                game = new Game(args[0]);
            else
                game = new Game();

            game.GameLoop();
        }
    }

    class TestScript : Script
    {
        private int rotDir = 0;
        private double speed = .1;

        public override void Init()
        {
            owner.transform.position = Game.screenSize / 2;
            owner.transform.scale = new Vector2D(1);
            Flipper();
        }
        public override void Update(double deltaTime)
        {
            owner.transform.Rotate(speed * 3 * deltaTime);
            owner.transform.Scale(1 - speed * 0.015 * deltaTime);
            speed += 9.81 * deltaTime;
        }

        private async void Flipper()
        {
            while (true)
            {
                rotDir = rotDir == 0 ? 1 : 0;
                switch (rotDir)
                {
                    case 1:
                        owner.transform.scale.x *= -1;
                        break;

                    case 0:
                        owner.transform.scale.y *= -1;
                        break;
                }
                await Task.Delay(500);
            }
        }
    }

    class NewComponent : Component
    {
        public int a;
        public int b;

        public override void SaveToJson(ref JsonObject obj)
        {
            obj["a"] = a;
            obj["b"] = b;
        }

        public override void LoadFromJson(JsonElement jsonElement)
        {
            a = jsonElement.GetProperty("a").GetInt32();
            b = jsonElement.GetProperty("b").GetInt32();
        }
    }

    class NewSystem : HnTRgine.System
    {
        public override Type[] RequiredComponents { get; protected set; } = [
            typeof(NewComponent)
        ];

        public override void Update(Entity entity)
        {
            NewComponent newComponent = entity.GetComponent<NewComponent>();
            Console.WriteLine(newComponent.a + newComponent.b);
        }
    }
}
