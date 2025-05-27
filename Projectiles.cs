// Author: Eitan Borochov
// File Name: Projectiles.cs
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

public class Projectile
{
    #region Attributes
    
    // Global constant gravity
    protected const int GRAVITY = 10;

    // Storing rectangle and position
    protected Rectangle rec;
    protected Vector2 position;
    
    // Storing velocity
    protected Vector2 velocity;
    // protected Vector2 initialVel;
    
    // Storing damage that the projectile deals
    protected int damage;

    // Storing image, origin point, and rotation angle
    protected Texture2D image;
    protected Vector2 origin;
    protected float rotationAngle;
    
    // // Storing projectile state
    // protected const byte ACTIVE = 0;
    // protected const byte INACTIVE = 1;
    // protected byte state = INACTIVE;

    #endregion

    #region Contructor

    public Projectile(Rectangle rec, Vector2 initialVel, Texture2D image)
    {
        // Storing parameters
        this.rec = rec;
        this.velocity = initialVel;
        this.image = image;
        
        // Storing initial position
        position.X = rec.X;
        position.Y = rec.Y;
        
        // Calculating rotation angle *in radians, and origin point 
        rotationAngle = (float)Math.Atan2(initialVel.Y, initialVel.X);
        Vector2 origin = new Vector2(rec.Width / 2, rec.Height / 2);
    }

    #endregion

    #region Getters & Setters

    

    #endregion

    #region Behaviours

    public virtual void Update(float timePassed)
    {
        // Adding gravity to Y velocity 
        velocity.Y -= GRAVITY * timePassed;
    }

    // Drawing object
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, rec, null, Color.White, rotationAngle, origin, SpriteEffects.None, 0);
    }

    #endregion
}