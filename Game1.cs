// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: May 9th 2025
// Description: Main file that handles the central logic for the game

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

    #region Game State Variables
    // Storing constant variable constants for game states
    const byte MENU = 0;
    const byte GAMEPLAY = 1;
    const byte PAUSE = 2;
    const byte SETTINGS = 3;
    const byte TUTORIAL = 4;
    const byte ENDGAME = 5;
    
    // Storing current game state
    byte gameState = GAMEPLAY;

    #endregion
    
    // Storing sprite Fonts
    SpriteFont titleFont;
    SpriteFont HUDFont;
    SpriteFont textFont;

    #region Day Night Cycle Variables
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
    
    // Storing king tower and its image
    private Texture2D kingTowerImg;
    private KingTower kingTower;

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
        
        // Loading king tower image
        kingTowerImg = Content.Load<Texture2D>("Images/Sprites/KingTower");
        
        // Loading king tower
        kingTower = new KingTower(kingTowerImg, nightBGRec.Location.ToVector2(), kingTowerImg.Width, kingTowerImg.Height, 266, 100);
    }

    protected override void Update(GameTime gameTime)
    {
        switch (gameState)
        {
            case MENU:
                
                
                break;
            
            case GAMEPLAY:
                // Casting night sky every day night cycle
                DayNightCycle(gameTime);
                
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
                
                
                break;
            
            case GAMEPLAY:
                // Drawing night sky overlay
                _spriteBatch.Draw(nightBGImg, nightBGRec, Color.White * skyOpacity);
                
                // Drawing king tower
                kingTower.Draw(_spriteBatch);
                
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
        
        // Finish sprite batch
        _spriteBatch.End();

        base.Draw(gameTime);
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
}
