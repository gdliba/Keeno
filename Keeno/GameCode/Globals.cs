using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace Keeno
{
    static class Globals
    {
        static KeyboardState _keyboard;
        public static void UpdateInput() => _keyboard = Keyboard.GetState();

        public static bool MoveUP => _keyboard.IsKeyDown(Keys.W);
        public static bool MoveLEFT => _keyboard.IsKeyDown(Keys.A);
        public static bool MoveDOWN => _keyboard.IsKeyDown(Keys.S);
        public static bool MoveRIGHT => _keyboard.IsKeyDown(Keys.D);
        public static bool PickUpKeeno => _keyboard.IsKeyDown(Keys.Q);
        public static bool Interact => _keyboard.IsKeyDown(Keys.E);


        //public static bool NoMovementKeysPressed { get { return !MoveUP && !MoveLEFT && !MoveDOWN && !MoveRIGHT; } }

        public const int TilemapColumns = 49;
        public const int TileWidth_andHeight = 16;

        public const int TreeTileIndex = 51;
        public const int ChoppedTreeTileIndex = 313;
        public const int OccupiedTileIndex = 0;
        public const int TownCentreTileIndex = 983;
        public const int TileSelectedIndex = 624;

        public static Rectangle TileSelectSrcRect { get { return new Rectangle(1, 1, 1, 1); } }


        public static int PlayerMovementSpeed = 50;
        public static int KeenoMovementSpeed = 20;




    }
}
