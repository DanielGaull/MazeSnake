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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Net;

namespace MazeSnake
{
    public delegate void MazeSnakeMethod(bool callOnExit);
    /// <summary>
    /// This class functions as the Game1 class, since the Game1 class isn't serializable
    /// </summary>
    [Serializable]
    class MazeSnake
    {
        #region Fields

        Random rand = new Random();

        Popup exitWarningPopup;
        bool isShowingPopup = false;

        Popup requestUpdatePopup;
        //bool needsUpdate = false;
        Version maxVersion = new Version();
        //string mazesnakeUrl = "purasu.itch.io/mazesnake";

        Snake snake;
        User player;
        User userToDelete;
        Color snakeColor = Color.White;

        Timer mazeTimer;
        Rectangle timerRect;
        const int TIMER_OFFSET = 10;

        Popup queryUserDeletePopup;

        string versionPrefix = LanguageTranslator.Translate("MazeSnake Version") + " ";
        Vector2 versionPos = new Vector2();

        //List<Firework> fireworks = new List<Firework>();

        SpriteFont bigFont;
        SpriteFont mediumFont;
        SpriteFont smallFont;

        List<Rectangle> snowflakes = new List<Rectangle>();
        Texture2D snowflakeImg;
        const int SNOWFLAKE_SIZE = 3;
        Checkbox showSnowBox;
        const int SNOW_BOX_SPACING = 50;

        Options universalSettings = new Options();

        int windowWidth = 0;
        int windowHeight = 0;

        bool backupUsers = true;

        Menu menu;

        InventoryInterface inventory;

        MouseCursor mouse;

        GameState gameState = GameState.MainMenu;
        GameState previousState = GameState.MainMenu;

        KeyboardState currentKeyboardState = new KeyboardState();
        KeyboardState previousKeyboardState = new KeyboardState();

        bool isMazeWorm = false;
        bool blackFriday = false;
        bool christmas = false;
        bool halloween = false;
        bool thanksgiving = false;
        bool easter = false;
        bool indepDay = false;

        const float BLACK_FRIDAY_DISCOUNT = 0.9f;

        bool customCursorVisible = true;

        List<Skin> skins;

        Texture2D powerUpTexture;

        Maze level;

        const int SPACING = 5;

        const int COINS_ON_LVL_UP = 10;

        const int XPBAR_WIDTH = 200;
        const int XPBAR_HEIGHT = 20;

        bool saved = true;

        const string WHITE_RECT_SPRITE = "whiterectangle";
        const string WHITE_CIRCLE_SPRITE = "whitecircle";
        Texture2D whiteRectImg;

        Notification achNotification;

        Popup blackFridayPopup;

        ContentManager content;

        MazeSnakeMethod exit;

        Timer waitAnd = new Timer(0, TimerUnits.Seconds);
        event Action waitAndDo = null;

        bool updateGame = true; 

        int streak = 0;

        #endregion

        #region Constructors

        public MazeSnake(ContentManager content, MazeSnakeMethod onExit, int windowWidth, int windowHeight)
        {
            try
            {
                this.windowWidth = windowWidth;
                this.windowHeight = windowHeight;
                this.content = content;
                this.exit = onExit;
            }
            #region Exception Handling
            catch (ArgumentNullException e)
            {
                HandleError(e, "Sorry, I don't deal with null.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                HandleError(e, "Oops, we can't count.");
            }
            catch (ArgumentException e)
            {
                HandleError(e, "Hey, kid, that's illegal!");
            }
            catch (DivideByZeroException e)
            {
                HandleError(e, "0 / 0 = ????????");
            }
            catch (NotFiniteNumberException e)
            {
                HandleError(e, "NO COUNTERFEIT NUMBERS");
            }
            catch (ArithmeticException e)
            {
                HandleError(e, "We'z bad at maths.");
            }
            catch (IndexOutOfRangeException e)
            {
                HandleError(e, "Counting ain't my strong suit.");
            }
            catch (InvalidCastException e)
            {
                HandleError(e, "Cannot convert");
            }
            catch (InvalidOperationException e)
            {
                HandleError(e, "Oops, that's not allowed");
            }
            catch (NullReferenceException e)
            {
                HandleError(e, "Not again!");
            }
            catch (OutOfMemoryException e)
            {
                HandleError(e, "I-I-I can't remember!");
            }
            catch (StackOverflowException e)
            {
                HandleError(e, "Thanks a lot");
            }
            catch (Exception e)
            {
                HandleError(e, null);
            }
            #endregion
        }

        #endregion

        #region Public Methods

        public void LoadContent()
        {
            try
            {
                #region Load Content

                // Resetting options
                //Options o = new Options();
                //o.DontShowCtrlWPopup = false;
                //o.Language = (int)Language.English;
                //o.RecThousandGift = false;
                //o.UsedBlackFridayDiscount = false;
                //o.Save();

                User.Initialize();

                // Initialize sounds
                Dictionary<Sounds, string> soundAssets = new Dictionary<Sounds, string>()
                {
                    { Sounds.Teleport, "teleportsound" },
                    { Sounds.Speed, "speedpw" },
                    { Sounds.Error, "error" },
                    { Sounds.Nature, "nature" },
                    { Sounds.Theme, "mazesnaketheme" },
                    { Sounds.WallBreaker, "powerupsound" },
                    { Sounds.Coin, "coincollect" },
                    { Sounds.Achievement, "achievement" },
                    { Sounds.Explosion, "explosion" },
                    { Sounds.Buzzing, "wallbreakbuzzing" },
                    { Sounds.Click, "buttonclick" },
                    { Sounds.Forcefield, "forcefieldsound" },
                    { Sounds.Lose, "lose" },
                    { Sounds.Freeze, "freezesound" },
                    { Sounds.Star, "starcollect" },
                };
                Sound.Initialize(content, soundAssets);

                // This code makes sure that removed and changed skins stay the way they are supposed to be
                // Let this be a lesson: Create Skin.[Skin] properties and have user store types
                // This way this code won't be needed
                List<User> users = User.LoadUsers();
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].CurrentSkin.Type == SnakeSkinType.PoshSnake && users[i].CurrentSkin.HasHat == false)
                    {
                        users[i].CurrentSkin = new Skin("poshsnake", Color.White, true, LanguageTranslator.Translate("Posh Snake"),
                            SnakeSkinType.PoshSnake, "tophat");
                    }

                    if (users[i].CurrentSkin.Type == SnakeSkinType.ElectricEel_Removed)
                    {
                        users[i].CurrentSkin = users[i].Skins[0];
                    }
                    else if (users[i].CurrentSkin.Type == SnakeSkinType.EasterSnake_Removed)
                    {
                        users[i].CurrentSkin = users[i].Skins[0];
                    }
                    else if (users[i].CurrentSkin.Type == SnakeSkinType.SecretSnake_Removed)
                    {
                        users[i].CurrentSkin = users[i].Skins[0];
                    }

                    for (int j = 0; j < users[i].Skins.Count - 1; j++)
                    {
                        if (users[i].Skins[j].Type == SnakeSkinType.PoshSnake && users[i].Skins[j].HasHat == false)
                        {
                            users[i].Skins.RemoveAt(j);
                            users[i].Skins.Add(new Skin("poshsnake", Color.White, true, LanguageTranslator.Translate("Posh Snake"), SnakeSkinType.PoshSnake,
                            "tophat"));
                        }
                        if (users[i].CurrentSkin.Type == SnakeSkinType.ElectricEel_Removed)
                        {
                            users[i].Skins.RemoveAt(j);
                        }
                        else if (users[i].CurrentSkin.Type == SnakeSkinType.SecretSnake_Removed)
                        {
                            users[i].Skins.RemoveAt(j);
                        }
                        else if (users[i].CurrentSkin.Type == SnakeSkinType.EasterSnake_Removed)
                        {
                            users[i].Skins.RemoveAt(j);
                        }
                    }
                }

                easter = Holidays.IsEasterWeek();
                indepDay = Holidays.IsIndependenceDayWeek();
                halloween = Holidays.IsHalloweenWeek();
                thanksgiving = Holidays.IsThanksgivingWeek();
                blackFriday = Holidays.IsBlackFriday();
                christmas = Holidays.IsChristmasWeek();

