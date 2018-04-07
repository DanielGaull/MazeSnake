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
    public class Popup
    {
        #region Fields & Properties

        string text = "";
        Vector2 textPosition;

        bool hasCheckbox = false;

        Texture2D backgroundImage;
        Rectangle drawRectangle;
        Color colorOfBackground = Color.MidnightBlue;
        MenuButton okButton;
        MenuButton cancelButton;
        Checkbox queryCheckbox;

        Texture2D transImg;
        Rectangle transRect;
        Color transColor;

        const int SPACING_Y = 60;
        const int SPACING_X = 40;
        const int BACKGROUND_WIDTH = 600;
        const int BACKGROUND_HEIGHT = 250;
        string BUTTON1 = LanguageTranslator.Translate("Okay");
        string BUTTON2 = LanguageTranslator.Translate("Cancel");
        const byte ALPHA = 100;

        OnButtonClick whenCancelButtonClicked;
        OnButtonClick whenOkButtonClicked;

        SpriteFont font;

        bool active = false;

        bool hasCancelButton = true;

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public bool CheckboxChecked
        {
            get
            {
                return queryCheckbox.IsChecked;
            }
            set
            {
                queryCheckbox.IsChecked = value;
            }
        }

        public int X
        {
            get
            {
                return drawRectangle.X;
            }
        }
        public int Y
        {
            get
            {
                return drawRectangle.Y;
            }
        }
        public int Width
        {
            get
            {
                return drawRectangle.Width;
            }
        }
        public int Height
        {
            get
            {
                return drawRectangle.Height;
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
            }
        }

        #endregion

        #region Constructors

        public Popup(ContentManager content, OnButtonClick whenOkButtonClicked, string checkboxSprite, string checkSprite, 
            SpriteFont font, string transAsset, int windowWidth, int windowHeight, bool hasCancelButton, SpriteFont bigFont)
        {
            this.hasCancelButton = hasCancelButton;

            backgroundImage = content.Load<Texture2D>(transAsset);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new OnButtonClick(HidePopup);
            this.whenOkButtonClicked = whenOkButtonClicked;

            queryCheckbox = new Checkbox(LanguageTranslator.Translate("Do not show again"), content, font, checkboxSprite, checkSprite);

            this.font = font;

            okButton = new MenuButton(new OnButtonClick(OnButtonClick), BUTTON1, content, 0, 0, true, bigFont);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, content, 0, 0, true, bigFont);
            }

            textPosition = new Vector2(0, 0);

            transImg = content.Load<Texture2D>(transAsset);
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }

        public Popup(ContentManager content, SpriteFont smallFont, string transAsset, int windowWidth, int windowHeight, bool hasCancelButton, SpriteFont bigFont)
        {
            this.hasCancelButton = hasCancelButton;

            backgroundImage = content.Load<Texture2D>(transAsset);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new OnButtonClick(HidePopup);

            font = smallFont;

            okButton = new MenuButton(new OnButtonClick(HidePopup), BUTTON1, content, 0, 0, true, bigFont);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, content, 0, 0, true, bigFont);
            }

            textPosition = new Vector2(0, 0);

            transImg = content.Load<Texture2D>(transAsset);
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }
        public Popup(ContentManager content, SpriteFont smallFont, string transAsset, int windowWidth, int windowHeight, bool hasCancelButton, SpriteFont bigFont,
            OnButtonClick whenOkButtonClicked)
        {
            this.hasCancelButton = hasCancelButton;

            backgroundImage = content.Load<Texture2D>(transAsset);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new OnButtonClick(HidePopup);
            this.whenOkButtonClicked = whenOkButtonClicked;

            font = smallFont;

            okButton = new MenuButton(new OnButtonClick(OnButtonClick), BUTTON1, content, 0, 0, true, bigFont);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, content, 0, 0, true, bigFont);
            }

            textPosition = new Vector2(0, 0);

            transImg = content.Load<Texture2D>(transAsset);
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }
        #endregion

        #region Public Methods

        public void ShowPopup(string text, bool shouldQueryShowPopupAgain, int x, int y)
        {
            this.text = text;
            active = true;
            drawRectangle.X = x;
            drawRectangle.Y = y;
            hasCheckbox = shouldQueryShowPopupAgain;
        }
        public void HidePopup()
        {
            active = false;
        }

        public void Update()
        {
            okButton.Text = LanguageTranslator.Translate("Okay");
            if (hasCancelButton)
            {
                cancelButton.Text = LanguageTranslator.Translate("Cancel");
            }
            if (active && hasCheckbox)
            {
                queryCheckbox.Text = LanguageTranslator.Translate("Do not show again.");
            }

            if (active)
            {
                okButton.Update();
                if (hasCancelButton)
                {
                    cancelButton.Update();
                }
                if (hasCheckbox)
                {
                    queryCheckbox.Update();
                }
            }

            // set button positions so they are correctly aligned with the popup background
            if (hasCancelButton)
            {
                okButton.X = drawRectangle.X + SPACING_X;
                okButton.Y = drawRectangle.Y + drawRectangle.Height - SPACING_Y;
                cancelButton.X = drawRectangle.Right - (SPACING_X + cancelButton.Width);
                cancelButton.Y = drawRectangle.Y + drawRectangle.Height - SPACING_Y;
            }
            else
            {
                // No cancel button, center Ok Button
                okButton.X = drawRectangle.X + (drawRectangle.Width / 2 - (okButton.Width / 2));
                okButton.Y = drawRectangle.Y + drawRectangle.Height - SPACING_Y;
            }

            // set the checkbox and text positions so they are correctly aligned with the popup background
            if (hasCheckbox)
            {
                queryCheckbox.X = okButton.DrawRectangle.Left;
                queryCheckbox.Y = okButton.DrawRectangle.Top - SPACING_Y;
            }
            textPosition.X = drawRectangle.X + SPACING_X;
            textPosition.Y = (float)drawRectangle.Y + SPACING_Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(transImg, transRect, transColor);

                spriteBatch.Draw(backgroundImage, drawRectangle, colorOfBackground);

                spriteBatch.DrawString(font, text, textPosition, Color.White);

                okButton.Draw(spriteBatch);

                if (hasCancelButton)
                {
                    cancelButton.Draw(spriteBatch);
                }

                if (hasCheckbox)
                {
                    queryCheckbox.Draw(spriteBatch, Color.White);
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnButtonClick()
        {
            HidePopup();
            whenOkButtonClicked();
        }

        #endregion
    }
}

