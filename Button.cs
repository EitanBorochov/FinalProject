// Author: Eitan Borochov
// File Name: Game1.cs
// Project Name: FinalProject
// Creation Date: May 13th 2025
// Modification Date: May 13th 2025
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
    
    private Texture2D buttonImg;
    private Rectangle buttonRec;
    private Action onClick;

    #endregion

    #region Constructor

    public Button(Texture2D buttonImg, int x, int y, int width, int height, Action onClick)
    {
        // Storing what action to do when button is clicked (update)
        this.onClick = onClick;
        
        // Constructing rectangle
        buttonRec = new Rectangle(x, y, width, height);
        
        // Storing image
        this.buttonImg = buttonImg;
    }

    #endregion

    #region Getters & Setters

    // Rectangle retriever
    public Rectangle GetRec()
    {
        return buttonRec;
    }

    #endregion

    #region Behaviors

    // Main update method that checks for interactions
    public void Update(MouseState mouse, MouseState prevMouse)
    {
        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
        {
            if (buttonRec.Contains(mouse.Position))
            {
                onClick();
            }
        }
    }
    
    // Drawing the button
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(buttonImg, buttonRec, Color.White);
    }

    #endregion
}