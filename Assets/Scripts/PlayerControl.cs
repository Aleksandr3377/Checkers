using System;
using UnityEngine;

public class PlayerControl : PlayerControlBase
{
    //public event Action<GameBoardCell> LowerTheChecker;
    public event Action<GameBoardCell> MoveChecker;
   // public event Action<GameBoardCell> CheckerWasBeaten;
    public GameManager gameManager;
    private Transform _selectedWhiteChecker;
    private Transform _selectedRedChecker;
    private Transform _selectedCell;
    [SerializeField] private GameColor gameColor;
 
    private void Update()
    {
        TryToSelectChecker();
        MoveCheckerToCell();
        BeatChecker();
    }

    private void TryToSelectChecker()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cell = gameManager.GetSelectedMouseObject("Cell");
        if (cell != null)
        {
            OnCellSelected(cell.GetComponent<GameBoardCell>(), gameColor);
           // LowerTheChecker?.Invoke(cell.GetComponent<GameBoardCell>());
        }
        
    }

 
    private void MoveCheckerToCell()
    {
        if (Input.GetMouseButton(0) && gameManager.allowanceForSelectingChecker == false)
        {
            GetCell(MoveChecker);
        }
    }

    private void BeatChecker()
    {
        if (Input.GetMouseButton(0) && gameManager.allowanceForSelectingChecker == false)
        {
         //   GetCell(CheckerWasBeaten);
        }
    }

    private void GetCell(Action<GameBoardCell> action)
    {
        var cell = gameManager.GetSelectedMouseObject("Cell");
        action?.Invoke(cell.GetComponent<GameBoardCell>());
        gameManager.allowanceForSelectingChecker = true;
    }
}