using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{

    public class CWRotateProcessor : RotatorProcessor
    {
        public CWRotateProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "CW Rotator";
        public override int CellType => 3;
        public override string CellSpriteIndex => "CW Rotator";
        public override int RotationAmount => 1;


        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override void Clear()
        {

        }
    }
}