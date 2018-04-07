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
    public delegate void SkinAction(Skin skin);
    class ShopItemInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        Color bgColor = Color.White;

        Texture2D itemImg;
        Rectangle itemRect;
        public Skin ItemSkin;
        ShopItemType itemType;
        public ShopItemType ItemType
        {
            get
            {
                return itemType;
            }
        }
        public ItemType PowerupType;

        Snake snake;

        SkinAction skinBought;

        string costString = "";
        Vector2 costPos;
        public int Cost;

        string name = "";
        Vector2 namePos;

        SpriteFont font;

        MenuButton buyButton;

        const int BG_WIDTH = 145;
        const int BG_HEIGHT = 170;

        const int SPACING = 5;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
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
            }
        }

        public bool Active
        {
            get
            {
                return buyButton.Active;
            }
            set
            {
                buyButton.Active = value;
            }
        }

        #endregion

        #region Constructors

        public ShopItemInterface(ContentManager content, string whiteRectAsset, string itemAsset, SpriteFont smallFont, OnButtonClick buyClicked,
            string itemName, int cost, int x, int y, int itemWidth, int itemHeight, ItemType powerupType)
        {
            // Initialize image objects
            bgImg = content.Load<Texture2D>(whiteRectAsset);
            itemImg = content.Load<Texture2D>(itemAsset);
            costString = cost.ToString() + "c";
            name = itemName;

            this.Cost = cost;
            this.font = smallFont;

            bgColor.R = 124;
            bgColor.G = 216;
            bgColor.B = 247;

            // Initialize positioning objects
            bgRect = new Rectangle(x, y, BG_WIDTH, BG_HEIGHT);
            namePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(name).X / 2)), bgRect.Y + SPACING);

            buyButton = new MenuButton(buyClicked, LanguageTranslator.Translate("Buy") + " (" + cost + "c)", content, 0, 0, true, font);
            buyButton.X = bgRect.X + (bgRect.Width / 2 - (buyButton.Width / 2));
            buyButton.Y = bgRect.Bottom - buyButton.Height - SPACING;
            if (cost == 0)
            {
                buyButton.Text = LanguageTranslator.Translate("Buy") + " (" + LanguageTranslator.Translate("Free") + ")";
            }

            costPos = new Vector2(bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(costString).X / 2)),
                buyButton.Y - SPACING - (int)(font.MeasureString(costString).Y));

            itemRect = new Rectangle(bgRect.X + (bgRect.Width / 2 - (itemWidth / 2)), 0, itemWidth, itemHeight);
            itemRect.Y = (int)costPos.Y - itemRect.Height - SPACING;

            this.PowerupType = powerupType;
            itemType = ShopItemType.Powerup;
        }
        public ShopItemInterface(ContentManager content, string whiteRectAsset, Skin itemSkin, SpriteFont smallFont, SkinAction buyClicked,
            int cost, int x, int y, int itemWidth, int itemHeight, string tongueAsset)
        {
            // Initialize image objects
            bgImg = content.Load<Texture2D>(whiteRectAsset);
            costString = cost.ToString() + "c";
            name = itemSkin.Name;

            this.skinBought = buyClicked;
            this.ItemSkin = itemSkin;
            this.Cost = cost;
            this.font = smallFont;

            bgColor.R = 124;
            bgColor.G = 216;
            bgColor.B = 247;

            // Initialize positioning objects
            bgRect = new Rectangle(x, y, BG_WIDTH, BG_HEIGHT);
            namePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(name).X / 2)), bgRect.Y + SPACING);

            buyButton = new MenuButton(new OnButtonClick(SkinClicked), LanguageTranslator.Translate("Buy") + " (" + cost + "c)", content, 0, 0, true, font);
            buyButton.X = bgRect.X + (bgRect.Width / 2 - (buyButton.Width / 2));
            buyButton.Y = bgRect.Bottom - buyButton.Height - SPACING;

            costPos = new Vector2(bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(costString).X / 2)),
                buyButton.Y - SPACING - (int)(font.MeasureString(costString).Y));

            snake = new Snake(itemSkin, 0, 0, content, tongueAsset);
            snake.X = bgRect.X + (bgRect.Width / 2 - (snake.Width / 2));
            snake.Y = (int)costPos.Y - snake.Height - SPACING;

            itemType = ShopItemType.Skin;
        }

        #endregion

        #region Public Methods

        public void Update(bool isBuyButtonActive)
        {
            // Update Values
            buyButton.Text = LanguageTranslator.Translate("Buy") + " (" + Cost + "c)";
            name = LanguageTranslator.Translate(name);
            costString = Cost + "c";
            if (Cost == 0)
            {
                buyButton.Text = LanguageTranslator.Translate("Buy") + " (" + LanguageTranslator.Translate("Free") + ")";
            }

            namePos.X = bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(name).X / 2));
            namePos.Y = bgRect.Y + SPACING;
            if (itemType == ShopItemType.Powerup)
            {
                costPos.Y = buyButton.Y - SPACING - (int)(font.MeasureString(costString).Y);
                itemRect.X = bgRect.X + (bgRect.Width / 2 - (itemRect.Width / 2));
                itemRect.Y = (int)costPos.Y - itemRect.Height - SPACING;
            }
            else
            {
                costPos.Y = buyButton.Y - SPACING - (int)(font.MeasureString(costString).Y);
                snake.X = bgRect.X + (bgRect.Width / 2 - (snake.Width / 2));
                snake.Y = (int)costPos.Y - snake.Height - SPACING;
            }

            costPos.X = bgRect.X + (bgRect.Width / 2 - (int)(font.MeasureString(costString).X / 2));
            buyButton.X = bgRect.X + (bgRect.Width / 2 - (buyButton.Width / 2));
            buyButton.Y = bgRect.Bottom - buyButton.Height - SPACING;

            buyButton.Active = isBuyButtonActive;

            // Update Objects
            buyButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, bgColor);
            spriteBatch.DrawString(font, name, namePos, Color.DarkGoldenrod);
            if (itemType == ShopItemType.Powerup)
            {
                spriteBatch.Draw(itemImg, itemRect, Color.White);
            }
            else
            {
                snake.Draw(spriteBatch);
            }
            spriteBatch.DrawString(font, costString, costPos, Color.DarkGoldenrod);
            buyButton.Draw(spriteBatch);
        }

        #endregion

        #region Private Methods

        private void SkinClicked()
        {
            if (itemType == ShopItemType.Skin)
            {
                skinBought(ItemSkin);
            }
        }

        #endregion
    }

    public enum ShopItemType
    {
        Skin,
        Powerup
    }
}
