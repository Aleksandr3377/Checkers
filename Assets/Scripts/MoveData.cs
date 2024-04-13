using System;
using System.Collections.Generic;

public class MoveData
{
     private GameBoardCell _startCell;
     public GameBoardCell StartCell
     {
          get => _startCell;
          set
          {
               if (value == StartCell)
               {
                    return;
               }
               if (StartCellLocked)
               {
                    throw new InvalidOperationException("Start cell is locked from modification");
               } 
               _startCell = value;
          }
     }
     public GameBoardCell DestCell;
     public bool StartCellLocked { get; set; }
     public List<GameBoardCell> QueenCells;
}