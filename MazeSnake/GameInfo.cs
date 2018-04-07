using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MazeSnake
{
    public static class GameInfo
    {
        #region Fields

        static Random rand = new Random();

        public static Version Version = new Version(1, 1, 0);
        public static Language Language = (Language)(new Options().Language);

        public static List<MenuButton> Buttons = new List<MenuButton>();

        public static List<string> Credits = new List<string>()
        {
            #region Credits
            "Based on the idea by LIAM NELSON",
            "PROGRAMMING",
            "Head Programmer - DANIEL GAULL",
            "Assistant Programmer - JIM GAULL",
            "Thanks to all the kind people at Stack Overflow",
            "Who brought MazeSnake to life",
            "Additional thanks to to Jamis Buck, owner of Buckblog (weblog.jamisbuck.org).",
            "Whose website helped create the mazes",
            "\nSOUND EFFECTS",
            "From freesound.org:",
            "Achievement - Kastenfrosch",
            "Click - Swordmaster767",
            "Coin Collect - bradwesson",
            "Electric Buzz - keweldog",
            "Electric Powerup - RandomationPictures",
            "Error - Splashdust",
            "Explosion - Nbs Dark",
            "Forcefield Powerup - Randomation Pictures",
            "Freeze Powerup (original sound edited) - Mystikuum",
            "Lose Sound - RandomationPictures",
            "Main Menu Music - levelclearer",
            "Nature - J.Zazvurek",
            "Speed Powerup - RandomationPictures",
            "Star Collect - ertfelda",
            "Teleport Powerup - fins",
            "Thank you to everyone who helped make these sounds;",
            "without you, this would not have been possible.",
            "\nAll art assets created in paint.net",
            "\n\nSpecial thanks to my mother, who listened to the endless talking of MazeSnake's bugs.",
            "And thank YOU, for playing this game.",
            "\nProgrammed in Monogame 3.5",
            "\n\nIt's not over.",
            "It's only just begun.",
            "\n\n\n\nDuoplus Software Presents...\n\n\n\n",
            #endregion
        };

        public const int STARTING_MAZE_TIMER_VAL = 60;

        #endregion

        #region Public Methods

        public static int RewardForStreak(int streak)
        {
            return (int)Math.Floor((Math.Pow(streak, 2)) / 6 + 1);
        }

        #endregion

        #region Extension Methods

        public static SerializableColor Serialize(this Color color)
        {
            return new SerializableColor(color.R, color.G, color.B, color.A);
        }

        public static List<Cell> Clone(this List<Cell> list)
        {
            return list.Select(x => x.Clone()).ToList();
        }

        public static List<User> ConvertToNewUser(this List<OldUser> list)
        {
            List<User> users = new List<User>();

            for (int i = 0; i < list.Count; i++)
            {
                User n = new User(list[i].Username, list[i].Coins, "");
                n.Skins.Clear();
                for (int j = 0; i < list[i].Skins.Count; j++)
                {
                    n.AddSkin(list[i].Skins[j].Convert());
                }
                n.CurrentSkin = list[i].CurrentSkin.Convert();
                users.Add(n);
            }

            return users;
        }

        public static T Random<T>(this List<T> list)
        {
            return list[rand.Next(0, list.Count)];
        }

        public static bool IsOpposite(this Direction dir, Direction direction)
        {
            if (dir == Direction.Up && direction == Direction.Down)
            {
                return true;
            }
            else if (dir == Direction.Down && direction == Direction.Up)
            {
                return true;
            }
            else if (dir == Direction.Left && direction == Direction.Right)
            {
                return true;
            }
            else if (dir == Direction.Right && direction == Direction.Left)
            {
                return true;
            }
            return false;
        }
        public static Direction OppositeOf(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
            }
            return Direction.Up;
        }

        #endregion
    }
}