                whiteRectImg = content.Load<Texture2D>(WHITE_RECT_SPRITE);
                bigFont = content.Load<SpriteFont>("Font1");
                mediumFont = content.Load<SpriteFont>("mediumfont");
                smallFont = content.Load<SpriteFont>("smallfont");

                // Static initialize methods must ALWAYS be executed first so that other classes can use them as soon as they need them
                Dictionary<int, AchievementCriteria> criteriaDict = new Dictionary<int, AchievementCriteria>()
                {
                    { 0, new AchievementCriteria(IsFirstMazeAchComplete) },
                    { 1, new AchievementCriteria(IsSecondMazeAchComplete) },
                    { 2, new AchievementCriteria(IsThirdMazeAchComplete) },
                    { 3, new AchievementCriteria(IsFourthMazeAchComplete) },

                    { 4, new AchievementCriteria(MazeWormFound) },

                    { 5, new AchievementCriteria(IsFirstPowerupAchComplete) },
                    { 6, new AchievementCriteria(IsSecondPowerupAchComplete) },
                    { 7, new AchievementCriteria(IsThirdPowerupAchComplete) },
                    { 8, new AchievementCriteria(IsFourthPowerupAchComplete) },
                    { 9, new AchievementCriteria(IsFifthPowerupAchComplete) },
                    { 10, new AchievementCriteria(IsSixthPowerupAchComplete) },
                    { 11, new AchievementCriteria(IsSeventhPowerupAchComplete) },

                    { 12, new AchievementCriteria(IsFirstTeleportAchComplete) },
                    { 13, new AchievementCriteria(IsSecondTeleportAchComplete) },
                    { 14, new AchievementCriteria(IsThirdTeleportAchComplete) },
                    { 33, new AchievementCriteria(IsFourthTeleportAchComplete) },

                    { 16, new AchievementCriteria(IsFirstSpeedAchComplete) },
                    { 17, new AchievementCriteria(IsSecondSpeedAchComplete) },
                    { 18, new AchievementCriteria(IsThirdSpeedAchComplete) },
                    { 19, new AchievementCriteria(IsFourthSpeedAchComplete) },

                    { 20, new AchievementCriteria(IsFirstWallBreakAchComplete) },
                    { 21, new AchievementCriteria(IsSecondWallBreakAchComplete) },
                    { 22, new AchievementCriteria(IsThirdWallBreakAchievementComplete) },
                    { 23, new AchievementCriteria(IsFourthWallBreakAchievementComplete) },

                    { 24, new AchievementCriteria(IsMasterAchComplete) },

                    { 25, new AchievementCriteria(IsFirstForcefieldAchComplete) },
                    { 26, new AchievementCriteria(IsSecondForcefieldAchComplete) },
                    { 27, new AchievementCriteria(IsThirdForcefieldAchComplete) },
                    { 28, new AchievementCriteria(IsFourthForcefieldAchComplete) },

                    { 29, new AchievementCriteria(IsFirstFrozenAchComplete) },
                    { 30, new AchievementCriteria(IsSecondFrozenAchComplete) },
                    { 31, new AchievementCriteria(IsThirdFrozenAchComplete) },
                    { 32, new AchievementCriteria(IsFourthFrozenAchComplete) },
                };

                Dictionary<int, int> achCoinDict = new Dictionary<int, int>()
                {
                    { 0, 5 },
                    { 1, 10 },
                    { 2, 86 },
                    { 3, 1000 },
                    { 4, 300 },
                    { 5, 5 },
                    { 6, 10 },
                    { 7, 25},
                    { 8, 50 },
                    { 9, 100 },
                    { 10, 500 },
                    { 11, 1000 },
                    { 12, 20 },
                    { 13, 100 },
                    { 14, 200 },
                    { 33, 1000 },
                    { 15, 50 },
                    { 16, 10 },
                    { 17, 50 },
                    { 18, 100 },
                    { 19, 500 },
                    { 20, 20 },
                    { 21, 100 },
                    { 22, 200 },
                    { 23, 500 },
                    { 24, 1000 },
                    { 25, 15 },
                    { 26, 60 },
                    { 27, 125 },
                    { 28, 550 },
                    { 29, 10 },
                    { 30, 50 },
                    { 31, 100 },
                    { 32, 500 },
                };

                Dictionary<int, string> imgDict = new Dictionary<int, string>()
                {
                    { 0, "maze"},
                    { 1, "maze"},
                    { 2, "maze"},
                    { 3, "maze" },
                    { 4, "snake" },
                    { 5, "Powerup" },
                    { 6, "Powerup" },
                    { 7, "Powerup" },
                    { 8, "Powerup" },
                    { 9, "Powerup" },
                    { 10, "Powerup" },
                    { 11, "Powerup" },
                    { 12, "teleport" },
                    { 13, "teleport" },
                    { 14, "teleport" },
                    { 33, "teleport" },
                    { 15, "noaccess" },
                    { 16, "speedometer" },
                    { 17, "speedometer" },
                    { 18, "speedometer" },
                    { 19, "speedometer" },
                    { 20, "hammer" },
                    { 21, "hammer" },
                    { 22, "hammer" },
                    { 23, "hammer" },
                    { 24, "wizardsnake" },
                    { 25, "forcefieldpw" },
                    { 26, "forcefieldpw" },
                    { 27, "forcefieldpw" },
                    { 28, "forcefieldpw" },
                    { 29, "frozenpw" },
                    { 30, "frozenpw" },
                    { 31, "frozenpw" },
                    { 32, "frozenpw" },
                    // Add others here
                };

                Dictionary<int, AchievementMethod> rewardDict = new Dictionary<int, AchievementMethod>()
                {
                    { 0, new AchievementMethod(ShowAchNotification) },
                    { 1, new AchievementMethod(ShowAchNotification) },
                    { 2, new AchievementMethod(ShowAchNotification) },
                    { 3, new AchievementMethod(ShowAchNotification) },

                    { 4, new AchievementMethod(TheTwinEarned) },

                    { 5, new AchievementMethod(ShowAchNotification) },
                    { 6, new AchievementMethod(ShowAchNotification) },
                    { 7, new AchievementMethod(ShowAchNotification) },
                    { 8, new AchievementMethod(ShowAchNotification) },
                    { 9, new AchievementMethod(ShowAchNotification) },
                    { 10, new AchievementMethod(ShowAchNotification) },
                    { 11, new AchievementMethod(ShowAchNotification) },

                    { 12, new AchievementMethod(ShowAchNotification) },
                    { 13, new AchievementMethod(ShowAchNotification) },
                    { 14, new AchievementMethod(ShowAchNotification) },
                    { 33, new AchievementMethod(ShowAchNotification) },

                    { 15, new AchievementMethod(ShowAchNotification) },

                    { 16, new AchievementMethod(ShowAchNotification) },
                    { 17, new AchievementMethod(ShowAchNotification) },
                    { 18, new AchievementMethod(ShowAchNotification) },
                    { 19, new AchievementMethod(ShowAchNotification) },

                    { 20, new AchievementMethod(ShowAchNotification) },
                    { 21, new AchievementMethod(ShowAchNotification) },
                    { 22, new AchievementMethod(ShowAchNotification) },
                    { 23, new AchievementMethod(ShowAchNotification) },

                    { 24, new AchievementMethod(MazeMasterEarned) },

                    { 25, new AchievementMethod(ShowAchNotification) },
                    { 26, new AchievementMethod(ShowAchNotification) },
                    { 27, new AchievementMethod(ShowAchNotification) },
                    { 28, new AchievementMethod(ShowAchNotification) },

                    { 29, new AchievementMethod(ShowAchNotification) },
                    { 30, new AchievementMethod(ShowAchNotification) },
                    { 31, new AchievementMethod(ShowAchNotification) },
                    { 32, new AchievementMethod(ShowAchNotification) },
                };

                Achievements.Initialize(content, criteriaDict, imgDict, rewardDict, Color.SaddleBrown, achCoinDict);

                // A list of the tutorial images in order
                List<string> tutorialAssets = new List<string>()
                {
                    "tutorial_orig menu",
                    "tutorial_create user",
                    "tutorial_username",
                    "tutorial_pick user",
                    "tutorial_main menu",
                    "tutorial_loading",
                    "tutorial_maze",
                    "tutorial_achievement",
                    "tutorial_view achievement",
                    "tutorial_stats",
                    "tutorial_shop",
                    "tutorial_skins",
                    "tutorial_skin changed",
                    "tutorial_final",
                };

