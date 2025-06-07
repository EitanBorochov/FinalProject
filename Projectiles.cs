// Author: Eitan Borochov
// File Name: Projectiles.cs
// Project Name: FinalProject
// Creation Date: May 26th 2025
// Modification Date: June 6th 2025
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

    // Storing rectangle and position
    protected Rectangle rec;
    protected Vector2 position;
    
    // Storing velocity
    protected Vector2 velocity;
    
    // Storing damage that the projectile deals
    protected int damage;

    // Storing image, origin point, and rotation angle
    protected Texture2D image;

    #endregion

    #region Contructor

    public Projectile(Rectangle rec, float launchSpeed, Vector2 mousePos, bool highAngle, Texture2D image, int damage, float maxMag)
    {
        // Storing parameters
        this.rec = rec;
        velocity = FindLaunchVelocity(rec.Location.ToVector2(), mousePos, highAngle, launchSpeed, maxMag);
        this.image = image;
        
        // Storing initial position
        position.X = rec.X;
        position.Y = rec.Y;
        
        // Storing damage
        this.damage = damage;
    }

    #endregion

    #region Getters & Setters

    public Rectangle Rec
    {
        get => rec;
    }

    public int Damage
    {
        get => damage;
    }

    #endregion
    
    #region Behaviours

    public virtual void Update(float timePassed)
    {
        // Adding gravity to Y velocity 
        velocity.Y -= Game1.GRAVITY * timePassed;
        
        // Translating projectile
        position.X += velocity.X * timePassed;
        position.Y += velocity.Y * timePassed;

        rec.X = (int)position.X;
        rec.Y = (int)position.Y;
    }

    // Drawing object
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, rec, Color.White);
    }
    
    // Calculating the initial velocity of any projectile
    private Vector2 FindLaunchVelocity(Vector2 projPos, Vector2 mousePos, bool highAngle, float launchSpeed, float maxMag)
    { 
        // Thank you, Mr. Lane
        //Store the calculated launch velocity to hit the given target location
        Vector2 vel = new Vector2(0,0);

        //Find the difference between the two points
        Vector2 diff = (mousePos - projPos);

        //Precalculate speed^2 and speed^4 to reduce repeated calculations
        //This is cheaper than using Math.Pow
        double speed2 = launchSpeed * launchSpeed;
        double speed4 = launchSpeed * launchSpeed * launchSpeed * launchSpeed;

        //Precalculate gravity * x to reduce repeated calculations
        double gx = Game1.GRAVITY * diff.X;
        
        // Clamping diff x and y to the maximum magnitude if the difference excedes the max magnitude
        if (diff.Length() > maxMag)
        {
            diff.X *= maxMag / diff.Length();
            diff.Y *= maxMag / diff.Length();
        }
        
        // Calculate the Discriminant (value under the root): s^4 - G(Gx^2 + 2s^2y)
        double root = speed4 - Game1.GRAVITY * (Game1.GRAVITY * diff.X * diff.X + 2 * speed2 * diff.Y);

        //Solutions only exist if the root is positive, otherwise do not launch
        if (root > 0)
        {
            //Take the squareroot of the discriminant
            root = Math.Sqrt(root);

            //Calculate both the high and low angle by finished the quadratic formula for each of +/-
            //and finally calculate the arctan
            float projHighAngle = (float)Math.Atan2(speed2 + root, -gx);
            float projLowAngle = (float)Math.Atan2(speed2 - root, -gx); ;

            if (highAngle)
            {
                //Launch at the high angle at a speed measured in pixels/second
                vel.Y = -(float)(launchSpeed * Math.Sin(projHighAngle));
                vel.X = (float)(launchSpeed * Math.Cos(projHighAngle));
            }
            else
            {
                //Launch at the low angle at a speed measured in pixels/second
                vel.Y = -(float)(launchSpeed * Math.Sin(projLowAngle));
                vel.X = (float)(launchSpeed * Math.Cos(projLowAngle));
            }
        }

        return vel;
    }


    #endregion
}