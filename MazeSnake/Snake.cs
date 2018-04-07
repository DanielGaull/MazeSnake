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
    public class Snake
    {
        #region Fields & Properties

        Direction direction;

        public Skin Skin;
        //Texture2D image;
        Rectangle drawRectangle;
        Texture2D tongueImage;
        public bool HasTongue = true;
        Texture2D hatImg;
        Rectangle hatRect;
        public bool HasHat = false;
        public Color ShadeColor = Color.DarkGreen;
        CombinableImage img;
        SpriteEffects effect = SpriteEffects.None;

        const int JACKPOT_VALUE = 50;

        Timer secondsTimer;
        Timer speedTimer;
        Timer wallBreakTimer;
        Timer forcefieldTimer;
        Timer frozenTimer;

        Texture2D lightningStrip;
        Rectangle lightningLocationRect;
        const int L_ANIMATION_WIDTH = 150;
        const int L_ANIMATION_HEIGHT = 100;
        const int L_FRAMES_PER_ROW = 5;

        Texture2D forcefieldImg;
        Rectangle forcefieldRect;

        Timer animationTimer;
        int currentFrame = 0;
        Rectangle sourceRect;

        int speed;
        const int REG_SPD = 4;
        const int PW_SPD = 8;

        int windowWidth;
        int windowHeight;
        int minX;
        int minY;
        bool shouldMove = false;

        KeyboardState keyState = Keyboard.GetState();

        ContentManager content;

        public const int REG_WIDTH = 120;
        public const int REG_HEIGHT = 50;
        public const int SMALL_WIDTH = 25;
        public const int SMALL_HEIGHT = 15;

        int tongueSpacing = 5;
        int hatSpacingX = 10;
        int hatSpacingY = 10;

        public Effect SnakeEffect;
        public float EffectTimeRemaining
        {
            get
            {
                switch (SnakeEffect)
                {
                    case Effect.Speed:
                        return (float)(Math.Ceiling(speedTimer.WaitTime) - Math.Floor(speedTimer.TimerFloat));
                    case Effect.WallBreaker:
                        return (float)(Math.Ceiling(wallBreakTimer.WaitTime) - Math.Floor(wallBreakTimer.TimerFloat));
                    case Effect.Forcefield:
                        return (float)(Math.Ceiling(forcefieldTimer.WaitTime) - Math.Floor(forcefieldTimer.TimerFloat));
                    case Effect.Frozen:
                        return (float)(Math.Ceiling(frozenTimer.WaitTime) - Math.Floor(frozenTimer.TimerFloat));
                }
                return 0;
            }
        }

        event Action EnemyCollision;

        List<int> enemiesCollided = new List<int>();

        public Rectangle Rectangle
        {
            set
            {
                drawRectangle = value;
            }
            get
            {
                return drawRectangle;
            }
        }
        public int X
        {
            get
            {
                return drawRectangle.X;
            }
            set
            {
                drawRectangle.X = value;
                //if (HasTongue)
                //{
                //    tongueRect.X = drawRectangle.X + drawRectangle.Width - tongueSpacing;
                //    tongueRect.Y = drawRectangle.Y + (drawRectangle.Height / 2) + tongueSpacing;
                //}
                if (HasHat)
                {
                    SetHatRectValues();
                }
            }
        }
        public int Y
        {
            get
            {
                return drawRectangle.Y;
            }
            set
            {
                drawRectangle.Y = value;
                //if (HasTongue)
                //{
                //    tongueRect.X = drawRectangle.X + drawRectangle.Width - tongueSpacing;
                //    tongueRect.Y = drawRectangle.Y + (drawRectangle.Height / 2) + tongueSpacing;
                //}
                if (HasHat)
                {
                    SetHatRectValues();
                }
            }
        }
        public int Width
        {
            get
            {
                return drawRectangle.Width;
            }
            set
            {
                drawRectangle.Width = value;
                forcefieldRect.Width = forcefieldRect.Height = value;
                //if (HasTongue)
                //{
                //    tongueRect.X = drawRectangle.X + drawRectangle.Width - tongueSpacing;
                //    tongueRect.Y = drawRectangle.Y + (drawRectangle.Height / 2) + tongueSpacing;
                //    tongueRect.Width = drawRectangle.Width / 12;
                //}
                if (HasHat)
                {
                    SetHatRectValues();
                }
            }
        }
        public int Height
        {
            get
            {
                return drawRectangle.Height;
            }
            set
            {
                drawRectangle.Height = value;
                //if (HasTongue)
                //{
                //    tongueRect.X = drawRectangle.X + drawRectangle.Width - tongueSpacing;
                //    tongueRect.Y = drawRectangle.Y + (drawRectangle.Height / 2) + tongueSpacing;
                //    tongueRect.Height = drawRectangle.Height / 5;
                //    tongueSpacing = drawRectangle.Height / 6;
                //}
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a snake for displaying skins
        /// </summary>
        public Snake(Skin skin, int x, int y, ContentManager content, string tongueAsset)
        {
            this.content = content;
            Skin = skin;

            tongueImage = content.Load<Texture2D>(tongueAsset);
            img = new CombinableImage();
            if (skin.HasTongue)
            {
                img.AddImage(tongueImage);
            }
            img.AddImage(content.Load<Texture2D>(skin.SkinAsset));

            ShadeColor = skin.Color.GetColor();
            HasTongue = skin.HasTongue;
            drawRectangle = new Rectangle(x, y, REG_WIDTH, REG_HEIGHT);
            //tongueRect = new Rectangle(0, 0, drawRectangle.Width / 12, drawRectangle.Height / 5);
            tongueSpacing = drawRectangle.Height / 6;

            HasHat = skin.HasHat;
            if (skin.HasHat)
            {
                hatImg = content.Load<Texture2D>(skin.HatAsset);
                hatRect = new Rectangle();
                SetHatRectValues();
            }

            direction = Direction.Up;
        }
        /// <summary>
        /// Creates a snake for playing
        /// </summary>
        public Snake(Skin skin, int windowWidth, int windowHeight, int minX, int minY, int x, int y, ContentManager content, 
            string tongueAsset, string lightningSprite, string forcefieldSprite)
            : this(skin, x, y, content, tongueAsset)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.minX = minX;
            this.minY = minY;
            this.Skin = skin;
            lightningStrip = content.Load<Texture2D>(lightningSprite);
            lightningLocationRect = new Rectangle(x, y, drawRectangle.Width, drawRectangle.Height);
            sourceRect = new Rectangle(0, 0, L_ANIMATION_WIDTH, L_ANIMATION_HEIGHT);

            animationTimer = new Timer(50, TimerUnits.Milliseconds);
            wallBreakTimer = new Timer(2.5f, TimerUnits.Seconds);
            secondsTimer = new Timer(1, TimerUnits.Seconds);
            speedTimer = new Timer(5, TimerUnits.Seconds);
            forcefieldTimer = new Timer(10, TimerUnits.Seconds);
            frozenTimer = new Timer(8, TimerUnits.Seconds);

            forcefieldImg = content.Load<Texture2D>(forcefieldSprite);
            forcefieldRect = new Rectangle(x, y, drawRectangle.Width, drawRectangle.Height);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime, List<PowerUp> powerUps, List<Wall> walls, Random rand, int windowWidth, int windowHeight, ref User player, List<Enemy> enemies)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            
            #region Handle Wall Collisions & Moving

            //if (!shouldMove)
            //{
            //    HandleWallCollisions(direction, walls, ref player);
            //}
            if (shouldMove)
            {
                Move(direction);
            }
            if (!ShouldMove(direction, walls, ref player))
            {
                HandleWallCollisions(direction, walls, ref player);
            }

            #endregion

            #region Updating With Enemies

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Intersects(drawRectangle))
                {
                    if (SnakeEffect == Effect.Forcefield)
                    {
                        if (!enemiesCollided.Contains(enemies[i].Id))
                        {
                            player.EnemiesAvoided++;
                            enemiesCollided.Add(enemies[i].Id);
                        }
                    }
                    else
                    { 
                        EnemyCollision?.Invoke();
                    }
                }
            }

            #endregion

            #region Handling Powerups

            CheckPowerUpPickup(powerUps, rand, walls, windowWidth, windowHeight, ref player);
            secondsTimer.Update(gameTime);

            if (SnakeEffect == Effect.Speed && secondsTimer.QueryWaitTime(gameTime))
            {
                // We've had speed for one second
                player.AddToStat(Stat.SpeedSeconds, 1);
            }

            if (SnakeEffect == Effect.Frozen && secondsTimer.QueryWaitTime(gameTime))
            {
                // We've been frozen for one second
                player.AddToStat(Stat.TimeFrozen, 1);
            }
            if (SnakeEffect == Effect.Frozen)
            {
                frozenTimer.Update(gameTime);
                if (frozenTimer.QueryWaitTime(gameTime))
                {
                    SnakeEffect = Effect.None;
                }
            }

            if (SnakeEffect == Effect.WallBreaker)
            {
                wallBreakTimer.Update(gameTime);
                if (wallBreakTimer.QueryWaitTime(gameTime))
                {
                    SnakeEffect = Effect.None;
                    if (Sound.IsPlaying(Sounds.Buzzing))
                    {
                        Sound.Stop(Sounds.Buzzing);
                    }
                }
                else
                {
                    if (!Sound.IsPlaying(Sounds.Buzzing))
                    {
                        Sound.PlaySound(Sounds.Buzzing);
                    }
                }
            }
            if (SnakeEffect != Effect.WallBreaker && Sound.IsPlaying(Sounds.Buzzing))
            {
                Sound.Stop(Sounds.Buzzing);
            }

            if (SnakeEffect == Effect.Forcefield)
            {
                forcefieldTimer.Update(gameTime);
                if (forcefieldTimer.QueryWaitTime(gameTime))
                {
                    SnakeEffect = Effect.None;
                }

                forcefieldRect.X = drawRectangle.X;
                forcefieldRect.Y = drawRectangle.Y - (forcefieldRect.Height / 2 - drawRectangle.Height / 2);
            }

            speedTimer.Update(gameTime);
            if (SnakeEffect == Effect.Speed && speedTimer.QueryWaitTime(gameTime))
            {
                SnakeEffect = Effect.None;
                speedTimer.Reset();
            }
            if (SnakeEffect == Effect.Speed)
            {
                speed = PW_SPD;
            }
            else
            {
                speed = REG_SPD;
            }

            if (lightningStrip != null)
            {
                animationTimer.Update(gameTime);
                if (SnakeEffect == Effect.WallBreaker && animationTimer.QueryWaitTime(gameTime))
                {
                    if (currentFrame + 1 >= L_FRAMES_PER_ROW)
                    {
                        currentFrame = 0;
                        sourceRect.X = 0;
                    }
                    else
                    {
                        currentFrame++;
                        sourceRect.X += L_ANIMATION_WIDTH;
                    }
                }
                lightningLocationRect.X = drawRectangle.X;
                lightningLocationRect.Y = drawRectangle.Y;
                lightningLocationRect.Width = drawRectangle.Width;
                lightningLocationRect.Height = drawRectangle.Height;
            }

            hatRect.X = drawRectangle.X + (2 * hatSpacingX);
            hatRect.Y = drawRectangle.Y - hatRect.Height;

            #endregion

            #region PC Controls
            keyState = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            shouldMove = false;

            // Check if any control keys are pressed.
            // PC controls
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W) && keyState.IsKeyUp(Keys.LeftControl) && keyState.IsKeyUp(Keys.RightControl)
                 && keyState.IsKeyUp(Keys.LeftAlt) && keyState.IsKeyUp(Keys.RightAlt))
            {
                direction = Direction.Up;
                //shouldMove = ShouldMove(direction, walls, ref player);
                shouldMove = true;
            }
            else if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S) && keyState.IsKeyUp(Keys.LeftAlt) && keyState.IsKeyUp(Keys.RightAlt))
            {
                direction = Direction.Down;
                //shouldMove = ShouldMove(direction, walls, ref player);
                shouldMove = true;
            }
            else if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                //shouldMove = ShouldMove(direction, walls, ref player);
                shouldMove = true;
                effect = SpriteEffects.None;
            }
            else if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                //shouldMove = ShouldMove(direction, walls, ref player);
                shouldMove = true;
                effect = SpriteEffects.FlipHorizontally;
            }
            #endregion

            #region Clamping

            // Clamp the snake so the player cannot navigate it out of the window.
            if (drawRectangle.X <= minX)
            {
                drawRectangle.X = minX;
            }
            if (drawRectangle.X >= windowWidth - drawRectangle.Width)
            {
                drawRectangle.X = windowWidth - drawRectangle.Width;
            }
            if (drawRectangle.Y <= minY)
            {
                drawRectangle.Y = minY;
            }
            if (drawRectangle.Y >= windowHeight - drawRectangle.Height)
            {
                drawRectangle.Y = windowHeight - drawRectangle.Height;
            }

            #endregion

            #region Hat Updating

            if (HasHat)
            {
                SetHatRectValues();
            }

            #endregion
        }

        public void Draw(SpriteBatch spritebatch)
        {
            // Draw the tongue first; that way, positioning doesn't have to be perfect, as it will be covered by the actual snake
            //if (HasTongue)
            //{
            //    spritebatch.Draw(tongueImage, tongueRect, Color.White);
            //}

            //spritebatch.Draw(image, drawRectangle, ShadeColor);
            //spritebatch.Draw(image, drawRectangle, null, ShadeColor, 0.0f, new Vector2(),
            //    SpriteEffects.None, 0.0f);

            List<Color> colors = new List<Color>();
            if (HasTongue)
            {
                colors.Add(Color.White);
            }
            colors.Add(ShadeColor);
            img.Draw(spritebatch, 0.0f, effect, colors, drawRectangle);
            if (lightningStrip != null && SnakeEffect == Effect.WallBreaker)
            {
                spritebatch.Draw(lightningStrip, lightningLocationRect, sourceRect, Color.White);
            }
            if (forcefieldImg != null && SnakeEffect == Effect.Forcefield)
            {
                spritebatch.Draw(forcefieldImg, forcefieldRect, Color.White);
            }
            if (HasHat)
            {
                spritebatch.Draw(hatImg, hatRect, null, Color.White, 0.0f, new Vector2(), effect, 1.0f);
            }
        }

        public void ChangeRectangle(Rectangle newRectangle)
        {
            drawRectangle = newRectangle;
        }

        public void AddEnemyCollisionHandler(Action handler)
        {
            EnemyCollision += handler;
        }

        public void Reset()
        {
            drawRectangle.X = 0;
            drawRectangle.Y = 0;
            speed = REG_SPD;
            SnakeEffect = Effect.None;
            effect = SpriteEffects.None;
            enemiesCollided.Clear();
            if (Sound.IsPlaying(Sounds.Buzzing))
            {
                Sound.Stop(Sounds.Buzzing);
            }
        }

        public void ChangeSkin(Skin newSkin)
        {
            Skin = newSkin;
            HasTongue = newSkin.HasTongue;
            ShadeColor = newSkin.Color.GetColor();
            HasHat = newSkin.HasHat;
            if (HasHat)
            {
                hatSpacingX = drawRectangle.Width / 3;
                hatSpacingY = drawRectangle.Height / 4;
                hatImg = content.Load<Texture2D>(newSkin.HatAsset);
                hatRect = new Rectangle(drawRectangle.X + (2 * hatSpacingX), 0, drawRectangle.Width / 3,
                    drawRectangle.Width / 3);
                hatRect.Y = (drawRectangle.Y - hatRect.Height) + hatSpacingY;
            }
            img.RemoveAll();
            if (newSkin.HasTongue)
            {
                img.AddImage(tongueImage);
            }
            img.AddImage(content.Load<Texture2D>(newSkin.SkinAsset));
        }

        public void Teleport(Random rand, int windowWidth, int windowHeight)
        {
            drawRectangle.X = rand.Next(0, windowWidth);
            drawRectangle.Y = rand.Next(0, windowHeight);
        }

        #endregion

        #region Private Methods

        private void SetHatRectValues()
        {
            hatSpacingX = drawRectangle.Width / 3;
            hatSpacingY = drawRectangle.Height / 4;
            hatRect.X = drawRectangle.X + (2 * hatSpacingX);
            hatRect.Y = (drawRectangle.Y - hatRect.Height) + hatSpacingY;
            hatRect.Width = drawRectangle.Width / 3;
            hatRect.Height = drawRectangle.Width / 3;

            if (effect == SpriteEffects.FlipHorizontally)
            {
                int distance = hatRect.X - (drawRectangle.X + (drawRectangle.Width / 2));
                hatRect.X -= distance + hatRect.Width + (hatSpacingX / 2);
            }
        }

        private void CheckPowerUpPickup(List<PowerUp> powerUps, Random rand, List<Wall> walls, int windowWidth, int windowHeight, 
            ref User user)
        {
            foreach (PowerUp powerUp in powerUps)
            {
                if (powerUp.IsIntersecting(drawRectangle) && (!powerUp.IsPickedUp))
                {
                    powerUp.Collect(windowWidth, windowHeight);
                    user.AddToStat(Stat.PowerupsCollected, 1);
                    switch (powerUp.Type)
                    {
                        case PowerupType.Speed:
                            SnakeEffect = Effect.Speed;
                            speedTimer.Reset();
                            Sound.PlaySound(Sounds.Speed);
                            break;
                        case PowerupType.Teleport:
                            Teleport(rand, windowWidth, windowHeight);
                            foreach (Wall wall in walls)
                            {
                                if (drawRectangle.Intersects(wall.GetRectangle))
                                {
                                    Teleport(rand, windowWidth, windowHeight);
                                }
                            }
                            Sound.PlaySound(Sounds.Teleport);
                            user.AddToStat(Stat.TimesTeleported, 1);
                            break;
                        case PowerupType.WallBreaker:
                            SnakeEffect = Effect.WallBreaker;
                            wallBreakTimer.Reset();
                            Sound.PlaySound(Sounds.WallBreaker);
                            break;
                        case PowerupType.Jackpot:
                            user.AddCoins(JACKPOT_VALUE);
                            Sound.PlaySound(Sounds.Coin);
                            break;
                        case PowerupType.Forcefield:
                            SnakeEffect = Effect.Forcefield;
                            forcefieldTimer.Reset();
                            Sound.PlaySound(Sounds.Forcefield);
                            break;
                        case PowerupType.Frozen:
                            SnakeEffect = Effect.Frozen;
                            Sound.PlaySound(Sounds.Freeze);
                            frozenTimer.Reset();
                            break;
                    }
                }
            }
        }

        private bool ShouldMove(Direction dir, List<Wall> walls, ref User user)
        {
            foreach (Wall wall in walls)
            {
                if (HasHat)
                {
                    #region Check with Hat

                    if (((drawRectangle.Top < wall.GetRectangle.Bottom && drawRectangle.Bottom > wall.GetRectangle.Top &&
                        dir == Direction.Up && drawRectangle.Intersects(wall.GetRectangle))/*Checks if the snake intersects the wall*/ ||
                        (hatRect.Top < wall.GetRectangle.Bottom && hatRect.Bottom > wall.GetRectangle.Top && dir == Direction.Up &&
                        hatRect.Intersects(wall.GetRectangle))/*Checks if the hat intersects with the wall*/)
                        && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go up into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (((drawRectangle.Bottom > wall.GetRectangle.Top && drawRectangle.Top < wall.GetRectangle.Bottom &&
                        dir == Direction.Down && drawRectangle.Intersects(wall.GetRectangle)) /*Checks if snake intersects with wall*/ ||
                        (hatRect.Bottom > wall.GetRectangle.Top && hatRect.Top < wall.GetRectangle.Bottom && dir == Direction.Down &&
                        hatRect.Intersects(wall.GetRectangle))) && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go down into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (((drawRectangle.Right > wall.GetRectangle.Left && drawRectangle.Left < wall.GetRectangle.Right &&
                        dir == Direction.Right && drawRectangle.Intersects(wall.GetRectangle)) || hatRect.Right > wall.GetRectangle.Left &&
                        hatRect.Left < wall.GetRectangle.Right && dir == Direction.Right && hatRect.Intersects(wall.GetRectangle))
                        && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go right into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (((drawRectangle.Left < wall.GetRectangle.Right && drawRectangle.Right > wall.GetRectangle.Left &&
                        dir == Direction.Left && drawRectangle.Intersects(wall.GetRectangle)) || (hatRect.Left < wall.GetRectangle.Right &&
                        hatRect.Right > wall.GetRectangle.Left && dir == Direction.Left && hatRect.Intersects(wall.GetRectangle)))
                        && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go left into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }

                    #endregion
                }
                else
                {
                    #region Check without Hat

                    if ((drawRectangle.Top < wall.GetRectangle.Bottom && drawRectangle.Bottom > wall.GetRectangle.Top &&
                        dir == Direction.Up && drawRectangle.Intersects(wall.GetRectangle))/*Checks if the drawRectangle intersects the wall*/
                        && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go up into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (drawRectangle.Bottom > wall.GetRectangle.Top && drawRectangle.Top < wall.GetRectangle.Bottom &&
                        dir == Direction.Down && drawRectangle.Intersects(wall.GetRectangle) && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go down into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (drawRectangle.Right > wall.GetRectangle.Left && drawRectangle.Left < wall.GetRectangle.Right &&
                        dir == Direction.Right && drawRectangle.Intersects(wall.GetRectangle) && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go right into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }
                    else if (drawRectangle.Left < wall.GetRectangle.Right && drawRectangle.Right > wall.GetRectangle.Left &&
                        dir == Direction.Left && drawRectangle.Intersects(wall.GetRectangle) && !wall.Exploded && !wall.Exploding)
                    {
                        // Trying to go left into wall
                        if (SnakeEffect == Effect.WallBreaker)
                        {
                            wall.Explode();
                            user.AddToStat(Stat.WallsBroken, 1);
                        }

                        return false;
                    }

                    #endregion
                }
            }
            return true;
        }

        private void HandleWallCollisions(Direction dir, List<Wall> walls, ref User player)
        {
            switch (dir)
            {
                case Direction.Up:
                    while (!ShouldMove(dir, walls, ref player))
                    {
                        Move(Direction.Down);
                    }
                    break;
                case Direction.Down:
                    while (!ShouldMove(dir, walls, ref player))
                    {
                        Move(Direction.Up);
                    }
                    break;
                case Direction.Left:
                    while (!ShouldMove(dir, walls, ref player))
                    {
                        Move(Direction.Right);
                    }
                    break;
                case Direction.Right:
                    while (!ShouldMove(dir, walls, ref player))
                    {
                        Move(Direction.Left);
                    }
                    break;
            }

        }

        private void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    drawRectangle.Y -= speed;
                    hatRect.Y -= speed;
                    break;
                case Direction.Down:
                    drawRectangle.Y += speed;
                    hatRect.Y += speed;
                    break;
                case Direction.Left:
                    drawRectangle.X -= speed;
                    hatRect.X -= speed;
                    break;
                case Direction.Right:
                    drawRectangle.X += speed;
                    hatRect.X += speed;
                    break;
            }
        }

        #endregion
    }

    #region Enumerations

    public enum Effect
    {
        None,
        Speed,
        WallBreaker,
        Forcefield,
        Frozen,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    #endregion

}