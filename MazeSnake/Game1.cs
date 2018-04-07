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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields

        //Random rand = new Random();

        //Popup exitWarningPopup;
        //bool isShowingPopup = false;

        //SpriteFont font;

        //Snake snake;
        //User player;

        //PopupOptions popupSettings = new PopupOptions();

        //Menu menu;

        //InventoryInterface inventory;
        
        //MouseCursor mouse;

        //GameState gameState = GameState.MainMenu;
        //GameState previousState = GameState.MainMenu;

        //KeyboardState currentKeyboardState = new KeyboardState();
        //KeyboardState previousKeyboardState = new KeyboardState();

        //bool isMazeWorm = false;

        //bool levelGenerationStarted = false;

        //Texture2D powerUpTexture;

        //Texture2D wallImage;

        //Level level;

        //Song theme;

        //bool saved = true;

        //const string WHITE_RECT_SPRITE = "white rectangle";

        //Notification achNotification;

        int windowWidth = 815;
        int windowHeight = 676;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool isMouseVisible = false;

        MazeSnake game;

        #endregion

        #region Constructors
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;

            IsMouseVisible = false;
            //Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += new EventHandler<EventArgs>(WindowClientSizeChanged);
        }
        #endregion

        #region Protected and Private Methods

        //protected void WindowClientSizeChanged(object sender, EventArgs e)
        //{
        //    Rectangle windowSize = GraphicsDevice.PresentationParameters.Bounds;
        //    windowWidth = windowSize.Width;
        //    windowHeight = windowSize.Height;
        //}

        protected override void OnExiting(object sender, EventArgs args)
        {
            // Call custom quit method for custom behavior
            this.Quit(true);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //// Static initialize methods must ALWAYS be executed first so that other classes can use them as soon as they need them
            //Dictionary<int, AchievementCriteria> criteriaDict = new Dictionary<int, AchievementCriteria>()
            //{
            //    { 0, new AchievementCriteria(IsFirstMazeAchComplete) },
            //    { 1, new AchievementCriteria(IsSecondMazeAchComplete) },
            //    { 2, new AchievementCriteria(IsThirdMazeAchComplete) },

            //    { 3, new AchievementCriteria(MazeWormFound) },

            //    { 4, new AchievementCriteria(IsFirstPowerupAchComplete) },
            //    { 5, new AchievementCriteria(IsSecondPowerupAchComplete) },
            //    { 6, new AchievementCriteria(IsThirdPowerupAchComplete) },
            //    { 7, new AchievementCriteria(IsFourthPowerupAchComplete) },
            //    { 8, new AchievementCriteria(IsFifthPowerupAchComplete) },
            //    { 9, new AchievementCriteria(IsSixthPowerupAchComplete) },
            //    { 10, new AchievementCriteria(IsSeventhPowerupAchComplete) },

            //    { 11, new AchievementCriteria(IsFirstTeleportAchComplete) },
            //    { 12, new AchievementCriteria(IsSecondTeleportAchComplete) },
            //    { 13, new AchievementCriteria(IsThirdTeleportAchComplete) }, 

            //    { 14, new AchievementCriteria(IsPressGAchComplete) },

            //    { 15, new AchievementCriteria(IsFirstSpeedAchComplete) },
            //    { 16, new AchievementCriteria(IsSecondSpeedAchComplete) },
            //    { 17, new AchievementCriteria(IsThirdSpeedAchComplete) },
            //    { 18, new AchievementCriteria(IsFourthSpeedAchComplete) },
            //};

            //Dictionary<int, string> imgDict = new Dictionary<int, string>()
            //{
            //    { 0, "maze"},
            //    { 1, "maze"},
            //    { 2, "maze"},
            //    { 3, "mazeworm" },
            //    { 4, "Powerup" },
            //    { 5, "Powerup" },
            //    { 6, "Powerup" },
            //    { 7, "Powerup" },
            //    { 8, "Powerup" },
            //    { 9, "Powerup" },
            //    { 10, "Powerup" },
            //    { 11, "teleportsnake" },
            //    { 12, "teleportsnake" },
            //    { 13, "teleportsnake" },
            //    { 14, "noaccess" },
            //    { 15, "speedometer" },
            //    { 16, "speedometer" },
            //    { 17, "speedometer" },
            //    { 18, "speedometer" },
            //    // Add others here
            //};

            //Dictionary<int, AchievementMethod> rewardDict = new Dictionary<int, AchievementMethod>()
            //{
            //    { 0, new AchievementMethod(ShowAchNotification) }, 
            //    { 1, new AchievementMethod(ShowAchNotification) }, 
            //    { 2, new AchievementMethod(ShowAchNotification) }, 

            //    { 3, new AchievementMethod(TheTwinEarned) }, 

            //    { 4, new AchievementMethod(ShowAchNotification) },
            //    { 5, new AchievementMethod(ShowAchNotification) },
            //    { 6, new AchievementMethod(ShowAchNotification) },
            //    { 7, new AchievementMethod(ShowAchNotification) },
            //    { 8, new AchievementMethod(ShowAchNotification) },
            //    { 9, new AchievementMethod(ShowAchNotification) },
            //    { 10, new AchievementMethod(LivingPowerSourceEarned) },
                
            //    { 11, new AchievementMethod(ShowAchNotification) },
            //    { 12, new AchievementMethod(ShowAchNotification) },
            //    { 113, new AchievementMethod(ShowAchNotification) },
                
            //    { 14, new AchievementMethod(ShowAchNotification) },
                
            //    { 15, new AchievementMethod(ShowAchNotification) },
            //    { 16, new AchievementMethod(ShowAchNotification) },
            //    { 17, new AchievementMethod(ShowAchNotification) },
            //    { 18, new AchievementMethod(ShowAchNotification) },
            //};

            //Achievements.Initialize(Content, criteriaDict, imgDict, rewardDict);

            //exitWarningPopup = new Popup(Content, new OnButtonClick(HandleCtrlW), "checkbox", "check",
            //    "smallfont", WHITE_RECT_SPRITE, windowWidth, windowHeight, true, Content.Load<SpriteFont>("Font1"));

            //// Easter egg where there's a chance of it being "Maze Worm Mode" where the snake is a worm.
            //// 1% chance of happening
            //int randomNum = rand.Next(100);
            //string logoString;
            //string titleAsset;
            //if (randomNum == 1)
            //{
            //    // Maze Worm mode
            //    logoString = "mazeworm";
            //    isMazeWorm = true;
            //    titleAsset = "mazewormlogo";
            //}
            //else
            //{
            //    // Normal Mode
            //    logoString = "snake";
            //    isMazeWorm = false;
            //    titleAsset = "mazesnakelogo";
            //}

            //Dictionary<ItemType, InventoryAction> itemActions = new Dictionary<ItemType, InventoryAction>()
            //{
            //    { ItemType.SpeedPowerup, new InventoryAction(SpeedUsed) },
            //    { ItemType.TeleportPowerup, new InventoryAction(TeleportUsed) },
            //    { ItemType.InstaFin, new InventoryAction(SkipUsed) },
            //};

            //menu = new Menu(Content, mouse, windowWidth, windowHeight, logoString, new OnButtonClick(MakeGameStatePickUser), new OnButtonClick(Quit),
            //        new OnButtonClick(MakeGameStatePlaying), new OnButtonClick(MakeGameStateMainMenu), new OnButtonClick(MakeGameStateNewUser),
            //        new OnButtonClick(MakeGameStateAchievements), new OnButtonClick(BeginGame), new OnButtonClick(MakeGameStatePostUserSelect),
            //        new OnButtonClick(MakeGameStateStats), new OnButtonClick(MakeGameStateShop), Color.White, WHITE_RECT_SPRITE, 100, 
            //        "trashcan", new OnUserButtonClick(WhenUsernameClicked), new OnUserButtonClick(WhenTrashClicked), 
            //        "textboximage", "checkbox", "check", new OnUserCreation(OnUserCreation), 
            //        Color.Green, "whitecircle", "Coin", "lock", "check", Content.Load<SpriteFont>("Font1"), Content.Load<SpriteFont>("mediumfont"), 
            //        Content.Load<SpriteFont>("smallfont"), titleAsset, "daniellogo", "Powerup", "maze", itemActions);

            //theme = Content.Load<Song>("MazeSnakeTheme");

            //snake = new Snake("snake", windowWidth, windowHeight, 0, 0, Content, "smallfont", 50, 25, "hedgehog",
            //        isMazeWorm);

            //powerUpTexture = Content.Load<Texture2D>("Powerup");

            //wallImage = Content.Load<Texture2D>("wall");

            //level = new Level(snake, Content, "progressbarbox", WHITE_RECT_SPRITE, "smallfont", WHITE_RECT_SPRITE, windowWidth, windowHeight,
            //    logoString, rand, "Font1", isMazeWorm, "finish", "Powerup", titleAsset);

            //mouse = new MouseCursor(Content, "Cursor", "typingcursor");

            //font = Content.Load<SpriteFont>("smallfont");

            //inventory = new InventoryInterface(WHITE_RECT_SPRITE, Content, new List<InventoryItem>(), "leftarrow", windowWidth, windowHeight, font);

            //gameState = GameState.MainMenu;


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            game = new MazeSnake(Content, new MazeSnakeMethod(this.Quit), windowWidth, windowHeight);
            game.LoadContent();
        }

        protected void Quit(bool callOnExit)
        {
            if (callOnExit)
            {
                game.OnExiting(null, null);
            }
            this.Exit();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //try
            //{
            //    if (this.IsActive)
            //    {
            //        IsMouseVisible = false;

            //        #region Initialize Variables

            //        GameState backupPreviousState = previousState;
            //        previousKeyboardState = currentKeyboardState;
            //        currentKeyboardState = Keyboard.GetState();

            //        #endregion

            //        #region Changing Game State

            //        // Making sure previousState is correct so that there are no errors.
            //        previousState = gameState;
            //        backupPreviousState = previousState;
            //        if (previousState == gameState)
            //        {
            //            previousState = backupPreviousState;
            //        }

            //        // Pause Controls
            //        if (currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) && previousKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Escape))
            //        {
            //            HandleEscPress();
            //        }

            //        if (previousState != GameState.MainMenu)
            //        {
            //        }

            //        if (gameState == GameState.Playing && previousState == GameState.MainMenu && !levelGenerationStarted)
            //        {
            //            level.GenerateRandomLevel();
            //            levelGenerationStarted = true;
            //        }

            //        isShowingPopup = exitWarningPopup.Active;

            //        #endregion

            //        #region Handle Popups

            //        // Nice little ctrl + w shortcut to close the game
            //        if ((currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl)) && currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            //        {
            //            if (!popupSettings.DontShowCtrlWPopup)
            //            {
            //                exitWarningPopup.ShowPopup("The shortcut Ctrl+W exits the game. Are you sure you want to do this?", true, 100, 100);
            //            }
            //            else
            //            {
            //                Quit();
            //            }
            //        }

            //        #endregion

            //        #region BG Music Handling

            //        if (MediaPlayer.State == MediaState.Paused && gameState == GameState.Playing)
            //        {
            //            MediaPlayer.Resume();
            //        }
            //        else if (MediaPlayer.State == MediaState.Paused && (gameState == GameState.MainMenu || gameState == GameState.PickUser
            //            || gameState == GameState.NewUser || gameState == GameState.PostUserLoginMenu) && !menu.ShowingLogos)
            //        {
            //            MediaPlayer.Play(theme);
            //        }
            //        else if (gameState == GameState.Paused)
            //        {
            //            MediaPlayer.Pause();
            //        }
            //        else if (gameState == GameState.Playing)
            //        {
            //            MediaPlayer.Resume();
            //        }
            //        else if (MediaPlayer.State == MediaState.Stopped && gameState != GameState.Paused && !menu.ShowingLogos)
            //        {
            //            MediaPlayer.Play(theme);
            //        }
            //        #endregion

            //        #region Update Game Objects

            //        if ((gameState == GameState.Playing || gameState == GameState.Shop) && !isShowingPopup && player != null)
            //        {
            //            level.Update(level.GenerateRandomLevel, gameTime, wallImage, Content, "Coin", windowWidth, windowHeight, ref player);
            //            saved = false;
            //        }

            //        if (gameState == GameState.Playing && player == null)
            //        {
            //            // This means that a player has been selected, and they're playing a level, yet the player is null.
            //            // This cannot happen, so we throw an exception to prevent it. 
            //            throw new Exception("Player cannot be null while level is being completed. \nThis appears to be some form of user hack, as it is unnatural, " +
            //                "but this may be a bug.");
            //        }

            //        if (isShowingPopup)
            //        {
            //            exitWarningPopup.Update();
            //        }

            //        if (!isShowingPopup)
            //        {
            //            menu.Update(gameState, gameTime, windowWidth, windowHeight, ref player);
            //        }

            //        foreach (Achievement a in Achievements.AchievementsList)
            //        {
            //            if (a.CheckAndRewardCompletion(player) != null && a.CheckAndRewardCompletion(player) != player)
            //            {
            //                player = a.CheckAndRewardCompletion(player);
            //            }
            //        }

            //        if (achNotification != null)
            //        {
            //            achNotification.Update(gameTime);
            //        }

            //        if (player != null)
            //        {
            //            inventory.Items = player.Inventory;
            //        }
            //        if (gameState == GameState.Playing)
            //        {
            //            inventory.Update();
            //        }

            //        mouse.Update(menu.GetTextboxes());

            //        base.Update(gameTime);

            //        #endregion

            //        #region Save Game

            //        // Whenever we are on the main menu, and the player has not yet been saved, we'll save the game
            //        if ((gameState == GameState.MainMenu || gameState == GameState.PostUserLoginMenu) && !saved && player != null)
            //        {
            //            player.SerializeUser();
            //            saved = true;
            //        }

            //        #endregion
            //    }
            //    else
            //    {
            //        // If we're not in the game, the user should be able to use the mouse over it, without wasting time and memory on a program that isn't being used
            //        IsMouseVisible = true;
            //    }
            //}

            //#region Exception Handling
            ////catch (Exception e)
            ////{
            ////    Console.WriteLine("There's been an error in your game. Here's the message generated: " + e.Message +
            ////        "\nContact your provider with this information: " + "\nException Source Method: " + e.TargetSite
            ////         + "\nError Message: " + e.Message + "\nData: " + e.Data + "\nInnerException: " + e.InnerException + "\nSource: " + e.Source +
            ////         "\nStackTrace: " + e.StackTrace + "\n--------------------------------" +
            ////         "\nWe are sorry for the inconvenience. It is possible that you may have accidentally deleted a source \nfile that is " +
            ////         "critical to the program's operation. Again, we apologize. This error will hopefully be fixed in the next update.");
            ////    while (true) { }
            ////}
            //finally
            //{
            //}
            //#endregion

            this.isMouseVisible = this.IsMouseVisible;

            game.Update(gameTime, this.IsActive, ref isMouseVisible);

            try
            {
                IsMouseVisible = isMouseVisible;
            }
            catch (NullReferenceException)
            {

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            spriteBatch.Begin();

            //if (gameState == GameState.Playing || gameState == GameState.Paused)
            //{
            //    snake.Draw(spriteBatch);
            //    level.Draw(spriteBatch);
            //    if (level.LevelReady)
            //    {
            //        inventory.Draw(spriteBatch);
            //    }
            //}

            //menu.DrawMenu(spriteBatch, gameState, player, gameState != GameState.Playing || level.LevelReady);

            //if (achNotification != null)
            //{
            //    achNotification.Draw(spriteBatch);
            //}

            //if (!menu.ShowingLogos)
            //{
            //    exitWarningPopup.Draw(spriteBatch);
            //    mouse.Draw(spriteBatch, gameState);
            //}

            game.Draw(spriteBatch, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //#region GameState Delegates

        //private void HandleEscPress()
        //{
        //    if (gameState == GameState.Paused)
        //    {
        //        gameState = GameState.Playing;
        //    }
        //    else if (gameState == GameState.Playing && !level.ShowingLoadingScreen)
        //    {
        //        gameState = GameState.Paused;
        //    }
        //}

        //private void ResumeGame()
        //{
        //    gameState = GameState.Playing;
        //    MediaPlayer.Resume();
        //}
        //private void MakeGameStatePlaying()
        //{
        //    gameState = GameState.Playing;
        //}
        //private void BeginGame()
        //{
        //    gameState = GameState.Playing;
        //    level.Coins = player.Coins;
        //    level.GenerateRandomLevel();
        //}
        //private void MakeGameStateMainMenu()
        //{
        //    gameState = GameState.MainMenu;
        //    player = null;
        //}
        //private void MakeGameStatePickUser()
        //{
        //    gameState = GameState.PickUser;
        //}
        //private void MakeGameStateNewUser()
        //{
        //    gameState = GameState.NewUser;
        //}
        //private void MakeGameStateAchievements()
        //{
        //    gameState = GameState.ViewAchievements;
        //}
        //private void MakeGameStatePostUserSelect()
        //{
        //    gameState = GameState.PostUserLoginMenu;
        //}
        //private void MakeGameStateStats()
        //{
        //    gameState = GameState.ViewStatistics;
        //}
        //private void MakeGameStateShop()
        //{
        //    gameState = GameState.Shop;
        //}

        //private void HandleCtrlW()
        //{
        //    if (exitWarningPopup.CheckboxChecked)
        //    {
        //        popupSettings.DontShowCtrlWPopup = true;
        //        popupSettings.Save();
        //    }
        //    Quit();
        //}

        //private void Quit()
        //{
        //    // This makes sure that all files are saved before the game quits
        //    // Save all player data
        //    if (player != null)
        //    {
        //        player.SerializeUser();
        //    }
        //    // Finally, exit the game
        //    Exit();
        //}

        //#endregion

        //#region User Delegates

        //private void WhenUsernameClicked(User user)
        //{
        //    this.player = user;
        //    gameState = GameState.PostUserLoginMenu;
        //}
        //private void WhenTrashClicked(User user)
        //{
        //    List<User> users = User.LoadUsers();
        //    if (users != null)
        //    {
        //        for (int i = 0; i < users.Count; i++)
        //        {
        //            if (users[i].ID == user.ID)
        //            {
        //                User.DeleteUser(user);
        //                break;
        //            }
        //        }
        //    }

        //    if (player != null)
        //    {
        //        if (player.ID == user.ID)
        //        {
        //            player = null;
        //        }
        //    }
        //}

        //private void OnUserCreation(User user)
        //{
        //    user.SerializeUser();
        //    gameState = GameState.PickUser;
        //}

        //#endregion

        //#region Achievement Criteria Methods

        //private bool MazeWormFound()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 3 /* 3 is the ID of the achievement "The Twin"*/)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && isMazeWorm)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool IsFirstMazeAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 0 /* 0 is the ID of the achievement "Fin"*/)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.MazesCompleted >= 1)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSecondMazeAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 1 /* 1 is the ID of the achievement "x5"*/)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.MazesCompleted >= 5)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsThirdMazeAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 2 /* 2 is the ID of the achievement "x86"*/)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.MazesCompleted >= 86)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool IsFirstPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 4 /* 4 is the ID of the achievement "A Spark" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 1)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSecondPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 5 /* 5 is the ID of the achievement "Lighting For Breakfast" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 10)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsThirdPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 6 /* 6 is the ID of the achievement "Lighting For Lunch" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 25)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsFourthPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 7 /* 7 is the ID of the achievement "Lightning For Dinner" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 50)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsFifthPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 8 /* 8 is the ID of the achievement "A Delicacy" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 100)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSixthPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 9 /* 9 is the ID of the achievement "Electrified" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 500)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSeventhPowerupAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 10 /* 10 is the ID of the achievement "A Spark" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.PowerupsCollected >= 1000)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool IsFirstTeleportAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 11 /* 11 is the ID of the achievement "Poof!" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.TimesTeleported >= 10)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSecondTeleportAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 12 /* 12 is the ID of the achievement "Wait..." */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.TimesTeleported >= 50)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsThirdTeleportAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 13 /* 13 is the ID of the achievement "Where Am I?" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.TimesTeleported >= 100)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool IsFirstSpeedAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 15 /* 15 is the ID of the achievement "Zoom!" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.SpeedSeconds >= 60)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsSecondSpeedAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 16 /* 16 is the ID of the achievement "Speedy" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.SpeedSeconds >= 1800)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsThirdSpeedAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 17 /* 17 is the ID of the achievement "Wow!" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.SpeedSeconds >= 3600)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private bool IsFourthSpeedAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 18 /* 18 is the ID of the achievement "Blink" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && player.SpeedSeconds >= 86400)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private bool IsPressGAchComplete()
        //{
        //    if (player != null)
        //    {
        //        bool doesNotHaveAch = true;
        //        foreach (Achievement ach in player.AchievementsCompleted)
        //        {
        //            if (ach.ID == 14 /* 14 is the ID of the achievement "Gee, Thanks" */)
        //            {
        //                doesNotHaveAch = false;
        //            }
        //        }
        //        if (doesNotHaveAch && level.GPressed)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //#endregion

        //#region Custom Achievement Reward Methods

        //private void TheTwinEarned(Achievement ach)
        //{
        //    ShowAchNotification(ach);
        //    if (player != null)
        //    {
        //        player.Skins.Add("mazeworm");
        //        player.CurrentSkinAsset = "mazeworm";
        //    }
        //}

        //private void LivingPowerSourceEarned(Achievement ach)
        //{
        //    ShowAchNotification(ach);
        //    if (player != null)
        //    {
        //        player.Skins.Add("electriceel");
        //        player.CurrentSkinAsset = "electriceel";
        //        snake.ChangeSkin(Content.Load<Texture2D>("electriceel"));
        //    }
        //}

        //private void ShowAchNotification(Achievement ach)
        //{
        //    achNotification = new Notification(Content, WHITE_RECT_SPRITE, "You have completed the \nachievement \"" + ach.Name + "\"!",
        //        windowWidth, windowHeight, Content.Load<SpriteFont>("mediumfont"), new OnButtonClick(GoToAchievement));
        //    achNotification.Show();
        //}
        //private void GoToAchievement()
        //{
        //    if (player != null)
        //    {
        //        gameState = GameState.ViewAchievements;
        //    }
        //}

        //#endregion

        //#region Item Use Methods

        //private void SpeedUsed()
        //{
        //    snake.Effect = Effect.Speed;
        //}
        //private void TeleportUsed()
        //{
        //    snake.Teleport(rand, windowWidth, windowHeight);
        //}
        //private void SkipUsed()
        //{
        //    snake.X = 0;
        //    snake.Y = 0;
        //}

        //#endregion

        #endregion
    }
}