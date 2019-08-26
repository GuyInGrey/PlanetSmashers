using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineText : MonoBehaviour
{
    public string Text;
    public Text TextImage;

    private void Update()
    {
        TextImage.text = Text;
    }
}