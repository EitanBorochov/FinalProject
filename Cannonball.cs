// Author: Eitan Borochov
// File Name: Cannonball.cs
// Project Name: FinalProject
// Creation Date: May 26th 2025
// Modification Date: June 4th 2025
// Description: Parent class for all projectiles

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Cannonball : Projectile
{
    #region Attributes

    // Storing a constant cannonball speed
    public const float LAUNCH_SPEED = 1000f;
    
    // Storing a constant max zombie hit (how many zombies can cannonball hit in one explosion)
    public const int MAX_HITS = 5;
    
    // Storing collision hitbox that is double the size by default
    private Rectangle hitbox;

    #endregion
    
    #region Constructor

    public Cannonball(Rectangle rec, Vector2 mousePos, Texture2D image, int damage, int hitboxMultiplier, float maxMag) : 
            base(rec, LAUNCH_SPEED, mousePos, image, damage, maxMag)
    {
        // Calculating hitbox
        hitbox = new Rectangle(rec.X - (rec.Width * (hitboxMultiplier - 1) ) / 2, rec.Y, rec.Width * hitboxMultiplier, rec.Height);
    }

    // Creating new constructor only for landmine explosions that just need the cannonball to spawn and explode immediately
    public Cannonball(Rectangle rec, Texture2D img, int damage, int hitboxMultiplier) : base(rec, LAUNCH_SPEED, rec.Location.ToVector2(), img, damage, 100)
    {
        // Calculating hitbox
        hitbox = new Rectangle(rec.X - (rec.Width * (hitboxMultiplier - 1) ) / 2, rec.Y, rec.Width * hitboxMultiplier, rec.Height);
    }

    #endregion

    #region Getters & Setters

    /// <summary>
    /// Hitbox property to be modified
    /// </summary>
    public Rectangle Hitbox
    {
        get => hitbox;
        set => hitbox = value;
    }
    
    #endregion
    
    #region Behaviours
    
    /// <summary>
    /// Translates hitbox. Base update
    /// </summary>
    /// <param name="timePassed">time passed in seconds</param>
    public override void Update(float timePassedSeconds)
    {
        base.Update(timePassedSeconds);
        hitbox.Location = new Point(rec.X - (hitbox.Width - rec.Width) / 2, rec.Y);
    }

    #endregion
}