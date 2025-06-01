// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: May 29th 2025
// Description: Main driver class for the game

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Storing screen dimensions
    public static int screenWidth;
    private int screenHeight;
    
    // Storing mouse input
    private MouseState mouse;
    private MouseState prevMouse;
    
    // Storing a single pixel texture to draw a simple solid color
    private Texture2D pixelImg;

    #region Game State Variables
    // Storing constant variable constants for game states
    private const byte MENU = 0;
    private const byte LEVEL_1 = 1;
    private const byte LEVEL_2 = 2;
    private const byte PAUSE = 3;
    private const byte SETTINGS = 4;
    private const byte TUTORIAL = 5;
    private const byte ENDGAME = 6;
    
    // Storing current game state
    private byte gameState = MENU;

    #endregion

    #region UI Variables

    // Storing sprite Fonts
    private SpriteFont titleFont;
    private SpriteFont HUDFont;
    private SpriteFont smallFont;
    
    // Storing offset constant for drop shadow texts
    private readonly Vector2 DROP_SHADOW = new Vector2(4, 4);
    
    // Storing background image and rec for menu
    private Texture2D bgImg;
    private Rectangle bgRec;
    
    // Storing all button images
    private Texture2D level1Img;
    private Texture2D level2Img;
    private Texture2D settingsImg;
    private Texture2D tutorialBtnImg;
    
    // Storing all buttons
    private Button level1Button;
    private Button level2Button;
    private Button settingsButton;
    private Button tutorialButton;
    
    // Storing title image and rectangle
    private Texture2D titleImg;
    Rectangle titleRec;
    
    #endregion

    #region Gameplay Variables

    // Storing time passed
    private float timePassed;
    
    // Global constant gravity
    public const float GRAVITY = -900f;
    
    // Storing game background image and rectangle
    private Texture2D gameBgImg;
    private Rectangle gameBgRec;
    
    // Storing platform
    private Platform platform;
    
    // Storing buildable rectangle area
    private Rectangle buildableRec;
    
    // Storing king tower
    private KingTower kingTower;
    
    // Storing the two king tower locaations for each level
    private Vector2 lvl1KingPos;
    private Vector2 lvl2KingPos;
    
    // Storing zombies array
    private Zombie[] zombies = new Zombie[5];
    
    // Storing timer for day night cycle
    private Timer dayNightCycle = new Timer(200000, true);
    
    // Storing background night sky texture and rectangle
    private Texture2D nightBGImg;
    private Rectangle nightBGRec;
    
    // Storing night sky color multiplier, incrase and decrease constants, and multiplier modifier
    private float skyOpacity = 1f;
    private const int POSITIVE = 1;
    private const int NEGATIVE = -1;
    private int skyMultiplier;
    
    // Storing explosion animation
    private Animation[] explosionAnims;
    
    // Storing new array of walls
    private Wall[] walls = new Wall[4];
    
    // Width and height for the tower preview buttons
    private const int PREVIEW_SIZE = 80;
    
    // Level 1 wall preview will act as a preview to select and place a wall and it will act as a button
    private Button wallLvl1Prev;
    private Button wallLvl2Prev;
    
    // Storing red cross texture to cancel placement
    private Texture2D redCrossImg;

    #region Sub Region - HUD Variables

    // Storing days survived, string for display, and position vector
    private int dayCount = 0;
    private string dispDayCount = "Days: 0";
    private Vector2 dayCountPos;
    
    // Storing zombies killed, string for display, and position vector
    private static int mobsKilled = 0; // Setting to public so that zombie class will be able to modify it
    private static string dispMobsKilled = "Kills: 0";
    private Vector2 mobsKilledPos;
    
    // Storing king hp string display, and position vector
    private string dispKingHP = "HP: 1000";
    private Vector2 kingHPPos;
    
    // Storing coins, each zombie kill will score a few points to buy more upgrades
    private static int coins = 200;
    private string dispCoins = $"${coins}";
    private Vector2 coinsPos;

    #endregion

    #region Sub Region - Projectiles

    // Storing max number of cannonballs
    private Cannonball[] cannonballs = new Cannonball[4];

    #endregion

    #endregion
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    #region Monogame methods
    protected override void Initialize()
    {
        // Attempting to set screen dimensions to 1600x1000
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 1000;
        
        // Applying the screen dimensions changes
        _graphics.ApplyChanges();

        // Storing the resultant dimensions to determine the drawable space
        screenWidth = _graphics.GraphicsDevice.Viewport.Width;
        screenHeight = _graphics.GraphicsDevice.Viewport.Height;

        base.Initialize();
    }
    
    protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
    
            // Loading sprite fonts
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            HUDFont = Content.Load<SpriteFont>("Fonts/HUDFont");
            smallFont = Content.Load<SpriteFont>("Fonts/SmallFont");
            
            // Loading night background texture and rectangle
            nightBGImg = Content.Load<Texture2D>("Images/Backgrounds/PixelNightSky");
            nightBGRec = new Rectangle(0, 0, nightBGImg.Width * 2, nightBGImg.Height * 2);
            
            // Loading background image and rectangle
            bgImg = Content.Load<Texture2D>("Images/Backgrounds/MenuBackground");
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);
            
            // Loading game background
            gameBgImg = Content.Load<Texture2D>("Images/Backgrounds/GameBackground");
            gameBgRec = new Rectangle(0, 400, screenWidth, screenHeight);
            
            // Loading single pixel texture
            pixelImg = new Texture2D(GraphicsDevice, 1, 1);
            pixelImg.SetData(new[] { Color.White });
            
            // Loading platform and its texture (defining texture locally as it will be used in the Platform class)
            Texture2D platformImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Brick");
            platform = new Platform(platformImg, screenWidth, screenHeight);
            
            // Loading king tower, position, & image (defining king tower image locally as it will be used in the Tower class)
            Texture2D kingTowerImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/KingTower");
            Texture2D cannonballImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Cannonball");
            
            kingTower = new KingTower(kingTowerImg, nightBGRec.Location.ToVector2(), kingTowerImg.Width, 
                                    kingTowerImg.Height, 266, 330, 1000, cannonballImg, 4, 750);
            
            lvl1KingPos = new Vector2(screenWidth - kingTower.GetDisplayRec().Width / 2f, 
                                        platform.GetRec().Y - kingTower.GetHitbox().Height + 10);
            lvl2KingPos = new Vector2(WidthCenter(kingTower.GetHitbox().Width),
                                        platform.GetRec().Y - kingTower.GetHitbox().Height + 10);
    
            // Loading button images
            level1Img = Content.Load<Texture2D>("Images/Sprites/UI/Level1Button");
            level2Img = Content.Load<Texture2D>("Images/Sprites/UI/Level2Button");
            settingsImg = Content.Load<Texture2D>("Images/Sprites/UI/SettingsButton");
            tutorialBtnImg = Content.Load<Texture2D>("Images/Sprites/UI/TutorialButton");
            
            // Loading all buttons
            level1Button = new Button(level1Img, (int)WidthCenter(level1Img.Width) - 400, screenHeight - 300, 
                                        level1Img.Width, level1Img.Height, Level1Button);
            level2Button = new Button(level2Img, (int)WidthCenter(level1Img.Width) + 400, screenHeight - 300, 
                                        level2Img.Width, level2Img.Height, Level2Button);
            settingsButton = new Button(settingsImg, 50, 50, settingsImg.Width / 5, settingsImg.Height / 5, 
                                () => gameState = SETTINGS);
            tutorialButton = new Button(tutorialBtnImg, screenWidth - 50 - tutorialBtnImg.Width / 5, 50, 
                tutorialBtnImg.Width / 5, tutorialBtnImg.Height / 5, () => gameState = TUTORIAL);
            
            // Loading title image and rectangle
            titleImg = Content.Load<Texture2D>("Images/Sprites/UI/Title");
            titleRec = new Rectangle((int)WidthCenter(titleImg.Width * 0.75f), 10, 
                                    (int)(titleImg.Width * 0.75), (int)(titleImg.Height * 0.75));
            
            // Loading local zombie textures locally
            Texture2D[] zombieImgs = new Texture2D[5];
            zombieImgs[0] = Content.Load<Texture2D>("Images/Sprites/Gameplay/WildZombie/Walk");
            zombieImgs[1] = Content.Load<Texture2D>("Images/Sprites/Gameplay/WildZombie/Dead");
            zombieImgs[2] = Content.Load<Texture2D>("Images/Sprites/Gameplay/WildZombie/Attack_1");
            zombieImgs[3] = Content.Load<Texture2D>("Images/Sprites/Gameplay/WildZombie/Attack_2");
            zombieImgs[4] = Content.Load<Texture2D>("Images/Sprites/Gameplay/WildZombie/Attack_3");
            
            // Loading all the zombies
            for (int i = 0; i < zombies.Length; i++)
            {
                zombies[i] = new Zombie(zombieImgs, screenWidth, platform.GetRec().Y);
            }
            
            // Loading positions of HUD objects
            dayCountPos = new Vector2(5, 5);
            mobsKilledPos = new Vector2(5, dayCountPos.Y + HUDFont.MeasureString(dispDayCount).Y);
            kingHPPos = new Vector2(WidthCenter(HUDFont.MeasureString(dispKingHP).X), 5);
            coinsPos = new Vector2(WidthCenter(HUDFont.MeasureString(dispCoins).X),
                5 + HUDFont.MeasureString(dispKingHP).Y);
            
            // Loading explosion animation and saving local image for each cannonball
            Texture2D explosionImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/explosion");
            explosionAnims = new Animation[cannonballs.Length];
            for (int i = 0; i < explosionAnims.Length; i++)
            {
                explosionAnims[i] = new Animation(explosionImg, 5, 5, 23, 0, -1, 1, 1000, new Vector2(800, 500), false);
            }
            
            // Loading wall textures for buttons
            Texture2D lvl1Wall = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl1");
            Texture2D lvl2Wall = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl2");
            
            // Loading wall preview buttons using wall textures
            wallLvl1Prev = new Button(lvl1Wall, screenWidth - PREVIEW_SIZE - 5, 5, 
                PREVIEW_SIZE, PREVIEW_SIZE, Lvl1WallPrev);
            wallLvl2Prev = new Button(lvl2Wall, wallLvl1Prev.GetRec().X - 5 - PREVIEW_SIZE, 5,
                PREVIEW_SIZE, PREVIEW_SIZE, Lvl2WallPrev);
            
            // Loading red cross texture
            redCrossImg = Content.Load<Texture2D>("Images/Sprites/UI/RedCross");
                
            // Loading buildable rectangle to be on the floor as a preview of where you can build
            buildableRec = new Rectangle((int)WidthCenter(800), platform.GetRec().Y, 800, platform.GetRec().Height);
        }
    
    protected override void Update(GameTime gameTime)
        {
            // Updating mouse input
            prevMouse = mouse;
            mouse = Mouse.GetState();
            
            // Storing time passed
            timePassed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            switch (gameState)
            {
                case MENU:
                    // Checking for button clicks
                    level1Button.Update(mouse, prevMouse);
                    level2Button.Update(mouse, prevMouse);
                    settingsButton.Update(mouse, prevMouse);
                    tutorialButton.Update(mouse, prevMouse);
                    
                    break;
                
                case LEVEL_1:
                    // Updating game
                    UpdateGame(gameTime);
                    
                    break;
                
                case LEVEL_2:
                    // Updating game
                    UpdateGame(gameTime);
                    
                    break;
                
                case PAUSE:
                    
                    break;
                
                case SETTINGS:
                    
                    break;
                
                case TUTORIAL:
                    
                    break;
                
                case ENDGAME:
                    
                    break;
            }
    
            base.Update(gameTime);
        }
    
    protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.RoyalBlue);
    
            // Initializing sprite drawing batch
            _spriteBatch.Begin();
            
            // Drawing game based on game state
            switch (gameState)
            {
                case MENU:
                    // Drawing background
                    _spriteBatch.Draw(bgImg, bgRec, Color.White);
                    
                    // Draw title
                    _spriteBatch.Draw(titleImg, titleRec, Color.White);
                    
                    // Drawing buttons
                    level1Button.Draw(_spriteBatch);
                    level2Button.Draw(_spriteBatch);
                    settingsButton.Draw(_spriteBatch);
                    tutorialButton.Draw(_spriteBatch);
                    
                    break;
                
                case LEVEL_1:
                    // Drawing game
                    DrawGame();
                    break;
                
                case LEVEL_2:
                    // Drawing game
                    DrawGame();
                    break;
                
                case PAUSE:
                    // Drawing background
                    _spriteBatch.Draw(bgImg, bgRec, Color.White);
                    
                    break;
                
                case SETTINGS:
                    // Drawing background
                    _spriteBatch.Draw(bgImg, bgRec, Color.White);
                    
                    break;
                
                case TUTORIAL:
                    
                    break;
                
                case ENDGAME:
                    // Drawing background
                    _spriteBatch.Draw(bgImg, bgRec, Color.White);
                    
                    break;
            }
            
            // Finish sprite batch
            _spriteBatch.End();
    
            base.Draw(gameTime);
        }

    #endregion

    // Storing coins as a public property of Game1
    public static int Coins
    {
        get => coins;
        set => coins = value;
    }
    
    #region Centers & Drop Shadow

    // The function uses screen width and text/image width to center the image horizontally
    private float WidthCenter(float spriteWidth)
    {
        float center;

        // Calculating center by averaging the 2 widths
        center = (screenWidth - spriteWidth) / 2f;

        return center;
    }
    
    // The function uses screen height and text/image height to center the image vertically
    private float HeightCenter(float spriteHeight)
    {
        float center;

        // Calculating center by averaging the 2 heights
        center = (screenHeight - spriteHeight) / 2f;

        return center;
    }
    
    // Drawing text with drop shadow
    private void DrawWithShadow(SpriteFont font, string text, Vector2 pos, Color color, Color shadowColor)
    {
        _spriteBatch.DrawString(font, text, pos + DROP_SHADOW, shadowColor);
        _spriteBatch.DrawString(font, text, pos, color);
    }

    #endregion
    
    #region Game

    // Updating everything that is common to both levels
    private void UpdateGame(GameTime gameTime)
    {
        // Updating king tower and cannonballs
        kingTower.Update(mouse, gameTime);
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Update(timePassed);
            }
        }
        
        // Updating walls and checking for placement and collisions
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                // If this statement returns true it means the wall was destroyed
                if (walls[i].Update(mouse, buildableRec, ValidPlacement(walls[i].GetRec())))
                {
                    walls[i] = null;
                }
            }
        }
        
        // Updating tower preview buttons AFTER wall check so the wall won't automatically be placed
        wallLvl1Prev.Update(mouse, prevMouse);
        wallLvl2Prev.Update(mouse, prevMouse);
        
        // Checking for placement
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                walls[i].CheckPlacement(mouse, prevMouse, platform.GetRec());
            }
        }
        
        // Updating zombie
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Update(gameTime, timePassed * 1000);
        }
        
        // Updating explosion animations
        for (int i = 0; i < explosionAnims.Length; i++)
        {
            explosionAnims[i].Update(gameTime);
        }

        // Checking for clicks to launch cannons and collision
        LaunchCannon();
        CannonCollision();

        // Casting night sky every day night cycle
        DayNightCycle(gameTime);

        // Checking for zombie collisions
        ZombieCollision();
        
        // Updating coins display and centering it
        dispCoins = $"${coins}";
        coinsPos.X = WidthCenter(HUDFont.MeasureString(dispCoins).X);
        
        // SPAWNING ZOMBIES FOR BETA TESTING
        if (Keyboard.GetState().IsKeyDown(Keys.K))
        {
            // Checking for empty slot
            for (int i = 0; i < zombies.Length; i++)
            {
                zombies[i].Spawn(gameState);
            }
        }
    }

    // Drawing everything in both levels
    private void DrawGame()
    {
        // Drawing night sky overlay
        _spriteBatch.Draw(nightBGImg, nightBGRec, Color.White * skyOpacity);
        
        // Drawing background
        _spriteBatch.Draw(gameBgImg, gameBgRec, Color.White);
        
        // Drawing platform
        platform.Draw(_spriteBatch);
                
        // Drawing king tower
        kingTower.Draw(_spriteBatch);
        
        // Drawing zombies
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Draw(_spriteBatch);
        }
        
        // Drawing walls
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                walls[i].Draw(_spriteBatch);
            }
        }

        // Drawing cannon balls with their explosions
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Draw(_spriteBatch);
            }
            
            explosionAnims[i].Draw(_spriteBatch, Color.White);
        }
        
        // Drawing HUD
        DrawHUD();
        
        // Drawing buildable area rectangle as just a single solid color
        _spriteBatch.Draw(pixelImg, buildableRec, Color.Green * 0.5f);
    }

    #endregion

    public static void IncreaseMobsKilled()
    {
        mobsKilled++;
        dispMobsKilled = $"Kills: {mobsKilled}";
    }
        
    private void DayNightCycle(GameTime gameTime)
    {
        // Updating day night cycle timer
        dayNightCycle.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        
        // Checking if the cycle is over
        if (dayNightCycle.IsFinished())
        {
            // Flipping the sky multiplier to either turn the sky on or off
            if (skyOpacity == 0)
            {
                skyMultiplier = POSITIVE;
            }
            else if (skyOpacity == 1)
            {
                skyMultiplier = NEGATIVE;
                
                // Killing zombies
                for (int i = 0; i < zombies.Length; i++)
                {
                    zombies[i].KillZombie();
                }
                
                // Adding 1 to day count
                dayCount++;
                dispDayCount = $"Days: {dayCount}";
            }
            
            // Restarting the timer
            dayNightCycle.ResetTimer(true);
        }

        // Updating the sky opacity and clamping it
        skyOpacity += skyMultiplier * 0.01f;
        skyOpacity = MathHelper.Clamp(skyOpacity, 0, 1);
        
    }
    
    private void ZombieCollision()
    {
        // Checking collision with each zombie
        for (int i = 0; i < zombies.Length; i++)
        {
            // Storing a bool to keep track if the zombie is attacking or not
            bool isAttacking = false;
        
            // Checking for king tower
            if (zombies[i].GetRec().Intersects(kingTower.GetHitbox()))
            {
                // Dealing damage to king tower
                kingTower.HP = zombies[i].Attack(kingTower.HP);
                
                // Updating king tower HUD:
                dispKingHP = $"HP: {kingTower.HP}";
                
                // Setting attacking to true
                isAttacking = true;
            }

            // Checking for walls
            for (int j = 0; j < walls.Length; j++)
            {
                // Making sure wall isn't null and it is currently placed
                if (walls[j] != null && walls[j].IsPlaced())
                {
                    if (zombies[i].GetRec().Intersects(walls[j].GetRec()))
                    {
                        // Dealing damage to wall
                        walls[j].HP = zombies[i].Attack(walls[j].HP);
                        
                        // Setting attacking to true
                        isAttacking = true;
                    }
                }
            }
            
            // if the zombie isn't attacking, set it to walk
            if (!isAttacking)
            {
                zombies[i].Walk();
            }
        }
    }
    
    private void CannonCollision()
    {
        // Checking collision for each cannon ball
        for (int i = 0; i < cannonballs.Length; i++)
        {
            // Making sure its not null
            if (cannonballs[i] != null)
            {
                // Detecting collision with platform
                if (cannonballs[i].GetHitbox().Bottom >= platform.GetRec().Y)
                {
                    // Checking if any zombies were hit
                    for (int j = 0; j < zombies.Length; j++)
                    {
                        if (zombies[j].GetRec().Intersects(cannonballs[i].GetHitbox()))
                        {
                            // Dealing damage to zombie
                            zombies[j].HP -= cannonballs[i].GetDamage();
                        }
                    } 
                    // Playing explosion animation
                    explosionAnims[i].TranslateTo(cannonballs[i].GetRec().X - (explosionAnims[i].GetDestRec().Width - 
                                                                               cannonballs[i].GetRec().Width) / 2, 
                                                cannonballs[i].GetRec().Y - (explosionAnims[i].GetDestRec().Height - 
                                                                             cannonballs[i].GetRec().Height) / 2);
                    explosionAnims[i].Activate(true);
                    
                    // Setting to null so it can be reused
                    cannonballs[i] = null;
                }
            }
        }
    }
    
    private void LaunchCannon()
    {
        // Checking for king tower shots only at night time
        if (skyOpacity == 1)
        {
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                // Storing a new cannonball if the current one is null
                for (int i = 0; i < cannonballs.Length; i++)
                {
                    if (cannonballs[i] == null)
                    {
                        cannonballs[i] = kingTower.LaunchBall(mouse.Position.ToVector2());

                        break;
                    }
                }
            }
        }
    }

    private bool ValidPlacement(Rectangle building)
    {
        if (building.Intersects(kingTower.GetHitbox()))
        {
            return false;
        }

        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                if (building.Intersects(walls[i].GetRec()) && walls[i].IsPlaced())
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private void DrawHUD()
    {
        // Drawing day count, kill count, health with offset, and coins
        DrawWithShadow(HUDFont, dispDayCount, dayCountPos, Color.Yellow, Color.Black);
        DrawWithShadow(HUDFont, dispMobsKilled, mobsKilledPos, Color.IndianRed, Color.Black);
        DrawWithShadow(HUDFont, dispKingHP, kingHPPos, Color.Green, Color.Black);
        DrawWithShadow(HUDFont, dispCoins, coinsPos, Color.Gold, Color.DarkGoldenrod);
        
        // Drawing tower preview buttons and cancel options
        DrawWithPrice(wallLvl1Prev, 100);
        DrawWithPrice(wallLvl2Prev, 200);
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null && !walls[i].IsPlaced() && walls[i].GetLvl() == 1)
            {
                _spriteBatch.Draw(redCrossImg, wallLvl1Prev.GetRec(), Color.White);
            }
            else if (walls[i] != null && !walls[i].IsPlaced() && walls[i].GetLvl() == 2)
            {
                _spriteBatch.Draw(redCrossImg, wallLvl2Prev.GetRec(), Color.White);
            }
        }
    }
    
    // Meant for drawing preview buttons with prices on them
    private void DrawWithPrice(Button button, int price)
    {
        button.Draw(_spriteBatch);
        _spriteBatch.DrawString(smallFont, $"${price}", button.GetRec().Location.ToVector2(), Color. Gold);
    }

    #region Button Actions

    // Action for level 1 button click
    public void Level1Button()
    {
        // Setting game state to level 1
        gameState = LEVEL_1;
        
        // Updating king position
        kingTower.TranslateTo(lvl1KingPos);
    }

    // Action for level 2 button click
    public void Level2Button()
    {
        // Setting game state to level 2
        gameState = LEVEL_2;
        
        // Updating king position
        kingTower.TranslateTo(lvl2KingPos);
        
        // TESTING
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Spawn(LEVEL_2);
        }
    }

    public bool PreviewCheck()
    {
        // Checking if a wall is currently being placed
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                // If ANY wall is in preview, not valid
                if (!walls[i].IsPlaced())
                {
                    return false;
                }
            }
        }

        // TODO: CHECK FOR TOWERS AS WELL
        
        // If it passed all of them it must be valid
        return true;
    }

    public void Lvl1WallPrev()
    {
        // CHECK FOR COINS WHEN MECHANIC ADDED
        
        // Checking if ANY wall is currently being placed
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null && !walls[i].IsPlaced())
            {
                // Setting the wall to null and ending method
                walls[i] = null;
                return;
            }
        }
        
        // Checking for an empty slot in null array and if there is enough coins to buy
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] == null && PreviewCheck())
            {
                // Creating a new wall instance to be placed
                Texture2D wallImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl1");
                walls[i] = new Wall(wallImg, wallImg.Width / 2, wallImg.Height / 2, platform, 1);
                break;
            }
        }
    }

    public void Lvl2WallPrev()
    {
        // CHECK FOR COINS WHEN MECHANIC ADDED
        
        // Checking if ANY wall is currently being placed
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null && !walls[i].IsPlaced())
            {
                // Setting the wall to null and ending method
                walls[i] = null;
                return;
            }
        }
        // Checking for an empty slot in null array
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] == null && PreviewCheck())
            {
                // Creating a new wall instance to be placed
                Texture2D wallImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl2");
                walls[i] = new Wall(wallImg, wallImg.Width / 2, wallImg.Height / 2, platform, 2);
                break;
            }
        }
    }

    #endregion
    
}