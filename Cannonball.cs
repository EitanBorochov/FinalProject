// Author: Eitan Borochov
// File Name: Cannonball.cs
// Project Name: FinalProject
// Creation Date: May 26th 2025
// Modification Date: May 31st 2025
// Description: Parent class for all projectiles

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Cannonball : Projectile
{
    // Storing a constant cannon ball speed
    public const float LAUNCH_SPEED = 800f;
    
    // Storing collision hitbox that is double the size by default
    private Rectangle hitbox;
    
    #region Constructor

    public Cannonball(Rectangle rec, Vector2 mousePos, bool highAngle, Texture2D image, int damage, int hitboxMultiplier, float maxMag) : 
            base(rec, LAUNCH_SPEED, mousePos, highAngle, image, damage, maxMag)
    {
        // Calculating hitbox
        hitbox = new Rectangle(rec.X - (rec.Width * (hitboxMultiplier - 1) ) / 2, rec.Y, rec.Width * hitboxMultiplier, rec.Height);
    }

    #endregion

    #region Getters & Setters

    // Returning hitbox
    public Rectangle GetHitbox()
    {
        return hitbox;
    }

    // Setting hitbox width
    public void SetHitbox(int hitboxMultiplier)
    {
        hitbox.Width = rec.Width * hitboxMultiplier;
    }
    
    #endregion
    
    #region Behaviours
    
    public override void Update(float timePassed)
    {
        base.Update(timePassed);
        hitbox.Location = new Point(rec.X - (hitbox.Width - rec.Width) / 2, rec.Y);
    }

    #endregion
}