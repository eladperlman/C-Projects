//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: MainGame.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: The player will be shown a side-view, single-screen platformer level made up blocks.  
//These blocks come in a variety of types including a goal flag, brick walls, blocks with spikes coming from the bottom, 
//left and right, locked doors and pushable crates. The player must then navigate the level to collect all the gems and keys, 
//if necessary, and then make their way to the level-end goal.  
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PASS4_MonoGame
{
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Stores the possible gamestates
        private enum GameState { Instructions, Menu, Game_Entry, Game_Excection, Results, Name_Entry, HighScores, End_Results };

        //Stores current gamestate
        private GameState gameState = GameState.Menu;

        //Stores font used for every text output
        public static SpriteFont commandFont;

        //Stores the background image and rectangle for the UI section
        private Texture2D commandBgImg;
        private Rectangle commandBgRec;

        //Stores the background img and rectangle for the level
        private Texture2D gameBgImg;
        private Rectangle gameBgRec;

        //Stores the background img and rectangle for the command legend
        private Texture2D commandLegendImg;
        private Rectangle commandLegendRec;

        //Stores the empty bar and progress bar imgs and rectangle for the empty bar
        public static Texture2D emptyBarImg;
        public static Rectangle emptyBarRec;
        public static Texture2D progressBarImg;

        //Stores window's dimensions
        public static int windowWidth;
        public static int windowHeight;

        //Stores height of the window minus the UI height
        public static int playWindowHeight;

        //Stores the FPS of the game
        public const int FPS = 60;

        //Stores all 5 levels of the game and the index of the current level
        private int curLevel = 0;
        private Level[] levels = new Level[4];

        //Stores weather show legend is on
        private bool showLegend = false;

        //Stores weather current level was passed successfully
        private bool levelPassed;

        //Stores messages for passing and failing a level
        private string levelPassedMsg = "Congratulations! Passed the level you have. Press ENTER to continue";
        private string levelFailedMsg = "You didn't pass the level. Press ENTER to retry";
        private string diedMsg = "You died to a spike. Press ENTER to retry";

        //Stores the message that will display based on the result of your commands
        private string resultMsg;

        //Stores user's total score for all levels
        private int totalScore = 0;

        //Stores all the game object images
        public readonly static Texture2D[] gameObjectsImg = new Texture2D[8];

        //Stores constant values for each game object
        public const int PLAYER = 0;
        public const int WALL = 1;
        public const int CRATE = 2;
        public const int GOAL = 3;
        public const int DOOR = 4;
        public const int SPIKES = 5;
        public const int GEM = 6;
        public const int KEY = 7;

        //Stores Content Manager
        public static ContentManager manager;

        //Stores username
        private string userName = "";

        //Stores outFile and inFile variables
        private StreamWriter outFile;
        public static StreamReader inFile;

        //Stores all the games buttons
        private Button playBtn;
        private Button instructionsBtn;
        private Button highScoresBtn;
        private Button exitBtn;
        private Button returnBtn;
        
        //Stores the imgaes of all the games buttons
        private Texture2D playBtnImg;
        private Texture2D instructionsBtnImg;
        private Texture2D highScoresBtnImg;
        private Texture2D exitBtnImg;
        private Texture2D returnBtnImg;

        //Stores the image and rectangle of the instructions 
        private Texture2D instructionsImg;
        private Rectangle instructionsRec;

        //Stores the spacing between lines
        private const int LINE_SPACING = 40;

        //Stores the highscores screen image
        public static Texture2D highScoresBgImg;

        //Stores the menu background image and rectangle
        private Texture2D menuBgImg;
        private Rectangle menuBgRec;

        //Stores the name entry background image and rectangle
        private Texture2D nameBgImg;
        private Rectangle nameBgRec;

        //Stores the name the user is looking for and the result
        private string nameSearch = "";
        private string foundName = "";

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            manager = Content;
        }
        
        protected override void Initialize()
        {
            //Makes mouse visible
            IsMouseVisible = true;

            //Sets window dimensions and game FPS
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 740;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / FPS);

            //Applys changes
            graphics.ApplyChanges();
            
            //Sets the dimension variables to the dimensions of the screen
            windowWidth = graphics.GraphicsDevice.Viewport.Width;
            windowHeight = graphics.GraphicsDevice.Viewport.Height;

            //Calculates the play window height
            playWindowHeight = windowHeight - 200;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Initilizes highscores image
            highScoresBgImg = Content.Load<Texture2D>("Images/Sprites/highscores");

            //Initilizes all the game button's images
            playBtnImg = Content.Load<Texture2D>("Images/Sprites/play button");
            instructionsBtnImg = Content.Load<Texture2D>("Images/Sprites/instructions button");
            highScoresBtnImg = Content.Load<Texture2D>("Images/Sprites/highscores button");
            exitBtnImg = Content.Load<Texture2D>("Images/Sprites/exit button");
            returnBtnImg = Content.Load<Texture2D>("Images/Sprites/return to menu");

            //Initilizes all the game buttons
            playBtn = new Button(playBtnImg, new Vector2(470, 150));
            instructionsBtn = new Button(instructionsBtnImg, new Vector2(470, 250));
            highScoresBtn = new Button(highScoresBtnImg, new Vector2(470, 350));
            exitBtn = new Button(exitBtnImg, new Vector2(470, 450));
            returnBtn = new Button(returnBtnImg, new Vector2(470, 530));

            //Intilizes the instructions image and its rectangle
            instructionsImg = Content.Load<Texture2D>("Images/Sprites/instructions");
            instructionsRec = new Rectangle(0, 0, windowWidth, windowHeight);

            menuBgImg = Content.Load<Texture2D>("Images/Sprites/back");
            menuBgRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Loads main font
            commandFont = Content.Load<SpriteFont>("Fonts/command font");

            //Loads game background image and initilizes its rectangle
            gameBgImg = Content.Load<Texture2D>("Images/Sprites/game bg");
            gameBgRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Loads command background image and initilizes its rectangle
            commandBgImg = Content.Load<Texture2D>("Images/Sprites/command bg");
            commandBgRec = new Rectangle(0, windowHeight - 200, windowWidth, 200);

            //Loads command legend image and initilizes its rectangle
            commandLegendImg = Content.Load<Texture2D>("Images/Sprites/command legend");
            commandLegendRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Loads empty bar and progress bar images
            emptyBarImg = Content.Load<Texture2D>("Images/Sprites/empty bar");
            progressBarImg = Content.Load<Texture2D>("Images/Sprites/progress bar");

            //Loads name entry background image and initilizes its rectangle
            nameBgImg = Content.Load<Texture2D>("Images/Sprites/name entry bg");
            nameBgRec = new Rectangle(0, 0, windowWidth, windowHeight);

            //Initilizes empty bar's rectangle
            emptyBarRec = new Rectangle(30, 630, emptyBarImg.Width, emptyBarImg.Height);

            //Loads all the game object's images
            gameObjectsImg[PLAYER] = Content.Load<Texture2D>("Images/Sprites/player");
            gameObjectsImg[WALL] = Content.Load<Texture2D>("Images/Sprites/wall");
            gameObjectsImg[CRATE] = Content.Load<Texture2D>("Images/Sprites/crate");
            gameObjectsImg[GOAL] = Content.Load<Texture2D>("Images/Sprites/goal");
            gameObjectsImg[DOOR] = Content.Load<Texture2D>("Images/Sprites/door");
            gameObjectsImg[SPIKES] = Content.Load<Texture2D>("Images/Sprites/spikes");
            gameObjectsImg[GEM] = Content.Load<Texture2D>("Images/Sprites/gem");
            gameObjectsImg[KEY] = Content.Load<Texture2D>("Images/Sprites/key");

            //Iterates through each level
            for (int i = 0; i < levels.Length; ++i)
            {
                //Initilizes the level and subscribes to level's out of command event
                levels[i] = new Level("../../../../Content/Levels/Level " + (i + 1) + ".txt");
                levels[i].OutOfCommands += (levelPassed, isAlive) =>
                {
                    //Sets result message to the appropiate one based on level result
                    resultMsg = levelPassed ? levelPassedMsg : levelFailedMsg;

                    //isAlive is false then set resultMsg to diedMsg
                    if (!isAlive)
                    {
                        resultMsg = diedMsg;
                    }
                    
                    //Resets input command
                    Input.Instance.ResetCommand();

                    //Sets game state to results and levelPassed to the parameter levelPassed
                    gameState = GameState.Results;
                    this.levelPassed = levelPassed;
                };
            }

            //Loads the first level
            levels[curLevel].LoadLevel();
            //Subscribes to Input's CommandValid event

            //Creates a new instance of Input
            Input.Instance = new Input();

            //Subscribes to Input's CommandValid event with current levels set command function, commands is not null, 
            //and commandLength is a positive int that is below 69
            Input.Instance.CommandValid += (commands, commandLength) =>
            {
                levels[curLevel].SetCommand(commands, commandLength);
            };

            try
            {
                //Opens the file paths of the scores
                inFile = File.OpenText("../../../../Content/Scores.txt");
                inFile.Close();
            }

            //Catches any error
            catch (Exception e)
            {
                //Displays error message
                Console.WriteLine(e.Message);
            }

            //Subscribes to Input's command valid event
            Input.Instance.CommandValid += (c, l) => gameState = GameState.Game_Excection;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Calls Input's update
            Input.Instance.Update();

            //Based on the value of game state, the matching update method for the game state will be excecuted 
            switch (gameState)
            {
                case GameState.Instructions:
                    InstructionsUpdate();
                    break;
                case GameState.Menu:
                    MenuUpdate();
                    break;
                case GameState.HighScores:
                    HighScoresUpdate();
                    break;
                case GameState.Game_Entry:
                    GameEntryUpdate(gameTime);
                    break;
                case GameState.Game_Excection:
                    GameExecutionUpdate(gameTime);
                    break;
                case GameState.Results:
                    ResultsUpdate();
                    break;
                case GameState.End_Results:
                    EndResultsUpdate();
                    break;
                case GameState.Name_Entry:
                    NameEntryUpdate();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Begins spriteBatch
            spriteBatch.Begin();

            //Based on the value of game state, the matching draw method for the game state will be excecuted 
            switch (gameState)
            {
                case GameState.Instructions:
                    InstructionsDraw(spriteBatch);
                    break;
                case GameState.Menu:
                    MenuDraw(spriteBatch);
                    break;
                case GameState.HighScores:
                    HighScoresDraw(spriteBatch);
                    break;
                case GameState.Game_Entry:
                    GameEntryDraw(spriteBatch);
                    break;
                case GameState.Game_Excection:
                    GameExecutionDraw(spriteBatch);
                    break;
                case GameState.Results:
                    ResultsDraw(spriteBatch);
                    break;
                case GameState.End_Results:
                    EndResultsDraw(spriteBatch);
                    break;
                case GameState.Name_Entry:
                    NameEntryDraw(spriteBatch);
                    break;
            }
            
            //Ends spriteBatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic related to the instructions screen
        private void InstructionsUpdate()
        {
            if (returnBtn.ButtonPressed())
            {
                gameState = GameState.Menu;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic related to the menu screen
        private void MenuUpdate()
        {
            //Checks if play button was pressed, if so sets game state to game entry
            if (playBtn.ButtonPressed())
            {
                gameState = GameState.Game_Entry;
            }

            //Checks if instructions button was pressed, if so sets game state to instructions
            if (instructionsBtn.ButtonPressed())
            {
                gameState = GameState.Instructions;
            }

            //Checks if high scores button was pressed, if so sets game state to high scores
            if (highScoresBtn.ButtonPressed())
            {
                gameState = GameState.HighScores;
            }

            //Checks if exit button was pressed, if so exit game
            if (exitBtn.ButtonPressed())
            {
                Exit();
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic related to the highscores screen
        private void HighScoresUpdate()
        {
            //Check if enter has been pressed, if so do binary search for name
            if (Input.Instance.HasBeenPressed(Keys.Enter))
            {
                int foundNameIndex = Input.Instance.BinarySearch(HighScores.GetSortedByName(), 0, 
                                     HighScores.GetSortedByName().Length - 1, nameSearch, s => s.Split(',')[0]);
                
                //Checks if a name was found, if so set foundName to that name 
                if (foundNameIndex != -1)
                {
                    foundName = HighScores.GetSortedByName()[foundNameIndex];
                }
                else
                {
                    //Set found name to N/A
                    foundName = "N/A";
                }
            }

            //Checks if return button was pressed, if so set gamestate to menu and set nameSearch to empty
            if (returnBtn.ButtonPressed())
            {
                gameState = GameState.Menu;
                nameSearch = "";
                foundName = "";
            }

            //Sets nameSearch to new nameSearch
            nameSearch = Input.Instance.KeyboardInput(nameSearch);
        }

        //Pre: gameTime is not null
        //Post: N/A
        //Description: Handles all the logic related to the game entry screen
        private void GameEntryUpdate(GameTime gameTime)
        {
            //Calls Input's command entry function
            Input.Instance.CommandEntry();

            //Checks if L has been pressed, if so change showLegend to its opposite value
            if (Input.Instance.HasBeenPressed(Keys.L))
            {
                showLegend = !showLegend;
            }

            //Checks if X has been pressed, if so game state is set to menu
            if (Input.Instance.HasBeenPressed(Keys.X))
            {
                gameState = GameState.Menu;
            }

            //Increments current level's timer
            levels[curLevel].IncrementTimer(gameTime);
        }

        //Pre: gameTime is not null
        //Post: N/A
        //Description: Handles all the logic related to the game exceution screen
        private void GameExecutionUpdate(GameTime gameTime)
        {
            //Calls current level's update 
            levels[curLevel].Update(gameTime);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic related to the results screen
        private void ResultsUpdate()
        {
            //Checks if enter has been pressed
            if (Input.Instance.HasBeenPressed(Keys.Enter))
            {
                //Checks if level was passed successfully
                if (levelPassed)
                {
                    //Checks if current level is last level, if so reset all levels and set curLevel to 0 then set game state to end results
                    if (curLevel == levels.Length - 1)
                    {
                        curLevel = 0;
                        gameState = GameState.End_Results;
                    }
                    else
                    {
                        //Incriment curLevel, load up the next level, and set game state back to game entry
                        ++curLevel;
                        levels[curLevel].LoadLevel();
                        
                        //Subscribes to Input's CommandValid event
                        Input.Instance.CommandValid += (commands, commandLength) =>
                        {
                            levels[curLevel].SetCommand(commands, commandLength);
                        };

                        gameState = GameState.Game_Entry;
                    }

                    //Incriment total score by current level's score
                    totalScore += levels[curLevel].GetLevelScore();
                }
                else
                {
                    //Resets current level and sets game state back to game entry
                    levels[curLevel].ResetLevel();
                    gameState = GameState.Game_Entry;
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: If enter was pressed then game state is set to name entry
        private void EndResultsUpdate()
        {
            if (Input.Instance.HasBeenPressed(Keys.Enter))
            {
                gameState = GameState.Name_Entry;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the logic related to the name entry screen
        private void NameEntryUpdate()
        {
            //Checks if enter has been pressed
            if (Input.Instance.HasBeenPressed(Keys.Enter) && userName != "")
            {
                //Writes the new name and score to the file, and resets total score and name
                outFile = File.AppendText("../../../../Content/Scores.txt");
                outFile.WriteLine(userName + "," + totalScore);
                outFile.Close();
                totalScore = 0;
                userName = "";
                
                //Iterate through each level and call their ResetLevel function and reset timer
                foreach (Level level in levels)
                {
                    level.ResetLevel();
                    level.ResetTime();
                }

                //Sets game state to menu
                gameState = GameState.Menu;
            }

            //Sets userName to KeyboardInput while passing in userName
            userName = Input.Instance.KeyboardInput(userName);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the aspects of the instructions screen
        private void InstructionsDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(instructionsImg, instructionsRec, Color.White);
            returnBtn.Draw(spriteBatch);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the aspects of the menu screen
        private void MenuDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBgImg, menuBgRec, Color.White);
            playBtn.Draw(spriteBatch);
            instructionsBtn.Draw(spriteBatch);
            highScoresBtn.Draw(spriteBatch);
            exitBtn.Draw(spriteBatch);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draws all the aspects of the highscores screen
        private void HighScoresDraw(SpriteBatch spriteBatch)
        {
            HighScores.Draw(spriteBatch);
            HighScores.SortStatsFile();
            
            //Stores the top 10 scores
            string[] top10 = HighScores.GetTop10();

            //Iterates through each score in the top 10 scores and draws it 
            for (int i = 0; i < 10; ++i)
            {
                //If a score exists for current value of i then draw the score
                if (top10.Length > i)
                {
                    spriteBatch.DrawString(commandFont, (i + 1) + ". " + top10[i], new Vector2(420, 120 + LINE_SPACING * i), Color.White);
                }
                else
                {
                    //Draws the number it is on without the score
                    spriteBatch.DrawString(commandFont, (i + 1) + ". ", new Vector2(420, 120 + LINE_SPACING * i), Color.White);
                }
            }
            
            spriteBatch.DrawString(commandFont, "Searched Name: " + nameSearch, new Vector2(800, 120), Color.White);
            spriteBatch.DrawString(commandFont, "Result: " + foundName, new Vector2(800, 150), Color.White);
            returnBtn.Draw(spriteBatch);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the aspects of the game entry screen
        private void GameEntryDraw(SpriteBatch spriteBatch)
        {
            //Draws the background components of the game 
            DrawBackgroundComponents(spriteBatch);

            //Draws the current level timer
            levels[curLevel].DrawTimer(spriteBatch);

            //Calls the curernt level's game entry function
            levels[curLevel].GameEntryDraw(spriteBatch);

            //Calls Input's draw function
            Input.Instance.Draw(spriteBatch);

            //Draws the instructions to return to menu and to toggle command legend
            spriteBatch.DrawString(commandFont, "X to Return to Menu", new Vector2(30, 700), Color.White);
            spriteBatch.DrawString(commandFont, "L to Toggle Command Legend", new Vector2(950, 700), Color.White);

            //If showLegend is true, then draw the command legend
            if (showLegend)
            {
                spriteBatch.Draw(commandLegendImg, commandLegendRec, Color.White);
            }
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the aspects of the game execution screen
        private void GameExecutionDraw(SpriteBatch spriteBatch)
        {
            //Draws the background components of the game
            DrawBackgroundComponents(spriteBatch);

            //Draws the current level timer
            levels[curLevel].DrawTimer(spriteBatch);

            //Calls Input's draw function
            Input.Instance.Draw(spriteBatch);
            
            //Draws the empty bar and calls the game execution draw
            spriteBatch.Draw(emptyBarImg, emptyBarRec, Color.White);
            levels[curLevel].GameExecutionDraw(spriteBatch);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the aspects of the results screen
        private void ResultsDraw(SpriteBatch spriteBatch)
        {
            //Draws the background components of the game
            DrawBackgroundComponents(spriteBatch);

            //Calls the curernt level's game entry function
            levels[curLevel].GameEntryDraw(spriteBatch);

            //Calls the current level's game entry draw, and draws the result message of the level
            levels[curLevel].GameEntryDraw(spriteBatch);
            spriteBatch.DrawString(commandFont, "Status: " + resultMsg, new Vector2(400, 590), Color.White);
            
            //Checks if level was passed 
            if (levelPassed)
            {
                //Draws the current level's stats (time, command length, and score)
                spriteBatch.DrawString(commandFont, "Time: " + levels[curLevel].GetLevelTime(), new Vector2(400, 620), Color.White);
                spriteBatch.DrawString(commandFont, "Command Length: " + levels[curLevel].GetCommandLength(), new Vector2(400, 650), Color.White);
                spriteBatch.DrawString(commandFont, "Score: " + levels[curLevel].GetLevelScore(), new Vector2(400, 680), Color.White);
            }
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the end results components
        private void EndResultsDraw(SpriteBatch spriteBatch)
        {
            //Stores total time and score for all levels
            double totalTime = 0;
            int totalScore = 0;

            //Draws the background components of the game
            DrawBackgroundComponents(spriteBatch);

            //Calls the curernt level's game entry function
            levels[curLevel].GameEntryDraw(spriteBatch);

            //Iterates through each level
            for (int i = 0; i < levels.Length; ++i)
            {
                //Draws the current level's stats (time, command length, and score)
                spriteBatch.DrawString(commandFont, "Level " + (i + 1) + ": Time: " + levels[i].GetLevelTime() + ", Command Length: " + levels[i].GetCommandLength() + ", Score: " + levels[i].GetLevelScore(), new Vector2(400, 590 + 20 * i), Color.White);
                
                //Adds current level's time and score to total time and score
                totalTime += levels[i].GetLevelTime();
                totalScore += levels[i].GetLevelScore();
            }

            //Draws the total stats (time, command length, and score)
            spriteBatch.DrawString(commandFont, "Total Time: " + totalTime + ", Total Score: " + totalScore, new Vector2(400, 700), Color.White);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws the level's background image and the UI background
        private void DrawBackgroundComponents(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameBgImg, gameBgRec, Color.White);
            spriteBatch.Draw(commandBgImg, commandBgRec, Color.White);

            spriteBatch.DrawString(commandFont, "Level: " + (curLevel + 1), new Vector2(30, 10), Color.White);
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws all the aspects of the name entry screen
        private void NameEntryDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(nameBgImg, nameBgRec, Color.White);

            spriteBatch.DrawString(commandFont, "Your score: " + totalScore, new Vector2(400, 270), Color.White);
            spriteBatch.DrawString(commandFont, "Enter your Name: " + userName, new Vector2(400, 300), Color.White);
        }
    }
}