                exitWarningPopup = new Popup(content, new OnButtonClick(HandleCtrlW), "checkbox", "check",
                    smallFont, WHITE_RECT_SPRITE, windowWidth, windowHeight, true, bigFont);

                if (blackFriday)
                {
                    blackFridayPopup = new Popup(content, smallFont, WHITE_RECT_SPRITE, windowWidth, windowHeight, false, bigFont);
                    blackFridayPopup.ShowPopup(LanguageTranslator.Translate("It's Black Friday! Get awesome deals on all items in the shop. However, " +
                        "you can\nonly buy one item with this discount, so choose carefully!"), false, windowWidth / 2 - (blackFridayPopup.Width / 2),
                        windowHeight / 2 - (blackFridayPopup.Height / 2));
                }

                // Easter egg where there's a chance of it being "Maze Worm Mode" where the snake is a worm.
                // 1% chance of happening
                Skin skin;
                string titleAsset;
                isMazeWorm = (rand.Next(100) == 1);
                if (isMazeWorm)
                {
                    // Maze Worm mode
                    titleAsset = "mazewormlogo";
                    skin = new Skin("snake", Color.SaddleBrown, false, "MazeWorm", SnakeSkinType.MazeWorm);
                }
                else
                {
                    titleAsset = "mazesnakelogo";
                    // Normal Mode
                    skin = new Skin("snake", Color.DarkGreen, true, "MazeSnake", SnakeSkinType.MazeSnake);
                }

                Dictionary<ItemType, InventoryAction> itemActions = new Dictionary<ItemType, InventoryAction>()
                {
                    { ItemType.SpeedPowerup, new InventoryAction(SpeedUsed) },
                    { ItemType.TeleportPowerup, new InventoryAction(TeleportUsed) },
                    //{ ItemType.Finish, new InventoryAction(SkipUsed) },
                    { ItemType.WallBreakPowerup, new InventoryAction(WallBreakUsed) },
                    { ItemType.ForcefieldPowerup, new InventoryAction(ForcefieldUsed) },
                    { ItemType.FrozenPowerup, new InventoryAction(FrozenUsed) },
                };

                Dictionary<SnakeSkinType, int> skinCosts = new Dictionary<SnakeSkinType, int>()
                {
                    { SnakeSkinType.MazeSnake, 0 },
                    { SnakeSkinType.MazeWorm, 0 },
                    { SnakeSkinType.RedSnake, 75 },
                    { SnakeSkinType.OrangeSnake, 75 },
                    { SnakeSkinType.YellowSnake, 75 },
                    { SnakeSkinType.BlueSnake, 75 },
                    { SnakeSkinType.PurpleSnake, 75 },
                    { SnakeSkinType.AlbinoSnake, 125 },
                    { SnakeSkinType.SeaSnake, 140 },
                    { SnakeSkinType.CamoSnake, 150 },
                    { SnakeSkinType.RainbowSnake, 300 },
                    { SnakeSkinType.ZebraSnake, 250 },
                    { SnakeSkinType.SantaSnake, 450 },
                    { SnakeSkinType.PoshSnake, 550 },
                    { SnakeSkinType.BumbleSnake, 130 },
                    { SnakeSkinType.GhostSnake, 200 },
                    { SnakeSkinType.AmericaSnake, 0 },
                    { SnakeSkinType.SnowSnake, 130 },
                    { SnakeSkinType.CornucopiaSnake, 270 },
                    { SnakeSkinType.RoboSnake, 265 },
                    { SnakeSkinType.DullSnake, 25 },
                    { SnakeSkinType.PolkaDotSnake, 240 },
                    { SnakeSkinType.OldSnake, 160 },
                    { SnakeSkinType.YoungSnake, 160 },
                    { SnakeSkinType.PixelSnake, 180 },
                    { SnakeSkinType.BuilderSnake, 210 },
                    { SnakeSkinType.HomecomingSnake, 315 },
                    { SnakeSkinType.CosmicSnake, 170 },
                };
                Dictionary<ItemType, int> pwCosts = new Dictionary<ItemType, int>()
                {
                    { ItemType.SpeedPowerup, 15 },
                    { ItemType.TeleportPowerup, 10 },
                    { ItemType.WallBreakPowerup, 40 },
                    { ItemType.ForcefieldPowerup, 20 },
                    { ItemType.FrozenPowerup, 25 },
                    //{ ItemType.Finish, 50 },
                };
                Costs.Initialize(skinCosts, pwCosts);

                skins = new List<Skin>()
                {
                    new Skin("snake", Color.DarkRed, true, LanguageTranslator.Translate("Red Snake"), SnakeSkinType.RedSnake),
                    new Skin("snake", Color.Orange, true, LanguageTranslator.Translate("Orange Snake"), SnakeSkinType.OrangeSnake),
                    new Skin("snake", Color.Yellow, true, LanguageTranslator.Translate("Yellow Snake"), SnakeSkinType.YellowSnake),
                    new Skin("snake", Color.CornflowerBlue, true, LanguageTranslator.Translate("Blue Snake"), SnakeSkinType.BlueSnake),
                    new Skin("snake", Color.Purple, true, LanguageTranslator.Translate("Purple Snake"), SnakeSkinType.PurpleSnake),
                    new Skin("snake", Color.White, true, LanguageTranslator.Translate("Albino Snake"), SnakeSkinType.AlbinoSnake),
                    new Skin("snake", Color.SeaGreen, true, LanguageTranslator.Translate("Sea Snake"), SnakeSkinType.SeaSnake),
                    new Skin("camosnake", Color.White, true, LanguageTranslator.Translate("Camo Snake"), SnakeSkinType.CamoSnake),
                    new Skin("rainbowsnake", Color.White, true, LanguageTranslator.Translate("Rainbow Snake"), SnakeSkinType.RainbowSnake),
                    new Skin("zebrasnake", Color.White, true, LanguageTranslator.Translate("Zebra Snake"), SnakeSkinType.ZebraSnake),
                    new Skin("zebrasnake", Color.Yellow, true, LanguageTranslator.Translate("Bumble Snake"), SnakeSkinType.BumbleSnake),
                    new Skin("poshsnake", Color.White, true, LanguageTranslator.Translate("Posh Snake"), SnakeSkinType.PoshSnake, "tophat"),
                    new Skin("cybersnake", Color.White, false, LanguageTranslator.Translate("RoboSnake"), SnakeSkinType.RoboSnake),
                    new Skin("snake", Color.Gray, true, LanguageTranslator.Translate("Dull Snake"), SnakeSkinType.DullSnake),
                    new Skin("polkadotsnake", Color.White, true, LanguageTranslator.Translate("Polka Dot Snake"), SnakeSkinType.PolkaDotSnake),
                    new Skin("oldsnake", Color.White, true, LanguageTranslator.Translate("Old Snake"), SnakeSkinType.OldSnake),
                    new Skin("snake", Color.DarkGreen, true, LanguageTranslator.Translate("Young Snake"), SnakeSkinType.YoungSnake, "bballcap"),
                    new Skin("snake", Color.DarkGreen, true, LanguageTranslator.Translate("Builder Snake"), SnakeSkinType.BuilderSnake, "hardhat"),
                    new Skin("snake", Color.DarkGreen, true, LanguageTranslator.Translate("Homecoming Snake"), SnakeSkinType.HomecomingSnake, "gradhat"),
                    new Skin("pixelsnake", Color.White, false, LanguageTranslator.Translate("Pixel Snake"), SnakeSkinType.PixelSnake),
                    new Skin("cosmicsnake", Color.White, true, LanguageTranslator.Translate("Cosmic Snake"), SnakeSkinType.CosmicSnake),
                };
                if (christmas)
                {
                    skins.Add(new Skin("santasnake", Color.White, false, LanguageTranslator.Translate("Santa Snake"), SnakeSkinType.SantaSnake, "santahat"));
                }
                if (halloween)
                {
                    skins.Add(new Skin("ghostsnake", Color.White, false, "Ghost Snake", SnakeSkinType.GhostSnake));
                }
                if (Holidays.IsWinter())
                {
                    skins.Add(new Skin("snowmansnake", Color.White, false, "Snowsnake", SnakeSkinType.SnowSnake, "tophat"));
                }
                if (indepDay)
                {
                    skins.Add(new Skin("americasnake", Color.White, true, "Freedom Snake", SnakeSkinType.AmericaSnake));
                }
                if (thanksgiving)
                {
                    skins.Add(new Skin("cornucopiasnake", Color.White, false, "Cornucopia", SnakeSkinType.CornucopiaSnake));
                }
                skins = skins.OrderBy(s => skinCosts[s.Type]).ToList();

