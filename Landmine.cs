// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: June 6th 2025
// Modification Date: June 6th 2025
// Description: Landmine will explode as soon as any zombie steps on it, can be placed anywhere, and is a 1 time use.
// For more efficient code, the landmine will be part of the tower family and tower list.

using System;
using System.Collections.Generic;
using System.Linq;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Landmine : Tower
{
    #region Attributes

    // Storing rectangle and image of landmine
    private Rectangle rec;
    private Texture2D img;
    
    // Storing possible states for when its placed or not
    private const byte PREVIEW = 1;
    private const byte PLACED = 2;
    
    // Storing current state
    private byte state = PREVIEW;
    
    // Storing damage
    private int damage = 10;
    
    // Storing price
    private static int price = 275;

    #endregion

    #region Constructor

    public Landmine(Texture2D img, int scale, int platformTop) : base(img, img.Width * scale, img.Height * scale)
    {
        // Storing image from parameter
        this.img = img;
        
        // Setting up rectangle
        rec = new Rectangle(0, platformTop - img.Height * scale, img.Width * scale, img.Height * scale);
    }

    #endregion

    #region Getters & Setters

    // Returning default price of this class
    public static int GetDefaultPrice()
    {
        return price;
    }

    #endregion

    #region Behaviours

    /* Main goal of update is to check for zombie collisions, if it detects any zombie, returns true which will spawn
     a cannonball in place of the landmine, the cannonball should explode immediately.*/
    public override bool Update(MouseState mouse, int screenWidth, Zombie[] zombies)
    {
        // Translating during preview
        PreviewStateTranslation(mouse, screenWidth);
        
        // Checking for collisions with zombies only if state is placed
        if (state == PLACED)
        {
            for (int i = 0; i < zombies.Length; i++)
            {
                // If a collision occured, returning true which sends the signal to summon a cannonball
                if (zombies[i].Rec.Intersects(rec))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private void PreviewStateTranslation(MouseState mouse, int screenWidth)
    {
        // Checking if state is preview
        if (state == PREVIEW)
        {
            // Making sure mouse is in screen boundaries
            if (mouse.Position.X < 0 && mouse.Position.X > screenWidth - rec.Width)
            {
                // Translating bomb to mouse position
                rec.X = mouse.Position.X;
            }
        }
    }
    
    // Checks for placement of landmine, returns true if placement is cancelled
    public override bool CheckPlacement(MouseState mouse, MouseState prevMouse)
    {
        if (state == PREVIEW)
        {
            // Checking for right click button cancel
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
            {
                // Returning true which means delete landmine
                return true;
            }

            // Checking if user has enough money and the placement is valid
            if (Game1.Coins >= price)
            {
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    // placing tower
                    state = PLACED;

                    // Taking away coins
                    Game1.Coins -= price;
                }
            }
        }

        return false;
    }

    public override void Draw(SpriteBatch spriteBatch, int buildRecCenter, Color placedColor)
    {
        // Drawing transparent if in preview
        if (state == PREVIEW)
        {
            // Drawing red if not enough coins
            if (Game1.Coins >= price)
            {
                spriteBatch.Draw(img, rec, placedColor * 0.8f);
            }
            else
            {
                spriteBatch.Draw(img, rec, Color.Red * 0.8f);
            }
        }
        else
        {
            spriteBatch.Draw(img, rec, placedColor);
        }
    }

    #endregion
}