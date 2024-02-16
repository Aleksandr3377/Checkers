using UnityEngine;

public class PlayerControl : PlayerControlBase
{
    private Transform _selectedWhiteChecker;
    private Transform _selectedRedChecker;
    private Transform _selectedCell;
    [SerializeField] private GameColor gameColor;
 
    private void Update()
    {
        TryToSelectChecker();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void TryToSelectChecker()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cell = GetCell();
        if (cell != null)
        {
            OnCellSelected(cell.GetComponent<GameBoardCell>(), gameColor);
        }
    }

    private GameObject GetSelectedMouseObject(string tagOfObject)
    {
        GameObject selectedObject = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.CompareTag(tagOfObject))
            {
                selectedObject =hit.collider.gameObject;
            }
        }

        return selectedObject;
    }
    
    private GameObject GetCell()
    {
        var cell = GetSelectedMouseObject("Cell");
        return cell;
    }
}