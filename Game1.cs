// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: June 9th 2025
// Description: Main driver class for the game

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    
    // Storing random number generator
    public static Random rng = new Random();

    // Storing screen dimensions
    public static int screenWidth;
    private int screenHeight;
    
    // Storing mouse input
    private MouseState mouse;
    private MouseState prevMouse;
    
    // Storing keyboard input
    private KeyboardState kb;
    private KeyboardState prevKb;
    
    // Storing a single pixel texture to draw a simple solid color
    public static Texture2D pixelImg;

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
    
    // Storing background image for game over screen
    private Texture2D gameoverImg;
    
    // Storing position of kills and days in game over
    private Vector2 gameoverKillPos;
    private Vector2 gameoverDayPos;
    
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
    
    // Storing array of tutorial image slides and index of current slide
    private Texture2D[] tutorialImgs = new Texture2D[5];
    private byte currentSlide = 0;
    
    #endregion

    #region Gameplay Variables

    // Storing time passed
    private float timePassedSeconds;
    
    // Storing game background image and rectangle
    private Texture2D gameBgImg;
    private Rectangle gameBgRec;
    
    // Storing platform
    private Platform platform;
    
    // Storing a public instance that will be uniform for the whole project of Message Manager
    public static MessageManager messageManager;
    
    // Storing buildable rectangle area
    private Rectangle buildableRec;
    
    // Storing king tower
    private KingTower kingTower;
    
    // Storing the two king tower locaations for each level
    private Vector2 lvl1KingPos;
    private Vector2 lvl2KingPos;
    
    // Storing max number of zombies which will change every night
    private const int INITIAL_MAX_ZOMBIES = 10;
    private int maxZombies = INITIAL_MAX_ZOMBIES;
    
    // Storing zombies max hp as it increases every night
    private const int INITIAL_ZOMBIE_HP = 20;
    private int zombieHP = INITIAL_ZOMBIE_HP;
    
    // Storing zombies array
    private Zombie[] zombies = new Zombie[100];
    
    // Storing nighttime length that will increase every night
    private const int INITIAL_NIGHTTIME = 40000;
    private int nightTime = INITIAL_NIGHTTIME;
    
    // Storing timer for day night cycle
    private Timer dayNightCycle = new Timer(INITIAL_NIGHTTIME, false);
    
    // Storing background night sky texture and rectangle
    private Texture2D nightBGImg;
    private Rectangle nightBGRec;
    
    // Storing night sky color multiplier, incrase and decrease constants, and multiplier modifier
    private float skyOpacity = 0f;
    private const int POSITIVE = 1;
    private const int NEGATIVE = -1;
    private int skyMultiplier;
    
    // Storing explosion animation
    private Animation[] explosionAnims;
    
    // Storing a list of defences (i.e. towers pre change)
    private List<Defence> defences = new List<Defence>();
    
    // Storing if any in preview
    private bool isAnyInPreview;
    
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
    private const int INITIAL_COINS = 200;
    private static int coins = INITIAL_COINS;
    private string dispCoins = $"${coins}";
    private Vector2 coinsPos;
    
    // Storing position to display how much time left in round
    private Vector2 timeLeftDispPos;
    
    // Width and height for the defence preview buttons
    private const int PREVIEW_SIZE = 80;
    
    // Storing buttons to purchase walls
    private Button[] wallPrevs = new Button[2];
    
    // Storing buttons to purchase archer defences
    private Button[] archerPrevs = new Button[3];
    
    // Storing button to purchase landmine
    private Button landminePrev;
    
    // Storing currently selected preview button
    private Button selBut = null;
    
    // Storing button to demolish building
    private Button demolishBuilding; 
    
    // Storing demolishing bool (if currently in demolishing or not)
    private bool demolishing;
    
    // Storing how many zombies were killed in an update
    public static int zombiesKilledPerUpdate = 0;
    
    #endregion

    #region Sub Region - Projectiles

    // Storing max number of cannonballs inside array size
    private Cannonball[] cannonballs = new Cannonball[10];
    
    // Storing max number of arrows inside array size
    private Arrow[] arrows = new Arrow[20];

    #endregion

    #endregion

    #region Sounds

    // Storing sounds for various game activities. Making them public static so they can be played on any class
    public static SoundEffect cannonSnd;
    public static SoundEffect arrowSnd;
    
    public static SoundEffect defencePlacedSnd;
    public static SoundEffect defenceDestroyedSnd;

    public static SoundEffect buttonSnd;
    
    public static SoundEffect zombieAttackSnd;
    public static SoundEffect zombieHitSnd;
    public static SoundEffect zombieSpawnSnd;

    #endregion

    #region Music

    // Storing game background music
    private Song gameMusic;
    
    // Storing an array of time spans in which a new song starts playing in game music soundtrack (17 total)
    private TimeSpan[] gameMusicSongTimes = new TimeSpan[] { new TimeSpan(0, 3, 46), new TimeSpan(0, 7, 0), 
                                                             new TimeSpan(0, 10, 38), new TimeSpan(0, 14, 13),
                                                             new TimeSpan(0, 17, 32), new TimeSpan(0, 21, 22),
                                                             new TimeSpan(0, 25, 1), new TimeSpan(0, 28, 05),
                                                             new TimeSpan(0, 31, 49), new TimeSpan(0, 35, 50),
                                                             new TimeSpan(0, 39, 13), new TimeSpan(0, 42, 41),
                                                             new TimeSpan(0, 46, 0), new TimeSpan(0, 49, 29),
                                                             new TimeSpan(0, 52, 53), new TimeSpan(0, 56, 11),
                                                             new TimeSpan(0, 59, 36) };

    #endregion
    
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    #region Monogame methods
    protected override void Initialize()
    {
        // Attempting to set screen dimensions to 1600x1000
        graphics.PreferredBackBufferWidth = 1600;
        graphics.PreferredBackBufferHeight = 1000;
        
        // Applying the screen dimensions changes
        graphics.ApplyChanges();

        // Storing the resultant dimensions to determine the drawable space
        screenWidth = graphics.GraphicsDevice.Viewport.Width;
        screenHeight = graphics.GraphicsDevice.Viewport.Height;

        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
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
        
        // Loading game over background
        gameoverImg = Content.Load<Texture2D>("Images/Backgrounds/GameoverBg");
        
        // Loading game background
        gameBgImg = Content.Load<Texture2D>("Images/Backgrounds/GameBackground");
        gameBgRec = new Rectangle(0, 400, screenWidth, screenHeight);
        
        // Loading single pixel texture
        pixelImg = new Texture2D(GraphicsDevice, 1, 1);
        pixelImg.SetData(new[] { Color.White });
        
        // Loading platform and its texture (defining texture locally as it will be used in the Platform class)
        Texture2D platformImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Brick");
        platform = new Platform(platformImg, screenWidth, screenHeight);
        
        // Loading tutorial image slides
        for (int i = 0; i < tutorialImgs.Length; i++)
        {
            tutorialImgs[i] = Content.Load<Texture2D>($"Images/Sprites/UI/Tutorial/Slide{i + 1}");
        }
        
        // Loading message manager
        messageManager = new MessageManager(smallFont, new Vector2(10, platform.Rec.Top - 150));
        
        // Loading king tower, position, & image (defining king tower image locally as it will be used in the Tower class)
        Texture2D kingTowerImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/KingTower");
        Texture2D cannonballImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Cannonball");
        
        kingTower = new KingTower(kingTowerImg, nightBGRec.Location.ToVector2(), kingTowerImg.Width, 
                                kingTowerImg.Height, 266, 330, cannonballImg, 750);
        
        lvl1KingPos = new Vector2(screenWidth - kingTower.DisplayRec.Width / 2f + 30, 
                                    platform.Rec.Y - kingTower.Hitbox.Height + 10);
        lvl2KingPos = new Vector2(CenterWidth(kingTower.Hitbox.Width),
                                    platform.Rec.Y - kingTower.Hitbox.Height + 10);

        // Loading button images
        level1Img = Content.Load<Texture2D>("Images/Sprites/UI/Level1Button");
        level2Img = Content.Load<Texture2D>("Images/Sprites/UI/Level2Button");
        settingsImg = Content.Load<Texture2D>("Images/Sprites/UI/SettingsButton");
        tutorialBtnImg = Content.Load<Texture2D>("Images/Sprites/UI/TutorialButton");
        
        // Loading all menu buttons
        level1Button = new Button(level1Img, (int)CenterWidth(level1Img.Width) - 400, screenHeight - 300, 
                                    level1Img.Width, level1Img.Height, Level1Button);
        level2Button = new Button(level2Img, (int)CenterWidth(level1Img.Width) + 400, screenHeight - 300, 
                                    level2Img.Width, level2Img.Height, Level2Button);
        settingsButton = new Button(settingsImg, 50, 50, settingsImg.Width / 5f, settingsImg.Height / 5f, 
                            () => gameState = SETTINGS);
        tutorialButton = new Button(tutorialBtnImg, screenWidth - 50 - tutorialBtnImg.Width / 5f, 50, 
            tutorialBtnImg.Width / 5f, tutorialBtnImg.Height / 5f, TutorialButton);
        
        // Loading title image and rectangle
        titleImg = Content.Load<Texture2D>("Images/Sprites/UI/Title");
        titleRec = new Rectangle((int)CenterWidth(titleImg.Width * 0.75f), 10, 
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
        
        // Loading blood animation image variants
        Texture2D[] bloodImgs = new Texture2D[5];
        for (int i = 0; i < bloodImgs.Length; i++)
        {
            bloodImgs[i] = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Blood/Blood{i+1}");
        }
        
        
        // Loading all the zombies
        for (int i = 0; i < zombies.Length; i++)
        {
            // Randomizing variant and blood animation type
            zombies[i] = new Zombie(zombieImgs[rng.Next(0, 3)], bloodImgs[rng.Next(0, bloodImgs.Length)], screenWidth, 
                          platform.Rec.Y);
        }
        
        // Loading positions of HUD objects
        dayCountPos = new Vector2(5, 5);
        mobsKilledPos = new Vector2(5, dayCountPos.Y + HUDFont.MeasureString(dispDayCount).Y);
        timeLeftDispPos = new Vector2(5, mobsKilledPos.Y + HUDFont.MeasureString(dispDayCount).Y);

        kingHPPos = new Vector2(CenterWidth(HUDFont.MeasureString(dispKingHP).X), 5);
        coinsPos = new Vector2(CenterWidth(HUDFont.MeasureString(dispCoins).X),
            5 + HUDFont.MeasureString(dispKingHP).Y);
        
        // Loading explosion animation and saving local image for each cannonball
        Texture2D explosionImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/explosion");
        explosionAnims = new Animation[cannonballs.Length];
        for (int i = 0; i < explosionAnims.Length; i++)
        {
            explosionAnims[i] = new Animation(explosionImg, 5, 5, 23, 0, -1, 1, 1000, new Vector2(800, 500), 2, false);
        }
        
        // Storing red cross texture to show cancel placement
        Texture2D redCrossImg = Content.Load<Texture2D>("Images/Sprites/UI/RedCross");
        
        // Loading wall textures for buttons
        Texture2D lvl1Wall = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl1");
        Texture2D lvl2Wall = Content.Load<Texture2D>("Images/Sprites/Gameplay/Wall/WallLvl2");
        
        // Loading wall preview buttons using wall textures
        wallPrevs[0] = new Button(lvl1Wall, redCrossImg, screenWidth - PREVIEW_SIZE - 5, 5, 
            PREVIEW_SIZE, PREVIEW_SIZE, () => WallPrev(0), () => WallPrevDrawHover(0));
        wallPrevs[1] = new Button(lvl2Wall, redCrossImg, wallPrevs[0].Rec.X - 5 - PREVIEW_SIZE, 5,
            PREVIEW_SIZE, PREVIEW_SIZE, () => WallPrev(1), () => WallPrevDrawHover(1));
        
        // Loading archer tower textures
        Texture2D lvl1ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl1");
        Texture2D lvl2ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl2");
        Texture2D lvl3ArcherImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/ArcherTowerLvl3");
        
        // Loading archer tower preview buttons (couldn't do a loop because of the x offset)
        archerPrevs[0] = new Button(lvl1ArcherImg, redCrossImg, wallPrevs[1].Rec.X - 5 - PREVIEW_SIZE, 5,
                              PREVIEW_SIZE * ((float)lvl1ArcherImg.Width / lvl1ArcherImg.Height), PREVIEW_SIZE, 
                            () => ArchPrev(0), () => ArcherPrevDrawHover(0));
        
        archerPrevs[1] = new Button(lvl2ArcherImg, redCrossImg, archerPrevs[0].Rec.X - 5 - archerPrevs[0].Rec.Width, 5,
                               PREVIEW_SIZE * ((float)lvl2ArcherImg.Width / lvl2ArcherImg.Height), PREVIEW_SIZE, 
                             () => ArchPrev(1), () => ArcherPrevDrawHover(1));
        
        archerPrevs[2] = new Button(lvl3ArcherImg, redCrossImg, archerPrevs[1].Rec.X - 5 - archerPrevs[1].Rec.Width, 5,
                               PREVIEW_SIZE * ((float)lvl3ArcherImg.Width / lvl3ArcherImg.Height), PREVIEW_SIZE, 
                             () => ArchPrev(2), () => ArcherPrevDrawHover(2));
        
        // Loading landmine texture and button
        Texture2D landmineImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/LandMine");
        landminePrev = new Button(landmineImg, redCrossImg, archerPrevs[2].Rec.X - 5 - PREVIEW_SIZE,
                                  5 + PREVIEW_SIZE * (1 - (float)landmineImg.Height / landmineImg.Width), PREVIEW_SIZE, 
                                  PREVIEW_SIZE * (float)landmineImg.Height / landmineImg.Width, 
                                  LandminePrev, LandminePrevDrawHover);
        
        // Constructing demolish building button and image
        Texture2D demolishImg = Content.Load<Texture2D>("Images/Sprites/UI/TrashCan");
        demolishBuilding = new Button(demolishImg, 
                                      screenWidth - PREVIEW_SIZE * ((float)demolishImg.Width / demolishImg.Height), 
                                      wallPrevs[0].Rec.Bottom + 5, 
                                      PREVIEW_SIZE * ((float)demolishImg.Width / demolishImg.Height),
                                      PREVIEW_SIZE, DemolishButton);
            
        // Loading buildable rectangle to be on the floor as a preview of where you can build
        buildableRec = new Rectangle((int)CenterWidth(screenWidth / 2), platform.Rec.Y, screenWidth / 2,
                                     platform.Rec.Height);
        
        // Loading all sounds
        cannonSnd = Content.Load<SoundEffect>("Audio/Sounds/CannonShoot");
        arrowSnd = Content.Load<SoundEffect>("Audio/Sounds/ArrowShoot");
        
        buttonSnd = Content.Load<SoundEffect>("Audio/Sounds/ButtonClick");
        
        defencePlacedSnd = Content.Load<SoundEffect>("Audio/Sounds/BuildingPlaced");
        defenceDestroyedSnd = Content.Load<SoundEffect>("Audio/Sounds/BuildingBreak");
        
        zombieAttackSnd = Content.Load<SoundEffect>("Audio/Sounds/ZombieAttack");
        zombieHitSnd = Content.Load<SoundEffect>("Audio/Sounds/ZombieHit");
        zombieSpawnSnd = Content.Load<SoundEffect>("Audio/Sounds/ZombieSpawn");
        
        // Loading game soundtrack
        gameMusic = Content.Load<Song>("Audio/Music/GameMusic");

        // Setting volume of music and setting repeat to true
        MediaPlayer.Volume = 0.4f;
        MediaPlayer.IsRepeating = true;
        
        // Starting music
        ShuffleMusic();
    }
    
    protected override void Update(GameTime gameTime)
    {
        // Updating mouse input
        prevMouse = mouse;
        mouse = Mouse.GetState();
        
        // Updating keyboard input
        prevKb = kb;
        kb = Keyboard.GetState();
        
        // Storing time passed
        timePassedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
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
                
                // Checking for click to move slideshow:
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    currentSlide++;
                }
                
                // Checking if slideshow is over
                if (currentSlide == 5)
                {
                    gameState = MENU;
                }
                
                break;
            
            case ENDGAME:
                
                // Checking for enter to go back to menu
                if (kb.IsKeyDown(Keys.Enter) && !prevKb.IsKeyDown(Keys.Enter))
                {
                    gameState = MENU;
                }
                break;
        }

        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.RoyalBlue);

        // Initializing sprite drawing batch
        spriteBatch.Begin();
        
        // Drawing game based on game state
        switch (gameState)
        {
            case MENU:
                // Drawing background
                spriteBatch.Draw(bgImg, bgRec, Color.White);
                
                // Draw title
                spriteBatch.Draw(titleImg, titleRec, Color.White);
                
                // Drawing buttons
                level1Button.Draw(spriteBatch, mouse.Position);
                level2Button.Draw(spriteBatch, mouse.Position);
                settingsButton.Draw(spriteBatch, mouse.Position);
                tutorialButton.Draw(spriteBatch, mouse.Position);
                
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
                spriteBatch.Draw(bgImg, bgRec, Color.White);
                
                break;
            
            case SETTINGS:
                // Drawing background
                spriteBatch.Draw(bgImg, bgRec, Color.White);
                
                break;
            
            case TUTORIAL:
                // Drawing current slide. Using bgRec since it just fills the whole screen
                spriteBatch.Draw(tutorialImgs[currentSlide], bgRec, Color.White);
                
                break;
            
            case ENDGAME:
                // Drawing background
                spriteBatch.Draw(gameoverImg, bgRec, Color.White);
                
                // Drawing final stats
                DrawWithShadow(HUDFont, dispMobsKilled, gameoverKillPos, Color.Red, Color.Black);
                DrawWithShadow(HUDFont, dispDayCount, gameoverDayPos, Color.Yellow, Color.Black);
                
                break;
        }
        
        // Finish sprite batch
        spriteBatch.End();

        base.Draw(gameTime);
    }

    #endregion

    /// <summary>
    /// Storing coins as a public property of Game1 
    /// </summary>
    public static int Coins
    {
        get => coins;
        set => coins = value;
    }
    
    /// <summary>
    /// Method accessible by zombies class to increase kill count
    /// </summary>
    public static void IncreaseMobsKilled()
    {
        mobsKilled++;
        dispMobsKilled = $"Kills: {mobsKilled}";
    }
        
    /// <summary>
    /// Manages day night cycle by keeping track of a timer and increasing/decreasing night sky opacity
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void DayNightCycle(GameTime gameTime)
    {
        // Updating day night cycle timer
        dayNightCycle.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        
        // Checking if the cycle is over
        if (dayNightCycle.IsFinished())
        {
            // Flipping the sky multiplier to either turn the sky on or off
            // Turning nighttime
            if (skyOpacity == 0)
            {
                skyMultiplier = POSITIVE;
                
                // Updating wave stats and spawning zombies
                ZombieWaves();
            }
            // Turning daytime
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
            dayNightCycle.ResetTimer(true, nightTime);
        }

        // Updating the sky opacity and clamping it
        skyOpacity += skyMultiplier * 0.01f;
        skyOpacity = MathHelper.Clamp(skyOpacity, 0, 1);
        
    }

    #region Centers & Drop Shadow

    /// <summary>
    /// The function uses screen width and text/image width to center the image horizontally
    /// </summary>
    /// <param name="spriteWidth"> The width of the thing that is centered horizontally on the screen</param>
    /// <returns>Returns an X position on screen of the rectangle/vector2 of the object</returns>
    private float CenterWidth(float spriteWidth)
    {
        float center;

        // Calculating center by averaging the 2 widths
        center = (screenWidth - spriteWidth) / 2f;

        return center;
    }
    
    /// <summary>
    /// The function uses screen height and text/image height to center the image vertically
    /// </summary>
    /// <param name="spriteHeight">The width of the thing that is centered vertically on the screen</param>
    /// <returns>Returns a Y position on screen of the rectangle/vector2 of the object</returns>
    private float CenterHeight(float spriteHeight)
    {
        float center;

        // Calculating center by averaging the 2 heights
        center = (screenHeight - spriteHeight) / 2f;

        return center;
    }
    
    /// <summary>
    /// Drawing string with drop shadow
    /// </summary>
    /// <param name="font">SpriteFont of text</param>
    /// <param name="text">Actual text/message</param>
    /// <param name="pos">Vector 2 position of text on screen</param>
    /// <param name="color">Color of actual text</param>
    /// <param name="shadowColor">Color of drop shadow background</param>
    private void DrawWithShadow(SpriteFont font, string text, Vector2 pos, Color color, Color shadowColor)
    {
        spriteBatch.DrawString(font, text, pos + DROP_SHADOW, shadowColor);
        spriteBatch.DrawString(font, text, pos, color);
    }

    #endregion

    #region Update objects

    /// <summary>
    /// Central method to keep track of all objects that are being updated
    /// </summary>More actions
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void UpdateObjects(GameTime gameTime)
    {
        // Updating king tower, cannonballs and their explosions
        UpdateKing(gameTime);
        UpdateCannonballs();
        UpdateExplosions(gameTime);
        
        // Updating archer defences, checking for placement and collisions, and updating arrows
        UpdateArrows();
        
        // Updating walls and checking for placement and collisions
        UpdateDefences(gameTime);
        
        // Updating zombies
        UpdateZombies(gameTime);
    }

    /// <summary>
    /// Updating king tower and checking for cannon launches
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void UpdateKing(GameTime gameTime)
    {
        if (kingTower.Update(gameTime, mouse, buildableRec, true, screenWidth, zombies))
        {
            gameState = ENDGAME;
            
            // Loading positions of kill and day counts in game over screen
            gameoverKillPos = new Vector2(CenterWidth(HUDFont.MeasureString(dispMobsKilled).X) - 500,
                CenterHeight(HUDFont.MeasureString(dispMobsKilled).Y));
            gameoverDayPos = new Vector2(CenterWidth(HUDFont.MeasureString(dispDayCount).X) + 500,
                CenterHeight(HUDFont.MeasureString(dispDayCount).Y));
        }
    }

    /// <summary>
    /// Looping through cannonballs and updating the ones that are not null
    /// </summary>
    private void UpdateCannonballs()
    {
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Update(timePassedSeconds);
            }
        }
    }

    /// <summary>
    /// Updating all Defences by looping through them. Method also checks if defence is a landmine to create an explosion
    /// or if its an archer tower to shoot an arrow
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void UpdateDefences(GameTime gameTime)
    {
        // Looping through tower list to update
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null)
            {
                if (defences[i].Update(gameTime, mouse, buildableRec, ValidPlacement(defences[i].Hitbox), screenWidth, zombies))
                {
                    // Checking if its landmine, spawning cannonball to explode at landmine location
                    if (defences[i].IsPlaced() && defences[i] is Landmine)
                    {
                        // Checking for an empty slot in cannonball array
                        for (int j = 0; j < cannonballs.Length; j++)
                        {
                            if (cannonballs[j] == null)
                            {
                                // Creating new cannonball to explode at landmine location
                                Texture2D cannonballImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Cannonball");
                                cannonballs[j] = new Cannonball(defences[i].Hitbox, cannonballImg, defences[i].Damage, 3);
                                break;
                            }
                        }
                    }
                    // Playing tower destroyed sound if it's not a landmine
                    else if (defences[i].IsPlaced())
                    {
                        PlaySound(defenceDestroyedSnd, 0.5f);
                    }
                    
                    // Removing tower
                    defences.RemoveAt(i);
                }
                // Shooting arrows
                // Finding an empty arrow slot
                else if (defences[i] is ArcherTower)
                {
                    for (int j = 0; j < arrows.Length; j++)
                    {
                        if (arrows[j] == null)
                        {
                            // Creating a new arrow to fire to the closest zombie
                            arrows[j] = ((ArcherTower)defences[i]).ShootArrow(LocateNearestZombie(defences[i].Hitbox,
                                ((ArcherTower)defences[i]).Range));
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Looping through arrows and updating the ones that are not null
    /// </summary>
    private void UpdateArrows()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
            {
                arrows[i].Update(timePassedSeconds);
            }
        }
    }

    /// <summary>
    /// Looping through zombies and Updating them
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void UpdateZombies(GameTime gameTime)
    {
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Update(gameTime, gameState, zombieHP, skyOpacity == 1f);
        }
    }

    /// <summary>
    /// Updating explosion animations for the ones that are activeMore actions
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update</param>
    private void UpdateExplosions(GameTime gameTime)
    {
        for (int i = 0; i < explosionAnims.Length; i++)
        {
            explosionAnims[i].Update(gameTime);
        }
    }

    #endregion

    #region Update Buttons

    /// <summary>
    /// Central method to consentrate the button updates for game. Button updates check for mouse clicks
    /// </summary>
    private void UpdateButtons()
    {
        UpdateWallButtons();
        UpdateArcherButtons();
        if (landminePrev.Update(mouse, prevMouse))
        {
            selBut = landminePrev;
        }
    }

    /// <summary>
    /// Updating wall placement buttons
    /// </summary>
    private void UpdateWallButtons()
    {
        for (int i = 0; i < wallPrevs.Length; i++)
        {
            if (wallPrevs[i].Update(mouse, prevMouse))
            {
                selBut = wallPrevs[i];
            }
        }
    }

    /// <summary>
    /// Updating archer tower placement buttons
    /// </summary>
    private void UpdateArcherButtons()
    {
        for (int i = 0; i < archerPrevs.Length; i++)
        {
            if (archerPrevs[i].Update(mouse, prevMouse))
            {
                selBut = archerPrevs[i];
            }
        }
    }

    #endregion
    
    #region Game Main Methods

    /// <summary>
    /// Main method that updates ANYTHING to do with the actual gameplay
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in each update. Used to update most objects</param>
    private void UpdateGame(GameTime gameTime)
    {
        // resetting zombie kill count per update
        zombiesKilledPerUpdate = 0;
        
        // Casting night sky every day night cycle
        DayNightCycle(gameTime);
        
        // Updating message manager
        messageManager.Update(gameTime);
        
        // Updating all objects in game
        UpdateObjects(gameTime);
        
        // Checking if any defence in preview
        CheckIfAnythingInPrev();
        
        // Updating defence preview buttons and checking if they're selected
        UpdateButtons();
        
        // Checking for cannon launch
        LaunchCannon();
        
        // Checking for placement for defences
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null)
            {
                defences[i].CheckPlacement(mouse, prevMouse);
            }
        }
        
        // Handling all of collisions
        Collisions();
        
        // Updating coins display and centering it
        dispCoins = $"${coins}";
        coinsPos.X = CenterWidth(HUDFont.MeasureString(dispCoins).X);
        
        // Updating demolish button and checking for demolition
        demolishBuilding.Update(mouse, prevMouse);
        Demolition();
        
        // Displaying how many zombies were killed per update
        if (zombiesKilledPerUpdate == 1)
        {
            messageManager.DisplayMessage("1 ZOMBIE KILLED", Color.Green, Color.Black);
        }
        else if (zombiesKilledPerUpdate > 1)
        {
            messageManager.DisplayMessage($"{zombiesKilledPerUpdate} ZOMBIES KILLED", Color.DarkGreen, Color.White);
        }
    }

    /// <summary>
    /// Main method to draw ANYTHING to do with the actual gameplay
    /// </summary>
    private void DrawGame()
    {
        // Drawing night sky overlay
        spriteBatch.Draw(nightBGImg, nightBGRec, Color.White * skyOpacity);
        
        // Drawing background
        spriteBatch.Draw(gameBgImg, gameBgRec, Color.White);
        
        // Drawing platform
        platform.Draw(spriteBatch);
                
        // Drawing king tower
        kingTower.Draw(spriteBatch, buildableRec.Center.X, Color.White);
        
        // Drawing zombies
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Draw(spriteBatch);
        }
        
        // Drawing defences
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null)
            {
                if (demolishing)
                {
                    defences[i].Draw(spriteBatch, buildableRec.Center.X, Color.Red);
                }
                else
                {
                    defences[i].Draw(spriteBatch, buildableRec.Center.X, Color.White);
                }
            }
        }

        // Drawing cannonballs with their explosions
        for (int i = 0; i < cannonballs.Length; i++)
        {
            if (cannonballs[i] != null)
            {
                cannonballs[i].Draw(spriteBatch);
            }
            
            explosionAnims[i].Draw(spriteBatch, Color.White);
        }
        
        // Drawing arrows
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] != null)
            {
                arrows[i].Draw(spriteBatch);
            }
        }
        
        // Drawing display messages
        messageManager.Draw(spriteBatch);
        
        // Drawing HUD
        DrawHud();
        
        // Drawing buildable area rectangle as just a single solid color
        spriteBatch.Draw(pixelImg, buildableRec, Color.Green * 0.5f);
    }

    #endregion

    #region Collisions

    /// <summary>
    /// Central method that includes everything to do with object collisions
    /// </summary>
    private void Collisions()
    {
        ZombieCollision();
        CannonCollision();
        ArrowCollision();
    }
    
    /// <summary>
    /// Loops through zombie array and checks for collisions with defences, updates king health if king tower is attacked
    /// </summary>
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

            // Checking for defences
            for (int j = 0; j < defences.Count; j++)
            {
                // Making sure wall isn't null and it is currently placed
                if (defences[j] != null && defences[j].IsPlaced())
                {
                    if (zombies[i].Rec.Intersects(defences[j].Hitbox))
                    {
                        // Dealing damage to wall
                        defences[j].HP = zombies[i].Attack(defences[j].HP);
                        
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
    
    /// <summary>
    /// Checking for cannonball collisions; checks for zombie collision with hitbox, plays explosion animation,
    /// and deletes cannonball
    /// </summary>
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
                    
                    // Keeping count of how many zombies were hit to max it to MAX_HIT
                    int count = 0;
                    
                    // Checking if any zombies were hit and damaging all that did (clamping max number of zombies per attack to 4 to balance)
                    for (int j = 0; j < zombies.Length; j++)
                    {
                        if (zombies[j].Rec.Intersects(cannonballs[i].Hitbox))
                        {
                            // Dealing damage to zombie and increasing count
                            zombies[j].HitZombie(cannonballs[i].Damage);
                            count++;
                        }

                        // Ending if max zombies were hit
                        if (count == Cannonball.MAX_HITS)
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
                    
                    // Playing cannon sound
                    PlaySound(cannonSnd, 0.15f);
                    
                    // Setting to null so it can be reused
                    cannonballs[i] = null;
                }
            }
        }
    }

    /// <summary>
    /// Checking for arrow collisions with zombies: deals damage to zombies and deletes arrow
    /// </summary>
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
                        zombies[j].HitZombie(arrows[i].Damage);
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

    /// <summary>
    /// Locates the nearest zombie to a defence by looping through zombie list and comparing to see which is closest
    /// </summary>
    /// <param name="towerRec">Rectangle of the defence that checks proximity</param>
    /// <param name="radius">The maximum distance the zombie can be from the tower to be tracked</param>
    /// <returns>Returns the position of the closest zombie, returns (5000, 5000) if no zombies are in proxy</returns>
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
                        // Storing the closest zombie's location
                        closestZombie = zombies[i].Rec.Location.ToVector2();
                    }
                }
            }
        }
        
        return closestZombie;
    }
    
    /// <summary>
    /// Launches cannonball. Checks if its currently day time and if any tower is currently being placed
    /// </summary>
    private void LaunchCannon()
    {
        // Checking again if any tower is in preview
        CheckIfAnythingInPrev();
        
        // Checking for king tower shots only at nighttime and when other stuff is not in preview
        if (skyOpacity == 1 && !isAnyInPreview)
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

    /// <summary>
    /// Checks if a defence can be placed in the current location without intersecting any other hitbox
    /// </summary>
    /// <param name="building">The defence that is being checked</param>
    /// <returns>Returns if the building can be placed in that spot or not</returns>
    private bool ValidPlacement(Rectangle building)
    {
        // Checking intersection with king tower
        if (building.Intersects(kingTower.Hitbox))
        {
            return false;
        }

        // Checking intersection with wall defences
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null && building.Intersects(defences[i].Hitbox) && defences[i].IsPlaced())
            {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// Drawing anything to do with the top hud during gameplay
    /// </summary>
    private void DrawHud()
    {
        // Drawing day count, kill count, time left in round, health with offset, and coins
        DrawWithShadow(HUDFont, dispDayCount, dayCountPos, Color.Yellow, Color.Black);
        DrawWithShadow(HUDFont, dispMobsKilled, mobsKilledPos, Color.IndianRed, Color.Black);
        DrawWithShadow(HUDFont, $"{(int)dayNightCycle.GetTimeRemaining() / 1000}", timeLeftDispPos, 
            Color.White, Color.Black);
        
        DrawWithShadow(HUDFont, dispKingHP, kingHPPos, Color.Green, Color.Black);
        DrawWithShadow(HUDFont, dispCoins, coinsPos, Color.Gold, Color.DarkGoldenrod);
        
        // Drawing demolish button
        demolishBuilding.Draw(spriteBatch, mouse.Position);
        
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
        
        // Drawing landmine tower preview with price
        DrawWithPrice(landminePrev, Landmine.GetDefaultPrice());
    }
    
    /// <summary>More actions
    /// Meant for drawing preview buttons with prices on them
    /// </summary>
    /// <param name="button">The button that has the price drawn on it</param>
    /// <param name="price">The price of the item that the button represents. > 0</param>
    private void DrawWithPrice(Button button, int price)
    {
        button.Draw(spriteBatch, mouse.Position);
        spriteBatch.DrawString(smallFont, $"${price}", button.Rec.Location.ToVector2(), Color. Gold);
    }
    
    /// <summary>
    /// Checking if any defence was clicked during demolition. If it was, defence is deleted
    /// </summary>
    private void Demolition()
    {
        // Proceeding if currently demolishing
        if (demolishing)
        {
            // Checking for mouse click
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                // Looping through all defences to see if mouse is on any of them
                for (int i = 0; i < defences.Count; i++)
                {
                    if (defences[i] != null && defences[i].Hitbox.Contains(mouse.Position))
                    {
                        // Calculating refund
                        int refund = (int)(defences[i].Price * defences[i].HPPercentage);
                        
                        // displaying message to user about demolish
                        messageManager.DisplayMessage($"DEFENCE DESTROYED | Refund: {refund}", Color.Red, Color.Black);
                        
                        // Destroying wall and giving refund
                        coins += refund;
                        defences.RemoveAt(i);
                        
                        // Playing destroyed building sound
                        PlaySound(defenceDestroyedSnd, 0.5f);
                        
                        // exiting demolishing state and ending method
                        demolishing = false;
                        return;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Increases zombie HP, amount of zombies, and night time length every night cycle
    /// </summary>
    private void ZombieWaves()
    {
        // Modifying max zombie count with respect to how many nights passed
        maxZombies = INITIAL_MAX_ZOMBIES + Convert.ToInt32(Math.Pow(dayCount, 1.2) * 2);
        if (maxZombies > 100)
        {
            maxZombies = 100;
        }
        
        // Adding 2 more seconds to each night that passes
        // Timer reset happens in day and night cycle
        nightTime += 5000;
        
        // Spawning zombies when its nighttime
        SpawnZombies();
        
        // Adding to zombie HP every 5 nights
        if (dayCount % 5 == 0)
        {
            zombieHP += 5;
        }
    }

    /// <summary>
    /// Spawns all zombies of maxZombies length
    /// </summary>
    private void SpawnZombies()
    {
        for (int i = 0; i < maxZombies; i++)
        {
            zombies[i].Spawn(gameState, zombieHP);
        }
    }
    
    /// <summary>
    /// Checks if any defence is being placed. True if no defences are in preview
    /// </summary>
    private void CheckIfAnythingInPrev()
    {
        // Checking if any tower is currently being placed
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null && !defences[i].IsPlaced())
            {
                isAnyInPreview = true;
                return;
            }
        }
        
        // If it passed all of them it must be valid so unselecting all buttons
        if (selBut != null)
        {
            selBut.Deselect();
            selBut = null;
        }
        
        isAnyInPreview = false;
    }
    
    /// <summary>
    /// Deselects all buttons
    /// </summary>
    private void AllButtonDeselect()
    {
        // Deselecting button
        selBut.Deselect();
        selBut = null;
        
        // Removing any tower in preview
        for (int i = 0; i < defences.Count; i++)
        {
            if (defences[i] != null && !defences[i].IsPlaced())
            {
                defences.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Resetting anything that changed through out the night
    /// </summary>
    private void ResetGame()
    {
        // Resetting kill and day count, nighttime length, coins, and zombie HP
        mobsKilled = 0;
        dayCount = 0;
        nightTime = INITIAL_NIGHTTIME;
        coins = INITIAL_COINS;
        zombieHP = INITIAL_ZOMBIE_HP;
        
        // Clearing defences list
        defences.Clear();
        
        // Resetting king tower HP
        kingTower.ResetHP();
    }

    #region Button Actions

    /// <summary>
    /// Setting game state to tutorial and reseting slideshow
    /// </summary>
    private void TutorialButton()
    {
        gameState = TUTORIAL;
        currentSlide = 0;
        
        // Shuffling music song
        ShuffleMusic();
    }
    
    /// <summary>
    /// Sends player to the level 1 game mode and prepares game
    /// </summary>
    private void Level1Button()
    {
        // Setting game state to level 1
        gameState = LEVEL_1;
        
        // Updating king position
        kingTower.TranslateTo(lvl1KingPos);
        
        // Modifying buildable rec to be the right third of the screen
        buildableRec.Width = screenWidth / 3;
        buildableRec.X = screenWidth - buildableRec.Width;
        
        // Resetting game
        ResetGame();
        
        // Starting day night cycle timer
        dayNightCycle.ResetTimer(true);
        
        // Shuffling music song
        ShuffleMusic();
    }

    /// <summary>
    /// Sends player to lvl 2 game mode and preps game
    /// </summary>
    private void Level2Button()
    {
        // Setting game state to level 2
        gameState = LEVEL_2;
        
        // Updating king position
        kingTower.TranslateTo(lvl2KingPos);
        
        // Modifying buildable rec to be the right third of the screen
        buildableRec.Width = screenWidth / 2;
        buildableRec.X = (int)CenterWidth(screenWidth / 2f);
        
        // Resetting game
        ResetGame();
        
        // Starting day night cycle timer
        dayNightCycle.ResetTimer(true);
        
        // Shuffling music song
        ShuffleMusic();
    }
    
    /// <summary>
    /// Action for when a button is clicked to place a wall
    /// </summary>
    /// <param name="lvl">The lvl of the wall that is being placed (between 1 and 2)</param>
    private void WallPrev(byte lvl)
    {
        // Checking if any tower is currently being placed
        if (!isAnyInPreview)
        {
            // Creating a new wall inside defences to be placed
            Texture2D wallImg = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Wall/WallLvl{lvl + 1}");
            defences.Add(new Wall(wallImg, wallImg.Width / 2, wallImg.Height / 2, 6, platform.Rec, lvl));

            // Selecting button
            wallPrevs[lvl].Select();
            selBut = wallPrevs[lvl];
        }
        else if (selBut != null && isAnyInPreview)
        {
            AllButtonDeselect();
        }
    }

    /// <summary>
    /// Action for when a button is clicked to place an archer tower
    /// </summary>
    /// <param name="lvl">The lvl of the archer tower that is being placed (between 1 and 3)</param>
    private void ArchPrev(byte lvl)
    {
        // Checking if any tower is currently being placed
        if (!isAnyInPreview)
        {
            // Creating a new archer tower instance to be placed
            Texture2D img = Content.Load<Texture2D>($"Images/Sprites/Gameplay/Archer/ArcherTowerLvl{lvl + 1}");
            Texture2D arrowImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Archer/Arrow");
            ArcherTower archerTower = new ArcherTower(img, new Vector2(0, platform.Rec.Top - img.Height / 2 + 5),
                img.Width / 2, img.Height / 2, img.Width / 2,
                img.Height / 2, arrowImg, lvl);
            defences.Add(archerTower);
            
            // Selecting button
            archerPrevs[lvl].Select();
            selBut = archerPrevs[lvl];
        }
        else if (selBut != null && isAnyInPreview)
        {
            AllButtonDeselect();
        }
    }

    /// <summary>
    /// Action for when a button is clicked to place a landmine
    /// </summary>
    private void LandminePrev()
    {
        // Checking if any tower is currently being placed
        if (!isAnyInPreview)
        {
            // Creating a new landmine instance to be placed and adding it to tower list
            Texture2D img = Content.Load<Texture2D>("Images/Sprites/Gameplay/LandMine");
            defences.Add(new Landmine(img, 2, platform.Rec.Top));
            
            // Selecting button
            landminePrev.Select();
            selBut = landminePrev;
        }
        else if (selBut != null && isAnyInPreview)
        {
            AllButtonDeselect();
        }
    }

    /// <summary>More actions
    /// Toggles demolishing state if button is clicked
    /// </summary>
    private void DemolishButton()
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

    /// <summary>
    /// Draws the stats of a wall when hovering over the wall preview button
    /// </summary>
    /// <param name="lvl">The level of the wall that is hovered over</param>
    private void WallPrevDrawHover(byte lvl)
    {
        // Drawing background for easier text read
        spriteBatch.Draw(pixelImg, new Rectangle(mouse.Position, 
                                    smallFont.MeasureString($"HP: {Wall.GetDefaultHP(lvl)}").ToPoint()), Color.White);
        
        // Drawing HP of wall
        spriteBatch.DrawString(smallFont, $"HP: {Wall.GetDefaultHP(lvl)}", mouse.Position.ToVector2(), Color.Green);
    }

    /// <summary>
    /// Draws the stats of an archer tower when hovering over the wall preview button
    /// </summary>
    /// <param name="lvl">The lvl of the archer tower that is hovered over</param>
    private void ArcherPrevDrawHover(byte lvl)
    {
        // Drawing background for easier text read
        spriteBatch.Draw(pixelImg, new Rectangle(mouse.Position.X, mouse.Position.Y, 
            (int)smallFont.MeasureString($"Cooldown Time: {ArcherTower.GetDefaultCooldownLength(lvl)}s").X,
                (int)(smallFont.MeasureString($"HP: {ArcherTower.GetDefaultHP(lvl)}").Y * 3)), Color.White);
        
        // Drawing stats of archer tower
        spriteBatch.DrawString(smallFont, $"HP: {ArcherTower.GetDefaultHP(lvl)}", mouse.Position.ToVector2(), Color.Green);
        spriteBatch.DrawString(smallFont, $"Damage: {ArcherTower.GetDefaultDamage(lvl)}", 
                        mouse.Position.ToVector2() + new Vector2(0, smallFont.MeasureString("1").Y), Color.Red);
        spriteBatch.DrawString(smallFont, $"Cooldown Time: {ArcherTower.GetDefaultCooldownLength(lvl)}s", 
            mouse.Position.ToVector2() + new Vector2(0, smallFont.MeasureString("1").Y * 2), Color.CornflowerBlue);
    }

    /// <summary>
    /// Draws the stats of a landmine when hovering over the wall preview button
    /// </summary>
    private void LandminePrevDrawHover()
    {
        // Drawing background for easier readability
        spriteBatch.Draw(pixelImg, new Rectangle(mouse.Position, 
                    smallFont.MeasureString($"Damage: {Landmine.GetDefaultDamage()}").ToPoint()), Color.White);
        
        // Drawing damage of landmine
        spriteBatch.DrawString(smallFont, $"Damage: {Landmine.GetDefaultDamage()}", mouse.Position.ToVector2(), Color.Red);
    }

    #endregion
    
    /// <summary>
    /// Creating sound instance of any sound and volume
    /// </summary>
    /// <param name="sound">The sound effect being played</param>
    /// <param name="soundVolume">The volume of the sound that is played, float, 0 to 1</param>
    public static void PlaySound(SoundEffect sound, float soundVolume)
    {
        // Playing chosen sound at default volume
        SoundEffectInstance snd = sound.CreateInstance();
        snd.Volume = soundVolume;
        snd.Play();
    }

    /// <summary>
    /// Randomizes a random index for the time span array which correspond to a time stamp of a new song
    /// </summary>
    private void ShuffleMusic()
    {
        // Starting game music by randomizing which song it starts on
        MediaPlayer.Stop();
        MediaPlayer.Play(gameMusic, gameMusicSongTimes[rng.Next(0, gameMusicSongTimes.Length)]);
    }
}