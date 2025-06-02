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

    #endregion

    #region Behaviours

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