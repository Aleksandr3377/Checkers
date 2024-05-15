using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameBoardHelper _gameBoardHelper;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] private ScoreManager _scoreManager;
    private MoveData _moveData;

    public bool CanUserBeatEnemy(GameBoardCell currentCell, GameBoardCell selectedCell)
    {
        if (!CanJump(currentCell.Position, selectedCell.Position)) return false;

        var beatPosition =
            _gameBoardHelper.GetCellBetweenStartAndDestCells(currentCell.Position, selectedCell.Position);
        return !beatPosition.IsEmpty && beatPosition.PlacedChecker.GameColor !=
            _gameManager.CurrentPlayer.GameColor;
    }

    public bool CanUserBeatEnemy(GameBoardCell cell)
    {
        var availableCells = _gameBoardHelper.GetAvailableJumpDestinationCells(cell);
        return availableCells.Any(cellToJump => CanUserBeatEnemy(cell, cellToJump));
    }

    public void CheckIfPlayerHasBeatenAllCheckers()
    {
        var isPlayerBeatAllEnemyCheckers =
            _scoreManager.PlayerScores.Any(score => score.Value ==_checkerBoard.StartCheckersCount);
        if (!isPlayerBeatAllEnemyCheckers) return;

        var winColor = _scoreManager.PlayerScores.First(x => x.Value == _checkerBoard.StartCheckersCount).Key;
        OnPlayerHasWon(winColor);
    }

    private static bool CanJump(Vector2Int currentPosition, Vector2Int destPosition)
    {
        var distanceToBeatEnemy = 2;
        return Mathf.Abs(destPosition.x - currentPosition.x) == distanceToBeatEnemy &&
               Mathf.Abs(destPosition.y - currentPosition.y) == distanceToBeatEnemy;
    }

    public bool IsCellsWithinBound(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _checkerBoard.Cells.GetLength(0) &&
               pos.y >= 0 && pos.y < _checkerBoard.Cells.GetLength(1);
    }

    public void CheckPossibilityOfMovements()
    {
        var isCellHasToMove = false;
        foreach (var cell in _checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _gameManager.CurrentPlayer.GameColor) continue;
            
            var potentialPositions = _gameBoardHelper.GetPotentialPositionsToMove(cell, _gameManager.CurrentPlayer.GameColor);
            var availableCellsToMove = _gameBoardHelper.GetBoardCells(potentialPositions);
            availableCellsToMove = availableCellsToMove.Where(x => x.IsEmpty).ToList();
            isCellHasToMove = availableCellsToMove.Any();
            if (isCellHasToMove) break;
        }

        if (isCellHasToMove) return;

        DefineWinnerByImpossibleMovement(_gameManager.CurrentPlayer.GameColor);
    }

    private void DefineWinnerByImpossibleMovement(GameColor color)
    {
        if (_gameManager.CurrentPlayer.GameColor == color)
            _gameManager.DefinedWinner.text = "Red player has won";
        
        else if (_gameManager.CurrentPlayer.GameColor != color)
            _gameManager.DefinedWinner.text = "White player has won";
        _gameManager.ActivateButtonRestart();
    }

    private void OnPlayerHasWon(GameColor gameColor)
    {
        _gameManager.DefinedWinner.text = gameColor switch
        {
            GameColor.White => "Red player won",
            GameColor.Red => "White player won",
            _ => ""
        };
        _gameManager.ActivateButtonRestart();
    }

    public bool IsCheckerCanBeMoved(int direction, GameBoardCell previousSelectedCell,
        GameBoardCell selectedCell)
    {
        return (selectedCell.Position.x - previousSelectedCell.Position.x == direction &&
                Mathf.Abs(selectedCell.Position.y - previousSelectedCell.Position.y) == 1);
    }

    public bool IsCheckerCanBeMovedToNeighbourCell(GameBoardCell selectedCell)
    {
        return _gameManager.CurrentlySelectedCell != null && _gameManager.CurrentlySelectedCell.HasRisenPlacedObject &&
               selectedCell.IsEmpty;
    }

    public void CheckIfPlayerMustBeatEnemyChecker()
    {
        foreach (var cell in _checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _gameManager.CurrentPlayer.GameColor) continue;

            if (CanUserBeatEnemy(cell))
            {
                // _gameManager.MoveData.StartCellLocked = false;
                 //_gameManager.MoveData.StartCell = cell;
               //  _gameManager.MoveData.StartCellLocked = true;
              _gameManager.MoveData.RestrictToMoveCell.Add(cell);
            }

          //  _gameManager.MoveData.StartCellLocked = false;
        }
    }

    // public void CheckIfCheckerTransformedToQueen()
    // {
    //     for (var row = 0; row < _checkerBoard.Rows; row++)
    //     {
    //         for (var column = 0; column < _checkerBoard.Colums; column++)
    //         {
    //             var cell = _checkerBoard.Cells[row, column];
    //             if (!cell.IsEmpty && _gameManager.CurrentPlayer.GameColor == GameColor.White)
    //             {
    //                 var positionWhereCellCanTransformToChecker = 7;
    //
    //                 if (cell != _checkerBoard.Cells[positionWhereCellCanTransformToChecker, column]) continue;
    //
    //                 _moveData.QueenCells.Add(cell);
    //                     // _gameBoardHelper.TransformCheckerToQueen(cell);
    //             }
    //             else
    //             {
    //                 var positionWhereCellCanTransformToChecker = 0;
    //                 if (cell != _checkerBoard.Cells[positionWhereCellCanTransformToChecker, column]) continue;
    //
    //                 _moveData.QueenCells.Add(cell);
    //                // _gameBoardHelper.TransformCheckerToQueen(cell);
    //             }
    //         }
    //     }
    // }
}