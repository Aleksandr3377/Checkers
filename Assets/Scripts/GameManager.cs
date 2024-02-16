using System;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerControlBase[] players;
    private PlayerControlBase _currentPlayer;
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

    private void OnCellSelected(GameBoardCell cell, GameColor playerColor)
    {
        if (cell.HasRisenPlacedObject)
        {
            cell.MovePlacedObjectToGround();
            _previousSelectedCell = null;
            return;
        }
        
        // Якщо нова клітинка не пуста і має шашку іношого кольору 
        if (!cell.IsEmpty && cell.PlacedChecker.GameColor != playerColor) return;
        // Зробити щоб коли настиснув на нову клітинку яка має шашку , тоді попередню опустити і нову підняти і зберегти currentlySelected
        if (!cell.IsEmpty)
        {
            cell.RisePlacedObject();
            if (_previousSelectedCell!=null && _previousSelectedCell.HasRisenPlacedObject)
            {
                _previousSelectedCell.MovePlacedObjectToGround();
            }

            _previousSelectedCell = cell;
            return;
        }
        // Якщо юзер вибрав клітинку с шашкой і нова клітинка не має шашки тоді зробити хід
        if (_previousSelectedCell != null && _previousSelectedCell.HasRisenPlacedObject && cell.IsEmpty)
        {
           MoveCheckerToCell(_previousSelectedCell,cell);
            _previousSelectedCell = null;
            SwitchPlayer();
        }
    }

    private void MoveCheckerToCell(GameBoardCell initialCell,GameBoardCell destinationCell)
    {
        var position = destinationCell.transform.position;
       initialCell.PlacedChecker.transform.DOMove(new Vector3(position.x, destinationCell.anchor.transform.position.y, position.z), 0.5f)
            .OnComplete(()=>initialCell.Place(destinationCell.PlacedChecker));
        initialCell.PlacedChecker = null;
    }
}