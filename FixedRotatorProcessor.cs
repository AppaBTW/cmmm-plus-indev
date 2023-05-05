using Modding.PublicInterfaces.Cells;
using System.Threading;
using Modding;


namespace Indev2
{
    public class FixedRotatorProcessor : TickedCellStepper
    {
        public FixedRotatorProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Fixed Rotator";
        public override int CellType => 17;
        public override string CellSpriteIndex => "FixedRotator";

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
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    _cellGrid.RotateCell(targetCell.Value, cell.Transform.Direction);
                }
            }
        }
    }
}