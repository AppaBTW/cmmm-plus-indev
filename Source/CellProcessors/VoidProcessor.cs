using Modding;
using Modding.PublicInterfaces.Cells;
using System.Threading;
namespace Indev2
{
    [LockRotation]
    public class VoidProcessor : SteppedCellProcessor
    {
        public override string Name => "Void";
        public override int CellType => 20;
        public override string CellSpriteIndex => "Void";

        public VoidProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return false;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return false;
        }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                if (cell.SpriteVariant == 1)
                {
                    continue;
                }
                BasicCell useCell = cell;
                useCell.SpriteVariant = 1;
                _cellGrid.RemoveCell(cell);
                _cellGrid.AddCell(useCell);
            }
        }
        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override void Clear() { }
    }
}