using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class ConverterCellProcessor : SteppedCellProcessor
    {
        public ConverterCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Converter";
        public override int CellType => 12;
        public override string CellSpriteIndex => "Converter";

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

                if (!_cellGrid.InBounds(targetPos))
                    continue;

                var targetCell = _cellGrid.GetCell(targetPos);

                if (targetCell is null)
                    continue;

                var referencePos = cell.Transform.Position - cell.Transform.Direction.AsVector2Int;
                var referenceCell = _cellGrid.GetCell(referencePos);

                if (targetCell is not null && referenceCell is not null && (targetCell.Value.Instance.Type != referenceCell.Value.Instance.Type || targetCell.Value.Transform.Direction != referenceCell.Value.Transform.Direction))
                {
                    _cellGrid.AddCell(targetPos, referenceCell.Value.Transform.Direction, (int)(uint)referenceCell.Value.Instance.Type, cell.Transform);
                }
                

            }
        }



        public override void Clear()
        {
        }
    }
}
