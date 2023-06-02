using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;
using UnityEngine;

namespace Indev2
{
    public class FallCellProcessor : SteppedCellProcessor
    {
        public FallCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Fall";
        public override int CellType => 13;
        public override string CellSpriteIndex => "Fall";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return true;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                if (ct.IsCancellationRequested)
                    return;

                // Save the previous transform to be used when the cell moves
                var prevTransform = cell.Transform;

                // Check the cell in front of the fall cell (use the fall cells direction)
                var targetPos = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;
                while (_cellGrid.InBounds(targetPos) && _cellGrid.GetCell(targetPos) == null)
                {
                    // Update the target position for the next iteration
                    targetPos += cell.Transform.Direction.AsVector2Int;
                }

                // Store the position where the fall cell stops
                var finalPos = targetPos - cell.Transform.Direction.AsVector2Int;

                // Move the cell to the final position
                _cellGrid.RemoveCell(cell.Transform.Position);
                _cellGrid.AddCell(finalPos, cell.Transform.Direction, CellType, prevTransform);
            }
        }
        public override void Clear()
        {
        }
    }
}
