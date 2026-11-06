using System.Numerics;


namespace HnTRgine.Vectors
{
    class Vector2D
    {
        public double x, y;
        public double Lenght { get => Math.Sqrt(x * x + y * y); }
        public Vector2D(double? x=null, double? y=null)
        {
            this.x = x ?? 0;
            this.y = y ?? x ?? 0;
        }

        public Vector2D(Vector2 vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        public override string ToString() => $"{x},{y}";
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.x + b.x, a.y + b.y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.x - b.x, a.y - b.y);
        public static Vector2D operator *(Vector2D a, double n) => new Vector2D(a.x * n, a.y * n);
        public static Vector2D operator /(Vector2D a, double n) => new Vector2D(a.x / n, a.y / n);
        public static double operator *(Vector2D a, Vector2D b) => a.x * b.x + a.y * b.y;
        public static Vector2D FromString(string? s)
        {
            if (!string.IsNullOrEmpty(s))
                return new Vector2D(double.Parse(s.Split(",")[0]), double.Parse(s.Split(",")[1]));
            throw new Exception($"Couldn't create Vector2D from string {s}!");
        }
        public static explicit operator Vector2I(Vector2D a) => new Vector2I(Convert.ToInt32(a.x), Convert.ToInt32(a.y));
        public static implicit operator Vector2(Vector2D a) => new Vector2((float)a.x, (float)a.y);

        public static Vector2D up = new Vector2D(0, 1);
        public static Vector2D down = new Vector2D(0, -1);
        public static Vector2D right = new Vector2D(1, 0);
        public static Vector2D left = new Vector2D(-1, 0);
    }

    class Vector2I
    {
        public int x, y;
        public double Lenght { get => MathF.Sqrt(x * x + y * y); }
        public Vector2I(int? x=null, int? y=null)
        {
            this.x = x ?? 0;
            this.y = y ?? x ?? 0;
        }

        public override string ToString() => $"{x},{y}";
        public static Vector2I operator +(Vector2I a, Vector2I b) => new Vector2I(a.x + b.x, a.y + b.y);
        public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.x - b.x, a.y - b.y);
        public static Vector2I operator *(Vector2I a, int n) => new Vector2I(a.x * n, a.y * n);
        public static Vector2I operator /(Vector2I a, int n) => new Vector2I(a.x / n, a.y / n);
        public static double operator *(Vector2I a, Vector2I b) => a.x * b.x + a.y * b.y;
        public static Vector2I FromString(string s) => new Vector2I(int.Parse(s.Split(",")[0]), int.Parse(s.Split(",")[1]));
        public static implicit operator Vector2D(Vector2I a) => new Vector2D(a.x, a.y);
        public static implicit operator Vector3D(Vector2I a) => new Vector3D(a.x, a.y, 0);

        public static Vector2I up = new Vector2I(0, 1);
        public static Vector2I down = new Vector2I(0, -1);
        public static Vector2I right = new Vector2I(1, 0);
        public static Vector2I left = new Vector2I(-1, 0);
    }

    class Vector3D
    {
        public double x, y, z;
        public double Lenght { get => Math.Sqrt(x * x + y * y + z * z); }
        public Vector3D(double? x=null, double? y=null, double? z=null)
        {
            this.x = x ?? 0;
            this.y = y ?? x ?? 0;
            this.z = z ?? x ?? 0;
        }

        public Vector3D(Vector3 vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        public override string ToString() => $"{x},{y},{z}";
        public static Vector3D operator +(Vector3D a, Vector3D b) => new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3D operator -(Vector3D a, Vector3D b) => new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3D operator *(Vector3D a, double n) => new Vector3D(a.x * n, a.y * n, a.z * n);
        public static Vector3D operator /(Vector3D a, double n) => new Vector3D(a.x / n, a.y / n, a.z / n);
        public static double operator *(Vector3D a, Vector3D b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static Vector3D FromString(string? s)
        {
            if (!string.IsNullOrEmpty(s))
                return new Vector3D(double.Parse(s.Split(",")[0]), double.Parse(s.Split(",")[1]), double.Parse(s.Split(",")[2]));
            throw new Exception($"Couldn't create Vector3D from string {s}!");
        }
        public static explicit operator Vector3I(Vector3D a) => new Vector3I(Convert.ToInt32(a.x), Convert.ToInt32(a.y), Convert.ToInt32(a.z));
        public static implicit operator Vector3(Vector3D a) => new Vector3((float)a.x, (float)a.y, (float)a.z);
        public static implicit operator Vector2(Vector3D a) => new Vector2((float)a.x, (float)a.y);
        public static implicit operator Vector3D(Vector3 a) => new Vector3D(a.X, a.Y, a.Z);

        public static Vector3D up = new Vector3D(0, 1, 0);
        public static Vector3D down = new Vector3D(0, -1, 0);
        public static Vector3D forward = new Vector3D(0, 0, 1);
        public static Vector3D back = new Vector3D(0, 0, -1);
        public static Vector3D right = new Vector3D(1, 0, 0);
        public static Vector3D left = new Vector3D(-1, 0, 0);
    }

    class Vector3I
    {
        public int x, y, z;
        public double Lenght { get => MathF.Sqrt(x * x + y * y + z * z); }
        public Vector3I(int? x=null, int? y=null, int? z=null)
        {
            this.x = x ?? 0;
            this.y = y ?? x ?? 0;
            this.z = z ?? x ?? 0;
        }

        public override string ToString() => $"{x},{y},{z}";
        public static Vector3I operator +(Vector3I a, Vector3I b) => new Vector3I(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3I operator -(Vector3I a, Vector3I b) => new Vector3I(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3I operator *(Vector3I a, int n) => new Vector3I(a.x * n, a.y * n, a.z * n);
        public static Vector3I operator /(Vector3I a, int n) => new Vector3I(a.x / n, a.y / n, a.z / n);
        public static double operator *(Vector3I a, Vector3I b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static Vector3I FromString(string s) => new Vector3I(int.Parse(s.Split(",")[0]), int.Parse(s.Split(",")[1]), int.Parse(s.Split(",")[2]));
        public static implicit operator Vector3D(Vector3I a) => new Vector3D(a.x, a.y, a.z);

        public static Vector3I up = new Vector3I(0, 1, 0);
        public static Vector3I down = new Vector3I(0, -1, 0);
        public static Vector3I forward = new Vector3I(0, 0, 1);
        public static Vector3I back = new Vector3I(0, 0, -1);
        public static Vector3I right = new Vector3I(1, 0, 0);
        public static Vector3I left = new Vector3I(-1, 0, 0);
    }
}
