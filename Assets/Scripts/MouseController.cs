using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetSmashers
{
    public class MouseController : MonoBehaviour
    {
        public Text CoordinateText;
        public Transform OutlineSprite;

        public BoardManager Board;

        public void Update()
        {
            var mousePos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            var mouseGamePos = Cube.Round(HexManager.PixelToHex(mousePos).ToCube()).ToHex();
            CoordinateText.text = string.Format("X: {0}, Y: {1}", mouseGamePos.Q, mouseGamePos.R);
            OutlineSprite.position = mouseGamePos.ToPixel();

            Ship hoveringShip = null;
            Board.LoopShips(new System.Action<Ship>(a =>
            {
                if (a.Location == mouseGamePos)
                {
                    hoveringShip = a;
                }
            }));

            if (hoveringShip == null)
            {
                OutlineSprite.gameObject.GetComponent<OutlineText>().Text = "";
            }
            else
            {
                var playerName = Board.Players.Where(p => p.ID == hoveringShip.PlayerID).First().Name;
                var playerColor = Board.Players.Where(p => p.ID == hoveringShip.PlayerID).First().Color;
                var shipName = hoveringShip.Type;
                OutlineSprite.gameObject.GetComponent<OutlineText>().Text = playerName + " - " + shipName;
                OutlineSprite.gameObject.GetComponent<OutlineText>().TextImage.color = playerColor;
            }
        }
    }
}