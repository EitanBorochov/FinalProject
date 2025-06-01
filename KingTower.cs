// Author: Eitan Borochov
// File Name: Tower.cs
// Project Name: FinalProject
// Creation Date: May 27th 2025
// Modification Date: May 29th 2025
// Description: Inhereted tower class for the king tower

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FinalProject;

public class KingTower : Tower
{
    // Storing projectile image
    private Texture2D projImg;
    private Rectangle projRec;
    
    // Storing damage
    private int damage;
    
    #region Constructor

    public KingTower(Texture2D towerImg, Vector2 position, int width, int height, int hitboxWidth, 
                    int hitboxHeight, int health, Texture2D projectileImg, int damage, int cooldownLength) : 
                    base(towerImg, position, width, height, hitboxWidth, hitboxHeight, health, cooldownLength)
    {
        this.projImg = projectileImg;
        projRec = new Rectangle(0, 0, projImg.Width, projImg.Height);
        this.damage = damage;

    }

    #endregion

    #region Behaviours
    
    // Launching cannon ball
    public Cannonball LaunchBall(Vector2 mousePos)
    {
        if (cooldownTimer.IsFinished())
        {
            if (mousePos.X < Game1.screenWidth / 2)
            {
                // Setting it on top RIGHT of tower
                projRec.Location = hitbox.Location;
            }
            else
            {
                // Setting it on top LEFT of screen
                projRec.Location = new Point(hitbox.Right, hitbox.Top);
            }

            cooldownTimer.ResetTimer(true);

            return new Cannonball(projRec, mousePos, false, projImg, damage, 5, 400);
        }

        return null;
    }

    #endregion
    
}