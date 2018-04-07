using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MazeSnake
{
    public static class Achievements
    {
        #region Fields

        public static List<Achievement> AchievementsList = new List<Achievement>();

        static Dictionary<int, AchievementCriteria> criteriaDict = new Dictionary<int, AchievementCriteria>();
        static Dictionary<int, string> imgDict = new Dictionary<int, string>();
        static Dictionary<int, AchievementMethod> rewardDict = new Dictionary<int, AchievementMethod>();
        static Dictionary<int, int> coinDict = new Dictionary<int, int>();

        #endregion

        #region Public Methods

        public static void Initialize(ContentManager content, Dictionary<int, AchievementCriteria> criteriaDictionary,
            Dictionary<int, string> imgDictionary, Dictionary<int, AchievementMethod> rewardDictionary, Color mazewormColor,
            Dictionary<int, int> coinDictionary)
        {
            criteriaDict = criteriaDictionary;
            imgDict = imgDictionary;
            rewardDict = rewardDictionary;
            coinDict = coinDictionary;

            // Fill the list with achievements
            int nextId = -1;
            AddAchievement("Fin", "Finish a maze", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("x20", "Finish 20 mazes", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("x86", "Finish 86 mazes", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Puzzle God", "Finish 1,000 mazes", AchievementDifficulty.Red, ++nextId, false, content);

            AddAchievement("The Twin", "Find MazeWorm", AchievementDifficulty.Red, ++nextId, true, content);
            AchievementsList[nextId].ImageColor = Color.SaddleBrown.Serialize();

            AddAchievement("A Spark", "Collect a powerup", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Lightning for Breakfast", "Collect 10 powerups", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Lightning for Lunch", "Collect 25 powerups", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Lightning for Dinner", "Collect 50 powerups", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("A Delicacy", "Collect 100 powerups", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Electrified", "Collect 500 powerups", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Living Power Source", "Collect 1,000 powerups", AchievementDifficulty.Red, ++nextId, true, content);

            AddAchievement("Poof!", "Teleport 10 times", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Wait...", "Teleport 50 times", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("Where am I?", "Teleport 100 times", AchievementDifficulty.Orange, ++nextId, false, content);

            // Achievements below rely on the fact that their ids stay the same. Removing this achievement will 
            // result in bugs unless the ids stay the same
            nextId++;
            //AddAchievement("Gee, thanks", "Fail", AchievementDifficulty.Orange, ++nextId, true, content);

            AddAchievement("Zoom!", "Have speed for a total of 1 minute", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Speedy", "Have speed for a total of 30 minutes", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("Wow!", "Have speed for a total of 1 hour", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Blink", "Have speed for a total of 1 day", AchievementDifficulty.Red, ++nextId, false, content);

            AddAchievement("Iron Fist", "Break 100 walls", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Outta My Way!", "Break 500 walls", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("Snake, SMASH!", "Break 1,000 walls", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Slayer of Weeds", "Break 5,000 walls", AchievementDifficulty.Red, ++nextId, false, content);

            AddAchievement("Master of MazeSnake", "Complete all of the achievements", AchievementDifficulty.Red, ++nextId, true,
                content);

            AddAchievement("Safe!", "Avoid 10 enemies", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Denied", "Avoid 50 enemies", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("Not Today", "Avoid 100 enemies", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Can't Touch This", "Avoid 500 enemies", AchievementDifficulty.Red, ++nextId, false, content);

            AddAchievement("Chilly", "Freeze the clock for a total of 1 minute", AchievementDifficulty.Green, ++nextId, false, content);
            AddAchievement("Cold", "Freeze the clock for a total of 30 minutes", AchievementDifficulty.Yellow, ++nextId, false, content);
            AddAchievement("Frozen", "Freeze the clock for a total of 1 hour", AchievementDifficulty.Orange, ++nextId, false, content);
            AddAchievement("Frigid", "Freeze the clock for a total of 1 day", AchievementDifficulty.Red, ++nextId, false, content);

            AddAchievement("HOW DID I GET HERE?!", "Teleport 500 times", AchievementDifficulty.Red, ++nextId, false, content);
        }

        public static Achievement GetAchievement(string name)
        {
            foreach (Achievement a in AchievementsList)
            {
                if (a.Name == name)
                {
                    return a;
                }
            }
            return null;
        }

        #endregion

        #region Private Methods

        private static void AddAchievement(string name, string desc, AchievementDifficulty difficulty, int id, bool mystery, ContentManager content)
        {
            AchievementsList.Add(new Achievement(content, GetImgKey(id), name, desc, difficulty, GetCoinKey(id),
                GetCriteriaKey(id), mystery, id, GetCustomRewardKey(id)));
        }

        private static AchievementCriteria GetCriteriaKey(int id)
        {
            if (criteriaDict.ContainsKey(id))
            {
                return criteriaDict[id];
            }
            return null;
        }

        private static string GetImgKey(int id)
        {
            if (imgDict.ContainsKey(id))
            {
                return imgDict[id];
            }
            return null;
        }

        private static AchievementMethod GetCustomRewardKey(int id)
        {
            if (rewardDict.ContainsKey(id))
            {
                return rewardDict[id];
            }
            return null;
        }

        private static int GetCoinKey(int id)
        {
            if (coinDict.ContainsKey(id))
            {
                return coinDict[id];
            }
            return 0;
        }

        #endregion
    }
}
