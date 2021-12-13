//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Crate.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to a crate game object 
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Crate : GameObject
    {
        //Stores if crates collided with crates from any horizontal or vertical direction
        private bool[] collideCrate = { false, false, false, false };

        //Stores delegate of NotifyCrateCollectableCol, and its event that happens when crate collides with collectable from above
        public delegate bool NotifyCrateCollectableCol();
        public event NotifyCrateCollectableCol CrateCollectableCol;

        //Stores delegate of NotifyPlayer, and its event that happens when crate collides with collectable not from above
        public delegate void NotifyPlayer(GameObject obj);
        public event NotifyPlayer CollectableCollision;

        //Stores if crate has a collectable on top of itself
        private bool isCollectableOnTop = false;

        //Pre: x and y are above 0 and below the game windows dimensions
        //Post: N/A
        //Description: Calls base class's constructor with crate image and its position
        public Crate(int x, int y) : base(MainGame.gameObjectsImg[MainGame.CRATE], x, y)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Calls base class's ResetObject function and calls crate's ResetCollideCrate function
        public override void ResetObject()
        {
            base.ResetObject();
            ResetCollideCrate();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Calls crate's ResetCollideCrate function
        public override void ResetCollisions()
        {
            ResetCollideCrate();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Applys gravity to crate, adds the speed vector to crate's true position and sets x speed to 0
        public override void Update()
        {
            speed.Y += gravity;

            TruePos += speed;

            speed.X = 0;
        }

        //Pre: crate is not null
        //Post: N/A
        //Description: Calculates type of collision for both crates and sets the appropiate direction of collision to true, as well as block crate from moving
        public override void CollisionWith(Crate crate)
        {
            collideCrate[(int)TypeOfCollision(crate)] = true;
            crate.collideCrate[(int)crate.TypeOfCollision(this)] = true;
            BlockedCollision(crate);
        }

        //Pre: door is not null
        //Post: N/A
        //Description: If door is visible, crate is blocked from moving into it
        public override void CollisionWith(Door door)
        {
            if (door.Visibility)
            {
                BlockedCollision(door);
            }
        }

        //Pre: gem is not null
        //Post: N/A
        //Description: Invokes CollectableCollision with gem, and checks if gem is visible if so crate is blocked from moving into it
        public override void CollisionWith(Gem gem)
        {
            CollectableCollision.Invoke(gem);

            if (gem.Visibility)
            {
                BlockedCollision(gem);
            }
        }

        //Pre: goal is not null
        //Post: N/A
        //Description: Crate is blocked from moving into goal
        public override void CollisionWith(Goal goal)
        {
            BlockedCollision(goal);
        }

        //Pre: key is not null
        //Post: N/A
        //Description: Invokes CollectableCollision with key, and checks if key is visible if so crate is blocked from moving into it
        public override void CollisionWith(Key key)
        {
            CollectableCollision.Invoke(key);

            if (key.Visibility)
            {
                BlockedCollision(key);
            }
        }

        //Pre: spike is not null
        //Post: N/A
        //Description: Crate is blocked from moving into spike
        public override void CollisionWith(Spikes spike)
        {
            BlockedCollision(spike);
        }

        //Pre: wall is not null
        //Post: N/A
        //Description: Crate is blocked from moving into wall
        public override void CollisionWith(Wall wall)
        {
            BlockedCollision(wall);
        }

        //Pre: player is not null
        //Post: N/A
        //Description: Handles the logic of the response of the crate when it collides with player
        public override void CollisionWith(Player player)
        {
            //Checks if player push is true
            if (player.GetPush())
            {
                //Stores isCollectableOnTop to the result of the CrateCollectableCol event
                isCollectableOnTop = CrateCollectableCol.Invoke();

                //Checkf is no collectable is on top, player collided from the side and if no crates collide from the horizontal direction or above
                if (!isCollectableOnTop && (TypeOfCollision(player) == CollisionType.Right || TypeOfCollision(player) == CollisionType.Left) &&
                !collideCrate[(int)CollisionType.Right] && !collideCrate[(int)CollisionType.Left] &&
                !collideCrate[(int)CollisionType.Top])
                {
                    //Sets x speed to player's x speed, and moves the crate by that speed, and Invokes move event  
                    speed.X = player.GetSpeed().X;
                    TruePos += new Vector2(speed.X, 0);
                    InvokeMove(this);
                }
            }
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Calls given object's collision with function with itself 
        public override void InformCollision(GameObject obj)
        {
            obj.CollisionWith(this);
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Calls base class's blockCollision with obj and checks if y speed is 0, if so set x speed to 0
        protected override void BlockedCollision(GameObject obj)
        {
            base.BlockedCollision(obj);

            if (speed.Y == 0)
            {
                speed.X = 0;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: sets each collideCrate to false
        private void ResetCollideCrate()
        {
            for (int i = 0; i < collideCrate.Length; ++i)
            {
                collideCrate[i] = false;
            }
        }
    }
}
