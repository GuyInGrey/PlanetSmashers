using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetSmashers
{
    public static class Extentions
    {
        public static void ToHexPosition(this GameObject o, Hex h)
        {
            o.transform.position = h.ToPixel(); 
        }

        public static Hex GetHexPosition(this GameObject o)
        {
            return Hex.Round(HexManager.PixelToHex(o.transform.position));
        }

        public static Ship Ship(this GameObject o)
        {
            return o.GetComponent<Ship>();
        }

        public static Hex[] Surrounding(this Hex h)
        {
            return new Hex[]
            {
                new Hex(h.Q, h.R - 1),
                new Hex(h.Q, h.R + 1),
                new Hex(h.Q - 1, h.R + 1),
                new Hex(h.Q + 1, h.R - 1),
                new Hex(h.Q - 1, h.R),
                new Hex(h.Q + 1, h.R),
            };
        }
    }
}