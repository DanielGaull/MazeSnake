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
using System.Text.RegularExpressions;

namespace MazeSnake
{
    class Shop
    {
        #region Fields

        User user;
        List<InventoryItem> itemsToAdd = new List<InventoryItem>();
        List<Skin> skinsToAdd = new List<Skin>();
        Dictionary<Skin, int> skinCosts = new Dictionary<Skin, int>();
        int coinsToRemove = 0;

        ShopState state = ShopState.Powerups;

        ContentManager content;
        
        int speedCost = Costs.PowerupCosts[ItemType.SpeedPowerup];
        int teleportCost = Costs.PowerupCosts[ItemType.TeleportPowerup];
        int wallBreakCost = Costs.PowerupCosts[ItemType.WallBreakPowerup];
        int forcefieldCost = Costs.PowerupCosts[ItemType.ForcefieldPowerup];
        int frozenCost = Costs.PowerupCosts[ItemType.FrozenPowerup];
        //int instaFinCost = INSTAFIN_COST;

        const int powerupWidth = 50;
        const int powerupHeight = 50;

        Dictionary<ItemType, InventoryAction> itemActions;

        SkinAction skinBought;

        // Asset values
        string speedAsset;
        string teleportAsset;
        string wallBreakAsset;
        string instaFinAsset;

        const int ITEMS_PER_ROW = 5;
        const int X_OFFSET = 25;
        const int Y_OFFSET = 80;
        const int SPACING = 10;

        //List<List<ShopItemInterface>> shopPages = new List<List<ShopItemInterface>>();
        //List<ShopItemInterface> shopInterfaces = new List<ShopItemInterface>();

        MenuButton nextPageButton;
        MenuButton prevPageButton;

        List<ShopItemInterface> pwInterfaces = new List<ShopItemInterface>();

        int skinPage;
        List<List<ShopItemInterface>> skinPages = new List<List<ShopItemInterface>>();
        List<ShopItemInterface> skinInterfaces = new List<ShopItemInterface>();

        MenuButton pwButton;
        MenuButton skinButton;

        Action onItemPurchase;

        SpriteFont bigFont;

        int windowWidth;
        int windowHeight;

        #endregion

        #region Constructors

