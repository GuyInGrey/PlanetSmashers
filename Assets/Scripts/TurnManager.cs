using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetSmashers
{
    public class TurnManager : MonoBehaviour
    {
        public BoardManager Board;
        public GameObject Selection;
        public Camera Camera;

        List<Ship> HasChanged;

        bool selectionMode;
        Ship LastSelected;

        public Sprite HighlightSprite;
        List<Transform> Highlighted;
        public Text ResourcesText;

        public void Update()
        {
            foreach (var p in Board.Players)
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

            if (HasChanged == null)
            {
                HasChanged = new List<Ship>();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
                var mousePosHex = Hex.Round(HexManager.PixelToHex(mousePos));
                
                if (!selectionMode)
                {
                    Board.LoopShips(new Action<Ship>(s =>
                    {
                        if (s.PlayerID == Board.PlayerID && 
                        !HasChanged.Contains(s) && 
                        s.Location == mousePosHex && 
                        s.Type != "Station") // If you're selecing one of your ships that hasn't moved yet.
                        {
                            LastSelected = s;
                            Selection.SetActive(true);
                            Selection.ToHexPosition(mousePosHex);
                            selectionMode = true;

                            Highlighted = new List<Transform>();
                            var range = HexManager.WithinRange(s.Location, s.Speed);
                            foreach (var r in range)
                            {
                                var highlight = Instantiate(Selection);
                                highlight.ToHexPosition(r);
                                var sr = highlight.GetComponent<SpriteRenderer>();
                                sr.sprite = HighlightSprite;
                                var c = Board.Players.Where(p => p.ID == s.PlayerID).First().Color;
                                sr.color = new Color(c.r, c.g, c.b, 0.5f);
                                Highlighted.Add(highlight.transform);
                            }
                        }
                    }), Board.PlayerID);
                }
                else
                {
                    var didChange = false;
                    if (LastSelected.Location.Distance(mousePosHex) > LastSelected.Speed)
                    {
                        return;
                    }

                    var clickedOnShip = false;
                    Board.LoopShips(new Action<Ship>(s =>
                    {
                        if (s.Location == mousePosHex)
                        {
                            clickedOnShip = true;
                        }

                        if (s.Location == mousePosHex)
                        {
                            if (s.PlayerID == LastSelected.PlayerID && s.Type == "Carrier" && LastSelected.Type == "Fighter")
                            {
                                // Carrier Dock Code Here
                                didChange = true;
                            }
                            else if (s.PlayerID != LastSelected.PlayerID)
                            {
                                s.Health -= LastSelected.Damage - 0.5f;
                                LastSelected.Health -= s.Damage;

                                if (s.Health <= 0)
                                {
                                    s.IsDestroyed = true;
                                }
                                if (LastSelected.Health <= 0)
                                {
                                    LastSelected.IsDestroyed = true;
                                }
                                didChange = true;
                            }
                        }
                    }));

                    if (!clickedOnShip)
                    {
                        if (mousePosHex.Distance(LastSelected.Location) <= LastSelected.Speed)
                        {
                            LastSelected.MoveTo(mousePosHex);
                            didChange = true;
                        }
                    }

                    if (didChange)
                    {
                        HasChanged.Add(LastSelected);
                        LastSelected = null;
                        selectionMode = false;
                        Selection.SetActive(false);
                        foreach (var h in Highlighted)
                        {
                            Destroy(h.gameObject);
                        }
                        Highlighted = new List<Transform>();
                    }
                }
            }
        }

        public void EndTurn()
        {
            HasChanged = new List<Ship>();

            foreach (var p in Board.Players)
            {
                foreach (var s in p.Ships)
                {
                    if (s.Type == "Miner")
                    {
                        foreach (var b in Board.BodyLocations)
                        {
                            if (b == null)
                            {
                                continue;
                            }

                            if (b.Location == s.Location)
                            {
                                p.Resources++;
                                b.ResourcesRemaining--;
                            }
                        }
                    }
                }
            }

            ResourcesText.text = Board.Players.Where(p => p.ID == Board.PlayerID).First().Resources.ToString();

            for (var i = 0; i < Board.BodyLocations.Count; i++)
            {
                if (Board.BodyLocations[i] == null)
                {
                    Board.BodyLocations.RemoveAt(i);
                    i--;
                }
            }

            foreach (var p in Board.Players)
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

            // A.I. Turns

            for (var i = 0; i < 6; i++)
            {
                var player = Board.Players.Where(a => a.ID == i).FirstOrDefault();

                if (player == null || player.ID == Board.PlayerID)
                {
                    continue;
                }

                foreach (var s in player.Ships)
                {
                    if (s.Type == "Miner")
                    {
                        var sortedBodies = Board.BodyLocations.OrderBy(a => a.Location.Distance(s.Location)).ToList();
                        Board.LoopShips(new Action<Ship>(a => {
                            if (a != s)
                            {
                                sortedBodies.RemoveAll(b => b.Location == a.Location);
                            }
                        }));

                        if (sortedBodies.Count <= 0)
                        {
                            continue;
                        }

                        var aim = sortedBodies[0].Location;
                        var pos = new List<Hex>(s.GoTo(aim));

                        Board.LoopShips(new Action<Ship>(a => {
                            pos.RemoveAll(b => b == a.Location);
                        }));

                        if (pos.Count > 0)
                        {
                            s.MoveTo(pos[0]);
                        }
                    }
                    else if (s.Type == "Fighter")
                    {
                        Ship aim = null;
                        Board.LoopShips(new Action<Ship>(a => {
                            if (aim == null || a.Location.Distance(s.Location) < aim.Location.Distance(s.Location))
                            {
                                if (a.PlayerID != s.PlayerID)
                                {
                                    aim = a;
                                }
                            }
                        }));
                        Debug.Log("3 " + aim);
                        var pos = new List<Hex>(s.GoTo(aim.Location));
                        Debug.Log("1 " + pos.Count);
                        Board.LoopShips(new Action<Ship>(a => {
                            if (a.PlayerID == s.PlayerID)
                            {
                                pos.RemoveAll(b => b == a.Location);
                            }
                        }));

                        Debug.Log("2 " + pos.Count);
                        if (pos.Count > 0)
                        {
                            if (pos[0] == aim.Location)
                            {
                                s.Attack(aim);
                            }
                            else
                            {
                                s.MoveTo(pos[0]);
                            }
                        }
                    }
                }
            }
        }
    }
}