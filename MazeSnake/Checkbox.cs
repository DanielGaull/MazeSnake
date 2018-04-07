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
    class Checkbox
    {
        #region Fields & Properties

        public string Text = "";

        Texture2D image;
        Texture2D checkImage;
        Rectangle drawRectangle;
        Rectangle checkRectangle;

        SpriteFont font;

        MouseState mouse;

        const int SPACING_X = 10;

        const int SIZE = 20;

        bool selected = false;

        bool clickStarted = false;
        bool buttonReleased = true;

        public bool IsChecked { get; set; }

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
                return drawRectangle.Width + (int)font.MeasureString(Text).X;
            }
        }
        public int Height
        {
            get
            {
                return drawRectangle.Height + (int)font.MeasureString(Text).Y;
            }
        }

        #endregion

        #region Constructors
        public Checkbox(string message, ContentManager content, SpriteFont font, string checkboxImageName, string checkImageName)
        {
            this.Text = message;

            this.font = font;
            image = content.Load<Texture2D>(checkboxImageName);
            checkImage = content.Load<Texture2D>(checkImageName);

            drawRectangle = new Rectangle(0, 0, SIZE, SIZE);
            checkRectangle = new Rectangle(0, 0, SIZE, SIZE);
        }
        public Checkbox(string message, bool isChecked, ContentManager content, SpriteFont font, string checkboxImageName, string checkImageName)
            : this(message, content, font, checkboxImageName, checkImageName)
        {
            this.IsChecked = isChecked;
        }
        #endregion

        #region Public Methods
        public void Update()
        {
            mouse = Mouse.GetState();

            //click processing to make a realistic checkbox
            selected = (drawRectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1)));
            if (selected && mouse.LeftButton == ButtonState.Pressed)
            {
                clickStarted = true;
                buttonReleased = false;
            }
            else if (!(mouse.LeftButton == ButtonState.Pressed) && selected)
            {
                buttonReleased = true;
            }

            if (clickStarted && buttonReleased)
            {
                IsChecked = !IsChecked;
                clickStarted = false;
                buttonReleased = true;
            }

            checkRectangle.X = drawRectangle.Center.X - SPACING_X;
            checkRectangle.Y = drawRectangle.Y - (SPACING_X / 2);
        }
        public void Draw(SpriteBatch spriteBatch, Color textColor)
        {
            spriteBatch.Draw(image, drawRectangle, Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(drawRectangle.Right + SPACING_X, drawRectangle.Y), textColor);
            if (IsChecked)
            {
                spriteBatch.Draw(checkImage, checkRectangle, Color.White);
            }

        }
        #endregion
    }
}
