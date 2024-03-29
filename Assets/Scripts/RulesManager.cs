using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameBoardHelper _gameBoardHelper;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] private ScoreManager _scoreManager;

    public bool CanUserBeatEnemy(GameBoardCell currentCell, GameBoardCell selectedCell)
    {
        if (!CanJump(currentCell.Position, selectedCell.Position)) return false;

        var beatPosition =
            _gameBoardHelper.GetCellBetween(currentCell.Position, selectedCell.Position);
        return !beatPosition.IsEmpty && beatPosition.PlacedChecker.GameColor !=
            _gameManager.CurrentPlayer.GameColor;
    }

    public bool CanUserBeatEnemyAgain(GameBoardCell cell)
    {
        var availableCells = _gameBoardHelper.GetAvailableJumpDestinationCells(cell);
        if (!availableCells.Any()) return false;

        return availableCells.Any(cellToJump => CanUserBeatEnemy(cell, cellToJump));
    }

    public void CheckIfPlayerHasBeatenAllCheckers()
    {
        var isPlayerBeatAllEnemyCheckers = _scoreManager.PlayerScores.Values.Any(score => score == _checkerBoard.StartCheckersCount);
        if (!isPlayerBeatAllEnemyCheckers) return;

        var winColor = _scoreManager.PlayerScores.First(x => x.Value == _checkerBoard.StartCheckersCount).Key;
        OnPlayerHasWon(winColor);
    }

    private static bool CanJump(Vector2Int currentPosition, Vector2Int destPosition)
    {
        var availableDistanceToBeatEnemy = 2;
        return Mathf.Abs(destPosition.x - currentPosition.x) == availableDistanceToBeatEnemy &&
               Mathf.Abs(destPosition.y - currentPosition.y) == availableDistanceToBeatEnemy;
    }

    public bool IsCellsWithinBound(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _checkerBoard.Cells.GetLength(0) &&
               pos.y >= 0 && pos.y < _checkerBoard.Cells.GetLength(1);
    }

    //private bool AreNearestCellsEmpty([NotNull] List<GameBoardCell> listOfCells, GameBoardCell currentCell)
    // {
    //     if (listOfCells == null) throw new ArgumentNullException(nameof(listOfCells));
    //
    //     var allPossibleEnemyCheckersAreEmpty = listOfCells
    //         .Select(cell => (cell.Position + currentCell.Position) / 2)
    //         .Select(cellPos => _checkerBoard.Cells[cellPos.x, cellPos.y])
    //         .All(x => x.IsEmpty);
    //     return allPossibleEnemyCheckersAreEmpty;
    // }

    public void CheckPossibilityOfMovements()
    {
        const int radius = 1;
        var isCellHasToMove = false;
        foreach (var cell in _checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _gameManager.CurrentPlayer.GameColor) continue;

            var potentialPositions = _gameBoardHelper.GetPotentialPositions(cell, radius);
            var availableCellsToMove = _gameBoardHelper.GetBoardCells(potentialPositions);
            availableCellsToMove = availableCellsToMove.Where(x => !x.IsEmpty).ToList();
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
       _gameManager.SwitchOffPlayerGame();
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
        _gameManager.SwitchOffPlayerGame();
    }

    public bool IsCheckerCanBeMoved(int direction, GameBoardCell previousSelectedCell,
        GameBoardCell selectedCell)
    {
        return selectedCell.Position.x - previousSelectedCell.Position.x == direction &&
               Mathf.Abs(selectedCell.Position.y - previousSelectedCell.Position.y) == 1;
    }
    
    public bool IsCheckerCanBeMovedToNeighbourCell(GameBoardCell selectedCell)
    {
        return _gameManager.CurrentlySelectedCell != null && _gameManager.CurrentlySelectedCell.HasRisenPlacedObject &&
               selectedCell.IsEmpty;
    }
    
    // private void CheckIfPlayerCanBeatEnemyChecker()
    // {
    //     foreach (var cell in checkerBoard.Cells)
    //     {
    //         if (cell.IsEmpty || cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;
    //
    //         CanUserBeatEnemyAgain(cell);
    //         if (CanUserBeatEnemyAgain(cell))
    //         {
    //             _mustUseSpecificCheckerCellToMove = cell;
    //             // _mustUseSpecificCheckerCellToMove = _moveData.StartCell;
    //         }
    //     }
    // }
}