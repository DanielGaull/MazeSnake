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
    class Notification
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        Color bgColor = Color.MidnightBlue;

        EnterDirection currentDir;

        MenuButton viewButton;
        MenuButton hideButton;

        SpriteFont font;

        public string Text = "";
        Vector2 textPos;

        const int SPACING = 5;

        const int WIDTH = 600;

        Timer timer;

        bool showing = false;

        bool entering = false;
        bool leaving = false;

        int windowWidth;
        int windowHeight;

        #endregion

        #region Constructors

        public Notification(ContentManager content, string whiteRectSprite, string text, int windowWidth, int windowHeight, SpriteFont font)
        {
            int height = (int)font.MeasureString(text).Y + SPACING;
            bgImg = content.Load<Texture2D>(whiteRectSprite);
            bgRect = new Rectangle(0, 0 - height, WIDTH, height);
            bgRect.X = windowWidth / 2 - (bgRect.Width / 2);

            hideButton = new MenuButton(new OnButtonClick(Hide), " X ", content, 0, 0, true, font);

            textPos = new Vector2();

            this.Text = text;
            this.font = font;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            timer = new Timer(3, TimerUnits.Seconds);
        }

        public Notification(ContentManager content, string whiteRectSprite, string text, int windowWidth, int windowHeight, SpriteFont font, 
            OnButtonClick whenViewButtonClicked) 
             : this(content, whiteRectSprite, text, windowWidth, windowHeight, font)
        {
            viewButton = new MenuButton(whenViewButtonClicked, LanguageTranslator.Translate("View"), content, 0, 0, true, font);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime, EnterDirection enterDir)
        {
            // Update Location
            if (viewButton != null)
            {
                viewButton.Text = LanguageTranslator.Translate("View");
            }

            if (enterDir == EnterDirection.Top && bgRect.Y == windowHeight)
            {
                bgRect.Y = 0 - bgRect.Height;
            }
            if (enterDir == EnterDirection.Bottom && bgRect.Y == 0 - bgRect.Height)
            {
                bgRect.Y = windowHeight;
            }

            hideButton.X = (bgRect.Width + bgRect.X) - (SPACING + hideButton.Width);
            hideButton.Y = bgRect.Y + (bgRect.Height / 2 - (hideButton.Height / 2));
            if (viewButton != null)
            {
                viewButton.X = hideButton.X - (viewButton.Width + SPACING);
                viewButton.Y = hideButton.Y;
            }

            textPos.X = bgRect.X + SPACING;
            textPos.Y = bgRect.Y + SPACING;

            // Update Objects
            if (viewButton != null && showing)
            {
                viewButton.Update();
            }

            hideButton.Update();

            if (showing)
            {
                timer.Update(gameTime);
            }

            if (timer.QueryWaitTime(gameTime))
            {
                leaving = true;
            }

            #region Entering and Leaving Animation

            if (entering)
            {
                currentDir = enterDir;
                if (enterDir == EnterDirection.Top)
                {
                    if (bgRect.Y < SPACING)
                    {
                        bgRect.Y += 2;
                    }
                    else
                    {
                        entering = false;
                    }
                }
                else if (enterDir == EnterDirection.Bottom)
                {
                    if (bgRect.Y > windowHeight - bgRect.Height - SPACING)
                    {
                        bgRect.Y -= 2;
                    }
                    else
                    {
                        entering = false;
                    }
                }
            }
            else if (leaving)
            {
                if (bgRect.Y < windowHeight / 2)
                {
                    if (bgRect.Y >= 0 - bgRect.Height)
                    {
                        bgRect.Y -= 2;
                    }
                    else
                    {
                        leaving = false;
                        showing = false;
                    }
                }
                else
                {
                    if (bgRect.Y < windowHeight)
                    {
                        bgRect.Y += 2;
                    }
                    else
                    {
                        leaving = false;
                        showing = false;
                    }
                }
            }

            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (showing)
            {
                spriteBatch.Draw(bgImg, bgRect, bgColor);
                spriteBatch.DrawString(font, Text, textPos, Color.Orange);
                hideButton.Draw(spriteBatch);

                if (viewButton != null)
                {
                    viewButton.Draw(spriteBatch);
                }
            }
        }

        public void Show()
        {
            showing = true;
            entering = true;
        }
        public void Hide()
        {
            showing = false;
            entering = false;
            leaving = true;
        }

        #endregion
    }

    public enum EnterDirection
    {
        Top,
        Bottom
    }
}
