using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    [LockRotation]
    public class VoidProcessor : CellProcessor
    {
        public override string Name => "Void";
        public override int CellType => 20;
        public override string CellSpriteIndex => "Void";


        public VoidProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return false;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return false;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
            cell.SpriteVariant = 1;
        }

        public override void Clear()
        {
            //do nothing
        }
    }
}