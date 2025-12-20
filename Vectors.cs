using System.Numerics;


namespace HnTRgine
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
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.x + b.x, a.y + b.y);
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
        public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.x + b.x, a.y + b.y);
        public static Vector2I operator *(Vector2I a, int n) => new Vector2I(a.x * n, a.y * n);
        public static Vector2I operator /(Vector2I a, int n) => new Vector2I(a.x / n, a.y / n);
        public static double operator *(Vector2I a, Vector2I b) => a.x * b.x + a.y * b.y;
        public static Vector2I FromString(string s) => new Vector2I(int.Parse(s.Split(",")[0]), int.Parse(s.Split(",")[1]));
        public static implicit operator Vector2D(Vector2I a) => new Vector2D(a.x, a.y);
    }
}
