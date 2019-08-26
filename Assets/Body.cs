using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetSmashers
{
    public class Body : MonoBehaviour
    {
        public int ResourcesRemaining;
        int maxResources;
        public Image ResourceImage;

        public Hex Location => gameObject.GetHexPosition();

        private void Start()
        {
            maxResources = ResourcesRemaining;
        }

        private void Update()
        {
            var deadColor = Color.black;
            var aliveColor = Color.blue;

            ResourceImage.color = Color.Lerp(deadColor, aliveColor, ResourcesRemaining / (float)maxResources);
            ResourceImage.fillAmount = ResourcesRemaining / (float)maxResources;

            if (ResourcesRemaining <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}