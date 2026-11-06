using Raylib_cs;


namespace HnTRgine.Systems
{
    using HnTRgine.Vectors;
    using HnTRgine.Components;
    using HnTRgine.EntityManagement;

    abstract class System
    {
        public virtual Type[] RequiredComponents { get; protected set; } = [];

        public virtual void Init(Entity entity) { }
        public virtual void Update(Entity entity) { }
    }

    class SpriteRenderingSystem : System
    {
        public override Type[] RequiredComponents { get; protected set; } = [
            typeof(Sprite)
        ];

        public override void Update(Entity entity)
        {
            Texture2D texture = entity.GetComponent<Sprite>().Texture;
            Raylib.DrawTexturePro(
                texture,
                new Rectangle(
                    0,
                    0,
                    texture.Dimensions.X * Math.Sign(entity.transform.scale.x),
                    texture.Dimensions.Y * Math.Sign(entity.transform.scale.y)),
                new Rectangle(entity.transform.position,
                    MathF.Abs((float)entity.transform.scale.x) * texture.Width,
                    MathF.Abs((float)entity.transform.scale.y) * texture.Height),
                new Vector2D(
                    Math.Abs(entity.transform.scale.x) * texture.Dimensions.X * 0.5f,
                    Math.Abs(entity.transform.scale.y) * texture.Dimensions.Y * 0.5f
                ),
                (float)entity.transform.rotation,
                Color.White
            );
        }
    }

    class ScriptManagementSystem : System
    {
        public override Type[] RequiredComponents { get; protected set; } = [
            typeof(Script)
        ];

        public override void Init(Entity entity)
        {
            Script[] scripts = entity.GetScripts().ToArray();
            foreach (Script script in scripts)
                script.Init();
        }

        public override void Update(Entity entity)
        {
            Script[] scripts = entity.GetScripts().ToArray();
            foreach (Script script in scripts)
                script.Update();
        }
    }

    class CameraRendererSystem : System
    {
        public override Type[] RequiredComponents { get; protected set; } = [
            typeof(Camera)
        ];

        private Vector3D a = new Vector3D();

        public override void Init(Entity entity)
        {
            Camera camera = entity.GetComponent<Camera>();
            camera.position = entity.transform.position;
            camera.target = camera.target + camera.position;
        }

        public override void Update(Entity entity)
        {
            Camera camera = entity.GetComponent<Camera>();
            camera.position = entity.transform.position;
            camera.target = camera.forward + camera.position;
        }
    }
}
