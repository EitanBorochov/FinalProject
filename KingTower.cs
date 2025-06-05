// Author: Eitan Borochov
// File Name: KingTower.cs
// Project Name: FinalProject
// Creation Date: May 27th 2025
// Modification Date: June 2nd 2025
// Description: Inhereted tower class for the king tower

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FinalProject;

public class KingTower : Tower
{
    #region Constructor

    public KingTower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, 
                    int hitboxHeight, Texture2D projectileImg, int cooldownLength) : 
                    base(towerImg, position, width, height, hitboxWidth, hitboxHeight, projectileImg, cooldownLength)
    {
        // Storing health and damage
        health = 1000;
        damage = 4;
    }

    #endregion

    #region Behaviours

    public override bool Update(GameTime gameTime, MouseState mouse, Rectangle buildableRec, bool isValid)
    {
        // Updating cooldown timer
        cooldownTimer.Update(gameTime);
        
        return base.Update(gameTime, mouse, buildableRec, isValid);
    }

    // Launching cannon ball
    public Cannonball LaunchBall(Vector2 mousePos)
    {
        if (cooldownTimer.IsFinished())
        {
            if (mousePos.X < hitbox.Left)
            {
                // Setting it on top RIGHT of tower
                projRec.Location = hitbox.Location;
            }
            else if (mousePos.X > hitbox.Right)
            {
                // Setting it to top LEFT of tower
                projRec.Location = new Point(hitbox.Right, hitbox.Top);
            }
            else
            {
                return null;
            }

            cooldownTimer.ResetTimer(true);

            return new Cannonball(projRec, mousePos, false, projImg, damage, 5, 350);
        }

        return null;
    }

    #endregion
}