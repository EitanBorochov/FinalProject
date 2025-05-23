// Author: Eitan Borochov
// File Name: Tower.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: May 23rd 2025
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
    private Texture2D towerImg;
    
    // Storing rectangles of the king tower, king, cannons, and hitbox
    private Rectangle displayRec;
    private Rectangle hitbox;
    
    // Storing king tower HP
    private int health = 1000;

    #endregion

    #region Constructors
    
    // Setting up constructor
    public Tower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, int hitboxHeight)
    {
        // Storing temporary variables for calculations
        int hitboxX;
        int hitboxY;
        
        // Storing the given image in global variable
        this.towerImg = towerImg;
        
        // Creating new rectangle using the given parameters
        displayRec = new Rectangle((int)position.X, (int)position.Y, width, height);
        
        // Calculating hitbox coordinates to always be on the center bottom of the display rec
        hitboxX = displayRec.Center.X - hitboxWidth / 2; 
        hitboxY = displayRec.Bottom - hitboxHeight; 

        // Creating new rectangle for hitbox
        hitbox = new Rectangle(hitboxX, hitboxY, hitboxWidth, hitboxHeight);
    }

    #endregion

    #region Getters & Setters

    // Getters:
    // Returns display rectangle
    public Rectangle GetDisplayRec()
    {
        return displayRec;
    }
    
    // Returns hitbox rectangle
    public Rectangle GetHitbox()
    {
        return hitbox;
    }
    
    // Modifying tower HP property
    public int HP
    {
        get => this.health;
        set
        {
            if (value >= 0);
            {
                this.health = value;
            }
        }
    }
    
    // Setting a new image for the object
    public void SetImage(Texture2D towerImg, int width, int height)
    {
        // Updating image
        this.towerImg = towerImg;
        
        // Updating width and height
        displayRec.Width = width;
        displayRec.Height = height;
    }

    #endregion

    #region Behaviours
    
    // Updating tower
    public void Update()
    {
        // Checking if tower is alive of dead
        if (health <= 0)
        {
            TranslateTo(new Vector2(-1000, -1000));
        }
    }

    // Drawing tower
    public void Draw(SpriteBatch spriteBatch)
    {
        // Check if tower is alive
        if (health > 0)
        {
            spriteBatch.Draw(towerImg, displayRec, Color.White);
        }
    }
    
    // Returning the state of the tower (dead or alive)
    public bool IsAlive()
    {
        if (health > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // Translate object
    public void TranslateTo(Vector2 position)
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