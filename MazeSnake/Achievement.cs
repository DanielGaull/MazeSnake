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
    public delegate void AchievementMethod(Achievement achiement);
    public delegate bool AchievementCriteria();
    [Serializable]
    public class Achievement
    {
        #region Fields and Properties

        public int Id = 0;
        public string Name = "";
        public string Description = "";

        public string Image;
        public SerializableColor ImageColor = Color.White.Serialize();

        public AchievementDifficulty Difficulty;
        public int CoinsOnCompletion = 0;
        // No longer used but required for serialization
        public int XpOnCompletion = 0;

        public AchievementMethod OnCompletion;
        public AchievementCriteria CompletionCriteria;

        public bool MysteryAchievement = false;

        public RewardType AchRewardType;

        public bool Completed = false;

        #endregion

        #region Constructors

        public Achievement()
        {
        }

        public Achievement(ContentManager content, string imgSpriteName, string name, string desc, AchievementDifficulty difficulty, 
            int coinsOnCompletion, AchievementCriteria criteria, bool isMystery, int id)
        {
            Image = imgSpriteName;
            Name = name;
            Description = desc;
            Difficulty = difficulty;
            CoinsOnCompletion = coinsOnCompletion;
            CompletionCriteria = criteria;
            MysteryAchievement = isMystery;
            Id = id;
            if (CoinsOnCompletion > 0)
            {
                AchRewardType = RewardType.Coins;
            }
            else
            {
                AchRewardType = RewardType.Skin;
            }
        }

        public Achievement(ContentManager content, string imgSpriteName, string name, string desc, AchievementDifficulty difficulty, 
            int coinsOnCompletion, AchievementCriteria criteria, bool isMystery, int ID, AchievementMethod onCompletion)
            : this(content, imgSpriteName, name, desc, difficulty, coinsOnCompletion, criteria, isMystery, ID)
        {
            OnCompletion = onCompletion;
        }
        
        #endregion

        #region Public Methods

        public User CheckAndRewardCompletion(User user)
        {
            if (CompletionCriteria != null)
            {
                if (CompletionCriteria())
                {
                    user.AddCoins(CoinsOnCompletion);
                    user.AchievementsCompleted.Add(Clone(this));
                    OnCompletion?.Invoke(this);
                    Completed = true;
                    return user;
                }
                else
                {
                    return user; 
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a copy of the achievement object
        /// </summary>
        /// <param name="ach">The achievement to copy</param>
        /// <returns>The copy of "ach"</returns>
        public static Achievement Clone(Achievement ach)
        {
            Achievement clone = new Achievement();

            clone.Name = ach.Name;
            clone.OnCompletion = ach.OnCompletion;
            clone.MysteryAchievement = ach.MysteryAchievement;
            clone.Image = ach.Image;
            clone.Id = ach.Id;
            clone.Difficulty = ach.Difficulty;
            clone.Description = ach.Description;
            clone.CompletionCriteria = ach.CompletionCriteria;
            clone.Completed = ach.Completed;
            clone.CoinsOnCompletion = ach.CoinsOnCompletion;
            clone.AchRewardType = ach.AchRewardType;

            return clone;
        }

        #endregion
    }

    #region Enums

    public enum RewardType
    {
        Coins,
        Skin
    }

    public enum AchievementDifficulty
    {
        Green,
        Yellow,
        Orange,
        Red
    }

    #endregion
    
}
