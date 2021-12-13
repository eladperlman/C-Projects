//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: GameObject.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to any game object
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class GameObject
    {
        //Stores object's visibility
        public bool Visibility { get; set; } = true;

        //Stores object's image and rectangle
        protected Texture2D img;
        protected Rectangle rec;

        //Stores object's original position and their true position
        protected Point ogPos;
        private Vector2 truePos;

        //Stores the constant values for x and y axis
        private const int X = 0;
        private const int Y = 1;

        //Stores delegate type Notify that takes a gameobject that is not null
        public delegate void Notify(GameObject obj);

        //Stores event Move 
        public event Notify Move;

        //Stores true position of object
        public Vector2 TruePos
        {
            //gets truePos
            get => truePos;

            set
            {
                //Sets x and y position of truePos to clamp result
                truePos.X = Clamp(0, value.X, MainGame.windowWidth - rec.Width, X);
                truePos.Y = Clamp(0, value.Y, MainGame.playWindowHeight - rec.Height, Y);

                //Sets rec location to truePos
                rec.Location = truePos.ToPoint();
            }
        }

        //Pre: min max and value are all valid floats, axis is either 1 or -1
        //Post: Returns the value is either between the window's dimensions or either the minimum point or max point of the screen
        //Description: Checks and returns the value is either between the window's dimensions or either the minimum point or max point of the screen
        private float Clamp(float min, float value, float max, int axis)
        {
            float result = Math.Min(Math.Max(min, max), Math.Max(Math.Min(min, max), value));

            if (result == min || result == max)
            {
                if (axis == X)
                {
                    speed.X = 0;
                }
                else
                {
                    speed.Y = 0;
                }
            }
            return result;
        }

        //Stores object's speed
        protected Vector2 speed = Vector2.Zero;

        //Stores the gravity value
        protected float gravity = 30f / MainGame.FPS;

        //Stores object's original speed
        protected Vector2 ogSpeed = new Vector2(3, 8);

        //Stores all collision types
        public enum CollisionType { Right, Left, Top, Bottom};

        //Pre: x and y are positive ints that are within the window's dimensions, img is not null
        //Post: N/A
        //Description: Sets up object's image, rectangle and positions
        public GameObject(Texture2D img, int x, int y)
        {
            this.img = img;

            ogPos = new Point(x, y);
            TruePos = new Vector2(x, y);
            rec = new Rectangle(x, y, img.Width, img.Height);
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets truPos to ogPos, visibility to true and speed to 0
        public virtual void ResetObject()
        {
            TruePos = new Vector2(ogPos.X, ogPos.Y);

            Visibility = true;
            speed = Vector2.Zero;
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Invokes move event with obj
        protected void InvokeMove(GameObject obj)
        {
            Move.Invoke(obj);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by crate to reset their collisions
        public virtual void ResetCollisions()
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by crate to check their collisions above
        public virtual void CheckCollisionAbove()
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with crate
        public virtual void CollisionWith(Crate crate)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with door
        public virtual void CollisionWith(Door door)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with gem
        public virtual void CollisionWith(Gem gem)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with goal
        public virtual void CollisionWith(Goal goal)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with key
        public virtual void CollisionWith(Key key)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with player
        public virtual void CollisionWith(Player player)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with spike
        public virtual void CollisionWith(Spikes spike)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to implement their reaction to a collision with wall
        public virtual void CollisionWith(Wall wall)
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects to inform other objects of collisions
        public virtual void InformCollision(GameObject obj) 
        {
        }

        //Pre: N/A
        //Post: N/A
        //Description: Used by objects as the general update method
        public virtual void Update()
        {
        }

        //Pre: N/A
        //Post: returns the side that collision occured on
        //Description: Checks which side a collision occured on
        protected CollisionType TypeOfCollision(GameObject obj)
        {
            //Calculates average of widths and heightsof both rectangles
            float w = 0.5f * (rec.Width + obj.rec.Width);
            float h = 0.5f * (rec.Height + obj.rec.Height);

            //Calculates difference in centers of the two rectangles
            float dx = rec.Center.X - obj.rec.Center.X;
            float dy = rec.Center.Y - obj.rec.Center.Y;

            //Stores the change in width and height for both rectangles
            float wy = w * dy;
            float hx = h * dx;

            //Checks if the change in width is higher than the one in height
            if (wy > hx)
            {
                //Checks if it is still bigger than the negative value of the change in height, if so return top collision
                if (wy > -hx)
                {
                    return CollisionType.Top;
                }

                //return right collision
                return CollisionType.Right;
            }
            else
            {
                //Checks if change in widht is bigger than the negative value of the change in height, if so return left collision
                if (wy > -hx)
                {
                    return CollisionType.Left;
                }

                //return bottom collision
                return CollisionType.Bottom;
            }
        }

        //Pre: otherObj is not null
        //Post: N/A
        //Description: Blocks the gameobject from travelling within the other object
        protected virtual void BlockedCollision(GameObject otherObj)
        {
            //Stores difference of other object's right side and this object's left
            float futureLeft = otherObj.GetRec().Right - (rec.Left);

            //Stores difference of other object's left side and this object's right
            float futureRight = otherObj.GetRec().Left - (rec.Right);

            //Stores difference of other object's bottom and this object's top
            float futureTop = otherObj.GetRec().Bottom - (rec.Top);

            //Stores difference of other object's top and this object's bottom
            float futureBottom = otherObj.GetRec().Top - (rec.Bottom);

            //Store type of collision that occured between the two objects
            CollisionType collisionType = TypeOfCollision(otherObj);

            //Checks if it is a left collision, if so move x location by futureLeft, and set x speed to 0
            if (collisionType == CollisionType.Left)
            {
                TruePos += new Vector2(futureLeft, 0);
                speed.X = 0;
            }
            //Checks if it is a right collision, if so move x location by futureRight, and set x speed to 0
            else if (collisionType == CollisionType.Right)
            {
                TruePos += new Vector2(futureRight, 0);
                speed.X = 0;
            }
            //Checks if it is a top collision, if so move x location by futureTop, and set y speed to 0
            else if (collisionType == CollisionType.Top)
            {
                TruePos += new Vector2(0, futureTop);
                speed.Y = 0;
            }
            else
            {
                //move y location by futureBottom, and set y speed to 0
                TruePos += new Vector2(0, futureBottom);
                speed.Y = 0;
            }
        }

        //Pre: obj is not null
        //Post: returns if both objects collided
        //Description: Checks and returns if both objects collided
        public bool BoxCollision(GameObject obj)
        {
            return rec.Intersects(obj.rec);
        }

        //Pre: N/A
        //Post: Returns if object's speed is 0 in both directions
        //Description: Checks and returns if object's speed is 0 in both directions
        public virtual bool IsNotMoving()
        {
            return speed.X == 0 && (int)speed.Y == 0;
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: if object is visible then draw it
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visibility)
            {
                spriteBatch.Draw(img, rec, Color.White);
            }
        }

        //Pre: N/A
        //Post: returns rec
        //Description: returns rec
        public Rectangle GetRec()
        {
            return rec;
        }

        //Pre: N/A
        //Post: returns speed
        //Description: returns speed
        public Vector2 GetSpeed()
        {
            return speed;
        }
    }
}
