using System;
using System.Collections.Generic;
using System.Linq;

public class MoveData
{
     private GameBoardCell _startCell;
     public readonly List<GameBoardCell> RestrictToMoveCell = new();
     public GameBoardCell StartCell
     {
          get => _startCell;
          set
          {
               if (value == StartCell)
               {
                    return;
               }
               if (StartCellLocked && !RestrictToMoveCell.Contains(value))
               {
                    throw new InvalidOperationException("Start cell is locked from modification");
               } 
               _startCell = value;
          }
     }
     public GameBoardCell DestCell;

     public bool StartCellLocked => RestrictToMoveCell.Any();
}