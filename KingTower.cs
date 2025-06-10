// Author: Eitan Borochov
// File Name: KingTower.cs
// Project Name: FinalProject
// Creation Date: May 27th 2025
// Modification Date: June 9th 2025
// Description: Inhereted tower class for the king tower

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FinalProject;

public class KingTower : Defence
{
    #region Constructor

    public KingTower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, 
                    int hitboxHeight, Texture2D projectileImg, int cooldownLength) : 
                    base(towerImg, position, width, height, hitboxWidth, hitboxHeight, projectileImg, cooldownLength)
    {
        // Storing health and damage
        health = 1000;
        initialHealth = health;
        damage = 4;
        
        // State is placed from the start
        state = PLACED;
    }

    #endregion

    #region Behaviours

    /// <summary>
    /// Updates cooldown timer
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates. Used for timer</param>
    /// <param name="mouse">Keeps track of state of mouse. Used for translation and placing</param>
    /// <param name="buildableRec">Area in which a tower can be placed on</param>
    /// <param name="isValid">Can a defence be placed at that location</param>
    /// <param name="screenWidth">Width of screen</param>
    /// <param name="zombies">Array of current zombies</param>
    /// <returns>Returns true if tower is dead (HP greater than 0)</returns>
    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
        // Updating cooldown timer
        cooldownTimer.Update(gameTime);
        
        return base.Update(gameTime, mouse, buildableRec, isValid, screenWidth, zombies);
    }

    /// <summary>
    /// Drawing tower
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    /// <param name="buildRecCenter">X center of the buildable area</param>
    /// <param name="placedColor">Color of tower when its placed</param>
    public override void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        spriteBatch.Draw(img, displayRec, placedColor);
        
        base.Draw(spriteBatch, buildRecCenter, placedColor);
    }

    /// <summary>
    /// Launching cannon ball
    /// </summary>
    /// <param name="mousePos">Position of current mouse acting as target</param>
    /// <returns>Returns a cannonball heading towards mouse position</returns>
    public Cannonball LaunchBall(Vector2 mousePos)
    {
        if (cooldownTimer.IsFinished())
        {
            if (mousePos.X < hitbox.Left)
            {
                // Setting it on top RIGHT of tower
                projRec.Location = hitbox.Location;
            }
            else if (mousePos.X > hitbox.Right)
            {
                // Setting it to top LEFT of tower
                projRec.Location = new Point(hitbox.Right, hitbox.Top);
            }
            else
            {
                return null;
            }

            cooldownTimer.ResetTimer(true);
            
            return new Cannonball(projRec, mousePos, projImg, damage, 5, 350);
        }

        return null;
    }

    #endregion
}