// Author: Eitan Borochov
// File Name: ArcherTower.cs
// Project Name: FinalProject
// Creation Date: June 2nd 2025
// Modification Date: June 2nd 2025
// Description: Inhereted tower class specifically for the archer tower
using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class ArcherTower : Tower
{
    #region Attributes

    // Storing possible states for when its placed or not
    private const byte INACTIVE = 0;
    private const byte PREVIEW = 1;
    private const byte PLACED = 2;
    
    // Storing current state
    private byte state = PREVIEW;
    
    // Storing if the preview is in a valid location
    private bool isValid = false;
    
    // storing lvl of tower for upgrades (starting at 0)
    private byte lvl;
    
    // Storing arrays for damage, price, and health that are dependent on the current lvl (prices is static since it needs to be globally accessible)
    private static int[] prices = new[]{150, 250, 475};
    private int[] healths = new []{250, 400, 650};
    private int[] damages = new []{3, 5, 7};

    #endregion

    #region Constructor

    // Basic constructor for the main Tower class attributes, with also lvl and price
    public ArcherTower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, int hitboxHeight, 
        Texture2D projectileImg, int cooldownLength, byte lvl) : 
        base(towerImg, position, width, height, hitboxWidth, hitboxHeight, projectileImg, cooldownLength)
    {
        // Storing parameters
        this.lvl = lvl;
        health = healths[lvl];
        damage = damages[lvl];
    }

    #endregion

    #region Getters & Setters

    public static int GetPrice(int lvl)
    {
        return prices[lvl];
    }
    
    public bool IsPlaced()
    {
        if (state == PLACED)
        {
            return true;
        }

        return false;
    }

    public byte Lvl
    {
        get => lvl;
        set => lvl = value;
    }

    #endregion

    #region Behaviours
    
    // Adding to base update
    public bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, bool isValid)
    {
        // Base update:
        base.Update(mouse, gameTime);
        
        // Storing if current location is valid for placement by collision
        this.isValid = isValid;
        
        PreviewStateTranslation(mouse, buildableRec);

        if (healths[lvl] <= 0)
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
                hitbox.X = (int)mouse.Position.ToVector2().X;
            }
            else if (mouse.Position.ToVector2().X >= buildableRec.Right - hitbox.Width)
            {
                hitbox.X = buildableRec.Right - hitbox.Width;
            }
            else
            {
                hitbox.X = buildableRec.Left;
            }
            
            displayRec.Location = hitbox.Location;
        }
    }

    public void CheckPlacement(MouseState mouse, MouseState prevMouse,
        Rectangle platform)
    {
        if (state == PREVIEW)
        {
            // Checking for right click button cancel
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
            {
                // Setting state to inactive and ending method
                state = INACTIVE;
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

    public void Draw(SpriteBatch spriteBatch, int buildRecCenter)
    {
        // Drawing transparent preview state
        if (state == PREVIEW)
        {
            // Drawing red if invalid and regular if it is valid
            if (Game1.Coins < prices[lvl] || !isValid)
            {
                // Drawing flipped if it's on the left side of the screen
                if (hitbox.Center.X > buildRecCenter)
                {
                    spriteBatch.Draw(towerImg, displayRec, Color.Red * 0.8f);
                }
                else
                {
                    spriteBatch.Draw(towerImg, displayRec, null, Color.Red * 0.8f, 0, 
                        Vector2.Zero, SpriteEffects.FlipHorizontally, 1);
                }
            }
            else if (isValid)
            {
                // Drawing flipped if it's on the left side of the screen
                if (hitbox.Center.X > buildRecCenter)
                {
                    spriteBatch.Draw(towerImg, displayRec, Color.White * 0.8f);
                }
                else
                {
                    spriteBatch.Draw(towerImg, displayRec, null, Color.White * 0.8f, 0, 
                        Vector2.Zero, SpriteEffects.FlipHorizontally, 1);
                }
            }
        }
        else if (state == PLACED)
        {
            // Drawing it regularly if state is placed
            base.Draw(spriteBatch);
        }
    }

    // Shooting arrows
    // public Cannonball LaunchBall(Vector2 mousePos)
    // {
    //     if (cooldownTimer.IsFinished())
    //     {
    //         if (mousePos.X < hitbox.Left)
    //         {
    //             // Setting it on top RIGHT of tower
    //             projRec.Location = hitbox.Location;
    //         }
    //         else if (mousePos.X > hitbox.Right)
    //         {
    //             // Setting it to top LEFT of tower
    //             projRec.Location = new Point(hitbox.Right, hitbox.Top);
    //         }
    //         else
    //         {
    //             return null;
    //         }
    //
    //         cooldownTimer.ResetTimer(true);
    //
    //         return new Cannonball(projRec, mousePos, false, projImg, damage, 5, 350);
    //     }
    //
    //     return null;
    // }

    #endregion
}