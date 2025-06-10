// Author: Eitan Borochov
// File Name: MessageManager.cs
// Project Name: FinalProject
// Creation Date: June 8th 2025
// Modification Date: June 9th 2025
// Description: Manages all messages in a list and organizes them in order

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject;

public class MessageManager
{
    #region Attributes

    // Storing a list of messages
    private List<Message> messages = new List<Message>();
    
    // Storing position of the first message, this will stay the same
    private Vector2 firstPos;
    
    // Storing font for messages
    private SpriteFont font;

    #endregion

    #region Constructor

    public MessageManager(SpriteFont font, Vector2 firstPos)
    {
        // Storing input parameters
        this.firstPos = firstPos;
        this.font = font;
    }

    #endregion

    #region Behaviours

    /// <summary>
    /// Updates All messages and caps them to 6 at a time
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in between updates</param>
    public void Update(GameTime gameTime)
    {
        // Updating messages
        UpdateMessages(gameTime);
        
        // Capping messages to have a maximum of 6
        if (messages.Count > 6)
        {
            messages.RemoveRange(6, messages.Count - 6);
        }
    }

    /// <summary>
    /// Loops through message list and updates all of them
    /// </summary>
    /// <param name="gameTime">Keeps track of time passed in between updates</param>
    private void UpdateMessages(GameTime gameTime)
    {
        // Updating all messages
        for (int i = 0; i < messages.Count; i++)
        {
            // Updating and checking if message ran out
            if (messages[i] != null && messages[i].Update(gameTime))
            {
                // Removing message
                messages.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Creates a new message and adds it to the list
    /// </summary>
    /// <param name="message">The actual message that is sent</param>
    /// <param name="color">The color of the message</param>
    public void DisplayMessage(string message, Color color)
    {
        // Inserting new message at the top of the list
        messages.Insert(0, new Message(font, message, firstPos, color));
        
        // Translating all messages upwards
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i] != null)
            {
                // Translating each message up based on its height
                messages[i].TranslateY(firstPos.Y - i * font.MeasureString(message).Y - 5);
            }
        }
    }

    /// <summary>
    /// Draws messages on screen
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Message message in messages)
        {
            message.Draw(spriteBatch);
        }
    }

    #endregion
}