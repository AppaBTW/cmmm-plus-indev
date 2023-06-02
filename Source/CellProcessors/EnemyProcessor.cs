using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class EnemyCellProcessor : CellProcessor
    {
        public override string Name => "Enemy Cell";
        public override int CellType => 8;
        public override string CellSpriteIndex => "Enemy";


        public EnemyCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell existingCell)
        {
            _cellGrid.RemoveCell(existingCell);
            return false;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            return true;
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