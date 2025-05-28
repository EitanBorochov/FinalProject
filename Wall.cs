// Author: Eitan Borochov
// File Name: Tower.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: May 23rd 2025
// Description: Creates a new wall object which is made of tiles and meant to block the enemy from the damage dealing towers

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Wall
{
    #region Attributes

    // Storing tile images for levels 1 and 2, and current level
    private Texture2D tile1Img;
    private Texture2D tile2Img;
    private Texture2D currentImg;
    
    // Storing tile rec array and full rectangle
    private Rectangle[] tileRecs = new Rectangle[6];
    private Rectangle hitbox;
    
    // Storing health
    private int health = 400;
    
    // Storing possible states for when its placed or not
    private const byte INACTIVE = 0;
    private const byte PREVIEW = 1;
    private const byte PLACED = 2;
    
    // Storing current state
    private byte state = PREVIEW;
    
    // Storing if the preview is in a valid location
    private bool isValid = false;

    #endregion

    #region Constructor

    public Wall(Texture2D tile1Img, Texture2D tile2Img, int tileWidth, int tileHeight)
    {
        // Storing inputted parameters
        this.tile1Img = tile1Img;
        this.tile2Img = tile2Img;
        
        // Constructing rectangles
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i] = new Rectangle(0, tileHeight * i, tileWidth, tileHeight);
        }
        
        // Loading hitbox
        hitbox = new Rectangle(tileRecs[0].X, tileRecs[0].Y, tileRecs[0].Width, tileRecs.Length * tileRecs[0].Height);
        
        // Storing current image as level 1 initially
        currentImg = this.tile1Img;
    }

    #endregion

    #region Getters & Setters

    // Accessable health quantity
    public int HP
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    // Returning a rectangle that surrounds all of the tiles
    public Rectangle GetRec()
    {
        return hitbox;
    }

    #endregion

    #region Behaviours

    public bool Update(MouseState mouse, MouseState prevMouse, 
                Rectangle platform, Rectangle buildableRec, bool isValid)
    {
        // Storing if current location is valid for placement
        this.isValid = isValid;
        
        PreviewState(mouse, prevMouse, platform, buildableRec);

        if (health <= 0)
        {
            state = INACTIVE;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Allowing player to move the wall around while its in preview
    private void PreviewState(MouseState mouse, MouseState prevMouse, 
                Rectangle platform, Rectangle buildableRec)
    {
        if (state == PREVIEW)
        {
            #region Translation

            // Translating Y position to the mouse position if its in buildable area
            if (mouse.Position.ToVector2().Y + hitbox.Height < platform.Top)
            {
                TranslateY(mouse.Position.ToVector2().Y);
            }
            else
            {
                TranslateY(platform.Top - hitbox.Height);
            }
            
            // Translating X position to the mouse position if its in buildable area
            if (mouse.Position.ToVector2().X < buildableRec.Right - hitbox.Width
                && mouse.Position.ToVector2().X > buildableRec.Left)
            {
                TranslateX(mouse.Position.ToVector2().X);
            }
            else if (mouse.Position.ToVector2().X >= buildableRec.Right - hitbox.Width)
            {
                TranslateX(buildableRec.Right - hitbox.Width);
            }
            else
            {
                TranslateX(buildableRec.Left);
            }

            #endregion
        }

        if (isValid)
        {
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                state = PLACED;
                TranslateY(platform.Top - hitbox.Height);
            }
        }
    }

    public void TranslateY(float yPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].Y = (int)yPos + tileRecs[i].Height * i;
        }

        // Offsetting collision rec
        hitbox.Y = tileRecs[0].Y;
    }
    public void TranslateX(float xPos)
    {
        // Translating Y and offsetting each one from each other
        for (int i = 0; i < tileRecs.Length; i++)
        {
            tileRecs[i].X = (int)xPos;
        }

        // Offsetting collision rec
        hitbox.X = tileRecs[0].X;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < tileRecs.Length; i++)
        {
            if (state == PREVIEW)
            {
                if (isValid)
                {
                    spriteBatch.Draw(currentImg, tileRecs[i], Color.White * 0.8f);
                }
                else
                {
                    spriteBatch.Draw(currentImg, tileRecs[i], Color.Red * 0.8f);
                }
            }
            else if (state == PLACED)
            {
                spriteBatch.Draw(currentImg, tileRecs[i], Color.White);
            }
        }
    }

    #endregion
}