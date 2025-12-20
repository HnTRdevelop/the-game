using Raylib_cs;


namespace HnTRgine
{
    abstract class System
    {
        public abstract Type[] RequiredComponents { get; protected set; }

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
            Sprite sprite = entity.GetComponent<Sprite>();
            Transform transform = entity.GetComponent<Transform>();
            Raylib.DrawTexturePro(
                sprite.Texture, 
                new Rectangle(
                    0, 
                    0, 
                    sprite.Texture.Dimensions.X * Math.Sign(transform.scale.x), 
                    sprite.Texture.Dimensions.Y * Math.Sign(transform.scale.y)),
                new Rectangle(transform.position,
                    MathF.Abs((float)transform.scale.x) * sprite.Texture.Width, 
                    MathF.Abs((float)transform.scale.y) * sprite.Texture.Height),
                new Vector2D(
                    Math.Abs(transform.scale.x) * sprite.Texture.Dimensions.X * 0.5f,
                    Math.Abs(transform.scale.y) * sprite.Texture.Dimensions.Y * 0.5f
                ), 
                (float)transform.rotation, 
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
                script.Update(Raylib.GetFrameTime());
        }
    }
}
