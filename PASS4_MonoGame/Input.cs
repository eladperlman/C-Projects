//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Input.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to user input 
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
    class Input
    {
        //Stores current and previous key states 
        private KeyboardState currentKeyState;
        private KeyboardState previousKeyState;
        
        //Stores game's output font
        private SpriteFont font;

        //Stores a public static instance of Input
        public static Input Instance;

        //Stores a delgate Notify that takes a queue of chars and an positive int at maximum value of 68
        public delegate void Notify(Queue<char> commands, int commandLength);
        
        //Stores a command valid event 
        public event Notify CommandValid;

        //Stores user's command sequence
        private string command = "";

        //Stores the characters of a starting and closing brackets
        private const char START_BRACKET = 'S';
        private const char FINISH_BRACKET = 'F';

        //Stores all the valid characters a player can input while writing his command sequence
        private char[] validChars = { 'S', 'A', 'D', 'Q', 'E', 'C', 'S', 'F', '+', '-', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        //Stores ASCII value of space
        private const int SPACE = 32;

        //Stores input error message
        private string errorMessage = "";

        //Stores weather an input error has occured
        private bool errorOccured = false;

        //Stores an expanded queue of the player's commands
        private Queue<char> commands = new Queue<char>();

        //Stores the maximum amount of character for a command sequence
        private const int MAX_CHARACTERS = 68;

        private const int KEYS_COUNT = 130;

        //Pre: N/A
        //Post: N/A
        //Description: Sets font to MainGame's font 
        public Input()
        {
            font = MainGame.commandFont;
        }

        //Pre: N/A
        //Post: Returns currentKeyState
        //Description: Sets previous keyState to last updates currentKey State and sets currentKeyState to keyboard's keyState
        public KeyboardState GetState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            return currentKeyState;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Calls GetState function
        public void Update()
        {
            GetState();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Calls the GameEntryUpdate function
        public void CommandEntry()
        {
            GameEntryUpdate();
        }

        //Pre: N/A
        //Post: Returns message + character that was pressed
        //Description: Checks all keys if they were pressed, if so it returns message + that key, if not it returns the message
        public string KeyboardInput(string message)
        {
            //Increments through each character in keyboard
            for (int i = SPACE; i < KEYS_COUNT; ++i)
            {
                //Checks if current character was pressed, if so return message plus that character
                if (HasBeenPressed((Keys)i) && (Keys)i != Keys.Back)
                {
                    return message + Convert.ToChar(i);
                }

                if (HasBeenPressed(Keys.Back) && message.Length > 0)
                {
                    return message.Remove(message.Length - 1, 1);
                }
            }

            //Returns message
            return message;
        }

        //Pre: message is a valid string
        //Post: Returns the message plus the character the user entered this update
        //Description: Allows user to input command sequence using only the valid characters
        public string InputCommand(string message)
        {
            //Checks if message is not over the max character limit
            if (message.Length < MAX_CHARACTERS)
            {
                //Increments through each valid character
                for (int i = 0; i < validChars.Length; ++i)
                {
                    //Checks if current valid character was pressed, if so return message plus that character
                    if (HasBeenPressed((Keys)validChars[i]))
                    {
                        return message + validChars[i];
                    }
                }

                //Checks if plus was pressed, if so return message + plus
                if (HasBeenPressed(Keys.OemPlus))
                {
                    return message + '+';
                }

                //Checks if minus was pressed, if so return message + minus
                if (HasBeenPressed(Keys.OemMinus))
                {
                    return message + '-';
                }
            }

            //Checks if backspace was entered and if message is not empty, if so returns the message minus the last character
            if (HasBeenPressed(Keys.Back) && message.Length > 0)
            {
                return message.Remove(message.Length - 1, 1);
            }

            //Returns message
            return message;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Handles all the input logic of the game entry state
        private void GameEntryUpdate()
        {
            //Checks if error hasn't occured
            if (!errorOccured)
            {
                //Stores the newCommand by calling InputCommand with command
                string newCommand = InputCommand(command);

                //Checks if player entered any valid characters, if so set command to newCommand
                if (newCommand != command)
                {
                    command = newCommand;
                }

                //Check if enter was pressed
                if (HasBeenPressed(Keys.Enter))
                {
                    //Check if command is valid, if so reset command and error message, and invoke CommandValid with commands and command length
                    if (CheckCommand())
                    {
                        errorMessage = "";
                        CommandValid.Invoke(commands, command.Length);
                    }
                    else
                    {
                        //Set errorOccured to true, and add Press ENTER to try to the error message
                        errorOccured = true;
                        errorMessage += " Press ENTER to retry";
                    }
                }
            }
            else
            {
                //Check if enter was pressed, if so reset errorMessage and command, as well as set errorOccured to false
                if (HasBeenPressed(Keys.Enter))
                {
                    errorMessage = "";
                    command = "";
                    errorOccured = false;
                }
            }
            
        }

        //Pre: N/A
        //Post: Returns if command is valid
        //Description: Checks if command is valid and returns the result
        private bool CheckCommand()
        {
            //Stores index of current open bracket
            int sLoop = -1;

            //Store loop count and the amount of loops that already occured
            int loopCount = 1;
            int loopAmount = 0;

            //Check if command is empty, if so set appropiate error message and return false
            if (command.Length == 0)
            {
                errorMessage = "The command sequence is empty!";
                return false;
            }

            //Increments through each character in command
            for (int i = 0; i < command.Length; ++i)
            {
                //Checks if current character is an open bracket
                if (command[i] == START_BRACKET)
                {
                    //Check if another start bracket is open, if so set appropiate error message and return false
                    if (sLoop != -1)
                    {
                        errorMessage = "Loop has to be closed before starting another one! At Character: " + (i + 1);
                        return false;
                    }

                    //Checks if current character is the last character or if the next character is not a number
                    if (i + 1 == command.Length || !(command[i + 1] > '0' && command[i + 1] <= '9'))
                    {
                        //Sets error message to the amount of loops was not indicated and returns false
                        errorMessage = "The amount of loops has to be indiciated right after opening one! At Character: " + (i + 1);
                        return false;
                    }

                    //Sets loop amount to the number of loops and sets sLoop to the first character in the loop
                    loopAmount = command[i + 1] - '0';
                    sLoop = i + 2;
                    
                    //Increment i and continue to the next iteration
                    ++i;
                    continue;
                }

                //Check if current character is a number, if so set appropiate error message and return false
                else if (command[i] >= '1' && command[i] <= '9')
                {
                    errorMessage = "Numbers are not a valid operation, they are only used as loop counts! At Character: " + (i + 1);
                    return false;
                }

                //Check if current character is a closing bracket
                else if (command[i] == FINISH_BRACKET)
                {
                    //Check if no opening brackets was used, if so set appropiate error message and return false
                    if (sLoop == -1)
                    {
                        errorMessage = "Loop has to be open before using a closing bracket! At Character: " + (i + 1);
                        return false;
                    }

                    //Check if loop is empty, if so set appropiate error message and return false
                    if (i - sLoop == 0)
                    {
                        errorMessage = "No operations exists in the loop! At Character: " + (i + 1);
                        return false;
                    }

                    //Check if loopCount is not equal to the number of loops, if so set i back to sLoop and increment loopCount
                    if (loopCount < loopAmount)
                    {
                        i = sLoop;
                        ++loopCount;
                    }
                    else
                    {
                        //Set sLoop to -1 and loopCount to 1 and continue to the next iteration
                        sLoop = -1;
                        loopCount = 1;
                        continue;
                    }
                }

                //Enqueue current character to commands
                commands.Enqueue(command[i]);
            }

            //Check if a loop was left open, if so set appropiate error message and return false
            if (sLoop != -1)
            {
                errorMessage = "Loop has to be closed! Last Character";
                return false;
            }

            //return true
            return true;
        }

        //Pre: key is not null
        //Post: Returns if key is currently pressed and wasn't pressed last update
        //Description Checks if key is currently pressed and wasn't pressed last update and returns result
        public bool HasBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        //Pre: list is not null, Filter is not null
        //Post: Returns a sorted list based on the filter function
        //Description: Sorts a given list based on the filter function that is passed in
        public string[] MergeSort(string[] list, Func<string, string> Filter)
        {
            //Checks if list has 1 or fewer elements, if so return list
            if (list.Length <= 1)
            {
                return list;
            }

            //Returns the sorted list 
            return MergeSort(list, 0, list.Length - 1, Filter);
        }

        //Pre: list is not null, i and f are positive and not greater than list length - 1, filter is a valid Func
        //Post: Returns the list sorted based on the passed in Filter Func
        //Description: Sorts a given list based on the filter function that is passed in
        private string[] MergeSort(string[] list, int i, int f, Func<string, string> Filter)
        {
            //Stores the midpoint of the indecies of the list
            int midpoint = (i + f) / 2;

            //Checks if the list is not 1 element or less, if not call Merge with MergeSort of the first half of the list 
            //with MergeSort of the second half while both pass in the filter function
            if (f - i != 0)
            {
                return Merge(MergeSort(list, i, midpoint, Filter), MergeSort(list, midpoint + 1, f, Filter), Filter);
            }

            //Returns the last element
            return new[] { list[f] };
        }

        //Pre: right and left are both not null, Filter is not null
        //Post: returns a sorted merged array of the two given arrays
        //Description: Sorts a merged array of the two given arrays and returns the result
        private string[] Merge(string[] right, string[] left, Func<string, string> Filter)
        {
            //Creates an array of the size of the sum of the two arrays
            string[] mergedList = new string[right.Length + left.Length];

            //stores the index number of the low value of the left array and the low value of the right array
            int rightLow = 0;
            int leftLow = 0;

            //Iterates through each element in the mergedList
            for (int i = 0; i < mergedList.Length; ++i)
            {
                //Checks if rightLow is equal to length of right array, 
                //if so set current character of mergedList to element leftLow of left, and increment leftLow
                if (rightLow == right.Length)
                {
                    mergedList[i] = left[leftLow];
                    leftLow++;
                }
                //Checks if leftLow is equal to length of left array, 
                //if so set current character of mergedList to element rightLow of right, and increment rightLow
                else if (leftLow == left.Length)
                {
                    mergedList[i] = right[rightLow];
                    rightLow++;
                }
                //Checks if rightLow is greater than the length of right array, 
                //if so set current character of mergedList to element leftLow of left, and increment leftLow
                else if (CompareTwoStrings(Filter(right[rightLow]), Filter(left[leftLow])) == -1)
                {
                    mergedList[i] = left[leftLow];
                    ++leftLow;
                }
                //Checks if leftLow is equal to length of left array, 
                //if so set current character of mergedList to element rightLow of right, and increment rightLow
                else
                {
                    mergedList[i] = right[rightLow];
                    ++rightLow;
                }
            }

            //returns mergedList
            return mergedList;
        }

        //Pre: array is not null, i and f are not greater than the array length - 1, num is not null
        //Post: returns the index of the found element with the corresponding name, if not returns -1
        //Description: Searches for name in array and returns the index of the found element with the corresponding name, if not returns -1
        public int BinarySearch(string[] array, int i, int f, string name, Func<string, string> Filter)
        {
            //Calculates the midpoint of the given indecies
            int midPoint = (f + i) / 2;

            //Checks if array has elements
            if (array.Length > 0)
            {
                //Checks if name is equal to the array element with midpoint index, if so return midPoint 
                if (name == Filter(array[midPoint]))
                {
                    return midPoint;
                }
            }

            //If string is empty then return -1
            if (f - i <= 0)
            {
                return -1;
            }
            //If name is more than array at midpoint then return BinarySearch with the second half of the array
            else if (Filter(name).CompareTo(Filter(array[midPoint])) == 1)
            {
                return BinarySearch(array, midPoint + 1, f, name, Filter);
            }

            //return BinarySearch with the first half of the array
            return BinarySearch(array, i, midPoint - 1, name, Filter);
        }

        //Pre: item1 and item2 are not null
        //Post: returns the result of the comparsion
        //Description: Compares two string and returns result
        private static int CompareTwoStrings(string item1, string item2)
        {
            try
            {
                //Converts item1 and item2 to int and stores it into num1 and num2
                int num1 = Convert.ToInt32(item1);
                int num2 = Convert.ToInt32(item2);

                //Returns which one is bigger 
                return num1 < num2 ? -1 : 1;
            }
            //Catches exception
            catch (Exception)
            {
                //Returns the compare method result of the two strings
                return item1.CompareTo(item2);
            }
        }

        //Pre: spriteBatch is not empty
        //Post: N/A
        //Description: Draws all the input aspects of the game
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draws the command and its character count
            spriteBatch.DrawString(font, "Command: " + command, new Vector2(30, 560), Color.White);
            spriteBatch.DrawString(font, "Characters: " + command.Length + "/68", new Vector2(1050, 560), Color.White);

            //Checks if error message isn't empty, if so draw the error message
            if (errorMessage != "")
            {
                spriteBatch.DrawString(font, "Error: " + errorMessage, new Vector2(30, 605), Color.White);
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Sets command to empty
        public void ResetCommand()
        {
            command = "";
        }
    }
}
