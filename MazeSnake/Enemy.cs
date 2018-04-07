using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MazeSnake
{
    public class Enemy
    {
        #region Fields

        Texture2D img;
        public Rectangle Rectangle;
        SpriteEffects effects = SpriteEffects.None;
        Color color = Color.White;

        public Cell Cell;
        Cell movingTo = new Cell();
        bool isMovingToCell = false;
        const int SPEED = 2;
        Vector2 direction = new Vector2();
        float distance = 0.0f;
        Vector2 location = new Vector2();
        Vector2 start = new Vector2();
        Direction travelingIn = Direction.Down;

        Random rand = new Random();

        public int Id;
        static int maxId = 0;

        #endregion

        #region Constructors

        public Enemy(Texture2D img, int x, int y, int row, int column, int width, int height)
        {
            this.img = img;
            Rectangle = new Rectangle(x, y, width, height);
            Cell.Row = row;
            Cell.Column = column;
            location = new Vector2(x, y);
            Id = maxId + 1;
            maxId++;
        }

        #endregion

        #region Public Methods

        public void Start(Maze maze)
        {
            try
            {
                //GenerateWanderPath(maze);
                //MoveToCell(path[0]);
                for (int i = 0; i < maze.Cells.Count; i++)
                {
                    if (maze.Cells[i].Row == Cell.Row && maze.Cells[i].Column == Cell.Column)
                    {
                        Cell = maze.Cells[i];
                        break;
                    }
                }
                MoveToCell(Cell.Neighbors.Except(Cell.Neighbors.Where(x => Cell.WallInBetween(Cell, x, maze))).ToList().Random());
            }
            catch (Exception e)
            {
                for (int i = 0; i < maze.Cells.Count; i++)
                {
                    if (maze.Cells[i].Row == 12)
                    {

                    }
                }
                Console.WriteLine();
            }
        }

        public void Update(Maze maze)
        {
            if (isMovingToCell)
            {
                StepMoveToCell(maze);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, Rectangle, null, color, 0.0f, new Vector2(), effects, 1.0f);
        }

        public bool Intersects(Rectangle rect)
        {
            return rect.Intersects(Rectangle);
        }

        public static void ResetId()
        {
            maxId = 0;
        }

        #endregion

        #region Private Methods

        #region Unused

        //private void Pathfind(Cell snakeCell, Maze maze)
        //{
        //    path.Clear();
        //    visited.Clear();
        //    Cell tCell = new Cell(); // Imaginary cell

        //    cellCosts.Clear();
        //    cellCostStrings.Clear();
        //    cellCostLocs.Clear();
        //    for (int i = 0; i < maze.Cells.Count; i++)
        //    {
        //        cellCosts.Add(new Tuple<Cell, int>(maze.Cells[i], maze.Cells[i].Distance(snakeCell)));
        //        cellCostStrings.Add("(" + maze.Cells[i].Row + "," + maze.Cells[i].Column + ")");
        //        cellCostLocs.Add(new Vector2(Maze.GetCellCenter(maze.Cells[i]).X - font.MeasureString(cellCostStrings[i]).X / 2, 
        //            Maze.GetCellCenter(maze.Cells[i]).Y - font.MeasureString(cellCostStrings[i]).Y / 2));
        //        if (maze.Cells[i].Row == Cell.Row && maze.Cells[i].Column == Cell.Column)
        //        {
        //            tCell = maze.Cells[i];
        //        }
        //    }

        //    while (tCell.Row != snakeCell.Row || tCell.Column != snakeCell.Column)
        //    {
        //        List<int> ids = GetIdsFromList(cellCosts, tCell.Neighbors.Except(visited).ToList());
        //        List<int> costs = new List<int>();
        //        for (int i = 0; i < ids.Count; i++)
        //        {
        //            costs.Add(cellCosts[ids[i]].Item2);
        //        }
        //        int minCellIndex = 0;
        //        Cell minCell = new Cell();
        //        try
        //        {
        //            minCellIndex = costs.IndexOf(costs.Min());
        //            minCell = cellCosts[ids[minCellIndex]].Item1;
        //        }
        //        catch
        //        {

        //        }

        //        // At this point, we've found the neighboring cell with the lowest movement cost
        //        // Now make sure that there isn't a wall in between this cell and the cell we've just found
        //        // with the minimum cost

        //        while (Cell.WallInBetween(minCell, tCell, maze))
        //        {
        //            ids.RemoveAt(minCellIndex);
        //            costs.RemoveAt(minCellIndex);
        //            try
        //            {
        //                minCellIndex = costs.IndexOf(costs.Min());
        //                minCell = cellCosts[ids[minCellIndex]].Item1;
        //            }
        //            catch
        //            {

        //            }
        //        }

        //        // Now we've actually found the neighboring cell with the lowest cost that we're allowed to move to
        //        // Now "move" there, add the cell to the path, and continue moving
        //        tCell = minCell;
        //        visited.Add(minCell);
        //        path.Add(minCell);
        //    }
        //}

        //private List<int> GetIdsFromList(List<Tuple<Cell, int>> costs, List<Cell> input)
        //{
        //    List<int> returnVal = new List<int>();
        //    for (int i = 0; i < input.Count; i++)
        //    {
        //        for (int j = 0; j < costs.Count; j++)
        //        {
        //            if (input[i] == costs[j].Item1)
        //            {
        //                returnVal.Add(j);
        //            }
        //        }
        //    }
        //    return returnVal;
        //}

        //private void GenerateWanderPath(Maze maze)
        //{
        //    Random rand = new Random();

        //    path.Clear();
        //    visited.Clear();

        //    Cell tCell = new Cell(); // Imaginary cell
        //    List<Cell> neighbors = new List<Cell>();
        //    for (int i = 0; i < maze.Cells.Count; i++)
        //    {
        //        if (maze.Cells[i].Row == Cell.Row && maze.Cells[i].Column == Cell.Column)
        //        {
        //            tCell = maze.Cells[i];
        //            neighbors = tCell.Neighbors.Except(neighbors.Where(x => Cell.WallInBetween(tCell, x, maze))).ToList();
        //            break;
        //        }
        //    }

        //    while (tCell.Neighbors.Except(neighbors.Where(x => Cell.WallInBetween(tCell, x, maze))).ToList().Count > 0)
        //    {
        //        tCell = tCell.Neighbors.Except(neighbors.Where(x => Cell.WallInBetween(tCell, x, maze))).ToList()[rand.Next(0, tCell.Neighbors.Except(neighbors.Where(x => Cell.WallInBetween(tCell, x, maze))).ToList().Count)];
        //        path.Add(tCell);
        //        visited.Add(tCell);
        //        neighbors = tCell.Neighbors.Except(visited)
        //            .Except(neighbors.Where(x => Cell.WallInBetween(tCell, x, maze))).ToList();
        //    }
        //}

        #endregion

        #region Moving To Cells

        private void MoveToRandomCell(Maze maze)
        {
            Cell? cell = maze.GetCell(travelingIn, Cell, maze.Cells);
            if (cell != null)
            {
                if (Cell.WallInBetween((Cell)cell, Cell, maze))
                {
                    travelingIn = GetRandomDirection(travelingIn);
                }
                else
                {
                    MoveToCell((Cell)cell);
                }
            }
            else
            {
                travelingIn = GetRandomDirection(travelingIn);
            }
        }
        private Direction GetRandomDirection(Direction exclude)
        {
            List<Direction> availableDirections = Enum.GetValues(typeof(Direction)).OfType<Direction>()
                .Except(new List<Direction> { exclude }).ToList();
            if (availableDirections.Where(x => x != exclude).Count() > 0)
            {
                return availableDirections.Where(x => x != exclude).ToList().Random();
            }
            else
            {
                return availableDirections[0];
            }
        }

        private void MoveToCell(Cell cell)
        {
            isMovingToCell = true;
            Vector2 movingTo = new Vector2(Maze.GetCellCenter(cell).X - Rectangle.Width / 2,
                Maze.GetCellCenter(cell).Y - Rectangle.Height / 2);
            this.movingTo = cell;
            distance = Vector2.Distance(location, movingTo);
            direction = Vector2.Normalize(movingTo - location);
            start = new Vector2(location.X, location.Y);
        }
        private void StepMoveToCell(Maze maze)
        {
            location += direction * SPEED;
            if (Vector2.Distance(start, location) >= distance)
            {
                //if (pathIndex >= path.Count - 1)
                //{
                //    pathIndex = 0;
                //    GenerateWanderPath(maze);
                //}
                //else
                //{
                //    Cell = path[pathIndex];
                //    MoveToCell(path[pathIndex]);
                //}
                Cell = movingTo;
                MoveToRandomCell(maze);
                //MoveToCell(Cell.Neighbors.Except(Cell.Neighbors.Where(x => Cell.WallInBetween(Cell, x, maze)))
                //       .ToList().Random());
            }
            Rectangle.X = (int)location.X;
            Rectangle.Y = (int)location.Y;
        }

        #endregion

        #endregion
    }
}
