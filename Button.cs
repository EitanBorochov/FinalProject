// Author: Eitan Borochov
// File Name: Button.cs
// Project Name: FinalProject
// Creation Date: May 13th 2025
// Modification Date: June 4th, 2025
// Description: Creates a button object that handles its interactions

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Button
{
    #region Attributes
    
    // Storing image, rec, and action
    private Texture2D buttonImg;
    private Rectangle buttonRec;
    private Action onClick;

    // Storing cancel img and bool 
    private Texture2D cancelImg;
    private bool selected = false;

    #endregion

    #region Constructor

    public Button(Texture2D buttonImg, Texture2D cancelImg, float x, float y, float width, float height, Action onClick)
    {
        // Storing what action to do when button is clicked (update)
        this.onClick = onClick;
        
        // Storing cancel img
        this.cancelImg = cancelImg;
        
        // Constructing rectangle
        buttonRec = new Rectangle((int)x, (int)y, (int)width, (int)height);
        
        // Storing image
        this.buttonImg = buttonImg;
    }
    
    public Button(Texture2D buttonImg, float x, float y, float width, float height, Action onClick)
    {
        // Storing what action to do when button is clicked (update)
        this.onClick = onClick;
        
        // Constructing rectangle
        buttonRec = new Rectangle((int)x, (int)y, (int)width, (int)height);
        
        // Storing image
        this.buttonImg = buttonImg;
    }

    #endregion

    #region Getters & Setters

    // Rectangle retriever
    public Rectangle Rec
    {
        get => buttonRec;
    }

    public bool Selected
    {
        get => selected;
    }
    #endregion

    #region Behaviors

    // Main update method that checks for interactions
    public bool Update(MouseState mouse, MouseState prevMouse)
    {
        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
        {
            if (buttonRec.Contains(mouse.Position))
            {
                onClick();
                selected = true;
            }
        }

        return selected;
    }

    public void Deselect()
    {
        selected = false;
    }
    
    // Drawing the button
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(buttonImg, buttonRec, Color.White);

        if (selected)
        {
            spriteBatch.Draw(cancelImg, buttonRec, Color.White);
        }
    }

    #endregion
}