using System;
using System.Collections.Generic;
using CellEncoding;
using Indev2;
using Modding;


//MUST BE NAMED MOD
public class Mod : IMod
{
    public static Interface Interface;
    public string UniqueName => "CellMachineMysticModPlusRemake";
    public string DisplayName => "CMMM+";
    public string Author => "AppaBTW";
    public string Version => "2.0.0";
    public ILevelFormat LevelFormat => null;
    public string Description => "Ports the cells from cmmm+";
    public string[] Dependencies => Array.Empty<string>();

    public void Initialize(Interface @interface)
    {
        Interface = @interface;
    }

    public IEnumerable<CellProcessor> GetCellProcessors(ICellGrid cellGrid)
    {
        yield return new BasicCellProcessor(cellGrid);
        yield return new SlideCellProcessor(cellGrid);
        yield return new OneDirectionalCellProcessor(cellGrid);
        yield return new FreezeProcessor(cellGrid);
        yield return new PresentCellProcessor(cellGrid);
        yield return new StrangeCellProcessor(cellGrid);
        //yield return new DenierCellProcessor(cellGrid);
        //yield return new InputEnemyCellProcessor(cellGrid);
        yield return new ConverterCellProcessor(cellGrid);
        yield return new GlobalConverterCellProcessor(cellGrid);
        //yield return new InputGeneratorCellProcessor(cellGrid);
        yield return new GeneratorCellProcessor(cellGrid);
        //yield return new PhysicalGeneratorCellProcessor(cellGrid);
        yield return new CWRotateProcessor(cellGrid);
        yield return new CCWRotateProcessor(cellGrid);
        yield return new RandomRotateCellProcessor(cellGrid);
        yield return new FixedRotatorCellProcessor(cellGrid);
        yield return new FlipperCellProcessor(cellGrid);
        yield return new TeleporterCellProcessor(cellGrid);
        //yield return new PacmanCellProcessor(cellGrid);
        //yield return new PlayerCellProcessor(cellGrid);
        //yield return new InputMoverCellProcessor(cellGrid);
        //yield return new SpeedCellProcessor(cellGrid);
        yield return new MoverCellProcessor(cellGrid);
        yield return new PullerCellProcessor(cellGrid);
        yield return new NudgeCellProcessor(cellGrid);
        yield return new FallCellProcessor(cellGrid);
        yield return new WallCellProcessor(cellGrid);
        yield return new VoidProcessor(cellGrid);
        yield return new TrashCellProcessor(cellGrid);
        yield return new EnemyCellProcessor(cellGrid);
    }
}
