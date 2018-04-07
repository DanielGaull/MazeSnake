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
    public delegate void OnButtonClick();
    public delegate void OnButtonHover();
    public class MenuButton
    {
        #region Fields & Properties

        Texture2D bgImage;
        Rectangle drawRectangle;
        const string ASSET_NAME = "whiterectangle";
        Texture2D buttonImage;
        Rectangle buttonRectangle;

        ButtonType buttonType;

        const int TEXT_SPACING_WIDTH = 5;
        const int TEXT_SPACING_HEIGHT = 5;

        MouseState mouse;

        Color drawColor = Color.Blue;
        SpriteFont font;

        string text;
        bool isSelected = false;
        Vector2 textLoc = new Vector2();

        OnButtonClick onButtonClick;

        const int SPACING_Y = 10;
        const int SPACING_X = 10;

        bool clickStarted = false;
        bool buttonReleased = true;

        bool hasBackground = true;

        public bool Active = true;

        public int X
        {
            get
            {
                return drawRectangle.X;
            }
            set
            {
                drawRectangle.X = value;
                if (font != null)
                {
                    textLoc.X = drawRectangle.X + drawRectangle.Width / 2 - font.MeasureString(text).X / 2;
                }
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
                if (font != null)
                {
                    textLoc.Y = drawRectangle.Y + drawRectangle.Height / 2 - font.MeasureString(text).Y / 2;
                }
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
                if (font != null)
                {
                    textLoc.X = drawRectangle.X + drawRectangle.Width / 2 - font.MeasureString(text).X / 2;
                }
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
                if (font != null)
                {
                    textLoc.Y = drawRectangle.Y + drawRectangle.Height / 2 - font.MeasureString(text).Y / 2;
                }
            }
        }
        public Rectangle DrawRectangle
        {
            get
            {
                return drawRectangle;
            }
        }
        public int ImgWidth
        {
            get
            {
                return buttonRectangle.Width;
            }
            set
            {
                buttonRectangle.Width = value;
                if (!hasBackground)
                {
                    drawRectangle.Width = value;
                }
            }
        }
        public int ImgHeight
        {
            get
            {
                return buttonRectangle.Height;
            }
            set
            {
                buttonRectangle.Height = value;
                if (!hasBackground)
                {
                    drawRectangle.Height = value;
                }
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (buttonType == ButtonType.Text)
                {
                    drawRectangle.Width = (int)font.MeasureString(text).X + SPACING_X;
                    drawRectangle.Height = (int)font.MeasureString(text).Y + SPACING_Y;
                }
            }
        }

        #endregion

        #region Constructors

        public MenuButton(OnButtonClick onButtonClick, string buttonText, ContentManager content, int x, int y, bool hasBackground, SpriteFont font)
        {
            Active = true;
            this.onButtonClick = onButtonClick;
            text = buttonText;
            bgImage = content.Load<Texture2D>(ASSET_NAME);

            this.font = font;

            buttonType = ButtonType.Text;

            if (font != null)
            {
                drawRectangle = new Rectangle(x, y, ((int)font.MeasureString(buttonText).X + SPACING_X), 
                    ((int)font.MeasureString(buttonText).Y + SPACING_Y));
            }
            else
            {
                drawRectangle = new Rectangle(x, y, 1, 1);
            }

            this.hasBackground = hasBackground;

            textLoc = new Vector2(drawRectangle.X + drawRectangle.Width / 2 - font.MeasureString(text).X / 2,
                drawRectangle.Y + drawRectangle.Height / 2 - font.MeasureString(text).Y / 2);

            GameInfo.Buttons.Add(this);
        }
        public MenuButton(string imageName, OnButtonClick onButtonClick, ContentManager content, int x, int y, bool hasBackground)
        {
            this.onButtonClick = onButtonClick;
            bgImage = content.Load<Texture2D>(ASSET_NAME);

            buttonType = ButtonType.Image;

            buttonImage = content.Load<Texture2D>(imageName);
            buttonRectangle = new Rectangle(0, 0, buttonImage.Width, buttonImage.Height);

            drawRectangle = new Rectangle(x, y, buttonImage.Width + SPACING_X, buttonImage.Height + SPACING_Y);

            this.hasBackground = hasBackground;

            GameInfo.Buttons.Add(this);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (Active)
            {
                isSelected = (drawRectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1)));
                if (isSelected)
                {
                    drawColor = Color.DarkBlue;
                }
                else
                {
                    drawColor = Color.Blue;
                }
                if (isSelected && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    clickStarted = true;
                    buttonReleased = false;
                }
                else if (!(mouse.LeftButton == ButtonState.Pressed))
                {
                    buttonReleased = true;
                }

                if (clickStarted && buttonReleased)
                {
                    onButtonClick();
                    Sound.PlaySound(Sounds.Click);
                    clickStarted = false;
                    buttonReleased = true;
                }
            }
            else
            {
                drawColor = Color.Gray;
            }
        }
        public void Update(OnButtonHover onHover, OnButtonHover whenNotHovered)
        {
            if (Active)
            {
                isSelected = (drawRectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1)));
                if (isSelected)
                {
                    onHover();
                }
                else
                {
                    whenNotHovered();
                }
                if (isSelected && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    clickStarted = true;
                    buttonReleased = false;
                }
                else if (!(mouse.LeftButton == ButtonState.Pressed))
                {
                    buttonReleased = true;
                }

                if (clickStarted && buttonReleased)
                {
                    onButtonClick();
                    Sound.PlaySound(Sounds.Click);
                    clickStarted = false;
                    buttonReleased = true;
                }
            }
            else
            {
                drawColor = Color.Gray;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (hasBackground)
            {
                spriteBatch.Draw(bgImage, drawRectangle, drawColor);
            }
            switch (buttonType)
            {
                case ButtonType.Text:
                    if (Active && font != null)
                    {
                        spriteBatch.DrawString(font, text, textLoc, Color.Orange);
                    }
                    else if (font != null)
                    {
                        spriteBatch.DrawString(font, text, textLoc, Color.LightGray);
                    }
                    break;
                case ButtonType.Image:
                    buttonRectangle.X = drawRectangle.X + TEXT_SPACING_WIDTH;
                    buttonRectangle.Y = drawRectangle.Y + TEXT_SPACING_HEIGHT;
                    if (Active)
                    {
                        spriteBatch.Draw(buttonImage, buttonRectangle, Color.Orange);
                    }
                    else
                    {
                        spriteBatch.Draw(buttonImage, buttonRectangle, Color.LightGray);
                    }
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Color contentColor)
        {
            if (hasBackground)
            {
                spriteBatch.Draw(bgImage, drawRectangle, drawColor);
            }
            switch (buttonType)
            {
                case ButtonType.Text:
                    spriteBatch.DrawString(font, text, new Vector2(drawRectangle.X + TEXT_SPACING_WIDTH, drawRectangle.Y + TEXT_SPACING_HEIGHT), contentColor);
                    break;
                case ButtonType.Image:
                    buttonRectangle.X = drawRectangle.X + TEXT_SPACING_WIDTH;
                    buttonRectangle.Y = drawRectangle.Y + TEXT_SPACING_HEIGHT;
                    spriteBatch.Draw(buttonImage, buttonRectangle, contentColor);
                    break;
            }

        }

        public void Click()
        {
            onButtonClick();
        }
        public void ClickWithSound()
        {
            onButtonClick();
            Sound.PlaySound(Sounds.Click);
        }

        #endregion
    }

    public class ToggleButton
    {
        #region Fields & Properties

        MenuButton toggleButton;

        Texture2D falseImg;
        Rectangle falseRect;
        public bool Toggled;

        const int SPACING = 4;

        public int X
        {
            get
            {
                return toggleButton.X;
            }
            set
            {
                toggleButton.X = value;
                falseRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return toggleButton.Y;
            }
            set
            {
                toggleButton.Y = value;
                falseRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return toggleButton.Width;
            }
        }
        public int Height
        {
            get
            {
                return toggleButton.Height;
            }
        }

        event Action<bool> toggled;

        public MenuButton Button
        {
            get
            {
                return toggleButton;
            }
        }

        public bool Active
        {
            get
            {
                return toggleButton.Active;
            }
            set
            {
                toggleButton.Active = value;
            }
        }

        #endregion

        #region Constructors

        public ToggleButton(string insideImg, string falseImg, ContentManager content, int x, int y, int width, int height)
        {
            this.falseImg = content.Load<Texture2D>(falseImg);
            toggleButton = new MenuButton(insideImg, Toggle, content, x, y, true);
            toggleButton.ImgWidth = width - SPACING * 3;
            toggleButton.ImgHeight = height - SPACING * 3;
            toggleButton.Width = width;
            toggleButton.Height = height;
            falseRect = new Rectangle(x, y, toggleButton.Width, toggleButton.Height);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            toggleButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            toggleButton.Draw(spriteBatch);
            if (Toggled)
            {
                spriteBatch.Draw(falseImg, falseRect, Color.White);
            }
        }

        public void AddOnToggleHandler(Action<bool> handler)
        {
            toggled += handler;
        }

        public void ClickWithSound()
        {
            if (Active)
            {
                toggleButton.ClickWithSound();
            }
        }
        public void Click()
        {
            if (Active)
            {
                toggleButton.Click();
            }
        }

        #endregion

        #region Private Methods

        private void Toggle()
        {
            Toggled = !Toggled;
            toggled?.Invoke(Toggled);
        }

        #endregion
    }

    #region Enums
    public enum ButtonType
    {
        Text,
        Image
    }
    #endregion
}