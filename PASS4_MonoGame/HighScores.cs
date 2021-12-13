//Author Name: Elad Perlman
//Project Name: PASS4_MonoGame
//File Name: Higschores.cs
//Date Created: Dec, 15th, 2020
//Date Modified: Jan, 27th, 2021
//Description: Handles all the logic and graphics relating to higscores
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS4_MonoGame
{
    static class HighScores
    {
        //Stores the background img and rectangle, and initilizes them
        private static Texture2D bgImg = MainGame.highScoresBgImg;
        private static Rectangle bgRec = new Rectangle(0, 0, MainGame.windowWidth, MainGame.windowHeight);

        //Stores the scores lists that are sorted by name and scores
        private static string[] sortedByNames;
        private static string[] sortedByScores;

        public static void SortStatsFile()
        {
            //Reads all line and stores the sorted by names and by score basded on the merge sort
            string[] stats = File.ReadAllLines("../../../../Content/Scores.txt");
            sortedByNames = Input.Instance.MergeSort(stats, s => s.Split(',')[0]).Reverse().ToArray();
            sortedByScores = Input.Instance.MergeSort(stats, s => s.Split(',')[1]).Reverse().ToArray();
        }

        //Pre: spriteBatch is not null
        //Post: N/A
        //Description: Draws the background
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRec, Color.White);
        }

        //Pre: N/A
        //Post: returns the first 10 scores sorted by score
        //Description: returns the first 10 scores sorted by score
        public static string[] GetTop10()
        {
            return sortedByScores.Take(10).ToArray();
        }

        //Pre: N/A
        //Post: returns the first 10 scores sorted by name
        //Description: returns the first 10 scores sorted by name
        public static string[] GetSortedByName()
        {
            return sortedByNames;
        }
    }
}
