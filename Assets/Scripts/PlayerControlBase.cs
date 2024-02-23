using System;
using UnityEngine;

public abstract class PlayerControlBase:MonoBehaviour
{
    public event Action<GameBoardCell, GameColor> CellWasSelected;
    [SerializeField] private GameColor gameColor;
    public GameColor GameColor => gameColor;

    protected void OnCellSelected(GameBoardCell cell,GameColor color)
    {
        CellWasSelected?.Invoke(cell,color);
    }
    
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}