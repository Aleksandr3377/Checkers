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
    private bool _playerLost;
    private GameBoardCell _mandatoryCell;
    private GameBoardCell CurrentlySelectedCell=>checkerBoard.Cells.Cast<GameBoardCell>().
        FirstOrDefault(x=>x.HasRisenPlacedObject&&x.PlacedChecker.GameColor==_currentPlayer.GameColor);
    
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

    private void MakeMove(GameColor color, GameBoardCell previousSelectedCell,
        GameBoardCell selectedCell)
    {
        var playerDirection = GetPlayerDirection(color);
        if (IsCheckerCanBeMoved(playerDirection, previousSelectedCell, selectedCell))
        {
            MoveCheckerToCell(previousSelectedCell, selectedCell);
            SwitchPlayer();
        }
        else
        {
            TryToBeatEnemyChecker(previousSelectedCell, selectedCell);
        }
    }

    private void TryToBeatEnemyChecker(GameBoardCell previousSelectedCell, GameBoardCell selectedCell)
    {
        if (!IsCheckerCanBeatEnemy(previousSelectedCell, selectedCell)) return;

        var cellBetweenSelectedAndPrev =
            GetCellBetween(previousSelectedCell.Position, selectedCell.Position);
        MoveCheckerToCell(previousSelectedCell, selectedCell);
        DisableAndForgetChecker(cellBetweenSelectedAndPrev);
        AddScore();
        if (CanUserBeatEnemyAgain(selectedCell)) // ! додав і почало працювати
        {
            _mandatoryCell = selectedCell;
        }
        else
        {
            _mandatoryCell = null;
            SwitchPlayer();
        }
    }

    private void CheckPossibilityOfMovements()
    {
        const int radius = 1;
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.IsEmpty||cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;
            
            var potentialPositions = GetPotentialPositions(cell, radius);
            var availableCellsToMove = GetBoardCells(potentialPositions);
            availableCellsToMove = availableCellsToMove.Where(x => !x.IsEmpty).ToList();
            _playerLost = !availableCellsToMove.Any();
            if (_playerLost)
            {
                DefineWinnerByImpossibleMovement(_currentPlayer.GameColor);
            }
        }
    }

    private void CheckIfPlayerCanBeatEnemyChecker()
    {
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;

            CanUserBeatEnemyAgain(cell);
        }
    }

    private bool CanUserBeatEnemyAgain(GameBoardCell cell)
    { 
        var availableCells = GetAvailableJumpDestinationCells(cell); 
        return availableCells.Any() && !AreNearestCellsEmpty(availableCells, cell);
            // !
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
        var potentialPositions = GetPotentialPositions(cell,distanceToSearch);
        var boardCells= GetBoardCells(potentialPositions);
        return boardCells.Where(x => cell.IsEmpty).ToList();
    }

    private List<GameBoardCell> GetBoardCells(Vector2Int[] positions)
    {
        return positions
            .Where(IsCellsWithinBounds)
            .Select(pos => checkerBoard.Cells[pos.x, pos.y]).ToList();
        //   .Where(cell => cell.IsEmpty==isEmptyCheck).ToList();
    }
    
    private static Vector2Int[] GetPotentialPositions(GameBoardCell cell,int distance)
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

    private bool IsCellsWithinBounds(Vector2Int pos)
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

    private static void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    private bool IsSelectedCellHasRisenObject(GameBoardCell selectedCell) => selectedCell.HasRisenPlacedObject;

    private bool IsCheckerEmptyOrEqualsPlayerColor(GameBoardCell selectedCell)
    {
        return IsCellEmpty(selectedCell) || IsCheckerEqualsPlayerColor(selectedCell);
    }

    private bool IsCheckerCanBeMoved(int direction, GameBoardCell previousSelectedCell, GameBoardCell selectedCell) =>
        selectedCell.Position.x - previousSelectedCell.Position.x == direction &&
        Mathf.Abs(selectedCell.Position.y - previousSelectedCell.Position.y) == 1;

    private void DisableAndForgetChecker(GameBoardCell cellBetweenSelectedAndPrev)
    {
        cellBetweenSelectedAndPrev.PlacedChecker.gameObject.SetActive(false);
        cellBetweenSelectedAndPrev.ForgetPlacedChecker();
    }

    private void TryToMove(GameBoardCell selectedCell, GameColor playerColor)
    {
        if (IsSelectedCellHasRisenObject(selectedCell))
        {
            selectedCell.MovePlacedObjectToGround();
            return;
        }

        if (!IsCheckerEmptyOrEqualsPlayerColor(selectedCell)) return;

        if (!IsCellEmpty(selectedCell))
        {
            if (_mandatoryCell != null)
            {
                RiseCheckerOrLower(CurrentlySelectedCell);
            }
            else
            {
                RiseCheckerOrLower(selectedCell);
                return;
            }
           
        }

        if (!IsCheckerCanBeMovedToNeighbourCell(selectedCell)) return;

        if (CheckIfMandatoryCheckerNotEqualsPrevCheckerAndIsNotNull()) return;

        MakeMove(playerColor, CurrentlySelectedCell, selectedCell);
    }

    private void RiseCheckerOrLower(GameBoardCell cell)
    {      
        if (IsPrevCellRisen()) CurrentlySelectedCell.MovePlacedObjectToGround();

        cell.RisePlacedObject();
    }

    private void SetInitialPlayer() => _currentPlayer = players[0];

    private void DeactivateCurrentPlayer()
    {
        _currentPlayer.CellWasSelected -= TryToMove;
        _currentPlayer.Deactivate();
    }

    private void ActivateCurrentPlayer()
    {
        _currentPlayer.CellWasSelected += TryToMove;
        _currentPlayer.Activate();
    }

    private void SetNextPlayer()
    {
        var nextPlayerIndex = GetIndexOfNextPlayer();
        _currentPlayer = players[nextPlayerIndex];
    }

    private void DeactivateAllPlayers()
    {
        foreach (var player in players)
        {
            player.Deactivate();
        }
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

    private bool IsIndexWithinBounds(int currentIndex) => currentIndex == players.Length - 1;

    private bool IsPrevCellRisen() => CurrentlySelectedCell != null && CurrentlySelectedCell.HasRisenPlacedObject;

    private bool IsCheckerCanBeMovedToNeighbourCell(GameBoardCell selectedCell)
    {
        return CurrentlySelectedCell != null && CurrentlySelectedCell.HasRisenPlacedObject && selectedCell.IsEmpty;
    }

    private bool CheckIfMandatoryCheckerNotEqualsPrevCheckerAndIsNotNull()
    {
        return _mandatoryCell != null && _mandatoryCell != CurrentlySelectedCell;
    }

    private int GetCurrentPlayerIndex()
    {
        var currentIndex = Array.IndexOf(players, _currentPlayer);
        return currentIndex;
    }
    
    private bool IsPlayerCanBeatEnemyChecker(Vector2Int prevPosition, Vector2Int destPosition)
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