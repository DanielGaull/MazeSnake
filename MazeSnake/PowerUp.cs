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
    public class PowerUp
    {
        #region Fields & Properties

        Texture2D image;
        Rectangle drawRectangle;

        const int TEXTURE_WIDTH = 50;
        const int TEXTURE_HEIGHT = 50;

        bool clamped = false;
        bool inMotion = false;

        SpriteFont font;
        Vector2 displayPos;
        string display = "";
        public bool ShowingDisplay = false;
        Timer displayTimer;

        const int SPEED_CHANCE = 30;
        const int TELEPORT_CHANCE = 30;
        const int RESTART_CHANCE = 5;
        const int WALLBREAK_CHANCE = 10;
        const int JACKPOT_CHANCE = 5;
        const int FORCEFIELD_CHANCE = 20;

        int windowWidth;
        int windowHeight;

        public PowerupType Type { get; set; }
        public bool IsPickedUp { get; set; }

        public Rectangle Rectangle
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

        public bool InMotion
        {
            get
            {
                return inMotion;
            }
        }

        #endregion

        #region Constructors

        public PowerUp(Texture2D texture, int x, int y, Snake snake, int width, int height, PowerupType powerUpType, SpriteFont font)
        {
            this.font = font;
            this.image = texture;
            drawRectangle = new Rectangle(x, y, width, height);
            this.Type = powerUpType;

            displayTimer = new Timer(1, TimerUnits.Seconds);
            displayPos = new Vector2();
        }

        #endregion

        #region Public Methods

        public void Collect(int windowWidth, int windowHeight)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            if (!IsPickedUp)
            {
                IsPickedUp = true;
                ShowingDisplay = true;
                switch (this.Type)
                {
                    case PowerupType.Speed:
                        display = LanguageTranslator.Translate("Speed");
                        break;
                    case PowerupType.Teleport:
                    case PowerupType.Restart:
                        display = LanguageTranslator.Translate("Teleport");
                        break;
                    case PowerupType.WallBreaker:
                        display = LanguageTranslator.Translate("Electric");
                        break;
                    case PowerupType.Jackpot:
                        display = LanguageTranslator.Translate("Jackpot");
                        break;
                    case PowerupType.Frozen:
                        display = LanguageTranslator.Translate("Frozen");
                        break;
                    case PowerupType.Forcefield:
                        display = LanguageTranslator.Translate("Forcefield");
                        break;
                }
                displayPos.X = drawRectangle.X;
                displayPos.Y = drawRectangle.Y;
                if (displayPos.X + font.MeasureString(display).X > windowWidth)
                {
                    // The display text doesn't fit on the screen (right side)
                    displayPos.X = windowWidth - font.MeasureString(display).X;
                }
                else if (displayPos.X < 0)
                {
                    // The display text doesn't fit on the screen (left side)
                    displayPos.X = 0;
                }
                if (displayPos.Y + font.MeasureString(display).Y > windowHeight)
                {
                    // The display text doesn't fit on the screen (bottom)
                    displayPos.Y = windowHeight - font.MeasureString(display).Y;
                }
                else if (displayPos.Y < 0)
                {
                    // The display text doesn't fit on the screen (top)
                    displayPos.Y = 0;
                }
            }
        }

        public bool IsIntersecting(Rectangle rectangle)
        {
            return drawRectangle.Intersects(rectangle);
        }

        public void Update(int windowWidth, int windowHeight, List<Wall> walls, GameTime gameTime)
        {
            // Make sure the powerup doesnt delete itself from the aftereffects of the last update
            clamped = false;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            #region Clamping
            // Clamp so the powerup stays in the window
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
            #endregion

            #region Wall Handling

            foreach (Wall w in walls)
            {
                if (drawRectangle.Intersects(w.GetRectangle))
                {
                    drawRectangle.X++;
                    drawRectangle.Y++;
                    inMotion = true;
                }
                else
                {
                    inMotion = false;
                }

                // If this is true, we're stuck between wall and edge of window, so we'll simply remove the powerup.
                if (drawRectangle.Intersects(w.GetRectangle) && clamped)
                {
                    this.IsPickedUp = true;
                }
            }

            #endregion

            #region Text Display Updating

            if (ShowingDisplay)
            {
                displayTimer.Update(gameTime);
                if (displayTimer.QueryWaitTime(gameTime))
                {
                    ShowingDisplay = false;
                    return; // There're no non-display-related statements below, so we can return
                }
                displayPos.Y -= 0.5f;
            }

            #endregion
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (image != null && !IsPickedUp)
            {
                spritebatch.Draw(image, drawRectangle, Color.White);
            }
            if (ShowingDisplay)
            {
                spritebatch.DrawString(font, display, displayPos, Color.DarkOrange);
            }
        }

        /// <summary>
        /// Returns a random PowerupType; some types are rarer than others
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static PowerupType GetRandomType(Random rand)
        {
            int randomNum = rand.Next(100);

            if (randomNum <= SPEED_CHANCE)
            {
                return PowerupType.Speed;
            }
            else if (randomNum > SPEED_CHANCE && randomNum <= SPEED_CHANCE + TELEPORT_CHANCE)
            {
                return PowerupType.Teleport;
            }
            else if (randomNum > SPEED_CHANCE + TELEPORT_CHANCE && randomNum <= SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE)
            {
                return PowerupType.Restart;
            }
            else if (randomNum > SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE && randomNum <= SPEED_CHANCE + TELEPORT_CHANCE + 
                RESTART_CHANCE + WALLBREAK_CHANCE)
            {
                return PowerupType.WallBreaker;
            }
            else if (randomNum > SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE + WALLBREAK_CHANCE && 
                randomNum <= SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE + WALLBREAK_CHANCE + JACKPOT_CHANCE)
            {
                return PowerupType.Jackpot;
            }
            else if (randomNum > SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE + WALLBREAK_CHANCE + JACKPOT_CHANCE &&
                randomNum <= SPEED_CHANCE + TELEPORT_CHANCE + RESTART_CHANCE + WALLBREAK_CHANCE + JACKPOT_CHANCE + FORCEFIELD_CHANCE)
            {
                return PowerupType.Forcefield;
            }
            else
            {
                throw new InvalidOperationException("The added value of all CHANCEs must equal 100 (static PowerUp.GetRandomType()).");
            }
            // Just in case
            // (PowerupType)(rand.Next(Enum.GetNames(typeof(PowerupType)).ToList().Count))
        }

        #endregion
    }

    #region Enumerations

    public enum PowerupType
    {
        Speed,
        Teleport,
        Restart,
        WallBreaker,
        Jackpot,
        Frozen,
        Forcefield,
    }

    #endregion
}