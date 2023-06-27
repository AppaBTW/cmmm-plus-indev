using System.Linq;
using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class FreezeProcessor : TickedCellStepper
    {

        public override string Name => "Freeze Cell";
        public override int CellType => 9;
        public override string CellSpriteIndex => "Freeze";

        public FreezeProcessor(ICellGrid cellGrid) : base(cellGrid) { }

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

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void OnCellInit(ref BasicCell cell) { }

        public override void Clear() { }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                foreach (var direction in Direction.All)
                {
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);
                    if (targetCell is null)
                        continue;
                    if (targetCell.Value.Instance.Type == 20)
                        continue;
                    var basicCell = targetCell.Value;
                    basicCell.Frozen = true;

                    _cellGrid.RemoveCell(targetCell.Value);
                    _cellGrid.AddCell(basicCell);
                }
            }
        }
    }
}