                //if (indepDay)
                //{
                //    // Create some fireworks at the bottom of the original screen for the player to launch
                //    for (int i = 0; i < 1; i++)
                //    {
                //        fireworks.Add(new Firework(content, "firework", "particle", Color.Red, /*rand.Next(windowWidth)*/100, windowHeight - Firework.ROCKET_HEIGHT, 
                //            /*rand.Next(90) - 45*/0, true));
                //    }
                //}

                requestUpdatePopup = new Popup(content, smallFont, WHITE_RECT_SPRITE, windowWidth, windowHeight, true, bigFont);
                queryUserDeletePopup = new Popup(content, smallFont, WHITE_RECT_SPRITE, windowWidth, windowHeight, true, bigFont, new OnButtonClick(DeleteUser));

                snowflakeImg = content.Load<Texture2D>(WHITE_CIRCLE_SPRITE);
                showSnowBox = new Checkbox("Show Snowflakes", content, mediumFont, "checkbox", "check");
                showSnowBox.IsChecked = true;
                showSnowBox.X = windowWidth / 2 - showSnowBox.Width / 2;
                showSnowBox.Y = windowHeight / 2 - showSnowBox.Height / 2 + SNOW_BOX_SPACING;

                // This code will prevent our user data from being lost every update
                // This code was found at robindotnet.wordpress.com/2009/08/19/where-do-i-put-my-data-to-keep-it-safe-from-clickonce-updates/
                // though parts of it were modified
                // We need to load this in first as certain objects, like the Menu, will not function without the
                // user files already being loaded
                //string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //string userFilePath = Path.Combine(localAppData, "Purasu");

                //if (!Directory.Exists(userFilePath))
                //{
                //    Directory.CreateDirectory(userFilePath);
                //}

                //infolderUserFilePath = /*Path.Combine(Application.StartupPath, User.MAIN_PATH)*/User.MAIN_PATH;
                //appDataUserFilePath = Path.Combine(userFilePath, User.MAIN_PATH);

                //if (!File.Exists(appDataUserFilePath))
                //{
                //    // We're likely running MazeSnake for the first time
                //    File.Copy(infolderUserFilePath, appDataUserFilePath);
                //}
                //else
                //{
                //    // The game has been run and the correct data is saved at appDataUserFilePath
                //    if (File.Exists(appDataUserFilePath) && File.Exists(infolderUserFilePath))
                //    {
                //        // To use File.Copy, the source file cannot currently exist at the time of the method's execution
                //        File.Delete(infolderUserFilePath);
                //    }

                //    File.Copy(appDataUserFilePath, infolderUserFilePath);
                //}

                versionPos = new Vector2(0, windowHeight - smallFont.MeasureString(versionPrefix + GameInfo.Version.ToString()).Y);

                Dictionary<Language, OnButtonClick> langClickValues = new Dictionary<Language, OnButtonClick>()
                {
                    { Language.English, new OnButtonClick(MakeLangEnglish) },
                    { Language.Spanish, new OnButtonClick(MakeLangSpanish) },
                    //{ Language.Backwards, new OnButtonClick(MakeLangBackwards) },
                };

                float discount = 0;
                if (blackFriday && !universalSettings.UsedBlackFridayDiscount)
                {
                    discount = BLACK_FRIDAY_DISCOUNT;
                }
                else
                {
                    if (!blackFriday)
                    {
                        // Reset the discount settings so that the user can use the discount the next year
                        if (universalSettings.UsedBlackFridayDiscount)
                        {
                            universalSettings.UsedBlackFridayDiscount = false;
                            universalSettings.Save();
                        }
                    }
                }

                List<InventoryItem> items = new List<InventoryItem>()
                {
                    new InventoryItem("Speed Powerup", ItemType.SpeedPowerup),
                    new InventoryItem("Teleport Powerup", ItemType.TeleportPowerup),
                    new InventoryItem("Electric Powerup", ItemType.WallBreakPowerup),
                    //new InventoryItem("Forcefield Powerup", ItemType.ForcefieldPowerup),

                    //new InventoryItem("Finish", ItemType.Finish),
                };
                Dictionary<ItemType, string> itemAssets = new Dictionary<ItemType, string>()
                {
                    { ItemType.SpeedPowerup, "speed" },
                    { ItemType.TeleportPowerup, "teleport" },
                    { ItemType.WallBreakPowerup, "hammer" },
                    //{ ItemType.ForcefieldPowerup, "forcefield" },

                    //{ ItemType.Finish, "finishpw" },
                };

                List<int> achOrder = new List<int>()
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12,
                    13,
                    14,
                    33,
                    16,
                    17,
                    18,
                    19,
                    20,
                    21,
                    22,
                    23,
                    25,
                    26,
                    27,
                    28,
                    29,
                    30,
                    31,
                    32,
                    15,
                    24,
                };

                menu = new Menu(content, mouse, windowWidth, windowHeight, skin, new OnButtonClick(MakeGameStatePickUser),
                        new OnButtonClick(Quit),
                        new OnButtonClick(ResumeGame), new OnButtonClick(MakeGameStateMainMenu),
                        new OnButtonClick(MakeGameStateNewUser),
                        new OnButtonClick(MakeGameStateAchievements), new OnButtonClick(MakeGameStateGameMode),
                        new OnButtonClick(MakeGameStatePostUserSelect),
                        new OnButtonClick(MakeGameStateStats), new OnButtonClick(MakeGameStateShop),
                        new OnButtonClick(MakeGameStatePickSkin),
                        new OnButtonClick(MakeGameStateSettings), WHITE_RECT_SPRITE, 100, "trashcan",
                        new OnUserButtonClick(WhenUsernameClicked),
                        new OnUserButtonClick(WhenTrashClicked), "textboximage", "checkbox", "check",
                        new OnUserCreation(OnUserCreation),
                        new SkinAction(SkinSelected), Color.Green, WHITE_CIRCLE_SPRITE, "Coin", "lock", "check", bigFont, mediumFont,
                        smallFont, titleAsset, "duopluslogo", "Powerup", "maze", itemActions, "tongue", isMazeWorm, skins,
                        "rightarrow", "leftarrow", "gear", new OnButtonClick(MakeGameStateCredits),
                        new OnButtonClick(MakeGameStateHelp), tutorialAssets,
                        new OnButtonClick(MakeGameStateLanguage), "languageicon", langClickValues, discount, new Action(ItemBought),
                        items, itemAssets, rand, "gift", "soundicon", "cross", new Action<GameMode>(x => BeginGame(x)), achOrder,
                        "tophat");

                snake = new Snake(new Skin("snake", Color.DarkGreen, true, "MazeSnake", SnakeSkinType.MazeSnake),
                    windowWidth, windowHeight, 0, InventoryInterface.BG_HEIGHT, 0, 0, content, "tongue",
                    "purplelightninganimation", "forcefield");
                snake.Width = Snake.SMALL_WIDTH;
                snake.Height = Snake.SMALL_HEIGHT;
                snake.ShadeColor = snakeColor;
                snake.HasTongue = !isMazeWorm;

                powerUpTexture = content.Load<Texture2D>("Powerup");

                level = new Maze(snake, content, "progressbarbox", WHITE_RECT_SPRITE, "smallfont", WHITE_RECT_SPRITE, windowWidth,
                    windowHeight, skin, rand, "Font1", isMazeWorm, "finish", "Powerup", titleAsset, "tongue",
                    snakeColor, "explosionsheet", smallFont, "enemy", "Coin", "star");
                level.AddEnemyHitHandler(OnLose);
                level.AddLevelCompleteHandler(OnLevelSuccess);

                mazeTimer = new Timer(TimerUnits.Seconds, GameInfo.STARTING_MAZE_TIMER_VAL, smallFont, "Time");
                mazeTimer.X = windowWidth - TIMER_OFFSET - mazeTimer.Width;
                mazeTimer.Y = InventoryInterface.BG_HEIGHT - mazeTimer.Height - TIMER_OFFSET;
                timerRect = new Rectangle(mazeTimer.X - TIMER_OFFSET / 2, mazeTimer.Y - TIMER_OFFSET / 2,
                    mazeTimer.Width + TIMER_OFFSET, mazeTimer.Height + TIMER_OFFSET);