        public Shop(ContentManager content, string speedAsset, string teleportAsset, string wallBreakAsset, 
            string whiteRectSprite, SpriteFont smallFont, string mazeAsset, Dictionary<ItemType, InventoryAction> itemActions, 
            List<Skin> skins, string tongueAsset, SkinAction skinBought, string leftArrowAsset, string rightArrowAsset, 
            int windowWidth, int windowHeight, Action onItemPurchase, SpriteFont bigFont, string textboxAsset, 
            SpriteFont mediumFont, string forcefieldAsset, string frozenAsset)
        {
            this.onItemPurchase = onItemPurchase;

            this.bigFont = bigFont;

            this.content = content;

            this.speedAsset = speedAsset;
            this.teleportAsset = teleportAsset;
            this.wallBreakAsset = wallBreakAsset;
            this.instaFinAsset = mazeAsset;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.skinBought = skinBought;
            this.itemActions = itemActions;

            skinButton = new MenuButton(new OnButtonClick(MakeStateSkins), LanguageTranslator.Translate("Snakes"), content, 0, 0, true, bigFont);
            skinButton.Y = windowHeight - (skinButton.Height * 2) - SPACING;
            pwButton = new MenuButton(new OnButtonClick(MakeStatePowerups), LanguageTranslator.Translate("Powerups"), content, 0, 0, true, bigFont);
            pwButton.Y = skinButton.Y;
            skinButton.Width = pwButton.Width;
            pwButton.X = windowWidth / 2 - (SPACING / 2 + pwButton.Width);
            skinButton.X = windowWidth / 2 + SPACING / 2;

            nextPageButton = new MenuButton(rightArrowAsset, new OnButtonClick(NextPage), content, 0, 0, true);

            prevPageButton = new MenuButton(leftArrowAsset, new OnButtonClick(PrevPage), content, 0, 0, true);
            prevPageButton.Y = windowHeight - (prevPageButton.Height + SPACING);
            prevPageButton.X = SPACING * 2;

            nextPageButton.ImgWidth = prevPageButton.ImgWidth;
            nextPageButton.ImgHeight = prevPageButton.ImgHeight;
            nextPageButton.Width = prevPageButton.Width;
            nextPageButton.Height = prevPageButton.Height;

            nextPageButton.Y = windowHeight - (nextPageButton.Height + SPACING);
            nextPageButton.X = windowWidth - (SPACING + nextPageButton.Width);

            pwInterfaces = new List<ShopItemInterface>()
            {
                new ShopItemInterface(content, whiteRectSprite, speedAsset, smallFont, new OnButtonClick(SpeedBought), 
                    LanguageTranslator.Translate("Speed Powerup"), speedCost, 0, 0, powerupWidth, powerupHeight, ItemType.SpeedPowerup),
                new ShopItemInterface(content, whiteRectSprite, teleportAsset, smallFont, new OnButtonClick(TeleportBought), 
                    LanguageTranslator.Translate("Teleport Powerup"), teleportCost, 0, 0, powerupWidth, powerupHeight, 
                    ItemType.TeleportPowerup),
                new ShopItemInterface(content, whiteRectSprite, wallBreakAsset, smallFont, new OnButtonClick(WallBreakerBought), 
                    LanguageTranslator.Translate("Electric Powerup"), wallBreakCost, 0, 0, powerupWidth, powerupHeight, 
                    ItemType.WallBreakPowerup),
                new ShopItemInterface(content, whiteRectSprite, forcefieldAsset, smallFont, new OnButtonClick(ForcefieldBought),
                    LanguageTranslator.Translate("Forcefield Powerup"), forcefieldCost, 0, 0, powerupWidth, powerupHeight, 
                    ItemType.ForcefieldPowerup),
                new ShopItemInterface(content, whiteRectSprite, frozenAsset, smallFont, new OnButtonClick(FrozenBought),
                    LanguageTranslator.Translate("Frozen Powerup"), frozenCost, 0, 0, powerupWidth, powerupHeight, 
                    ItemType.FrozenPowerup),
                //new ShopItemInterface(content, whiteRectSprite, mazeAsset, smallFont, new OnButtonClick(FinishBought), 
                //    LanguageTranslator.Translate("Finish"), 50, 0, 0, powerupWidth, powerupHeight, ItemType.Finish),
            };

            for (int i = 0; i <= skins.Count - 1; i++)
            {
                // Iterates through the skins collection, making sure that all skins make it into the game
                // The old code was inefficient and filled with bugs
                skinInterfaces.Add(new ShopItemInterface(content, whiteRectSprite, skins[i], smallFont, new SkinAction(SkinBought), Costs.SkinCosts[skins[i].Type],
                    0, 0, Snake.REG_WIDTH, Snake.REG_HEIGHT, tongueAsset));
            }
            UpdateSkinPagesToInterfaces();
        }

        #endregion

        #region Public Methods

