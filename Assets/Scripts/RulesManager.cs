using UnityEngine;

public class RulesManager : MonoBehaviour
{
   public GameManager gameManager;
   
   public bool CheckIfPlayerCanMoveChecker(GameObject checker,GameObject selectedCell)
   {
      var checkerPosition = checker.transform.position;
      var selectedCellPosition = selectedCell.transform.position;
      var direction = (checker.CompareTag("WhiteChecker")) ? 1 : -1;
      if (Mathf.Abs(selectedCellPosition.x - checkerPosition.x) == 1.0f && selectedCellPosition.y == checkerPosition.y + direction)
      {
         return true;
      }

      return false;
   }
}
