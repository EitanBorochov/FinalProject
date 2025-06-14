﻿// Author: Eitan Borochov
// File Name: Message.cs
// Project Name: FinalProject
// Creation Date: June 8th 2025
// Modification Date: June 10th 2025
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
    private Color dropShadowColor;

    #endregion

    #region Constructor

    public Message(SpriteFont font, string message, Vector2 position, Color color, Color dropShadowColor)
    {
        // Storing input parameters
        this.font = font;
        this.message = message;
        this.position = position;
        this.color = color;
        this.dropShadowColor = dropShadowColor;
    }

    #endregion

    #region Behaviours

    /// <summary>
    /// Updating message on display and making it disappear after timer is done
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates. Used to update timer</param>
    /// <returns>Returns true if message disappeared</returns>
    public bool Update(GameTime gameTime)
    {
        // Updating timer
        dispTimer.Update(gameTime);
        
        // Fading out message when timer is done
        FadeOut(gameTime);
        
        // Returns true if message faded
        if (opacity == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Fading out message by lowering its opacity
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates. Used to update timer</param>
    private void FadeOut(GameTime gameTime)
    {
        // Checking if display timer is done
        if (dispTimer.IsFinished())
        {
            // lower opacity for it to disappear in 2s
            opacity -= gameTime.ElapsedGameTime.Milliseconds / 3;
            
            // Clamping opacity (>0)
            if (opacity < 0)
            {
                opacity = 0;
            }
        }
    }

    /// <summary>
    /// Translate Y position of message
    /// </summary>
    /// <param name="yPos">Future Y position of message</param>
    public void TranslateY(float yPos)
    {
        position.Y = yPos;
    }

    /// <summary>
    /// Drawing message on screen
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Drawing message with drop shadow
        spriteBatch.DrawString(font, message, position + DROP_SHADOW, dropShadowColor * opacity);
        spriteBatch.DrawString(font, message, position, color * opacity);
    }

    #endregion
}