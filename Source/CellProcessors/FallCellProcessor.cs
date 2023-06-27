using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class FallCellProcessor : SteppedCellProcessor
    {
        public FallCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override string Name => "Fall";
        public override int CellType => 15;
        public override string CellSpriteIndex => "Fall";

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

        public override void OnCellInit(ref BasicCell cell) { }

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
                var targetCell = _cellGrid.GetCell(targetPos);

                //  This does all the checks for stuff
                if (!_cellGrid.InBounds(targetPos))
                    continue;

                check:
                    {
                        if (targetCell is null)
                        {
                                targetPos += cell.Transform.Direction.AsVector2Int;
                                targetCell = _cellGrid.GetCell(targetPos);
                                if (_cellGrid.InBounds(targetPos))
                                goto check;


                        }
                    }
                targetPos -= cell.Transform.Direction.AsVector2Int;
                BasicCell UseCell = cell;
                UseCell.Transform = UseCell.Transform.SetPosition(targetPos);

                _cellGrid.RemoveCell(cell);
                _cellGrid.AddCell(UseCell);
            }
        }

        public override void Clear() { }
    }
}