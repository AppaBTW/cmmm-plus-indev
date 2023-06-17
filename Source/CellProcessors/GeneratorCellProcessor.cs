using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class GeneratorCellProcessor: SteppedCellProcessor
    {
        public GeneratorCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Generator";
        public override int CellType => 2;
        public override string CellSpriteIndex => "Generator";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force <= 0)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);

            if (targetCell is null)
            {
                _cellGrid.MoveCell(cell, target);
                return true;
            }

            if (!_cellGrid.PushCell(targetCell.Value, direction, force))
                return false;


            _cellGrid.MoveCell(cell, target);
            return true;
        }

        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override void Step(CancellationToken ct)
        {
            foreach (var inputCell in GetOrderedCellEnumerable())
            {
                //Swaping sprites
                BasicCell swapCell = inputCell;
                if (inputCell.SpriteVariant != 0)
                {

                    switch (inputCell.SpriteVariant)
                    {
                        case > 0 and < 2:
                            swapCell.SpriteVariant++;
                            break;
                        case 2:
                            swapCell.SpriteVariant = 0;
                            break;
                    }

                    _cellGrid.RemoveCell(inputCell);
                    _cellGrid.AddCell(swapCell);
                }

                var generatorCell = swapCell;

                if(ct.IsCancellationRequested)
                    return;
                var copyCell = _cellGrid.GetCell(generatorCell.Transform.Position - generatorCell.Transform.Direction.AsVector2Int);
                if (copyCell is null)
                    continue;

                if (copyCell.Value.Instance.Type == 20)
                    return;
                var targetPos = generatorCell.Transform.Position + generatorCell.Transform.Direction.AsVector2Int;

                if (!_cellGrid.InBounds(targetPos))
                    continue;

                var targetCell = _cellGrid.GetCell(targetPos);
                if(targetCell != null && targetCell.Value.Instance.Type != 20)
                    if (!_cellGrid.PushCell(targetCell.Value, generatorCell.Transform.Direction, 1))
                        continue;

                var newCellTransform = generatorCell.Transform;
                newCellTransform.Direction = copyCell.Value.Transform.Direction;
                var prevTransform = newCellTransform;
                var newCell = _cellGrid.AddCell(targetPos, copyCell.Value.Transform.Direction, copyCell.Value.Instance.Type, prevTransform);
                generatorCell.SpriteVariant = 1;
                _cellGrid.RemoveCell(generatorCell);
                _cellGrid.AddCell(generatorCell);
            }
        }

        public override void Clear()
        {

        }
    }
}