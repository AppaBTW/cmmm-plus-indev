using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class FlipperCellProcessor : SemiRotatorProcessor
    {
        public FlipperCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Flipper";
        public override int CellType => 14;
        public override string CellSpriteIndex => "FlipperPlus";
        public override int RotationAmount => 2;


        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override void Clear()
        {

        }
    }
}