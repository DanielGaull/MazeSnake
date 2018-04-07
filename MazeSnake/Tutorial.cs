using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;

namespace MazeSnake
{
    class Tutorial
    {
        #region Fields

        List<Texture2D> tutorialImages = new List<Texture2D>();
        int imgIndex;
        Rectangle tutorialRect;
        const int XSPACING = 10;
        const int YSPACING = 10;
        const int BIG_SPACING = 50;

        const int IMG_WIDTH = 700;
        const int IMG_HEIGHT = 500;

        MenuButton nextPageButton;
        MenuButton prevPageButton;

        #endregion

        #region Constructors

        public Tutorial(ContentManager content, List<string> tutorialAssetNames, string whiteRectAsset, int windowWidth, 
            int windowHeight, string leftArrowAsset, string rightArrowAsset)
        {
            // Initialize tutorial images
            foreach (string a in tutorialAssetNames)
            {
                tutorialImages.Add(content.Load<Texture2D>(a));
            }

            tutorialRect = new Rectangle(windowWidth / 2 - (IMG_WIDTH / 2), YSPACING, IMG_WIDTH, IMG_HEIGHT);

            nextPageButton = new MenuButton(rightArrowAsset, new OnButtonClick(NextPage), content, 0, 0, true);
            nextPageButton.X = windowWidth - nextPageButton.Width - YSPACING;
            nextPageButton.Y = windowHeight - nextPageButton.Height - YSPACING;

            prevPageButton = new MenuButton(leftArrowAsset, new OnButtonClick(PrevPage), content, XSPACING, nextPageButton.Y, true);
            prevPageButton.Width = nextPageButton.Width;
            prevPageButton.ImgWidth = nextPageButton.ImgWidth;
            prevPageButton.Height = nextPageButton.Height;
            prevPageButton.ImgHeight = nextPageButton.ImgHeight;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (imgIndex == 0)
            {
                // We are on the first image
                prevPageButton.Active = false;
            }
            else
            {
                prevPageButton.Active = true;
            }
            if (imgIndex == tutorialImages.Count - 1)
            {
                // We are on the last image
                nextPageButton.Active = false;
            }
            else
            {
                nextPageButton.Active = true;
            }

            nextPageButton.Update();
            prevPageButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            nextPageButton.Draw(spriteBatch);
            prevPageButton.Draw(spriteBatch);
            spriteBatch.Draw(tutorialImages[imgIndex], tutorialRect, Color.White);
        }

        #endregion

        #region Private Methods

        private void NextPage()
        {
            if (imgIndex < tutorialImages.Count - 1)
            {
                imgIndex++;
            }
        }

        private void PrevPage()
        {
            if (imgIndex > 0)
            {
                imgIndex--;
            }
        }

        #endregion
    }
}
