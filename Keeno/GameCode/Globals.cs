using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace Keeno
{
    static class Globals
    {
        static KeyboardState _keyboard;
        public static void UpdateInput() => _keyboard = Keyboard.GetState();

        public static bool Input_W => _keyboard.IsKeyDown(Keys.W);
        public static bool Input_A => _keyboard.IsKeyDown(Keys.A);
        public static bool Input_S => _keyboard.IsKeyDown(Keys.S);
        public static bool Input_D => _keyboard.IsKeyDown(Keys.D);
        public static bool Input_Q => _keyboard.IsKeyDown(Keys.Q);
        public static bool Input_E => _keyboard.IsKeyDown(Keys.E);


        //public static bool NoMovementKeysPressed { get { return !MoveUP && !MoveLEFT && !MoveDOWN && !MoveRIGHT; } }

        public const int TilemapColumns = 49;
        public const int Tile_Width_Height = 16;

        public const int TreeTileIndex = 51;
        public const int ChoppedTreeTileIndex = 313;
        public const int OccupiedTileIndex = 0;
        public const int TownCentreTileIndex = 983;
        public const int TileSelectedIndex = 624;

        // HourGlass
        public const int EmptyHourGlassIndex = 628;
        public const int FullHourGlassIndex = 630;

        //Inputs Tileset
        public const int InputsTilesetColumns = 34;
        public const int InputsTileset_Width_Height = 16;

        public const int InputsTilesetIndex_E = 87;
        public const int InputsTilesetIndex_Q = 630;

        public static Rectangle TileSelectSrcRect { get { return new Rectangle(1, 1, 1, 1); } }

        // Player
        public static int PlayerMovementSpeed = 50;

        // Keeno
        public static int KeenoMovementSpeed = 20;


        // Tree
        public static int TreeHealth = 3;



    }
}
