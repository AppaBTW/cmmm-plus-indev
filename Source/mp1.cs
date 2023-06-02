using System.Collections.Generic;
using System.Text;
using CellEncoding;
using Modding;
using Modding.PublicInterfaces.Cells;
using UnityEngine;

namespace Indev2.Encoder
{
    public class MP1 : ILevelFormat
    {
        private static readonly Dictionary<int, int> V3IdMap = new Dictionary<int, int>()
        {
            {0, 2},
            {1, 3},
            {2, 4},
            {3, 1},
            {4, 5},
            {5, 0},
            {6, 6},
            {7, 8},
            {8, 7}
        };

        private static Dictionary<char, int> _decodeDictionary;

        public string Name => "CMMM+ MP1";

        static MP1()
        {
            _decodeDictionary = new Dictionary<char, int>();

            for (int i = 0; i < 74; i++)
            {
                _decodeDictionary.Add(CellKey[i], i);
            }
        }

        public (byte[], string) EncodeLevel(ILevel level)
        {
            StringBuilder output = new StringBuilder();

            //Used variables
            var width = level.CellGrid.Width;
            var height = level.CellGrid.Height;


            //Header stuff
            output.Append("V3;");
            var encodedWidth = EncodeInt(width);
            var encodedHeight = EncodeInt(height);
            output.Append(encodedWidth);
            output.Append(";");
            output.Append(encodedHeight);
            output.Append(";");

            int[] cellData = new int[width * height];
            int dataIndex = 0;

            //Encode tiles (73 for true, 72 for false)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cellData[x + (y * width)] = level.CellGrid.IsDraggable(x, y) ? 73 : 72;
                }
            }

            //Encode cells
            foreach (BasicCell cell in level.CellGrid.GetCells())
            {
                var cellType = cell.Instance.Type;


                int type = -1;

                //reverse index V3IdMap
                foreach (var pair in V3IdMap)
                {
                    if (pair.Value != cellType) continue;
                    type = pair.Key;
                    break;
                }

                if(type == -1)
                    continue; //Skip if not found

                cellData[(int)cell.Transform.Position.x + ((int)cell.Transform.Position.y * width)] += (2 * (int)type) + (18 * cell.Transform.Direction.AsInt) - 72;
            }

            int runLength = 1;

            int matchLength;
            int maxMatchLength;
            int maxMatchOffset = 0;

            while (dataIndex < cellData.Length)
            {
                maxMatchLength = 0;
                for (int matchOffset = 1; matchOffset <= dataIndex; matchOffset++)
                {
                    matchLength = 0;
                    while (dataIndex + matchLength < cellData.Length && cellData[dataIndex + matchLength] ==
                           cellData[dataIndex + matchLength - matchOffset])
                    {
                        matchLength++;
                        if (matchLength > maxMatchLength)
                        {
                            maxMatchLength = matchLength;
                            maxMatchOffset = matchOffset - 1;
                        }
                    }
                }

                if (maxMatchLength > 3)
                {
                    if (EncodeInt(maxMatchLength).Length == 1)
                    {
                        if (EncodeInt(maxMatchOffset).Length == 1)
                        {
                            if (maxMatchLength > 3)
                            {
                                output.Append(")" + EncodeInt(maxMatchOffset) + EncodeInt(maxMatchLength));
                                dataIndex += maxMatchLength - 1;
                            }
                            else
                                output.Append(CellKey[cellData[dataIndex]]);
                        }
                        else
                        {
                            if (maxMatchLength > 3 + EncodeInt(maxMatchOffset).Length)
                            {
                                output.Append("(" + EncodeInt(maxMatchOffset) + ")" + EncodeInt(maxMatchLength));
                                dataIndex += maxMatchLength - 1;
                            }
                            else
                                output.Append(CellKey[cellData[dataIndex]]);
                        }
                    }
                    else
                    {
                        output.Append("(" + EncodeInt(maxMatchOffset) + "(" + EncodeInt(maxMatchLength) + ")");
                        dataIndex += maxMatchLength - 1;
                    }
                }
                else
                    output.Append(CellKey[cellData[dataIndex]]);

                maxMatchLength = 0;
                dataIndex += 1;
            }

            //Finalize
            output.Append($";{level.Properties.Description};{level.Properties.Name}");

            //Encode cell data
            var stringOutput = output.ToString();
            var byteData = Encoding.ASCII.GetBytes(stringOutput);

