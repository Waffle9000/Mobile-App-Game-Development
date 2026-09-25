using UnityEngine;

public class TextSizeButtons : MonoBehaviour
{
    public void SetSize(float factor) => TextScale.Factor = factor;
}

//This script is to allow the user to change the text size in the game. It uses the TextScale class to set the text size factor based on the button pressed.