using Raylib_cs;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Numerics;


namespace HnTRgine.Components
{
    using HnTRgine.Vectors;
    using HnTRgine.EntityManagement;

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
        public Vector3D position;
        public Vector3D scale;
        public double rotation;

        public Transform()
        {
            position = new Vector3D();
            scale = new Vector3D(1);
        }

        public Transform(Vector3D position)
        {
            this.position = position;
            scale = new Vector3D();
        }

        public Transform(Vector3D position, Vector3D scale)
        {
            this.position = position is null ? new Vector3D(0, 0) : position;
            this.scale = scale is null ? new Vector3D(1, 1) : scale;
        }

        public void Translate(Vector3D translation) => position += translation;

        public void Scale(Vector3D scale) => this.scale += scale;
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
            position = Vector3D.FromString(jsonElement.GetProperty("position").GetString());
            scale = Vector3D.FromString(jsonElement.GetProperty("scale").GetString());
            rotation = jsonElement.GetProperty("rotation").GetDouble();
        }
    }

    class Sprite : Component
    {
        public Texture2D Texture { get; private set; }
        public string image { get; private set; }

        public Sprite() => image = "";

        public Sprite(string image) => LoadImage(image);

        public void LoadImage(string? image)
        {
            if (string.IsNullOrEmpty(image))
                return;
            this.image = image;
            Image img = Raylib.LoadImage(Path.GetFullPath(this.image));
            Texture = Raylib.LoadTextureFromImage(img);
        }

        public override void SaveToJson(ref JsonObject obj) => obj["image"] = image;

        public override void LoadFromJson(JsonElement jsonElement) => LoadImage(jsonElement.GetProperty("image").GetString());
    }

    abstract class Script : Component
    {
        public virtual void Init() { }
        public virtual void Update() { }
    }

    class Camera : Component
    {
        public enum Projection { Perspective, Orthographic };

        private Raylib_cs.Camera3D camera;
        public Vector3D forward;
        public Vector3D target
        {
            get => camera.Target;
            set => camera.Target = value;
        }
        public Vector3D up
        {
            get => camera.Up;
            set => camera.Up = value;
        }
        public float fov_y
        {
            get => camera.FovY;
            set => camera.FovY = value;
        }
        public Projection projection
        {
            get => (Projection)camera.Projection;
            set => camera.Projection = (CameraProjection)value;
        }
        public Vector3D position
        {
            get => camera.Position;
            set => camera.Position = value;
        }

        public Camera(float? fov_y=null, Vector3D? up=null, Vector3D? forward=null, Projection? projection=null)
        {
            this.camera = new Raylib_cs.Camera3D();
            this.fov_y = fov_y ?? 90f;
            this.up = up ?? Vector3D.up;
            this.forward = forward ?? Vector3D.forward;
            this.projection = projection ?? Projection.Perspective;
        }

        public static implicit operator Camera3D(Camera cam) => cam.camera;
    }
}