                mouse = new MouseCursor(content, "Cursor", "typingcursor");

                inventory = new InventoryInterface(WHITE_RECT_SPRITE, content, new List<InventoryItem>(), 0, 0, windowWidth,
                    smallFont, itemActions, "speed", "teleport", "hammer", "forcefieldpw", "frozenpw");

                gameState = GameState.MainMenu;

                #region Updating Code
                //// Don't uncomment and test until game is public

                //// This is the auto-update feature, which makes it easy to update the game
                //// Check for updates
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(mazesnakeUrl);
                //req.Method = "GET";
                //WebResponse response = req.GetResponse();
                //StreamReader streamReader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                //string line;
                //while ((line = streamReader.ReadLine()) != null)
                //{
                //    if (line.StartsWith("Current Version: "))
                //    {
                //        // We have found the line that contains the current verison
                //        string versionStr = line.Substring("Current Version: ".Length);
                //        int major = int.Parse(versionStr.Substring(0, 1));
                //        int minor = int.Parse(versionStr.Substring(2, 3));
                //        int build = int.Parse(versionStr.Substring(4, 5));
                //        maxVersion = new Version(major, minor, build);

                //        if (GameInfo.Version/*Our current version*/ < maxVersion/*The maximum available version*/)
                //        {
                //            // There is new content available
                //            needsUpdate = true;
                //            requestUpdatePopup.ShowPopup("There is a new update for MazeSnake available (version {0}).\nWould you like to update?",
                //                false, windowWidth / 2 - (requestUpdatePopup.Width / 2), windowHeight / 2 - (requestUpdatePopup.Height / 2));
                //        }
                //    }
                //}
                //streamReader.Close();
                //response.Close();

                //// Update game if needed
                //if (needsUpdate)
                //{
                //    // Time to update the game
                //    WebBrowser msPage = new WebBrowser();
                //    msPage.Navigate(mazesnakeUrl);
                //    HtmlElementCollection classButton = msPage.Document.All;
                //    foreach (HtmlElement elem in classButton)
                //    {
                //        if (elem.GetAttribute("button download_btn") == "button")
                //        {
                //            elem.InvokeMember("click");
                //        }
                //    }

                //}
                #endregion

