using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class BotControl : PlayerControlBase
{
    private CheckerBoard _checkerBoard; 
    private GameBoardHelper _gameBoardHelper;
    private RulesManager _rulesManager;
    public override PlayerType PlayerType => PlayerType.Bot;

    public void Init(GameColor color,CheckerBoard checkerBoard, GameBoardHelper gameBoardHelper, RulesManager rulesManager)
    {
        _checkerBoard = checkerBoard;
        _gameBoardHelper = gameBoardHelper;
        _rulesManager = rulesManager;
        GameColor = color;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void SelectCell(GameBoardCell startCell = null)
    {
        base.SelectCell(startCell);
        RandomlySelectMove();
    }

    private void RandomlySelectMove()
    {
        var cellThatCanBeat = GetStartCellThatShouldBeatEnemy();
        if (cellThatCanBeat != null)
        {
            var destCell = GetJumpDestCell(cellThatCanBeat);
            ExecuteMove(cellThatCanBeat, destCell);
        }
        else
        {
            var startCell = GetRandomAvailableCellInList(GetPossibleCellsWithCheckers(true));
            var listOfPossibleDestCells = _gameBoardHelper.GetAvailableCells(startCell, GameColor);
            var destCell = GetRandomAvailableCellInList(listOfPossibleDestCells);
            if (destCell == null) return;

            ExecuteMove(startCell,destCell);
        }
    }

    private GameBoardCell GetStartCellThatShouldBeatEnemy()
    {
        if (StartCell != null)
        {
            return StartCell;
        }
        
        var cellsThatCanBeat = GetCellsThatCanBeatEnemy();
        if (!cellsThatCanBeat.Any()) return null;
        
        var cellWithCheckerToBeat = Random.Range(0, cellsThatCanBeat.Count - 1);
        return cellsThatCanBeat[cellWithCheckerToBeat];
    }

    private void ExecuteMove(GameBoardCell startCell, GameBoardCell destCell)
    {
        OnCellSelected(startCell, GameColor);
        StartCoroutine(DelayMove(destCell));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator DelayMove(GameBoardCell destCell)
    {
        yield return new WaitForSeconds(0.5f);
        OnCellSelected(destCell, GameColor);
    }

    private List<GameBoardCell> GetPossibleCellsWithCheckers(bool searchCellsForSelecting)
    {
        List<GameBoardCell> suitableCheckers = new();
        foreach (var cell in _checkerBoard.Cells)
        {
            if (cell.IsEmpty || cell.PlacedChecker.GameColor != GameColor) continue;

            if (searchCellsForSelecting)
            {
                AddSuitableCellsToList(cell, suitableCheckers);
            }
            else
            {
                suitableCheckers.Add(cell);
            }
        }

        return suitableCheckers;
    }

    private void AddSuitableCellsToList(GameBoardCell cell, ICollection<GameBoardCell> suitableCheckers)
    {
        var availableDestinationCells = _gameBoardHelper.GetAvailableCells(cell, GameColor);
        if (!availableDestinationCells.Any()) return;

        suitableCheckers.Add(cell);
    }

    private GameBoardCell GetRandomAvailableCellInList(IReadOnlyList<GameBoardCell> availableCells)
    {
        var randomCellIndex = Random.Range(0, availableCells.Count - 1);
        return availableCells[randomCellIndex];
    }

    private List<GameBoardCell> GetCellsThatCanBeatEnemy()
    {
        var cells = GetPossibleCellsWithCheckers(false);
        return cells.Where(cell => _rulesManager.CanUserBeatEnemy(cell)).ToList();
    }

    private GameBoardCell GetJumpDestCell(GameBoardCell cellThatCanBeat)
    {
        GameBoardCell jumpDestCell = null;
        var availablePositions = _gameBoardHelper.GetPotentialPositions(cellThatCanBeat, 2);
        var sortedPositions = availablePositions.Where(position => _rulesManager.IsCellsWithinBound(position)).ToArray();
        foreach (var position in sortedPositions)
        {
            var assumableEnemyPosition =
                _gameBoardHelper.GetCellBetweenStartAndDestCells(cellThatCanBeat.Position, position);
            if (assumableEnemyPosition.IsEmpty || assumableEnemyPosition.PlacedChecker.GameColor == GameColor) continue;
            
            var destCell = _checkerBoard.Cells[position.x, position.y];
            if (destCell.IsEmpty)
            {
                jumpDestCell = destCell;
            }
        }

        return jumpDestCell;
    }
}