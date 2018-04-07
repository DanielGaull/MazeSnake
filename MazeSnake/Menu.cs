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
    class Menu
    {
        #region Fields & Properties

        MenuButton playButton;
        MenuButton quitButton;

        MenuButton resumeButton;
        MenuButton preLoginButton;

        ToggleButton muteButton;
        const int MUTE_BUTTON_SIZE = 100;

        const float MIN_SCROLL_VAL = -1164.0f;
        const float MAX_SCROLL_VAL = 570.0f;

        GiftGiver gift;

        MenuButton submitInfoButton;
        Textbox usernameBox;
        string usernamePrefix = LanguageTranslator.Translate("Username") + ": ";
        Vector2 usernamePrePos = new Vector2();

        const int MAX_INTERFACE_Y = 500;

        Popup needsRequiredFields;

        MenuButton createUserButton;
        List<UserInterface> userInterfaces = new List<UserInterface>();

        int userPage = 0;
        const int USER_INTERFACES_PER_PAGE = 4;
        MenuButton nextPageButton;
        MenuButton prevPageButton;
        List<List<UserInterface>> userPages = new List<List<UserInterface>>();

        int achPage = 0;
        const int ACHIEVEMENTS_PER_PAGE = 4;
        List<List<AchievementInterface>> achPages = new List<List<AchievementInterface>>();
        List<AchievementInterface> achInterfaces = new List<AchievementInterface>();
        MenuButton achievementButton;

        MenuButton playAsUserButton;
        MenuButton mainMenuButton;
        MenuButton statsButton;

        MenuButton pickSkinButton;
        int skinPage = 0;
        const int SKINS_PER_ROW = 4;
        const int ROWS_PER_PAGE = 2;
        List<List<SkinInterface>> skinPages = new List<List<SkinInterface>>();
        List<SkinInterface> skinInterfaces = new List<SkinInterface>();

        GameState state;

        FillBar achBar;
        string achBarTxt = "";
        Vector2 achBarTxtPos;

        Texture2D titleImg;
        Rectangle titleRect;
        const int TITLE_WIDTH = 200;
        const int TITLE_HEIGHT = 50;

        Snake drawSnake;

        Texture2D grayImage;
        Rectangle grayRectangle;
        Color transColor;

        Coin displayCoin;
        Texture2D coinDisplayBgImg;
        Rectangle coinDisplayBgRect;
        public const int COIN_BG_WIDTH = 200;
        public const int COIN_BG_HEIGHT = 25;
        public int CoinDisplayX
        {
            get
            {
                return coinDisplayBgRect.X;
            }
        }
        public int CoinDisplayY
        {
            get
            {
                return coinDisplayBgRect.Y;
            }
        }

        SpriteFont bigFont;
        SpriteFont mediumFont;
        SpriteFont smallFont;

        const int YSPACING = 5;
        const int XSPACING = 50;
        const int BUTTON_Y_SPACING = 20;

        string askText = "";
        Vector2 askTextPos;

        int windowWidth;
        int windowHeight;

        bool isMazeWorm = false;
        Skin snakeSkin;

        ContentManager content;

        MenuButton languageButton;
        Dictionary<Language, MenuButton> languageButtons = new Dictionary<Language, MenuButton>();

        MenuButton helpButton;
        Tutorial tutorial;

        Shop shop;
        MenuButton shopButton;

        OnUserButtonClick onTrashClick;
        OnUserButtonClick whenUsernameClicked;
        OnUserCreation whenUserCreated;
        SkinAction skinSelected;

        ValueSlider volumeSlider;
        const int VOLUME_SLIDER_WIDTH = 600;
        const int VOLUME_SLIDER_HEIGHT = 25;
        Textbox changeUsernameBox;
        Checkbox showMouseWhilePlayingBox;
        MenuButton saveSettingsButton;
        User user;
        MenuButton settingsButton;

        MenuButton creditsButton;
        float creditsScrollValue = 0;
        float prevScrollVal = 0;
        const int SCROLL_DIVISION = 10;

        MenuButton classicButton;
        MenuButton freeplayButton;
        MenuButton starButton;

        public bool ShowingLogo = true;
        Timer logoTimer;
        Texture2D duoplusLogo;
        Rectangle logoRect;
        float logoAlpha = 0.0f;
        const float LOGO_ALPHA_CHANGE = 0.01f;
        Texture2D logoBgImg;
        Rectangle logoBgRect;
        bool undarkeningLogo = true;
        bool fadingLogo = false;
        bool waitingToFade = false;
        Color logoBgColor = Color.Black;

        string MAZES_COMPLETED_PREFIX = LanguageTranslator.Translate("Mazes Completed") + ": ";
        string POWERUPS_PREFIX = LanguageTranslator.Translate("Powerups Collected") + ": ";
        string TELEPORT_PREFIX = LanguageTranslator.Translate("Times Teleported") + ": ";
        string SPEED_PREFIX = LanguageTranslator.Translate("Seconds with Speed") + ": ";
        string COINS_PREFIX = LanguageTranslator.Translate("Coins Collected") + ": ";
        string WALL_BREAK_PREFIX = LanguageTranslator.Translate("Walls Broken") + ": ";
        string ACHIEVEMENT_STAT_PREFIX = LanguageTranslator.Translate("Achievements Completed") + ": ";
        string ENEMIES_AVOIDED_PREFIX = LanguageTranslator.Translate("Enemies Avoided") + ": ";
        string TIME_FROZEN_PREFIX = LanguageTranslator.Translate("Time Frozen") + ": ";

        string trashAsset = "";
        string whiteRectangleAsset = "";
        string whiteCircleAsset = "";
        string coinAsset = "";
        string lockSprite = "";
        string checkSprite = "";
        string checkboxSprite = "";
        string tongueAsset = "";
        string textboxAsset = "";

        #endregion

        #region Constructors

        public Menu(ContentManager content, MouseCursor cursor, int windowWidth, int windowHeight, Skin snakeSkin,
            OnButtonClick playButtonClicked, OnButtonClick quitButtonClicked, OnButtonClick resumeButtonClicked,
            OnButtonClick mainMenuButtonClicked, OnButtonClick createUserClicked, OnButtonClick achievementsClicked,
            OnButtonClick playAsUserButtonClicked, OnButtonClick backToPostLoginClicked, OnButtonClick statsClicked,
            OnButtonClick shopClicked, OnButtonClick pickSkinClicked, OnButtonClick settingsClicked, string whiteRectangleAsset,
            byte alphaValue, string trashAsset, OnUserButtonClick whenUsernameClicked, OnUserButtonClick whenTrashClicked,
            string textboxAsset, string checkboxAsset, string checkAsset, OnUserCreation whenUserCreated, SkinAction skinSelected,
            Color backgroundColor, string whiteCircleAsset, string coinAsset, string lockSprite, string checkSprite, SpriteFont bigFont,
            SpriteFont mediumFont, SpriteFont smallFont, string mazeSnakeLogoAsset, string purasuLogoAsset, string powerupAsset,
            string mazeAsset, Dictionary<ItemType, InventoryAction> itemActions, string tongueAsset, bool isMazeWorm, List<Skin> skins,
            string rightArrowAsset, string leftArrowAsset, string gearAsset, OnButtonClick creditsClicked, OnButtonClick helpClicked,
            List<string> tutorialAssets, OnButtonClick languageClicked, string globeAsset,
            Dictionary<Language, OnButtonClick> langsClickedList, float discountPercent, Action onItemPurchase,
            List<InventoryItem> items, Dictionary<ItemType, string> itemAssets, Random rand, string giftAsset, string soundAsset,
            string crossedOutAsset, Action<GameMode> gameModeSelected, List<int> achIdOrder, string hatAsset)
        {
            this.trashAsset = trashAsset;
            this.whiteRectangleAsset = whiteRectangleAsset;
            this.whiteCircleAsset = whiteCircleAsset;
            this.coinAsset = coinAsset;
            this.lockSprite = lockSprite;
            this.checkSprite = checkSprite;
            this.checkboxSprite = checkboxAsset;
            this.tongueAsset = tongueAsset;
            this.textboxAsset = textboxAsset;

            this.whenUsernameClicked = whenUsernameClicked;
            this.whenUserCreated = whenUserCreated;
            this.onTrashClick = whenTrashClicked;
            this.skinSelected = skinSelected;

            this.content = content;

            this.isMazeWorm = isMazeWorm;
            this.snakeSkin = snakeSkin;

            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;

            this.bigFont = bigFont;
            this.mediumFont = mediumFont;
            this.smallFont = smallFont;

            logoTimer = new Timer(3000, TimerUnits.Milliseconds);
            duoplusLogo = content.Load<Texture2D>(purasuLogoAsset);
            logoRect = new Rectangle(0, 0, windowWidth, windowHeight);
            logoBgImg = content.Load<Texture2D>(whiteRectangleAsset);
            logoBgRect = new Rectangle(0, 0, windowWidth, windowHeight);

            titleImg = content.Load<Texture2D>(mazeSnakeLogoAsset);
            titleRect = new Rectangle(windowWidth / 2 - (TITLE_WIDTH / 2), 0, TITLE_WIDTH, TITLE_HEIGHT);

            coinDisplayBgImg = content.Load<Texture2D>(whiteRectangleAsset);
            coinDisplayBgRect = new Rectangle(windowWidth - COIN_BG_WIDTH - YSPACING, YSPACING, COIN_BG_WIDTH, COIN_BG_HEIGHT);
            displayCoin = new Coin(coinAsset, content, coinDisplayBgRect.X + YSPACING, coinDisplayBgRect.Y, COIN_BG_HEIGHT - YSPACING, COIN_BG_HEIGHT - YSPACING, 0);

            askTextPos = new Vector2(windowWidth / 2 - ((int)smallFont.MeasureString(askText).X / 2),
                titleRect.Y + YSPACING + titleRect.Height);

            drawSnake = new Snake(snakeSkin, 0, 0, content, tongueAsset);

            playButton = new MenuButton(playButtonClicked, LanguageTranslator.Translate("Start"), content, 0, 0, true, bigFont);
            playButton.X = windowWidth / 2 - (playButton.Width / 2);
            playButton.Y = (int)askTextPos.Y + BUTTON_Y_SPACING;

            creditsButton = new MenuButton(creditsClicked, LanguageTranslator.Translate("Credits"), content, 0, 0, true, bigFont);
            creditsButton.X = windowWidth / 2 - (creditsButton.Width / 2);
            creditsButton.Y = windowHeight - creditsButton.Height - BUTTON_Y_SPACING;

            quitButton = new MenuButton(quitButtonClicked, LanguageTranslator.Translate("Quit"), content, 0, 0, true, bigFont);
            quitButton.X = windowWidth / 2 - (quitButton.Width / 2);
            quitButton.Y = playButton.Y + BUTTON_Y_SPACING + quitButton.Height;

            playAsUserButton = new MenuButton(playAsUserButtonClicked, LanguageTranslator.Translate("Play"), content, 0, 0, true, bigFont);
            playAsUserButton.X = windowWidth / 2 - (playAsUserButton.Width / 2);
            playAsUserButton.Y = (int)askTextPos.Y + BUTTON_Y_SPACING;

            resumeButton = new MenuButton(resumeButtonClicked, LanguageTranslator.Translate("Resume"), content, 0, 0, true, bigFont);
            resumeButton.X = windowWidth / 2 - (resumeButton.Width / 2);
            resumeButton.Y = playAsUserButton.Y;

            achievementButton = new MenuButton(achievementsClicked, LanguageTranslator.Translate("Achievements"), content, 0, 0, true, bigFont);
            achievementButton.X = windowWidth / 2 - (achievementButton.Width / 2);
            achievementButton.Y = playAsUserButton.Y + BUTTON_Y_SPACING + achievementButton.Height;

            mainMenuButton = new MenuButton(backToPostLoginClicked, LanguageTranslator.Translate("Back"), content, XSPACING, 0, true, bigFont);
            mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);
            mainMenuButton.Y = resumeButton.Y + resumeButton.Height + BUTTON_Y_SPACING;

            statsButton = new MenuButton(statsClicked, LanguageTranslator.Translate("Stats"), content, 0, 0, true, bigFont);
            statsButton.X = windowWidth / 2 - (statsButton.Width / 2);
            statsButton.Y = achievementButton.Y + achievementButton.Height + BUTTON_Y_SPACING;

            this.createUserButton = new MenuButton(createUserClicked, LanguageTranslator.Translate("Create New User"), content, 0, 0, true, bigFont);
            createUserButton.Y = windowHeight - YSPACING - createUserButton.Height;
            createUserButton.X = windowWidth / 2 - (createUserButton.Width / 2);

            preLoginButton = new MenuButton(mainMenuButtonClicked, LanguageTranslator.Translate("Main Menu"), content, 0, 0, true, bigFont);
            preLoginButton.X = XSPACING;
            preLoginButton.Y = createUserButton.Y;

            settingsButton = new MenuButton(gearAsset, settingsClicked, content, 0, 0, true);
            settingsButton.X = windowWidth - (BUTTON_Y_SPACING + settingsButton.Width);
            settingsButton.Y = windowHeight - (YSPACING + settingsButton.Height);

            helpButton = new MenuButton(helpClicked, " ? ", content, settingsButton.X, settingsButton.Y, true, bigFont);
            helpButton.X = windowWidth - (BUTTON_Y_SPACING + helpButton.Width);
            helpButton.Y = windowHeight - (YSPACING + helpButton.Height);
            tutorial = new Tutorial(content, tutorialAssets, whiteRectangleAsset, windowWidth, windowHeight, leftArrowAsset, rightArrowAsset);

            gift = new GiftGiver(skins, items, content, whiteRectangleAsset, giftAsset, achievementButton.X + YSPACING, playAsUserButton.Y, rand, null, tongueAsset,
                coinAsset, itemAssets, whiteCircleAsset, smallFont, bigFont, windowWidth, windowHeight, new SkinAction(AddSkin));

            languageButton = new MenuButton(globeAsset, languageClicked, content, helpButton.X - YSPACING - helpButton.Width, helpButton.Y, true);
            languageButton.Width = helpButton.Height;
            languageButton.Height = languageButton.Width;
            int nextY = drawSnake.Rectangle.Bottom + BUTTON_Y_SPACING;
            List<Language> allLangs = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
            for (int i = 0; i < allLangs.Count; i++)
            {
                languageButtons.Add(allLangs[i], new MenuButton(langsClickedList[allLangs[i]], LanguageTranslator.GetName(allLangs[i]), content, 0, nextY, true, bigFont));
                languageButtons[allLangs[i]].X = windowWidth / 2 - (languageButtons[allLangs[i]].Width / 2);
                nextY += languageButtons[allLangs[i]].Height + BUTTON_Y_SPACING;
            }

            achBar = new FillBar(content, whiteRectangleAsset, 0, 0, drawSnake.Rectangle.Bottom + BUTTON_Y_SPACING, 100, 20,
                Achievements.AchievementsList.Count, smallFont);
            achBar.MaxValue = Achievements.AchievementsList.Count;
            achBar.X = windowWidth / 2 - (achBar.Width / 2);
            achBarTxtPos = new Vector2((achBar.X + achBar.Width) - smallFont.MeasureString(achBarTxt).X, achBar.Y);

            grayImage = content.Load<Texture2D>(whiteRectangleAsset);
            grayRectangle = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = alphaValue;

            shopButton = new MenuButton(shopClicked, LanguageTranslator.Translate("Shop"), content, 0, 0, true, bigFont);
            shopButton.X = windowWidth / 2 - (shopButton.Width / 2);
            shopButton.Y = statsButton.Y + statsButton.Height + BUTTON_Y_SPACING;

            pickSkinButton = new MenuButton(pickSkinClicked, LanguageTranslator.Translate("Snakes"), content, 0, 0, true, bigFont);
            pickSkinButton.X = windowWidth / 2 - (pickSkinButton.Width / 2);
            pickSkinButton.Y = shopButton.Y + shopButton.Height + BUTTON_Y_SPACING;

            nextPageButton = new MenuButton(rightArrowAsset, NextPage, content, 0, 0, true);
            nextPageButton.X = windowWidth - XSPACING - nextPageButton.Width;
            nextPageButton.Y = preLoginButton.Y - nextPageButton.Height - BUTTON_Y_SPACING;

            prevPageButton = new MenuButton(leftArrowAsset, PrevPage, content, 0, 0, true);

            nextPageButton.ImgWidth = prevPageButton.ImgWidth;
            nextPageButton.ImgHeight = prevPageButton.ImgHeight;
            nextPageButton.Width = prevPageButton.Width;
            nextPageButton.Height = prevPageButton.Height;

            prevPageButton.X = preLoginButton.X;
            prevPageButton.Y = nextPageButton.Y;

            submitInfoButton = new MenuButton(new OnButtonClick(OnCreateUserClick), LanguageTranslator.Translate("Create User"), content, 0, 0, true, bigFont); // Flip
            submitInfoButton.X = windowWidth / 2 - (submitInfoButton.Width / 2);
            submitInfoButton.Y = createUserButton.Y;

            shop = new Shop(content, "speed", "teleport", "hammer", whiteRectangleAsset, smallFont, mazeAsset, itemActions, skins, 
                tongueAsset, new SkinAction(AddSkin), leftArrowAsset, rightArrowAsset, windowWidth, windowHeight, onItemPurchase, 
                bigFont, textboxAsset, mediumFont, "forcefieldpw", "frozenpw");
            if (discountPercent > 0.0f)
            {
                shop.AddDiscount(discountPercent);
            }

            // Create an interface for each user
            List<User> users = User.LoadUsers();
            if (users != null)
            {
                foreach (User u in users)
                {
                    UserInterface interfaceToAdd = new UserInterface(u, content, trashAsset, bigFont, whiteRectangleAsset, 0, 0,
                        new OnUserButtonClick(OnUserSignIn), whenTrashClicked, tongueAsset);
                    userInterfaces.Add(interfaceToAdd);
                    UpdateUserPagesToInterfaces();
                }
            }

            // Create an interface for each achievement
            if (Achievements.AchievementsList != null)
            {
                List<Achievement> achs = Achievements.AchievementsList.OrderBy(x => achIdOrder.IndexOf(x.Id)).ToList();
                foreach (Achievement a in achs)
                {
                    AchievementInterface interfaceToAdd;
                    if (a.MysteryAchievement)
                    {
                        interfaceToAdd = new AchievementInterface(content, whiteRectangleAsset, lockSprite, 0, 0, bigFont,
                            smallFont, a, whiteCircleAsset, checkAsset);
                    }
                    else
                    {
                        interfaceToAdd = new AchievementInterface(content, whiteRectangleAsset, a,
                            0, 0, bigFont, smallFont, whiteCircleAsset, checkSprite);
                    }
                    achInterfaces.Add(interfaceToAdd);
                }
                UpdateAchPagesToInterfaces();
            }

            usernameBox = new Textbox(0, 0, content, textboxAsset, bigFont, true, whiteRectangleAsset);
            usernameBox.X = windowWidth / 2 - (usernameBox.DrawRectangle.Width / 2);
            usernameBox.Y = (int)askTextPos.Y + BUTTON_Y_SPACING;

            needsRequiredFields = new Popup(content, smallFont, whiteRectangleAsset, windowWidth, windowHeight, false, bigFont);

            muteButton = new ToggleButton(soundAsset, crossedOutAsset, content, languageButton.X - YSPACING - languageButton.Width,
                languageButton.Y, languageButton.Width, languageButton.Height);
            muteButton.AddOnToggleHandler(new Action<bool>(x => Sound.PlaySounds = !x));

            classicButton = new MenuButton(new OnButtonClick(() => gameModeSelected(GameMode.Classic)),
                LanguageTranslator.Translate("Classic"), content, 0, 0, true, bigFont);
            classicButton.X = windowWidth / 2 - classicButton.Width / 2;
            classicButton.Y = titleRect.Bottom + BUTTON_Y_SPACING;

            freeplayButton = new MenuButton(new OnButtonClick(() => gameModeSelected(GameMode.Freeplay)),
                LanguageTranslator.Translate("Freeplay"), content, 0, 0, true, bigFont);
            freeplayButton.X = windowWidth / 2 - freeplayButton.Width / 2;
            freeplayButton.Y = classicButton.DrawRectangle.Bottom + BUTTON_Y_SPACING;

            starButton = new MenuButton(new OnButtonClick(() => gameModeSelected(GameMode.Star)),
                LanguageTranslator.Translate("Star Mode"), content, 0, 0, true, bigFont);
            starButton.X = windowWidth / 2 - starButton.Width / 2;
            starButton.Y = freeplayButton.DrawRectangle.Bottom + BUTTON_Y_SPACING;
        }

        #endregion

        #region Public Methods

        public void DrawMenu(SpriteBatch spriteBatch, GameState state, User user, bool shouldDrawCoinDisplay)
        {
            if (state == GameState.NewUser)
            {
                usernameBox.Drawn = true;
            }
            else
            {
                usernameBox.Drawn = false;
            }

            switch (state)
            {
                case GameState.Playing:

                    if (shouldDrawCoinDisplay)
                    {
                        DrawCoinDisplay(spriteBatch, user);
                    }

                    break;

                case GameState.Paused:

                    if (shouldDrawCoinDisplay)
                    {
                        DrawCoinDisplay(spriteBatch, user);
                    }
                    spriteBatch.Draw(grayImage, grayRectangle, transColor);
                    resumeButton.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);

                    break;

                case GameState.MainMenu:

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    string drawText = LanguageTranslator.Translate("Please sign in to play");
                    spriteBatch.DrawString(smallFont, drawText,
                        new Vector2(windowWidth / 2 - ((smallFont.MeasureString(drawText).X) / 2), askTextPos.Y), Color.Black);

                    drawSnake.Draw(spriteBatch);

                    playButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    creditsButton.Draw(spriteBatch);
                    helpButton.Draw(spriteBatch);
                    languageButton.Draw(spriteBatch);
                    muteButton.Draw(spriteBatch);

                    if (ShowingLogo)
                    {
                        // On drawing the color: Multiply by "logoAlpha" if we are fading the logo altogether
                        // If not, multiply by 1 (maintains color)
                        spriteBatch.Draw(logoBgImg, logoBgRect, logoBgColor * (fadingLogo ? logoAlpha : 1.0f));
                        if (duoplusLogo != null)
                        {
                            spriteBatch.Draw(duoplusLogo, logoRect, Color.White * logoAlpha);
                        }
                    }

                    break;

                case GameState.PickUser:

                    int lastYUser = titleRect.Y + titleRect.Height + XSPACING;
                    if (userPage <= userPages.Count - 1)
                    {
                        foreach (UserInterface ui in userPages[userPage])
                        {
                            ui.X = windowWidth / 2 - (ui.Width / 2);
                            ui.Y = lastYUser;
                            ui.Draw(spriteBatch);
                            lastYUser += BUTTON_Y_SPACING + ui.Height;
                        }
                    }

                    spriteBatch.DrawString(smallFont, askText, askTextPos, Color.Black);

                    createUserButton.Draw(spriteBatch);

                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch);

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    preLoginButton.Draw(spriteBatch);

                    drawSnake.Draw(spriteBatch);

                    break;

                case GameState.NewUser:

                    // Draw menu objects
                    usernameBox.Draw(spriteBatch, true);

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    drawText = LanguageTranslator.Translate("Create New User");
                    spriteBatch.DrawString(smallFont, drawText,
                        new Vector2(windowWidth / 2 - (smallFont.MeasureString(drawText).X / 2), askTextPos.Y),
                        Color.Black);
                    spriteBatch.DrawString(bigFont, usernamePrefix, usernamePrePos, Color.Black);

                    playButton.Draw(spriteBatch);
                    submitInfoButton.Draw(spriteBatch);

                    if (needsRequiredFields.Active)
                    {
                        needsRequiredFields.Draw(spriteBatch);
                    }

                    break;

                case GameState.ViewAchievements:

                    int lastYAchievements = titleRect.Height + titleRect.Y + XSPACING;
                    if (achPage <= achPages.Count - 1)
                    {
                        foreach (AchievementInterface ai in achPages[achPage])
                        {
                            ai.X = windowWidth / 2 - (ai.Width / 2);
                            ai.Y = lastYAchievements;
                            ai.Draw(spriteBatch, user);
                            lastYAchievements += BUTTON_Y_SPACING + ai.Height;
                        }
                    }

                    achBar.Draw(spriteBatch, Color.RoyalBlue);
                    spriteBatch.DrawString(smallFont, achBarTxt, achBarTxtPos, Color.White);

                    mainMenuButton.Draw(spriteBatch);

                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch);

                    drawSnake.Draw(spriteBatch);
                    spriteBatch.Draw(titleImg, titleRect, Color.White);

                    if (shouldDrawCoinDisplay)
                    {
                        DrawCoinDisplay(spriteBatch, user);
                    }

                    break;

                case GameState.PostUserLoginMenu:

                    drawText = LanguageTranslator.Translate("Playing as") + " " + user.Username;
                    spriteBatch.DrawString(smallFont, drawText,
                        new Vector2(windowWidth / 2 - ((int)(smallFont.MeasureString(drawText).X / 2)), askTextPos.Y), Color.Black);

                    achievementButton.Draw(spriteBatch);
                    playAsUserButton.Draw(spriteBatch);
                    preLoginButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    statsButton.Draw(spriteBatch);
                    shopButton.Draw(spriteBatch);
                    pickSkinButton.Draw(spriteBatch);
                    settingsButton.Draw(spriteBatch);
                    gift.Draw(spriteBatch);

                    drawSnake.Draw(spriteBatch);
                    spriteBatch.Draw(titleImg, titleRect, Color.White);

                    if (shouldDrawCoinDisplay)
                    {
                        DrawCoinDisplay(spriteBatch, user);
                    }

                    break;

                case GameState.ViewStatistics:

                    mainMenuButton.Draw(spriteBatch);

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    drawSnake.Draw(spriteBatch);

                    // Draw stats
                    int nextY = titleRect.Y + YSPACING + titleRect.Height;

                    drawText = LanguageTranslator.Translate("Showing statistics for") + " " + user.Username;
                    spriteBatch.DrawString(smallFont, drawText,
                        new Vector2(windowWidth / 2 - ((int)smallFont.MeasureString(drawText).X / 2), nextY), Color.Black);
                    nextY += BUTTON_Y_SPACING * 2;

                    spriteBatch.DrawString(mediumFont, MAZES_COMPLETED_PREFIX + user.MazesCompleted, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(MAZES_COMPLETED_PREFIX + user.MazesCompleted).X) / 2), nextY),
                        Color.DarkRed);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, POWERUPS_PREFIX + user.PowerupsCollected, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(POWERUPS_PREFIX + user.PowerupsCollected).X) / 2), nextY), Color.Orange);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, TELEPORT_PREFIX + user.TimesTeleported, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(TELEPORT_PREFIX + user.TimesTeleported).X) / 2), nextY), Color.Yellow);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, SPEED_PREFIX + user.SpeedSeconds, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(SPEED_PREFIX + user.SpeedSeconds).X) / 2), nextY), Color.LightGreen);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, COINS_PREFIX + user.CoinsCollected, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(COINS_PREFIX + user.CoinsCollected).X) / 2), nextY), Color.LightBlue);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, ACHIEVEMENT_STAT_PREFIX + user.AchievementsCompleted.Count,
                        new Vector2(windowWidth / 2 - ((int)(mediumFont.MeasureString(ACHIEVEMENT_STAT_PREFIX +
                        user.AchievementsCompleted.Count).X) / 2), nextY), Color.Indigo);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, WALL_BREAK_PREFIX + user.WallsBroken, new Vector2(windowWidth / 2 -
                        ((int)(mediumFont.MeasureString(WALL_BREAK_PREFIX + user.WallsBroken).X) / 2), nextY), Color.Purple);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, ENEMIES_AVOIDED_PREFIX + user.EnemiesAvoided, new Vector2(windowWidth / 2 -
                        (int)(mediumFont.MeasureString(ENEMIES_AVOIDED_PREFIX + user.EnemiesAvoided).X / 2), nextY), Color.White);
                    nextY += BUTTON_Y_SPACING;

                    spriteBatch.DrawString(mediumFont, TIME_FROZEN_PREFIX + user.TimeFrozen, new Vector2(windowWidth / 2 -
                        (int)(mediumFont.MeasureString(TIME_FROZEN_PREFIX + user.TimeFrozen).X / 2), nextY), Color.Cyan);
                    nextY += BUTTON_Y_SPACING;

                    break;

                case GameState.Shop:

                    mainMenuButton.Draw(spriteBatch);
                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    drawSnake.Draw(spriteBatch);

                    DrawCoinDisplay(spriteBatch, user);

                    shop.Draw(spriteBatch);

                    break;

                case GameState.PickSkin:

                    mainMenuButton.Draw(spriteBatch);

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    drawSnake.Draw(spriteBatch);
                    DrawCoinDisplay(spriteBatch, user);

                    int row = 0;
                    int lastXSkin = XSPACING;
                    int lastY = titleRect.Height + BUTTON_Y_SPACING;
                    int interfacesInCurrentRow = 0;
                    if (skinPage <= skinPages.Count - 1)
                    {
                        foreach (SkinInterface si in skinPages[skinPage])
                        {
                            si.X = lastXSkin;
                            lastXSkin += si.Width + XSPACING;
                            if (interfacesInCurrentRow >= SKINS_PER_ROW)
                            {
                                row++;
                                si.X = lastXSkin = XSPACING;
                                lastXSkin += si.Width + XSPACING;
                                interfacesInCurrentRow = 0;
                            }
                            if (row == 0)
                            {
                                si.Y = lastY;
                            }
                            else if (row == 1)
                            {
                                si.Y = lastY + (BUTTON_Y_SPACING * 2) + si.Height;
                            }
                            si.Draw(spriteBatch);
                            interfacesInCurrentRow++;
                        }
                    }

                    CheckSkinInterfaceActive(user);

                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch);

                    break;

                case GameState.Settings:

                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    drawSnake.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);
                    if (volumeSlider != null)
                    {
                        volumeSlider.Draw(spriteBatch);
                    }
                    if (changeUsernameBox != null)
                    {
                        spriteBatch.DrawString(bigFont, usernamePrefix, usernamePrePos, Color.Black);
                        changeUsernameBox.Drawn = true;
                        changeUsernameBox.Draw(spriteBatch);
                    }
                    if (showMouseWhilePlayingBox != null)
                    {
                        showMouseWhilePlayingBox.Draw(spriteBatch, Color.Black);
                    }
                    if (saveSettingsButton != null)
                    {
                        saveSettingsButton.Draw(spriteBatch);
                    }
                    needsRequiredFields.Draw(spriteBatch);

                    break;

                case GameState.Credits:

                    int y = (int)creditsScrollValue;

                    y += drawSnake.Height + BUTTON_Y_SPACING;

                    foreach (string line in GameInfo.Credits)
                    {
                        spriteBatch.DrawString(smallFont, line, new Vector2(windowWidth / 2 - (smallFont.MeasureString(line).X / 2), y), Color.Black);
                        y += (int)smallFont.MeasureString(line).Y;
                    }

                    spriteBatch.Draw(titleImg, new Rectangle(titleRect.X, y, titleRect.Width, titleRect.Height), Color.White);
                    drawSnake.Y = y;
                    drawSnake.Draw(spriteBatch);

                    y += BUTTON_Y_SPACING + titleImg.Height;
                    preLoginButton.Y = y;
                    preLoginButton.Draw(spriteBatch);

                    break;

                case GameState.Help:
                    preLoginButton.Draw(spriteBatch);
                    tutorial.Draw(spriteBatch);
                    break;

                case GameState.SelectLanguage:

                    drawSnake.Draw(spriteBatch);
                    spriteBatch.Draw(titleImg, titleRect, Color.White);
                    preLoginButton.Draw(spriteBatch);
                    foreach (KeyValuePair<Language, MenuButton> kv in languageButtons)
                    {
                        kv.Value.Draw(spriteBatch);
                    }

                    break;

                case GameState.SelectGameMode:

                    drawSnake.Draw(spriteBatch);
                    spriteBatch.Draw(titleImg, titleRect, Color.White);

                    drawText = LanguageTranslator.Translate("Please select a gamemode.");
                    spriteBatch.DrawString(smallFont, drawText,
                        new Vector2(windowWidth / 2 - ((smallFont.MeasureString(drawText).X) / 2), askTextPos.Y), Color.Black);

                    classicButton.Draw(spriteBatch);
                    freeplayButton.Draw(spriteBatch);
                    starButton.Draw(spriteBatch);

                    mainMenuButton.Draw(spriteBatch);

                    break;
            }
        }

        public void Update(GameState state, GameTime gameTime, int windowWidth, int windowHeight, ref User user, GameState prevState)
        {
            #region Class-Wide Updating

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.state = state;

            usernamePrefix = LanguageTranslator.Translate("Username") + ": ";

            drawSnake.ShadeColor = snakeSkin.Color.GetColor();
            drawSnake.X = titleRect.X - drawSnake.Width;
            if (state != GameState.Credits)
            {
                drawSnake.Y = YSPACING;
                creditsScrollValue = windowHeight / 2;
            }

            this.user = user;

            // Page can't be negative, but the user may choose to make it be
            if (userPage < 0)
            {
                userPage = 0;
            }
            if (achPage < 0)
            {
                achPage = 0;
            }

            if (state != GameState.NewUser && usernameBox.Content != "")
            {
                usernameBox.Content = "";
            }

            if (state == GameState.Paused)
            {
                mainMenuButton.Text = LanguageTranslator.Translate("Main Menu");
            }
            else
            {
                mainMenuButton.Text = LanguageTranslator.Translate("Back");
            }

            if (state != GameState.PostUserLoginMenu && state != GameState.PickUser)
            {
                preLoginButton.Text = LanguageTranslator.Translate("Main Menu");
            }
            else if (state == GameState.PickUser)
            {
                preLoginButton.Text = LanguageTranslator.Translate("Back");
            }

            if (state != GameState.ViewAchievements)
            {
                achPage = 0;
            }

            // The main menu text is used on nearly all menus, so we'll always update the values of it
            titleRect.X = windowWidth / 2 - (titleRect.Width / 2);
            if (!ShowingLogo)
            {
                titleRect.Y = YSPACING;
            }

            #endregion

            #region Menu-Specific Updating

            switch (state)
            {
                case GameState.Playing:

                    break;
                case GameState.MainMenu:

                    // Update values
                    playButton.Text = LanguageTranslator.Translate("Start");
                    playButton.X = windowWidth / 2 - (playButton.Width / 2);
                    playButton.Y = (int)askTextPos.Y + BUTTON_Y_SPACING;

                    creditsButton.Text = LanguageTranslator.Translate("Credits");
                    creditsButton.X = windowWidth / 2 - (creditsButton.Width / 2);
                    creditsButton.Y = windowHeight - creditsButton.Height - YSPACING;

                    quitButton.Text = LanguageTranslator.Translate("Quit");
                    quitButton.X = windowWidth / 2 - (quitButton.Width / 2);
                    quitButton.Y = playButton.Y + BUTTON_Y_SPACING + quitButton.Height;

                    helpButton.X = windowWidth - (BUTTON_Y_SPACING + helpButton.Width);
                    helpButton.Y = windowHeight - (YSPACING + helpButton.Height);

                    // Deal with the pre-menu logo showing
                    if (!ShowingLogo)
                    {
                        // Update objects
                        playButton.Update();
                        quitButton.Update();
                        creditsButton.Update();
                        helpButton.Update();
                        languageButton.Update();
                        muteButton.Update();
                    }
                    else
                    {
                        if (waitingToFade)
                        {
                            logoTimer.Update(gameTime);
                            if (logoTimer.QueryWaitTime(gameTime))
                            {
                                waitingToFade = false;
                                fadingLogo = true;
                            }
                        }

                        if (fadingLogo)
                        {
                            logoAlpha -= LOGO_ALPHA_CHANGE;
                            if (logoAlpha <= 0)
                            {
                                // We're done with the logos
                                ShowingLogo = false;
                                undarkeningLogo = false;
                                waitingToFade = false;
                                fadingLogo = false;
                            }
                        }
                        
                        if (undarkeningLogo)
                        {
                            logoAlpha += LOGO_ALPHA_CHANGE;
                            if (logoAlpha >= 1.0f)
                            {
                                waitingToFade = true;
                                undarkeningLogo = false;
                            }
                        }
                    }

                    break;

                case GameState.Paused:

                    // Update values
                    resumeButton.Text = LanguageTranslator.Translate("Resume");
                    resumeButton.X = windowWidth / 2 - (resumeButton.Width / 2);
                    resumeButton.Y = playAsUserButton.Y;

                    mainMenuButton.Text = LanguageTranslator.Translate("Main Menu");
                    mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);
                    mainMenuButton.Y = resumeButton.Y + resumeButton.Height + BUTTON_Y_SPACING;

                    quitButton.Text = LanguageTranslator.Translate("Save & Quit Game");
                    quitButton.X = windowWidth / 2 - (quitButton.Width / 2);
                    quitButton.Y = mainMenuButton.Y + mainMenuButton.Height + BUTTON_Y_SPACING;

                    // Update objects
                    resumeButton.Update();
                    mainMenuButton.Update();
                    quitButton.Update();

                    break;

                case GameState.PickUser:

                    // Clamping, Efficiency Fixes, and Bug Fixes
                    for (int i = 0; i < userPages.Count; i++)
                    {
                        if (userPages[i].Count == 0)
                        {
                            userPages.RemoveAt(i);
                        }
                    }

                    while (userPage > userPages.Count - 1)
                    {
                        userPage--;
                    }

                    // Update values
                    preLoginButton.X = XSPACING;
                    preLoginButton.Y = createUserButton.Y;

                    nextPageButton.X = windowWidth - XSPACING - nextPageButton.Width;
                    nextPageButton.Y = preLoginButton.Y - nextPageButton.Height - BUTTON_Y_SPACING;

                    prevPageButton.X = preLoginButton.X;
                    prevPageButton.Y = nextPageButton.Y;

                    createUserButton.Text = LanguageTranslator.Translate("Create New User");
                    createUserButton.X = windowWidth / 2 - (createUserButton.Width / 2);

                    askText = LanguageTranslator.Translate("Who are you?");
                    askTextPos.X = windowWidth / 2 - ((int)(smallFont.MeasureString(askText).X) / 2);

                    // Update objects
                    if (userPages.Count > 0)
                    {
                        for (int i = 0; i < userPages[userPage].Count; i++)
                        {
                            userPages[userPage][i].Update();
                        }
                    }

                    if (userPages.Count <= 1)
                    {
                        // Both buttons should be disabled, as there is only 1 page
                        // This must be tested for first, otherwise, the since the page always starts equal to 0, the below criteria will be met
                        // and the game will act like there are 2 pages when there is only 1
                        nextPageButton.Active = false;
                        prevPageButton.Active = false;
                        // Since there is only one page, we have to set the page integer to 0
                        userPage = 0;
                    }
                    else if (userPage == 0)
                    {
                        // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                        nextPageButton.Active = true;
                        prevPageButton.Active = false;
                    }
                    else if (userPage + 1 == userPages.Count)
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

                    nextPageButton.Update();
                    prevPageButton.Update();

                    createUserButton.Update();

                    preLoginButton.Update();

                    break;

                case GameState.NewUser:

                    // Update values
                    usernameBox.X = windowWidth / 2 - (usernameBox.DrawRectangle.Width / 2);
                    usernameBox.Y = (int)askTextPos.Y + BUTTON_Y_SPACING;

                    usernamePrePos.X = usernameBox.X - (int)bigFont.MeasureString(usernamePrefix).X;
                    usernamePrePos.Y = usernameBox.Y;

                    playButton.Text = LanguageTranslator.Translate("Cancel");
                    playButton.X = XSPACING;
                    playButton.Y = createUserButton.Y;

                    submitInfoButton.Text = LanguageTranslator.Translate("Create User");
                    submitInfoButton.X = windowWidth / 2 - (submitInfoButton.Width / 2);

                    // Update objects
                    usernameBox.Update(gameTime, new Action(HandleEnterPress));

                    if (needsRequiredFields.Active)
                    {
                        needsRequiredFields.Update();
                    }

                    submitInfoButton.Update();
                    playButton.Update();

                    break;

                case GameState.ViewAchievements:

                    // Clamping/Efficiency
                    for (int i = 0; i < achPages.Count; i++)
                    {
                        if (achPages[i].Count == 0)
                        {
                            achPages.RemoveAt(i);
                        }
                    }

                    while (achPage > achPages.Count - 1)
                    {
                        achPage--;
                    }

                    // Update Values
                    mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);
                    mainMenuButton.Y = windowHeight - YSPACING - mainMenuButton.Height;

                    nextPageButton.Y = windowHeight - YSPACING - nextPageButton.Height;
                    prevPageButton.Y = nextPageButton.Y;

                    achBar.Value = user.AchievementsCompleted.Count;
                    achBarTxt = Math.Floor((achBar.Value / achBar.MaxValue) * 100).ToString() + "%";
                    achBarTxtPos.X = (achBar.X + achBar.Width) - smallFont.MeasureString(achBarTxt).X;

                    // Update Objects

                    if (achPages.Count <= 1)
                    {
                        // Both buttons should be disabled, as there is only 1 page
                        // This must be tested for first, otherwise, the since the page always starts equal to 0, the below criteria will be met
                        // and the game will act like there are 2 pages when there is only 1
                        nextPageButton.Active = false;
                        prevPageButton.Active = false;
                        // Since there is only one page, we have to set the page integer to 0
                        userPage = 0;
                    }
                    else if (achPage == 0)
                    {
                        // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                        nextPageButton.Active = true;
                        prevPageButton.Active = false;
                    }
                    else if (achPage + 1 == achPages.Count)
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

                    mainMenuButton.Update();
                    prevPageButton.Update();
                    nextPageButton.Update();

                    break;

                case GameState.PostUserLoginMenu:

                    // Update values
                    preLoginButton.Text = LanguageTranslator.Translate("Log Out");
                    preLoginButton.X = XSPACING;
                    preLoginButton.Y = windowHeight - YSPACING - preLoginButton.Height;

                    quitButton.Text = LanguageTranslator.Translate("Quit");
                    quitButton.X = windowWidth / 2 - (quitButton.Width / 2);
                    quitButton.Y = pickSkinButton.Y + pickSkinButton.Height + BUTTON_Y_SPACING;

                    playAsUserButton.Text = LanguageTranslator.Translate("Play");
                    playAsUserButton.X = windowWidth / 2 - (playAsUserButton.Width / 2);

                    achievementButton.Text = LanguageTranslator.Translate("Achievements");
                    achievementButton.X = windowWidth / 2 - (achievementButton.Width / 2);

                    statsButton.Text = LanguageTranslator.Translate("Stats");
                    statsButton.X = windowWidth / 2 - (statsButton.Width / 2);

                    shopButton.Text = LanguageTranslator.Translate("Shop");
                    shopButton.X = windowWidth / 2 - (shopButton.Width / 2);

                    pickSkinButton.Text = LanguageTranslator.Translate("Snakes");
                    pickSkinButton.X = windowWidth / 2 - (pickSkinButton.Width / 2);

                    // Update objects
                    if (!gift.ShowingPopup)
                    {
                        playAsUserButton.Update();
                        achievementButton.Update();
                        preLoginButton.Update();
                        quitButton.Update();
                        statsButton.Update();
                        shopButton.Update();
                        pickSkinButton.Update();
                        settingsButton.Update();
                    }
                    gift.Update(user);

                    break;

                case GameState.ViewStatistics:

                    // Update Values
                    mainMenuButton.X = XSPACING;
                    mainMenuButton.Y = windowHeight - YSPACING - preLoginButton.Height;

                    ACHIEVEMENT_STAT_PREFIX = LanguageTranslator.Translate(ACHIEVEMENT_STAT_PREFIX);
                    MAZES_COMPLETED_PREFIX = LanguageTranslator.Translate(MAZES_COMPLETED_PREFIX);
                    POWERUPS_PREFIX = LanguageTranslator.Translate(POWERUPS_PREFIX);
                    COINS_PREFIX = LanguageTranslator.Translate(COINS_PREFIX);
                    WALL_BREAK_PREFIX = LanguageTranslator.Translate(WALL_BREAK_PREFIX);
                    SPEED_PREFIX = LanguageTranslator.Translate(SPEED_PREFIX);
                    TELEPORT_PREFIX = LanguageTranslator.Translate(TELEPORT_PREFIX);
                    TIME_FROZEN_PREFIX = LanguageTranslator.Translate(TIME_FROZEN_PREFIX);
                    ENEMIES_AVOIDED_PREFIX = LanguageTranslator.Translate(ENEMIES_AVOIDED_PREFIX);

                    // Update Objects
                    mainMenuButton.Update();

                    break;

                case GameState.Shop:

                    // Update Values
                    mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);
                    mainMenuButton.Y = windowHeight - YSPACING - mainMenuButton.Height;

                    // Update Objects
                    mainMenuButton.Update();
                    shop.Update(ref user, gameTime);

                    break;

                case GameState.PickSkin:

                    // Update Values
                    mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);
                    mainMenuButton.Y = windowHeight - YSPACING - preLoginButton.Height;

                    nextPageButton.Y = windowHeight - YSPACING - nextPageButton.Height;
                    prevPageButton.Y = nextPageButton.Y;

                    if (skinPages.Count > 0)
                    {
                        for (int i = 0; i < skinPages[skinPage].Count; i++)
                        {
                            skinPages[skinPage][i].Update();
                        }
                    }

                    // Make sure the skin interfaces match the skins
                    List<Skin> currentSkins = new List<Skin>();
                    foreach (SkinInterface si in skinInterfaces)
                    {
                        currentSkins.Add(si.Skin);
                    }
                    foreach (Skin s in user.Skins)
                    {
                        if (!currentSkins.Contains(s))
                        {
                            skinInterfaces.Add(new SkinInterface(s, skinSelected, content, whiteRectangleAsset, smallFont, 
                                tongueAsset, 0, 0));
                            AddSkin(s);
                        }
                    }

                    foreach (List<SkinInterface> l in skinPages)
                    {
                        List<SkinInterface> empty = new List<SkinInterface>();
                        if (l == empty)
                        {
                            skinPages.Remove(l);
                        }
                    }

                    if (skinPages.Count <= 1)
                    {
                        // Both buttons should be disabled, as there is only 1 page
                        // This must be tested for first, otherwise, the since the page always starts equal to 0, the below criteria will be met
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

                    // Update Objects
                    nextPageButton.Update();
                    prevPageButton.Update();
                    mainMenuButton.Update();

                    break;

                case GameState.Settings:

                    // Update Values
                    mainMenuButton.X = XSPACING;
                    mainMenuButton.Y = windowHeight - YSPACING - preLoginButton.Height;

                    usernamePrePos.X = changeUsernameBox.X - (int)(bigFont.MeasureString(usernamePrefix).X);
                    usernamePrePos.Y = changeUsernameBox.Y;

                    // Update Objects
                    mainMenuButton.Update();
                    needsRequiredFields.Update();
                    if (volumeSlider != null)
                    {
                        volumeSlider.Update();
                        // Change the volume instantly, as it always affects the game and we want changes to volume to be rendered immediately
                        if (user.Settings.Volume != (int)volumeSlider.Value)
                        {
                            user.ChangeVolume((int)volumeSlider.Value);
                        }
                    }
                    if (changeUsernameBox != null)
                    {
                        if (prevState != GameState.Settings)
                        {
                            changeUsernameBox.Content = user.Username;
                        }
                        changeUsernameBox.Update(gameTime, new Action(HandleEnterPress));
                    }
                    if (showMouseWhilePlayingBox != null)
                    {
                        showMouseWhilePlayingBox.Update();
                    }
                    if (saveSettingsButton != null)
                    {
                        saveSettingsButton.Update();
                    }

                    break;

                case GameState.Credits:

                    preLoginButton.X = windowWidth / 2 - (preLoginButton.Width / 2);
                    preLoginButton.Update();

                    if (Mouse.GetState().ScrollWheelValue != prevScrollVal)
                    {
                        // If the value is too high, then we can't allow the user to scroll higher
                        if (creditsScrollValue < MAX_SCROLL_VAL)
                        {
                            // We are scrolling
                            creditsScrollValue += (Mouse.GetState().ScrollWheelValue - prevScrollVal) / SCROLL_DIVISION;
                        }
                    }
                    else
                    {
                        creditsScrollValue -= 0.4f;
                    }
                    prevScrollVal = Mouse.GetState().ScrollWheelValue;

                    if (creditsScrollValue < MIN_SCROLL_VAL || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        preLoginButton.Click();
                    }

                    break;

                case GameState.Help:

                    // Update Values
                    preLoginButton.X = windowWidth / 2 - (preLoginButton.Width / 2);
                    preLoginButton.Y = windowHeight - YSPACING - preLoginButton.Height;

                    // Update Objects
                    preLoginButton.Update();
                    tutorial.Update();

                    break;

                case GameState.SelectLanguage:

                    // Update objects
                    preLoginButton.Update();
                    foreach (KeyValuePair<Language, MenuButton> kv in languageButtons)
                    {
                        if (kv.Key == GameInfo.Language)
                        {
                            kv.Value.Active = false;
                        }
                        else
                        {
                            kv.Value.Active = true;
                        }
                        kv.Value.Update();
                    }

                    break;

                case GameState.SelectGameMode:

                    mainMenuButton.X = XSPACING;
                    mainMenuButton.Y = windowHeight - YSPACING - preLoginButton.Height;
                    mainMenuButton.Update();

                    classicButton.Text = LanguageTranslator.Translate("Classic");
                    freeplayButton.Text = LanguageTranslator.Translate("Freeplay");
                    starButton.Text = LanguageTranslator.Translate("Star Mode");

                    classicButton.X = windowWidth / 2 - classicButton.Width / 2;
                    classicButton.Y = titleRect.Bottom + BUTTON_Y_SPACING;
                    freeplayButton.X = windowWidth / 2 - freeplayButton.Width / 2;
                    freeplayButton.Y = classicButton.DrawRectangle.Bottom + BUTTON_Y_SPACING;
                    starButton.X = windowWidth / 2 - starButton.Width / 2;
                    starButton.Y = freeplayButton.DrawRectangle.Bottom + BUTTON_Y_SPACING;

                    classicButton.Update();
                    freeplayButton.Update();
                    starButton.Update();

                    break;
            }

            #endregion
        }

        public List<Textbox> GetTextboxes()
        {
            List<Textbox> textboxes = new List<Textbox>();

            if (state == GameState.NewUser)
            {
                textboxes.Add(usernameBox);
            }
            if (state == GameState.Settings && changeUsernameBox != null)
            {
                textboxes.Add(changeUsernameBox);
            }

            return textboxes;
        }

        public void DeleteUser(User user)
        {
            bool keepGoing = true;
            for (int i = 0; i < userPages.Count && keepGoing; i++)
            {
                for (int j = 0; j < userPages[i].Count && keepGoing; j++)
                {
                    if (userPages[i][j].User.Id == user.Id)
                    {
                        userPages[i].RemoveAt(j);
                        keepGoing = false;
                    }
                }
            }

            for (int i = 0; i < userInterfaces.Count; i++)
            {
                if (userInterfaces[i].User.Id == user.Id)
                {
                    userInterfaces.RemoveAt(i);
                    break;
                }
            }
        }

        public void EndDiscount()
        {
            shop.RemoveDiscount();
        }

        #endregion

        #region Private Methods

        private void InitializeLoginVars(User user)
        {
            skinInterfaces.Clear();
            foreach (Skin skin in user.Skins)
            {
                skinInterfaces.Add(new SkinInterface(skin, skinSelected, content, whiteRectangleAsset, smallFont, tongueAsset, 0, 0));
            }

            saveSettingsButton = new MenuButton(new OnButtonClick(SaveSettings), LanguageTranslator.Translate("Save"), content, 0, createUserButton.Y, true, bigFont);
            saveSettingsButton.X = windowWidth / 2 - (saveSettingsButton.Width / 2);

            volumeSlider = new ValueSlider(user.Settings.Volume, whiteRectangleAsset, windowWidth / 2 - (VOLUME_SLIDER_WIDTH / 2),
                titleRect.Bottom + BUTTON_Y_SPACING, content, VOLUME_SLIDER_WIDTH, VOLUME_SLIDER_HEIGHT, smallFont, LanguageTranslator.Translate("Volume"));

            changeUsernameBox = new Textbox(0, volumeSlider.Y + volumeSlider.Height + BUTTON_Y_SPACING, content, textboxAsset,
                bigFont, false, whiteRectangleAsset);
            changeUsernameBox.X = windowWidth / 2 - (changeUsernameBox.Width / 2);
            changeUsernameBox.Content = user.Username;

            showMouseWhilePlayingBox = new Checkbox(LanguageTranslator.Translate("Show mouse cursor while playing"), content, smallFont, checkboxSprite, checkSprite);
            showMouseWhilePlayingBox.X = windowWidth / 2 - (showMouseWhilePlayingBox.Width / 2);
            showMouseWhilePlayingBox.Y = changeUsernameBox.Y + changeUsernameBox.Height + BUTTON_Y_SPACING;
            showMouseWhilePlayingBox.IsChecked = user.Settings.ShowMouseWhilePlaying;
        }

        private void DrawCoinDisplay(SpriteBatch spriteBatch, User user)
        {
            if (user != null)
            {
                spriteBatch.Draw(coinDisplayBgImg, coinDisplayBgRect, Color.Gray);
                displayCoin.Draw(spriteBatch);
                spriteBatch.DrawString(mediumFont, user.Coins.ToString(),
                    new Vector2(coinDisplayBgRect.Right - (int)(mediumFont.MeasureString(user.Coins.ToString()).X + YSPACING), displayCoin.DrawRectangle.Y),
                    Color.Goldenrod);
            }
        }

        private void OnCreateUserClick()
        {
            bool shouldCreateUser = true;
            List<User> users = User.LoadUsers();
            if (usernameBox.Content == "")
            {
                shouldCreateUser = false;
                needsRequiredFields.ShowPopup(LanguageTranslator.Translate("Please enter a username."), false, windowWidth / 2 - (needsRequiredFields.Width / 2),
                    windowHeight / 2 - (needsRequiredFields.Height / 2));
            }
            else if (usernameBox.Content.Trim() == "")
            {
                shouldCreateUser = false;
                needsRequiredFields.ShowPopup(LanguageTranslator.Translate("Your username must include at least one non-whitespace character."), false,
                    windowWidth / 2 - (needsRequiredFields.Width / 2), windowHeight / 2 - (needsRequiredFields.Height / 2));
            }
            else
            {
                if (users != null && users.Count > 0)
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (!(users[i].Username.ToLower() != usernameBox.Content.ToLower()))
                        {
                            shouldCreateUser = false;
                            needsRequiredFields.ShowPopup("\"" + users[i].Username + "\" " +
                                LanguageTranslator.Translate("already exists. Please choose a different\nusername."),
                                false, windowWidth / 2 - (needsRequiredFields.Width / 2), windowHeight / 2 - (needsRequiredFields.Height / 2));
                        }
                    }
                }
            }
            if (shouldCreateUser)
            {
                CreateUser();
            }
        }

        private void OnUserSignIn(User user)
        {
            whenUsernameClicked(user);
            InitializeLoginVars(user);
            UpdateSkinPagesToInterfaces();
            foreach (AchievementInterface a in achInterfaces)
            {
                a.Completed = false;
                a.Achievement.Completed = false;
                // The AchievementInterface class will check on it's own to see if it is completed
            }
        }

        private void CreateUser()
        {
            User user = new User(usernameBox.Content.Trim(), 0, snakeSkin.SkinAsset);

            UserInterface interfaceToAdd = new UserInterface(user, content, trashAsset, bigFont, whiteRectangleAsset, 0, 0, new OnUserButtonClick(OnUserSignIn),
                new OnUserButtonClick(onTrashClick), tongueAsset);

            userInterfaces.Add(interfaceToAdd);

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (userPages.Count > 0)
            {
                for (int i = 0; i < userPages.Count; i++)
                {
                    if (userPages[i].Count < USER_INTERFACES_PER_PAGE)
                    {
                        userPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        userPages.Add(new List<UserInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                userPages.Add(new List<UserInterface>());
                userPages[0].Add(interfaceToAdd);
            }

            user.Id = User.GetNextId();

            whenUserCreated(user);
        }

        private void NextPage()
        {
            if (state == GameState.PickUser)
            {
                userPage++;
            }
            else if (state == GameState.ViewAchievements)
            {
                achPage++;
            }
            else if (state == GameState.PickSkin)
            {
                skinPage++;
            }
        }
        private void PrevPage()
        {
            if (state == GameState.PickUser)
            {
                userPage--;
            }
            else if (state == GameState.ViewAchievements)
            {
                achPage--;
            }
            else if (state == GameState.PickSkin)
            {
                skinPage--;
            }
        }

        #endregion

        #region Page Handling

        private void AddUser(User user)
        {
            UserInterface interfaceToAdd = new UserInterface(user, content, trashAsset, bigFont, whiteRectangleAsset, 0, 0, new OnUserButtonClick(OnUserSignIn),
                new OnUserButtonClick(onTrashClick), tongueAsset);

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (userPages.Count > 0)
            {
                for (int i = 0; i < userPages.Count; i++)
                {
                    if (userPages[i].Count < USER_INTERFACES_PER_PAGE)
                    {
                        userPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        userPages.Add(new List<UserInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                userPages.Add(new List<UserInterface>());
                userPages[0].Add(interfaceToAdd);
            }

        }
        private void AddAch(Achievement ach)
        {
            AchievementInterface interfaceToAdd;
            if (!ach.MysteryAchievement)
            {
                interfaceToAdd = new AchievementInterface(content, whiteRectangleAsset, ach, 0, 0, bigFont, smallFont, whiteCircleAsset, checkSprite);
            }
            else
            {
                interfaceToAdd = new AchievementInterface(content, whiteRectangleAsset, lockSprite, 0, 0, bigFont, smallFont, ach, whiteCircleAsset,
                    checkSprite);
            }

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (achPages.Count > 0)
            {
                for (int i = 0; i < achPages.Count; i++)
                {
                    if (achPages[i].Count < ACHIEVEMENTS_PER_PAGE)
                    {
                        achPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        achPages.Add(new List<AchievementInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                achPages.Add(new List<AchievementInterface>());
                achPages[0].Add(interfaceToAdd);
            }
        }
        public void AddSkin(Skin skin)
        {
            SkinInterface interfaceToAdd = new SkinInterface(skin, skinSelected, content, whiteRectangleAsset, smallFont, 
                tongueAsset, 0, 0);

            if (skinInterfaces.Where(x => x.Skin.Type == skin.Type).Count() <= 0)
            {
                // We need to make sure that skinInterfaces stores the skin, too (to prevent bugs
                // where user is given duplicate skins)
                skinInterfaces.Add(interfaceToAdd);
            }

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (skinPages.Count > 0)
            {
                for (int i = 0; i < skinPages.Count; i++)
                {
                    if (skinPages[i].Count < SKINS_PER_ROW * ROWS_PER_PAGE)
                    {
                        skinPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        skinPages.Add(new List<SkinInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                skinPages.Add(new List<SkinInterface>());
                skinPages[0].Add(interfaceToAdd);
            }

            if (skinPages.Count > 0)
            {
                for (int i = 0; i < skinPages.Count; i++)
                {
                    if (skinPages[i].Count == 0)
                    {
                        // Page is empty, but value should have been put in it by now
                        skinPages.RemoveAt(i);
                    }
                }
            }
        }

        private void UpdateUserPagesToInterfaces()
        {
            userPages.Clear();

            for (int i = 0; i < userInterfaces.Count; i++)
            {
                AddUser(userInterfaces[i].User);
            }
        }
        private void UpdateAchPagesToInterfaces()
        {
            achPages.Clear();

            for (int i = 0; i < achInterfaces.Count; i++)
            {
                AddAch(achInterfaces[i].Achievement);
            }
        }
        private void UpdateSkinPagesToInterfaces()
        {
            skinPages.Clear();

            for (int i = 0; i < skinInterfaces.Count; i++)
            {
                AddSkin(skinInterfaces[i].Skin);
            }
        }

        private void CheckSkinInterfaceActive(User user)
        {
            for (int i = 0; i < skinPages.Count; i++)
            {
                foreach (SkinInterface si in skinPages[i])
                {
                    if (user.CurrentSkin.MemberEquals(si.Skin))
                    {
                        // The user's skin is equal to the interface's skin
                        si.IsButtonActive = false;
                    }
                    else
                    {
                        si.IsButtonActive = true;
                    }
                }
            }

            foreach (SkinInterface si in skinInterfaces)
            {
                if (user.CurrentSkin.MemberEquals(si.Skin))
                {
                    // The user's skin is equal to the interface's skin
                    si.IsButtonActive = false;
                }
                else
                {
                    si.IsButtonActive = true;
                }
            }
        }

        private void SaveSettings()
        {
            bool shouldContinue = true;
            if (user != null)
            {
                user.ChangeMouseWhilePlaying(showMouseWhilePlayingBox.IsChecked);

                bool shouldChangeUsername = true;
                foreach (User u in User.LoadUsers())
                {
                    if (u.Username == changeUsernameBox.Content.Trim() && u.Id != user.Id)
                    {
                        // We've reached a username that is already taken
                        shouldChangeUsername = false;
                        shouldContinue = false;
                        needsRequiredFields.ShowPopup("\"" + changeUsernameBox.Content.Trim() + "\" " +
                            LanguageTranslator.Translate("already exists. Please choose a different\nusername."), false,
                            windowWidth / 2 - (needsRequiredFields.Width / 2), windowHeight / 2 - (needsRequiredFields.Height / 2));
                    }
                }
                if (changeUsernameBox.Content.Trim() == "")
                {
                    shouldChangeUsername = false;
                }

                if (shouldChangeUsername)
                {
                    user.ChangeUsername(changeUsernameBox.Content.Trim());
                }
            }

            if (shouldContinue)
            {
                mainMenuButton.Click();
            }
        }

        private void HandleEnterPress()
        {
            if (state == GameState.NewUser)
            {
                submitInfoButton.ClickWithSound();
            }
            else if (state == GameState.Settings)
            {
                saveSettingsButton.ClickWithSound();
            }
        }

        #endregion
    }
}