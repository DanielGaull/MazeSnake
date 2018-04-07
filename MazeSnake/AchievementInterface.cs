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
    class AchievementInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        Color bgColor;

        const int BG_WIDTH = 500;
        const int BG_HEIGHT = 100;

        const int SPACING = 5;

        Texture2D thumbnailImg;
        Rectangle thumbnailRect;
        Color thumbnailColor;

        Texture2D checkImg;
        Rectangle checkRect;

        string name;
        string desc;
        SpriteFont bigFont;
        SpriteFont smallFont;
        Vector2 namePos;
        Vector2 descPos;

        Texture2D difficultyImg;
        Rectangle difficultyRect;
        Color difficultyColor;
        const int DIFFICULTY_CIRCLE_DIAMETER = 20;

        ContentManager content;

        public bool MysteryAch = false;

        public Achievement Achievement;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                UpdateVars();
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
                UpdateVars();
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
                UpdateVars();
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
                UpdateVars();
            }
        }

        public bool Completed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new AchievementInterface
        /// </summary>
        public AchievementInterface(ContentManager content, string whiteRectSprite, Achievement achievement, int x, int y,
            SpriteFont bigFont, SpriteFont smallFont, string whiteCirSprite, string checkSprite)
        {
            Achievement = achievement;
            this.content = content;

            checkImg = content.Load<Texture2D>(checkSprite);
            thumbnailImg = content.Load<Texture2D>(Achievement.Image);
            thumbnailColor = Achievement.ImageColor.GetColor();
            bgImg = content.Load<Texture2D>(whiteRectSprite);

            name = achievement.Name;
            desc = achievement.Description;

            this.bigFont = bigFont;
            this.smallFont = smallFont;

            MysteryAch = false;

            bgColor = Color.White;
            bgColor.R = 124;
            bgColor.G = 216;
            bgColor.B = 247;

            difficultyImg = content.Load<Texture2D>(whiteCirSprite);

            bgRect = new Rectangle(x, y, BG_WIDTH, BG_HEIGHT);
            UpdateVars();
        }

        /// <summary>
        /// Creates a new Interface for a mystery achievement
        /// </summary>
        public AchievementInterface(ContentManager content, string whiteRectSprite, string lockSprite, int x, int y, SpriteFont bigFont,
            SpriteFont smallFont, Achievement achievement, string whiteCircleSprite, string checkSprite)
        {
            Achievement = achievement;
            this.content = content;

            bgImg = content.Load<Texture2D>(whiteRectSprite);
            thumbnailImg = content.Load<Texture2D>(lockSprite);
            thumbnailColor = Achievement.ImageColor.GetColor();
            difficultyImg = content.Load<Texture2D>(whiteCircleSprite);
            checkImg = content.Load<Texture2D>(checkSprite);

            bgRect = new Rectangle(x, y, BG_WIDTH, BG_HEIGHT);
            thumbnailRect = new Rectangle(x + SPACING, y + SPACING, DIFFICULTY_CIRCLE_DIAMETER * 2, DIFFICULTY_CIRCLE_DIAMETER * 2);

            bgColor = Color.Gray;

            name = "???";
            desc = "This is a mystery achievement.";

            descPos = new Vector2(thumbnailRect.X + thumbnailRect.Width + SPACING, (thumbnailRect.Y + thumbnailRect.Height) - (int)(smallFont.MeasureString(desc).Y));
            namePos = new Vector2(descPos.X, SPACING * 1.5f + bgRect.Y);

            this.bigFont = bigFont;
            this.smallFont = smallFont;

            MysteryAch = true;
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch, User user)
        {
            if (user.AchievementsCompleted.Count > 0)
            {
                foreach (Achievement a in user.AchievementsCompleted)
                {
                    if (a.Id == Achievement.Id)
                    {
                        Completed = true;
                    }
                }
            }

            if (MysteryAch && Completed)
            {
                MysteryToCompleted();
            }

            if (MysteryAch)
            {
                spriteBatch.Draw(bgImg, bgRect, bgColor);
                spriteBatch.Draw(thumbnailImg, thumbnailRect, Color.LightGray);

                spriteBatch.DrawString(smallFont, desc, descPos, Color.Black);
                spriteBatch.DrawString(bigFont, name, namePos, Color.Black);
            }
            else
            {
                spriteBatch.Draw(bgImg, bgRect, bgColor);
                spriteBatch.Draw(thumbnailImg, thumbnailRect, thumbnailColor);

                spriteBatch.Draw(difficultyImg, difficultyRect, difficultyColor);
                spriteBatch.DrawString(smallFont, desc, descPos, Color.Black);
                spriteBatch.DrawString(bigFont, name, namePos, Color.Black);

                if (Completed)
                {
                    spriteBatch.Draw(checkImg, checkRect, Color.White);
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateVars()
        {
            // This method is very useful. When the position and size properties change, the positions and sizes of the elements must be changed as well
            if (MysteryAch)
            {
                thumbnailRect = new Rectangle(bgRect.X + SPACING, bgRect.Y + SPACING, DIFFICULTY_CIRCLE_DIAMETER * 2, DIFFICULTY_CIRCLE_DIAMETER * 2);
            }
            else
            {
                thumbnailRect = new Rectangle(bgRect.X + SPACING, bgRect.Y + SPACING, thumbnailImg.Width, thumbnailImg.Height);
            }

            this.Completed = Achievement.Completed;

            descPos = new Vector2(thumbnailRect.X + thumbnailRect.Width + SPACING, (int)(bigFont.MeasureString(name).Y + SPACING + bgRect.Y));
            namePos = new Vector2(descPos.X, SPACING * 1.5f + bgRect.Y);

            checkRect = new Rectangle(0 /*Like with the reward rectangle, the x must be done separately as it requires a currently null Width*/,
                0 /*This is here for the same reason as the X ^ */,
                bgRect.Height / 2 /*We want the width and height to be equal, and the check to be as tall as the interface, so the width = bgRect.Height / 2*/,
                bgRect.Height / 2);
            checkRect.X = bgRect.X - checkRect.Width;
            checkRect.Y = bgRect.Y + (bgRect.Height / 2 - (checkRect.Width / 2));

            switch (Achievement.Difficulty)
            {
                case AchievementDifficulty.Green:
                    difficultyColor = Color.LightGreen;
                    break;
                case AchievementDifficulty.Yellow:
                    difficultyColor = Color.Yellow;
                    break;
                case AchievementDifficulty.Orange:
                    difficultyColor = Color.Orange;
                    break;
                case AchievementDifficulty.Red:
                    difficultyColor = Color.Red;
                    break;
            }
            difficultyRect = new Rectangle(thumbnailRect.X, 0, DIFFICULTY_CIRCLE_DIAMETER, DIFFICULTY_CIRCLE_DIAMETER); // Width is null right now; caculate Y afterwards
            difficultyRect.Y = (bgRect.Y + bgRect.Height) - (difficultyRect.Height + SPACING);
        }

        private void MysteryToCompleted()
        {
            desc = Achievement.Description;
            name = Achievement.Name;
            thumbnailImg = content.Load<Texture2D>(Achievement.Image);
            // Change the background color back to normal
            // This makes sure that we're changing a white color, not gray
            bgColor = Color.White;
            bgColor.R = 124;
            bgColor.G = 216;
            bgColor.B = 247;
            MysteryAch = false;
        }

        #endregion
    }
}
