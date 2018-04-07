using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;

namespace MazeSnake
{
    class InventoryInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        public const int BG_HEIGHT = 100;

        Dictionary<ItemType, InventoryAction> itemActions = new Dictionary<ItemType, InventoryAction>();
        List<InventoryItem> items = new List<InventoryItem>();
        public List<InventoryItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                SetCountVars(items);
            }
        }

        string title = LanguageTranslator.Translate("Inventory");
        Vector2 titlePos;

        string label = "";
        Vector2 labelPos;

        Texture2D speedImg;
        Texture2D teleportImg;
        Texture2D wallBreakImg;
        Texture2D forcefieldImg;
        Texture2D iceImg;
        //Texture2D finImg;
        Rectangle speedRect;
        Rectangle teleportRect;
        Rectangle wallBreakRect;
        Rectangle forcefieldRect;
        Rectangle iceRect;
        //Rectangle finRect;
        Vector2 speedPos;
        Vector2 teleportPos;
        Vector2 wallBreakPos;
        Vector2 forcefieldPos;
        Vector2 icePos;
        //Vector2 finPos;
        int speedCount;
        int teleportCount;
        int wallBreakCount;
        int forcefieldCount;
        int iceCount;
        //int finCount;
        
        const int LABEL_OFFSET = 15;
        
        bool clickStarted = false;
        bool altStarted = false;
        
        const int SPACING = 5;
        const int WIDE_SPACING = SPACING * 9;

        const int START_X = 20;

        SpriteFont font;

        ContentManager content;

        bool hasEffect = false;
        Rectangle effectDisplayBg;
        Texture2D effectDisplayImg;
        Rectangle effectDisplayRect;
        string effectTimeDisplay;
        Vector2 effectTimeLoc;
        const int EFFECT_DISPLAY_WIDTH = 85;
        const int EFFECT_DISPLAY_HEIGHT = 35;
        const int STATUS_EFFECT_SIZE = 25;

        #endregion

        #region Constructors

        public InventoryInterface(string whiteRectAsset, ContentManager content, List<InventoryItem> items, int x, int y, int width,
            SpriteFont font, Dictionary<ItemType, InventoryAction> itemActions, string speedAsset, string teleportAsset, 
            string wallBreakAsset, string forcefieldAsset, string iceAsset)
        {
            this.items = items;
            this.itemActions = itemActions;

            this.font = font;
            this.content = content;

            SetCountVars(items);

            bgImg = content.Load<Texture2D>(whiteRectAsset);

            speedImg = content.Load<Texture2D>(speedAsset);
            teleportImg = content.Load<Texture2D>(teleportAsset);
            wallBreakImg = content.Load<Texture2D>(wallBreakAsset);
            forcefieldImg = content.Load<Texture2D>(forcefieldAsset);
            iceImg = content.Load<Texture2D>(iceAsset);
            //finImg = content.Load<Texture2D>(finishAsset);

            titlePos = new Vector2(bgRect.X + SPACING, bgRect.Y);

            speedRect = new Rectangle(START_X, (int)(titlePos.Y + font.MeasureString(title).Y), speedImg.Width, speedImg.Height);
            teleportRect = new Rectangle(speedRect.X + WIDE_SPACING + speedRect.Width, speedRect.Y, teleportImg.Width, 
                teleportImg.Height);
            wallBreakRect = new Rectangle(teleportRect.X + WIDE_SPACING + teleportRect.Width, teleportRect.Y, wallBreakImg.Width, 
                wallBreakImg.Height);
            forcefieldRect = new Rectangle(wallBreakRect.X + WIDE_SPACING + wallBreakRect.Width, wallBreakRect.Y, forcefieldImg.Width,
                forcefieldImg.Height);
            iceRect = new Rectangle(forcefieldRect.X + WIDE_SPACING + forcefieldRect.Width, forcefieldRect.Y, iceImg.Width,
                iceImg.Height);
            //finRect = new Rectangle(wallBreakRect.X + WIDE_SPACING + wallBreakRect.Width, wallBreakRect.Y, finImg.Width, 
            //finImg.Height);

            speedPos = new Vector2((speedRect.X + speedRect.Width) - (SPACING * 2), (speedRect.Y + speedRect.Height) - (SPACING * 2));
            teleportPos = new Vector2((teleportRect.X + teleportRect.Width) - (SPACING * 2), 
                (teleportRect.Y + teleportRect.Height) - (SPACING * 2));
            wallBreakPos = new Vector2((wallBreakRect.X + wallBreakRect.Width) - (SPACING * 2), 
                (wallBreakRect.Y + wallBreakRect.Height) - (SPACING * 2));
            forcefieldPos = new Vector2((forcefieldRect.X + forcefieldRect.Width) - (SPACING * 2),
                (forcefieldRect.Y + forcefieldRect.Height) - (SPACING * 2));
            icePos = new Vector2((iceRect.X + iceRect.Width) - (SPACING * 2), (iceRect.Y + iceRect.Height) - (SPACING * 2));
            //finPos = new Vector2((finRect.X + finRect.Width) - (SPACING * 2), (finRect.Y + finRect.Height) - (SPACING * 2));

            bgRect = new Rectangle(x, y, width, BG_HEIGHT);

            labelPos = new Vector2();

            effectDisplayBg = new Rectangle(bgRect.Right - WIDE_SPACING * 2 - EFFECT_DISPLAY_WIDTH, 
                bgRect.Bottom - SPACING - EFFECT_DISPLAY_HEIGHT, EFFECT_DISPLAY_WIDTH, EFFECT_DISPLAY_HEIGHT);
            effectDisplayRect = new Rectangle(effectDisplayBg.X + SPACING, effectDisplayBg.Top + SPACING,
                STATUS_EFFECT_SIZE, STATUS_EFFECT_SIZE);
            effectTimeLoc = new Vector2(effectDisplayRect.Right + SPACING, effectDisplayRect.Y);
        }

        #endregion

        #region Public Methods

        public void Update(Effect effect, int time, GameMode gameMode)
        {
            labelPos.X = Mouse.GetState().X + LABEL_OFFSET;
            labelPos.Y = Mouse.GetState().Y;

            CheckPowerupClicks(gameMode);
            if (Mouse.GetState().LeftButton != ButtonState.Pressed)
            {
                clickStarted = false;
            }
            if (!(Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt)))
            {
                altStarted = false;
            }

            titlePos.X = bgRect.X + SPACING;
            titlePos.Y = bgRect.Y;

            hasEffect = (effect != Effect.None);
            if (hasEffect)
            {
                effectTimeDisplay = "00:" + time.ToString("00");
                switch (effect)
                {
                    case Effect.Speed:
                        effectDisplayImg = speedImg;
                        break;
                    case Effect.WallBreaker:
                        effectDisplayImg = wallBreakImg;
                        break;
                    case Effect.Forcefield:
                        effectDisplayImg = forcefieldImg;
                        break;
                    case Effect.Frozen:
                        effectDisplayImg = iceImg;
                        break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            //if (isOpen || opening || closing)
            //{
            //    for (int i = 0; i < items.Count; i++)
            //    {
            //        spriteBatch.Draw(itemImgs[i], itemRects[i], Color.White);
            //    }
            //}
            spriteBatch.DrawString(font, title, titlePos, Color.Black);

            spriteBatch.Draw(speedImg, speedRect, Color.White);
            spriteBatch.Draw(teleportImg, teleportRect, Color.White);
            spriteBatch.Draw(wallBreakImg, wallBreakRect, Color.White);
            spriteBatch.Draw(forcefieldImg, forcefieldRect, Color.White);
            spriteBatch.Draw(iceImg, iceRect, Color.White);
            //spriteBatch.Draw(finImg, finRect, Color.White);

            spriteBatch.DrawString(font, "x" + speedCount, speedPos, Color.Black);
            spriteBatch.DrawString(font, "x" + teleportCount, teleportPos, Color.Black);
            spriteBatch.DrawString(font, "x" + wallBreakCount, wallBreakPos, Color.Black);
            spriteBatch.DrawString(font, "x" + forcefieldCount, forcefieldPos, Color.Black);
            spriteBatch.DrawString(font, "x" + iceCount, icePos, Color.Black);
            //spriteBatch.DrawString(font, "x" + finCount, finPos, Color.Black);

            if (hasEffect)
            {
                spriteBatch.Draw(bgImg, effectDisplayBg, new Color(100, 100, 100));
                spriteBatch.Draw(effectDisplayImg, effectDisplayRect, Color.White);
                spriteBatch.DrawString(font, effectTimeDisplay, effectTimeLoc, Color.White);
            }

            if (AreAnyItemsBeingHovered())
            {
                // An item has the mouse hovering over it, so we must draw the item label
                spriteBatch.DrawString(font, label, labelPos, Color.Black);
            }
        }

        #endregion

        #region Private Methods

        private void CheckPowerupClicks(GameMode gameMode)
        {
            MouseState mouse = Mouse.GetState();

            if (speedRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            {
                // This item has the mouse hovering over it
                label = LanguageTranslator.Translate("Speed") + "\nAlt+S";
            }
            if ((((speedRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
                !clickStarted) || (IsAlt(Keys.S) && !altStarted))) && speedCount > 0)
            {
                // We have used the speed powerup
                itemActions[ItemType.SpeedPowerup]();
                clickStarted = true;
                altStarted = true;
            }

            if (teleportRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            {
                // This item has the mouse hovering over it
                label = LanguageTranslator.Translate("Teleport") + "\nAlt+T";
            }
            if ((((teleportRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
                !clickStarted) || (IsAlt(Keys.T) && !altStarted))) && teleportCount > 0)
            {
                // We have used the speed powerup
                itemActions[ItemType.TeleportPowerup]();
                clickStarted = true;
                altStarted = true;
            }

            if (wallBreakRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            {
                // This item has the mouse hovering over it
                label = LanguageTranslator.Translate("Electric") + "\nAlt+W";
            }
            if ((((wallBreakRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
                !clickStarted) || (IsAlt(Keys.W) && !altStarted))) && wallBreakCount > 0)
            {
                // We have used the speed powerup
                itemActions[ItemType.WallBreakPowerup]();
                clickStarted = true;
                altStarted = true;
            }

            if (forcefieldRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            {
                // This item has the mouse hovering over it
                label = LanguageTranslator.Translate("Forcefield") + "\nAlt+F";
            }
            if ((((forcefieldRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
                !clickStarted) || (IsAlt(Keys.F) && !altStarted))) && forcefieldCount > 0 && 
                !(gameMode == GameMode.Freeplay || gameMode == GameMode.Star))
            {
                // We have used the speed powerup
                itemActions[ItemType.ForcefieldPowerup]();
                clickStarted = true;
                altStarted = true;
            }

            if (iceRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            {
                // This item has the mouse hovering over it
                label = LanguageTranslator.Translate("Frozen") + "\nAlt+R";
            }
            if ((((iceRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
                !clickStarted) || (IsAlt(Keys.R) && !altStarted))) && iceCount > 0 && gameMode != GameMode.Freeplay)
            {
                // We have used the speed powerup
                itemActions[ItemType.FrozenPowerup]();
                clickStarted = true;
                altStarted = true;
            }

            //if (finRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)))
            //{
            //    // This item has the mouse hovering over it
            //    label = LanguageTranslator.Translate("Finish") + "\nAlt+F";
            //}
            //if ((((finRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && mouse.LeftButton == ButtonState.Pressed &&
            //    !clickStarted) || (IsAlt(Keys.F) && !altStarted))) && finCount > 0)
            //{
            //    // We have used the speed powerup
            //    itemActions[ItemType.Finish]();
            //    clickStarted = true;
            //    altStarted = true;
            //}
        }

        private bool IsAlt(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            return (state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt)) && state.IsKeyDown(key);
        }

        //private void OnArrowClick()
        //{
        //    if (isOpen)
        //    {
        //        arrowEffect = SpriteEffects.FlipHorizontally;
        //        closing = true;
        //    }
        //    else
        //    {
        //        arrowEffect = SpriteEffects.None;
        //        opening = true;
        //    }
        //}

        private bool AreAnyItemsBeingHovered()
        {
            MouseState mouseState = Mouse.GetState();
            Rectangle mouse = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            return (speedRect.Intersects(mouse)) || (teleportRect.Intersects(mouse)) || (wallBreakRect.Intersects(mouse))
                /* || (finRect.Intersects(mouse))*/ || (forcefieldRect.Intersects(mouse)) || (iceRect.Intersects(mouse));
        }

        private void SetCountVars(List<InventoryItem> items)
        {
            speedCount = 0;
            teleportCount = 0;
            wallBreakCount = 0;
            forcefieldCount = 0;
            iceCount = 0;
            //finCount = 0;

            foreach (InventoryItem item in items)
            {
                switch (item.Type)
                {
                    case ItemType.SpeedPowerup:
                        speedCount++;
                        break;
                    case ItemType.TeleportPowerup:
                        teleportCount++;
                        break;
                    case ItemType.WallBreakPowerup:
                        wallBreakCount++;
                        break;
                    case ItemType.ForcefieldPowerup:
                        forcefieldCount++;
                        break;
                    case ItemType.FrozenPowerup:
                        iceCount++;
                        break;
                        //case ItemType.Finish:
                        //    finCount++;
                        //    break;
                }
            }
        }

        #endregion
    }
}
