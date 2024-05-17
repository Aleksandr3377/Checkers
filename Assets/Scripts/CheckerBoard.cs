using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    public GameBoardCell[,] Cells { get; private set; }
    public int Rows => 8;
    public int Colums => 8;
    private readonly Color _gameBoardCellBlack = Color.black;
    private readonly Color _gameBoardCellWhite = Color.white;
    public int StartCheckersCount = 12;
    [SerializeField] private Checker _checkerPrefab;
    [SerializeField] private GameObject _checkersParent;
    [SerializeField] private GameBoardCell _gameBoardCellPrefab;

    private void Awake()
    {
        InstantiateCheckerBoard();
        InstantiateCheckers();
    }

    private void InstantiateCheckerBoard()
    {
        Cells = new GameBoardCell[Rows, Colums];
        for (var row = 0; row < Rows; row++)
        {
            for (var colum = 0; colum < Colums; colum++)
            {
                var currentColor = (row + colum) % 2 == 0 ? _gameBoardCellBlack : _gameBoardCellWhite;
                var cell = Instantiate(_gameBoardCellPrefab, new Vector3(row, 0, colum), Quaternion.identity,
                    transform);
                _gameBoardCellPrefab.ChangeColor(cell.gameObject, currentColor);
                cell.gameObject.SetActive(true);
                cell.InitCell(new Vector2Int(row, colum));
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
                switch (row)
                {
                    case < 3:
                        SpawnAndInitColorForChecker(currentGameBoardCell, GameColor.White);
                        break;
                    case > 4:
                        SpawnAndInitColorForChecker(currentGameBoardCell, GameColor.Red);
                        break;
                }
            }
        }
    }

    private void SpawnAndInitColorForChecker(GameBoardCell currentGameBoardCell, GameColor color)
    {
        var checker = Instantiate(_checkerPrefab, _checkersParent.transform);
        checker.Init(color);
        currentGameBoardCell.Place(checker);
    }

    public Vector3 CalculateCenterOfDesk()
    {
        var firstCell = Cells[0, 0];
        var fourthCell = Cells[7, 7];
        var centerOfGameBoard = (firstCell.transform.position + fourthCell.transform.position) / 2f;
        return centerOfGameBoard;
    }
}