        public void Update(ref User user, GameTime gameTime)
        {
            pwButton.Active = state != ShopState.Powerups;
            skinButton.Active = state != ShopState.Skins;
            skinButton.Text = LanguageTranslator.Translate("Snakes");
            pwButton.Text = LanguageTranslator.Translate("Powerups");
            skinButton.Width = pwButton.Width;
            pwButton.X = windowWidth / 2 - (SPACING / 2 + pwButton.Width);
            skinButton.X = windowWidth / 2 + SPACING / 2;

            pwButton.Update();
            skinButton.Update();

            if (skinPage < 0)
            {
                skinPage = 0;
            }
            if (skinPages.Count <= 1)
            {
                // Both buttons should be disabled, as there is only 1 page
                // This must be tested for first, otherwise, since the page always starts equal to 0, the below criteria will be met
                // and the game will act like there are 2 pages when there is only 1
                nextPageButton.Active = false;
                prevPageButton.Active = false;
                // Since there is only one page, we have to set the page integer to 0
                skinPage = 0;
            }
            else if (skinPage == 0)
            {
                // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                nextPageButton.Active = true;
                prevPageButton.Active = false;
            }
            else if (skinPage + 1 == skinPages.Count)
            {
                // We are on the last page, so the forward button must be disabled
                nextPageButton.Active = false;
                prevPageButton.Active = true;
            }
            else
            {
                // We must be somewhere in the middle, so both buttons should be enabled
                nextPageButton.Active = true;
                prevPageButton.Active = true;
            }

            this.user = user;

            int row = 0;
            int lastX = 0;
            int lastY = 0;
            int interfacesInCurrentRow = 0;
            switch (state)
            {
                #region Powerups

                case ShopState.Powerups:

                    row = 0;
                    lastX = X_OFFSET;
                    lastY = Y_OFFSET;
                    interfacesInCurrentRow = 0;
                    if (pwInterfaces.Count > 0)
                    {
                        foreach (ShopItemInterface si in pwInterfaces)
                        {
                            si.X = lastX;
                            lastX += si.Width + SPACING;
                            if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                            {
                                row++;
                                si.X = lastX = X_OFFSET;
                                lastX += si.Width + SPACING;
                                interfacesInCurrentRow = 0;
                            }
                            if (row == 0)
                            {
                                si.Y = lastY;
                            }
                            else if (row == 1)
                            {
                                si.Y = lastY + SPACING + si.Height;
                            }
                            interfacesInCurrentRow++;
                        }
                    }

                    foreach (ShopItemInterface si in pwInterfaces)
                    {
                        si.Update(user.Coins >= si.Cost);
                    }

                    break;

                #endregion

                #region Skins

                case ShopState.Skins:

                    // Update ShopItemInterface values
                    row = 0;
                    lastX = X_OFFSET;
                    lastY = Y_OFFSET;
                    interfacesInCurrentRow = 0;
                    if (skinPage <= skinPages.Count - 1)
                    {
                        foreach (ShopItemInterface si in skinPages[skinPage])
                        {
                            si.X = lastX;
                            lastX += si.Width + SPACING;
                            if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                            {
                                row++;
                                si.X = lastX = X_OFFSET;
                                lastX += si.Width + SPACING;
                                interfacesInCurrentRow = 0;
                            }
                            if (row == 0)
                            {
                                si.Y = lastY;
                            }
                            else if (row == 1)
                            {
                                si.Y = lastY + SPACING + si.Height;
                            }
                            interfacesInCurrentRow++;
                        }
                    }

                    bool skinNotOwned = true;
                    foreach (ShopItemInterface si in skinPages[skinPage])
                    {
                        foreach (Skin s in user.Skins)
                        {
                            if (!(si.ItemSkin.MemberEquals(s)))
                            {
                                // If true, the current ShopItemInterface's skin is owned by the signed-in user
                                skinNotOwned = true;
                            }
                            else
                            {
                                skinNotOwned = false;
                                break;
                                // We have found the skin in the user's inventory. Continued running of this loop may result in this value incorrectly
                                // being set to true
                            }
                        }
                        bool isBuyButtonActive = (user.Coins >= si.Cost) && skinNotOwned;
                        si.Update(isBuyButtonActive);
                    }

                    break;

                #endregion
            }

            // We need to do it this way, because the delegates for when items are bought won't work, 
            // as no parameters can be passed in to an OnButtonClick delegate
            foreach (InventoryItem item in itemsToAdd)
            {
                user.Inventory.Add(item);
            }
            itemsToAdd.Clear();

            foreach (Skin skin in skinsToAdd)
            {
                user.Skins.Add(skin);
            }
            skinsToAdd.Clear();

            nextPageButton.Update();
            prevPageButton.Update();

            user.Coins -= coinsToRemove;
            coinsToRemove = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            pwButton.Draw(spriteBatch);
            skinButton.Draw(spriteBatch);
            switch (state)
            {
                case ShopState.Skins:

                    foreach (ShopItemInterface si in skinPages[skinPage])
                    {
                        si.Draw(spriteBatch);
                    }
                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch);

                    break;

                case ShopState.Powerups:

                    foreach (ShopItemInterface si in pwInterfaces)
                    {
                        si.Draw(spriteBatch);
                    }

                    break;
            }
        }

