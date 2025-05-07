// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: May 7th 2025
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
    
    // Storing sprite Fonts
    SpriteFont titleFont;
    SpriteFont HUDFont;
    SpriteFont textFont;
    
    // Storing timer for day night cycle
    Timer dayNightCycle = new Timer(10000, true);
    
    // Storing background night sky texture and rectangle
    Texture2D nightBGImg;
    Rectangle nightBGRec;
    
    // Storing night sky color multiplier, incrase and decrease constants, and multiplier modifier
    float skyOpacity = 0f;
    const int POSITIVE = 1;
    const int NEGATIVE = -1;
    int skyMultiplier;
    
    
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
    }

    protected override void Update(GameTime gameTime)
    {
        // Updating day night cycle
        dayNightCycle.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        DayNightCycle();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.RoyalBlue);

        // Initializing sprite drawing batch
        _spriteBatch.Begin();

        _spriteBatch.Draw(nightBGImg, nightBGRec, Color.White * skyOpacity);
        
        // Finish sprite batch
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DayNightCycle()
    {
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
            
            dayNightCycle.ResetTimer(true);
        }

        // Updating the sky opacity and clamping it
        skyOpacity += skyMultiplier * 0.01f;
        skyOpacity = MathHelper.Clamp(skyOpacity, 0, 1);
        
    }
}
