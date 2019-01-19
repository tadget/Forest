using UnityEngine;

namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public struct Coord
    {
        public readonly int x, y, z;

        public Coord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public List<Coord> GetSurroundingCoords(int radius)
        {
            List<Coord> coords = new List<Coord>();

            for (int _z = z-radius; _z <= z+radius; _z++)
                for (int _x = x - radius; _x <= x + radius; _x++)
                {
                    var coord = new Coord(_x, 0, _z);
                    if(coord != this)
                        coords.Add(coord);
                }

            return coords;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", x, y, z);
        }

        public override int GetHashCode() {
            uint h = 0x811c9dc5;
            h = (h ^ (uint)x) * 0x01000193;
            h = (h ^ (uint)y) * 0x01000193;
            h = (h ^ (uint)z) * 0x01000193;
            return (int)h;
        }

        public override bool Equals(Object obj)
        {
            return obj is Coord && Equals((Coord)obj);
        }

        public bool Equals(Coord other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public static bool operator ==(Coord lhs, Coord rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Coord lhs, Coord rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
