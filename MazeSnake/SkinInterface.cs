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
    class SkinInterface
    {
        #region Fields & Properties

        Snake snake;

        Color color;

        MenuButton selectButton;

        Texture2D bgImg;
        Rectangle bgRect;
        const int WIDTH = 145;
        const int HEIGHT = 165;

        const int SPACING = 10;

        public bool IsButtonActive
        {
            get
            {
                return selectButton.Active;
            }
            set
            {
                selectButton.Active = value;
            }
        }

        string skinName = "";
        Vector2 skinNamePos;

        public Skin Skin;

        SpriteFont font;

        SkinAction skinSelected;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
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
                return bgRect.Width;
            }
            set
            {
                bgRect.Width = value;
            }
        }

        #endregion

        #region Constructors

        public SkinInterface(Skin skin, SkinAction whenSelectClicked, ContentManager content, string bgAsset, SpriteFont font, string tongueAsset,
            int x, int y)
        {
            this.Skin = skin;
            this.font = font;
            this.skinSelected = whenSelectClicked;

            color = Color.White;
            color.R = 124;
            color.G = 216;
            color.B = 247;

            bgRect = new Rectangle(x, y, WIDTH, HEIGHT);

            skinName = skin.Name;
            skinNamePos = new Vector2(bgRect.X + (bgRect.Width / 2 - ((int)font.MeasureString(skinName).X / 2)), bgRect.Y + SPACING);

            selectButton = new MenuButton(new OnButtonClick(SelectClicked), LanguageTranslator.Translate("Select"), content, 0, 0,
                true, font);
            selectButton.X = bgRect.X + (bgRect.Width / 2 - (selectButton.Width / 2));
            selectButton.Y = bgRect.Bottom - selectButton.Height - SPACING;

            snake = new Snake(skin, 0, 0, content, tongueAsset);
            snake.X = bgRect.X + (bgRect.Width / 2 - (snake.Width / 2));
            snake.Y = selectButton.Y - snake.Rectangle.Height - SPACING;

            bgImg = content.Load<Texture2D>(bgAsset);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // Update Positioning  Vars
            skinNamePos.X = bgRect.X + (bgRect.Width / 2 - ((int)font.MeasureString(skinName).X / 2));
            skinNamePos.Y = bgRect.Y + SPACING; 
            selectButton.X = bgRect.X + (bgRect.Width / 2 - (selectButton.Width / 2));
            selectButton.Y = bgRect.Bottom - selectButton.Height - SPACING;
            snake.X = bgRect.X + (bgRect.Width / 2 - (snake.Width / 2));
            snake.Y = selectButton.Y - snake.Rectangle.Height - SPACING;

            // Update Objects
            skinName = LanguageTranslator.Translate(skinName);

            selectButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, color);
            snake.Draw(spriteBatch);
            selectButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, skinName, skinNamePos, Color.DarkGoldenrod);
        }

        #endregion

        #region Private Methods

        public void SelectClicked()
        {
            skinSelected(this.Skin);
        }

        #endregion
    }
}