                #endregion
            }
            #region Exception Handling
            catch (ContentLoadException e)
            {
                HandleError(e, "Where is it..?");
            }
            catch (ArgumentNullException e)
            {
                HandleError(e, "Sorry, I don't deal with null.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                HandleError(e, "Oops, we can't count.");
            }
            catch (ArgumentException e)
            {
                HandleError(e, "Hey, kid, that's illegal!");
            }
            catch (DivideByZeroException e)
            {
                HandleError(e, "0 / 0 = ????????");
            }
            catch (NotFiniteNumberException e)
            {
                HandleError(e, "NO COUNTERFEIT NUMBERS");
            }
            catch (ArithmeticException e)
            {
                HandleError(e, "We'z bad at maths.");
            }
            catch (IndexOutOfRangeException e)
            {
                HandleError(e, "Counting ain't my strong suit.");
            }
            catch (InvalidCastException e)
            {
                HandleError(e, "Cannot convert");
            }
            catch (InvalidOperationException e)
            {
                HandleError(e, "Oops, that's not allowed");
            }
            catch (NullReferenceException e)
            {
                HandleError(e, "Not again!");
            }
            catch (OutOfMemoryException e)
            {
                HandleError(e, "I-I-I can't remember!");
            }
            catch (StackOverflowException e)
            {
                HandleError(e, "Thanks a lot");
            }
            catch (Exception e)
            {
                HandleError(e, null);
            }
            #endregion
        }

        public void Update(GameTime gameTime, bool isActive, ref bool isMouseVisible)
        {
            try
            {
                if (isActive)
                {
                    isMouseVisible = false;

                    Sound.Update();

                    if (waitAndDo != null)
                    {
                        waitAnd.Update(gameTime);
                        if (waitAnd.QueryWaitTime(gameTime))
                        {
                            waitAndDo();
                            waitAndDo = null;
                        }
                    }

                    #region Initialize Variables

                    GameState backupPreviousState = previousState;
                    previousKeyboardState = currentKeyboardState;
                    currentKeyboardState = Keyboard.GetState();

                    #endregion

                    #region Changing Game State

                    // Making sure previousState is correct so that there are no errors.
                    previousState = gameState;
                    backupPreviousState = previousState;
                    if (previousState == gameState)
                    {
                        previousState = backupPreviousState;
                    }

                    // Pause Controls
                    if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) &&
                        previousKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape))
                    {
                        HandleEscPress();
                    }

                    // Ability to open inventory by pressing "E"
                    //if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) &&
                    //    previousKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && gameState == GameState.Playing)
                    //{
                    //    HandleEPress();
                    //}

                    if (player != null && gameState == GameState.Playing)
                    {
                        customCursorVisible = player.Settings.ShowMouseWhilePlaying;
                    }
                    else
                    {
                        customCursorVisible = true;
                    }

                    isShowingPopup = exitWarningPopup.Active || requestUpdatePopup.Active
                        || queryUserDeletePopup.Active;

                    #endregion

                    #region Handle Popups

                    // Nice little ctrl + w shortcut to close the game
                    if ((currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)) && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        if (!universalSettings.DontShowCtrlWPopup)
                        {
                            exitWarningPopup.ShowPopup(LanguageTranslator.Translate("The shortcut Ctrl+W exits the game. Are you sure you want to do this?"),
                                true, 100, 100);
                        }
                        else
                        {
                            Quit();
                        }
                    }

                    #endregion

                    #region BG Music Handling

                    if (gameState == GameState.Playing)
                    {
                        if (Sound.IsPlaying(Sounds.Theme))
                        {
                            Sound.Stop(Sounds.Theme);
                        }
                        if (!Sound.IsPlaying(Sounds.Nature))
                        {
                            Sound.PlaySound(Sounds.Nature);
                        }
                    }
                    else if (gameState != GameState.Playing && gameState != GameState.Paused)
                    {
                        if (!Sound.IsPlaying(Sounds.Theme))
                        {
                            Sound.PlaySound(Sounds.Theme);
                        }
                        if (Sound.IsPlaying(Sounds.Nature))
                        {
                            Sound.Stop(Sounds.Nature);
                        }
                    }

                    #endregion

                    #region Update Game Objects

                    if (updateGame)
                    {
                        if (player != null)
                        {
                            snake.ChangeSkin(player.CurrentSkin);
                            // Don't update the timer unless we are playing and a maze is ready, 
                            // popups are not showing, the clock is not frozen and we are not in freeplay mode
                            if (gameState == GameState.Playing && level.LevelReady && !isShowingPopup &&
                                level.GameMode != GameMode.Freeplay)
                            {
                                if (snake.SnakeEffect == Effect.Frozen)
                                {
                                    if (Sound.IsPlaying(Sounds.Nature))
                                    {
                                        Sound.Stop(Sounds.Nature);
                                    }
                                }
                                else
                                {
                                    mazeTimer.Update(gameTime);
                                    if (mazeTimer.QueryWaitTime(gameTime))
                                    {
                                        OnLose();
                                    }
                                }
                            }
                        }

                        EnterDirection enterDir;
                        if (snake.Y < windowHeight / 2)
                        {
                            // Snake is in the top half of the maze
                            enterDir = EnterDirection.Bottom;
                        }
                        else
                        {
                            // Snake is in the bottom half of the maze
                            enterDir = EnterDirection.Top;
                        }
                        if (achNotification != null)
                        {
                            achNotification.Update(gameTime, enterDir);
                        }

                        if (player != null)
                        {
                            inventory.Items = player.Inventory;
                        }
                        if (gameState == GameState.Playing && level.LevelReady)
                        {
                            inventory.Update(snake.SnakeEffect, (int)snake.EffectTimeRemaining, level.GameMode);
                        }

                        if ((gameState == GameState.Playing) && !isShowingPopup && player != null)
                        {
                            level.Update(gameTime, whiteRectImg, content,
                                windowWidth, windowHeight, ref player, !(exitWarningPopup.Active));
                            saved = false;
                        }
                    }

                    if (blackFridayPopup != null)
                    {
                        if (blackFridayPopup.Active && !menu.ShowingLogo)
                        {
                            blackFridayPopup.Update();
                        }
                    }

                    if (gameState == GameState.PickUser)
                    {
                        if (queryUserDeletePopup.Active)
                        {
                            queryUserDeletePopup.Update();
                        }
                    }

                    // If it's winter (December & January), then add & update snowflakes
                    if (Holidays.IsWinter() && gameState != GameState.Paused)
                    {
                        if (gameState == GameState.MainMenu)
                        {
                            showSnowBox.Update();
                        }

                        if (showSnowBox.IsChecked)
                        {
                            // To add snowflakes, we add a rectangle to the snowflakes list
                            snowflakes.Add(new Rectangle(rand.Next(windowWidth), 0, SNOWFLAKE_SIZE, SNOWFLAKE_SIZE));

                            // Update existing snowflakes
                            for (int i = 0; i < snowflakes.Count; i++)
                            {
                                Rectangle replaceVal = snowflakes[i];
                                replaceVal.Y++;
                                snowflakes[i] = replaceVal;
                            }

                            // Check if any snowflakes are at the bottom of the screen. If so, remove them so they don't lag the program
                            for (int i = 0; i < snowflakes.Count; i++)
                            {
                                if (snowflakes[i].Y > windowHeight)
                                {
                                    snowflakes.RemoveAt(i);
                                }
                            }
                        }
                    }

                    versionPrefix = LanguageTranslator.Translate("MazeSnake Version") + " ";

                    // Make sure that no two skins in the user's inventory are the same
                    if (player != null)
                    {
                        player.Skins =
                            player.Skins
                            .GroupBy(skin => skin.Name)
                            .Select(g => g.First())
                            .ToList();
                    }

                    if (exitWarningPopup.Active)
                    {
                        exitWarningPopup.Update();
                        if (menu.ShowingLogo)
                        {
                            menu.Update(gameState, gameTime, windowWidth, windowHeight, ref player, previousState);
                        }
                    }
                    else
                    {
                        menu.Update(gameState, gameTime, windowWidth, windowHeight, ref player, previousState);
                    }

                    foreach (Achievement a in Achievements.AchievementsList)
                    {
                        if (a.CheckAndRewardCompletion(player) != null && a.CheckAndRewardCompletion(player) != player)
                        {
                            player = a.CheckAndRewardCompletion(player);
                        }
                    }

                    //if (!level.LevelReady && inventory.IsOpen)
                    //{
                    //    // We're loading a new maze, and have either completed one or are generating the first in the session
                    //    // Either way, we want to close the inventory so it isn't in the player's way
                    //    inventory.IsOpen = false;
                    //}

                    mouse.Update(menu.GetTextboxes());

                    #endregion

                    #region Save Game

                    // Whenever we are on the main menu, and the player has not yet been saved, we'll save the game
                    if ((gameState == GameState.MainMenu || gameState == GameState.PostUserLoginMenu ||
                        gameState == GameState.ViewAchievements) // The achNotification object allows users to go straight from playing to achievements
                        && !saved && player != null)
                    {
                        SaveGame();
                        saved = true;
                    }

                    #endregion
                }
                else
                {
                    // If we're not in the game, the user should be able to use the mouse over it, without wasting time and memory on a program that isn't being used
                    isMouseVisible = true;
                    if (menu.ShowingLogo)
                    {
                        // It's a pain to wait for logos to load, so we allow this if the player isn't viewing the window
                        menu.Update(gameState, gameTime, windowWidth, windowHeight, ref player, previousState);
                    }
                }
            }
            #region Exception Handling
            catch (ArgumentNullException e)
            {
                HandleError(e, "Sorry, I don't deal with null.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                HandleError(e, "Oops, we can't count.");
            }
            catch (ArgumentException e)
            {
                HandleError(e, "Hey, kid, that's illegal!");
            }
            catch (DivideByZeroException e)
            {
                HandleError(e, "0 / 0 = ????????");
            }
            catch (NotFiniteNumberException e)
            {
                HandleError(e, "NO COUNTERFEIT NUMBERS");
            }
            catch (ArithmeticException e)
            {
                HandleError(e, "We'z bad at maths.");
            }
            catch (IndexOutOfRangeException e)
            {
                HandleError(e, "Counting ain't my strong suit.");
            }
            catch (InvalidCastException e)
            {
                HandleError(e, "Cannot convert");
            }
            catch (InvalidOperationException e)
            {
                HandleError(e, "Oops, that's not allowed");
            }
            catch (NullReferenceException e)
            {
                HandleError(e, "Not again!");
            }
            catch (OutOfMemoryException e)
            {
                HandleError(e, "I-I-I can't remember!");
            }
            catch (StackOverflowException e)
            {
                HandleError(e, "Thanks a lot");
            }
            catch (Exception e)
            {
                HandleError(e, null);
            }
            #endregion
            finally
            {
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            try
            {
                // Draw snake, inventory, & level
                if (gameState == GameState.Playing || gameState == GameState.Paused)
                {
                    snake.Draw(spriteBatch);
                    level.Draw(spriteBatch);
                    if (level.LevelReady)
                    {
                        inventory.Draw(spriteBatch);
                    }
                }

                // Draw popups
                if (player != null)
                {
                    if ((gameState == GameState.Playing || gameState == GameState.Paused) && level.LevelReady
                        && level.GameMode != GameMode.Freeplay)
                    {
                        spriteBatch.Draw(whiteRectImg, timerRect, Color.Gray);
                        mazeTimer.Draw(spriteBatch, Color.White);
                    }
                }

                // Draw version text
                if (gameState == GameState.MainMenu)
                {
                    spriteBatch.DrawString(smallFont, versionPrefix + GameInfo.Version.ToString(), versionPos, Color.Black);
                }

                //// Draw fireworks
                //if (indepDay && gameState == GameState.MainMenu)
                //{
                //    for (int i = 0; i <= fireworks.Count - 1; i++)
                //    {
                //        fireworks[i].Draw(spriteBatch);
                //    }
                //}

                // Draw menu
                menu.DrawMenu(spriteBatch, gameState, player, gameState != GameState.Playing || level.LevelReady);

                // Draw Black Friday popup
                if (blackFridayPopup != null)
                {
                    if (blackFridayPopup.Active && !menu.ShowingLogo)
                    {
                        blackFridayPopup.Draw(spriteBatch);
                    }
                }

                // Draw query delete popup
                if (gameState == GameState.PickUser && queryUserDeletePopup.Active)
                {
                    queryUserDeletePopup.Draw(spriteBatch);
                }

                // Draw notifications (second-to-last)
                if (achNotification != null)
                {
                    achNotification.Draw(spriteBatch);
                }

                // Draw snowflakes, if it's winter
                if (Holidays.IsWinter() && (gameState != GameState.Playing || level.LevelReady) && !menu.ShowingLogo)
                {
                    if (showSnowBox.IsChecked)
                    {
                        foreach (Rectangle r in snowflakes)
                        {
                            spriteBatch.Draw(snowflakeImg, r, Color.White);
                        }
                    }
                    if (gameState == GameState.MainMenu)
                    {
                        showSnowBox.Draw(spriteBatch, Color.Black);
                    }
                }

                // Draw custom mouse cursor; always last
                if (!menu.ShowingLogo)
                {
                    exitWarningPopup.Draw(spriteBatch);
                    if (customCursorVisible)
                    {
                        mouse.Draw(spriteBatch);
                    }
                }
            }
            #region Exception Handling
            catch (ArgumentNullException e)
            {
                HandleError(e, "Sorry, I don't deal with null.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                HandleError(e, "Oops, we can't count.");
            }
            catch (ArgumentException e)
            {
                HandleError(e, "Hey, kid, that's illegal!");
            }
            catch (DivideByZeroException e)
            {
                HandleError(e, "0 / 0 = ????????");
            }
            catch (NotFiniteNumberException e)
            {
                HandleError(e, "NO COUNTERFEIT NUMBERS");
            }
            catch (ArithmeticException e)
            {
                HandleError(e, "We'z bad at maths.");
            }
            catch (IndexOutOfRangeException e)
            {
                HandleError(e, "Counting ain't my strong suit.");
            }
            catch (InvalidCastException e)
            {
                HandleError(e, "Cannot convert");
            }
            catch (InvalidOperationException e)
            {
                HandleError(e, "Oops, that's not allowed");
            }
            catch (NullReferenceException e)
            {
                HandleError(e, "Not again!");
            }
            catch (OutOfMemoryException e)
            {
                HandleError(e, "I-I-I can't remember!");
            }
            catch (StackOverflowException e)
            {
                HandleError(e, "Thanks a lot");
            }
            catch (Exception e)
            {
                HandleError(e, null);
            }
            #endregion
        }

        public void OnExiting(object sender, EventArgs args)
        {
            try
            {
                // Make sure game is saved before exiting
                Quit();
            }
            #region Exception Handling
            catch (ArgumentNullException e)
            {
                HandleError(e, "Sorry, I don't deal with null.");
            }
            catch (ArgumentOutOfRangeException e)
            {
                HandleError(e, "Oops, we can't count.");
            }
            catch (ArgumentException e)
            {
                HandleError(e, "Hey, kid, that's illegal!");
            }
            catch (DivideByZeroException e)
            {
                HandleError(e, "0 / 0 = ????????");
            }
            catch (NotFiniteNumberException e)
            {
                HandleError(e, "NO COUNTERFEIT NUMBERS");
            }
            catch (ArithmeticException e)
            {
                HandleError(e, "We'z bad at maths.");
            }
            catch (IndexOutOfRangeException e)
            {
                HandleError(e, "Counting ain't my strong suit.");
            }
            catch (InvalidCastException e)
            {
                HandleError(e, "Cannot convert");
            }
            catch (InvalidOperationException e)
            {
                HandleError(e, "Oops, that's not allowed");
            }
            catch (NullReferenceException e)
            {
                HandleError(e, "Not again!");
            }
            catch (OutOfMemoryException e)
            {
                HandleError(e, "I-I-I can't remember!");
            }
            catch (StackOverflowException e)
            {
                HandleError(e, "Thanks a lot");
            }
            catch (Exception e)
            {
                HandleError(e, null);
            }
            #endregion
        }

        #endregion

        #region Private Methods

        #region User Delegates

        private void SkinSelected(Skin skin)
        {
            if (player != null)
            {
                player.CurrentSkin = skin;
                snake.ChangeSkin(skin);
            }
        }
        private void WhenUsernameClicked(User user)
        {
            this.player = user;
            gameState = GameState.PostUserLoginMenu;
        }
        private void WhenTrashClicked(User user)
        {
            queryUserDeletePopup.ShowPopup(LanguageTranslator.Translate("Are you sure you want to delete") + " " + user.Username + "?", false,
                windowWidth / 2 - (queryUserDeletePopup.Width / 2), windowHeight / 2 - (queryUserDeletePopup.Height / 2));
            userToDelete = user;
        }
        private void DeleteUser()
        {
            List<User> users = User.LoadUsers();
            if (users != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Id == userToDelete.Id)
                    {
                        User.DeleteUser(userToDelete);
                        break;
                    }
                }
            }

            if (player != null)
            {
                if (player.Id == userToDelete.Id)
                {
                    player = null;
                }
            }

            menu.DeleteUser(userToDelete);
        }

        private void OnUserCreation(User user)
        {
            if (isMazeWorm)
            {
                user.Skins.Clear();
                user.Skins.Add(new Skin("snake", Color.SaddleBrown, false, "MazeWorm", SnakeSkinType.MazeWorm));
            }
            else
            {
                user.Skins.Clear();
                user.Skins.Add(new Skin("snake", Color.DarkGreen, true, "MazeSnake", SnakeSkinType.MazeSnake));
            }
            user.CurrentSkin = user.Skins[user.Skins.Count - 1];

            user.SerializeUser();
            gameState = GameState.PickUser;
        }

        #endregion

        #region Achievement Criteria Methods

        private bool MazeWormFound()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 4 /* 4 is the ID of the achievement "The Twin"*/)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && isMazeWorm)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstMazeAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 0 /* 0 is the ID of the achievement "Fin"*/)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.MazesCompleted >= 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondMazeAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 1 /* 1 is the ID of the achievement "x20"*/)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.MazesCompleted >= 20)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdMazeAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 2 /* 2 is the ID of the achievement "x86"*/)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.MazesCompleted >= 86)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthMazeAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 3 /* 3 is the ID of the achievement "Puzzle God"*/)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.MazesCompleted >= 1000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 5 /* 5 is the ID of the achievement "A Spark" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 6 /* 6 is the ID of the achievement "Lighting For Breakfast" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 10)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 7 /* 7 is the ID of the achievement "Lighting For Lunch" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 25)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 8 /* 8 is the ID of the achievement "Lightning For Dinner" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 50)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFifthPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 9 /* 9 is the ID of the achievement "A Delicacy" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSixthPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 10 /* 10 is the ID of the achievement "Electrified" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSeventhPowerupAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 11 /* 11 is the ID of the achievement "Living Power Source" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.PowerupsCollected >= 1000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstTeleportAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 12 /* 12 is the ID of the achievement "Poof!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimesTeleported >= 10)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondTeleportAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 13 /* 13 is the ID of the achievement "Wait..." */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimesTeleported >= 50)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdTeleportAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 14 /* 14 is the ID of the achievement "Where Am I?" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimesTeleported >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthTeleportAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 33 /* 33 is the ID of the achievement "HOW DID I GET HERE?!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimesTeleported >= 500)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstSpeedAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 16 /* 16 is the ID of the achievement "Zoom!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.SpeedSeconds >= 60)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondSpeedAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 17 /* 17 is the ID of the achievement "Speedy" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.SpeedSeconds >= 1800)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdSpeedAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 18 /* 18 is the ID of the achievement "Wow!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.SpeedSeconds >= 3600)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthSpeedAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 19 /* 19 is the ID of the achievement "Blink" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.SpeedSeconds >= 86400)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstWallBreakAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 20 /* 20 is the ID of the achievement "Iron Fist" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.WallsBroken >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondWallBreakAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 21 /* 21 is the ID of the achievement "Outta My Way!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.WallsBroken >= 500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdWallBreakAchievementComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 22 /* 22 is the ID of the achievement "Snake, SMASH!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.WallsBroken >= 1000)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthWallBreakAchievementComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 23 /* 23 is the ID of the achievement "Slayer of Weeds" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.WallsBroken >= 5000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstForcefieldAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 25 /* 25 is the ID of the achievement "Safe!" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.EnemiesAvoided >= 10)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondForcefieldAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 26 /* 26 is the ID of the achievement "Denied" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.EnemiesAvoided >= 50)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdForcefieldAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 27 /* 27 is the ID of the achievement "Not Today" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.EnemiesAvoided >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthForcefieldAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 28 /* 28 is the ID of the achievement "Can't Touch This" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.EnemiesAvoided >= 500)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFirstFrozenAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 29 /* 29 is the ID of the achievement "Chilly" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimeFrozen >= 60)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSecondFrozenAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 30 /* 30 is the ID of the achievement "Cold" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimeFrozen >= 1800)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsThirdFrozenAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 31 /* 31 is the ID of the achievement "Cold" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimeFrozen >= 3600)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFourthFrozenAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 32 /* 32 is the ID of the achievement "Cold" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.TimeFrozen >= 86400)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMasterAchComplete()
        {
            if (player != null)
            {
                bool doesNotHaveAch = true;
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 24 /* 24 is the ID of the achievement "Master of MazeSnake" */)
                    {
                        doesNotHaveAch = false;
                    }
                }
                if (doesNotHaveAch && player.AchievementsCompleted.Count >= Achievements.AchievementsList.Count - 1)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Custom Achievement Reward Methods

        private void TheTwinEarned(Achievement ach)
        {
            ShowAchNotification(ach);
            if (player != null)
            {
                Skin mazeWorm = new Skin("snake", Color.SaddleBrown, false, "MazeWorm", SnakeSkinType.MazeWorm);
                player.Skins.Add(mazeWorm);
                menu.AddSkin(mazeWorm);
                snake.ChangeSkin(mazeWorm);
            }
        }

        private void MazeMasterEarned(Achievement ach)
        {
            ShowAchNotification(ach);
            if (player != null)
            {
                Skin wizardSnake = new Skin("wizardsnake", Color.White, false, LanguageTranslator.Translate("Wizard"),
                    SnakeSkinType.WizardSnake);
                player.Skins.Add(wizardSnake);
                menu.AddSkin(wizardSnake);
                snake.ChangeSkin(wizardSnake);
            }
        }

        private void ShowAchNotification(Achievement ach)
        {
            Sound.PlaySound(Sounds.Achievement);
            achNotification = new Notification(content, WHITE_RECT_SPRITE,
                LanguageTranslator.Translate("You have completed the \nachievement") + "\""
                + ach.Name + "\"!",
                windowWidth, windowHeight, mediumFont, new OnButtonClick(GoToAchievement));
            achNotification.Show();
        }
        private void GoToAchievement()
        {
            if (player != null)
            {
                gameState = GameState.ViewAchievements;
            }
        }

        #endregion

        #region Item Use Methods

        private void SpeedUsed()
        {
            level.PowerUps.Add(new PowerUp(null, snake.X, snake.Y, snake, 1, 1, PowerupType.Speed, smallFont));
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i].Type == ItemType.SpeedPowerup)
                {
                    player.Inventory.RemoveAt(i);
                }
            }
        }
        private void TeleportUsed()
        {
            level.PowerUps.Add(new PowerUp(null, snake.X, snake.Y, snake, 1, 1, PowerupType.Teleport, smallFont));
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i].Type == ItemType.TeleportPowerup)
                {
                    player.Inventory.RemoveAt(i);
                    break;
                }
            }
        }
        //private void SkipUsed()
        //{
        //    snake.X = 0;
        //    snake.Y = 0;
        //    for (int i = 0; i < player.Inventory.Count; i++)
        //    {
        //        if (player.Inventory[i].Type == ItemType.Finish)
        //        {
        //            player.Inventory.RemoveAt(i);
        //        }
        //    }
        //}
        private void WallBreakUsed()
        {
            level.PowerUps.Add(new PowerUp(null, snake.X, snake.Y, snake, 1, 1, PowerupType.WallBreaker, smallFont));
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i].Type == ItemType.WallBreakPowerup)
                {
                    player.Inventory.RemoveAt(i);
                }
            }
        }
        private void ForcefieldUsed()
        {
            level.PowerUps.Add(new PowerUp(null, snake.X, snake.Y, snake, 1, 1, PowerupType.Forcefield, smallFont));
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i].Type == ItemType.ForcefieldPowerup)
                {
                    player.Inventory.RemoveAt(i);
                }
            }
        }
        private void FrozenUsed()
        {
            level.PowerUps.Add(new PowerUp(null, snake.X, snake.Y, snake, 1, 1, PowerupType.Frozen, smallFont));
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i].Type == ItemType.FrozenPowerup)
                {
                    player.Inventory.RemoveAt(i);
                }
            }
        }

        #endregion

        #region GameState Delegates

        private void HandleEscPress()
        {
            if (gameState == GameState.Paused)
            {
                ResumeGame();
            }
            else if (gameState == GameState.Playing && !level.ShowingLoadingScreen)
            {
                gameState = GameState.Paused;
                Sound.PauseAllSounds();
            }
        }

        private void ResumeGame()
        {
            gameState = GameState.Playing;
            Sound.ResumeAllSounds();
        }
        private void MakeGameStatePlaying()
        {
            gameState = GameState.Playing;
        }
        private void BeginGame(GameMode gameMode)
        {
            gameState = GameState.Playing;
            level.Coins = player.Coins;
            level.GenerateRandomLevel(gameMode);
        }
        private void MakeGameStateMainMenu()
        {
            gameState = GameState.MainMenu;
            player = null;
            streak = 0;
        }
        private void MakeGameStatePickUser()
        {
            gameState = GameState.PickUser;
        }
        private void MakeGameStateNewUser()
        {
            gameState = GameState.NewUser;
        }
        private void MakeGameStateAchievements()
        {
            gameState = GameState.ViewAchievements;
        }
        private void MakeGameStatePostUserSelect()
        {
            gameState = GameState.PostUserLoginMenu;
        }
        private void MakeGameStateStats()
        {
            gameState = GameState.ViewStatistics;
        }
        private void MakeGameStateShop()
        {
            gameState = GameState.Shop;
        }
        private void MakeGameStatePickSkin()
        {
            gameState = GameState.PickSkin;
        }
        private void MakeGameStateSettings()
        {
            gameState = GameState.Settings;
        }
        private void MakeGameStateCredits()
        {
            gameState = GameState.Credits;
        }
        private void MakeGameStateHelp()
        {
            gameState = GameState.Help;
        }
        private void MakeGameStateLanguage()
        {
            gameState = GameState.SelectLanguage;
        }
        private void MakeGameStateGameMode()
        {
            gameState = GameState.SelectGameMode;
        }

        private void HandleCtrlW()
        {
            if (exitWarningPopup.CheckboxChecked)
            {
                universalSettings.DontShowCtrlWPopup = true;
                universalSettings.Save();
            }
            Quit();
        }

        private void Quit()
        {
            try
            {
                if (saved)
                {
                    // We've already saved the game
                    exit(false);
                }
                // This makes sure that all files are saved before the game quits
                // Save all player data
                SaveGame();
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException || e is IOException || e is FileLoadException)
                {
                    backupUsers = false;
                }
            }
            finally
            {
                if (backupUsers)
                {
                    User.BackupUsers();
                }
                else
                {
                    // There has been some error with the files. Simply rewrite the original file with the backup file
                    User.OverwriteOrigFile();
                }
            }
            // Finally, exit the game
            exit(false);
        }

        #endregion

        #region Set Language Delegates

        private void MakeLangEnglish()
        {
            GameInfo.Language = Language.English;
            universalSettings.Language = (int)Language.English;
            universalSettings.Save();
        }
        private void MakeLangSpanish()
        {
            GameInfo.Language = Language.Spanish;
            universalSettings.Language = (int)Language.Spanish;
            universalSettings.Save();
        }
        //private void MakeLangBackwards()
        //{
        //    GameInfo.Language = Language.Backwards;
        //    universalSettings.Language = (int)Language.Backwards;
        //    universalSettings.Save();
        //}

        #endregion

        #region Other

        private void SaveGame()
        {
            if (player != null)
            {
                player.SerializeUser();
            }
        }

        private void OnLevelSuccess()
        {
            ResetLevel();
            player.AddToStat(Stat.MazesCompleted, 1);
            if (level.GameMode == GameMode.Classic)
            {
                player.AddCoins(GameInfo.RewardForStreak(streak));
                streak++;
            }
        }
        private void ResetLevel()
        {
            level.GenerateRandomLevel(level.GameMode);
            mazeTimer.Reset();
            updateGame = true;
        }
        private void OnLose()
        {
            Sound.PlaySound(Sounds.Lose);
            WaitAndDo(2, ResetLevel);
            updateGame = false;
            streak = 0;
        }

        private void WaitAndDo(int seconds, Action action)
        {
            waitAnd.WaitTime = seconds;
            waitAnd.Reset();
            waitAndDo += action;
        }

        private void HandleError(Exception e, string quote)
        {
            Sound.PlaySound(Sounds.Error);
            if (quote == null)
            {
                if (MessageBox.Show("Message: " + e.Message + "\nData: " + e.Data.ToString() + "\nStack Trace: " + e.StackTrace +
                    "\nSource: " + e.Source + "\nTarget Site" + e.TargetSite +
                    "\nThis is on us; you have no control over this error. Please contact your provider with this information.",
                    "There's been an error.", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            else
            {
                if (MessageBox.Show("There's been an error. Message: " + e.Message +
                        "\nData: " + e.Data.ToString() + "\nStack Trace: " + e.StackTrace +
                        "\nThis is on us; you have no control over this error. Please contact your provider with this information.", "\"" + quote + "\"",
                        MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private void ItemBought()
        {
            if (!universalSettings.UsedBlackFridayDiscount)
            {
                // End the discount
                menu.EndDiscount();
                universalSettings.UsedBlackFridayDiscount = true;
                universalSettings.Save();
            }
        }

        #endregion

        #endregion
    }

    #region Enumerations

    public enum GameState
    {
        Playing,
        MainMenu,
        Paused,
        PickUser,
        NewUser,
        ViewAchievements,
        PostUserLoginMenu,
        ViewStatistics,
        Shop,
        PickSkin,
        Settings,
        Credits,
        Help,
        SelectLanguage,
        SelectGameMode,
    }

    public enum GameMode
    {
        Classic,
        Freeplay,
        Star,
    }

    #endregion
}