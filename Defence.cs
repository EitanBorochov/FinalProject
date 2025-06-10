// Author: Eitan Borochov
// File Name: defence.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: June 9th 2025
// Description: Handles everything to do with the defences and their properties

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Defence
{
    #region Attributes

    // Storing images of the king defence, king, and cannons
    protected Texture2D img;
    
    // Storing rectangles of the king defence, king, cannons, and hitbox
    protected Rectangle displayRec;
    protected Rectangle hitbox;
    
    // Storing defence HP and initial HP
    protected int health = 1;
    protected int initialHealth = 1;
    
    // Storing action timer
    protected Timer cooldownTimer;
    
    // Storing projectile image and rectangle
    protected Texture2D projImg;
    protected Rectangle projRec;
    
    // Storing damage
    protected int damage;
    
    // Storing possible states for when its placed or not
    protected const byte PREVIEW = 1;
    protected const byte PLACED = 2;
    
    // Storing current state
    protected byte state = PREVIEW;
    
    // Storing if the preview is in a valid location
    protected bool isValid;
    
    // storing lvl of defence for upgrades (starting at 0)
    protected byte lvl;
    
    // Storing defence price
    protected int price;
    
    // Storing health bar rectangle to be displayed on top of defence. One full and one empty
    protected Rectangle emptyHPBarRec;
    protected Rectangle fullHPBarRec;

    #endregion

    #region Constructors
    
    // Setting up constructors, one with projectiles and one without
    public Defence(Texture2D img, Vector2 position, int width, int height, 
        int hitboxWidth, int hitboxHeight, Texture2D projectileImg, int cooldownTimerLength)
    {
        // Storing temporary variables for calculations
        int hitboxX;
        int hitboxY;
        
        // Storing the given image in global variable
        this.img = img;
        
        // Creating new rectangle using the given parameters
        displayRec = new Rectangle((int)position.X, (int)position.Y, width, height);
        
        // Calculating hitbox coordinates to always be on the center bottom of the display rec
        hitboxX = displayRec.Center.X - hitboxWidth / 2; 
        hitboxY = displayRec.Bottom - hitboxHeight; 

        // Creating new rectangle for hitbox
        hitbox = new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
        
        // Constructing action timer
        this.cooldownTimer = new Timer(cooldownTimerLength, true);
        
        // Loading projectile data
        this.projImg = projectileImg;
        projRec = new Rectangle(0, 0, projImg.Width, projImg.Height);
        
        // Loading health bars
        LoadHPBars();
    }
    
    // Constructor for wall
    public Defence(Texture2D img, int width, int height)
    {
        // Storing the given image in global variable
        this.img = img;
        
        // Constructing hitbox
        hitbox = new Rectangle(0, 0, width, height);
        
        // Loading health bars
        LoadHPBars();
    }

    private void LoadHPBars()
    {
        // Loading health bars
        emptyHPBarRec = new Rectangle(displayRec.X, displayRec.Y - 20, hitbox.Width, 10);
        fullHPBarRec = emptyHPBarRec;
    }

    #endregion

    #region Getters & Setters
    
    /// <summary>
    /// Returns the display rectangle
    /// </summary>
    public virtual Rectangle DisplayRec => displayRec;

    /// <summary>
    /// Returns hitbox rectangle
    /// </summary>
    public virtual Rectangle Hitbox => hitbox;

    /// <summary>
    /// Modifying defence HP property
    /// </summary>
    public virtual int HP
    {
        get => health;
        set => health = value;
    }
    
    /// <summary>
    /// Returning damage that defence deals
    /// </summary>
    public virtual int Damage => damage;

    /// <summary>
    /// Returning price of current defence
    /// </summary>
    public int Price => price;

    /// <summary>
    /// Checks if defence is placed or not
    /// </summary>
    /// <returns>Returning if defence is placed or not</returns>
    public bool IsPlaced()
    {
        if (state == PLACED)
        {
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Returning the percentage of the remaining HP that the defence has
    /// </summary>
    public float HPPercentage => (float)health / initialHealth;

    #endregion

    #region Behaviours
    
    /// <summary>
    /// Basic update that all defences include
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates. Used for timer</param>
    /// <param name="mouse">Keeps track of state of mouse. Used for translation and placing</param>
    /// <param name="buildableRec">Area in which a tower can be placed on</param>
    /// <param name="isValid">Can a defence be placed at that location</param>
    /// <param name="screenWidth">Width of screen</param>
    /// <param name="zombies">Array of current zombies</param>
    /// <returns>Returns true if defence is dead (HP greater than 0)</returns>
    public virtual bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
        this.isValid = isValid;
        
        // Updating health bar
        fullHPBarRec.Width = (int)(emptyHPBarRec.Width * HPPercentage);
        
        // Translating during preview
        PreviewStateTranslation(mouse, buildableRec);
        
        // Returning that the defence is down
        if (health <= 0)
        {
            return true;
        }
        
        return false;
    }
    

    /// <summary>
    /// Drawing Health bar above defence, defence will be drawn in each inherited class
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    /// <param name="buildRecCenter">X center of the buildable area</param>
    /// <param name="placedColor">Color of tower when its placed</param>
    public virtual void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        // Drawing health bars:
        if (state == PLACED)
        {
            spriteBatch.Draw(Game1.pixelImg, emptyHPBarRec, Color.White);
            spriteBatch.Draw(Game1.pixelImg, fullHPBarRec, Color.Green);
        }
    }

    /// <summary>
    /// Checks if player attempts to place defence somewhere
    /// </summary>
    /// <param name="mouse"></param>
    /// <param name="prevMouse"></param>
    public virtual void CheckPlacement(MouseState mouse, MouseState prevMouse)
    {
        if (state == PREVIEW)
        {
            // Checking for right click button cancel
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
            {
                // Returning true which means delete landmine
                health = 0;
            }

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                // Checking if user has enough money and the placement is valid
                if (isValid && Game1.Coins >= price)
                {
                    // placing defence
                    state = PLACED;

                    // Taking away coins
                    Game1.Coins -= price;
                    
                    // Translating health bar to follow defence
                    emptyHPBarRec.X = hitbox.X;
                    emptyHPBarRec.Y = displayRec.Y - emptyHPBarRec.Height * 2;
                    fullHPBarRec.Location = emptyHPBarRec.Location;
                    
                    // Playing placing sound
                    Game1.PlaySound(Game1.defencePlacedSnd, 0.5f);

                    // Sending feedback to user, wall placed
                    Game1.messageManager.DisplayMessage($"DEFENCE PLACED", Color.Green, Color.Black);
                }
                else if (!isValid)
                {
                    // Sending feedback to user, invalid location
                    Game1.messageManager.DisplayMessage("INVALID PLACEMENT", Color.Red, Color.Black);
                }
                else if (Game1.Coins < price)
                {
                    // Sending feedback to user, not enough money
                    Game1.messageManager.DisplayMessage("NOT ENOUGH MONEY", Color.Red, Color.Black);
                }
            }
        }
    }
    
    /// <summary>
    /// Allowing player to move the archer tower around while its in preview
    /// </summary>
    /// <param name="mouse">Gives mouse position and if clicked or not</param>
    /// <param name="buildableRec">Area in which player can build in</param>
    protected virtual void PreviewStateTranslation(MouseState mouse, Rectangle buildableRec)
    {
        if (state == PREVIEW)
        {
            // Translating X position to the mouse position if its in buildable area
            if (mouse.Position.ToVector2().X < buildableRec.Right - hitbox.Width
                && mouse.Position.ToVector2().X > buildableRec.Left)
            {
                TranslateX(mouse.Position.ToVector2().X);
            }
            // If mouse is to the right of the buildableRec, snap to right
            else if (mouse.Position.ToVector2().X >= buildableRec.Right - hitbox.Width)
            {
                TranslateX(buildableRec.Right - hitbox.Width);
            }
            // If mouse is to the left of the buildableRec, snap to left
            else if (mouse.Position.ToVector2().X < buildableRec.Left)
            {
                TranslateX(buildableRec.Left);
            }
        }
    }
    
    /// <summary>
    /// Translate hitbox to chosen location
    /// </summary>
    /// <param name="position">Position of translation</param>
    public virtual void TranslateTo(Vector2 position)
    {
        // Translating hitbox
        hitbox.X = (int)position.X;
        hitbox.Y = (int)position.Y;
        
        // Updating display rec to match
        displayRec.X = hitbox.Center.X - displayRec.Width / 2; 
        displayRec.Y = hitbox.Bottom - displayRec.Height; 
    }

    /// <summary>
    /// Translating only X value
    /// </summary>
    /// <param name="xPos">Future X position of hitbox</param>
    public virtual void TranslateX(float xPos)
    {
        // Translating hitbox
        hitbox.X = (int)xPos;
        
        // Updating display rec to match
        displayRec.X = hitbox.Center.X - displayRec.Width / 2; 
    }

    /// <summary>
    /// Translating only Y value
    /// </summary>
    /// <param name="yPos">Future Y position of hitbox</param>
    public virtual void TranslateY(float yPos)
    {
        // Translating hitbox
        hitbox.X = (int)yPos;
        
        // Updating display rec to match
        displayRec.Y = hitbox.Bottom - displayRec.Height; 
    }

    #endregion
}