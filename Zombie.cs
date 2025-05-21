// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: May 21st 2025
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
    private const byte DEAD = 1;
    private const byte ATTACK1 = 2;
    private const byte ATTACK2 = 3;
    private const byte ATTACK3 = 4;

    private byte zombieState;

    // Storing zombie animation sprites
    private Texture2D[] images = new Texture2D[5];
    
    // Storing zombie animations
    private Animation[] attackAnims = new Animation[5];
    
    // Storing zombie position
    private Vector2 position;
    
    // Storing action Timer (Timer for when zombie will move or attack)
    private Timer actionTimer = new Timer(750, false);
    
    // Storing zombie health and attack damage
    private int health = 20;
    private int damage = 4;
    
    // Storing zombie moving speed (px/s)
    private int speed = 50;

    #endregion

    #region Constructor
    // Constructor
    public Zombie(Texture2D[] images)
    {
        
    }

    #endregion
}