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
    class GiftGiver
    {
        #region Fields & Properties

        MenuButton button;

        readonly DateTime USER_INIT_GIFT_TIME = new DateTime(1, 1, 1, 0, 0, 0, 0);
        string timeUntilGift = "";
        public const int HOURS_UNTIL_NEXT_GIFT = 22;

        public GiftRewardType Type;
        Snake drawSnake;
        // Only use if reward isn't a skin
        Texture2D itemImg;
        Rectangle itemRect;
        Color itemColor = Color.White;

        string popupText = "";

        bool showingGift;

        Popup rewardPopup;

        List<Skin> skins;
        List<InventoryItem> items;

        Action onReward;

        const int WIDTH = 50;
        const int HEIGHT = 50;
        const int SPACING = 5;

        User user;

        Random rand;

        const int MONEY_CHANCE = 55;
        const int SKIN_CHANCE = 15;
        const int ITEM_CHANCE = 30;

        const int MONEY_MAX = 1000;
        const int MONEY_MIN = 50;
        const int XP_MAX = 1000;
        const int XP_MIN = 50;

        ContentManager content;
        string coinAsset;
        string tongueAsset;
        Dictionary<ItemType, string> itemAssets;
        string xpAsset;

        int windowWidth;
        int windowHeight;

        SkinAction skinAdded;

        public bool ShowingPopup
        {
            get
            {
                if (rewardPopup != null)
                {
                    return rewardPopup.Active;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Constructors

        public GiftGiver(List<Skin> skins, List<InventoryItem> items, ContentManager content, string bgAsset, string interiorAsset, int x, int y, Random rand,
            Action onReward, string tongueAsset, string coinAsset, Dictionary<ItemType, string> itemAssets, string xpAsset, SpriteFont smallFont,
            SpriteFont bigFont, int windowWidth, int windowHeight, SkinAction skinAdded)
        {
            this.content = content;
            this.tongueAsset = tongueAsset;
            this.coinAsset = coinAsset;
            this.itemAssets = itemAssets;
            this.xpAsset = xpAsset;

            this.skinAdded = skinAdded;

            this.skins = skins;
            this.items = items;
            this.rand = rand;
            this.onReward = onReward;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            button = new MenuButton(interiorAsset, new OnButtonClick(RewardGift), content, x, y, true);
            button.ImgWidth = WIDTH - (SPACING * 2);
            button.ImgHeight = HEIGHT - (SPACING * 2);
            button.Width = WIDTH;
            button.Height = HEIGHT;

            rewardPopup = new Popup(content, smallFont, bgAsset, windowWidth, windowHeight, false, bigFont);
        }

        #endregion

        #region Public Methods

        public void Update(User user)
        {
            this.user = user;
            button.Update();
            if (rewardPopup.Active)
            {
                rewardPopup.Update();
            }
            if (user != null)
            {
                TimeSpan nextGiftTime = user.LastReceivedGift.AddHours(HOURS_UNTIL_NEXT_GIFT) - DateTime.Now;
                timeUntilGift = nextGiftTime.Hours.ToString("00") + ":" + nextGiftTime.Minutes.ToString("00") + ":" + 
                    nextGiftTime.Seconds.ToString("00");
                if (rewardPopup.Active && !showingGift)
                {
                    rewardPopup.Text = string.Format("You have already collected your daily gift. You can receive another in {0}.", 
                        timeUntilGift);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            button.Draw(spriteBatch);
            if (rewardPopup.Active)
            {
                rewardPopup.Draw(spriteBatch);
                if (showingGift)
                {
                    if (itemImg != null)
                    {
                        spriteBatch.Draw(itemImg, itemRect, itemColor);
                    }
                    else if (drawSnake != null)
                    {
                        drawSnake.Draw(spriteBatch);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void RewardGift()
        {
            // Users created before the update will have a bad gift time (1/1/0001 12:00:00.00 AM),
            // so we must make sure that the player can receive a gift
            if (user.LastReceivedGift == USER_INIT_GIFT_TIME)
            {
                user.LastReceivedGift = DateTime.Now.AddHours(0 - HOURS_UNTIL_NEXT_GIFT);
            }
            if (DateTime.Compare(user.LastReceivedGift.AddHours(HOURS_UNTIL_NEXT_GIFT), DateTime.Now) <= 0)
            {
                showingGift = true;

                // First make sure we don't have a special holiday gift for our player
                if (Holidays.IsChristmasWeek())
                {
                    AddRandCoins(rand, user, 3, "Merry Christmas! You've received 3x the coins! You've received {c} coins!");
                }
                else
                {
                    int randomNum = rand.Next(100);
                    if (randomNum <= MONEY_CHANCE)
                    {
                        //(int)Math.Floor((double)(Math.Abs(rand.Next(MONEY_MAX) - rand.Next(MONEY_MAX)) * (1 + MONEY_MAX - MONEY_MIN) + MONEY_MIN))
                        AddRandCoins(rand, user, 1);
                    }
                    else if (randomNum > MONEY_CHANCE && randomNum <= ITEM_CHANCE + MONEY_CHANCE)
                    {
                        int index = rand.Next(items.Count - 1);
                        user.AddItem(items[index]);
                        popupText = "You've received a " + items[index].Name + "!";
                        Type = GiftRewardType.Item;
                        itemImg = content.Load<Texture2D>(itemAssets[items[index].Type]);
                        itemRect = new Rectangle(0, 0, WIDTH, HEIGHT);
                        itemColor = Color.White;
                    }
                    else if (randomNum > MONEY_CHANCE + ITEM_CHANCE && randomNum <= SKIN_CHANCE + ITEM_CHANCE + MONEY_CHANCE)
                    {
                        int index = 0;
                        int counter = 0;
                        bool addingCoins = false;
                        do
                        {
                            index = rand.Next(skins.Count - 1);
                            counter++;
                            if (counter == skins.Count)
                            {
                                AddRandCoins(rand, user, 1);
                                addingCoins = true;
                                break;
                            }
                        }
                        while (user.Skins.Contains(skins[index]));

                        if (!addingCoins)
                        {
                            user.AddSkin(skins[index]);
                            skinAdded(skins[index]);
                            popupText = "You've received the " + skins[index].Name + "!";
                            Type = GiftRewardType.Skin;
                            drawSnake = new Snake(skins[index], 0, 0, content, tongueAsset);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The added value of all CHANCEs must equal 100 (private void Gift.RewardGift()).");
                    }
                }
                rewardPopup.ShowPopup(popupText, false, windowWidth / 2 - (rewardPopup.Width / 2), windowHeight / 2 - (rewardPopup.Height / 2));
                if (itemRect != null)
                {
                    itemRect.X = rewardPopup.X + (rewardPopup.Width / 2 - (itemRect.Width / 2));
                    itemRect.Y = rewardPopup.Y + (rewardPopup.Height / 2 - (itemRect.Height / 2));
                }
                if (drawSnake != null)
                {
                    drawSnake.X = rewardPopup.X + (rewardPopup.Width / 2 - (drawSnake.Width / 2));
                    drawSnake.Y = rewardPopup.Y + (rewardPopup.Height / 2 - (drawSnake.Height / 2));
                }
                if (onReward != null)
                {
                    onReward();
                }
                user.ChangeLastReceivedGift(DateTime.Now);
            }
            else
            {
                showingGift = false;
                rewardPopup.ShowPopup(String.Format("You have already collected your daily gift. You can receive another in {0}.", timeUntilGift),
                    false, windowWidth / 2 - (rewardPopup.Width / 2), windowHeight / 2 - (rewardPopup.Height / 2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="user"></param>
        /// <param name="multiplier"></param>
        /// <param name="formatText">Format using {c} to represent coins</param>
        private void AddRandCoins(Random rand, User user, int multiplier, string formatText)
        {
            int coins = (int)(MONEY_MAX * Math.Pow(rand.Next(10) / 10.0f, 4.0f));
            coins -= coins % 5;
            if (coins < MONEY_MIN)
            {
                coins = MONEY_MIN;
            }
            coins *= multiplier;
            user.AddCoins(coins);
            Type = GiftRewardType.Coins;
            itemImg = content.Load<Texture2D>(coinAsset);
            itemRect = new Rectangle(0, 0, WIDTH, HEIGHT);
            itemColor = Color.White;
            popupText = formatText.Replace("{c}", coins.ToString());
        }
        private void AddRandCoins(Random rand, User user, int multiplier)
        {
            this.AddRandCoins(rand, user, multiplier, "You've received {c} coins!");
        }

        #endregion
    }
    #region Enums

    public enum GiftRewardType
    {
        Coins,
        Xp,
        Item,
        Skin
    }

    #endregion
}
