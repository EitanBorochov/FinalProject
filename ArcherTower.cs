// Author: Eitan Borochov
// File Name: ArcherTower.cs
// Project Name: FinalProject
// Creation Date: June 2nd 2025
// Modification Date: June 9th 2025
// Description: Inherited tower class specifically for the archer tower
using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;    

namespace FinalProject;

public class ArcherTower : Defence
{
    #region Attributes
    
    // Storing parallel arrays for damage, price, shooting range, cooldown timer, and health
    // that are dependent on the current lvl (prices is static since it needs to be globally accessible)
    // Prices
    private static readonly int[] PRICES = {150, 250, 475};
    
    // Healths
    private static readonly int[] HEALTHS = {150, 225, 400};
    
    // Damage & Attack related
    private static readonly int[] DAMAGES = {2, 4, 7};
    private static readonly int[] RANGES = { 350, 400, 475 };
    private static readonly int[] COOLDOWN_LENGTHS = { 600, 550, 500 };

    #endregion

    #region Constructor

    // Basic constructor for the main Tower class attributes, with also lvl and price
    public ArcherTower(Texture2D img, Vector2 position, int width, int height, int hitboxWidth, int hitboxHeight, 
        Texture2D projectileImg, byte lvl) : 
        base(img, position, width, height, hitboxWidth, hitboxHeight, projectileImg, COOLDOWN_LENGTHS[lvl])
    {
        // Storing array at index of level as main variable in parent class 
        this.lvl = lvl;
        health = HEALTHS[lvl];
        initialHealth = HEALTHS[lvl];
        damage = DAMAGES[lvl];
        price = PRICES[lvl];
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

    /// <summary>
    /// Returns default damage for any lvl of tower
    /// </summary>
    /// <param name="lvl">Level of tower</param>
    /// <returns>damage of tower with level lvl</returns>
    public static int GetDefaultDamage(int lvl)
    {
        return DAMAGES[lvl];
    }

    /// <summary>
    /// Returns default cooldown length for any lvl of tower
    /// </summary>
    /// <param name="lvl">Level of tower</param>
    /// <returns>cooldown length of tower with level lvl, float seconds</returns>
    public static float GetDefaultCooldownLength(int lvl)
    {
        return (float)Math.Round(COOLDOWN_LENGTHS[lvl] / 1000f, 2);
    }
    
    /// <summary>
    /// Returns range of current tower instance
    /// </summary>
    public int Range => RANGES[lvl];

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
        // Updating timer
        cooldownTimer.Update(gameTime);

        return base.Update(gameTime, mouse, buildableRec, isValid, screenWidth, zombies);
    }

    /// <summary>
    /// Draws the archer tower in its different conditions
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
            if (Game1.Coins < price|| !isValid)
            {
                // Drawing flipped if it's on the left side of the screen
                if (hitbox.Center.X > buildRecCenter)
                {
                    spriteBatch.Draw(img, displayRec, Color.Red * 0.8f);
                }
                else
                {
                    spriteBatch.Draw(img, displayRec, null, Color.Red * 0.8f, 0, 
                        Vector2.Zero, SpriteEffects.FlipHorizontally, 1);
                }
            }
            else if (isValid)
            {
                // Drawing flipped if it's on the left side of the screen
                if (hitbox.Center.X > buildRecCenter)
                {
                    spriteBatch.Draw(img, displayRec, Color.White * 0.8f);
                }
                else
                {
                    spriteBatch.Draw(img, displayRec, null, Color.White * 0.8f, 0, 
                        Vector2.Zero, SpriteEffects.FlipHorizontally, 1);
                }
            }
        }
        else if (state == PLACED)
        {
            // Drawing flipped if it's on the left side of the screen
            if (hitbox.Center.X > buildRecCenter)
            {
                spriteBatch.Draw(img, displayRec, placedColor);
            }
            else
            {
                spriteBatch.Draw(img, displayRec, null, placedColor, 0, 
                    Vector2.Zero, SpriteEffects.FlipHorizontally, 1);
            }
        }
        
        // Base draws health bar on top of tower
        base.Draw(spriteBatch, buildRecCenter, placedColor);
    }

    /// <summary>
    /// Shoots an arrow when the cooldown timer is done
    /// </summary>
    /// <param name="nearestTarget">Position of nearest target, where to shoot</param>
    /// <returns>Returns an arrow to main class that flies towards target</returns>
    public Arrow ShootArrow(Vector2 nearestTarget)
    {
        // Firing arrows every time cooldown timer goes off
        if (cooldownTimer.IsFinished() && state == PLACED)
        {
            // Reactivating timer
            cooldownTimer.ResetTimer(true);
            
            // Checking if nearest target is in range
            if (nearestTarget != new Vector2(5000, 5000))
            {
                // Firing at nearest target:
                // Setting projectile rectangle location to be top center
                projRec.Location = new Point(displayRec.Center.X, displayRec.Y);
                
                // Playing arrow sound
                Game1.PlaySound(Game1.arrowSnd, 0.1f);
                
                return new Arrow(projRec, nearestTarget, projImg, damage, RANGES[lvl]);
            }
        }
    
        return null;
    }

    #endregion
}