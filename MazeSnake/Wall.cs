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
    public class Wall
    {
        #region Fields

        Texture2D image;
        Rectangle drawRectangle;

        Timer animationTimer;
        public bool Exploding = false;
        public bool Exploded = false;
        Texture2D explosionStrip;
        Rectangle explosionRect;
        Rectangle sourceRect;

        const int FRAMES_PER_ROW = 5;
        const int ROWS = 5;
        const int ROW_SIZE = 65;
        const int COLUMN_SIZE = 64;
        int row = 0;
        int column = 0;

        #endregion

        #region Constructors

        public Wall(int x, int y, Texture2D sprite, int width, int height, string explosionStripString, ContentManager content)
        {
            this.image = sprite;
            drawRectangle = new Rectangle(x, y, width, height);
            animationTimer = new Timer(10, TimerUnits.Milliseconds);
            explosionStrip = content.Load<Texture2D>(explosionStripString);
            if (width > height)
            {
                explosionRect = new Rectangle(x, y - (width / 2), width, width);
            }
            else
            {
                explosionRect = new Rectangle(x - (height / 2), y, height, height);
            }
            sourceRect = new Rectangle(0, 0, ROW_SIZE, COLUMN_SIZE);
        }

        public Wall(int x, int y, Texture2D sprite, int width, int height, bool active, string explosionStripString, ContentManager content)
            : this(x, y, sprite, width, height, explosionStripString, content)
        {
            this.Active = active;
        }

        public Wall(int x, int y, Texture2D sprite, int width, int height, bool active, int column, int row, Direction2 direction2, string explosionStripString,
            ContentManager content)
            : this(x, y, sprite, width, height, active, explosionStripString, content)
        {
            Row = row;
            Column = column;
            this.Direction = direction2;
        }
        #endregion

        #region Public Methods

        public void Update(GameTime gameTime, Random rand)
        {
            if (Exploding)
            {
                animationTimer.Update(gameTime);

                if (animationTimer.QueryWaitTime(gameTime))
                {
                    if (column < FRAMES_PER_ROW - 1)
                    {
                        // We are NOT on the last frame of the column
                        sourceRect.X += COLUMN_SIZE;
                        column++;
                    }
                    else
                    {
                        // We are on the last frame of the column
                        if (row >= ROWS - 1 && column >= FRAMES_PER_ROW - 1)
                        {
                            // We are displaying the last image of the sprite sheet,
                            // and are no longer exploding
                            this.Exploding = false;
                            this.Exploded = true;
                            return;
                        }
                        // Go to the first image of the next column
                        column = 0;
                        row++;
                        sourceRect.X = 0;
                        sourceRect.Y += ROW_SIZE;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color shade)
        {
            if (!(image == null || drawRectangle == null) && !(Exploded || Exploding))
            {
                spriteBatch.Draw(image, drawRectangle, shade);
            }
            if (Exploding)
            {
                spriteBatch.Draw(explosionStrip, explosionRect, sourceRect, Color.White);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            color.R = 0;
            color.G = 60;
            color.B = 0;
            this.Draw(spriteBatch, color);
        }

        public void Explode()
        {
            Exploding = true;
            Sound.PlaySound(Sounds.Explosion);
        }

        public static Wall DoWallsConnect(Wall wall, List<Wall> walls, Texture2D wallImage)
        {
            Wall nextToWall = null;

            foreach (Wall w in walls)
            {
                // Check if the walls are touching each other.
                if ((wall.GetRectangle.Right == w.GetRectangle.Left) || (wall.GetRectangle.Left == w.GetRectangle.Right) ||
                    (wall.GetRectangle.Bottom == w.GetRectangle.Top) || (wall.GetRectangle.Top == w.GetRectangle.Bottom))
                {
                    // Walls are connected, return new wall
                    nextToWall = w;
                }
            }

            return nextToWall;
        }

        #endregion

        #region Properties
        public Rectangle GetRectangle
        {
            get
            {
                return drawRectangle;
            }
        }
        public int X
        {
            get
            {
                return drawRectangle.X;
            }
            set
            {
                drawRectangle.X = value;
            }
        }
        public int Y
        {
            get
            {
                return drawRectangle.Y;
            }
            set
            {
                drawRectangle.Y = value;
            }
        }

        public int Width
        {
            get
            {
                return drawRectangle.Width;
            }
            set
            {
                drawRectangle.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return drawRectangle.Height;
            }
            set
            {
                drawRectangle.Height = value;
            }
        }
        public int Row { get; set; }
        public int Column { get; set; }
        public Direction2 Direction { get; set; }
        public bool Active { get; set; }
        #endregion
    }
    #region Enumerations

    public enum Direction2
    {
        UpDown,
        LeftRight
    }

    #endregion
}