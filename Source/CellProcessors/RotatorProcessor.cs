using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public abstract class RotatorProcessor : TickedCellStepper
    {
        public abstract int RotationAmount { get; }

        protected RotatorProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                if(ct.IsCancellationRequested)
                    return;
                foreach (var direction in Direction.All)
                {
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    if (targetCell.Value.Instance.Type == 20)
                        continue;
                    if (targetCell.Value.Instance.Type == 6)
                        continue;
                    BasicCell useCell;
                    useCell = (BasicCell)targetCell;
                    _cellGrid.RemoveCell(useCell);
                    useCell.Transform = useCell.Transform.Rotate(RotationAmount);
                    _cellGrid.AddCell(useCell);
                    continue;
                }
            }
        }

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
    }
}