using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    public GameBoardCell[,] Cells { get; private set; }
    public Checker checkerPrefab;
    public GameObject checkersParent;
    public GameBoardCell gameBoardCellPrefab;
    public int Rows { get; } = 8;
    public int Colums { get; } = 8;
    private readonly Color _gameBoardCellBlack = Color.black;
    private readonly Color _gameBoardCellWhite = Color.white; 
    public int StartCheckersCount = 12;

    private void Awake()
    {
        InstantiateCheckBoard();
        InstantiateCheckers();
    }

    private void InstantiateCheckBoard()
    {
        Cells = new GameBoardCell[Rows, Colums];
        for (var row = 0; row < Rows; row++)
        {
            for (var colum = 0; colum < Colums; colum++)
            {
                var currentColor = (row + colum) % 2 == 0 ? _gameBoardCellBlack : _gameBoardCellWhite;
                var cell = Instantiate(gameBoardCellPrefab, new Vector3(row, 0, colum), Quaternion.identity, transform);
                gameBoardCellPrefab.ChangeColor(cell.gameObject, currentColor);
                cell.gameObject.SetActive(true);
                cell.Init(new Vector2Int(row,colum));
                Cells[row, colum] = cell;
            }
        }
    }

    private void InstantiateCheckers()
    {
        for (var row = 0; row < Cells.GetLength(0); row++)
        {
            for (var colum = row % 2; colum < Cells.GetLength(1); colum += 2)
            {
                var currentGameBoardCell = Cells[row, colum];
                if (row < 3)
                {
                    SpawnAndInitColorForChecker(currentGameBoardCell, GameColor.White);
                }
                else if (row > 4)
                {
                    SpawnAndInitColorForChecker(currentGameBoardCell, GameColor.Red);
                }
            }
        }
    }

    private void SpawnAndInitColorForChecker(GameBoardCell currentGameBoardCell, GameColor color)
    {
        var checker = Instantiate(checkerPrefab,checkersParent.transform);
        checker.Init(color);
        currentGameBoardCell.Place(checker);
    }
}
