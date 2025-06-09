// Author: Eitan Borochov
// File Name: MessageManager.cs
// Project Name: FinalProject
// Creation Date: June 8th 2025
// Modification Date: June 8th 2025
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

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Message message in messages)
        {
            message.Draw(spriteBatch);
        }
    }

    #endregion
}