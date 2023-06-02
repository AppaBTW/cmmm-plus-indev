using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    [LockRotation]
    public class WallCellProcessor : CellProcessor
    {
        public override string Name => "Wall Cell";
        public override int CellType => 6;
        public override string CellSpriteIndex => "Wall";


        public WallCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return false;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
            //do nothing
        }

        public override void Clear()
        {
            //do nothing
        }
    }
}