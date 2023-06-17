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

                var targetPos = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;
                var count = 0;
                while (_cellGrid.InBounds(targetPos) && _cellGrid.GetCell(targetPos) == null)
                {
                    targetPos = cell.Transform.Position + (cell.Transform.Direction.AsVector2Int * count);
                    count++;
                }
                var targetCell = _cellGrid.GetCell(targetPos);
                if (targetCell != null)
                    continue;
                _cellGrid.AddCell(targetPos, cell.Transform.Direction, cell.Instance.Type, cell.Transform);
                _cellGrid.RemoveCell(cell);
            }
        }
        public override void Clear()
        {
        }
    }
}
