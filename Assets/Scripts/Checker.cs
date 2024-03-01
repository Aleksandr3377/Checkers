using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public GameColor GameColor { get; private set; }
    private readonly Dictionary<GameColor, Color> _colors = new()
    {
        { GameColor.White, Color.white },
        { GameColor.Red, Color.red }
    };
    
    public void Init(GameColor gameColor)
    {
        GameColor = gameColor;
        ChangeColor(_colors[gameColor]);
    }

    private void ChangeColor(Color colorOfObject)
    {
        var rend = GetComponent<Renderer>();
        rend.material.color = colorOfObject;
    }
}