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
    /// <summary>
    /// A class that can combine Texture2D's, acting as one Texture
    /// </summary>
    class CombinableImage
    {
        #region Fields

        List<Texture2D> images = new List<Texture2D>();

        #endregion

        #region Constructors

        public CombinableImage()
        {
        }

        public CombinableImage(params Texture2D[] images)
        {
            this.images = images.ToList();
        }

        #endregion

        #region Public Images

        public void AddImage(Texture2D image)
        {
            images.Add(image);
        }
        public void RemoveImage(Texture2D image)
        {
            if (images.Contains(image))
            {
                images.Remove(image);
            }
        }
        public void RemoveAll()
        {
            images.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, float rotation, SpriteEffects effects, List<Color> colors, Rectangle drawRectangle)
        {
            for (int i = 0; i < images.Count; i++)
			{
                Color drawColor;
                if (colors.ElementAt(i) == null)
                {
                    drawColor = Color.White;
                }
                else
                {
                    drawColor = colors[i];
                }
                spriteBatch.Draw(images[i], drawRectangle, null, drawColor, rotation, new Vector2(), effects, 0.0f);
			}
        }

        #endregion
    }
}
