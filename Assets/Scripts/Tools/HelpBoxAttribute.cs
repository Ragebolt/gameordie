using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBoxAttribute : PropertyAttribute
{
    public string text;
    public float height;

    public HelpBoxAttribute(string text, float height = 40f)
    {
        this.text = text;
        this.height = height;
    }
}