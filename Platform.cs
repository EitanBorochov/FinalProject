// Author: Eitan Borochov
// File Name: Platform.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: June 9th 2025 
// Description: Handles collision, properties, and drawing of platform

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Platform
{
    #region Attributes

    // Storing array of brick recs and one full platform rectangle
    Rectangle[] brickRecs;
    Rectangle platformRec;

    // Storing image of brick
    Texture2D brickImg;

    #endregion

    #region Constructor

    // constructor
    public Platform(Texture2D brickImg, int screenWidth, int screenHeight)
    {
        // Setting brick image to input image
        this.brickImg = brickImg;
        
        // Calculating array length
        int length = screenWidth / brickImg.Width;

        // Defining array
        brickRecs = new Rectangle[length];
        
        // Offsetting brick locations to be next to each other
        for (int i = 0; i < brickRecs.Length; i++)
        {
            // Loading each rectangle to be next to each other and halfway into the bottom of the screen
            brickRecs[i] = new Rectangle(i * brickImg.Width, screenHeight - brickImg.Height / 2, 
                                        brickImg.Width, brickImg.Height);
        }
        
        // Setting a full Rectangle for easier collisions
        platformRec = new Rectangle(brickRecs[0].X, brickRecs[0].Y, brickRecs.Length * brickRecs[0].Width,
                                    brickRecs[0].Height);
    }

    #endregion

    #region Getters & Setters

    /// <summary>
    /// Getting the platform rec for calculations or collisions
    /// </summary>
    public Rectangle Rec => platformRec;

    #endregion

    #region Behaviours

    /// <summary>
    /// Draws each brick separately in a loop
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Drawing platform
        foreach (Rectangle brickRec in brickRecs)
        {
            spriteBatch.Draw(brickImg, brickRec, Color.White);
        }
    }

    #endregion
}