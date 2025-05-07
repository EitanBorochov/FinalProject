// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 6th 2025
// Modification Date: May 7th 2025
// Description: Main file that handles the central logic for the game

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
    
    // Storing sky color modifier, sky aplha color, and dark sky color overlay
    int skyModif = 1;
    Color nightColor = Color.MidnightBlue;
    
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
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Initializing sprite drawing batch
        _spriteBatch.Begin();
        
        
        // Finish sprite batch
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DayNightCycle()
    {
        if (nightColor.A > 0 && nightColor.A < 1)
        {
            nightColor.A -= (byte)skyModif;

            if (dayNightCycle.IsFinished())
            {
                skyModif *= -1;
                
                dayNightCycle.ResetTimer(true);
            }
        }
    }
}
