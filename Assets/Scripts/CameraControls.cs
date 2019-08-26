using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetSmashers
{
    public class CameraControls : MonoBehaviour
    {
        public float Speed;
        public float ZoomSpeed;
        public Camera Camera;

        public void FixedUpdate()
        {
            var currentPos = transform.position;

            if (Input.GetKey(KeyCode.W))
            {
                currentPos.y += Speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                currentPos.y -= Speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                currentPos.x -= Speed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                currentPos.x += Speed * Time.fixedDeltaTime;
            }

            transform.position = currentPos;

            var scroll = -Input.GetAxis("Mouse ScrollWheel");
            var toZoom = Time.fixedDeltaTime * ZoomSpeed * scroll * 20;

            Camera.orthographicSize = Mathf.Clamp(Camera.orthographicSize + toZoom, 2f, 50f);
        }
    }
}