            return (byteData, stringOutput);
        }

        public DecodeResult Decode(byte[] data)
        {
            //Get ascii string
            string ascii = Encoding.ASCII.GetString(data);
            return Decode(ascii);
        }

        public DecodeResult Decode(string level)
        {
            return DecodeV3(level);
        }

        public bool Matches(byte[] data)
        {
            string ascii = System.Text.Encoding.ASCII.GetString(data);
            return ascii.StartsWith("V3;");
        }

        public bool Matches(string level) => level.StartsWith("V3;");




        //Hella spaghetti code, but it works so shouldn't be changed
        private const string CellKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!$%&+-.=?^{}";
        //Manually decode V3 since it's not a format
        public static DecodeResult DecodeV3(string input)
        {
            DecodeResult result = new DecodeResult();

            var args = input.Split(';');

            int length;
            int dataIndex = 0;
            int gridIndex = 0;
            string temp;

            var width = DecodeString(args[1], _decodeDictionary);
            var height = DecodeString(args[2], _decodeDictionary);

            int[] cellDataHistory = new int[width * height];
            int offset;

            var cells = new List<BasicCell>();
            var levelName = args[5];

            var sizeVector = new Vector2Int(width, height);

            var dragSpots = new List<Vector2Int>();

            while (dataIndex < args[3].Length)
            {
                if (args[3][dataIndex] == ')' || args[3][dataIndex] == '(')
                {
                    if (args[3][dataIndex] == ')')
                    {
                        dataIndex += 2;
                        offset = _decodeDictionary[args[3][dataIndex - 1]];
                        length = _decodeDictionary[args[3][dataIndex]];

                    }
                    else
                    {
                        dataIndex++;
                        temp = "";
                        while (args[3][dataIndex] != ')' && args[3][dataIndex] != '(')
                        {
                            temp += args[3][dataIndex];
                            dataIndex++;
                        }
                        offset = DecodeString(temp, _decodeDictionary);
                        if (args[3][dataIndex] == ')')
                        {
                            dataIndex++;
                            length = _decodeDictionary[args[3][dataIndex]];
                        }
                        else
                        {
                            dataIndex++;
                            temp = "";
                            while (args[3][dataIndex] != ')')
                            {
                                temp += args[3][dataIndex];
                                dataIndex++;
                            }
                            length = DecodeString(temp, _decodeDictionary);
                        }
                    }
                    for (int i = 0; i < length; i++)
                    {
                        var cell = GetV3Cell(cellDataHistory[gridIndex - offset - 1], gridIndex, sizeVector);
                        if(cell.Item1 != null)
                            cells.Add(cell.Item1.Value);
                        if(cell.Item2 != null)
                            dragSpots.Add(cell.Item2.Value);
                        cellDataHistory[gridIndex] = cellDataHistory[gridIndex - offset - 1];
                        gridIndex++;
                    }
                }
                else
                {
                    var cell = GetV3Cell(_decodeDictionary[args[3][dataIndex]], gridIndex, sizeVector);
                    if(cell.Item1 != null)
                        cells.Add(cell.Item1.Value);
                    if(cell.Item2 != null)
                        dragSpots.Add(cell.Item2.Value);
                    cellDataHistory[gridIndex] = _decodeDictionary[args[3][dataIndex]];
                    gridIndex++;
                }

                dataIndex++;
            }

            return new DecodeResult()
            {
                Cells = cells.ToArray(),
                DependMod = "Vanilla",
                Format = "V3",
                // ReSharper disable once HeapView.BoxingAllocation
                Name = levelName,
                Size = new Vector2Int(width, height),
                Description = args[4],
                DragSpots = dragSpots.ToArray()
            };
        }

        private static (BasicCell?, Vector2Int?) GetV3Cell(int c, int i, Vector2Int size)
        {

            Vector2Int? spot = null;
            if (c % 2 == 1)
                spot = new Vector2Int(i % size.x, i / size.x);
            if (c >= 72)
                return (null, spot);

            var type = V3IdMap[((c / 2) % 9)];
            var pos = new Vector2Int(i % size.x, i / size.x);
            var dir = (c / 18);

            return (new BasicCell(type, new CellTransform(pos, Direction.FromInt(dir))), spot);
        }

        private static int DecodeString(string str, Dictionary<char, int> decode)
        {
            int output = 0;
            foreach (char c in str)
            {
                output *= 74;
                output += decode[c];
            }
            return output;
        }

        private string EncodeInt(int num)
        {
            if (num < 74)
                return CellKey[num % 74].ToString();

            string output = "";
            while (num > 0)
            {
                output = CellKey[num % 74] + output;
                num /= 74;
            }
            return output;
        }


    }
}