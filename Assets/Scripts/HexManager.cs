using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetSmashers
{
    public class HexManager : MonoBehaviour
    {
        public static Hex PixelToHex(Vector2 point)
        {
            var q = (Mathf.Sqrt(3) / 3 * point.x - 1f / 3f * point.y);
            var r = (2f / 3f * point.y);
            return new Hex(q, r);
        }

        public static Hex[] WithinRange(Hex center, float N)
        {
            var toReturn = new List<Hex>();
            for (var x = -N; x <= N; x++)
            {
                for (var y = -N; y <= N; y++)
                {
                    for (var z = -N; z <= N; z++)
                    {
                        if (x + y + z == 0)
                        {
                            var c = new Cube(x, y, z);
                            toReturn.Add(Cube.Add(c, center.ToCube()).ToHex());
                        }
                    }
                }
            }
            return toReturn.ToArray();
        }
    }

    public class Hex
    {
        public float Q;
        public float R;

        public static Hex Zero => new Hex(0, 0);

        public Hex(float q, float r)
        {
            Q = q;
            R = r;
        }

        public Cube ToCube()
        {
            return new Cube(Q, -Q - R, R);
        }

        public Vector2 ToPixel()
        {
            var x = (Mathf.Sqrt(3) * Q + Mathf.Sqrt(3) / 2 * R);
            var y = (3f / 2f * R);
            return new Vector2(x, y);
        }

        public float Distance(Hex b)
        {
            var ac = ToCube();
            var bc = b.ToCube();
            return ac.Distance(bc);
        }

        public static Hex Round(Hex x)
        {
            return Cube.Round(x.ToCube()).ToHex();
        }

        public override bool Equals(object b)
        {
            if (b is Hex h)
            {
                return Q == h.Q && R == h.R;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1997189103;
            hashCode = hashCode * -1521134295 + Q.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Hex a, Hex b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Hex a, Hex b)
        {
            return !a.Equals(b);
        }
    }

    public class Cube
    {
        public float X;
        public float Y;
        public float Z;

        public Cube(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Hex ToHex()
        {
            return new Hex(X, Z);
        }

        public float Distance(Cube b)
        {
            return (Mathf.Abs(X - b.X) + Mathf.Abs(Y - b.Y) + Mathf.Abs(Z - b.Z)) / 2f;
        }

        public static Cube Add(Cube a, Cube b)
        {
            return new Cube(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Cube Round(Cube c)
        {
            var rx = Mathf.Round(c.X);
            var ry = Mathf.Round(c.Y);
            var rz = Mathf.Round(c.Z);

            var xDiff = Mathf.Abs(rx - c.X);
            var yDiff = Mathf.Abs(ry - c.Y);
            var zDiff = Mathf.Abs(rz - c.Z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new Cube(rx, ry, rz);
        }
    }
}