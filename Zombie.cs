// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: June 9th 2025
// Description: Zombie object, handles attacking, translating, animations, etc.

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
    
    // Storing blood animation, each zombie has a different random one
    private Animation bloodAnim;
    
    // Storing respawn timer for when zombie dies
    private Timer respawnTimer = new Timer(3000, false);

    #endregion

    #region Constructor
    // Constructor
    public Zombie(Texture2D[] images, Texture2D bloodAnimImg, int screenWidth, int groundLevel)
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
        
        // Creating blood animation
        bloodAnim = new Animation(bloodAnimImg, 6, 6, 29, 0, -1, 1, 1000, pos, 0.5f ,false);
    }
    
    #endregion
    
    #region Getters & Setters

    /// <summary>
    /// Returning rectangle of current state animation
    /// </summary>
    public Rectangle Rec
    {
        get
        {
            // Making sure the state is not inactive
            if (state == INACTIVE)
            {
                // Returning a rectangle out of screen as zombie is inactive
                return new Rectangle(-1000, -1000, 0, 0);
            }

            return anims[state].GetDestRec();
        }
    }
    
    /// <summary>
    /// Checking if zombie is dead
    /// </summary>
    /// <returns>Returning true if zombie is dead</returns>
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
    
    /// <summary>
    /// Spawning zombie and resetting it
    /// </summary>
    /// <param name="gameLvl">Level of current game (level 1 or level 2)</param>
    /// <param name="zombieHealth">Health of zombie to reset to</param>
    public void Spawn(byte gameLvl, int zombieHealth)
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
                if (Game1.rng.Next(1, 101) > 50)
                {
                    // Setting position to left of screen
                    pos = leftOfScreen;
                    dir = 1;
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
            
            // Playing spawning sound
            Game1.PlaySound(Game1.zombieSpawnSnd, 0.1f);

            // Starting action timer and respawn timer
            actionTimer.ResetTimer(true);
            respawnTimer.ResetTimer(false);

            // Reseting health
            health = zombieHealth;

            // Setting zombie state to walk and translating that animation to the start
            state = WALK;
            anims[state].TranslateTo(pos.X, pos.Y);
        }
    }

    /// <summary>
    /// Updating zombie and making decisions for what to do every update
    /// </summary>
    /// <param name="gameTime">Keeps track of time between update</param>
    /// <param name="gameLvl">Level of current game mode</param>
    /// <param name="zombieHealth">Health of zombie for when it respawns</param>
    /// <param name="isNightTime">Is it currently nighttime</param>
    public void Update(GameTime gameTime, byte gameLvl, int zombieHealth, bool isNightTime)
    {
        if (state != INACTIVE)
        {
            // Updating timer & Animations
            actionTimer.Update(gameTime);
            anims[state].Update(gameTime);
            bloodAnim.Update(gameTime);

            // Zombie Action
            Action(gameTime);

            // Checking if zombie is dead
            if (health <= 0)
            {
                KillZombie();
            }
        }
        else if (state == INACTIVE && isNightTime)
        {
            // Updating respawn timer
            respawnTimer.Update(gameTime);
            
            //Checking for if its done and its time for a respawn
            if (respawnTimer.IsFinished())
            {
                Spawn(gameLvl, zombieHealth);
            }
        }
    }
    
    /// <summary>
    /// Determining zombie action based on state
    /// </summary>
    /// <param name="gameTime">Keeps track of time between updates</param>
    private void Action(GameTime gameTime)
    {
        // Killing zombie if dying animation is done
        if (state == DYING && anims[DYING].IsFinished()) 
        {
            // Deactivating zombie
            state = INACTIVE;
            respawnTimer.ResetTimer(true);
        }
        else if (state == WALK && actionTimer.IsFinished())
        {
            // Resetting animation
            anims[state].Activate(true);
            
            // Translating zombie pos
            pos.X += speed * dir * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            anims[state].TranslateTo(pos.X, pos.Y);
            
            // Reactivating timer
            actionTimer.ResetTimer(true);
        }
    }
    
    /// <summary>
    /// Killing zombie
    /// </summary>
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
            
            // Adding to zombie kill count per update
            Game1.zombiesKilledPerUpdate++;
        }
    }

    /// <summary>
    /// Dealing damage to a defence
    /// </summary>
    /// <param name="defenceHP">health of defence</param>
    /// <returns>health of defence when damage is dealt</returns>
    public int Attack(int defenceHP)
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
                defenceHP -= damage;
                
                // Playing attack sound
                Game1.PlaySound(Game1.zombieAttackSnd, 0.03f);

                // Resetting action timer
                actionTimer.ResetTimer(true);
            }
        }

        return defenceHP;
    }

    /// <summary>
    /// Dealing damage to current zombie
    /// </summary>
    /// <param name="damage">How much damage to deal to current zombie</param>
    public void HitZombie(int damage)
    {
        // Lowering health
        health -= damage;
        
        // Playing blood animation at zombie's location
        bloodAnim.TranslateTo(pos.X, pos.Y);
        bloodAnim.Activate(true);
        
        // Playing damage sound
        Game1.PlaySound(Game1.zombieHitSnd, 0.1f);
    }

    /// <summary>
    /// Setting zombie state to walk if its not dead
    /// </summary>
    public void Walk()
    {
        // Making sure zombie is alive
        if (state != DYING && state != INACTIVE)
        {
            // Setting zombie to walk
            state = WALK;
        }
    }
    
    /// <summary>
    /// Drawing zombie
    /// </summary>
    /// <param name="spriteBatch">Current batch of sprite draws. Each update there is a new one</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (state != INACTIVE)
        {
            anims[state].Draw(spriteBatch, Color.White, effect);
            bloodAnim.Draw(spriteBatch, Color.White, effect);
        }
    }

    #endregion
}