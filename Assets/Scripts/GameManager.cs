using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerControlBase[] players;
    public bool allowanceForSelectingChecker = true;
    private PlayerControlBase _currentPlayer;

    private void Start()
    {
        foreach (var player in players)
        {
            player.Deactivate();
        }
        ListenToMoveCheckerEvent();
        SwitchPlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchPlayer();
        }
    }

    public GameObject GetSelectedMouseObject(string tagOfObject)
    {
        GameObject selectedObject = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.CompareTag(tagOfObject))
            {
                selectedObject = GetObjectTransform(hit.collider.gameObject);
            }
        }

        return selectedObject;
    }

    private void SwitchPlayer()
    {
        if (_currentPlayer == null)
        {
            _currentPlayer = players[0];
        }
        else
        {
            _currentPlayer.CellWasSelected -= OnCellSelected;
            _currentPlayer.Deactivate();
            var currentIndex = Array.IndexOf(players, _currentPlayer);
            int nextPlayerIndex;
            if (currentIndex == players.Length - 1)
            {
                nextPlayerIndex = 0;
            }
            else
            {
                nextPlayerIndex = currentIndex + 1;
            }

            _currentPlayer = players[nextPlayerIndex];
        }

        _currentPlayer.CellWasSelected += OnCellSelected;
        _currentPlayer.Activate();
    }
    
    private void OnCellSelected(GameBoardCell cell,GameColor color)
    {
        if (cell.HasRisenPlacedObject)
        {
            cell.MovePlacedObjectToGround();
        }
        else
        {
            cell.RisePlacedObject();
        }
    }
    

    private void ListenToMoveCheckerEvent()
    {
      //  players.MoveChecker += MoveCell;
    }

    private void MoveCell(GameBoardCell cell)
    {
        cell.MoveCheckerToCell(cell);
    }
    
    private void ListenToBeatCheckerEvent()
    {
     //   players.MoveChecker += Beat;
    }

    private void Beat(GameBoardCell cell)
    {
        cell.TakeChecker(cell);
    }


    private GameObject GetObjectTransform(GameObject obj)
    {
        var selectObject = obj.transform.gameObject;
        return selectObject;
    }
    
}