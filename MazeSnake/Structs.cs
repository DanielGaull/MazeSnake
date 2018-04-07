using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MazeSnake
{
    [Serializable]
    public struct Settings
    {
        public int Volume;
        public bool ShowMouseWhilePlaying;

        public Settings(int volume, bool showMouseWhilePlaying)
        {
            Volume = volume;
            ShowMouseWhilePlaying = showMouseWhilePlaying;
        }
    }

    public struct Cell
    {
        public int Row;
        public int Column;
        public List<Cell> Neighbors;

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
            Neighbors = new List<Cell>();
        }

        public bool MemberEquals(Cell cell)
        {
            return this.Row == cell.Row && this.Column == cell.Column;
        }

        public bool IsNeighbor(Cell cell)
        {
            if ((Row == cell.Row && (cell.Column + 1 == Column || cell.Column - 1 == Column)
                                    && !MemberEquals(cell)) || (Column == cell.Column && (cell.Row + 1 == Row || cell.Row - 1 == Row)
                                    && !MemberEquals(cell)))
            {
                // "cell" neighbors this cell
                return true;
            }
            return false;
        }

        public int Distance(Cell cell)
        {
            return Math.Abs(Row - cell.Row) + Math.Abs(Column - cell.Column);
        }

        public bool ContainsWall(Wall wall)
        {
            if ((Row == wall.Row && Column == wall.Column) ||
                (Row == wall.Row - 1 && Column == wall.Column && wall.Direction == Direction2.LeftRight) ||
                (Row == wall.Row && Column == wall.Column - 1 && wall.Direction == Direction2.UpDown))
            {
                // "wall" is within this cell
                return true;
            }
            return false;
        }
        public bool ContainsWallInDirection(Maze maze, Direction dir)
        {
            for (int i = 0; i < maze.Walls.Count; i++)
            {
                switch (dir)
                {
                    case Direction.Up:
                        if (maze.Walls[i].Row == Row &&
                            maze.Walls[i].Column == Column &&
                            maze.Walls[i].Direction == Direction2.LeftRight)
                        {
                            return true;
                        }
                        break;
                    case Direction.Down:
                        if (maze.Walls[i].Row == Row + 1 &&
                            maze.Walls[i].Column == Column &&
                            maze.Walls[i].Direction == Direction2.LeftRight)
                        {
                            return true;
                        }
                        break;
                    case Direction.Left:
                        if (maze.Walls[i].Row == Row &&
                            maze.Walls[i].Column == Column &&
                            maze.Walls[i].Direction == Direction2.UpDown)
                        {
                            return true;
                        }
                        break;
                    case Direction.Right:
                        if (maze.Walls[i].Row == Row &&
                            maze.Walls[i].Column == Column + 1 &&
                            maze.Walls[i].Direction == Direction2.UpDown)
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        public static bool WallInBetween(Cell c1, Cell c2, Maze maze)
        {
            // We first check if there are any walls that are contained within c1 and c2. If so, we return
            // true because both cells contain the same wall, meaning it's in between them (make a list of
            // walls that meet the criteria, then check if the cound is bigger than zero, meaning that a
            // wall meeting the criteria was found)
            return maze.Walls.Where(x => c1.ContainsWall(x) && c2.ContainsWall(x)).Count() > 0;
        }

        public Cell Clone()
        {
            return new Cell { Row = this.Row, Column = this.Column, Neighbors = this.Neighbors };
        }

        public static bool operator ==(Cell c1, Cell c2)
        {
            return (c1.MemberEquals(c2));
        }
        public static bool operator !=(Cell c1, Cell c2)
        {
            return !(c1.MemberEquals(c2));
        }
        public override bool Equals(object obj)
        {
            return MemberEquals((Cell)obj);
        }
        public override int GetHashCode()
        {
            return Row * Column;
        }
    }

    public struct Turtle
    {
        public int Row;
        public int Column;
        public Cell Cell;
        int MAX_ROW;
        int MAX_COLUMN;

        public Turtle(int row, int column, Cell cell, int maxRow, int maxColumn)
        {
            this.Row = row;
            this.Column = column;
            this.Cell = cell;
            MAX_ROW = maxRow;
            MAX_COLUMN = maxColumn;
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    if (Row > 0)
                    {
                        Row--;
                    }
                    else
                    {
                        Row++;
                    }
                    break;
                case Direction.Down:
                    if (Row < MAX_ROW)
                    {
                        Row++;
                    }
                    else
                    {
                        Row--;
                    }
                    break;
                case Direction.Left:
                    if (Column > 0)
                    {
                        Column--;
                    }
                    else
                    {
                        Column++;
                    }
                    break;
                case Direction.Right:
                    if (Column < MAX_COLUMN)
                    {
                        Column++;
                    }
                    else
                    {
                        Column--;
                    }
                    break;
            }
        }

        public Direction? PickRandomDirection(Random rand, HashSet<Direction> unallowedValues)
        {
            Direction returnVal = (Direction)rand.Next(4);
            if (unallowedValues.Count < Enum.GetNames(typeof(Direction)).ToList().Count/* This value is equal to the number of possible directions (probably 4) */)
            {
                while (unallowedValues.Contains(returnVal))
                {
                    returnVal = (Direction)rand.Next(4);
                }
            }
            else
            {
                return null;
            }

            return returnVal;
        }
    }

    [Serializable]
    public struct Skin
    {
        public string SkinAsset;
        public SerializableColor Color;
        public bool HasTongue;
        public string Name;
        public SnakeSkinType Type;
        public bool HasHat;
        public string HatAsset;

        public Skin(string skinAsset, Color color, bool hasTongue, string name, SnakeSkinType type)
        {
            SkinAsset = skinAsset;
            Color = color.Serialize();
            HasTongue = hasTongue;
            Name = name;
            Type = type;
            HasHat = false;
            HatAsset = "";
        }
        public Skin(string skinAsset, Color color, bool hasTongue, string name, SnakeSkinType type, string hatAsset)
        {
            SkinAsset = skinAsset;
            Color = color.Serialize();
            HasTongue = hasTongue;
            Name = name;
            Type = type;
            HasHat = true;
            HatAsset = hatAsset;
        }

        public bool MemberEquals(Skin skin)
        {
            return (this.SkinAsset == skin.SkinAsset && this.Color == skin.Color && this.HasTongue == skin.HasTongue && this.Name == skin.Name
                && this.Type == skin.Type && this.HatAsset == skin.HatAsset && this.HasHat == skin.HasHat);
        }
    }
    [Serializable]
    public struct OldSkin
    {
        public string SkinAsset;
        public Color Color;
        public bool HasTongue;
        public string Name;
        public SnakeSkinType Type;
        public bool HasHat;
        public string HatAsset;

        public Skin Convert()
        {
            if (HasHat)
            {
                return new Skin(SkinAsset, Color, HasTongue, Name, Type, HatAsset);
            }
            else
            {
                return new Skin(SkinAsset, Color, HasTongue, Name, Type);
            }
        }
    }

    [Serializable]
    public struct SerializableColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public SerializableColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color GetColor()
        {
            return new Color((int)R, (int)G, (int)B, (int)A);
        }

        public static bool operator ==(SerializableColor a, SerializableColor b)
        {
            return (a.R == b.R) && (a.G == b.G) && (a.B == b.B) && (a.A == b.A);
        }
        public static bool operator !=(SerializableColor a, SerializableColor b)
        {
            return !((a.R == b.R) && (a.G == b.G) && (a.B == b.B) && (a.A == b.A));
        }

        public override bool Equals(object obj)
        {
            return this == (SerializableColor)obj;
        }
        public override int GetHashCode()
        {
            return int.Parse(string.Format("{0}{1}{2}{3}", R, G, B, A));
        }
    }

    public delegate void InventoryAction();
    [Serializable]
    public struct InventoryItem
    {
        #region Fields & Properties

        public string Name;
        public ItemType Type;

        #endregion

        #region Constructor

        public InventoryItem(string name, ItemType type)
        {
            this.Name = name;
            this.Type = type;
        }

        #endregion
    }
}