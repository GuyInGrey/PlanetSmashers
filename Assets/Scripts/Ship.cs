using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetSmashers
{
    [ExecuteAlways]
    public class Ship : MonoBehaviour
    {
        public SpriteRenderer ShipColorRenderer;
        public Color Color;
        public int Speed;
        public string Type;
        public int PlayerID;
        public float Damage;
        public int Cost;

        public Hex Location => gameObject.GetHexPosition();
        public float Health;
        public bool IsDestroyed;

        public float Rotation;

        public Image HealthImage;
        float maxHealth;

        private void Start()
        {
            maxHealth = Health;
        }

        public void Update()
        {
            ShipColorRenderer.color = Color;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                Rotation
            );

            var deadColor = Color.red;
            var aliveColor = Color.green;

            HealthImage.color = Color.Lerp(deadColor, aliveColor, Health / maxHealth);
            HealthImage.fillAmount = Health / maxHealth;

            if (IsDestroyed)
            {
                Destroy(gameObject);
            }
        }

        public void MoveTo(Hex to)
        {
            var p = to.ToPixel();
            var v3 = (Vector3)p - transform.position;
            Rotation = (Mathf.Atan2(v3.y, v3.x) * 57.3f) - 90f;
            transform.position = p;
        }

        public Hex[] GoTo(Hex to)
        {
            if (Location == new Hex(-11, 0))
            {

            }

            if (Location == to) { return new Hex[0]; }
            var possible = HexManager.WithinRange(Location, Speed);
            possible = possible.OrderBy(p => p.Distance(to)).ToArray();
            return possible;
        }

        public void Attack(Ship s)
        {
            s.Health -= Damage - 0.5f;
            Health -= s.Damage;
            IsDestroyed = Health < 0;
            s.IsDestroyed = s.Health < 0;
        }
    }
}