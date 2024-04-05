using System;
using UnityEngine;

public abstract class PlayerControlBase:MonoBehaviour
{
    public event Action<GameBoardCell, GameColor> CellWasSelected; 
    [SerializeField] private GameColor _gameColor;
    public GameColor GameColor
    {
        get => _gameColor;
        set => _gameColor = value;
    }

    protected GameBoardCell StartCell;

    protected void OnCellSelected(GameBoardCell cell, GameColor color)
    {
       // Deactivate();
        CellWasSelected?.Invoke(cell,color);
    }
    
    public virtual void SelectCell(GameBoardCell startCell = null)
    {
        StartCell = startCell;
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}