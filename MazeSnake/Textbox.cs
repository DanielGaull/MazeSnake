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
    public delegate Textbox OnTextboxClick();
    public class Textbox
    {
        #region Fields and Propterties

        string text = "";
        Vector2 textPos;
        SpriteFont font;

        string keyName = "";

        public bool NumbersOnly = false;

        public int X
        {
            get
            {
                return DrawRectangle.X;
            }
            set
            {
                int previousX = DrawRectangle.X;
                cursorRectangle.X += value - previousX;
                DrawRectangle.X = value;
            }
        }
        public int Y
        {
            get
            {
                return DrawRectangle.Y;
            }
            set
            {
                DrawRectangle.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return DrawRectangle.Width;
            }
            set
            {
                DrawRectangle.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return DrawRectangle.Height;
            }
            set
            {
                DrawRectangle.Height = value;
            }
        }

        bool spammingKey = false;

        public bool Drawn = false;

        Texture2D image;
        public Rectangle DrawRectangle;

        Texture2D cursorImage;
        Rectangle cursorRectangle;
        int cursorPosition = 0;
        bool arrowKeyPrevDown = false;

        // A button that will check for clicking on the textbox. Don't draw.
        MenuButton checkForClickButton;
        OnButtonClick onClick;

        bool shortcutPreviouslyPressed = false;

        Microsoft.Xna.Framework.Input.Keys[] pressedKeys;
        Microsoft.Xna.Framework.Input.Keys[] lastKeys;
        Microsoft.Xna.Framework.Input.Keys key;
        Microsoft.Xna.Framework.Input.Keys lastKey;
        KeyboardState keyboard;
        Timer pressKeysTimerLong;
        Timer pressKeysTimerShort;
        const int TEXT_EDGE_SPACING_X = 5;
        const int TEXT_EDGE_SPACING_Y = 5;
        const int CURSOR_SPACING = 7;

        Timer cursorBlinkTimer;
        bool showCursor = false;

        public string Content
        {
            get
            {
                return text;
            }
            set
            {
                if (value != text)
                {
                    cursorPosition = value.Length;
                    cursorRectangle.X = (int)(font.MeasureString(value).X) + cursorRectangle.Width + DrawRectangle.X;
                }
                text = value;
            }
        }

        public bool Active = true;

        #endregion

        #region Constructors

        public Textbox(int x, int y, ContentManager content, string imageName, SpriteFont font, bool active, string rectAsset)
        {
            this.X = x;
            this.Y = y;
            this.Active = active;

            image = content.Load<Texture2D>(imageName);
            DrawRectangle = new Rectangle(x, y, image.Width + 200, image.Height + TEXT_EDGE_SPACING_Y);

            cursorBlinkTimer = new Timer(500, TimerUnits.Milliseconds);

            textPos = new Vector2(DrawRectangle.X + TEXT_EDGE_SPACING_X, DrawRectangle.Y + TEXT_EDGE_SPACING_Y);
            this.font = font;

            pressKeysTimerLong = new Timer(750, TimerUnits.Milliseconds);
            pressKeysTimerShort = new Timer(125, TimerUnits.Milliseconds);

            cursorImage = content.Load<Texture2D>(rectAsset);
            cursorRectangle = new Rectangle(DrawRectangle.X + TEXT_EDGE_SPACING_X, DrawRectangle.Y + TEXT_EDGE_SPACING_Y, TEXT_EDGE_SPACING_Y,
                DrawRectangle.Height - (TEXT_EDGE_SPACING_Y * 3));

            checkForClickButton = new MenuButton(new OnButtonClick(HandleClick), "", content, x, y, false, font);
            checkForClickButton.Width = DrawRectangle.Width;
            checkForClickButton.Height = DrawRectangle.Height;
        }
        public Textbox(int x, int y, ContentManager content, string imageName, SpriteFont font, bool active, string rectAsset, OnButtonClick onClick)
            : this(x, y, content, imageName, font, active, rectAsset)
        {
            this.onClick = onClick;
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (cursorBlinkTimer.QueryWaitTime(gameTime))
            {
                showCursor = !showCursor;
            }

            cursorRectangle.Y = DrawRectangle.Y + CURSOR_SPACING;

            cursorBlinkTimer.Update(gameTime);

            textPos.X = DrawRectangle.X + TEXT_EDGE_SPACING_X;
            textPos.Y = DrawRectangle.Y + TEXT_EDGE_SPACING_Y;
            if (checkForClickButton != null)
            {
                checkForClickButton.X = X;
                checkForClickButton.Y = Y;
                checkForClickButton.Update();
            }

            if (Active)
            {
                keyboard = Keyboard.GetState();
                pressedKeys = keyboard.GetPressedKeys();
                key = Microsoft.Xna.Framework.Input.Keys.None;
                if (pressedKeys.Count() > 0)
                {
                    var keys = pressedKeys.Where(x => x != Microsoft.Xna.Framework.Input.Keys.LeftShift &&
                        x != Microsoft.Xna.Framework.Input.Keys.RightShift &&
                        x != Microsoft.Xna.Framework.Input.Keys.LeftControl &&
                        x != Microsoft.Xna.Framework.Input.Keys.RightControl && !lastKeys.Contains(x)).ToList();
                    if (keys.Count > 0)
                    {
                        key = keys[0];
                    }
                    else if (pressedKeys.Contains(lastKey))
                    {
                        // Only allow key spamming if there is only one character key pressed
                        key = lastKey;
                    }
                }
                keyName = key.ToString();
                if (key != Microsoft.Xna.Framework.Input.Keys.None)
                {
                    // We know at this point that we are typing a key, like Enter or Backspace or a letter, number, or symbol
                    if (!spammingKey)
                    {
                        // We must first check to see if we have been pressing the key before
                        if (lastKeys.Contains(key))
                        {
                            // We have to check the long timer
                            pressKeysTimerLong.Update(gameTime);
                            if (pressKeysTimerLong.QueryWaitTime(gameTime))
                            {
                                spammingKey = true;
                                HandleKey(key);
                            }
                        }
                        else
                        {
                            pressKeysTimerLong.Reset();
                            HandleKey(key);
                        }
                    }
                    else
                    {
                        if (lastKeys.Contains(key))
                        {
                            pressKeysTimerShort.Update(gameTime);
                            if (pressKeysTimerShort.QueryWaitTime(gameTime))
                            {
                                // We are pressing & holding a key and can now "spam" it
                                HandleKey(key);
                            }
                        }
                        else
                        {
                            spammingKey = false;
                        }
                    }
                }
                lastKey = key;

                #region Handle Caret Moving

                if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && !arrowKeyPrevDown)
                {
                    arrowKeyPrevDown = true;
                    if (cursorPosition > 0)
                    {
                        cursorRectangle.X -= (int)font.MeasureString(text[cursorPosition - 1].ToString()).X + cursorRectangle.Width;
                        cursorPosition--;
                    }
                }
                else if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && !arrowKeyPrevDown)
                {
                    arrowKeyPrevDown = true;
                    if (cursorPosition < text.Length)
                    {
                        cursorRectangle.X += (int)font.MeasureString(text[cursorPosition].ToString()).X + cursorRectangle.Width;
                        cursorPosition++;
                    }
                }
                else if (!(keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right)))
                {
                    arrowKeyPrevDown = false;
                }

                if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Home))
                {
                    cursorPosition = 0;
                    cursorRectangle.X = DrawRectangle.X + TEXT_EDGE_SPACING_X;
                }
                else if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.End))
                {
                    cursorPosition = text.Length;
                    cursorRectangle.X = DrawRectangle.X + ((int)font.MeasureString(text).X + cursorRectangle.Width);
                }

                #endregion

                #region Paste Support

                if ((keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)) &&
                    keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V) && !shortcutPreviouslyPressed)
                {
                    string clipboardText = Clipboard.GetText();
                    if (!(font.MeasureString(text + clipboardText).X > DrawRectangle.Width))
                    {
                        // The clipboard text is short enough, and will not extend past the textbox's length
                        text += clipboardText;
                    }
                    shortcutPreviouslyPressed = true;
                }
                else if (!((keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)) &&
                    keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V)))
                {
                    shortcutPreviouslyPressed = false;
                }

                #endregion

                MouseState mouse = Mouse.GetState();
                Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);
                if (!(DrawRectangle.Intersects(mouseRect)) && mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    Active = false;
                }

                lastKeys = pressedKeys;
            }
        }
        public void Update(GameTime gameTime, Action whenEnterPressed)
        {
            Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                whenEnterPressed();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Drawn)
            {
                spriteBatch.Draw(image, DrawRectangle, Color.White);
                spriteBatch.DrawString(font, text, textPos, Color.Black);
                if (Active && showCursor)
                {
                    spriteBatch.Draw(cursorImage, cursorRectangle, Color.Black);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, bool drawn)
        {
            Drawn = drawn;
            this.Draw(spriteBatch);
        }

        #endregion

        #region Private Methods

        private void HandleKey(Microsoft.Xna.Framework.Input.Keys key)
        {
            string textToAdd = "";
            if (keyName.Length == 1 && (font.MeasureString(text).X < DrawRectangle.Width - (TEXT_EDGE_SPACING_X * 4)))
            {
                if (!(keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl)))
                {
                    if (!NumbersOnly)
                    {
                        if (HoldingShift(keyboard))
                        {
                            textToAdd = HandleShift(keyboard, key);
                        }
                        else
                        {
                            textToAdd = keyName.ToLower();
                        }
                    }
                }
            }
            else if (key == Microsoft.Xna.Framework.Input.Keys.Space && (font.MeasureString(text).X < DrawRectangle.Width - (TEXT_EDGE_SPACING_X * 4)))
            {
                if (!NumbersOnly)
                {
                    textToAdd = " ";
                }
            }
            else if (key == Microsoft.Xna.Framework.Input.Keys.Back && text.Length > 0 && cursorPosition > 0)
            {
                cursorPosition--;
                text = text.Remove(cursorPosition, 1);
            }
            else if (key == Microsoft.Xna.Framework.Input.Keys.Delete && text.Length > 0 && cursorPosition < text.Length)
            {
                // We've hit the delete key and are not on the last character of the string
                text = text.Remove(cursorPosition, 1);
            }
            else if (keyName.Length > 1 && (font.MeasureString(text).X < DrawRectangle.Width - (TEXT_EDGE_SPACING_X * 4)))
            {
                textToAdd = HandleSymbol(key, keyboard);
            }

            // This is for arrow key support
            text = text.Insert(cursorPosition, textToAdd);

            cursorPosition += textToAdd.Length;
            cursorRectangle.X = DrawRectangle.X + ((int)font.MeasureString(text.Substring(0, cursorPosition)).X + cursorRectangle.Width);
        }

        private string HandleSymbol(Microsoft.Xna.Framework.Input.Keys key, KeyboardState keyboard)
        {
            string returnVal = "";
            if ((!(keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))) ||
                NumbersOnly)
            {
                switch (key)
                {
                    case Microsoft.Xna.Framework.Input.Keys.D0:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad0:
                        returnVal = "0";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D1:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad1:
                        returnVal = "1";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D2:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad2:
                        returnVal = "2";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D3:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad3:
                        returnVal = "3";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D4:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad4:
                        returnVal = "4";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D5:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad5:
                        returnVal = "5";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D6:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad6:
                        returnVal = "6";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D7:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad7:
                        returnVal = "7";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D8:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad8:
                        returnVal = "8";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.D9:
                    case Microsoft.Xna.Framework.Input.Keys.NumPad9:
                        returnVal = "9";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemPeriod:
                    case Microsoft.Xna.Framework.Input.Keys.Decimal:
                        returnVal = ".";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemComma:
                        returnVal = ",";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.Multiply:
                        returnVal = "*";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemPipe:
                        returnVal = @"\";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemPlus:
                        returnVal = "=";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemMinus:
                    case Microsoft.Xna.Framework.Input.Keys.Subtract:
                        returnVal = "-";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets:
                        returnVal = "[";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets:
                        returnVal = "]";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemQuestion:
                    case Microsoft.Xna.Framework.Input.Keys.Divide:
                        returnVal = "/";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemQuotes:
                        returnVal = "'";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemSemicolon:
                        returnVal = ";";
                        break;

                    case Microsoft.Xna.Framework.Input.Keys.OemTilde:
                        returnVal = "`";
                        break;

                    default:
                        returnVal = "";
                        break;
                }
            }
            else
            {
                returnVal = HandleShift(keyboard, key);
            }

            if (NumbersOnly)
            {
                if (returnVal.Length > 0)
                {
                    if (char.IsDigit(returnVal.ToCharArray()[0]))
                    {
                        return returnVal;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return returnVal;
            }
        }

        private bool HoldingShift(KeyboardState keyboard)
        {
            return (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift));
        }

        private string HandleShift(KeyboardState keyboard, Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key.ToString().Length == 1)
            {
                return key.ToString().ToUpper();
            }

            if ((keyboard.GetPressedKeys()[0] == Microsoft.Xna.Framework.Input.Keys.LeftShift ||
                keyboard.GetPressedKeys()[0] == Microsoft.Xna.Framework.Input.Keys.RightShift) && (keyboard.GetPressedKeys().Count() > 1))
            {
                key = keyboard.GetPressedKeys()[1];
            }

            switch (key)
            {
                case Microsoft.Xna.Framework.Input.Keys.D0:
                    return ")";

                case Microsoft.Xna.Framework.Input.Keys.D1:
                    return "!";

                case Microsoft.Xna.Framework.Input.Keys.D2:
                    return "@";

                case Microsoft.Xna.Framework.Input.Keys.D3:
                    return "#";

                case Microsoft.Xna.Framework.Input.Keys.D4:
                    return "$";

                case Microsoft.Xna.Framework.Input.Keys.D5:
                    return "%";

                case Microsoft.Xna.Framework.Input.Keys.D6:
                    return "^";

                case Microsoft.Xna.Framework.Input.Keys.D7:
                    return "&";

                case Microsoft.Xna.Framework.Input.Keys.D8:
                    return "*";

                case Microsoft.Xna.Framework.Input.Keys.D9:
                    return "(";

                case Microsoft.Xna.Framework.Input.Keys.OemPipe:
                    return "|";

                case Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets:
                    return "}";

                case Microsoft.Xna.Framework.Input.Keys.OemComma:
                    return "<";

                case Microsoft.Xna.Framework.Input.Keys.OemMinus:
                    return "_";

                case Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets:
                    return "{";

                case Microsoft.Xna.Framework.Input.Keys.OemPeriod:
                    return ">";

                case Microsoft.Xna.Framework.Input.Keys.OemPlus:
                    return "+";

                case Microsoft.Xna.Framework.Input.Keys.OemQuestion:
                    return "?";

                case Microsoft.Xna.Framework.Input.Keys.OemQuotes:
                    return "\"";

                case Microsoft.Xna.Framework.Input.Keys.OemSemicolon:
                    return ":";

                case Microsoft.Xna.Framework.Input.Keys.OemTilde:
                    return "~";
            }

            return "";
        }

        private void HandleClick()
        {
            if (onClick != null)
            {
                onClick();
            }
            Active = true;
        }

        #endregion
    }
}
