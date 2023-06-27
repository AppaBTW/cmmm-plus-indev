using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class PullerCellProcessor : SteppedCellProcessor
    {
        public PullerCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override string Name => "Puller";
        public override int CellType => 18;
        public override string CellSpriteIndex => "Puller";

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force == -1)
            {
                if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
                    return false;
                return true;
            }
            var target = cell.Transform.Position + direction.AsVector2Int;

            if (!cell.Frozen)
                if (direction == cell.Transform.Direction)
                    force++;
                else if (direction.Axis == cell.Transform.Direction.Axis)
                    force--;
            if (direction != cell.Transform.Direction)
            {
                if (force <= 0)
                    return false;
            }

            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);

            if (targetCell is null)
            {
                _cellGrid.MoveCell(cell, target);
                return true;
            }

            if (!_cellGrid.PushCell(targetCell.Value, direction, force))
                return false;

            _cellGrid.MoveCell(cell, target);
            return true;
        }

        public override void OnCellInit(ref BasicCell cell)
        {

        }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                if (ct.IsCancellationRequested)
                    return;
                var targetPos = cell.Transform.Position - cell.Transform.Direction.AsVector2Int;
                var targetCell = _cellGrid.GetCell(targetPos);
                var targetFront = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;

                if (!_cellGrid.InBounds(targetFront))
                    continue;
                if (targetCell is null | !_cellGrid.InBounds(targetPos))
                {
                    _cellGrid.PushCell(cell, cell.Transform.Direction, 1);
                    continue;
                }

                if (targetCell.Value.Instance.Type == 8 | targetCell.Value.Instance.Type == 7 | targetCell.Value.Instance.Type == 19)
                {
                    _cellGrid.RemoveCell(cell);
                    continue;
                }

                if (targetCell.Value.Instance.Type != 18)
                {
                    _cellGrid.PushCell(targetCell.Value, cell.Transform.Direction, 1);
                }
            }
        }

        public override void Clear()
        {

        }
    }
}