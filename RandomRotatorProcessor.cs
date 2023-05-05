using Modding.PublicInterfaces.Cells;
using System.Threading;
using Modding;
using UnityEngine;

namespace Indev2
{
    public class RandomRotatorProcessor : TickedCellStepper
    {
        public RandomRotatorProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Random Rotator";
        public override int CellType => 18;
        public override string CellSpriteIndex => "RandomRotator";

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

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
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

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                if (ct.IsCancellationRequested)
                    return;
                foreach (var direction in Direction.All)
                {
                    int randomNum = Random.Range(0, 2);
                    int rotationAmount;
                    if (randomNum == 0)
                    {
                        rotationAmount = -90;
                    }
                    else
                    {
                        rotationAmount = 90;
                    }
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    _cellGrid.RotateCell(targetCell.Value, targetCell.Value.Transform.Direction.Rotate(rotationAmount));
                }
            }
        }
    }
}