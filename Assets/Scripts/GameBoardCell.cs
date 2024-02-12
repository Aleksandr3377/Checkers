using DG.Tweening;
using UnityEngine;

public class GameBoardCell : MonoBehaviour
{
    public Transform anchor;
    private Checker PlacedChecker { get; set; }
    public bool HasRisenPlacedObject { get; private set; }

    public void Place(Checker checker)
    {
        checker.transform.position = anchor.position + Vector3.up * checker.transform.lossyScale.y * 0.5f;
        PlacedChecker = checker;
    }

    public void RisePlacedObject()
    {
        if (PlacedChecker == null) return;
        
        var position = PlacedChecker.transform.position;
        PlacedChecker.transform.DOMove(new Vector3(position.x, anchor.transform.position.y + 0.5f, position.z), 0.5f);
        HasRisenPlacedObject = true;
    }
    
    public void MovePlacedObjectToGround()
    {
        if (PlacedChecker == null) return;
        
        var position = PlacedChecker.transform.position;
        PlacedChecker.transform.DOMove( new Vector3(position.x, anchor.transform.position.y, position.z),0.5f);
        HasRisenPlacedObject = false;
    }
    
    public void MoveCheckerToCell(GameBoardCell selectedCell)
    {
        var position = selectedCell.transform.position;
        PlacedChecker.transform.DOMove(new Vector3(position.x, anchor.transform.position.y, position.z), 0.5f);
    }
    
    public void TakeChecker(GameBoardCell selectedCell)
    {
        var position = selectedCell.transform.position;
        PlacedChecker.transform.DOMove(new Vector3(position.x, anchor.transform.position.y, position.z), 0.5f);
        gameObject.SetActive(false);
    }
    
    public void ChangeColor(GameObject prefab, Color color)
    {
        var rend = prefab.GetComponent<Renderer>();
        rend.material.color = color;
    }
}