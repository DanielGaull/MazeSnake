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
    class Coin
    {
        #region Fields

        Texture2D image;
        Rectangle drawRectangle;

        public int CoinValue = 1;

        bool inMotion = false;
        bool clamped = false;

        #endregion

        #region Properties

        public Rectangle DrawRectangle 
        { 
            get
            {
                return drawRectangle;
            }
            set
            {
                drawRectangle = value;
            }
        }

        public bool IsPickedUp { get; set; }

        public bool InMotion 
        {
            get
            {
                return inMotion;
            }
        }

        #endregion

        #region Constructor

        public Coin(string coinSprite, ContentManager content, int x, int y)
        {
            image = content.Load<Texture2D>(coinSprite);
            drawRectangle = new Rectangle(x, y, image.Width, image.Height);
        }

        public Coin(string coinSprite, ContentManager content, int x, int y, int width, int height, int coinValue)
            : this(coinSprite, content, x, y)
        {
            this.drawRectangle.Width = width;
            this.drawRectangle.Height = height;
            this.CoinValue = coinValue;
        }

        #endregion

        #region Public Methods

        public void Update(List<Wall> walls, int windowWidth, int windowHeight)
        {
            // Reset clamping variable
            clamped = false;

            // Clamp the coin so it cannot go out of the window
            if (drawRectangle.X <= 0)
            {
                drawRectangle.X = 0;
                clamped = true;
            }
            if (drawRectangle.X >= windowWidth - drawRectangle.Width)
            {
                drawRectangle.X = windowWidth - drawRectangle.Width;
                clamped = true;
            }
            if (drawRectangle.Y <= 0)
            {
                drawRectangle.Y = 0;
                clamped = true;
            }
            if (drawRectangle.Y >= windowHeight - drawRectangle.Height)
            {
                drawRectangle.Y = windowHeight - drawRectangle.Height;
                clamped = true;
            }

            // Make sure the coin isn't stuck in any walls
            foreach (Wall w in walls)
            {
                if (drawRectangle.Intersects(w.GetRectangle))
                {
                    inMotion = true;
                    drawRectangle.X++;
                    drawRectangle.Y++;
                }
                else
                {
                    inMotion = false;
                }

                // If this is true, we're stuck between wall & edge of window; remove coin
                if (drawRectangle.Intersects(w.GetRectangle) && clamped)
                {
                    IsPickedUp = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, drawRectangle, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Color shade)
        {
            spriteBatch.Draw(image, drawRectangle, shade);
        }

        #endregion
    }
}
