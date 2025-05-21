// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: May 21st 2025
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
    int screenWidth;
    int screenHeight;
    
    // Storing mouse input
    private MouseState mouse;
    private MouseState prevMouse;

    #region Game State Variables
    // Storing constant variable constants for game states
    const byte MENU = 0;
    const byte LEVEL_1 = 1;
    const byte LEVEL_2 = 2;
    const byte PAUSE = 3;
    const byte SETTINGS = 4;
    const byte TUTORIAL = 5;
    const byte ENDGAME = 6;
    
    // Storing current game state
    byte gameState = MENU;

    #endregion

    #region UI Variables

    // Storing sprite Fonts
    SpriteFont titleFont;
    SpriteFont HUDFont;
    SpriteFont textFont;
    
    // Storing background image and rec for menu
    Texture2D bgImg;
    Rectangle bgRec;
    
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

    // Storing platform
    Platform platform;
    
    // Storing king tower
    private Tower kingTower;
    
    // Storing the two king tower locaations for each level
    Vector2 lvl1KingPos;
    Vector2 lvl2KingPos;
    
    // Storing zombies array
    private Zombie[] zombies = new Zombie[20];
    
    // Storing timer for day night cycle
    Timer dayNightCycle = new Timer(5000, true);
    
    // Storing background night sky texture and rectangle
    public Texture2D nightBGImg;
    Rectangle nightBGRec;
    
    // Storing night sky color multiplier, incrase and decrease constants, and multiplier modifier
    float skyOpacity = 0f;
    const int POSITIVE = 1;
    const int NEGATIVE = -1;
    int skyMultiplier;

    #endregion
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

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
        textFont = Content.Load<SpriteFont>("Fonts/TextFont");
        
        // Loading night background texture and rectangle
        nightBGImg = Content.Load<Texture2D>("Images/Backgrounds/PixelNightSky");
        nightBGRec = new Rectangle(0, 0, nightBGImg.Width * 2, nightBGImg.Height * 2);
        
        // Loading background image and rectangle
        bgImg = Content.Load<Texture2D>("Images/Backgrounds/MenuBackground");
        bgRec = new Rectangle(0, 0, screenWidth, screenHeight);
        
        // Loading platform and its texture (defining texture localy as it will be used in the Platform class)
        Texture2D platformImg;
        platformImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/Brick");
        platform = new Platform(platformImg, screenWidth, screenHeight);
        
        // Loading king tower, position, & image (defining king tower image localy as it will be used in the Tower class)
        Texture2D kingTowerImg;
        kingTowerImg = Content.Load<Texture2D>("Images/Sprites/Gameplay/KingTower");
        kingTower = new Tower(kingTowerImg, nightBGRec.Location.ToVector2(), kingTowerImg.Width, 
                                kingTowerImg.Height, 266, 100);
        lvl1KingPos = new Vector2(screenWidth - kingTower.GetDisplayRec().Width / 2f, 
                                    platform.GetPlatformRec().Y - kingTower.GetHitbox().Height + 10);
        lvl2KingPos = new Vector2(WidthCenter(kingTower.GetHitbox().Width),
                                    platform.GetPlatformRec().Y - kingTower.GetHitbox().Height + 10);

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
        
        // Loading all of the zombies
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i] = new Zombie(zombieImgs);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        // Updating mouse input
        prevMouse = mouse;
        mouse = Mouse.GetState();
        
        switch (gameState)
        {
            case MENU:
                level1Button.Update(mouse, prevMouse);
                level2Button.Update(mouse, prevMouse);
                settingsButton.Update(mouse, prevMouse);
                tutorialButton.Update(mouse, prevMouse);
                
                break;
            
            case LEVEL_1:
                // Casting night sky every day night cycle
                DayNightCycle(gameTime);
                
                break;
            
            case LEVEL_2:
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
            }
            
            // Restarting the timer
            dayNightCycle.ResetTimer(true);
        }

        // Updating the sky opacity and clamping it
        skyOpacity += skyMultiplier * 0.01f;
        skyOpacity = MathHelper.Clamp(skyOpacity, 0, 1);
        
    }

    private void DrawGame()
    {
        // Drawing night sky overlay
        _spriteBatch.Draw(nightBGImg, nightBGRec, Color.White * skyOpacity);
        
        // Drawing platform
        platform.Draw(_spriteBatch);
                
        // Drawing king tower
        kingTower.Draw(_spriteBatch);
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
    }

    #endregion
    
}
