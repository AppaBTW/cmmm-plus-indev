using Modding;
using Modding.PublicInterfaces.Cells;
using System.Linq;

namespace Indev2
{
    public class GlobalConverterCellProcessor : SteppedCellProcessor
    {
        public GlobalConverterCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Global Converter";
        public override int CellType => 14;
        public override string CellSpriteIndex => "GlobalConverter";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return false;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void Step(System.Threading.CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                // Find the cells in front and behind the current cell
                var targetPos = cell.Transform.Position + cell.Transform.Direction.AsVector2Int;
                var targetCell = _cellGrid.GetCell(targetPos);
                var referencePos = cell.Transform.Position - cell.Transform.Direction.AsVector2Int;
                var referenceCell = _cellGrid.GetCell(referencePos);

                if (targetCell is not null && referenceCell is not null && (targetCell.Value.Instance.Type != referenceCell.Value.Instance.Type || targetCell.Value.Transform.Direction != referenceCell.Value.Transform.Direction))
                {
                    var cellsToConvert = _cellGrid.GetCells().Where(a => a.Instance.Type == targetCell.Value.Instance.Type).ToList();

                    foreach (var c in cellsToConvert)
                    {
                        var oldTransform = c.Transform;
                        _cellGrid.AddCell(c.Transform.Position, referenceCell.Value.Transform.Direction, (int)(uint)referenceCell.Value.Instance.Type, oldTransform);
                    }
                }
            }
        }



        public override void Clear()
        {
        }
    }
}
