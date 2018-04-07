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
    class Timer
    {
        public float TimerFloat;
        public float WaitTime;
        TimerUnits timerUnit;

        TimerType type;

        SpriteFont font;
        string timerPrefix;
        TimerDisplayType disType;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width
        {
            get
            {
                return (int)font.MeasureString(timerPrefix + ": " + ((int)TimerFloat).ToString()).X;
            }
        }
        public int Height
        {
            get
            {
                return (int)font.MeasureString(timerPrefix + ": " + ((int)TimerFloat).ToString()).Y;
            }
        }

        /// <summary>
        /// Creates a regular timer
        /// </summary>
        public Timer(float waitTime, TimerUnits timerUnit)
        {
            this.timerUnit = timerUnit;
            WaitTime = waitTime;
            type = TimerType.NonDrawable;
            disType = TimerDisplayType.CountUp;
        }

        /// <summary>
        /// Creates a drawable timer display that counts up
        /// </summary>
        public Timer(TimerUnits timeUnit, SpriteFont font, string timePrefix)
        {
            type = TimerType.Drawable;
            this.font = font;
            this.timerPrefix = timePrefix;
            this.timerUnit = timeUnit;
            disType = TimerDisplayType.CountUp;
        }
        /// <summary>
        /// Creates a drawable timer that counts down from the specified value
        /// </summary>
        /// <param name="timeUnit">The unit of time to use</param>
        /// <param name="waitTime">The value to count down from</param>
        public Timer(TimerUnits timeUnit, int waitTime, SpriteFont font, string timePrefix)
        {
            type = TimerType.Drawable;
            this.font = font;
            timerPrefix = timePrefix;
            timerUnit = timeUnit;
            WaitTime = waitTime;
            disType = TimerDisplayType.Countdown;
            TimerFloat = waitTime;
        }

        /// <summary>
        /// Returns whether or not the timer wait time has run out.
        /// </summary>
        /// <param name="gameTime">the current game time</param>
        /// <returns>returns whether or not the wait time has run out</returns>
        public bool QueryWaitTime(GameTime gameTime)
        {
            if (disType == TimerDisplayType.CountUp)
            {
                if (TimerFloat >= WaitTime)
                {
                    this.Reset();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if ((int)TimerFloat <= 0.0f)
                {
                    this.Reset();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (disType == TimerDisplayType.CountUp)
            {
                switch (timerUnit)
                {
                    case TimerUnits.Seconds:
                        TimerFloat += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        break;
                    case TimerUnits.Minutes:
                        TimerFloat += (float)gameTime.ElapsedGameTime.TotalMinutes;
                        break;
                    case TimerUnits.Milliseconds:
                        TimerFloat += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        break;
                    case TimerUnits.Hours:
                        TimerFloat += (float)gameTime.ElapsedGameTime.TotalHours;
                        break;
                    case TimerUnits.Days:
                        TimerFloat += (float)gameTime.ElapsedGameTime.TotalDays;
                        break;
                }
            }
            else
            {
                switch (timerUnit)
                {
                    case TimerUnits.Seconds:
                        TimerFloat -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        break;
                    case TimerUnits.Minutes:
                        TimerFloat -= (float)gameTime.ElapsedGameTime.TotalMinutes;
                        break;
                    case TimerUnits.Milliseconds:
                        TimerFloat -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        break;
                    case TimerUnits.Hours:
                        TimerFloat -= (float)gameTime.ElapsedGameTime.TotalHours;
                        break;
                    case TimerUnits.Days:
                        TimerFloat -= (float)gameTime.ElapsedGameTime.TotalDays;
                        break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color textColor)
        {
            if (type == TimerType.Drawable)
            {
                spriteBatch.DrawString(font, timerPrefix + ": " + ((int)TimerFloat).ToString(), new Vector2(X, Y), textColor);
            }
        }

        public void Reset()
        {
            if (disType == TimerDisplayType.CountUp)
            {
                TimerFloat = 0.0f;
            }
            else
            {
                TimerFloat = WaitTime;
            }
        }
    }

    public enum TimerUnits
    {
        Seconds,
        Minutes,
        Milliseconds,
        Days,
        Hours,
    }
    public enum TimerType
    {
        NonDrawable,
        Drawable,
    }
    public enum TimerDisplayType
    {
        CountUp,
        Countdown
    }
}
