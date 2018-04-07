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
    class ValueSlider
    {
        #region Fields & Properties

        public float Value = 0.0f;
        public string ValueName = "";
        public bool ShowName = true;

        Texture2D image;
        Rectangle bgRect;
        Rectangle sliderRect;
        const int SLIDER_WIDTH = 30;
        const int SPACING = 4;

        // The x value to increase by for every 1 added to value
        float PIXELS_PER_VALUE = 0.0f;

        SpriteFont font;

        MouseState mouse;
        bool sliding = false;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                sliderRect.X = (int)(value * PIXELS_PER_VALUE) + bgRect.X - (sliderRect.Width / 2);
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
                sliderRect.Y = value - (sliderRect.Height - (SPACING / 2));
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
                PIXELS_PER_VALUE = value / 100.0f;
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

        public ValueSlider(int value, string whiteRectAsset, int x, int y, ContentManager content, int width, int height, SpriteFont valueFont, string valueName)
        {
            this.Value = value;
            this.font = valueFont;
            if (valueName.Length > 0 && valueName != null) // Empty or null string
            {
                this.ValueName = valueName;
            }
            else
            {
                this.ValueName = "Value";
            }
            bgRect = new Rectangle(x, y, width, height);
            PIXELS_PER_VALUE = bgRect.Width / 100.0f;

            sliderRect = new Rectangle(0, 0, SLIDER_WIDTH, height + SPACING);
            sliderRect.X = ((int)(value * PIXELS_PER_VALUE) + bgRect.X) - (sliderRect.Width / 2);
            sliderRect.Y = y - (SPACING / 2);

            image = content.Load<Texture2D>(whiteRectAsset);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // Check for a click and update Value accordingly
            mouse = Mouse.GetState();

            // If true, the mouse is currently clicked over the slider rectangle
            sliding = (bgRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) || sliderRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1))) 
                && mouse.LeftButton == ButtonState.Pressed;

            if (sliding)
            {
                // We have to let the user drag the slider
                sliderRect.X = mouse.X - (sliderRect.Width / 2);
            }

            // We must clamp the sliderRect position
            if (sliderRect.X + (sliderRect.Width / 2) < bgRect.X)
            {
                sliderRect.X = bgRect.X - (sliderRect.Width / 2);
            }
            else if (sliderRect.X + (sliderRect.Width / 2) > bgRect.X + bgRect.Width)
            {
                // The slider is at the end of the allowed value
                sliderRect.X = (bgRect.X + bgRect.Width) - (sliderRect.Width / 2);
            }

            // Finally, we must edit the value based on the slider's position
            Value = ((sliderRect.X + (sliderRect.Width / 2)) - bgRect.X) /* the center of the slider */ / PIXELS_PER_VALUE;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, bgRect, Color.Blue);
            spriteBatch.Draw(image, sliderRect, Color.Orange);
            spriteBatch.DrawString(font, ValueName + ": " + ((int)Value).ToString(), new Vector2(bgRect.X, sliderRect.Y - (SPACING * 5)), Color.Black);
        }

        #endregion
    }
}
