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
    public delegate void OnUserButtonClick(User user);

    public class UserInterface
    {
        #region Fields & Properties

        public User User;

        string username;

        Snake snake;

        Texture2D trashImg;

        Texture2D rectImg;
        Rectangle bgRect;

        ContentManager content;

        Color bgColor = Color.White;

        MenuButton trashButton;
        Color trashColor = Color.DarkGray;

        MenuButton usernameButton;
        Color usernameColor = Color.Orange;

        const int SPACING = 10;
        const int WIDTH = 600;
        const int HEIGHT = 93;
        const int EDIT_IMG_WIDTH = 50;
        const int EDIT_IMG_HEIGHT = 65;
        const int SNAKE_WIDTH = 125;
        const int SNAKE_HEIGHT = 50;

        SpriteFont font;

        OnUserButtonClick trashClicked;
        OnUserButtonClick usernameClicked;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;

                snake.X = this.X + SPACING;
                if (snake.HasHat)
                {
                    snake.Y = bgRect.Bottom - snake.Height - SPACING;
                }
                else
                {
                    snake.Y = bgRect.Y + (bgRect.Height / 2 - (snake.Height / 2));
                }

                usernameButton.X = snake.Rectangle.Right + SPACING;
                usernameButton.Y = bgRect.Y + ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2));

                trashButton.X = (bgRect.Right - (SPACING * 2)) - 40;
                trashButton.Y = bgRect.Y + (SPACING / 10);
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

                snake.X = this.X + SPACING;
                if (snake.HasHat)
                {
                    snake.Y = bgRect.Bottom - snake.Height - SPACING;
                }
                else
                {
                    snake.Y = bgRect.Y + (bgRect.Height / 2 - (snake.Height / 2));
                }

                usernameButton.X = snake.Rectangle.Right + SPACING;
                usernameButton.Y = bgRect.Y + ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2));

                trashButton.X = (bgRect.Right - (SPACING * 2)) - 40;
                trashButton.Y = bgRect.Y + (SPACING / 10);
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
                usernameButton.X = snake.Rectangle.Right + SPACING;
                usernameButton.Y = bgRect.Y + ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2));

                trashButton.X = (bgRect.Right - (SPACING * 2)) - 40;
                trashButton.Y = bgRect.Y + (SPACING / 10); 
                
                snake.X = this.X + SPACING;
                if (snake.HasHat)
                {
                    snake.Y = bgRect.Bottom - snake.Height - SPACING;
                }
                else
                {
                    snake.Y = bgRect.Y + (bgRect.Height / 2 - (snake.Height / 2));
                }
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
                usernameButton.X = snake.Rectangle.Right + SPACING;
                usernameButton.Y = bgRect.Y + ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2));

                trashButton.X = (bgRect.Right - (SPACING * 2)) - 40;
                trashButton.Y = bgRect.Y + (SPACING / 10);

                snake.X = this.X + SPACING;
                if (snake.HasHat)
                {
                    snake.Y = bgRect.Bottom - snake.Height - SPACING;
                }
                else
                {
                    snake.Y = bgRect.Y + (bgRect.Height / 2 - (snake.Height / 2));
                }
            }
        }

        #endregion

        #region Constructors

        public UserInterface(User user, ContentManager content, string trashAsset, SpriteFont font,
            string rectAsset, int x, int y, OnUserButtonClick whenUserClicked, OnUserButtonClick whenTrashClicked, string tongueAsset)
        {
            this.User = user;
            this.username = user.Username;
            this.content = content;
            this.font = font;

            this.trashClicked = whenTrashClicked;
            this.usernameClicked = whenUserClicked;

            rectImg = content.Load<Texture2D>(rectAsset);
            bgRect = new Rectangle(x, y, WIDTH, HEIGHT);

            snake = new Snake(user.CurrentSkin, x + SPACING, 0, content, tongueAsset);
            snake.Y = bgRect.Bottom - snake.Height - SPACING;

            trashImg = content.Load<Texture2D>(trashAsset);
            trashButton = new MenuButton(trashAsset, new OnButtonClick(TrashClick), content,
                (bgRect.Right - (SPACING * 2)) - 40, bgRect.Top + (SPACING / 10), false);
            trashButton.ImgWidth = EDIT_IMG_WIDTH;
            trashButton.ImgHeight = EDIT_IMG_HEIGHT;
            trashButton.Active = true;

            usernameButton = new MenuButton(new OnButtonClick(UsernameClick), username, content, snake.Rectangle.Right + SPACING, bgRect.Y +
                ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2)), false, font);
            usernameButton.Active = true;

            bgColor.R = 124;
            bgColor.G = 216;
            bgColor.B = 247;

            usernameColor.R = 209;
            usernameColor.G = 136;
            usernameColor.B = 0;

            username = User.Username;
            usernameButton.Text = this.username;

            usernameButton.X = snake.Rectangle.Right + SPACING;
            usernameButton.Y = bgRect.Y + ((bgRect.Height / 2) - (int)(font.MeasureString(username).Y / 2));

            trashButton.X = (bgRect.Right - (SPACING * 2)) - 40;
            trashButton.Y = bgRect.Y + (SPACING / 10);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            trashButton.Update(new OnButtonHover(TrashHover), new OnButtonHover(TrashUnHover));
            usernameButton.Update(new OnButtonHover(UsernameHover), new OnButtonHover(UsernameUnHover));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background, as it makes the interface more visually appealing
            spriteBatch.Draw(rectImg, bgRect, bgColor);

            // Draw skin for an easier way to recognize the user
            snake.ChangeSkin(this.User.CurrentSkin);
            snake.Draw(spriteBatch);

            // Draw clickable username
            usernameButton.Draw(spriteBatch, usernameColor);

            // Draw clickable trash button
            trashButton.Draw(spriteBatch, trashColor);
        }

        #endregion

        #region Private Methods

        private void TrashHover()
        {
            trashColor = Color.Gray;
        }
        private void TrashUnHover()
        {
            trashColor = Color.DarkGray;
        }

        private void UsernameHover()
        {
            usernameColor.R = 176;
            usernameColor.G = 114;
            usernameColor.B = 0;
        }
        private void UsernameUnHover()
        {
            usernameColor.R = 209;
            usernameColor.G = 136;
            usernameColor.B = 0;
        }

        private void TrashClick()
        {
            trashClicked(User);
        }
        private void UsernameClick()
        {
            usernameClicked(User);
        }

        #endregion
    }
}
