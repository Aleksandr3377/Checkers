using System;
using UnityEngine;

public abstract class PlayerControlBase:MonoBehaviour
{
    public event Action<GameBoardCell, GameColor> CellWasSelected; 
    [SerializeField] private GameColor _gameColor;
    public GameColor GameColor => _gameColor;

    protected void OnCellSelected(GameBoardCell cell, GameColor color)
    {
        CellWasSelected?.Invoke(cell,color);
    }
    
    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}