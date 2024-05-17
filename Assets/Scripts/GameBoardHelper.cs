using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoardHelper : MonoBehaviour
{
    [SerializeField] private CheckerBoard _checkerBoard;
    [SerializeField] private RulesManager _rulesManager;

    public GameBoardCell GetCellBetweenStartAndDestCells(Vector2Int prevPosition, Vector2Int destPosition)
    {
        var cellBetween = (prevPosition + destPosition) / 2;
        return _checkerBoard.Cells[cellBetween.x, cellBetween.y];
    }

    public IEnumerable<GameBoardCell> GetAvailableJumpDestinationCells(GameBoardCell cell)
    {
        var potentialPositions = GetPotentialPositions(cell, 2);
        var boardCells = GetBoardCells(potentialPositions);
        return boardCells.Where(x => x.IsEmpty).ToList();
    }

    public Vector2Int[] GetPotentialPositions(GameBoardCell cell, int distance)
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

    public List<GameBoardCell> GetAvailableCells(GameBoardCell cell, GameColor color)
    {
        var potentialPositions = GetPotentialPositionsToMove(cell, color);
        var boardCells = GetBoardCells(potentialPositions);
        return boardCells.Where(x => x.IsEmpty).ToList();
    }

    public Vector2Int[] GetPotentialPositionsToMove(GameBoardCell cell, GameColor color)
    {
        if (color == GameColor.White)
        {
            var potentialPositions = new[]
            {
                cell.Position + new Vector2Int(1, 1),
                cell.Position + new Vector2Int(1, -1),
            };
            return potentialPositions;
        }
        else
        {
            var potentialPositions = new[]
            {
                cell.Position + new Vector2Int(-1, 1),
                cell.Position + new Vector2Int(-1, -1),
            };
            return potentialPositions;
        }
    }

    public IEnumerable<GameBoardCell> GetBoardCells([NotNull] Vector2Int[] positions)
    {
        if (positions == null) throw new ArgumentNullException(nameof(positions));

        return positions
            .Where(pos => _rulesManager.IsCellsWithinBound(pos))
            .Select(pos => _checkerBoard.Cells[pos.x, pos.y]).ToList();
    }

    public Vector3 GetEndPosition(GameBoardCell initialCell, GameBoardCell destinationCell)
    {
        var checkerTransform = initialCell.PlacedChecker.transform;
        var endPosition = destinationCell.Anchor.position + 0.5f * checkerTransform.lossyScale.y * Vector3.up;
        return endPosition;
    }

    public int GetPlayerDirection(GameColor color)
    {
        var playerDirection = color == GameColor.White ? 1 : -1;
        return playerDirection;
    }

    public void AddOutlineOntoCheckers(params GameBoardCell[] cellsWithCheckers)
    {
        foreach (var cell in cellsWithCheckers)
        {
            cell.PlacedChecker.AddComponent<Outline>();
            var outline = cell.PlacedChecker.GetComponent<Outline>();
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 10f;
        }
    }

    public void RemoveOutline(params GameBoardCell[] cellsWithCheckers)
    {
        foreach (var cell in cellsWithCheckers)
        {
            var outline = cell.PlacedChecker.GetComponent<Outline>();
            Destroy(outline);
        }
    }
}