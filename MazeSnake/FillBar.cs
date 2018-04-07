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
    class FillBar
    {
        #region Fields & Properties

        Texture2D whiteRectImg;
        Rectangle bgRect;
        Rectangle xpRect;
        float PIXELS_PER_VALUE = 0.0f;
        const int SPACING = 3;
        float value = 0.0f;
        float maxValue = 0;
        SpriteFont font;

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                xpRect.Width = (int)(value * PIXELS_PER_VALUE);
            }
        }
        public float MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                PIXELS_PER_VALUE = bgRect.Width / maxValue;
                xpRect.Width = (int)(value * PIXELS_PER_VALUE);
            }
        }

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                xpRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return bgRect.Y;
            }
            set
            {
                bgRect.Y = value;
                xpRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return bgRect.Width;
            }
            set
            {
                bgRect.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return bgRect.Height;
            }
            set
            {
                bgRect.Height = value;
            }
        }

        #endregion

        #region Constructors

        public FillBar(ContentManager content, string whiteRectAsset, int value, int x, int y, int width, int height, 
            float maxValue, SpriteFont font)
        {
            this.value = value;
            this.font = font;

            whiteRectImg = content.Load<Texture2D>(whiteRectAsset);

            bgRect = new Rectangle(x, y, width, height);
            PIXELS_PER_VALUE = bgRect.Width / maxValue;

            xpRect = new Rectangle(x, y, (int)(value * PIXELS_PER_VALUE), height);
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch, Color xpColor)
        {
            spriteBatch.Draw(whiteRectImg, bgRect, Color.Gray);
            spriteBatch.Draw(whiteRectImg, xpRect, xpColor);
            spriteBatch.DrawString(font, value + "/" + maxValue, new Vector2(bgRect.X, bgRect.Y), Color.White);
        }

        public void IncreaseValue(int amount)
        {
            value += amount;
            xpRect.Width = (int)(value * PIXELS_PER_VALUE);
        }

        #endregion
    }
}
