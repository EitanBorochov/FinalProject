// Author: Eitan Borochov
// File Name: Platform.cs
// Project Name: FinalProject
// Creation Date: May 9th 2025
// Modification Date: May 12th 2025
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

    // Getting the platform rec for calculations or collisions
    public Rectangle GetRec()
    {
        return platformRec;
    }

    #endregion

    #region Behaviours

    public void Draw(SpriteBatch _spriteBatch)
    {
        // Drawing platform
        for (int i = 0; i < brickRecs.Length; i++)
        {
            _spriteBatch.Draw(brickImg, brickRecs[i], Color.White);
        }
    }

    #endregion



}