// using UnityEngine;
//
// public class CheckerSpawner : MonoBehaviour
// {
//     public GameBoardCell gameBoardCellPrefab;
//     public GameObject whiteCheckerPrefab;
//     public GameObject redCheckerPrefab;
//     public CheckerBoard checkerBoard;
//    // public GameObject anchor;
//
//     private void Start()
//     {
//        // InstantiateCheckers();
//     }
//
//     private void InstantiateCheckers()
//     {
//         var anchor = gameBoardCellPrefab.anchor;
//         for (var i = 0; i < checkerBoard.Cells.GetLength(0); i++)
//         {
//             for (var j = 0; j < checkerBoard.Cells.GetLength(1); j++)
//             {
//                 var curentCell = checkerBoard.Cells[i, j];
//                 if (curentCell.name.Contains("Black"))
//                 {
//                     if (j < 3)
//                     {
//                         Instantiate(whiteCheckerPrefab, new Vector3(i, anchor.transform.position.y, j), Quaternion.identity,checkerBoard.transform);
//                     }
//                     else if (j > 4)
//                     {
//                         Instantiate(redCheckerPrefab, new Vector3(i, anchor.transform.position.y, j), Quaternion.identity,checkerBoard.transform);
//                     }
//                 }
//             }
//         }
//     }
// }
