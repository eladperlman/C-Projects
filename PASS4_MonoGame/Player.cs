//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Player.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to a player game object 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Player : GameObject
    {
        //Stores player's gem and key count
        private int keyCount = 0;
        private int gemCount = 0;

        //Stores player's total keys collected in the level
        private int keysCollected = 0;

        //Stores collect and push booleans
        private bool collect = false;
        private bool push = false;

        //Stores player's x direction and the constant values for the right and left directions
        private int dir = 1;
        private const int RIGHT = 1;
        private const int LEFT = -1;

        //Stores player's target spot's x location
        private int targetSpot;

        //Stores weather player is on ground, and if he is alive
        private bool isGrounded = false;
        private bool isAlive = true;

        //Stores the delegate type of notify crate, and the event ResetCollideCrate
        public delegate void NotifyCrate(); 
        public event NotifyCrate ResetCollideCrate;

        //Pre: x, and y are between the 0 and the game window's dimensions
        //Post: N/A
        //Description: Calls the gameobject's constructor with player image and its position
        public Player(int x, int y) : base(MainGame.gameObjectsImg[MainGame.PLAYER], x, y)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Reset's all the player's components
        public override void ResetObject()
        {
            //Calls base class's reset object function
            base.ResetObject();

            //Resets all of player's gem and key counts to 0 as well as the push and collect booleans to false
            keyCount = 0;
            gemCount = 0;
            keysCollected = 0;
            collect = false;
            push = false;
            isAlive = true;
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Collects key or gem if they are visible and increments the matching count variable
        public void CollectCollectable(GameObject obj)
        {
            //Checks if object is visible
            if (obj.Visibility)
            {
                //Checks if object is a key, if so incriment its key counts and sets its visibility to false
                if (obj is Key)
                {
                    ++keyCount;
                    ++keysCollected;
                    obj.Visibility = false;
                }
                //Checks if object is a gem, if so incriment its gem count and sets its visibility to false
                else if (obj is Gem)
                {
                    ++gemCount;
                    obj.Visibility = false;
                }
            }
        }

        //Pre: command is not null and is one of the valid commands
        //Post: N/A
        //Description: Excecutes the command given based on which one it is
        public void ExcecuteCommand(char command)
        {
            //Resets collect and push variables
            collect = false;
            push = false;

            //Checks which case the command matches and excecutes the command's action
            switch (command)
            {
                case 'A':
                    //Sets x speed to negative original x speed, and the direction to left
                    speed.X = -ogSpeed.X;
                    dir = LEFT;

                    //Sets target spot to the x value of the tile to the left
                    targetSpot = (int)TruePos.X - rec.Width;
                    break;
                case 'D':
                    //Sets x speed to original x speed, and the direction to right
                    speed.X = ogSpeed.X;
                    dir = RIGHT;

                    //Sets target spot to the x value of the tile to the right
                    targetSpot = (int)TruePos.X + rec.Width;
                    break;
                case 'Q':
                    //Sets speed to negative original x speed and the original y speed, as well as the direction to left
                    speed.X = -ogSpeed.X;
                    speed.Y = -ogSpeed.Y;
                    dir = LEFT;

                    //Sets target spot to the x value of the tile to the left
                    targetSpot = (int)TruePos.X - rec.Width;
                    break;
                case 'E':
                    //Sets speed to original x and y speeds, as well as the direction to right
                    speed.X = ogSpeed.X;
                    speed.Y = -ogSpeed.Y;
                    dir = RIGHT;

                    //Sets target spot to the x value of the tile to the right
                    targetSpot = (int)TruePos.X + rec.Width;
                    break;
                case 'C':
                    //Sets collect to true
                    collect = true;
                    break;
                case '-':
                    //Sets push to true
                    push = true;

                    //Sets speed to negative original x speed and the original y speed, as well as the direction to left
                    speed.X = -ogSpeed.X;
                    dir = LEFT;

                    //Sets target spot to the x value of the tile to the left
                    targetSpot = (int)TruePos.X - rec.Width;
                    break;
                case '+':
                    //Sets push to true
                    push = true;

                    //Sets speed to original x and yspeeds, as well as the direction to right
                    speed.X = ogSpeed.X;
                    dir = RIGHT;

                    //Sets target spot to the x value of the tile to the right
                    targetSpot = (int)TruePos.X + rec.Width;
                    break;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the player's logic during the game
        public override void Update()
        {
            //Sets isGrounded to false
            isGrounded = false;

            //Checks if player is in the air, if so sets its x speed to original x speed times player's direction
            if ((int)speed.Y < 0 && speed.X == 0)
            {
                speed.X = ogSpeed.X * dir;
            }

            //Checks if player reached target goal, if so set x speed to 0
            if (TruePos.X == targetSpot)
            {
                speed.X = 0;
            }

            //Applys gravity to the player
            speed.Y += gravity;

            //Adds the speed vector to the player's true position
            TruePos += speed;
        }

        //Pre: door is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a door 
        public override void CollisionWith(Door door)
        {
            //Checks if player is on top of door, if so set isGrounded to true
            if (TypeOfCollision(door) == CollisionType.Bottom)
            {
                isGrounded = true;
            }

            //Checks if player has keys and door is visible, if so decrement keyCount and set door's visibility to false
            if (keyCount > 0 && door.Visibility)
            {
                --keyCount;
                door.Visibility = false;
            }
            //If player has no keys and door is visible, block the player from moving into it
            else if (keyCount == 0 && door.Visibility)
            {
                BlockedCollision(door);
            }
        }

        //Pre: crate is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a crate 
        public override void CollisionWith(Crate crate)
        {
            //Checks if player is on top of door, if so set isGrounded to true
            if (TypeOfCollision(crate) == CollisionType.Bottom)
            {
                isGrounded = true;
            }

            //Checks if player reached his target spot, if so invoke ResetCollideCrate event
            if (TruePos.X == targetSpot)
            {
                ResetCollideCrate.Invoke();
            }

            //Checks if crate still collides with player, if so block the player from moving into it
            if (BoxCollision(crate))
            {
                BlockedCollision(crate);
            }
        }

        //Pre: wall is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a wall 
        public override void CollisionWith(Wall wall)
        {
            //Checks if player is on top of door, if so set isGrounded to true
            if (TypeOfCollision(wall) == CollisionType.Bottom)
            {
                isGrounded = true;
            }

            //block the player from moving into the wall
            BlockedCollision(wall);
        }

        //Pre: gem is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a gem 
        public override void CollisionWith(Gem gem)
        {
            //Checks if collect is true and gem is visible, if so increment gemCount and set its visibility to false
            if (gem.Visibility && collect)
            {
                ++gemCount;
                gem.Visibility = false;
            }
        }

        //Pre: key is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a key 
        public override void CollisionWith(Key key)
        {
            //Checks if collect is true and gem is visible, if so increment key counts and set its visibility to false
            if (key.Visibility && collect)
            {
                ++keyCount;
                ++keysCollected;
                key.Visibility = false;
            }
        }

        //Pre: spike is not null
        //Post: N/A
        //Description: Handle's the player's response to colliding with a spike 
        public override void CollisionWith(Spikes spike)
        {
            isAlive = false;
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Calls given object's collision with function with itself 
        public override void InformCollision(GameObject obj)
        {
            obj.CollisionWith(this);
        }

        //Pre: N/A
        //Post: Returns if player is stationary
        //Description: Checks if player is stationary and returns result
        public override bool IsNotMoving()
        {
            return speed.X == 0 && (int)speed.Y == 0 && isGrounded;
        }

        //Pre: N/A
        //Post: Returns push variable
        //Description: Returns push variable
        public bool GetPush()
        {
            return push;
        }

        //Pre: N/A
        //Post: Returns keysCollected variable
        //Description: Returns keysCollected variable
        public int GetKeysCollected()
        {
            return keysCollected;
        }

        //Pre: N/A
        //Post: Returns gemCount variable
        //Description: Returns gemCount variable
        public int GetGemsCollected()
        {
            return gemCount;
        }

        //Pre: N/A
        //Post: Returns isAlive variable
        //Description: Returns isAlive variable
        public bool IsAlive()
        {
            return isAlive;
        }
    }
}
