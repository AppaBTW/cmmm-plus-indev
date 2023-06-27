using Modding;
using Modding.PublicInterfaces.Cells;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Indev2
{
    public class InputMoverCellProcessor : SteppedCellProcessor
    {

        public InputMoverCellProcessor(ICellGrid cellGrid) : base(cellGrid) { }

        public override string Name => "Input Mover";
        public override int CellType => 46;
        public override string CellSpriteIndex => "InputMover";

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force == -1)
            {
                if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
                    return false;
                return true;
            }
            if (cell.SpriteVariant == 1)
            {
                if (direction == cell.Transform.Direction)
                    force++;
                else if (direction.Axis == cell.Transform.Direction.Axis)
                    force--;
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
            
            bool leftDown = Input.GetMouseButtonDown(0);
            foreach (var cell in GetOrderedCellEnumerable())
            {
                if (ct.IsCancellationRequested)
                    return;
                BasicCell useCell = cell;
                if (leftDown)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = 0;
                    if (Vector2.Distance(mousePos, cell.Transform.Position) < 100f)
                    {
                        if (useCell.SpriteVariant == 0)
                        useCell.SpriteVariant = 1;
                        else
                        useCell.SpriteVariant = 0;
                    }
                    
                }
                if (useCell.SpriteVariant == 1)
                {
                    _cellGrid.PushCell(useCell, cell.Transform.Direction, 0);
                }
            
            }
            
        }
    }
}