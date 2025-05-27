// Author: Eitan Borochov
// File Name: Cannonball.cs
// Project Name: FinalProject
// Creation Date: May 26th 2025
// Modification Date: May 26th 2025
// Description: Parent class for all projectiles

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Cannonball : Projectile
{
    #region Constructor

    public Cannonball(Rectangle rec, Vector2 initialVel, Texture2D image, int damage) : base(rec, initialVel, image)
    {
        // Storing damage from tower object (for if there are upgrades and such)
        this.damage = damage;
    }

    #endregion

    #region Getters & Setters

    

    #endregion

    #region Behaviours

    public override void Update(float timePassed)
    {
        // Call parent update version
        base.Update(timePassed);
        
        // Updating rotation angle
        rotationAngle += 0.2f;
        
    }

    #endregion
}