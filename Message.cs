// Author: Eitan Borochov
// File Name: Message.cs
// Project Name: FinalProject
// Creation Date: June 8th 2025
// Modification Date: June 8th 2025
// Description: Creates a message to be displayed on screen when an action is done

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject;

public class Message
{
    #region Attributes
    
    // Storing drop shadow constant for better readability
    private readonly Vector2 DROP_SHADOW = new Vector2(2, 2);

    // Storing timer for how long message will be displayed
    private Timer dispTimer = new Timer(4000, true);
    
    // Storing alpha color multiplier to fade out message
    private float opacity = 1f;
    
    // Storing font and string of display message
    private SpriteFont font;
    private string message;
    
    // Storing position of message
    private Vector2 position;
    
    // Storing color of message
    private Color color;

    #endregion

    #region Constructor

    public Message(SpriteFont font, string message, Vector2 position, Color color)
    {
        // Storing input parameters
        this.font = font;
        this.message = message;
        this.position = position;
        this.color = color;
    }

    #endregion

    #region Getters & Setters
    

    #endregion

    #region Behaviours

    public bool Update(GameTime gameTime)
    {
        // Updating timer
        dispTimer.Update(gameTime);
        
        // Fading out message when timer is done
        FadeOut(gameTime);
        
        if (opacity == 0)
        {
            return true;
        }

        return false;
    }

    private void FadeOut(GameTime gameTime)
    {
        // Checking if display timer is done
        if (dispTimer.IsFinished())
        {
            // lower opacity for it to disappear in 2s
            opacity -= gameTime.ElapsedGameTime.Milliseconds / 2;
            
            // Clamping opacity (>0)
            if (opacity < 0)
            {
                opacity = 0;
            }
        }
    }

    public void TranslateY(float yPos)
    {
        position.Y = yPos;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Drawing message with drop shadow
        spriteBatch.DrawString(font, message, position + DROP_SHADOW, Color.Black * opacity);
        spriteBatch.DrawString(font, message, position, color * opacity);
    }

    #endregion
}