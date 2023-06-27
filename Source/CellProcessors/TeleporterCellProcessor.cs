using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class TeleporterCellProcessor : SteppedCellProcessor
    {
        public TeleporterCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override string Name => "Teleporter";
        public override int CellType => 17;
        public override string CellSpriteIndex => "Teleporter";

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
                var referencePos = cell.Transform.Position - cell.Transform.Direction.AsVector2Int;
                var referenceCell = _cellGrid.GetCell(referencePos);
                var targetPos = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;
                var targetCell = _cellGrid.GetCell(targetPos);

                //  This does all the checks for stuff
                if (referenceCell is null)
                    continue;
                if (referenceCell.Value.Instance.Type == 20)
                    continue;
                if (referenceCell.Value.Instance.Type == 19)
                    continue;
                if (!_cellGrid.InBounds(targetPos))
                    continue;

                BasicCell useCell = referenceCell.Value;
                useCell.Transform = useCell.Transform.SetPosition(targetPos);
                useCell.Transform = useCell.Transform.SetDirection(referenceCell.Value.Transform.Direction);
                useCell.PreviousTransform = useCell.PreviousTransform.SetPosition(referenceCell.Value.Transform.Position);
                if (targetCell is not null)
                {
                    if (targetCell.Value.Instance.Type == 19)
                    {
                    check:
                        {
                            if (targetCell is not null)
                            {
                                if (targetCell.Value.Instance.Type == 19 && targetCell.Value.Transform.Direction == cell.Transform.Direction)
                                {
                                    targetPos += cell.Transform.Direction.AsVector2Int;
                                    targetCell = _cellGrid.GetCell(targetPos);
                                    goto check;
                                }
                            }
                        }
                    }
                    else
                    {
                        targetCell = _cellGrid.GetCell(targetPos);
                        if (!_cellGrid.PushCell(targetCell.Value, cell.Transform.Direction, 1))
                        {
                            continue;
                        }
                    }

                }

                if (targetCell is not null)
                {
                    targetCell = _cellGrid.GetCell(targetPos);
                    if (!_cellGrid.PushCell(targetCell.Value, cell.Transform.Direction, 1))
                    {
                        continue;
                    }
                }
                useCell.Transform = useCell.Transform.SetPosition(targetPos);

                _cellGrid.RemoveCell(referenceCell.Value);
                _cellGrid.AddCell(useCell);
            }
        }

        public override void Clear() { }
    }
}