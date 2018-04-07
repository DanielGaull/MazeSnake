using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace MazeSnake
{
    class LoadingScreen
    {
        #region Fields

        ProgressBar progressBar;

        Texture2D backgroundImage;
        Rectangle backgroundRectangle;

        SpriteFont tipFont;
        string tip = "";
        List<string> tips = new List<string>();

        Texture2D titleImg;
        Rectangle titleRect;
        const int TITLE_WIDTH = 200;
        const int TITLE_HEIGHT = 50;

        Snake snake;
        Color snakeColor;

        Vector2 tipLocation = new Vector2();

        Random rand;

        Timer timer;

        DateTime dateTime = DateTime.Now;

        const int YSPACING = 100;

        int windowWidth;
        int windowHeight;

        ContentManager content;
        string bigFontName;

        bool isMazeWorm = false;

        #endregion

        #region Constructors

        public LoadingScreen(ContentManager content, string progressBarImage, string progressImageName, string fontName,
            string backgroundImageName, int windowWidth, int windowHeight, Skin snakeSkin, Random rand, string bigFontName, bool isMazeWorm,
            int millisecondDelay, string titleAsset, string tongueAsset, Color snakeColor)
        {
            this.snakeColor = snakeSkin.Color.GetColor();

            progressBar = new ProgressBar(content, progressBarImage, progressImageName, fontName, Color.DarkRed, 0, 0, true);
            progressBar.X = (windowWidth / 2) - (progressBar.Width / 2);
            progressBar.Y = windowHeight - YSPACING * 2;

            backgroundImage = content.Load<Texture2D>(backgroundImageName);
            backgroundRectangle = new Rectangle(0, 0, windowWidth, windowHeight);

            tipFont = content.Load<SpriteFont>(fontName);

            titleImg = content.Load<Texture2D>(titleAsset);
            titleRect = new Rectangle(0, YSPACING, TITLE_WIDTH, TITLE_HEIGHT);
            titleRect.X = windowWidth / 2 - (titleRect.Width / 2);

            snake = new Snake(snakeSkin, 0, 0, content, tongueAsset);
            snake.X = titleRect.X - snake.Width;
            snake.Y = YSPACING;

            timer = new Timer(millisecondDelay, TimerUnits.Milliseconds);

            this.rand = rand;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.bigFontName = bigFontName;
            this.content = content;
            this.isMazeWorm = isMazeWorm;

            #region Load Tips
            // Read in the tips and put them in the tips list.
            StreamReader file = null;
            try
            {
                // Works, but tips.txt is saved in a weird spot. Must open with Notepad to edit
                file = new StreamReader("tips.txt");
                string line = file.ReadLine();
                while (line != null)
                {
                    if (line.StartsWith("//"))
                    {
                        line = file.ReadLine();
                        continue;
                    }
                    if (line.StartsWith("_on-"))
                    {
                        int month;
                        int day;
                        string tip;
                        try
                        {
                            month = int.Parse(line.Substring(line.IndexOf('-'), line.IndexOf('/')));
                            day = int.Parse(line.Substring(line.IndexOf('/'), line.IndexOf('~')));
                            tip = line.Substring(line.IndexOf('~'), line.Length - 1);

                            if (dateTime.Month == month && dateTime.Day == day)
                            {
                                tips.Add(tip);
                            }
                        }
                        catch (Exception)
                        {

                        }

                        #region Unused
                        //if (line.StartsWith("_on1225") && dateTime.Month == 12 && dateTime.Day == 25)
                        //{
                        //    string addTip = line.Split('~')[1];
                        //}
                        //else if (line.StartsWith("_on1031") && dateTime.Month == 10 && dateTime.Day == 31)
                        //{
                        //    string addTip = line.Split('~')[1];
                        //}
                        //else if (line.StartsWith("_on0115") && dateTime.Month == 1 && dateTime.Day == 15)
                        //{
                        //    string addTip = line.Split('~')[1];
                        //}
                        #endregion
                    }
                    else
                    {
                        line = line.Replace("$version", GameInfo.Version.ToString());
                        tips.Add(line);
                    }
                    line = file.ReadLine();
                }
            }
            catch (FileNotFoundException)
            {
                tips.Add("Error 404: Helpful stuff not found.");
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("There's been an error in your game. Here's the message generated: " + e.Message +
            //        "\nContact your provider with this information: " + "\nException Source Method: " + e.TargetSite
            //         + "\nError Message: " + e.Message + "\nData: " + e.Data + "\nInnerException: " + e.InnerException + "\nSource: " + e.Source +
            //         "\nStackTrace: " + e.StackTrace + "\n--------------------------------" +
            //         "\nWe are sorry for the inconvenience. It is possible that you may have accidentally deleted a source \nfile that is " +
            //         "critical to the program's operation. Again, we apologize. This error will hopefully be fixed in the next update.");
            //    while (true) { }
            //}
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }

            #endregion

            tip = tips[this.rand.Next(tips.Count)];

            tipLocation = new Vector2(windowWidth / 2 - ((int)tipFont.MeasureString(tip).X / 2), progressBar.Y + progressBar.Height + (YSPACING / 2));
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            progressBar.Update();
            if (timer.QueryWaitTime(gameTime))
            {
                tip = tips[rand.Next(tips.Count)];
                tipLocation.X = windowWidth / 2 - ((int)tipFont.MeasureString(tip).X / 2);
                tipLocation.Y = progressBar.Y + progressBar.Height + (YSPACING / 2);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundImage, backgroundRectangle, Color.DarkBlue);

            progressBar.Draw(spriteBatch, Color.White);

            spriteBatch.DrawString(tipFont, tip, tipLocation, Color.White);

            snake.ShadeColor = snakeColor;
            snake.Draw(spriteBatch);

            spriteBatch.Draw(titleImg, titleRect, Color.White);
        }

        public void AddPercentage(int percent)
        {
            progressBar.AddPercentage(percent);
        }

        public void SetPercentage(int percent)
        {
            progressBar.Percentage = percent;
        }

        #endregion
    }
}
