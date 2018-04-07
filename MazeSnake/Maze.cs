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
    public class Maze
    {
        #region Fields & Properties

        public List<Wall> Walls = new List<Wall>();
        public List<PowerUp> PowerUps = new List<PowerUp>();
        public Point StartPos = new Point();
        public Point EndPos = new Point();
        public bool LevelReady = false;
        public bool ShowingLoadingScreen = false;

        public int Step = 0;

        public int Coins = 0;

        //------------------------------------------------------------------------------------------------------------------------//

        const int MAX_POWERUPS_PER_LEVEL = 3;
        const int MAX_COINS_PER_LEVEL = 5;
        const int MAX_ENEMIES_PER_LEVEL = 3;

        Snake snake;
        Turtle turtle;

        List<Coin> coins = new List<Coin>();

        public List<Cell> Cells = new List<Cell>();
        List<Cell> unvisitedCells = new List<Cell>();
        List<Cell> visitedCells = new List<Cell>();

        const int CELL_SIZE = 50;
        const int SMALL_WALL_SIDE = 4;
        const int ROWS = 12;
        const int COLUMNS = 17;
        const int CELLS = ROWS * COLUMNS;
        const int ROW_SIZE = CELL_SIZE - (SMALL_WALL_SIDE / 2);

        const int POWERUP_SIZE = 30;
        const int COIN_SIZE = CELL_SIZE - (SMALL_WALL_SIDE * 4);
        const int ENEMY_SIZE = CELL_SIZE - (SMALL_WALL_SIDE * 6);

        const int GOTO_COIN_X = 477;
        const int GOTO_COIN_Y = 209;

        bool isMazeWorm = false;

        string explosionStripAsset = "";

        Vector2 addCoinLocation = new Vector2();
        string addCoinText = "";
        Color addCoinColor = Color.DarkGoldenrod;
        Timer addCoinTimer;
        bool showingAddCoin = false;

        Random rand;
        int windowWidth;
        int windowHeight;

        LoadingScreen loadingScreen;

        Texture2D wallImage;
        Texture2D powerUpImage;

        // Fin = finish
        Texture2D finImg;
        Rectangle finRect;

        SpriteFont font;

        List<Enemy> enemies = new List<Enemy>();

        string coinSprite;
        string enemySprite;

        event Action LevelComplete;

        public GameMode GameMode;

        Texture2D starImg;
        List<Rectangle> stars = new List<Rectangle>();
        const int STARS_PER_LEVEL = 7;
        const int STAR_SIZE = 35;

        #endregion

        #region Constructors

        public Maze(Snake snake, ContentManager content, string progressBarImage, string progressImageName, string smallFontName,
            string backgroundImageName, int windowWidth, int windowHeight, Skin skin, Random rand, string bigFontName,
            bool isMazeWorm, string finImgString, string powerUpSprite, string titleAsset, string tongueAsset, Color snakeColor,
            string explosionStripAsset, SpriteFont font, string enemySprite, string coinSprite, string starSprite)
        {
            this.snake = snake;
            this.rand = rand;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.isMazeWorm = isMazeWorm;
            this.explosionStripAsset = explosionStripAsset;
            this.font = font;
            this.coinSprite = coinSprite;
            this.enemySprite = enemySprite;

            turtle = new Turtle(0, 0, new Cell(0, 0), ROWS, COLUMNS);

            loadingScreen = new LoadingScreen(content, progressBarImage, progressImageName, smallFontName, backgroundImageName,
                windowWidth, windowHeight, skin, rand, bigFontName, isMazeWorm, 5000, titleAsset, tongueAsset, snakeColor);

            finImg = content.Load<Texture2D>(finImgString);
            finRect = new Rectangle(0, InventoryInterface.BG_HEIGHT, CELL_SIZE - SMALL_WALL_SIDE, CELL_SIZE - SMALL_WALL_SIDE);

            powerUpImage = content.Load<Texture2D>(powerUpSprite);

            addCoinTimer = new Timer(1, TimerUnits.Seconds);

            starImg = content.Load<Texture2D>(starSprite);
        }

        #endregion

        #region Public Methods

        public void GenerateRandomLevel(GameMode gameMode)
        {
            GameMode = gameMode;
            // Set LevelReady to false because the level isn't ready to be used
            LevelReady = false;
            // Tells update that level is generating.
            Step = 1;
            ShowingLoadingScreen = true;
            // Reset in case another level has already been generated
            loadingScreen.SetPercentage(0);
            Walls.Clear();
            PowerUps.Clear();
            coins.Clear();
            enemies.Clear();
            snake.Reset();
            unvisitedCells.Clear();
            Cells.Clear();
            visitedCells.Clear();
            // Set StartPos and EndPos
            EndPos = new Point(0, 0);
            StartPos = new Point(windowWidth - snake.Rectangle.Width, windowHeight - snake.Rectangle.Height);
            snake.X = StartPos.X;
            snake.Y = StartPos.Y;
            Enemy.ResetId();
        }

        public void Update(GameTime gameTime, Texture2D wallImage, ContentManager content,
            int windowWidth, int windowHeight, ref User user, bool updateSnake)
        {
            #region Regular Updating

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            if (!LevelReady)
            {
                loadingScreen.Update(gameTime);
            }

            for (int i = 0; i < Walls.Count; i++)
            {
                if (Walls[i].Exploding)
                {
                    Walls[i].Update(gameTime, rand);
                }
                if (Walls[i].Exploded)
                {
                    Walls.RemoveAt(i);
                }
            }

            if (showingAddCoin)
            {
                addCoinTimer.Update(gameTime);
                addCoinLocation.Y -= 0.5f;
                if (addCoinTimer.QueryWaitTime(gameTime))
                {
                    showingAddCoin = false;
                }
            }

            if (snake.Rectangle.Intersects(finRect) && GameMode != GameMode.Star)
            {
                Rectangle snakeRect = snake.Rectangle;
                snakeRect.X = StartPos.X;
                snakeRect.Y = StartPos.Y;
                snake.ChangeRectangle(snakeRect);
                LevelComplete?.Invoke();
            }

            if (GameMode == GameMode.Star)
            {
                for (int i = 0; i < stars.Count; i++)
                {
                    if (snake.Rectangle.Intersects(stars[i]))
                    {
                        Sound.PlaySound(Sounds.Star);
                        stars.RemoveAt(i);
                        if (stars.Count == 0)
                        {
                            Rectangle snakeRect = snake.Rectangle;
                            snakeRect.X = StartPos.X;
                            snakeRect.Y = StartPos.Y;
                            snake.ChangeRectangle(snakeRect);
                            LevelComplete?.Invoke();
                        }
                    }
                }
            }

            if (LevelReady && updateSnake)
            {
                snake.Update(gameTime, PowerUps, Walls, rand, windowWidth, windowHeight, ref user, enemies);
            }

            if (LevelReady)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(this);
                }
            }

            foreach (PowerUp p in PowerUps)
            {
                p.Update(windowWidth, windowHeight, Walls, gameTime);
                if (!p.IsPickedUp)
                {
                    if (snake.Rectangle.Intersects(p.Rectangle) && p.Type == PowerupType.Restart)
                    {
                        snake.X = StartPos.X;
                        snake.Y = StartPos.Y;
                        user.AddToStat(Stat.PowerupsCollected, 1);
                        user.AddToStat(Stat.TimesTeleported, 1);
                        Sound.PlaySound(Sounds.Teleport);
                        p.Collect(windowWidth, windowHeight);
                    }
                }
            }

            foreach (Coin c in coins)
            {
                if (!c.IsPickedUp)
                {
                    c.Update(Walls, windowWidth, windowHeight);
                    if (snake.Rectangle.Intersects(c.DrawRectangle))
                    {
                        // Coin has been picked up; increase coin value
                        addCoinLocation = new Vector2(c.DrawRectangle.X,
                            c.DrawRectangle.Y + (int)font.MeasureString("+" + c.CoinValue).Y);
                        addCoinText = "+" + c.CoinValue;
                        user.AddCoins(1);
                        Sound.PlaySound(Sounds.Coin);
                        user.AddToStat(Stat.CoinsCollected, 1);
                        showingAddCoin = true;

                        c.IsPickedUp = true;
                    }
                }
            }

            this.wallImage = wallImage;

            #endregion

            #region Maze Generation

            // Uses random maze algorithms to ensure that the maze is random
            switch (Step)
            {
                case 1:
                    // Part 1: Create a grid to start with
                    #region Create Grid

                    // Start by drawing all the walls up and down.
                    for (int i = 0; i < windowWidth; i++)
                    {
                        for (int j = 1; j < windowHeight; j++)
                        {
                            int x = j * ROW_SIZE;
                            int y = (i * ROW_SIZE) + InventoryInterface.BG_HEIGHT;

                            if (x > windowWidth || y > windowHeight)
                            {
                                continue;
                            }

                            Walls.Add(new Wall(x, y, wallImage, SMALL_WALL_SIDE, CELL_SIZE, true, j, i, Direction2.UpDown,
                                explosionStripAsset, content));
                        }
                    }
                    // Then draw the walls across
                    for (int i = 1; i < windowWidth; i++)
                    {
                        for (int j = 0; j < windowHeight; j++)
                        {
                            int x = j * ROW_SIZE;
                            int y = (i * ROW_SIZE) + InventoryInterface.BG_HEIGHT;

                            if (x > windowWidth || y > windowHeight || y < InventoryInterface.BG_HEIGHT)
                            {
                                continue;
                            }

                            Walls.Add(new Wall(x, y, wallImage, CELL_SIZE, SMALL_WALL_SIDE, true, j, i, Direction2.LeftRight, explosionStripAsset, content));
                        }
                    }

                    // Initialize our unvisited cells list
                    for (int i = 0; i < ROWS; i++)
                    {
                        for (int j = 0; j < COLUMNS; j++)
                        {
                            unvisitedCells.Add(new Cell(i, j));
                        }
                    }
                    // Initialize cell neighbors
                    foreach (Cell c in unvisitedCells)
                    {
                        foreach (Cell n in unvisitedCells)
                        {
                            if (c.IsNeighbor(n))
                            {
                                // The "n" cell is a neighbor of "c"
                                c.Neighbors.Add(n);
                            }
                        }
                    }
                    // Currently, all the cells are unvisited, so we must set our cells list to all of the unvisited ones (this is efficient as it prevents
                    // additional calculations when initializing unvisitedCells
                    Cells = unvisitedCells.Clone();

                    // Set the turtle's initial cell
                    turtle.Row = 0;
                    turtle.Column = 0;
                    for (int i = 0; i < unvisitedCells.Count; i++)
                    {
                        if (unvisitedCells[i].Row == turtle.Row && unvisitedCells[i].Column == turtle.Column)
                        {
                            turtle.Cell = unvisitedCells[i];
                            visitedCells.Add(unvisitedCells[i]);
                            unvisitedCells.RemoveAt(i);
                            break;
                        }
                    }

                    // Increase step
                    Step++;

                    #endregion
                    break;
                case 2:
                    // Part 2: Generate the actual maze
                    #region Generate Maze

                    GenerateHuntAndKill(content, coinSprite);

                    loadingScreen.AddPercentage(1);

                    #endregion
                    break;
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw walls
            foreach (Wall w in Walls)
            {
                if (w.Active)
                {
                    w.Draw(spriteBatch);
                }
            }

            // Draw powerups and coins
            foreach (PowerUp p in PowerUps)
            {
                if (!p.IsPickedUp || p.ShowingDisplay)
                {
                    p.Draw(spriteBatch);
                }
            }

            foreach (Coin c in coins)
            {
                if (!c.IsPickedUp)
                {
                    c.Draw(spriteBatch);
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            if (GameMode == GameMode.Star)
            {
                for (int i = 0; i < stars.Count; i++)
                {
                    spriteBatch.Draw(starImg, stars[i], Color.White);
                }
            }
            else
            {
                // Draw finish
                spriteBatch.Draw(finImg, finRect, Color.White);
            }

            if (!LevelReady)
            {
                loadingScreen.Draw(spriteBatch);
            }

            if (showingAddCoin && LevelReady)
            {
                spriteBatch.DrawString(font, addCoinText, addCoinLocation, addCoinColor);
            }
        }

        public void AddEnemyHitHandler(Action handler)
        {
            snake.AddEnemyCollisionHandler(handler);
        }
        public void AddLevelCompleteHandler(Action handler)
        {
            LevelComplete += handler;
        }

        private Cell? GetCell(Direction dir, List<Cell> cells)
        {
            return GetCell(dir, new Cell(turtle.Row, turtle.Column), cells);
        }
        public Cell? GetCell(Direction dir, Cell from, List<Cell> cells)
        {
            Cell cell = new Cell();
            switch (dir)
            {
                case Direction.Up:
                    cell.Row = from.Row - 1;
                    cell.Column = from.Column;
                    break;
                case Direction.Down:
                    cell.Row = from.Row + 1;
                    cell.Column = from.Column;
                    break;
                case Direction.Left:
                    cell.Row = from.Row;
                    cell.Column = from.Column - 1;
                    break;
                case Direction.Right:
                    cell.Row = from.Row;
                    cell.Column = from.Column + 1;
                    break;
            }

            foreach (Cell c in cells)
            {
                if (c.Column == cell.Column && c.Row == cell.Row)
                {
                    return c;
                }
            }

            return null;
        }

        public int SelectWall(Direction dir)
        {
            int row = 0;
            int column = 0;
            Direction2 dir2 = Direction2.LeftRight;

            switch (dir)
            {
                case Direction.Up:
                    dir2 = Direction2.LeftRight;
                    row = turtle.Row;
                    column = turtle.Column;
                    break;
                case Direction.Down:
                    dir2 = Direction2.LeftRight;
                    row = turtle.Row + 1;
                    column = turtle.Column;
                    break;
                case Direction.Left:
                    dir2 = Direction2.UpDown;
                    row = turtle.Row;
                    column = turtle.Column;
                    break;
                case Direction.Right:
                    dir2 = Direction2.UpDown;
                    row = turtle.Row;
                    column = turtle.Column + 1;
                    break;
            }

            int index = -1;

            for (int i = 0; i < Walls.Count; i++)
            {
                if (Walls[i].Column == column && Walls[i].Row == row && Walls[i].Direction == dir2)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static Point GetCellCenter(Cell cell)
        {
            return new Rectangle(cell.Column * ROW_SIZE, cell.Row * ROW_SIZE + InventoryInterface.BG_HEIGHT, ROW_SIZE, ROW_SIZE).Center;
        }

        public static Cell GetCellFromLocation(Vector2 location)
        {
            int row = 0;
            int col = 0;

            col = (int)Math.Floor(location.X / ROW_SIZE);
            row = (int)Math.Floor((location.Y - InventoryInterface.BG_HEIGHT) / ROW_SIZE);

            return new Cell(row, col);
        }
        public static Cell GetCellFromLocation(int x, int y)
        {
            return GetCellFromLocation(new Vector2(x, y));
        }

        #endregion

        #region Private Methods

        private void GenerateHuntAndKill(ContentManager content, string coinSprite)
        {
            //  Hunt-And-Kill Algorithm
            // 1.) Start Turtle at (0, 0).
            // 2.) Randomly move the Turtle in different directions until you reach a "dead end".
            // 3.) If the turtle reaches a dead end (all neighbors are visited), search the rows for the
            //     first cell with a visited neighbor.
            // 4.) Upon finding this cell, carve a path between the two. Then, starting from the originally found cell, 
            //     continue randomly carving
            // 5.) Repeat until you reach a dead end and all cells are visited.
            // 6.) During the maze, randomly add a coin and powerups.

            if (IsDeadEnd())
            {
                if (unvisitedCells.Count <= 0)
                {
                    // There are no unvisited cells, so we can start playing the level
                    LevelReady = true;
                    ShowingLoadingScreen = false;
                    Step++;
                    AddCoinsAndPowerups(content, coinSprite);
                    if (GameMode != GameMode.Freeplay && GameMode != GameMode.Star)
                    {
                        AddEnemies(content, enemySprite);
                    }
                    if (GameMode == GameMode.Star)
                    {
                        AddStars();
                    }
                    return;
                }
                else
                {
                    // We simply set the turtle's position to the first available unvisited cell
                    ScanAndSetNewLocation();
                }
            }
            else
            {
                if (!RandomlyMoveTurtle())
                {
                    // Unfortunately, we were unable to move, as all directions were taken.
                    ScanAndSetNewLocation();
                }
            }
        }
        //private void GenerateBinaryTree(ContentManager content, string coinSprite)
        //{
        //    // Binary Tree Algoritm Steps
        //    // 1.) Start with grid
        //    // 2.) Start turtle at (0, 0)
        //    // 3.) Carve either right or down
        //    // 4.) Continue throughout the rows

        //    // Check if maze is finished generating
        //    if (turtle.Row == ROWS - 1 && turtle.Column == COLUMNS - 1)
        //    {
        //        // Maze is done generating
        //        LevelReady = true;
        //        Step++;
        //        ShowingLoadingScreen = false;
        //        AddCoinsAndPowerups(content, coinSprite);
        //        AddEnemies(content, enemySprite);
        //        return;
        //    }

        //    // Carve either right, or down
        //    Direction carveDir;
        //    if (turtle.Row == ROWS - 1)
        //    {
        //        // We are in the bottom row; carve right
        //        carveDir = Direction.Right;
        //    }
        //    else if (turtle.Column == COLUMNS - 1)
        //    {
        //        // We are in the right-most row; carve down
        //        carveDir = Direction.Down;
        //    }
        //    else
        //    {
        //        if (rand.Next(2) == 0)
        //        {
        //            carveDir = Direction.Down;
        //        }
        //        else
        //        {
        //            carveDir = Direction.Right;
        //        }
        //    }
        //    Walls.RemoveAt(SelectWall(carveDir));

        //    if (turtle.Column == COLUMNS - 1)
        //    {
        //        turtle.Move(Direction.Down);
        //        turtle.Column = 0;
        //    }
        //    else
        //    {
        //        turtle.Move(Direction.Right);
        //    }
        //}

        private void AddCoinsAndPowerups(ContentManager content, string coinSprite)
        {
            // Add coins, if we are not in freeplay mode
            if (GameMode != GameMode.Freeplay)
            {
                int coinsPerLevel = rand.Next(MAX_COINS_PER_LEVEL + 1);
                int coinX;
                int coinY;
                for (int i = 0; i < coinsPerLevel; i++)
                {
                    do
                    {
                        coinX = rand.Next(windowWidth);
                        coinX -= (coinX % ROW_SIZE) - SMALL_WALL_SIDE;
                        coinY = rand.Next(InventoryInterface.BG_HEIGHT, windowHeight);
                        coinY -= (coinY % ROW_SIZE) - (SMALL_WALL_SIDE * 2);
                    }
                    while (snake.Rectangle.Intersects(new Rectangle(coinX, coinY, COIN_SIZE, COIN_SIZE)));

                    coins.Add(new Coin(coinSprite, content, coinX, coinY, COIN_SIZE, COIN_SIZE, 1));
                }
            }

            // Add powerups
            int powerupsPerLevel = rand.Next(MAX_POWERUPS_PER_LEVEL + 1);
            int pwX;
            int pwY;
            for (int i = 0; i < powerupsPerLevel; i++)
            {
                do
                {
                    pwX = rand.Next(windowWidth);
                    pwX -= (pwX % ROW_SIZE) - SMALL_WALL_SIDE;
                    pwY = rand.Next(InventoryInterface.BG_HEIGHT, windowHeight);
                    pwY -= (pwY % ROW_SIZE) - (SMALL_WALL_SIDE * 2);
                }
                while (snake.Rectangle.Intersects(new Rectangle(pwX, pwY, POWERUP_SIZE, POWERUP_SIZE)));

                PowerupType type = PowerupType.Speed;
                do
                {
                    type = PowerUp.GetRandomType(rand);
                }
                while (((type == PowerupType.Frozen || type == PowerupType.Forcefield) && GameMode == GameMode.Freeplay) || 
                    ((type == PowerupType.Forcefield) && GameMode == GameMode.Star));

                PowerUps.Add(new PowerUp(powerUpImage, pwX, pwY, snake, POWERUP_SIZE, POWERUP_SIZE, type, font));
            }
        }
        private void AddEnemies(ContentManager content, string enemyAsset)
        {
            int enemyCount = rand.Next(MAX_ENEMIES_PER_LEVEL + 1);
            int eX, eY;
            Texture2D enemyImg = content.Load<Texture2D>(enemyAsset);
            for (int i = 0; i < enemyCount; i++)
            {
                do
                {
                    eX = rand.Next(windowWidth);
                    eX -= (eX % ROW_SIZE) - SMALL_WALL_SIDE;
                    eY = rand.Next(InventoryInterface.BG_HEIGHT, windowHeight);
                    eY -= (eY % ROW_SIZE) - (SMALL_WALL_SIDE * 2);
                }
                while (snake.Rectangle.Intersects(new Rectangle(eX, eY, ENEMY_SIZE, ENEMY_SIZE)));

                enemies.Add(new Enemy(enemyImg, eX, eY, GetCellFromLocation(eX, eY).Row, GetCellFromLocation(eX, eY).Column,
                    ENEMY_SIZE, ENEMY_SIZE));
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Start(this);
            }
        }

        private void AddStars()
        {
            int x, y;
            List<Vector2> takenLocs = new List<Vector2>();
            for (int i = 0; i < STARS_PER_LEVEL; i++)
            {
                do
                {
                    x = rand.Next(windowWidth);
                    x -= (x % ROW_SIZE) - SMALL_WALL_SIDE;
                    y = rand.Next(InventoryInterface.BG_HEIGHT, windowHeight);
                    y -= (y % ROW_SIZE) - (SMALL_WALL_SIDE * 2);
                }
                while (snake.Rectangle.Intersects(new Rectangle(x, y, ENEMY_SIZE, ENEMY_SIZE)) && takenLocs.Contains(new Vector2(x, y)));

                takenLocs.Add(new Vector2(x, y));
                stars.Add(new Rectangle(x, y, STAR_SIZE, STAR_SIZE));
            }
        }

        private bool IsDeadEnd()
        {
            foreach (Cell n in turtle.Cell.Neighbors)
            {
                foreach (Cell c in unvisitedCells)
                {
                    if (n.MemberEquals(c))
                    {
                        // The turtle still has at least one unvisited neighbor.
                        return false;
                    }
                    // If the above if statement is false, the cell we have tested is NOT unvisited
                }
            }
            return true;
        }

        /// <summary>
        /// Will move the turtle in a random direction and carve a path there
        /// </summary>
        /// <returns>Whether or not the move was successful</returns>
        private bool RandomlyMoveTurtle()
        {
            // This makes sure that the turtle does not attempt to remove a wall that is already gone
            HashSet<Direction> unallowedDirValues = new HashSet<Direction>();
            for (int i = 0; i < Enum.GetNames(typeof(Direction)).ToList().Count; i++)
            {
                if (SelectWall((Direction)i) == -1)
                {
                    unallowedDirValues.Add((Direction)i);
                }
                else if (GetCell((Direction)i, unvisitedCells) == null)
                {
                    unallowedDirValues.Add((Direction)i);
                }
            }
            Direction? turtleMoveDir = turtle.PickRandomDirection(rand, unallowedDirValues);
            if (turtleMoveDir == null)
            {
                return false;
            }

            int index = SelectWall(turtleMoveDir ?? default(Direction));
            Walls.RemoveAt(index);
            turtle.Move(turtleMoveDir ?? default(Direction));
            // Set the turtle's cell value
            for (int i = 0; i < unvisitedCells.Count; i++)
            {
                if (unvisitedCells[i].Row == turtle.Row && unvisitedCells[i].Column == turtle.Column)
                {
                    turtle.Cell = unvisitedCells[i];
                    visitedCells.Add(unvisitedCells[i]);
                    unvisitedCells.RemoveAt(i);
                    break;
                }
            }
            return true;
        }

        private void ScanAndSetNewLocation()
        {
            bool cHasVisitedNeighbor = false;
            Cell neighbor = new Cell();
            foreach (Cell c in unvisitedCells)
            {
                foreach (Cell n in c.Neighbors)
                {
                    if (visitedCells.Contains(n))
                    {
                        // "c" has a neighbor which has been visited before.
                        cHasVisitedNeighbor = true;
                        neighbor = n;
                        break;
                    }
                }
                if (cHasVisitedNeighbor)
                {
                    turtle.Cell = c;
                    turtle.Row = c.Row;
                    turtle.Column = c.Column;
                    unvisitedCells.Remove(c);
                    visitedCells.Add(c);

                    // Carve a passage from "c" and the current cell
                    if (neighbor.Row < c.Row)
                    {
                        Walls.RemoveAt(SelectWall(Direction.Up));
                    }
                    else if (neighbor.Row > c.Row)
                    {
                        Walls.RemoveAt(SelectWall(Direction.Down));
                    }
                    else if (neighbor.Column < c.Column)
                    {
                        Walls.RemoveAt(SelectWall(Direction.Left));
                    }
                    else if (neighbor.Column > c.Column)
                    {
                        Walls.RemoveAt(SelectWall(Direction.Right));
                    }

                    return;
                }
            }
        }

        #endregion
    }
}
