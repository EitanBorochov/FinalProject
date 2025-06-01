// Author: Eitan Borochov
// File Name: Tower.cs
// Project Name: FinalProject
// Creation Date: May 28th 2025
// Modification Date: May 31st 2025
// Description: Creates a new wall object which is made of tiles and meant to block the enemy from the damage dealing towers

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Wall
{
    #region Attributes

    // Storing tile images for levels 1 and 2, and current level
    private Texture2D img;
    
    // Storing tile rec array and full rectangle
    private Rectangle[] tileRecs = new Rectangle[6];
    private Rectangle hitbox;
    
    // Storing health
    private int health;
    
    // Storing possible states for when its placed or not
    private const byte INACTIVE = 0;
    private const byte PREVIEW = 1;
    private const byte PLACED = 2;
    
    // Storing current state
    private byte state = PREVIEW;
    
    // Storing if the preview is in a valid location
    private bool isValid = false;
    
    // storing lvl of wall
    private byte lvl;
    
    // Storing price
    private int price;
    
    #endregion

    #region Constructor

    public Wall(Texture2D img, int tileWidth, int tileHeight, Platform platform, byte lvl)
    {
        // Storing inputted parameters
        this.img = img;
        this.health = health;
        this.lvl = lvl;
        
        // Constructing rectangles
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i] = new Rectangle(0, tileHeight * i, tileWidth, tileHeight);
        }
        
        // Loading hitbox
        hitbox = new Rectangle(tileRecs[0].X, tileRecs[0].Y, tileRecs[0].Width, tileRecs.Length * tileRecs[0].Height);
        TranslateY(platform.GetRec().Top - hitbox.Height);
        
        // Loading health according to level
        if (lvl == 1)
        {
            health = 400;
            price = 100;
        }
        else
        {
            health = 850;
            price = 200;
        }
    }

    #endregion

    #region Getters & Setters

    // Accessable health quantity
    public int HP
    {
        get => health;
        set => health = value;
    }

    public int Price
    {
        get => price;
        set => price = value;
    }

    // Returning a rectangle that surrounds all of the tiles
    public Rectangle GetRec()
    {
        return hitbox;
    }

    public bool IsPlaced()
    {
        if (state == PLACED)
        {
            return true;
        }

        return false;
    }

    public byte GetLvl()
    {
        return lvl;
    }

    #endregion

    #region Behaviours

    // Main update method that returns a true or false if the tower is down or not
    public bool Update(MouseState mouse, Rectangle buildableRec, bool isValid)
    {
        // Storing if current location is valid for placement by collision
        this.isValid = isValid;
        
        PreviewStateTranslation(mouse, buildableRec);

        if (health <= 0)
        {
            state = INACTIVE;
        }
        
        // Returning that the tower is down
        if (state == INACTIVE)
        {
            return true;
        }
        
        return false;
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

    public void CheckPlacement(MouseState mouse, MouseState prevMouse, 
        Rectangle platform)
    {
        if (state == PREVIEW)
        {
            if (isValid && Game1.Coins >= price)
            {
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    // placing tower
                    state = PLACED;
                    TranslateY(platform.Top - hitbox.Height);
                    
                    // Taking away coins
                    Game1.Coins -= price;
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

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < tileRecs.Length; i++)
        {
            // Drawing transparent preview state
            if (state == PREVIEW)
            {
                // Drawing red if invalid and regular if it is valid
                if (Game1.Coins < price || !isValid)
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
                spriteBatch.Draw(img, tileRecs[i], Color.White);
            }
        }
    }

    #endregion
}