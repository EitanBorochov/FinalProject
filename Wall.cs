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

public class Wall : Tower
{
    #region Attributes

    // Storing tile images for levels 1 and 2, and current level
    private Texture2D img;
    
    // Storing tile rec array and full rectangle
    private Rectangle[] tileRecs;
    
    // Storing a price array, the current price will be the price of index lvl
    private static int[] prices = {100, 300};
    
    // Storing health array, the current health will be the health of index lvl
    private int[] healths = {350, 800};
    private static readonly int[] INITIAL_HEALTHS = {350, 800};
    
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
        health = healths[lvl];
        initialHealth = INITIAL_HEALTHS[lvl];
        
        // Constructing rectangles
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i] = new Rectangle(0, tileHeight * i, tileWidth, tileHeight);
        }
        
        // Loading hitbox
        hitbox = new Rectangle(tileRecs[0].X, tileRecs[0].Y, tileRecs[0].Width, tileRecs.Length * tileRecs[0].Height);
        TranslateY(platformRec.Top - hitbox.Height);
    }

    #endregion

    #region Getters & Setters
    
    // Returning the default price at each lvl
    public static int GetDefaultPrice(int lvl)
    {
        return prices[lvl];
    }

    #endregion

    #region Behaviours

    // Main update method that returns a true or false if the tower is down or not
    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, bool isValid)
    {
        // Storing if current location is valid for placement by collision
        this.isValid = isValid;
        
        PreviewStateTranslation(mouse, buildableRec);

        return base.Update(gameTime, mouse, buildableRec, isValid);
    }

    // Allowing player to move the wall around while its in preview
    private void PreviewStateTranslation(MouseState mouse, Rectangle buildableRec)
    {
        if (state == PREVIEW)
        {
            // Translating X position to the mouse position if its in buildable area
            if (mouse.Position.ToVector2().X < buildableRec.Right - hitbox.Width
                && mouse.Position.ToVector2().X > buildableRec.Left)
            {
                TranslateX(mouse.Position.ToVector2().X);
            }
            else if (mouse.Position.ToVector2().X >= buildableRec.Right - hitbox.Width)
            {
                TranslateX(buildableRec.Right - hitbox.Width);
            }
            else
            {
                TranslateX(buildableRec.Left);
            }
        }
    }

    public override void CheckPlacement(MouseState mouse, MouseState prevMouse, 
        Rectangle platform)
    {
        if (state == PREVIEW)
        {
            // Checking for right click button cancel
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
            {
                // Killing tower and ending method
                healths[lvl] = 0;
                return;
            }

            // Checking if user has enough money and the placement is valid
            if (isValid && Game1.Coins >= prices[lvl])
            {
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    // placing tower
                    state = PLACED;
                    TranslateY(platform.Top - hitbox.Height);
                    
                    // Taking away coins
                    Game1.Coins -= prices[lvl];
                }
            }
        }
    }

    public void TranslateY(float yPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].Y = (int)yPos + tileRecs[i].Height * i;
        }

        // Offsetting collision rec
        hitbox.Y = tileRecs[0].Y;
    }
    public void TranslateX(float xPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].X = (int)xPos;
        }

        // Offsetting collision rec
        hitbox.X = tileRecs[0].X;
    }

    public override void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        for (int i = 0; i < tileRecs.Length; i++)
        {
            // Drawing transparent preview state
            if (state == PREVIEW)
            {
                // Drawing red if invalid and regular if it is valid
                if (Game1.Coins < prices[lvl] || !isValid)
                {
                    spriteBatch.Draw(img, tileRecs[i], Color.Red * 0.8f);
                }
                else if (isValid)
                {
                    spriteBatch.Draw(img, tileRecs[i], Color.White * 0.8f);
                }
            }
            // Drawing permenant state
            else if (state == PLACED)
            {
                spriteBatch.Draw(img, tileRecs[i], placedColor);
            }
        }
    }

    #endregion
}