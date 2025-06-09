// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: June 6th 2025
// Modification Date: June 6th 2025
// Description: Landmine will explode as soon as any zombie steps on it, can be placed anywhere, and is a 1 time use.
// For more efficient code, the landmine will be part of the tower family and tower list.

using System;
using System.Collections.Generic;
using System.Linq;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Landmine : Defence
{
    #region Attributes
    
    // Storing constant price
    private const int PRICE = 275;
    
    // Storing constant damage
    private const int DAMAGE = 10;
    
    #endregion

    #region Constructor

    public Landmine(Texture2D img, int scale, int platformTop) : base(img, img.Width * scale, img.Height * scale)
    {
        // Storing image from parameter
        this.img = img;
        
        // Storing parent damage and price
        damage = DAMAGE;
        price = PRICE;

        // TESTING
        health = 10;
        
        // Setting up rectangle
        hitbox = new Rectangle(0, platformTop - img.Height * scale, img.Width * scale, img.Height * scale);
        displayRec = hitbox;
    }

    #endregion

    #region Getters & Setters

    // Returning default price of this class
    public static int GetDefaultPrice()
    {
        return PRICE;
    }

    public static int GetDefaultDamage()
    {
        return DAMAGE;
    }
    
    #endregion

    #region Behaviours

    /* Main goal of update is to check for zombie collisions, if it detects any zombie, returns true which will spawn
     a cannonball in place of the landmine, the cannonball should explode immediately.*/
    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
        this.isValid = isValid;
        
        // Translating during preview
        PreviewStateTranslation(mouse, screenWidth);
        
        // Checking for collisions with zombies only if state is placed
        if (state == PLACED)
        {
            for (int i = 0; i < zombies.Length; i++)
            {
                // If a collision occured, returning true which sends the signal to summon a cannonball
                if (zombies[i].Rec.Intersects(hitbox))
                {
                    // Returning true to explode landmine
                    return true;
                }
            }
        }

        return base.Update(gameTime, mouse, buildableRec, isValid, screenWidth, zombies);
    }
    
    private void PreviewStateTranslation(MouseState mouse, int screenWidth)
    {
        // Checking if state is preview and placement is valid
        if (state == PREVIEW)
        {
            // Making sure mouse is in screen boundaries
            if (mouse.Position.X > 0 && mouse.Position.X < screenWidth - hitbox.Width)
            {
                // Translating bomb to mouse position
                hitbox.X = mouse.Position.X;
            }
        }
    }
    
    // Checks for placement of landmine, returns true if placement is cancelled
    public override void CheckPlacement(MouseState mouse, MouseState prevMouse)
    {
        if (state == PREVIEW)
        {
            // Checking for right click button cancel
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
            {
                // Returning true which means delete defence
                health = 0;
            }

            // Checking if user has enough money and the placement is valid
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                if (isValid && Game1.Coins >= price)
                {
                    // placing tower
                    state = PLACED;

                    // Taking away coins
                    Game1.Coins -= price;
                }
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        // Drawing transparent preview state
        if (state == PREVIEW)
        {
            // Drawing red if invalid and regular if it is valid
            if (Game1.Coins < price || !isValid)
            {
                spriteBatch.Draw(img, hitbox, Color.Red * 0.8f);
            }
            else if (isValid)
            {
                spriteBatch.Draw(img, hitbox, Color.White * 0.8f);
            }
        }
        else if (state == PLACED)
        {
            spriteBatch.Draw(img, hitbox, placedColor);
        }
    }

    #endregion
}