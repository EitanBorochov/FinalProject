// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: June 5th 2025
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
    
    // Storing random number generator
    public static Random rng = new Random();

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
    private Wall[] walls = new Wall[10];
    
    // Storing an array of archer towers
    private ArcherTower[] archers = new ArcherTower[10];
    
    

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
    private static int coins = 100000;
    private string dispCoins = $"${coins}";
    private Vector2 coinsPos;
    
    // Width and height for the tower preview buttons
    private const int PREVIEW_SIZE = 80;
    
    // Storing buttons to purchase walls
    private Button[] wallPrevs = new Button[2];
    
    // Storing buttons to purchase archer towers
    private Button[] archerPrevs = new Button[3];
    
    // Storing button to demolish building
    private Button demolishBuilding; 
    
    // Storing demolishing bool (if currently in demolishing or not)
    private bool demolishing;
    
    // Storing red cross texture to cancel placement
    private Texture2D redCrossImg;
    
    #endregion

    #region Sub Region - Projectiles

    // Storing max number of cannonballs inside array size
    private Cannonball[] cannonballs = new Cannonball[4];
    
    // Storing max number of arrows inside array size
    private Arrow[] arrows = new Arrow[20];

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
                                kingTowerImg.Height, 266, 330, cannonballImg, 1000);
        
        lvl1KingPos = new Vector2(screenWidth - kingTower.DisplayRec.Width / 2f + 30, 
                                    platform.Rec.Y - kingTower.Hitbox.Height + 10);
        lvl2KingPos = new Vector2(WidthCenter(kingTower.Hitbox.Width),
                                    platform.Rec.Y - kingTower.Hitbox.Height + 10);

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
        settingsButton = new Button(settingsImg, 50, 50, settingsImg.Width / 5f, settingsImg.Height / 5f, 
                            () => gameState = SETTINGS);
        tutorialButton = new Button(tutorialBtnImg, screenWidth - 50 - tutorialBtnImg.Width / 5f, 50, 
            tutorialBtnImg.Width / 5f, tutorialBtnImg.Height / 5f, () => gameState = TUTORIAL);
        
        // Loading title image and rectangle
        titleImg = Content.Load<Texture2D>("Images/Sprites/UI/Title");
        titleRec = new Rectangle((int)WidthCenter(titleImg.Width * 0.75f), 10, 
                                (int)(titleImg.Width * 0.75), (int)(titleImg.Height * 0.75));
        
        // Loading local zombie textures locally
        Texture2D[][] zombieImgs = new Texture2D[3][];
        
        // Loading all variants
        for (int i = 0; i < zombieImgs.Length; i++)
        {
            zombieImgs[i] = new Texture2D[5];
            zombieImgs[i][0] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Zombie{i+1}/Walk");
            zombieImgs[i][1] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Zombie{i+1}/Dead"); 
            zombieImgs[i][2] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Zombie{i+1}/Attack_1");
            zombieImgs[i][3] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Zombie{i+1}/Attack_2");
            zombieImgs[i][4] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Zombie{i+1}/Attack_3");
        }
        
        // Loading all the zombies
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i] = new Zombie(zombieImgs[rng.Next(0, 3)], screenWidth, platform.Rec.Y);
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
        wallPrevs[0] = new Button(lvl1Wall, screenWidth - PREVIEW_SIZE - 5, 5, 
            PREVIEW_SIZE, PREVIEW_SIZE, () => WallPrev(0));
        wallPrevs[1] = new Button(lvl2Wall, wallPrevs[0].Rec.X - 5 - PREVIEW_SIZE, 5,
            PREVIEW_SIZE, PREVIEW_SIZE, () => WallPrev(1));
        
        // Loading archer tower textures
        Texture2D lvl1ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl1");
        Texture2D lvl2ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl2");
        Texture2D lvl3ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl3");
        
        // Loading archer tower preview buttons (couldn't do a loop because of the x offset)
        archerPrevs[0] = new Button(lvl1ArcherImg, wallPrevs[1].Rec.X - 5 - PREVIEW_SIZE, 5,
            PREVIEW_SIZE * ((float)lvl1ArcherImg.Width / lvl1ArcherImg.Height), PREVIEW_SIZE, () => ArchPrev(0));
        
        archerPrevs[1] = new Button(lvl2ArcherImg, archerPrevs[0].Rec.X - 5 - archerPrevs[0].Rec.Width, 5,
            PREVIEW_SIZE * ((float)lvl2ArcherImg.Width / lvl2ArcherImg.Height), PREVIEW_SIZE, () => ArchPrev(1));
        
        archerPrevs[2] = new Button(lvl3ArcherImg, archerPrevs[1].Rec.X - 5 - archerPrevs[1].Rec.Width, 5,
            PREVIEW_SIZE * ((float)lvl3ArcherImg.Width / lvl3ArcherImg.Height), PREVIEW_SIZE, () => ArchPrev(2));
        
        // Constructing demolish building button and image
        Texture2D demolishImg = Content.Load<Texture2D>("Images/Sprites/UI/TrashCan");
        demolishBuilding = new Button(demolishImg, screenWidth - PREVIEW_SIZE * ((float)demolishImg.Width / demolishImg.Height), 
            wallPrevs[0].Rec.Bottom + 5, PREVIEW_SIZE * ((float)demolishImg.Width / demolishImg.Height), 
            PREVIEW_SIZE, DemolishButton);
        
        // Loading red cross texture
        redCrossImg = Content.Load<Texture2D>("Images/Sprites/UI/RedCross");
            
        // Loading buildable rectangle to be on the floor as a preview of where you can build
        buildableRec = new Rectangle((int)WidthCenter(800), platform.Rec.Y, 800, platform.Rec.Height);
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
    
    // Method accessible by zombies to increase points
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

    #region Update objects

    private void UpdateObjects(GameTime gameTime)
    {
        // Updating king tower, cannonballs and their explosions
        UpdateKing(gameTime);
        UpdateCannonballs();
        UpdateExplosions(gameTime);
        
        // Updating archer towers, checking for placement and collisions, and updating arrows
        UpdateArchers(gameTime);
        UpdateArrows();
        
        // Updating walls and checking for placement and collisions
        UpdateWalls();
        
        // Updating zombies
        UpdateZombies(gameTime);
    }

    private void UpdateKing(GameTime gameTime)
    {
        kingTower.Update(mouse, gameTime);
        
        // Checking for cannon launch
        LaunchCannon();
    }

    private void UpdateCannonballs()
    {
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Update(timePassed);
            }
        }
    }

    private void UpdateArchers(GameTime gameTime)
    {
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null)
            {
                // If this statement returns true it means the wall was destroyed
                if (archers[i].Update(gameTime, mouse, buildableRec, ValidPlacement(archers[i].Hitbox)))
                {
                    archers[i] = null;
                }
                
                // Shooting arrows
                // Finding an empty arrow slot
                for (int j = 0; j < arrows.Length; j++)
                {
                    if (arrows[j] == null)
                    {
                        // Creating a new arrow to fire to the closest zombie
                        arrows[j] = archers[i].ShootArrow(LocateNearestZombie(archers[i].Hitbox, archers[i].Range));
                        break;
                    }
                }
            }
        }
    }

    private void UpdateArrows()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
            {
                arrows[i].Update(timePassed);
            }
        }
    }

    private void UpdateWalls()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                // If this statement returns true it means the wall was destroyed
                if (walls[i].Update(mouse, buildableRec, ValidPlacement(walls[i].Hitbox)))
                {
                    walls[i] = null;
                }
            }
        }
    }

    private void UpdateZombies(GameTime gameTime)
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Update(gameTime, timePassed * 1000);
        }
    }

    private void UpdateExplosions(GameTime gameTime)
    {
        for (int i = 0; i < explosionAnims.Length; i++)
        {
            explosionAnims[i].Update(gameTime);
        }
    }

    #endregion
    
    #region Game Main Methods

    // Updating everything that is common to both levels
    private void UpdateGame(GameTime gameTime)
    {
        // Casting night sky every day night cycle
        DayNightCycle(gameTime);
        
        // Updating all objects in game
        UpdateObjects(gameTime);
        
        // Updating tower preview buttons AFTER preview check so it won't place automatically
        wallPrevs[0].Update(mouse, prevMouse);
        wallPrevs[1].Update(mouse, prevMouse);
        archerPrevs[0].Update(mouse, prevMouse);
        archerPrevs[1].Update(mouse, prevMouse);
        archerPrevs[2].Update(mouse, prevMouse);
        
        // Updating demolish button and checking for demolition
        demolishBuilding.Update(mouse, prevMouse);
        Demolition();
        
        // Checking for placement for walls
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                walls[i].CheckPlacement(mouse, prevMouse, platform.Rec);
            }
        }
        
        // Checking for placement for archer towers
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null)
            {
                archers[i].CheckPlacement(mouse, prevMouse, platform.Rec);
            }
        }
        
        // Handling all of collisions
        Collisions();
        
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
                if (demolishing)
                {
                    walls[i].Draw(_spriteBatch, Color.Red);
                }
                else
                {
                    walls[i].Draw(_spriteBatch, Color.White);
                }
            }
        }
        
        // Drawing archer towers
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null)
            {
                if (demolishing)
                {
                    archers[i].Draw(_spriteBatch, buildableRec.Center.X, Color.Red);
                }
                else
                {
                    archers[i].Draw(_spriteBatch, buildableRec.Center.X, Color.White);
                }
            }
        }

        // Drawing cannonballs with their explosions
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Draw(_spriteBatch);
            }
            
            explosionAnims[i].Draw(_spriteBatch, Color.White);
        }
        
        // Drawing arrows
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
            {
                arrows[i].Draw(_spriteBatch);
            }
        }
        
        // Drawing HUD
        DrawHud();
        
        // Drawing buildable area rectangle as just a single solid color
        _spriteBatch.Draw(pixelImg, buildableRec, Color.Green * 0.5f);
    }

    #endregion

    #region Collisions

    private void Collisions()
    {
        ZombieCollision();
        CannonCollision();
        ArrowCollision();
    }
    
    private void ZombieCollision()
    {
        // Checking collision with each zombie
        for (int i = 0; i < zombies.Length; i++)
        {
            // Storing a bool to keep track if the zombie is attacking or not
            bool isAttacking = false;
        
            // Checking for king tower
            if (zombies[i].Rec.Intersects(kingTower.Hitbox))
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
                    if (zombies[i].Rec.Intersects(walls[j].Hitbox))
                    {
                        // Dealing damage to wall
                        walls[j].HP = zombies[i].Attack(walls[j].HP);
                        
                        // Setting attacking to true
                        isAttacking = true;
                    }
                }
            }
            
            // Checking for archer towers
            for (int j = 0; j < archers.Length; j++)
            {
                // Making sure wall isn't null and it is currently placed
                if (archers[j] != null && archers[j].IsPlaced())
                {
                    if (zombies[i].Rec.Intersects(archers[j].Hitbox))
                    {
                        // Dealing damage to wall
                        archers[j].HP = zombies[i].Attack(archers[j].HP);
                        
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
        // Checking collision for each cannonball
        for (int i = 0; i < cannonballs.Length; i++)
        {
            // Making sure it's not null
            if (cannonballs[i] != null)
            {
                // Detecting collision with platform
                if (cannonballs[i].Hitbox.Bottom >= platform.Rec.Y)
                {
                    cannonballs[i].Hitbox = new Rectangle(cannonballs[i].Hitbox.X, 
                        platform.Rec.Top - cannonballs[i].Hitbox.Height,
                        cannonballs[i].Hitbox.Width, cannonballs[i].Hitbox.Height);
                    
                    // Keeping count of how many zombies were hit to max it to 4
                    int count = 0;
                    
                    // Checking if any zombies were hit and damaging all that did (clamping max number of zombies per attack to 4 to balance)
                    for (int j = 0; j < zombies.Length; j++)
                    {
                        if (zombies[j].Rec.Intersects(cannonballs[i].Hitbox))
                        {
                            // Dealing damage to zombie and increasing count
                            zombies[j].HP -= cannonballs[i].Damage;
                            count++;
                        }

                        // Ending if  4 zombies were hit
                        if (count == 4)
                        {
                            break;
                        }
                    } 
                    // Playing explosion animation
                    explosionAnims[i].TranslateTo(cannonballs[i].Rec.X - (explosionAnims[i].GetDestRec().Width - 
                                                                               cannonballs[i].Rec.Width) / 2, 
                        cannonballs[i].Rec.Y - (explosionAnims[i].GetDestRec().Height - 
                                                     cannonballs[i].Rec.Height) / 2 - 10);
                    explosionAnims[i].Activate(true);
                    
                    // Setting to null so it can be reused
                    cannonballs[i] = null;
                }
            }
        }
    }

    private void ArrowCollision()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            // Making sure arrow is not null
            if (arrows[i] != null)
            {
                // Checking if the arrow hit any zombie
                for (int j = 0; j < zombies.Length; j++)
                {
                    if (arrows[i].Rec.Intersects(zombies[j].Rec))
                    {
                        // Dealing damage to zombie and deleting arrow
                        zombies[j].HP -= arrows[i].Damage;
                        arrows[i] = null;
                        break;
                    }
                }
                
                // Setting it to null if it hits the ground
                if (arrows[i] != null && arrows[i].Rec.Bottom >= platform.Rec.Top)
                {
                    arrows[i] = null;
                }
            }
        }
    }

    #endregion

    private Vector2 LocateNearestZombie(Rectangle towerRec, float radius)
    {
        // Storing nearest distance and current distance
        Vector2 closestZombie = new Vector2(5000, 5000); // Making sure the closest distance would be updated from the first zombie
        float currentDist;
        
        for (int i = 0; i < zombies.Length; i++)
        {
            // Making sure zombie isn't dead
            if (!zombies[i].IsDead())
            {
                // Calculating distance between tower and each zombie
                currentDist = Vector2.Distance(towerRec.Center.ToVector2(), zombies[i].Rec.Location.ToVector2());

                // Checking if distance is in radius
                if (currentDist <= radius)
                {
                    // Checking if the distance is less than the closest
                    if (currentDist < Vector2.Distance(closestZombie, towerRec.Center.ToVector2()))
                    {
                        // Storing closest zombie's location
                        closestZombie = zombies[i].Rec.Location.ToVector2();
                    }
                }
            }
        }
        
        return closestZombie;
    }
    
    private void LaunchCannon()
    {
        // Checking for king tower shots only at nighttime and when other stuff is not in preview
        if (skyOpacity == 1 && PreviewCheck())
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
        // Checking intersection with king tower
        if (building.Intersects(kingTower.Hitbox))
        {
            return false;
        }

        // Checking intersection with wall towers
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i] != null)
            {
                if (building.Intersects(walls[i].Hitbox) && walls[i].IsPlaced())
                {
                    return false;
                }
            }
        }
        
        // Checking intersection with archer towers
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null)
            {
                if (building.Intersects(archers[i].Hitbox) && archers[i].IsPlaced())
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private void DrawHud()
    {
        // Drawing day count, kill count, health with offset, and coins
        DrawWithShadow(HUDFont, dispDayCount, dayCountPos, Color.Yellow, Color.Black);
        DrawWithShadow(HUDFont, dispMobsKilled, mobsKilledPos, Color.IndianRed, Color.Black);
        DrawWithShadow(HUDFont, dispKingHP, kingHPPos, Color.Green, Color.Black);
        DrawWithShadow(HUDFont, dispCoins, coinsPos, Color.Gold, Color.DarkGoldenrod);
        
        // Drawing wall tower preview options
        for (int i = 0; i < wallPrevs.Length; i++)
        {
            DrawWithPrice(wallPrevs[i], Wall.GetDefaultPrice(i));
        }
        
        // Drawing archer tower preview options
        for (int i = 0; i < archerPrevs.Length; i++)
        {
            DrawWithPrice(archerPrevs[i], ArcherTower.GetDefaultPrice(i));
        }
        
        // Drawing demolish button
        demolishBuilding.Draw(_spriteBatch);
        
        // Drawing cancel buttons on wall buttons when in preview
        foreach (Wall wall in walls)
        {
            for (int i = 0; i < wallPrevs.Length; i++)
            {
                if (wall != null && !wall.IsPlaced() && wall.Lvl == i)
                {
                    _spriteBatch.Draw(redCrossImg, wallPrevs[i].Rec, Color.White);
                }
            }
        }
        
        // Drawing cancel buttons on archer tower buttons when in preview
        foreach (ArcherTower tower in archers)
        {
            for (int i = 0; i < archerPrevs.Length; i++)
            {
                if (tower != null && !tower.IsPlaced() && tower.Lvl == i)
                {
                    _spriteBatch.Draw(redCrossImg, archerPrevs[i].Rec, Color.White);
                }
            }
        }
    }
    
    // Meant for drawing preview buttons with prices on them
    private void DrawWithPrice(Button button, int price)
    {
        button.Draw(_spriteBatch);
        _spriteBatch.DrawString(smallFont, $"${price}", button.Rec.Location.ToVector2(), Color. Gold);
    }
    
    // Checking for demolition
    private void Demolition()
    {
        // Procceeding if currently demolishing
        if (demolishing)
        {
            // Checking for mouse click
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                // Looping through walls to see if mouse is on any of them
                for (int i = 0; i < walls.Length; i++)
                {
                    if (walls[i] != null && walls[i].Hitbox.Contains(mouse.Position))
                    {
                        // Destroying wall and giving refund
                        coins += (int)(walls[i].Price * walls[i].HPPercentage);
                        walls[i] = null;
                        
                        // exiting demolishing state and ending method
                        demolishing = false;
                        return;
                    }
                }
                
                // Looping through archer towers to see if mouse is on any of them
                for (int i = 0; i < archers.Length; i++)
                {
                    if (archers[i] != null && archers[i].Hitbox.Contains(mouse.Position))
                    {
                        // Destroying wall and giving refund
                        coins += (int)(archers[i].Price * archers[i].HPPercentage);
                        archers[i] = null;
                        
                        // exiting demolishing state and ending method
                        demolishing = false;
                        return;
                    }
                }
            }
        }
    }

    #region Button Actions

    // Action for level 1 button click
    public void Level1Button()
    {
        // Setting game state to level 1
        gameState = LEVEL_1;
        
        // Updating king position
        kingTower.TranslateTo(lvl1KingPos);
        
        // Modifying buildable rec to be the right third of the screen
        buildableRec.Width = screenWidth / 3;
        buildableRec.X = screenWidth - buildableRec.Width;
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

        // Checking if any archer tower is in preview
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null)
            {
                // If ANY archer towers is in preview, not valid
                if (!archers[i].IsPlaced())
                {
                    return false;
                }
            }
        }
        
        // If it passed all of them it must be valid
        return true;
    }

    public void WallPrev(byte lvl)
    {
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
                Texture2D wallImg = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Wall/WallLvl{lvl + 1}");
                walls[i] = new Wall(wallImg, wallImg.Width / 2, wallImg.Height / 2, 6, platform.Rec, lvl);
                break;
            }
        }
    }

    public void ArchPrev(byte lvl)
    {
        // Checking if ANY wall is currently being placed
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] != null && !archers[i].IsPlaced())
            {
                // Setting the wall to null and ending method
                archers[i] = null;
                return;
            }
        }
        
        // Checking for an empty slot in null array
        for (int i = 0; i < archers.Length; i++)
        {
            if (archers[i] == null && PreviewCheck())
            {
                // Creating a new wall instance to be placed
                Texture2D img = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Archer/ArcherTowerLvl{lvl + 1}");
                Texture2D arrowImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/Arrow");
                archers[i] = new ArcherTower(img, new Vector2(0, platform.Rec.Top - img.Height / 2 + 5), 
                                    img.Width / 2, img.Height / 2, img.Width / 2, img.Height / 2, arrowImg, lvl);
                break;
            }
        }
    }

    public void DemolishButton()
    {
        if (!demolishing)
        {
            demolishing = true;
        }
        else
        {
            demolishing = false;
        }
    }

    #endregion
    
}