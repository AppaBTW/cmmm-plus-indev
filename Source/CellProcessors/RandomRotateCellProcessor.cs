using Modding;
using Modding.PublicInterfaces.Cells;
using System.Threading;
using UnityEngine;

namespace Indev2
{

    public class RandomRotateCellProcessor : RotatorProcessor
    {
        public RandomRotateCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Random Rotator";
        public override int CellType => 23;
        public override string CellSpriteIndex => "RandomRotator";

        public override int RotationAmount => throw new System.NotImplementedException();

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                if(ct.IsCancellationRequested)
                    return;
                foreach (var direction in Direction.All)
                {
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    if (targetCell.Value.Instance.Type == 20)
                        continue;
                    int rotationAmount = Random.Range(333, 335);

                    if (rotationAmount == 333)
                    {
                        rotationAmount = 1;
                    }
                    else if (rotationAmount == 334)
                    {
                        rotationAmount = 3;
                    }

                    BasicCell useCell;
                    useCell = (BasicCell)targetCell;
                    _cellGrid.RemoveCell(useCell);
                    useCell.Transform = useCell.Transform.Rotate(rotationAmount);
                    _cellGrid.AddCell(useCell);
                    continue;
                }
            }
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

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

        public override void OnCellInit(ref BasicCell cell)
        {
        }

        public override void Clear()
        {
        }
    }
}