using System;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerControlBase[] players;
    private PlayerControlBase _currentPlayer;
    public CheckerBoard checkerBoard;
    private GameBoardCell _previousSelectedCell;

    private void Start()
    {
        foreach (var player in players)
        {
            player.Deactivate();
        }
        
        SwitchPlayer();
    }
    

    private void SwitchPlayer()
    {
        if (_currentPlayer == null)
        {
            _currentPlayer = players[0];
        }
        else
        {
            _currentPlayer.CellWasSelected -= OnCellSelected;
            //  _currentPlayer.CheckerWasMoved -= OnCellSelectedToMove;
            _currentPlayer.Deactivate();
            var currentIndex = Array.IndexOf(players, _currentPlayer);
            int nextPlayerIndex;
            if (currentIndex == players.Length - 1)
            {
                nextPlayerIndex = 0;
            }
            else
            {
                nextPlayerIndex = currentIndex + 1;
            }

            _currentPlayer = players[nextPlayerIndex];
        }

        _currentPlayer.CellWasSelected += OnCellSelected;
        //_currentPlayer.CheckerWasMoved += OnCellSelectedToMove;
        _currentPlayer.Activate();
    }

    private void OnCellSelected(GameBoardCell selectedCell, GameColor playerColor)
    {
        if (selectedCell.HasRisenPlacedObject)
        {
            selectedCell.MovePlacedObjectToGround();
            _previousSelectedCell = null;
            return;
        }
        
        // Якщо нова клітинка не пуста і має шашку іношого кольору 
        if (!selectedCell.IsEmpty && selectedCell.PlacedChecker.GameColor != playerColor) return;
        // Зробити щоб коли настиснув на нову клітинку яка має шашку , тоді попередню опустити і нову підняти і зберегти currentlySelected
        if (!selectedCell.IsEmpty)
        {
            selectedCell.RisePlacedObject();
            if (_previousSelectedCell!=null && _previousSelectedCell.HasRisenPlacedObject)
            {
                _previousSelectedCell.MovePlacedObjectToGround();
            }

            _previousSelectedCell = selectedCell;
            return;
        }
        // Якщо юзер вибрав клітинку с шашкой і нова клітинка не має шашки тоді зробити хід
        if (_previousSelectedCell != null && _previousSelectedCell.HasRisenPlacedObject && selectedCell.IsEmpty)
        {
            if ((Mathf.Abs(selectedCell.Position.x - _previousSelectedCell.Position.x) == 1) &&
                (Mathf.Abs(selectedCell.Position.y - _previousSelectedCell.Position.y) == 1))
            { 
                MoveCheckerToCell(_previousSelectedCell,selectedCell);
                _previousSelectedCell = null;
                SwitchPlayer();
            }
        }

        if (_previousSelectedCell != null && _previousSelectedCell.HasRisenPlacedObject && !selectedCell.IsEmpty)
        {
            if ((Mathf.Abs(selectedCell.Position.x - _previousSelectedCell.Position.x) == 1) &&
                (Mathf.Abs(selectedCell.Position.y - _previousSelectedCell.Position.y) == 1))
            {
                MoveCheckerToCell(_previousSelectedCell,selectedCell);
                selectedCell.PlacedChecker.gameObject.SetActive(false);
                _previousSelectedCell = null;
                SwitchPlayer();
            }
        }
    }

    private void MoveCheckerToCell(GameBoardCell initialCell,GameBoardCell destinationCell)
    {
        if (initialCell == null || destinationCell == null) throw new ArgumentNullException();

        var checkerTransform=initialCell.PlacedChecker.transform;
        var endPosition = destinationCell.anchor.position + (0.5f * checkerTransform.lossyScale.y*Vector3.up);
        initialCell.PlacedChecker.transform
            .DOMove(endPosition, 0.5f)
            .OnComplete(() =>
            {
                destinationCell.Place(initialCell.PlacedChecker);
                initialCell.ForgivePlacedChecker();
            });
    }

    private (int,int) FindLocationOfArray(GameBoardCell cell)
    {
        var cellRow=0;
        var cellColumn = 0;
        for (var row = 0; row < checkerBoard._rows; row++)
        {
            for (var column = 0; column < checkerBoard._colums; column++)
            {
                if (checkerBoard._cells[row, column] != cell) continue;
                
                cellRow = row;
                cellColumn = column;
                break;
            }
        }
        
        return (cellRow,cellColumn);
    }
}