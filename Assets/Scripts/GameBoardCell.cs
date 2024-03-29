using DG.Tweening;
using UnityEngine;

public class GameBoardCell : MonoBehaviour
{
    public Transform anchor;
    public Checker PlacedChecker { get; private set; }
    public bool HasRisenPlacedObject { get; private set; }
    public bool IsEmpty => PlacedChecker == null;
    public Vector2Int Position { get; private set; }

    public void Init(Vector2Int position)
    {
        Position = position;
        gameObject.name += $"X:{position.x},Y:{position.y}";
    }
    
    public void Place(Checker checker)
    {
        checker.transform.position = anchor.position + Vector3.up * checker.transform.lossyScale.y * 0.5f;
        PlacedChecker = checker;
    }

    public void ForgetPlacedChecker()
    {
        PlacedChecker = null;
        HasRisenPlacedObject = false;
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
        DoTweenMovement(position);
        HasRisenPlacedObject = false;
    }
    
    public void TakeChecker(GameBoardCell selectedCell)
    {
        var position = selectedCell.transform.position;
        DoTweenMovement(position);
        gameObject.SetActive(false);
    }
    
    public void ChangeColor(GameObject prefab, Color color)
    {
        var rend = prefab.GetComponent<Renderer>();
        rend.material.color = color;
    }

    private void DoTweenMovement(Vector3 position)
    {
        PlacedChecker.transform.DOMove(new Vector3(position.x, anchor.transform.position.y, position.z), 0.5f);
    }
}