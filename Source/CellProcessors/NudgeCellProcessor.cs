using Modding;
using Modding.PublicInterfaces.Cells;
using System.Threading;

namespace Indev2 {
  public class NudgeCellProcessor: SteppedCellProcessor {

    public NudgeCellProcessor(ICellGrid cellGrid): base(cellGrid) {}

    public override string Name => "Nudge";
    public override int CellType => 12;
    public override string CellSpriteIndex => "Nudge";

    public override bool TryPush(BasicCell cell, Direction direction, int force) {
      if (force == -1) {
        if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
          return false;
        return true;
      }
      if (cell.SpriteVariant == 1) {
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
      if (targetCell is null) {
        cell.SpriteVariant = 1;
        _cellGrid.MoveCell(cell, target);
        return true;
      }

      if (!_cellGrid.PushCell(targetCell.Value, direction, force))
        return false;

      cell.SpriteVariant = 1;
      _cellGrid.MoveCell(cell, target);

      return true;
    }
    public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell) {
      return true;
    }
    public override void OnCellInit(ref BasicCell cell) {}
    public override void Clear() {}

    public override void Step(CancellationToken ct) {
      foreach(var cell in GetOrderedCellEnumerable()) {
        if (ct.IsCancellationRequested)
          return;
        if (cell.SpriteVariant == 1) {
          _cellGrid.PushCell(cell, cell.Transform.Direction, 0);
        }

      }
    }
  }
}