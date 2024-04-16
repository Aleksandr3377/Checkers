using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public GameColor GameColor { get; private set; }

    [SerializeField] private MeshRenderer _renderer;
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
        _renderer.material.color = colorOfObject;
    }
}