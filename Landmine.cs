// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: June 6th 2025
// Modification Date: June 9th 2025
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
    private const int DAMAGE = 20;
    
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

    /// <summary>
    /// Returns default price of landmine
    /// </summary>
    /// <returns>Price of landmine</returns>
    public static int GetDefaultPrice()
    {
        return PRICE;
    }

    /// <summary>
    /// Returns default damage of landmine
    /// </summary>
    /// <returns>Damage of landmine</returns>
    public static int GetDefaultDamage()
    {
        return DAMAGE;
    }
    
    #endregion

    #region Behaviours

    /// <summary>
    /// Base Update with also checking for zombie collision
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates. Used for timer</param>
    /// <param name="mouse">Keeps track of state of mouse. Used for translation and placing</param>
    /// <param name="buildableRec">Area in which a tower can be placed on</param>
    /// <param name="isValid">Can a defence be placed at that location</param>
    /// <param name="screenWidth">Width of screen</param>
    /// <param name="zombies">Array of current zombies</param>
    /// <returns>If returns true, landmine should explode</returns>
    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
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
    
    /// <summary>
    /// Overriding default translation since landmine doesn't have bounds
    /// </summary>
    /// <param name="mouse">Gives mouse position and if clicked or not</param>
    /// <param name="buildableRec">Area in which player can build in</param>
    protected override void PreviewStateTranslation(MouseState mouse, Rectangle buildableRec)
    {
        // Checking if state is preview and placement is valid
        if (state == PREVIEW)
        {
            // Making sure mouse is in screen boundaries
            if (mouse.Position.X > 0 && mouse.Position.X < Game1.screenWidth - hitbox.Width)
            {
                // Translating bomb to mouse position
                hitbox.X = mouse.Position.X;
            }
        }
    }

    /// <summary>
    /// Drawing Landmine red or white depending on state and location
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    /// <param name="buildRecCenter">X center of the buildable area</param>
    /// <param name="placedColor">Color of tower when its placed</param>
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