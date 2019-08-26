using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetSmashers
{
    public class BoardManager : MonoBehaviour
    {
        public HexManager HexManager;
        public Text ResourcesText;

        public Transform TilesParent;
        public Transform EntityParent;
        public Transform BodyParent;

        public List<Sprite> Bodies;
        public GameObject BodyPrefab;
        public List<Body> BodyLocations;

        public GameObject HexPrefab;
        public GameObject StationPrefab;
        public GameObject FighterPrefab;
        public GameObject BattlecruiserPrefab;
        public GameObject CarrierPrefab;
        public GameObject MinerPrefab;

        public List<Player> PlayersTemplates;
        public List<Player> Players;

        public Camera Camera;

        public void LoopShips(Action<Ship> a)
        {
            foreach (var p in Players)
            {
                foreach (var s in p.Ships)
                {
                    a?.Invoke(s);
                }
            }
        }
        public void LoopShips(Action<Ship> a, int playerID)
        {
            foreach (var p in Players)
            {
                if (p.ID == playerID)
                {
                    foreach (var s in p.Ships)
                    {
                        a?.Invoke(s);
                    }
                }
            }
        }
        
        public int PlayerID = 0;
        public int NPCCount = 1;
        public int MapSize = 25;

        public List<Hex> StartingLocations;

        public void Start()
        {
            BodyLocations = new List<Body>();

            var halfRadius = (int)Mathf.Round(MapSize / 2f);
            StartingLocations = new List<Hex> {
                new Hex(0, -halfRadius),
                new Hex(halfRadius, -halfRadius),
                new Hex(-halfRadius, 0),
                new Hex(-halfRadius, halfRadius),
                new Hex(0, halfRadius),
                new Hex(halfRadius, 0),
            };

            var playerLoc = Spawn(PlayersTemplates[PlayerID]).ToPixel();
            var pos = new Vector3(playerLoc.x, playerLoc.y, -1);
            Camera.transform.position = pos;

            var boardTiles = new List<Hex>(HexManager.WithinRange(new Hex(0, 0), MapSize));
            for (var i = 0; i < boardTiles.Count; i++)
            {
                var hex = boardTiles[i];
                var pixel = hex.ToPixel();
                var h = Instantiate(HexPrefab, TilesParent);
                h.transform.position = pixel;
            }

            for (var i = 0; i < NPCCount; i++)
            {
                Spawn(PlayersTemplates[UnityEngine.Random.Range(0, PlayersTemplates.Count)]);
            }

            foreach (var p in Players)
            {
                foreach (var s in p.Ships)
                {
                    s.Color = p.Color;
                }
            }

            LoopShips(new Action<Ship>(s => {
                boardTiles.Remove(s.Location);
            }));
            boardTiles.Remove(Hex.Zero);

            for (var i = 0; i < 100; i++)
            {
                var bodyLocation = boardTiles[UnityEngine.Random.Range(0, boardTiles.Count)];
                var b = Instantiate(BodyPrefab, BodyParent);
                b.ToHexPosition(bodyLocation);
                b.GetComponent<SpriteRenderer>().sprite = Bodies[UnityEngine.Random.Range(0, Bodies.Count)];
                BodyLocations.Add(b.GetComponent<Body>());
            }

            Hex Spawn(Player template)
            {
                var startHex = StartingLocations[UnityEngine.Random.Range(0, StartingLocations.Count)];
                StartingLocations.Remove(startHex);
                PlayersTemplates.Remove(template);

                var stationObject = Instantiate(StationPrefab, EntityParent);
                stationObject.ToHexPosition(startHex);
                template.Ships.Add(stationObject.Ship());
                
                var minerPositions = new List<Hex>()
                {
                    new Hex(startHex.Q, startHex.R - 1),
                    new Hex(startHex.Q, startHex.R + 1),
                    new Hex(startHex.Q - 1, startHex.R + 1),
                    new Hex(startHex.Q + 1, startHex.R - 1),
                };

                var fighterPositions = new List<Hex>()
                {
                    new Hex(startHex.Q - 1, startHex.R),
                    new Hex(startHex.Q + 1, startHex.R),
                };

                for (var i = 0; i < minerPositions.Count; i++)
                {
                    var minerObj = Instantiate(MinerPrefab, EntityParent);
                    minerObj.ToHexPosition(minerPositions[i]);
                    template.Ships.Add(minerObj.Ship());
                }

                for (var i = 0; i < fighterPositions.Count; i++)
                {
                    var fighterObj = Instantiate(FighterPrefab, EntityParent);
                    fighterObj.ToHexPosition(fighterPositions[i]);
                    template.Ships.Add(fighterObj.Ship());
                }

                foreach (var s in template.Ships)
                {
                    s.PlayerID = template.ID;
                }
                Players.Add(template);

                return startHex;
            }
        }

        public void BuildMiner()
        {
            Build(MinerPrefab);
        }

        public void BuildFighter()
        {
            Build(FighterPrefab);
        }

        public void BuildCarrier()
        {
            Build(CarrierPrefab);
        }

        public void BuildBattlecruiser()
        {
            Build(BattlecruiserPrefab);
        }

        public void Build(GameObject prefab)
        {
            if (Players.Where(p => p.ID == PlayerID).First().Resources < prefab.GetComponent<Ship>().Cost)
            {
                return;
            }
            
            var playerStationHex = Players.Where(p => p.ID == PlayerID).First().Ships.Where(s => s.Type == "Station").First().Location;
            var spawnPositions = new List<Hex>()
            {
                new Hex(playerStationHex.Q, playerStationHex.R - 1),
                new Hex(playerStationHex.Q, playerStationHex.R + 1),
                new Hex(playerStationHex.Q - 1, playerStationHex.R + 1),
                new Hex(playerStationHex.Q + 1, playerStationHex.R - 1),
                new Hex(playerStationHex.Q - 1, playerStationHex.R),
                new Hex(playerStationHex.Q + 1, playerStationHex.R),
            };

            var willUse = -1;

            for (var i = 0; i < spawnPositions.Count; i++)
            {
                var isOccupied = false;
                LoopShips(new Action<Ship>(s =>
                {
                    if (s.Location == spawnPositions[i])
                    {
                        isOccupied = true;
                    }
                }));
                if (BodyLocations.ConvertAll(a => a.Location).Contains(spawnPositions[i]))
                {
                    isOccupied = true;
                }

                if (isOccupied == false)
                {
                    willUse = i;
                    break;
                }
            }

            if (willUse > -1)
            {
                var s = Instantiate(prefab, EntityParent);
                s.ToHexPosition(spawnPositions[willUse]);
                s.GetComponent<Ship>().PlayerID = PlayerID;
                s.GetComponent<Ship>().Color = Players.Where(p => p.ID == PlayerID).First().Color;
                Players.Where(p => p.ID == PlayerID).First().Ships.Add(s.GetComponent<Ship>());
                
                Players.Where(p => p.ID == PlayerID).First().Resources -= prefab.GetComponent<Ship>().Cost;
                ResourcesText.text = Players.Where(p => p.ID == PlayerID).First().Resources.ToString();
            }
        }

        private void Update()
        {
            foreach (var p in Players)
            {
                for (var i = 0; i < p.Ships.Count; i++)
                {
                    if (p.Ships[i] == null)
                    {
                        p.Ships.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}