        public void AddDiscount(float percentage)
        {
            foreach (ShopItemInterface item in skinInterfaces)
            {
                item.Cost = (int)(item.Cost * (1.0 - percentage));
            }
            foreach (ShopItemInterface item in pwInterfaces)
            {
                item.Cost = (int)(item.Cost * (1.0 - percentage));
            }
            speedCost = (int)(speedCost * (1.0 - percentage));
            teleportCost = (int)(teleportCost * (1.0 - percentage));
            wallBreakCost = (int)(wallBreakCost * (1.0 - percentage));
            teleportCost = (int)(teleportCost * (1.0 - percentage));
            frozenCost = (int)(frozenCost * (1.0 - percentage));
        }
        public void RemoveDiscount()
        {
            speedCost = Costs.PowerupCosts[ItemType.SpeedPowerup];
            teleportCost = Costs.PowerupCosts[ItemType.TeleportPowerup];
            wallBreakCost = Costs.PowerupCosts[ItemType.WallBreakPowerup];
            forcefieldCost = Costs.PowerupCosts[ItemType.ForcefieldPowerup];
            frozenCost = Costs.PowerupCosts[ItemType.FrozenPowerup];
            foreach (ShopItemInterface item in skinInterfaces)
            {
                item.Cost = Costs.SkinCosts[item.ItemSkin.Type];
            }
            foreach (ShopItemInterface item in pwInterfaces)
            {
                item.Cost = Costs.PowerupCosts[item.PowerupType];
            }
        }

        #endregion

        #region Private Methods

        #region Item Bought Delegates

        private void SpeedBought()
        {
            itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Speed Powerup"), ItemType.SpeedPowerup));
            coinsToRemove += speedCost;
            onItemPurchase();
        }
        private void TeleportBought()
        {
            itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Teleport Powerup"), ItemType.TeleportPowerup));
            coinsToRemove += teleportCost;
            onItemPurchase();
        }
        private void WallBreakerBought()
        {
            itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Electric Powerup"), ItemType.WallBreakPowerup));
            coinsToRemove += wallBreakCost;
            onItemPurchase();
        }
        private void ForcefieldBought()
        {
            itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Forcefield Powerup"), ItemType.ForcefieldPowerup));
            coinsToRemove += forcefieldCost;
            onItemPurchase();
        }
        private void FrozenBought()
        {
            itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Frozen Powerup"), ItemType.FrozenPowerup));
            coinsToRemove += frozenCost;
            onItemPurchase();
        }
        //private void FinishBought()
        //{
        //    itemsToAdd.Add(new InventoryItem(LanguageTranslator.Translate("Finish"), ItemType.Finish));
        //    coinsToRemove += instaFinCost;
        //    onItemPurchase();
        //}
        private void SkinBought(Skin skin)
        {
            skinsToAdd.Add(skin);
            skinBought(skin);
            foreach (ShopItemInterface item in skinInterfaces)
            {
                if (item.ItemSkin.MemberEquals(skin))
                {
                    // We've found the ShopItemInterface of the skin we need
                    coinsToRemove += item.Cost;
                }
            }
            onItemPurchase();
        }

        #endregion

        private void NextPage()
        {
            skinPage++;
        }
        private void PrevPage()
        {
            skinPage--;
        }

        private void AddSkinInterface(ShopItemInterface si)
        {
            ShopItemInterface interfaceToAdd = si;

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (skinPages.Count > 0)
            {
                for (int i = 0; i < skinPages.Count; i++)
                {
                    if (skinPages[i].Count < ITEMS_PER_ROW * 2)
                    {
                        skinPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        skinPages.Add(new List<ShopItemInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                skinPages.Add(new List<ShopItemInterface>());
                skinPages[0].Add(interfaceToAdd);
            }

            if (skinPages.Count > 0)
            {
                for (int i = 0; i < skinPages.Count; i++)
                {
                    if (skinPages[i].Count == 0)
                    {
                        // The page is empty. If we've reached this point, the page should
                        // NOT be empty
                        skinPages.RemoveAt(i);
                    }
                }
            }
        }
        private void UpdateSkinPagesToInterfaces()
        {
            skinPages.Clear();

            for (int i = 0; i < skinInterfaces.Count; i++)
            {
                AddSkinInterface(skinInterfaces[i]);
            }
        }

        private void MakeStatePowerups()
        {
            state = ShopState.Powerups;
        }
        private void MakeStateSkins()
        {
            state = ShopState.Skins;
        }

        #endregion
    }

    #region Enumerations

    public enum ItemType
    {
        SpeedPowerup,
        TeleportPowerup,
        WallBreakPowerup,
        ForcefieldPowerup,
        FrozenPowerup,
    }

    public enum ShopState
    {
        Powerups,
        Skins,
    }

    #endregion
}
