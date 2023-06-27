using Modding;
using Modding.PublicInterfaces.Cells;
using System.Collections.Generic;
using UnityEngine;

namespace Indev2
{
    public class PresentCellProcessor : CellProcessor
    {
        public override string Name => "Present Cell";
        public override int CellType => 22;
        public override string CellSpriteIndex => "Present";

        public PresentCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            _cellGrid.RemoveCell(basicCell);
            _cellGrid.RemoveCell(replacingCell);
            _cellGrid.AddCell(replacingCell.Transform.Position, replacingCell.Transform.Direction, Random.Range(0, 10));
            return false;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force == -1)
            {
                if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
                    return false;
                return true;
            }
            
            if (force <= 0)
                return false;
            return true;

        }

        public override void OnCellInit(ref BasicCell cell)
        {
            //do nothing
        }

        public override void Clear()
        {
            //do nothing
        }
    }
}