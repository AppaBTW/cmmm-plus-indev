using System.Linq;
using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;
using UnityEngine;

namespace Indev2
{
    public class StrangeCellProcessor : TickedCellStepper
    {

        public override string Name => "Strange Cell";
        public override int CellType => 25;
        public override string CellSpriteIndex => "Strange";

        public StrangeCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force == -1)
            {
                if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
                    return false;
                return true;
            }
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

        public override void OnCellInit(ref BasicCell cell) { }

        public override void Clear() { }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                int rng = Random.Range(0, 3);

                if (rng == 0)
                {
                    _cellGrid.PushCell(cell, Direction.FromInt(Random.Range(0, 4)), 1);
                }
                else if (rng == 1)
                {
                    foreach (var direction in Direction.All)
                    {
                        Rotate(Random.Range(0, 3), cell, direction.AsInt);
                    }

                    Rotate(Random.Range(0, 3), cell, -1);

                }
            }
        }

        private void Rotate(int RotationAmount, BasicCell cell, int directionint)
        {
            var targetCell = _cellGrid.GetCell(cell.Transform.Position);
            if (directionint != -1)
            {
                Direction direction = Direction.FromInt(directionint);
                var target = cell.Transform.Position + direction.AsVector2Int;
                targetCell = _cellGrid.GetCell(target);
            }

            if (targetCell == null)
                return;
            if (targetCell.Value.Instance.Type == 20)
                return;

            BasicCell useCell;
            useCell = (BasicCell)targetCell;
            _cellGrid.RemoveCell(useCell);
            useCell.Transform = useCell.Transform.Rotate(RotationAmount);
            _cellGrid.AddCell(useCell);
        }
    }
}