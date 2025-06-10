// Author: Eitan Borochov
// File Name: Arrow.cs
// Project Name: FinalProject
// Creation Date: June 4th 2025
// Modification Date: June 9th 2025
// Description: Inherited projectile class for arrows specifically

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Arrow : Projectile
{
    #region Attributes

    // Storing a constant arrow speed
    private const float LAUNCH_SPEED = 850f;
    
    // Storing rotation angle and origin
    private float rotationAngle;
    private Vector2 origin;

    #endregion
    
    #region Constructor

    public Arrow(Rectangle rec, Vector2 mousePos, Texture2D image, int damage, float maxMag) : 
        base(rec, LAUNCH_SPEED, mousePos, image, damage, maxMag)
    {
        // Making the size of the arrow smaller since the image is already very large
        this.rec.Width /= 10;
        this.rec.Height /= 10;
        
        origin.X = image.Width / 2f;
        origin.Y = image.Height / 2f;
    }

    #endregion
    
    #region Behaviours
    
    /// <summary>
    /// Calls rotation calculator and base update
    /// </summary>
    /// <param name="timePassed">time passed in seconds</param>
    public override void Update(float timePassed)
    {
        // Base update just translates by gravity and speed
        base.Update(timePassed);
        
        // Calculating rotation angle
        CalcRotation();
    }

    /// <summary>
    /// Calculates the rotation of the arrow in the air
    /// </summary>
    private void CalcRotation()
    {
        rotationAngle = (float)Math.Atan2(velocity.Y, velocity.X);
    }

    /// <summary>
    /// Draws arrow tilted
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, rec, null, Color.White, rotationAngle, origin, SpriteEffects.None, 1);
    }

    #endregion
}