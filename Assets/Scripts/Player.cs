using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetSmashers
{
    [Serializable]
    public class Player
    {
        public List<Ship> Ships;
        public Color Color;
        public string Name;
        public int ID;
        public int Resources;
    }
}