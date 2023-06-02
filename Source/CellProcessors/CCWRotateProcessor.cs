using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class CCWRotateProcessor : RotatorProcessor
    {
        public CCWRotateProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "CCW Rotator";
        public override int CellType => 4;
        public override string CellSpriteIndex => "CCW Rotator";
        public override int RotationAmount => -1;
        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override void Clear()
        {

        }
    }
}