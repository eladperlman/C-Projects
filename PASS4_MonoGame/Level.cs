//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Level.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to a level in the game
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    class Level
    {
        //Stores the level file its reading from
        private StreamReader inFile;

        //Stores all the gameobjects that are in the level
        private List<GameObject> gameObjects = new List<GameObject>();

        //Stores the player, and the goal's index in the gameObjects list
        private Player player;
        private int goalIndex;

        //Stores player's current action
        private string curAction;

        //Stores level score, and the level timer
        private int levelScore = 0;
        private double levelTimer = 0;

        //Stores total keys and gems collected
        private int totalKeys = 0;
        private int totalGems = 0;

        //Stores the rectangle of the progress bar
        private Rectangle progressBarRec;

        //Stores the a queue of all the commands
        private Queue<char> commands = new Queue<char>();

        //Stores the total amount of commands and the lenght of the command sequence string
        private int totalCommands;
        private int commandLength;

        //Stores delegate of NotifyMain that takes two valid boolean
        public delegate void NotifyMain(bool levelPassed, bool isDead);

        //Stores event of type NotifyMain when no more commands exist
        public event NotifyMain OutOfCommands;

        //Stores a dictionary of all valid characters and their action's description
        private Dictionary<char, string> commandDef = new Dictionary<char, string> 
        { 
            { 'A', "Move Left" }, { 'D', "Move Right" },
            { 'Q', "Jump Left" }, { 'E', "Jump Right" },
            { '+', "Push Right" }, { '-', "Push Left" },
            { 'C', "Collect" }
        };

        //Pre: level is not empty
        public Level(string level)
        {
            try
            {
                //Opens the path file of level and sets it to inFile
                inFile = File.OpenText(level);
            }

            //Catches any exception
            catch(Exception e)
            {
                //Displays the exception error to the console
                Console.WriteLine(e);
            }

            //Initilizes progressbar rec to have a width of zero
            progressBarRec = new Rectangle(MainGame.emptyBarRec.X, MainGame.emptyBarRec.Y, 0, MainGame.emptyBarRec.Height);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Adds all the gameObjects in the level file to the gameObjects list and stores their appropiate positions and types
        public void LoadLevel()
        {
            //Stores current line in file and its row number
            string line;
            int row = 0;

            //Iterates while the end of the file isn't reached
            while (!inFile.EndOfStream)
            {
                //Sets line to current line in level file
                line = inFile.ReadLine();

                //Increments through all characters in current line
                for (int i = 0; i < line.Length; ++i)
                {
                    //Based off the character, the appropiate gameobject is added to the list 
                    switch (Convert.ToInt32(line[i]) - '0')
                    {
                        case MainGame.PLAYER:
                            player = new Player(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row);
                            break;
                        case MainGame.WALL:
                            gameObjects.Add(new Wall(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            break;
                        case MainGame.CRATE:
                            Crate crate = new Crate(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row);
                            //Subsciribes to crate's CratCollectableCol and excecutes CrateCollectableCollision with crate when event happens
                            crate.CrateCollectableCol += () => CrateCollectableCollision(crate);

                            //Subsciribes to crate's CollectableCollision and excecutes CollectCollectable with c when event happens
                            crate.CollectableCollision += c => player.CollectCollectable(c);
                            
                            //Adds crate to gameObjects
                            gameObjects.Add(crate);
                            break;
                        case MainGame.GOAL:
                            gameObjects.Add(new Goal(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            goalIndex = gameObjects.Count - 1;
                            break;
                        case MainGame.DOOR:
                            gameObjects.Add(new Door(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            break;
                        case MainGame.SPIKES:
                            gameObjects.Add(new Spikes(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            break;
                        case MainGame.GEM:
                            gameObjects.Add(new Gem(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            //Increments totalGems
                            ++totalGems;
                            break;
                        case MainGame.KEY:
                            gameObjects.Add(new Key(MainGame.gameObjectsImg[MainGame.PLAYER].Width * i,
                                                                 MainGame.gameObjectsImg[MainGame.PLAYER].Height * row));
                            //Increments totalKeys
                            ++totalKeys;
                            break;
                    }
                }

                //Increments row
                ++row;
            }

            //Closes file
            inFile.Close();


            //Iterates through each gameObject and subscribes to its Move event and calls GameObjectCollisions with o when it sets off
            foreach (GameObject obj in gameObjects)
            {
                obj.Move += o => GameObjectCollisions(o);

                //Subsribes to player's ResetCollideCrate event and excecutes object's ResetCollisions when event goes off
                player.ResetCollideCrate += () => obj.ResetCollisions();
            }
        }

        //Pre: gameTime is not null
        //Post: N/A
        //Description: Increments levelTimer by the amount of ms passed from last update
        public void IncrementTimer(GameTime gameTime)
        {
            levelTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        //Pre: gameTime is not null
        //Post: N/A
        //Description: Handles all the logic of the level
        public void Update(GameTime gameTime)
        {
            //Increments timer
            IncrementTimer(gameTime);

            if (!player.IsAlive())
            {
                //Invokes the OutOfCommands event and passes false for passed level and false for isAlive
                OutOfCommands.Invoke(false, false);
                commands.Clear();
            }
            //Checks if all objects aren't moving
            if ((gameObjects.All(o => o.IsNotMoving()) && player.IsNotMoving()))
            {
                //Check if more commands exist, if so set curAction to the definition of the next action, 
                //and calls player ExcecuteCommand with the next command in the queue
                if (!commands.IsEmpty())
                {
                    curAction = commandDef[commands.Peek()];
                    player.ExcecuteCommand(commands.Dequeue());
                }
                else
                {
                    //Checks if player collides with goal and all gems were collected
                    if (player.BoxCollision(gameObjects[goalIndex]) && player.GetGemsCollected() == totalGems)
                    {
                        //Calculates levelScore, and invokes the OutOfCommands event and passes true for passed level and true for isAlive
                        levelScore = (int)levelTimer + (commandLength * 100);
                        OutOfCommands.Invoke(true, true);
                    }
                    else
                    {
                        //Invokes the OutOfCommands event and passes false for passed level and true for isAlive
                        OutOfCommands.Invoke(false, true);
                    }
                }
            }

            //Calculates progress bar's width based on the amount of commands excecuted out of the total commands
            progressBarRec.Width = (int)((double)(totalCommands - commands.Size()) / totalCommands * MainGame.emptyBarRec.Width);

            //Iterates through each gameObject
            foreach (GameObject obj in gameObjects)
            {
                //Calls gameObject's update function
                obj.Update();

                //Iterates through each gameObject
                foreach (GameObject otherObj in gameObjects)
                {
                    //Check the two gameObjects aren't identical and that they collide
                    if (obj != otherObj && obj.BoxCollision(otherObj))
                    {
                        //Check if both objects are crates, if so inform otherObject's collision with object
                        if (obj is Crate && otherObj is Crate)
                        {
                            otherObj.InformCollision(obj);
                        }
                        else
                        {
                            //Inform otherObject's collision with object and object's collision with otherObject
                            otherObj.InformCollision(obj);
                            obj.InformCollision(otherObj);
                        }
                    }
                }
            }

            //Calls the player's update function
            player.Update();

            //Iterates throguh each gameObject
            foreach (GameObject obj in gameObjects)
            {
                //Checks if object collided with player, if so Inform player's collision with object and object's collision with player
                if (obj.BoxCollision(player))
                {
                    player.InformCollision(obj);
                    obj.InformCollision(player);
                }
            }
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the gameobjects of the game
        public void GameEntryDraw(SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draw the timer of the level
        public void DrawTimer(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(MainGame.commandFont, "Time: " + levelTimer, new Vector2(30, 630), Color.White);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all game objects and stats such the current action and the user's gem and key count
        public void GameExecutionDraw(SpriteBatch spriteBatch)
        {
            //Calls the game entry draw
            GameEntryDraw(spriteBatch);

            //Draws all the count variables and current action as well as the progress bar
            spriteBatch.Draw(MainGame.progressBarImg, progressBarRec, Color.White);
            spriteBatch.DrawString(MainGame.commandFont, "Current Action: " + curAction, new Vector2(30, 690), Color.White);
            spriteBatch.DrawString(MainGame.commandFont, "Key Count: " + player.GetKeysCollected() + "/" + totalKeys, new Vector2(30, 600), Color.White);
            spriteBatch.DrawString(MainGame.commandFont, "Gem Count: " + player.GetGemsCollected() + "/" + totalGems, new Vector2(230, 600), Color.White);
        }

        //Pre: obj is not null
        //Post: N/A
        //Description: Checks collision between all game objects and the given objects
        public void GameObjectCollisions(GameObject obj)
        {
            //Iterates through each game object
            foreach (GameObject otherObj in gameObjects)
            {
                //Checks if current game object is not equal to given game object and if they collided
                if (obj != otherObj && obj.BoxCollision(otherObj))
                {
                    //Informs both game objects of the collision
                    otherObj.InformCollision(obj);
                    obj.InformCollision(otherObj);
                }
            }
        }

        //Pre: commands is not null, command length is a positive int below 69
        //Post: N/A
        //Description: Sets entered command to the queue command and the command length to the total commands
        public void SetCommand(Queue<char> commands, int commandLength) 
        {
            //Sets this commands to given commands, and totalCommands to commands size
            this.commands = commands;
            totalCommands = commands.Size();

            //Sets this commandLength to given commandLegth
            this.commandLength = commandLength;
        }

        //Pre: obj is not null
        //Post: Returns if given object collided with a collectable from above
        //Description: Checks and returns if given object collided with a collectable from above
        public bool CrateCollectableCollision(GameObject obj)
        {
            //Stores all objects that are right above the current object 
            IEnumerable<GameObject> objsAbove = gameObjects.Where(c => c.TruePos.X == obj.TruePos.X && (int)(c.TruePos.Y + c.GetRec().Height) == (int)obj.TruePos.Y);

            //Checks if any exist and if they are a collectable, if so return their visibility
            if (objsAbove.Count() != 0 && (objsAbove.First() is Key || objsAbove.First() is Gem))
            {
                return objsAbove.First().Visibility;
            }

            //Return false
            return false;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets all the aspects of the level
        public void ResetLevel()
        {
            //Iterates through each game object and resets them
            foreach (GameObject obj in gameObjects)
            {
                obj.ResetObject();
            }

            //Resets player
            player.ResetObject();

            //Sets progress bar width to 0
            progressBarRec.Width = 0;

            //Sets level score to 0
            levelScore = 0;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets the level timer
        public void ResetTime()
        {
            levelTimer = 0;
        }

        //Pre: N/A
        //Post: returns levelScore
        //Description: returns levelScore
        public int GetLevelScore()
        {
            return levelScore;
        }

        //Pre: N/A
        //Post: returns levelTimer
        //Description: returns levelTimer
        public double GetLevelTime()
        {
            return levelTimer;
        }

        //Pre: N/A
        //Post: returns commandLength
        //Description: returns commandLength
        public double GetCommandLength()
        {
            return commandLength;
        }
    }
}
