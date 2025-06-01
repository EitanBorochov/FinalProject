// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: May 31st 2025
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
    private const byte DYING = 1;
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
    private float speed = 1.5f;
    private int dir = 1;

    #endregion

    #region Constructor
    // Constructor
    public Zombie(Texture2D[] images, int screenWidth, int groundLevel)
    {
        // Storing the array of images for the animations
        this.images = images;
        
        // Creating the animations
        anims[WALK] = new Animation(images[WALK], 4, 2, 8, 0, 0, 1, 600, position, 1, false);
        anims[DYING] = new Animation(images[DYING], 5, 1, 5, 0, 0, 1, 500, position, 1, false);

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

    // Returning rectangle
    public Rectangle GetRec()
    {
        // ** ALL ZOMBIES HAVE SAME REC
        return anims[WALK].GetDestRec();
    }

    // Returning and setting current zombie state
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

    // Returning and setting zombie HP 
    public int HP
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    // Returning zombie damage
    public int GetDamage()
    {
        return damage;
    }
    
    // Returning and setting zombie position
    public Vector2 Pos
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
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
        if (zombieState != INACTIVE)
        {
            // Updating timer & Animations
            actionTimer.Update(timePassed);
            anims[zombieState].Update(gameTime);

            // Zombie Action
            Action(timePassed);

            // Checking if zombie is dead
            if (health <= 0)
            {
                KillZombie();
            }
        }
    }
    
    // Determining zombie action based on state
    private void Action(float timePassed)
    {
        // // Determining
        if (zombieState == DYING && anims[DYING].IsFinished()) 
        {
            // Translating zombie out of screen
            position.X = -100;
            position.Y = -100;
            for (int i = 0; i < zombieState - 1; i++)
            {
                anims[i].TranslateTo(position.X, position.Y);
            }
            
            // Deactivating zombie
            zombieState = INACTIVE;
        }
        else if (zombieState == WALK && actionTimer.IsFinished())
        {
            // Resetting animation
            anims[zombieState].Activate(true);
            
            // Translating zombie position
            position.X += speed * dir * timePassed;
            anims[zombieState].TranslateTo(position.X, position.Y);
            
            // Reactivating timer
            actionTimer.ResetTimer(true);
        }
    }
    
    // Killing zombie
    public void KillZombie()
    {
        // Making sure zombie is alive
        if (zombieState != DYING && zombieState != INACTIVE)
        {
            // Setting state to dying and playing animation
            zombieState = DYING;
            anims[DYING].Activate(true);
            anims[DYING].TranslateTo(position.X, position.Y);
            
            // adding one to death count
            Game1.IncreaseMobsKilled();
        }
    }

    // Dealing damage to tower
    public int Attack(int towerHP)
    {
        // Making sure zombie is alive
        if (zombieState != DYING && zombieState != INACTIVE)
        {
            // Setting zombie state to attack if it's not an attack and translating it to the position
            if (zombieState < ATTACK1 || zombieState > ATTACK3)
            {
                zombieState = (byte)rng.Next(ATTACK1, ATTACK3 + 1);
                anims[zombieState].TranslateTo(position.X, position.Y);
            }

            // Attacking when action timer is done
            if (actionTimer.IsFinished())
            {
                // Randomizing attack variant
                zombieState = (byte)rng.Next(ATTACK1, ATTACK3 + 1);

                // Translating animation and activating it 
                anims[zombieState].TranslateTo(position.X, position.Y);
                anims[zombieState].Activate(true);

                // Lowering tower HP
                towerHP -= damage;

                // Resetting action timer
                actionTimer.ResetTimer(true);
            }
        }

        return towerHP;
    }
    
    // Checking if zombie is attacking for collisions
    public bool IsAttacking()
    {
        if (zombieState == ATTACK1 || zombieState == ATTACK2 || zombieState == ATTACK3)
        {
            return true;
        }

        return false;
    }

    public void Walk()
    {
        // Making sure zombie is alive
        if (zombieState != DYING && zombieState != INACTIVE)
        {
            // Setting zombie to walk
            zombieState = WALK;
        }
    }
    
    // Drawing zombie
    public void Draw(SpriteBatch spriteBatch)
    {
        if (zombieState != INACTIVE)
        {
            anims[zombieState].Draw(spriteBatch, Color.White, effect);
        }
    }

    #endregion
}