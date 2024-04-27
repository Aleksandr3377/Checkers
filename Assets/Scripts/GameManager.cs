using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RulesManager _rulesManager;
    [SerializeField] private GameBoardHelper _gameBoardHelper; 
    [SerializeField] private MovementManager _movementManager;
    [SerializeField] private PlayerControlBase[] _players;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] private Button _buttonRestart; 
    [SerializeField] public TextMeshProUGUI DefinedWinner;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private float _timeScale = 1;
    [SerializeField] private SoundControl _soundControl;
    public PlayerControlBase CurrentPlayer { get; private set; }
    public MoveData MoveData;
    public GameBoardCell CurrentlySelectedCell => _checkerBoard.Cells.Cast<GameBoardCell>()
        .FirstOrDefault(x => x.HasRisenPlacedObject && x.PlacedChecker.GameColor == CurrentPlayer.GameColor);

    private void Awake()
    { 
        _players = _playerSpawner.SpawnPlayers(GameGlobalData.SpawnPlayerData).ToArray();
    }

    private void Start()
    {
        Time.timeScale = _timeScale;
        DeactivateAllPlayers();
        SwitchPlayer();
        _buttonRestart.gameObject.SetActive(false);
    }
    
    private void SwitchPlayer()
    {
        if (CurrentPlayer == null)
        {
            SetInitialPlayer();
        }
        else
        {
            DeactivateCurrentPlayer();
            SetNextPlayer();
        }

        ActivateCurrentPlayer();
        _rulesManager.CheckPossibilityOfMovements();
    }

    private void MakeMove(MoveData moveData)
    {
        var playerDirection = _gameBoardHelper.GetPlayerDirection(CurrentPlayer.GameColor);
        var startCell = moveData.StartCell;
        var destCell = moveData.DestCell;
        if (_rulesManager.IsCheckerCanBeMoved(playerDirection, startCell, destCell))
        {
            _movementManager.MoveCheckerToCell(startCell, destCell,()=>
            {
                _soundControl.PlaySound(SoundEffectType.Move);
                _rulesManager.CheckIfPlayerHasBeatenAllCheckers();
                SwitchPlayer();
                _rulesManager.CheckIfPlayerMustBeatEnemyChecker();
              //  _rulesManager.CheckIfCheckerTransformedToQueen();
            });
        }
        else
        {
            TryToBeatEnemyChecker(moveData);
            _rulesManager.CheckIfPlayerHasBeatenAllCheckers();
           // _rulesManager.CheckIfCheckerTransformedToQueen();
        }
    }

    private void TryToBeatEnemyChecker(MoveData moveData)
    {
        if (!_rulesManager.CanUserBeatEnemy(moveData.StartCell, moveData.DestCell)) return;
        
        _movementManager.MoveCheckerToCell(moveData.StartCell, moveData.DestCell, OnMoveFinished);
        _soundControl.PlaySound(SoundEffectType.Beat);
        var enemyPosition =
            _gameBoardHelper.GetCellBetweenStartAndDestCells(moveData.StartCell.Position, moveData.DestCell.Position);
        RemoveChecker(enemyPosition);
        _scoreManager.AddScore(CurrentPlayer.GameColor,1);
        var isPlayerShouldBeatAnotherChecker = _rulesManager.CanUserBeatEnemy(moveData.DestCell);
        if (isPlayerShouldBeatAnotherChecker)
        {
            moveData.StartCellLocked = false;
            moveData.StartCell = moveData.DestCell;
            moveData.StartCellLocked = true;
        }
        
        void OnMoveFinished()
        {
            if (_rulesManager.CanUserBeatEnemy(moveData.DestCell))
            {
                CurrentPlayer.SelectCell(moveData.StartCell);
            }
            else
            {
                moveData.StartCellLocked = false;
                SwitchPlayer();
                _rulesManager.CheckIfPlayerMustBeatEnemyChecker();
            }
        }
    }
    
    public void ActivateButtonRestart()
    {
        _buttonRestart.gameObject.SetActive(true);
        _buttonRestart.onClick.AddListener(RestartGame);
    }
    
    private static void RestartGame()
    {
        SceneManager.LoadScene("Menu");
    }

    private void RemoveChecker(GameBoardCell cell)
    {
        cell.PlacedChecker.gameObject.SetActive(false);
        cell.ForgetPlacedChecker();
    }

    private void TryToMove(GameBoardCell cell, GameColor playerColor)
    {
        if (cell.HasRisenPlacedObject)
        {
            cell.MovePlacedObjectToGround();
            _soundControl.PlaySound(SoundEffectType.Rise);
            if (!MoveData.StartCellLocked)
            {
                MoveData.StartCell = null;
            }
            return;
        }

        if (!cell.IsEmpty && cell.PlacedChecker.GameColor != CurrentPlayer.GameColor) return;
        
        if (!cell.IsEmpty && (!MoveData.StartCellLocked || MoveData.StartCell == cell))
        {
            if (MoveData.StartCell != null)
            {
                MoveData.StartCell.MovePlacedObjectToGround();
                _soundControl.PlaySound(SoundEffectType.Rise);
            }

            cell.RisePlacedObject();
            _soundControl.PlaySound(SoundEffectType.Rise);
            
            MoveData.StartCell = cell;
        }

        if (!_rulesManager.IsCheckerCanBeMovedToNeighbourCell(cell)) return;

        if (!MoveData.StartCellLocked)
        {
            MoveData.DestCell = cell;
            MakeMove(MoveData);
        }

        if (!MoveData.StartCellLocked || MoveData.StartCell != CurrentlySelectedCell) return;
        
        MoveData.DestCell = cell;
        TryToBeatEnemyChecker(MoveData);
    }
    
    private void SetInitialPlayer()
    {
        CurrentPlayer = _players[0];
    }

    private void DeactivateCurrentPlayer()
    {
        CurrentPlayer.CellWasSelected -= TryToMove;
      //  CurrentPlayer.Deactivate();
    }

    private void ActivateCurrentPlayer()
    {
        MoveData = new MoveData();
        CurrentPlayer.CellWasSelected += TryToMove;
        CurrentPlayer.SelectCell();
    }

    private void SetNextPlayer()
    {
        var nextPlayerIndex = GetIndexOfNextPlayer();
        CurrentPlayer = _players[nextPlayerIndex];
    }

    private void DeactivateAllPlayers()
    {
        foreach (var player in _players)
        {
            player.Deactivate();
        }
    }

    private int GetIndexOfNextPlayer()
    {
        var currentIndex = Array.IndexOf(_players, CurrentPlayer);
        int nextPlayerIndex;
        if (currentIndex == _players.Length - 1)
            nextPlayerIndex = 0;
        else
            nextPlayerIndex = currentIndex + 1;

        return nextPlayerIndex;
    }
}