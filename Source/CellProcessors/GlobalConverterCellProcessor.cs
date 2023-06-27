using Modding;
using Modding.PublicInterfaces.Cells;
using System.Linq;

namespace Indev2
{
    public class GlobalConverterCellProcessor : SteppedCellProcessor
    {
        public GlobalConverterCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override string Name => "Global Converter";
        public override int CellType => 24;
        public override string CellSpriteIndex => "GlobalConverter";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return false;
        }

        public override void OnCellInit(ref BasicCell cell) { }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void Step(System.Threading.CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                // Find the cells in front and behind the current cell
                var referencePos = cell.Transform.Position - cell.Transform.Direction.AsVector2Int;
                if (!_cellGrid.InBounds(referencePos))
                    continue;
                var referenceCell = _cellGrid.GetCell(referencePos);
                if (referenceCell is null)
                    continue;
                if (referenceCell.Value.Instance.Type == 20)
                    continue;
                var targetPos = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;

                if (!_cellGrid.InBounds(targetPos))
                    continue;

                var targetCell = _cellGrid.GetCell(targetPos);

                if (targetCell is null)
                    continue;
                if (referenceCell.Value.Instance.Type == 20)
                    continue;
                if (targetCell is not null && referenceCell is not null && (targetCell.Value.Instance.Type != referenceCell.Value.Instance.Type))
                {
                    var cellsToConvert = _cellGrid.GetCells().Where(a => a.Instance.Type == targetCell.Value.Instance.Type).ToList();

                    foreach (var c in cellsToConvert)
                    {
                        BasicCell useCell = referenceCell.Value;
                        useCell.Transform = c.Transform;
                        useCell.PreviousTransform = useCell.Transform;
                        _cellGrid.RemoveCell(c);
                        _cellGrid.AddCell(useCell);
                    }
                }
            }
        }

        public override void Clear() { }
    }
}