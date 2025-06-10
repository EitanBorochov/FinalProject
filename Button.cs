// Author: Eitan Borochov
// File Name: Button.cs
// Project Name: FinalProject
// Creation Date: May 13th 2025
// Modification Date: June 9th, 2025
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
    
    // Storing drawing action on hover
    private Action drawOnHover;

    // Storing cancel img and bool 
    private Texture2D cancelImg = null;
    private bool selected = false;

    #endregion

    #region Constructor

    public Button(Texture2D buttonImg, Texture2D cancelImg, float x, float y, float width, float height, Action onClick, Action drawOnHover)
    {
        // Storing what action to do when button is clicked (update)
        this.onClick = onClick;
        
        // Storing cancel img
        this.cancelImg = cancelImg;
        
        // Constructing rectangle
        buttonRec = new Rectangle((int)x, (int)y, (int)width, (int)height);
        
        // Storing image
        this.buttonImg = buttonImg;
        
        // Storing drawing action on hover
        this.drawOnHover = drawOnHover;
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

    /// <summary>
    /// Returns rectangle of button
    /// </summary>
    public Rectangle Rec => buttonRec;
    
    /// <summary>
    /// Selecting this button
    /// </summary>
    public void Select()
    {
        selected = true;
    }

    /// <summary>
    /// Deselecting this button
    /// </summary>
    public void Deselect()
    {
        selected = false;
    }

    #endregion

    #region Behaviors

    /// <summary>
    /// Main update method that checks for interactions
    /// </summary>
    /// <param name="mouse">current state of mouse</param>
    /// <param name="prevMouse">state of mouse from previous update</param>
    /// <returns></returns>
    public bool Update(MouseState mouse, MouseState prevMouse)
    {
        // Checking for mouse click
        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
        {
            if (buttonRec.Contains(mouse.Position))
            {
                // Performing set action
                onClick();
                
                // Playing click sound
                Game1.PlaySound(Game1.buttonSnd, 0.5f);
            }
        }

        return selected;
    }
    
    /// <summary>
    /// Drawing button with cancel option and hover options on top of it
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    /// <param name="mousePos">Position of current mouse</param>
    public void Draw(SpriteBatch spriteBatch, Point mousePos)
    {
        // Drawing button
        spriteBatch.Draw(buttonImg, buttonRec, Color.White);

        // Drawing cancel option if its available
        if (cancelImg != null && selected)
        {
            spriteBatch.Draw(cancelImg, buttonRec, Color.White);
        }

        // Drawing hover action
        if (drawOnHover != null && buttonRec.Contains(mousePos))
        {
            drawOnHover();
        }
    }

    #endregion
}