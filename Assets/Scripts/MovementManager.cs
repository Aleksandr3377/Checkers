using System;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MovementManager: MonoBehaviour
{
   [SerializeField] private GameBoardHelper _gameBoardHelper;
   [SerializeField] private RulesManager _rulesManager;
    
    public void MoveCheckerToCell(GameBoardCell initialCell, GameBoardCell destinationCell)
    {
        if (initialCell == null || destinationCell == null) throw new ArgumentNullException();

        var endPosition = _gameBoardHelper.GetEndPosition(initialCell, destinationCell);
        MoveChecker(initialCell, destinationCell, endPosition);
        _rulesManager.CheckPossibilityOfMovements();
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
}