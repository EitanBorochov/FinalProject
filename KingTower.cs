// Author: Eitan Borochov
// File Name: KingTower.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: May 9th 2025
// Description: Handles everything to do with the king tower and its properties

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class KingTower
{
    #region Attributes

    // Storing images of the king tower, king, and cannons
    private Texture2D towerImg;
    
    // Storing rectangles of the king tower, king, cannons, and hitbox
    private Rectangle displayRec;
    private Rectangle hitbox;
    
    // Storing king tower HP
    private int towerHP = 1000;

    #endregion

    #region Constructors
    
    // Setting up constructor
    public KingTower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, int hitboxHeight)
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
    
    // Returns king HP
    public int GetTowerHP()
    {
        return towerHP;
    }
    
    // Setters:
    // Setting tower HP
    public void SetTowerHP(int towerHP)
    {
        if (towerHP >= 0)
        {
            this.towerHP = towerHP;
        }
    }

    #endregion

    #region Behaviours

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(towerImg, displayRec, Color.White);
    }

    #endregion
}