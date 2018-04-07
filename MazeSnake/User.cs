using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace MazeSnake
{
    public delegate void OnUserCreation(User user);
    [Serializable]
    public class User
    {
        #region Fields & Properties

        public string Username = "";
        // Id is currently used for local identification
        public int Id;
        public Version LastPlayed = new Version();

        public Settings Settings = new Settings(100, true);

        const int NEW_USER_GIFT = 1000;

        public int Coins = 0;
        public List<Achievement> AchievementsCompleted = new List<Achievement>();
        public List<InventoryItem> Inventory = new List<InventoryItem>();
        public List<Skin> Skins = new List<Skin>();
        public Skin CurrentSkin;

        public DateTime LastReceivedGift = new DateTime();

        // Statistics
        public int MazesCompleted = 0;
        public int PowerupsCollected = 0;
        public int TimesTeleported = 0;
        public int SpeedSeconds = 0;
        public int CoinsCollected = 0;
        public int WallsBroken = 0;
        public int TimeFrozen = 0;
        public int EnemiesAvoided = 0;

        const int SPACING = 10;

        // No longer used but required for deserialization
        public int Level = 1;
        public int Xp = 0;
        public int XpPerLevel = 0;

        public static string usersPath = "";
        public static string MAIN_PATH = "";
        public static string BACKUP_PATH = "";

        #endregion

        #region Constructors

        public User(string username, int coins, string snakeAsset)
        {
            Username = username;
            CurrentSkin = new Skin(snakeAsset, Color.White, false, "", SnakeSkinType.MazeSnake);
            // Makes sure new users can receive a gift right away
            LastReceivedGift = DateTime.Now.AddHours(GiftGiver.HOURS_UNTIL_NEXT_GIFT * -1);
            Coins = coins;

            Options options = new Options();
            if (!options.RecThousandGift)
            {
                Coins += NEW_USER_GIFT;
                options.RecThousandGift = true;
                options.Save();
            }
        }

        #endregion

        #region Public Methods

        public static void Init()
        {
            FileStream fileStream = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                fileStream = File.Create(MAIN_PATH);
                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, new List<User>());
                fileStream = File.Create(BACKUP_PATH);
                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, new List<User>());
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public static void Initialize()
        {
            usersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Duoplus Software\MazeSnake");
            MAIN_PATH = Path.Combine(usersPath, "MazeSnakeUsers.bin");
            BACKUP_PATH = Path.Combine(usersPath, "MazeSnakeUsersBackup.bin");

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = null;
            List<User> users = new List<User>();
            try
            {
                if (!Directory.Exists(usersPath))
                {
                    Directory.CreateDirectory(usersPath);
                }
                if (!File.Exists(MAIN_PATH))
                {
                    fileStream = File.Create(MAIN_PATH);
                    bf.Serialize(fileStream, users);
                }
                if (!File.Exists(BACKUP_PATH))
                {
                    fileStream = File.Create(BACKUP_PATH);
                    bf.Serialize(fileStream, users);
                }
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public static List<User> LoadUsers()
        {
            FileStream fileStream = null;
            List<User> returnList = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(MAIN_PATH))
                {
                    fileStream = new FileStream(MAIN_PATH, FileMode.Open, FileAccess.Read);
                    returnList = (List<User>)binaryFormatter.Deserialize(fileStream);
                }
            }
            catch (SerializationException)
            {
                // If there is something wrong with the original file, we'll just copy over the backup file
                fileStream.Close();
                File.Delete(MAIN_PATH);
                File.Copy(BACKUP_PATH, MAIN_PATH);
                fileStream = new FileStream(MAIN_PATH, FileMode.Open, FileAccess.Read);
                returnList = (List<User>)binaryFormatter.Deserialize(fileStream);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return returnList;
        }

        public void SerializeUser()
        {
            FileStream fileStream = null;
            List<User> users = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(MAIN_PATH))
                {
                    fileStream = File.Open(MAIN_PATH, FileMode.Open);
                    users = (List<User>)binaryFormatter.Deserialize(fileStream);
                }

                LastPlayed = GameInfo.Version;

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Id == Id)
                    {
                        users.RemoveAt(i);
                    }
                }

                foreach (Achievement a in AchievementsCompleted)
                {
                    if (a.CompletionCriteria != null)
                    {
                        a.CompletionCriteria = null;
                    }
                    if (a.OnCompletion != null)
                    {
                        a.OnCompletion = null;
                    }
                }

                //if (Username == "test" && Coins < 10000)
                //{
                //    Coins = 10000;
                //}
                //if (Username == "Purasu" && Level < 17)
                //{
                //    Level = 17;
                //}

                // This puts the last played-as user at the top of the list, so it's easier to access them next time
                users.Insert(0, this);
                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, users);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        public static void DeleteUser(User user)
        {
            FileStream fileStream = null;
            List<User> users = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(MAIN_PATH))
                {
                    fileStream = File.Open(MAIN_PATH, FileMode.Open);
                    users = (List<User>)binaryFormatter.Deserialize(fileStream);
                }

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Id == user.Id)
                    {
                        users.RemoveAt(i);
                        break;
                    }
                }

                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, users);
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("There's been an error. This is likely due to some issues with the User files. If you have made any changes to " +
            //        FILE_PATH + ", your user information may have become corrupted. This error was caused by the DELETION of a user, meaning that " +
            //        "your info may still be okay. \nWe are sorry for the inconvenience. If the error message doesn't say anything about \"" + FILE_PATH +
            //        "\", then this may be a different error. Contact us if this is not a file error. Go to www.mazesnake.com to report this error." +
            //        "\nError Message: " + e.Message + "; Error thrown in non-static method: User.DeleteUser(User user)");
            //}
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public static void BackupUsers()
        {
            // Simply copies the content of the original file into the backup file
            FileStream fileStream = null;
            List<User> users = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(BACKUP_PATH))
                {
                    fileStream = File.Open(BACKUP_PATH, FileMode.Open);
                    users = User.LoadUsers();
                }

                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, users);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        public static void OverwriteOrigFile()
        {
            // This will overwrite FILE_PATH with the contents of BACKUP_PATH
            File.Copy(BACKUP_PATH, MAIN_PATH, true);
        }

        public static int GetNextId()
        {
            FileStream fileStream = null;
            List<User> users = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            int maxId = -1;
            try
            {
                if (File.Exists(MAIN_PATH))
                {
                    fileStream = File.Open(MAIN_PATH, FileMode.Open);
                    users = (List<User>)binaryFormatter.Deserialize(fileStream);
                }

                foreach (User u in users)
                {
                    if (u.Id > maxId)
                    {
                        maxId = u.Id;
                    }
                }
                // No matter what, maxId will be equal to the greatest ID value in users. We must increment it 
                // so that it will be the NEXT id, not the current biggest.
                maxId++;

                fileStream.Position = 0;
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("There's been an error. This is likely due to some issues with the User files. If you have made any changes to " +
            //        FILE_PATH + ", your user information may have become corrupted. This error was caused by the DELETION of a user, meaning that " +
            //        "your info may still be okay. \nWe are sorry for the inconvenience. If the error message doesn't say anything about \"" + FILE_PATH +
            //        "\", then this may be a different error. Contact us if this is not a file error. Go to www.mazesnake.com to report this error." +
            //        "\nError Message: " + e.Message + "; Error thrown in non-static method: User.DeleteUser(User user)");
            //}
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return maxId;
        }

        public void AddItem(InventoryItem item)
        {
            Inventory.Add(item);
        }
        public void AddSkin(Skin skin)
        {
            Skins.Add(skin);
        }
        public void AddCoins(int value)
        {
            Coins += value;
        }
        public void SetCoins(int value)
        {
            Coins = value;
        }
        public void AddToStat(Stat stat, int value)
        {
            switch (stat)
            {
                case Stat.CoinsCollected:
                    CoinsCollected += value;
                    break;
                case Stat.MazesCompleted:
                    MazesCompleted += value;
                    break;
                case Stat.PowerupsCollected:
                    PowerupsCollected += value;
                    break;
                case Stat.SpeedSeconds:
                    SpeedSeconds += value;
                    break;
                case Stat.TimesTeleported:
                    TimesTeleported += value;
                    break;
                case Stat.WallsBroken:
                    WallsBroken += value;
                    break;
                case Stat.TimeFrozen:
                    TimeFrozen += value;
                    break;
                case Stat.EnemiesAvoided:
                    EnemiesAvoided += value;
                    break;
            }
        }
        public void SetStat(Stat stat, int value)
        {
            switch (stat)
            {
                case Stat.CoinsCollected:
                    this.CoinsCollected = value;
                    break;
                case Stat.MazesCompleted:
                    this.MazesCompleted = value;
                    break;
                case Stat.PowerupsCollected:
                    this.PowerupsCollected = value;
                    break;
                case Stat.SpeedSeconds:
                    this.SpeedSeconds = value;
                    break;
                case Stat.TimesTeleported:
                    this.SpeedSeconds = value;
                    break;
                case Stat.WallsBroken:
                    this.WallsBroken = value;
                    break;
            }
        }

        public void ChangeLastReceivedGift(DateTime newTime)
        {
            this.LastReceivedGift = newTime;
        }

        //public void IncreaseXp(int amount)
        //{
        //    Xp += amount;
        //}
        //public void SetXpPerLevel(int amount)
        //{
        //    XpPerLevel = amount;
        //}

        public void ChangeUsername(string newName)
        {
            this.Username = newName;
        }
        public void ChangeVolume(int value)
        {
            this.Settings.Volume = value;
        }
        public void ChangeMouseWhilePlaying(bool value)
        {
            this.Settings.ShowMouseWhilePlaying = value;
        }

        //public static bool IsFileLocked(string filePath)
        //{
        //    FileStream stream = null;

        //    try
        //    {
        //        stream = File.Open(filePath, FileMode.OpenOrCreate);
        //    }
        //    catch (IOException)
        //    {
        //        // The file is currently being used by another process
        //        return true;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //        {
        //            stream.Close();
        //        }
        //    }

        //    // At this point, the file is not locked
        //    return false;
        //}

        #endregion
    }

    [Serializable]
    public class OldUser
    {
        public string Username = "";
        public int Id;
        public Settings Settings = new Settings(100, true);
        public int Coins = 0;
        public List<Achievement> AchievementsCompleted = new List<Achievement>();
        public List<InventoryItem> Inventory = new List<InventoryItem>();
        public List<OldSkin> Skins = new List<OldSkin>();
        public OldSkin CurrentSkin;
        public DateTime LastReceivedGift = new DateTime();
        public int MazesCompleted = 0;
        public int PowerupsCollected = 0;
        public int TimesTeleported = 0;
        public int SpeedSeconds = 0;
        public int CoinsCollected = 0;
        public int WallsBroken = 0;
        public int Level = 1;
        public int Xp = 0;
        public int XpPerLevel = 0;
    }

    #region Enumerations

    public enum Stat
    {
        MazesCompleted,
        PowerupsCollected,
        SpeedSeconds,
        TimesTeleported,
        CoinsCollected,
        WallsBroken,
        TimeFrozen,
        EnemiesAvoided,
    }

    #endregion
}