// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: May 22nd 2025
// Description: Zombie object, handles attacking, translating, etc.

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Zombie
{
    // Storing random number generator
    private Random rng = new Random();
    
    #region Attributes
    
    // Storing zombie state for animation logic
    private const byte WALK = 0;
    private const byte DEAD = 1;
    private const byte ATTACK1 = 2;
    private const byte ATTACK2 = 3;
    private const byte ATTACK3 = 4;
    private const byte INACTIVE = 5;

    private byte zombieState;

    // Storing zombie animation sprites
    private Texture2D[] images = new Texture2D[5];
    
    // Storing zombie animations
    private Animation[] anims = new Animation[5];
    
    // Storing zombie position
    private Vector2 position = new Vector2(-100, -100);
    
    // Storing defualt spawning screen dimensions
    private Vector2 leftOfScreen;
    private Vector2 rightOfScreen;
    
    // Storing sprite effect (flip orientation)
    private SpriteEffects effect;
    
    // Storing action Timer (Timer for when zombie will move or attack)
    private Timer actionTimer = new Timer(750, false);
    
    // Storing zombie health and attack damage
    private int health = 20;
    private int damage = 4;
    
    // Storing zombie moving speed and direction
    private int speed = 1;
    private int dir = 1;

    #endregion

    #region Constructor
    // Constructor
    public Zombie(Texture2D[] images, int screenWidth, int groundLevel)
    {
        // Storing the array of images for the animations
        this.images = images;
        
        // Creating the animations
        anims[WALK] = new Animation(images[WALK], 4, 2, 8, 0, 0, 1, 500, position, 1, false);
        anims[DEAD] = new Animation(images[DEAD], 5, 1, 5, 0, 0, 1, 500, position, 1, false);

        for (int i = ATTACK1; i <= ATTACK3; i++)
        {
            anims[i] = new Animation(images[i], 4, 1, 8, 0, 0, 1, 400, position, 1,false);
        }

        //Loading initial zombie positions
        leftOfScreen = new Vector2(-anims[0].GetDestRec().Width, groundLevel - anims[0].GetDestRec().Height);
        rightOfScreen = new Vector2(screenWidth, groundLevel - anims[0].GetDestRec().Height);
    }
    
    #endregion
    
    #region Getters & Setters

    public Rectangle GetDestRec()
    {
        return anims[zombieState].GetDestRec();
    }

    public byte State
    {
        get
        {
            return zombieState;
        }
        set
        {
            zombieState = value;
        }
    }
    
    #endregion

    #region Behaviours
    
    // Spawning zombie
    public void Spawn(byte level)
    {
        // Choosing initial position and orientation
        if (level == 1)
        {
            // Setting position and orientation
            position = leftOfScreen;
            effect = SpriteEffects.None;
        }
        else if (level == 2)
        {
            if (rng.Next(1, 3) == 1)
            {
                position = leftOfScreen;
                effect = SpriteEffects.None;
            }
            else
            {
                position = rightOfScreen;
                dir = -1;
                effect = SpriteEffects.FlipHorizontally;
            }
        }
        
        // Starting action timer
        actionTimer.ResetTimer(true);
        
        // Setting zombie state to walk
        zombieState = WALK;
    }

    // Zombie logic for each update
    public void Update(GameTime gameTime, float timePassed)
    {
        // Updating timer & Animations
        actionTimer.Update(timePassed);
        anims[zombieState].TranslateTo(position.X, position.Y);
        anims[zombieState].Update(gameTime);
        
        // Translating zombie every time the action timer goes off
        if (actionTimer.IsFinished())
        {
            // Resetting animation
            anims[zombieState].Activate(true);
            
            // Translating if state is walking
            if (zombieState == WALK)
            {
                position.X += speed * dir * timePassed;
            }

            // Reactivating timer
            actionTimer.ResetTimer(true);
        }
    }
    
    // Drawing zombie
    public void Draw(SpriteBatch spriteBatch)
    {
        anims[zombieState].Draw(spriteBatch, Color.White, effect);
    }

    #endregion
}