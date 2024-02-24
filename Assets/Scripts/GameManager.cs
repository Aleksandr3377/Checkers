using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerControlBase[] players;
    public CheckerBoard checkerBoard;
    public Action<int,GameColor> PlayerBeatEnemyChecker;
    public Button buttonRestart;
    public TextMeshProUGUI definedWinner;
    private GameBoardCell _previousSelectedCell;
    private PlayerControlBase _currentPlayer;
    private Action<GameColor> _playerHasWon;
    private int _scoreOfRedPlayerBeatenCheckers;
    private int _scoreOfWhitePlayerBeatenCheckers;
    private bool _allowanceForMovingSelectedChecker=false;
    private GameBoardCell _mustMoveThisChecker;

    private void Start()
    {
        foreach (var player in players)
        {
            player.Deactivate();
        }

        SwitchPlayer();
        _playerHasWon += DefineWinnerByImpossibleMovement;
        buttonRestart.onClick.AddListener(RestartGame);
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

        if (!selectedCell.IsEmpty && selectedCell.PlacedChecker.GameColor != playerColor) return;
        if (!selectedCell.IsEmpty)
        {
            selectedCell.RisePlacedObject();
            if (_previousSelectedCell != null && _previousSelectedCell.HasRisenPlacedObject)
            {
                _previousSelectedCell.MovePlacedObjectToGround();
            }

            _previousSelectedCell = selectedCell;
            return;
        }

        if (_previousSelectedCell != null && _previousSelectedCell.HasRisenPlacedObject && selectedCell.IsEmpty)
        {
            if(_mustMoveThisChecker!=null&&_mustMoveThisChecker!=_previousSelectedCell) return; 
            
            MoveChecker(playerColor, _previousSelectedCell, selectedCell);
        }
    }

    private void MoveChecker(GameColor color, GameBoardCell previousSelectedCell,
        GameBoardCell selectedCell)
    {
        //todo: Split CheckBordersThenMove 2 methods
        CheckBordersThenMove(color==GameColor.White?1:-1, previousSelectedCell, selectedCell);
    }

    private void CheckBordersThenMove(int direction, GameBoardCell previousSelectedCell, GameBoardCell selectedCell)
    {
        var isPlayerMovedToNeighbourCell = selectedCell.Position.x - previousSelectedCell.Position.x == direction &&
                                           Mathf.Abs(selectedCell.Position.y - previousSelectedCell.Position.y) == 1;
        if (isPlayerMovedToNeighbourCell)
        {
            MoveCheckerToCell(previousSelectedCell, selectedCell);
            _previousSelectedCell = null;
            SwitchPlayer();
          // CheckIfPlayerCanBeatEnemyChecker();         
        }
        else
        {
            TryToBeat(previousSelectedCell, selectedCell);
        }

        var allCellsAreOccupied = false;
        if (allCellsAreOccupied)
        {
            _playerHasWon.Invoke(_currentPlayer.GameColor);
        }
    }

    private void TryToBeat(GameBoardCell previousSelectedCell, GameBoardCell selectedCell)
    {
        var prevPosition = previousSelectedCell.Position;
        var destPosition = selectedCell.Position;
        var isAttemptToJumpOverCell = Mathf.Abs(destPosition.x - prevPosition.x) == 2 &&
                                      Mathf.Abs(destPosition.y - prevPosition.y) == 2;
        if (!isAttemptToJumpOverCell) return;
        
        var jumpThroughCellPos = (prevPosition + destPosition) / 2;
        var jumpThroughCell = checkerBoard.Cells[jumpThroughCellPos.x, jumpThroughCellPos.y];
        var isJumpingOverEnemy =
            !jumpThroughCell.IsEmpty && jumpThroughCell.PlacedChecker.GameColor != _currentPlayer.GameColor;
        if (!isJumpingOverEnemy) return;
        
        MoveCheckerToCell(previousSelectedCell, selectedCell);
        //todo: Refactor: remove duplication  dictionary
        if (_currentPlayer.GameColor == GameColor.Red)
        {
            _scoreOfRedPlayerBeatenCheckers++;
            PlayerBeatEnemyChecker?.Invoke(_scoreOfRedPlayerBeatenCheckers,_currentPlayer.GameColor);
        }
        else if(_currentPlayer.GameColor==GameColor.White)
        {
            _scoreOfWhitePlayerBeatenCheckers++;
            PlayerBeatEnemyChecker?.Invoke(_scoreOfWhitePlayerBeatenCheckers,_currentPlayer.GameColor);
        }
        jumpThroughCell.PlacedChecker.gameObject.SetActive(false);
        jumpThroughCell.ForgetPlacedChecker();
        CheckIfUserCanBeatEnemy(selectedCell);
    }
    
    private void CheckPossibilityOfMovement()
    {
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.PlacedChecker.GameColor == _currentPlayer.GameColor)
            {
                
            }
            
        }
    }

    private void CheckIfPlayerCanBeatEnemyChecker()
    {
        foreach (var cell in checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != _currentPlayer.GameColor) continue;

            CheckIfUserCanBeatEnemy(cell);
        }
    }

    private void CheckIfUserCanBeatEnemy(GameBoardCell cell)
    {
        var availableCells = GetAvailableJumpDestinationCells(cell);
        GetEnemyCheckerByPositionAndCheck(availableCells, cell);
    }

    private void GetEnemyCheckerByPositionAndCheck(List<GameBoardCell> listOfCells, GameBoardCell currentCell)
    {
        var allPossibleEnemyCheckersAreEmpty = true;
        for (var i = 0; i < listOfCells.Count; i++)
        {
            var cellWherePlayerCanMoveChecker = listOfCells[i];
            var assumableEnemyCheckerPos = (cellWherePlayerCanMoveChecker.Position + currentCell.Position) / 2;
            var assumableEnemyCell = checkerBoard.Cells[assumableEnemyCheckerPos.x, assumableEnemyCheckerPos.y];

            if (!assumableEnemyCell.IsEmpty)
            {
                allPossibleEnemyCheckersAreEmpty = false;
                break;
                allPossibleEnemyCheckersAreEmpty = true;
            }
        }
        
        if (allPossibleEnemyCheckersAreEmpty)
        {
            _mustMoveThisChecker = null;
            SwitchPlayer();
        }

        else
        {
            _previousSelectedCell = currentCell;
            _mustMoveThisChecker = currentCell;
        }
    }

    private List<GameBoardCell> GetAvailableJumpDestinationCells(GameBoardCell cell)
    {
        var potentialPositions = new[]
        {
            cell.Position+new Vector2Int(2,2),
            cell.Position+new Vector2Int(2,-2),
            cell.Position+new Vector2Int(-2,2),
            cell.Position+new Vector2Int(-2,-2),
        };
      return potentialPositions
          .Where(IsCellsWithinBounds)
          .Select(pos=>checkerBoard.Cells[pos.x,pos.y])
          .Where(cell=>cell.IsEmpty).ToList();
    }

    private bool IsCellsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < checkerBoard.Cells.GetLength(0) &&
               pos.y >= 0 && pos.y < checkerBoard.Cells.GetLength(1);
    }
    
    private void MoveCheckerToCell(GameBoardCell initialCell, GameBoardCell destinationCell)
    {
        if (initialCell == null || destinationCell == null) throw new ArgumentNullException();

        var checkerTransform = initialCell.PlacedChecker.transform;
        var endPosition = destinationCell.anchor.position + (0.5f * checkerTransform.lossyScale.y * Vector3.up);
        initialCell.PlacedChecker.transform
            .DOMove(endPosition, 0.5f)
            .OnComplete(() =>
            {
                destinationCell.Place(initialCell.PlacedChecker);
                initialCell.ForgetPlacedChecker();
            });
    }

    private void DefineWinnerByScore(int whiteScore,int redScore)
    {
        var allEnemyCheckersWereBeaten = 12;
        if (whiteScore == allEnemyCheckersWereBeaten)
        {
            definedWinner.text = "Red player won";
            ActivateButtonRestart();
        }
        else if(redScore==allEnemyCheckersWereBeaten)
        {
            definedWinner.text = "White player won";
            ActivateButtonRestart();
        }
    }

    private void DefineWinnerByImpossibleMovement(GameColor color)
    {
        if (_currentPlayer.GameColor == color)
        {
            definedWinner.text = "Red player has won";
        }
        else if (_currentPlayer.GameColor != color)
        {
            definedWinner.text = "White player has won";
        }
    }

    private void ActivateButtonRestart()
    {
        buttonRestart.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private (int, int) FindLocationOfArray(GameBoardCell cell)
    {
        var cellRow = 0;
        var cellColumn = 0;
        for (var row = 0; row < checkerBoard._rows; row++)
        {
            for (var column = 0; column < checkerBoard._colums; column++)
            {
                if (checkerBoard.Cells[row, column] != cell) continue;

                cellRow = row;
                cellColumn = column;
                break;
            }
        }

        return (cellRow, cellColumn);
    }
}