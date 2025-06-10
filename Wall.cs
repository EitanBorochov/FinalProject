// Author: Eitan Borochov
// File Name: Wall.cs
// Project Name: FinalProject
// Creation Date: May 28th 2025
// Modification Date: June 5th 2025
// Description: Creates a new wall object which is made of tiles and meant to block the enemy from the damage dealing towers

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Wall : Defence
{
    #region Attributes
    
    // Storing tile rec array and full rectangle
    private Rectangle[] tileRecs;
    
    // Storing a price array, the current price will be the price of index lvl
    private static readonly int[] PRICES = {100, 300};
    
    // Storing health array, the current health will be the health of index lvl
    private static readonly int[] HEALTHS = {350, 800};
    
    #endregion

    #region Constructor

    public Wall(Texture2D img, int tileWidth, int tileHeight, int numTiles, Rectangle platformRec, byte lvl) : 
        base(img, tileWidth, tileHeight * numTiles)
    {
        // Storing inputted parameters
        this.img = img;
        this.lvl = lvl;

        // Defining tile recs length
        tileRecs = new Rectangle[numTiles];
        
        // Storing health to health of chosen lvl
        health = HEALTHS[lvl];
        initialHealth = HEALTHS[lvl];
        
        // Storing price to price of chosen lvl
        price = PRICES[lvl];
        
        // Constructing rectangles
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i] = new Rectangle(0, tileHeight * i, tileWidth, tileHeight);
        }
        
        // Loading hitbox
        hitbox = new Rectangle(tileRecs[0].X, tileRecs[0].Y, tileRecs[0].Width, tileRecs.Length * tileRecs[0].Height);
        displayRec = hitbox;
        TranslateY(platformRec.Top - hitbox.Height);
    }

    #endregion

    #region Getters & Setters
    
    /// <summary>
    /// Returns default price for any lvl of tower
    /// </summary>
    /// <param name="lvl">Level of tower</param>
    /// <returns>Price of tower with level lvl</returns>
    public static int GetDefaultPrice(int lvl)
    {
        return PRICES[lvl];
    }
    
    /// <summary>
    /// Returns default HP for any lvl of tower
    /// </summary>
    /// <param name="lvl">Level of tower</param>
    /// <returns>HP of tower with level lvl</returns>
    public static int GetDefaultHP(int lvl)
    {
        return HEALTHS[lvl];
    }

    #endregion

    #region Behaviours

    /// <summary>
    /// Overriding TranslateY from Defence.cs as wall has to translate each tile seperately
    /// </summary>
    /// <param name="yPos">Future y position of top tile</param>
    public override void TranslateY(float yPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].Y = (int)yPos + tileRecs[i].Height * i;
        }

        // Offsetting collision rec
        hitbox.Y = tileRecs[0].Y;
        
        // Updating display rec
        displayRec.Y = hitbox.Y;
    }
    
    /// <summary>
    /// Overriding TranslateX from Defence.cs as wall has to translate each tile seperately
    /// </summary>
    /// <param name="xPos">Future x position of all tiles</param>
    public override void TranslateX(float xPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].X = (int)xPos;
        }

        // Offsetting collision rec
        hitbox.X = tileRecs[0].X;
        
        // Updating display rec
        displayRec.X = hitbox.X;
    }

    /// <summary>
    /// Drawing each tile separately based on the state of the wall
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    /// <param name="buildRecCenter">X center of the buildable area</param>
    /// <param name="placedColor">Color of tower when its placed</param>
    public override void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        for (int i = 0; i < tileRecs.Length; i++)
        {
            // Drawing transparent preview state
            if (state == PREVIEW)
            {
                // Drawing red if invalid and regular if it is valid
                if (Game1.Coins < PRICES[lvl] || !isValid)
                {
                    spriteBatch.Draw(img, tileRecs[i], Color.Red * 0.8f);
                }
                else if (isValid)
                {
                    spriteBatch.Draw(img, tileRecs[i], Color.White * 0.8f);
                }
            }
            // Drawing permanent state
            else if (state == PLACED)
            {
                spriteBatch.Draw(img, tileRecs[i], placedColor);
            }
        }
        
        // Base draws health bar on top of tower
        base.Draw(spriteBatch, buildRecCenter, placedColor); 
    }

    #endregion
}