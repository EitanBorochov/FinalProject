// Author: Eitan Borochov
// File Name: Arrow.cs
// Project Name: FinalProject
// Creation Date: June 4th 2025
// Modification Date: June 4th 2025
// Description: Inherited projectile class for arrows specifically

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Arrow : Projectile
{
    // Storing a constant arrow speed
    public const float LAUNCH_SPEED = 850f;
    
    // Storing rotation angle and origin
    private float rotationAngle;
    private Vector2 origin;
    
    #region Constructor

    public Arrow(Rectangle rec, Vector2 mousePos, bool highAngle, Texture2D image, int damage, float maxMag) : 
        base(rec, LAUNCH_SPEED, mousePos, highAngle, image, damage, maxMag)
    {
        // Making the size of the arrow smaller since the image is already very large
        this.rec.Width /= 10;
        this.rec.Height /= 10;
        
        origin.X = image.Width / 2f;
        origin.Y = image.Height / 2f;
    }

    #endregion
    
    #region Behaviours
    
    public override void Update(float timePassed)
    {
        // Base update just translates by gravity and speed
        base.Update(timePassed);
        
        // Calculating rotation angle
        CalcRotation();
    }

    private void CalcRotation()
    {
        rotationAngle = (float)Math.Atan2(velocity.Y, velocity.X);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, rec, null, Color.White, rotationAngle, origin, SpriteEffects.None, 1);
    }

    #endregion
}