using System.Text.Json;
using System.Text.Json.Nodes;
using Raylib_cs;


namespace HnTRgine
{    
    abstract class Component
    {
        private Entity? _owner;
        public Entity owner 
        { 
            get => _owner ?? throw new Exception($"Component {this} does not have owner!");
            set { 
                if (_owner is null) 
                _owner = value; 
            }
        }

        public virtual void SaveToJson(ref JsonObject obj) { }
        public virtual void LoadFromJson(JsonElement jsonElement) { }
    }

    class Transform : Component
    {
        public Vector2D position;
        public Vector2D scale;
        public double rotation;

        public Transform()
        {
            position = new Vector2D();
            scale = new Vector2D();
        }
        
        public Transform(Vector2D position)
        {
            this.position = position;
            scale = new Vector2D();
        }

        public Transform(Vector2D position, Vector2D scale)
        {
            this.position = position is null ? new Vector2D(0, 0) : position;
            this.scale = scale is null ? new Vector2D(1, 1) : scale;
        }

        public void Translate(Vector2D translation) => this.position += translation;

        public void Scale(Vector2D scale) => this.scale += scale;
        public void Scale(double scale) => this.scale *= scale;
        
        public void Rotate(double rotation)
        {
            this.rotation += rotation;
            while (this.rotation >= 360) this.rotation -= 360;
            while (this.rotation < 0) this.rotation += 360;
        }

        public override void SaveToJson(ref JsonObject obj)
        {
            obj["position"] = position.ToString();
            obj["scale"] = scale.ToString();
            obj["rotation"] = rotation;
        }

        public override void LoadFromJson(JsonElement jsonElement)
        {
            position = Vector2D.FromString(jsonElement.GetProperty("position").GetString());
            scale = Vector2D.FromString(jsonElement.GetProperty("scale").GetString());
            rotation = jsonElement.GetProperty("rotation").GetDouble();
        }
    }

    class Sprite : Component
    {
        public Texture2D Texture { get; private set; }
        public string image { get; private set; }

        public Sprite()
        {
            image = "";
        }

        public Sprite(string image)
        {
            this.image = image;
            LoadImage(image);
        }

        public void LoadImage(string? image)
        {
            if (string.IsNullOrEmpty(image))
                return;
            this.image = image;
            Image img = Raylib.LoadImage(Path.GetFullPath(this.image));
            Texture = Raylib.LoadTextureFromImage(img);
        }

        public override void SaveToJson(ref JsonObject obj)
        {
            obj["image"] = image;
        }

        public override void LoadFromJson(JsonElement jsonElement)
        {
            LoadImage(jsonElement.GetProperty("image").GetString());
        }
    }

    abstract class Script : Component
    {
        public virtual void Init() { }
        public virtual void Update(double deltaTime) { }
    }
}
