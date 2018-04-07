using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeSnake
{
    public static class Costs
    {
        public static Dictionary<SnakeSkinType, int> SkinCosts = new Dictionary<SnakeSkinType, int>();
        public static Dictionary<ItemType, int> PowerupCosts = new Dictionary<ItemType, int>();

        public static void Initialize(Dictionary<SnakeSkinType, int> skinCosts, Dictionary<ItemType, int> powerupCosts)
        {
            SkinCosts = skinCosts;
            PowerupCosts = powerupCosts;
        }
    }

    public enum SnakeSkinType
    {
        RedSnake = 0,
        OrangeSnake = 1,
        YellowSnake = 2, 
        MazeSnake = 3,
        BlueSnake = 4, 
        PurpleSnake = 5,
        PinkSnake = 6,
        AlbinoSnake = 7,
        MazeWorm = 8,
        ElectricEel_Removed = 9,
        SeaSnake = 10,
        CamoSnake = 11,
        RainbowSnake = 12,
        ZebraSnake = 13,
        PoshSnake = 14,
        SecretSnake_Removed = 15,
        WizardSnake = 16,
        BumbleSnake = 17,
        SantaSnake = 18,
        GhostSnake = 19,
        EasterSnake_Removed = 20,
        AmericaSnake = 21,
        SnowSnake = 22,
        CornucopiaSnake = 23,
        RoboSnake = 24,
        DullSnake = 25,
        PolkaDotSnake = 26,
        OldSnake = 27,
        YoungSnake = 28,
        HomecomingSnake = 29,
        BuilderSnake = 30,
        PixelSnake = 31,
        CosmicSnake = 32,
    }
}
