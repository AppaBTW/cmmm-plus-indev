using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    [LockRotation]
    public class TrashCellProcessor : CellProcessor
    {
        public override string Name => "Trash Cell";
        public override int CellType => 7;
        public override string CellSpriteIndex => "Trash";


        public TrashCellProcessor(ICellGrid cellGrid) : base(cellGrid)
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
            //do nothing
        }

        public override void Clear()
        {
            //do nothing
        }
    }
}