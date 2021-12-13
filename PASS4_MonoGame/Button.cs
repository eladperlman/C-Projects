//Author Name: Elad Perlman
//File Name: Button
//Project Name: PASS4_MonoGame
//Date Created: Jan 1st 2021
//Date Modified: Jan 27th 2021
//Description: This class takes care of all the logic and physics of a button, as well as drawing it 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Button
    {
        //Stores the buttons image and destination rectangle
        Texture2D buttonImg;
        Rectangle buttonRec;

        //Stores mouse's x and y position's
        Vector2 mouse;

        //Stores color of the button
        Color color = Color.White;

        //Pre: buttonImg is not null, buttonPos's x and y components are both positive floats
        //Post: N/A
        //Description: Sets global buttonImg to its local one, and sets up the buttons rectangle
        public Button(Texture2D buttonImg, Vector2 buttonPos)
        {
            this.buttonImg = buttonImg;
            buttonRec = new Rectangle((int)buttonPos.X, (int)buttonPos.Y, buttonImg.Width, buttonImg.Height);
        }

        //Pre: N/A
        //Post: Returns if mouse was clicked
        //Description: checks if mouse is on the button and if the mouse clicked on the button
        public bool ButtonPressed()
        {
            //Stores if mouse was clicked
            bool mouseClicked = false;

            //Sets the mouses x and y current position
            mouse.X = Mouse.GetState().X;
            mouse.Y = Mouse.GetState().Y;

            //Checks if mouse is on the button
            if (mouse.X >= buttonRec.X && mouse.X <= buttonRec.X + buttonRec.Width && mouse.Y >= buttonRec.Y && mouse.Y <= buttonRec.Y + buttonRec.Height)
            {
                //Sets button color to red
                color = Color.Red;

                //Checks if mouse was clicked
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //Sets mouse clicked to true
                    mouseClicked = true;
                }
            }
            else
            {
                //Sets the buttons color to its original color
                color = Color.White;
            }

            //Returns if mouse was clicked
            return mouseClicked;
        }

        //Pre: spriteBatch is a valid SpriteBatch
        //Post: N/A
        //Description: Draws the button
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonImg, buttonRec, color);
        }
    }
}
