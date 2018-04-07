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
    public class MouseCursor
    {
        #region Fields & Properties

        ContentManager content;
        Texture2D regImg;
        Texture2D typingImg;
        Texture2D currentImg;
        MouseState mouse;
        Rectangle drawRectangle;

        public Rectangle GetRectangle
        {
            get
            {
                return drawRectangle;
            }
        }

        const int INFLATION_SIZE = 10;
        int regWidth;
        int regHeight;
        int inflateWidth;
        int inflateHeight;

        #endregion

        #region Constructors

        public MouseCursor(ContentManager content, string regSprite, string typingSprite)
        {
            this.content = content;
            this.regImg = content.Load<Texture2D>(regSprite);
            this.typingImg = content.Load<Texture2D>(typingSprite);
            this.currentImg = regImg;
            mouse = Mouse.GetState();
            drawRectangle = new Rectangle(500, 500, regImg.Width, regImg.Height);
            regWidth = regImg.Width;
            regHeight = regImg.Height;
            inflateWidth = regWidth + INFLATION_SIZE;
            inflateHeight = regHeight + INFLATION_SIZE;
        }

        #endregion

        #region Public Methods

        public void Update(List<Textbox> textboxes)
        {
            currentImg = regImg;
            drawRectangle.Width = regWidth;
            drawRectangle.Height = regHeight;
            foreach (Textbox t in textboxes)
            {
                if (drawRectangle.Intersects(t.DrawRectangle))
                {
                    currentImg = typingImg;
                }
            }
            //foreach (MenuButton b in GameInfo.Buttons)
            //{
            //    if (drawRectangle.Intersects(b.DrawRectangle) && b.Active)
            //    {
            //        drawRectangle.Width = inflateWidth;
            //        drawRectangle.Height = inflateHeight;
            //    }
            //}
            if (textboxes.Count == 0) 
            {
                currentImg = regImg;
            }
            mouse = Mouse.GetState();
            drawRectangle.X = mouse.X;
            drawRectangle.Y = mouse.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentImg, drawRectangle, Color.White);
        }

        #endregion
    }
}