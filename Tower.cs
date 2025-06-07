// Author: Eitan Borochov
// File Name: Tower.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: June 6th 2025
// Description: Handles everything to do with the towers and their properties

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Tower
{
    #region Attributes

    // Storing images of the king tower, king, and cannons
    protected Texture2D img;
    
    // Storing rectangles of the king tower, king, cannons, and hitbox
    protected Rectangle displayRec;
    protected Rectangle hitbox;
    
    // Storing tower HP and initial HP
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
    
    // storing lvl of tower for upgrades (starting at 0)
    protected byte lvl;
    
    // Storing tower price
    protected int price;
    
    // Storing health bar rectangle to be displayed on top of tower. One full and one empty
    protected Rectangle emptyHPBarRec;
    protected Rectangle fullHPBarRec;

    #endregion

    #region Constructors
    
    // Setting up constructors, one with projectiles and one without
    public Tower(Texture2D img, Vector2 position, int width, int height, 
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
    public Tower(Texture2D img, int width, int height)
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
        emptyHPBarRec = new Rectangle(hitbox.X, hitbox.Y - 20, hitbox.Width, 10);
        fullHPBarRec = emptyHPBarRec;
    }

    #endregion

    #region Getters & Setters

    // Getters:
    // Returns display rectangle
    public virtual Rectangle DisplayRec
    {
        get => displayRec;
    }
    
    // Returns hitbox rectangle
    public virtual Rectangle Hitbox
    {
        get => hitbox;
    }
    
    // Modifying tower HP property
    public virtual int HP
    {
        get => this.health;
        set => this.health = value;
    }
    
    // Returning damage
    public virtual int Damage
    {
        get => damage;
    }
    
    // Setting a new image for the object
    public virtual void SetImage(Texture2D img, int width, int height)
    {
        // Updating image
        this.img = img;
        
        // Updating width and height
        displayRec.Width = width;
        displayRec.Height = height;
    }
    
    // Returning price of current tower
    public int Price
    {
        get => price;
        set => price = value;
    }
    
    // Returning if tower is placed or not
    public bool IsPlaced()
    {
        if (state == PLACED)
        {
            return true;
        }

        return false;
    }

    // Allowing modification of tower lvl
    public byte Lvl
    {
        get => lvl;
        set
        {
            // Making sure lvl is between 1 and 3
            if (lvl <= 3 && lvl > 0)
            {
                lvl = value;
            }
        }
    }
    
    // Returning health %
    public float HPPercentage
    {
        get => (float)health / initialHealth;
    }

    #endregion

    #region Behaviours
    
    // Updating tower
    public virtual bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
        // Updating health bar
        fullHPBarRec.Width = (int)(emptyHPBarRec.Width * HPPercentage);
        
        // Translating health bar to always follow tower
        emptyHPBarRec.X = hitbox.X;
        emptyHPBarRec.Y = hitbox.Y - emptyHPBarRec.Height * 2;
        fullHPBarRec.Location = emptyHPBarRec.Location;
        
        // Returning that the tower is down
        if (health <= 0)
        {
            return true;
        }
        
        return false;
    }
    

    // Drawing Health bar above tower, tower will be drawn in each inherited class
    public virtual void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        // Drawing health bars:
        spriteBatch.Draw(Game1.pixelImg, emptyHPBarRec, Color.White);
        spriteBatch.Draw(Game1.pixelImg, fullHPBarRec, Color.Green);
    }
    
    public virtual void CheckPlacement(MouseState mouse, MouseState prevMouse,
        Rectangle platform) 
    {}
    
    // Overloading CheckPlacement for landmine
    public virtual bool CheckPlacement(MouseState mouse, MouseState prevMouse)
    {
        return false;
    }
    
    // Translate object
    public virtual void TranslateTo(Vector2 position)
    {
        // Translating hitbox
        hitbox.X = (int)position.X;
        hitbox.Y = (int)position.Y;
        
        // Updating display rec to match
        displayRec.X = hitbox.Center.X - displayRec.Width / 2; 
        displayRec.Y = hitbox.Bottom - displayRec.Height; 
    }

    #endregion
}