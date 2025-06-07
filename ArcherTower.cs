// Author: Eitan Borochov
// File Name: ArcherTower.cs
// Project Name: FinalProject
// Creation Date: June 2nd 2025
// Modification Date: June 5th 2025
// Description: Inhereted tower class specifically for the archer tower
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
    private static int[] prices = {150, 250, 475};
    
    // Healths
    private int[] healths = {150, 225, 400};
    private static readonly int[] INITIAL_HEALTHS = {150, 225, 400};
    
    // Damage & Attack related
    private static int[] damages = {2, 4, 7};
    private static int[] ranges = { 300, 350, 425 };
    private static int[] cooldownLengths = { 600, 550, 500 };

    #endregion

    #region Constructor

    // Basic constructor for the main Tower class attributes, with also lvl and price
    public ArcherTower(Texture2D img, Vector2 position, int width, int height, int hitboxWidth, int hitboxHeight, 
        Texture2D projectileImg, byte lvl) : 
        base(img, position, width, height, hitboxWidth, hitboxHeight, projectileImg, cooldownLengths[lvl])
    {
        // Storing array at index of level as main variable in parent class 
        this.lvl = lvl;
        health = healths[lvl];
        initialHealth = INITIAL_HEALTHS[lvl];
        damage = damages[lvl];
        price = prices[lvl];
    }

    #endregion

    #region Getters & Setters

    // Returns default prices
    public static int GetDefaultPrice(int lvl)
    {
        return prices[lvl];
    }
    
    public int Range
    {
        get => ranges[lvl];
    }
    
    #endregion

    #region Behaviours
    
    // Adding to base update
    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, 
                                bool isValid, int screenWidth, Zombie[] zombies)
    {
        // Storing if current location is valid for placement by collision
        this.isValid = isValid;
        
        PreviewStateTranslation(mouse, buildableRec);
        
        // Updating timer
        cooldownTimer.Update(gameTime);

        return base.Update(gameTime, mouse, buildableRec, isValid, screenWidth, zombies);
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
                hitbox.X = (int)mouse.Position.ToVector2().X;
            }
            // If mouse is to the right of the buildableRec, snap to right
            else if (mouse.Position.ToVector2().X >= buildableRec.Right - hitbox.Width)
            {
                hitbox.X = buildableRec.Right - hitbox.Width;
            }
            // If mouse is to the left of the buildableRec, snap to left
            else if (mouse.Position.ToVector2().X < buildableRec.Left)
            {
                hitbox.X = buildableRec.Left;
            }
            
            displayRec.Location = hitbox.Location;
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
                health = 0;
                return;
            }

            // Checking if user has enough money and the placement is valid
            if (isValid && Game1.Coins >= prices[lvl])
            {
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    // placing tower
                    state = PLACED;
                    hitbox.Y = platform.Top - hitbox.Height;

                    // Taking away coins
                    Game1.Coins -= prices[lvl];
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

    // Shooting arrows
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
                
                return new Arrow(projRec, nearestTarget, false, projImg, damage, ranges[lvl]);
            }
        }
    
        return null;
    }

    #endregion
}