// Author: Eitan Borochov
// File Name: Zombie.cs
// Project Name: FinalProject
// Creation Date: May 20th 2025
// Modification Date: May 20th 2025
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

    // Storing zombie animation sprites
    private Texture2D walkImg;
    private Texture2D idleImg;
    private Texture2D deadImg;
    private Texture2D[] attackImgs = new Texture2D[3];
    
    // Storing zombie animations
    private Animation walkAnim;
    private Animation idleAnim;
    private Animation deadAnim;
    private Animation[] attackAnims = new Animation[3];
    
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
    public Zombie(Texture2D walkImg)
    {
        this.walkImg = walkImg;
        walkAnim = new Animation(this.walkImg, 8, 1, 8, 0, 0, 1, 500, position, false);
    }

    #endregion
}