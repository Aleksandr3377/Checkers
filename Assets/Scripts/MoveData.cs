using System;
using System.Collections.Generic;

public class MoveData
{
     private List<GameBoardCell> _startCells = new();
     public List<GameBoardCell> StartCells
     {
          get => _startCells;
          set
          {
               if (value == StartCells)
               {
                    return;
               }
               
               if (StartCellLocked)
               {
                    throw new InvalidOperationException("Start cell is locked from modification");
                    
               } 
               _startCells = value;
          }
     }
     public GameBoardCell DestCell;
     public bool StartCellLocked { get; set; }

     public void SetCells(params GameBoardCell[] gameBoardCell)
     {
         _startCells.Clear();
         _startCells.AddRange(gameBoardCell);
     }
}