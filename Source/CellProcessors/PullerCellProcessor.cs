using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class PullerCellProcessor : SteppedCellProcessor
    {
        public PullerCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Puller";
        public override int CellType => 21;
        public override string CellSpriteIndex => "Puller";

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if(!cell.Frozen)
                if (direction == cell.Transform.Direction)
                    force++;
                else if (direction.Axis == cell.Transform.Direction.Axis)
                    force--;

            if (force <= 0)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);


            BasicCell useCell;
            if (targetCell is null)
            {
                useCell = cell;

                _cellGrid.MoveCell(useCell, target);
                return true;
            }

            if (!_cellGrid.PushCell(targetCell.Value, direction, force))
                return false;


            useCell = cell;

            _cellGrid.MoveCell(useCell, target);
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

                _cellGrid.PushCell(cell, cell.Transform.Direction, 0);

                if (targetCell != null)
                    _cellGrid.PushCell((BasicCell)targetCell, cell.Transform.Direction, 0);
                //_cellGrid.RemoveCell((BasicCell)targetCell);


            }
        }

        public override void Clear()
        {

        }
    }
}