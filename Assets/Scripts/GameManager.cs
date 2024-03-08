using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public sealed class GameManager : MonoBehaviour
{
    public PlayerControlBase[] players;
    public ScoreManager scoreManager;
    public CheckerBoard checkerBoard;
    public event Action<int, GameColor> PlayerBeatEnemyChecker;
    public Button buttonRestart;
    public TextMeshProUGUI definedWinner;
    private PlayerControlBase _currentPlayer;
    private GameBoardCell _mustUseSpecificCheckerCellToMove;
    private MoveData _moveData;
    private GameBoardCell CurrentlySelectedCell => checkerBoard.Cells.Cast<GameBoardCell>()
        .FirstOrDefault(x => x.HasRisenPlacedObject && x.PlacedChecker.GameColor == _currentPlayer.GameColor);

    private void Start()
    {
        DeactivateAllPlayers();
        SwitchPlayer();
        scoreManager.PlayerHasWon += OnPlayerHasWon;
        buttonRestart.onClick.AddListener(RestartGame);
    }

    private void Update()
    {
        CheckPossibilityOfMovements();
    }

    private void SwitchPlayer()
    {
        if (IsCurrentPlayerNull())
        {
            SetInitialPlayer();
        }
        else
        {
            DeactivateCurrentPlayer();
            SetNextPlayer();
        }

        ActivateCurrentPlayer();
    }

    private void MakeMove(MoveData moveData)
    {
        var playerDirection = GetPlayerDirection(_currentPlayer.GameColor);
        var startCell = moveData.StartCell;
        var destCell = moveData.DestCell;
        if (IsCheckerCanBeMoved(playerDirection, startCell, destCell))
        {
            MoveCheckerToCell(startCell, destCell);
            SwitchPlayer();
            //CheckIfPlayerCanBeatEnemyChecker();
        }
        else
        {
            TryToBeatEnemyChecker(moveData);
        }
    }

    private void TryToBeatEnemyChecker(MoveData moveData)
    {
        if (!IsCheckerCanBeatEnemy(moveData.StartCell, moveData.DestCell)) return;

        var cellBetweenSelectedAndPrev =
            GetCellBetween(moveData.StartCell.Position, moveData.DestCell.Position);
        MoveCheckerToCell(moveData.StartCell, moveData.DestCell);
        DisableAndForgetChecker(cellBetweenSelectedAndPrev);
        AddScore();
        if (CanUserBeatEnemyAgain(moveData.DestCell))
        {
            _mustUseSpecificCheckerCellToMove = moveData.DestCell;
        }
        else
        {
            _mustUseSpecificCheckerCellToMove = null;
            SwitchPlayer();
          //  CheckIfPlayerCanBeatEnemyChecker();
        }
    }

    private void CheckPossibilityOfMovements()
    {
        const int radius = 1;
        var hasCellToMove = false;
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;

            var potentialPositions = GetPotentialPositions(cell, radius);
            var availableCellsToMove = GetBoardCells(potentialPositions);
            availableCellsToMove = availableCellsToMove.Where(x => !x.IsEmpty).ToList();
            hasCellToMove = availableCellsToMove.Any();
            if(hasCellToMove) break;
        }

        if (hasCellToMove) return;
        
        DefineWinnerByImpossibleMovement(_currentPlayer.GameColor);
    }

    private void CheckIfPlayerCanBeatEnemyChecker()
    {
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;

            if (CanUserBeatEnemyAgain(cell))
            {
                _mustUseSpecificCheckerCellToMove = cell;
               // _mustUseSpecificCheckerCellToMove = _moveData.StartCell;
            }
        }
    }

    private bool CanUserBeatEnemyAgain(GameBoardCell cell)
    {
        var availableCells = GetAvailableJumpDestinationCells(cell);
        return availableCells.Any() && !AreNearestCellsEmpty(availableCells, cell);
    }

    private bool AreNearestCellsEmpty([NotNull] List<GameBoardCell> listOfCells, GameBoardCell currentCell)
    {
        if (listOfCells == null) throw new ArgumentNullException(nameof(listOfCells));

        var allPossibleEnemyCheckersAreEmpty = listOfCells
            .Select(cell => (cell.Position + currentCell.Position) / 2)
            .Select(cellPos => checkerBoard.Cells[cellPos.x, cellPos.y])
            .All(IsCellEmpty);
        return allPossibleEnemyCheckersAreEmpty;
    }

    private List<GameBoardCell> GetAvailableJumpDestinationCells(GameBoardCell cell)
    {
        const int distanceToSearch = 2;
        var potentialPositions = GetPotentialPositions(cell, distanceToSearch);
        var boardCells = GetBoardCells(potentialPositions);
        return boardCells.Where(x => x.IsEmpty).ToList();
    }

    private List<GameBoardCell> GetBoardCells(Vector2Int[] positions)
    {
        return positions
            .Where(pos=>IsCellsWithinBound(pos))
            .Select(pos => checkerBoard.Cells[pos.x, pos.y]).ToList();
    }

    private static Vector2Int[] GetPotentialPositions(GameBoardCell cell, int distance)
    {
        var potentialPositions = new[]
        {
            cell.Position + new Vector2Int(distance, distance),
            cell.Position + new Vector2Int(distance, -distance),
            cell.Position + new Vector2Int(-distance, distance),
            cell.Position + new Vector2Int(-distance, -distance)
        };
        return potentialPositions;
    }

    private bool IsCellsWithinBound(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < checkerBoard.Cells.GetLength(0) &&
               pos.y >= 0 && pos.y < checkerBoard.Cells.GetLength(1);
    }

    private bool IsCheckerCanBeatEnemy(GameBoardCell previousSelectedCell, GameBoardCell selectedCell)
    {
        if (!IsPlayerCanBeatEnemyChecker(previousSelectedCell.Position, selectedCell.Position)) return false;

        var cellBetweenSelectedAndPrev = GetCellBetween(previousSelectedCell.Position, selectedCell.Position);
        return !IsCellEmpty(cellBetweenSelectedAndPrev) && !IsCheckerEqualsPlayerColor(cellBetweenSelectedAndPrev);
    }

    private static void MoveCheckerToCell(GameBoardCell initialCell, GameBoardCell destinationCell)
    {
        if (CheckForNull(initialCell, destinationCell)) throw new ArgumentNullException();

        var endPosition = GetEndPosition(initialCell, destinationCell);
        MoveChecker(initialCell, destinationCell, endPosition);
    }

    private static void MoveChecker(GameBoardCell initialCell, GameBoardCell destinationCell, Vector3 endPosition)
    {
        initialCell.PlacedChecker.transform
            .DOMove(endPosition, 0.5f)
            .OnComplete(() =>
            {
                destinationCell.Place(initialCell.PlacedChecker);
                initialCell.ForgetPlacedChecker();
            });
    }

    private static Vector3 GetEndPosition(GameBoardCell initialCell, GameBoardCell destinationCell)
    {
        var checkerTransform = initialCell.PlacedChecker.transform;
        var endPosition = destinationCell.anchor.position + 0.5f * checkerTransform.lossyScale.y * Vector3.up;
        return endPosition;
    }

    private static bool CheckForNull(Object initialCell, [NotNull] GameBoardCell destinationCell)
    {
        if (destinationCell == null) throw new ArgumentNullException(nameof(destinationCell));
        return initialCell == null || destinationCell == null;
    }

    private void OnPlayerHasWon(GameColor gameColor)
    {
        definedWinner.text = gameColor switch
        {
            GameColor.White => "Red player won",
            GameColor.Red => "White player won",
            _ => ""
        };
        ActivateButtonRestart();
    }

    private static int GetPlayerDirection(GameColor color)
    {
        var playerDirection = color == GameColor.White ? 1 : -1;
        return playerDirection;
    }

    private bool IsCurrentPlayerNull()
    {
        return _currentPlayer == null;
    }

    private void AddScore()
    {
        var addPointToCurrentScore = ++scoreManager.PlayerScores[_currentPlayer.GameColor];
        PlayerBeatEnemyChecker?.Invoke(addPointToCurrentScore, _currentPlayer.GameColor);
    }

    private void DefineWinnerByImpossibleMovement(GameColor color)
    {
        if (_currentPlayer.GameColor == color)
            definedWinner.text = "Red player has won";
        else if (_currentPlayer.GameColor != color) definedWinner.text = "White player has won";
    }

    private void ActivateButtonRestart()
    {
        buttonRestart.gameObject.SetActive(true);
    }

    private static void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private static bool IsSelectedCellHasRisenObject(GameBoardCell selectedCell)
    {
        return selectedCell.HasRisenPlacedObject;
    }
    
    private static bool IsCheckerCanBeMoved(int direction, GameBoardCell previousSelectedCell, GameBoardCell selectedCell)
    {
        return selectedCell.Position.x - previousSelectedCell.Position.x == direction &&
               Mathf.Abs(selectedCell.Position.y - previousSelectedCell.Position.y) == 1;
    }

    private static void DisableAndForgetChecker(GameBoardCell cellBetweenSelectedAndPrev)
    {
        cellBetweenSelectedAndPrev.PlacedChecker.gameObject.SetActive(false);
        cellBetweenSelectedAndPrev.ForgetPlacedChecker();
    }

    private void TryToMove(GameBoardCell cell, GameColor playerColor) //todo:refactor
    {
        if (IsSelectedCellHasRisenObject(cell))
        {
            cell.MovePlacedObjectToGround();
            return;
        }

        if (!IsCellEmpty(cell) && !IsCheckerEqualsPlayerColor(cell)) return;

        if (!IsCellEmpty(cell))
        { 
            DecideUpOrLowerCheckerOnCell(cell);
        }

        if (!IsCheckerCanBeMovedToNeighbourCell(cell)) return;

        if (!IsPlayerMustMoveSpecificCellChecker())
        {
            SetMoveDataCells(cell);
            MakeMove(_moveData);
        }

        if (IsPlayerMustMoveSpecificCellChecker() && _mustUseSpecificCheckerCellToMove == CurrentlySelectedCell)
        {
            SetMoveDataCells(cell);
            TryToBeatEnemyChecker(_moveData);
        }
    }

    private bool IsPlayerMustMoveSpecificCellChecker() => _mustUseSpecificCheckerCellToMove != null;

    private void SetMoveDataCells(GameBoardCell cell)
    {
        _moveData.StartCell = CurrentlySelectedCell;
        _moveData.DestCell = cell;
    }
    
    private void DecideUpOrLowerCheckerOnCell(GameBoardCell cell)
    {
        if (IsPrevCellRisen())
        {
            CurrentlySelectedCell.MovePlacedObjectToGround();
        }

        cell.RisePlacedObject();
    }

    private void SetInitialPlayer()
    {
        _currentPlayer = players[0];
    }

    private void DeactivateCurrentPlayer()
    {
        _currentPlayer.CellWasSelected -= TryToMove;
        _currentPlayer.Deactivate();
    }

    private void ActivateCurrentPlayer()
    {
        _currentPlayer.CellWasSelected += TryToMove;
        _currentPlayer.Activate();
        _moveData = new MoveData();
    }

    private void SetNextPlayer()
    {
        var nextPlayerIndex = GetIndexOfNextPlayer();
        _currentPlayer = players[nextPlayerIndex];
    }

    private void DeactivateAllPlayers()
    {
        foreach (var player in players) player.Deactivate();
    }

    private int GetIndexOfNextPlayer()
    {
        var currentIndex = GetCurrentPlayerIndex();
        int nextPlayerIndex;
        if (IsIndexWithinBounds(currentIndex))
            nextPlayerIndex = 0;
        else
            nextPlayerIndex = currentIndex + 1;

        return nextPlayerIndex;
    }

    private bool IsIndexWithinBounds(int currentIndex)
    {
        return currentIndex == players.Length - 1;
    }

    private bool IsPrevCellRisen()
    {
        return CurrentlySelectedCell != null && CurrentlySelectedCell.HasRisenPlacedObject;
    }

    private bool IsCheckerCanBeMovedToNeighbourCell(GameBoardCell selectedCell)
    {
        return CurrentlySelectedCell != null && CurrentlySelectedCell.HasRisenPlacedObject && selectedCell.IsEmpty;
    }
    
    private int GetCurrentPlayerIndex()
    {
        var currentIndex = Array.IndexOf(players, _currentPlayer);
        return currentIndex;
    }

    private static bool IsPlayerCanBeatEnemyChecker(Vector2Int prevPosition, Vector2Int destPosition)
    {
        return Mathf.Abs(destPosition.x - prevPosition.x) == 2 &&
               Mathf.Abs(destPosition.y - prevPosition.y) == 2;
    }

    private GameBoardCell GetCellBetween(Vector2Int prevPosition, Vector2Int destPosition)
    {
        var jumpThroughCellPos = (prevPosition + destPosition) / 2;
        return checkerBoard.Cells[jumpThroughCellPos.x, jumpThroughCellPos.y];
    }

    private static bool IsCellEmpty(GameBoardCell cellBetweenSelectedAndPrev)
    {
        return cellBetweenSelectedAndPrev.IsEmpty;
    }

    private bool IsCheckerEqualsPlayerColor(GameBoardCell cellBetweenSelectedAndPrev)
    {
        return cellBetweenSelectedAndPrev.PlacedChecker.GameColor == _currentPlayer.GameColor;
    }
}