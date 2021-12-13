//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Goal.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to a goal game object 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Goal : GameObject
    {
        //Pre: x, and y are between the 0 and the game window's dimensions
        //Post: N/A
        //Description: Calls the gameobject's constructor with goal image and its position
        public Goal(int x, int y) : base(MainGame.gameObjectsImg[MainGame.GOAL], x, y)
        {
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Calls given object's collision with function with itself 
        public override void InformCollision(GameObject obj)
        {
            obj.CollisionWith(this);
        }
    }
}
