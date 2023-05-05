using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class FlipperProcessor : TickedCellStepper
    {
        public FlipperProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Flipper";
        public override int CellType => 16;
        public override string CellSpriteIndex => "FlipperPlus";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force <= 0)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
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

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
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

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                if (ct.IsCancellationRequested)
                    return;
                foreach (var direction in Direction.All)
                {
                    if (direction.Axis != cell.Transform.Direction.Axis)
                        continue;

                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    _cellGrid.RotateCell(targetCell.Value, targetCell.Value.Transform.Direction.Rotate(2));
                }
            }
        }
    }
}