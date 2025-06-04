// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: June 4th 2025
// Description: Zombie object, handles attacking, translating, etc.

using System;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace FinalProject;

public class Zombie
{
    #region Attributes
    
    // Storing zombie state for animation logic
    private const byte WALK = 0;
    private const byte DYING = 1;
    private const byte ATTACK1 = 2;
    private const byte ATTACK2 = 3;
    private const byte ATTACK3 = 4;
    private const byte INACTIVE = 5;

    private byte state = INACTIVE;

    // Storing zombie animation sprites
    private Texture2D[] images = new Texture2D[5];
    
    // Storing zombie animations
    private Animation[] anims = new Animation[5];
    
    // Storing zombie pos
    private Vector2 pos = new Vector2(-100, -100);
    
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
    
    // Storing reward which is how many coins the player earns on kill
    private int reward = 10;

    #endregion

    #region Constructor
    // Constructor
    public Zombie(Texture2D[] images, int screenWidth, int groundLevel)
    {
        // Storing the array of images for the animations
        this.images = images;
        
        // Creating the animations
        anims[WALK] = new Animation(images[WALK], 4, 2, 8, 0, 0, 1, 750, pos, 1, false);
        anims[DYING] = new Animation(images[DYING], 5, 1, 5, 0, 0, 1, 500, pos, 1, false);

        for (int i = ATTACK1; i <= ATTACK3; i++)
        {
            anims[i] = new Animation(images[i], 4, 1, 4, 0, 0, 1, 400, pos, 1,false);
        }

        //Loading initial zombie poss
        leftOfScreen = new Vector2(-anims[0].GetDestRec().Width, groundLevel - anims[0].GetDestRec().Height);
        rightOfScreen = new Vector2(screenWidth, groundLevel - anims[0].GetDestRec().Height);
    }
    
    #endregion
    
    #region Getters & Setters

    // Returning rectangle
    public Rectangle Rec
    {
        get
        {
            // Making sure the state is not inactive
            if (state < 5)
            {
                return anims[state].GetDestRec();
            }

            // Returning any animation rectangle
            return anims[ATTACK3].GetDestRec();
        }
    }

    // Returning and setting current zombie state
    public byte State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
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
    public int Damage
    {
        get => damage;
    }
    
    // Returning and setting zombie pos
    public Vector2 Pos
    {
        get
        {
            return pos;
        }
        set
        {
            pos = value;
        }
    }
    
    // Checking if zombie is attacking for collisions
    public bool IsAttacking()
    {
        if (state is ATTACK1 or ATTACK2 or ATTACK3)
        {
            return true;
        }

        return false;
    }
    
    // Checking if zombie is dead
    public bool IsDead()
    {
        if (state == INACTIVE)
        {
            return true;
        }

        return false;
    }
    
    #endregion

    #region Behaviours
    
    // Spawning zombie
    public void Spawn(byte gameLvl)
    {
        if (state == INACTIVE)
        {
            // Choosing initial pos and orientation
            if (gameLvl == 1)
            {
                // Setting pos and orientation
                pos = leftOfScreen;
            }
            else if (gameLvl == 2)
            {
                if (Game1.rng.Next(1, 3) == 1)
                {
                    // Setting position to left of screen
                    pos = leftOfScreen;
                    effect = SpriteEffects.None;
                }
                else
                {
                    // Setting position to right of screen and inverting it
                    pos = rightOfScreen;
                    dir = -1;
                    effect = SpriteEffects.FlipHorizontally;
                }
            }

            // Starting action timer
            actionTimer.ResetTimer(true);

            // Reseting health
            health = 20;

            // Setting zombie state to walk and translating that animation to the start
            state = WALK;
            anims[state].TranslateTo(pos.X, pos.Y);
        }
    }

    // Zombie logic for each update
    public void Update(GameTime gameTime, float timePassed)
    {
        if (state != INACTIVE)
        {
            // Updating timer & Animations
            actionTimer.Update(timePassed);
            anims[state].Update(gameTime);

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
        if (state == DYING && anims[DYING].IsFinished()) 
        {
            // Translating zombie out of screen
            pos.X = -1000;
            pos.Y = -1000;
            for (int i = 0; i < state - 1; i++)
            {
                anims[i].TranslateTo(pos.X, pos.Y);
            }
            
            // Deactivating zombie
            state = INACTIVE;
        }
        else if (state == WALK && actionTimer.IsFinished())
        {
            // Resetting animation
            anims[state].Activate(true);
            
            // Translating zombie pos
            pos.X += speed * dir * timePassed;
            anims[state].TranslateTo(pos.X, pos.Y);
            
            // Reactivating timer
            actionTimer.ResetTimer(true);
        }
    }
    
    // Killing zombie
    public void KillZombie()
    {
        // Making sure zombie is alive
        if (state != DYING && state != INACTIVE)
        {
            // Setting state to dying and playing animation
            state = DYING;
            anims[DYING].Activate(true);
            anims[DYING].TranslateTo(pos.X, pos.Y);
            
            // adding one to death count
            Game1.IncreaseMobsKilled();
            
            // Rewarding player with coins
            Game1.Coins += reward;
        }
    }

    // Dealing damage to tower
    public int Attack(int towerHP)
    {
        // Making sure zombie is alive
        if (state != DYING && state != INACTIVE)
        {
            // Setting zombie state to attack if it's not an attack and translating it to the pos
            if (state < ATTACK1 || state > ATTACK3)
            {
                state = (byte)Game1.rng.Next(ATTACK1, ATTACK3 + 1);
                anims[state].TranslateTo(pos.X, pos.Y);
            }

            // Attacking when action timer is done
            if (actionTimer.IsFinished())
            {
                // Randomizing attack variant
                state = (byte)Game1.rng.Next(ATTACK1, ATTACK3 + 1);

                // Translating animation and activating it 
                anims[state].TranslateTo(pos.X, pos.Y);
                anims[state].Activate(true);

                // Lowering tower HP
                towerHP -= damage;

                // Resetting action timer
                actionTimer.ResetTimer(true);
            }
        }

        return towerHP;
    }

    public void Walk()
    {
        // Making sure zombie is alive
        if (state != DYING && state != INACTIVE)
        {
            // Setting zombie to walk
            state = WALK;
        }
    }
    
    // Drawing zombie
    public void Draw(SpriteBatch spriteBatch)
    {
        if (state != INACTIVE)
        {
            anims[state].Draw(spriteBatch, Color.White, effect);
        }
    }

    #endregion
}