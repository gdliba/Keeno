using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Keeno
{
    static class Globals
    {
        public static float DeltaTime { get; private set; }

        public static readonly Random RNG = new Random();

        #region Input Properties
        public static MouseState MsCurr { get; private set; }
        public static MouseState MsOld { get; private set; }

        public static KeyboardState KbCurr { get; private set; }
        public static KeyboardState KbOld { get; private set; }

        public static bool LeftClick { get; private set; }
        public static bool RightClick { get; private set; }
        public static bool MiddleClick { get; private set; }

        public static bool W_KeyDown => KeyDown(Keys.W);
        public static bool A_KeyDown => KeyDown(Keys.A);
        public static bool S_KeyDown => KeyDown(Keys.S);
        public static bool D_KeyDown => KeyDown(Keys.D);
        public static bool Q_KeyDown => KeyDown(Keys.Q);
        public static bool E_KeyDown => KeyDown(Keys.E);

        public static bool Q_KeyPress => KeyPress(Keys.Q);
        public static bool E_KeyPress => KeyPress(Keys.E);

        /// <summary>
        /// Key is newly pressed this frame.
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>true in the frame it was pressed</returns>
        public static bool KeyPress(Keys key)
        {
            return KbCurr.IsKeyDown(key) && KbOld.IsKeyUp(key);
        }

        /// <summary>
        /// Key is currently pressed (no change information)
        /// </summary>
        /// <param name="key">The key that is pressed</param>
        /// <returns>true if the key is held down this frame</returns>
        public static bool KeyDown(Keys key)
        {
            return KbCurr.IsKeyDown(key);
        }
        #endregion

        #region Tile Properties and Indexes
        // Tile Properties
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
        #endregion

        //public static Rectangle TileSelectSrcRect { get { return new Rectangle(1, 1, 1, 1); } }

        // Player
        public static int PlayerMovementSpeed = 50;

        // Keeno
        public static float KeenoMovementSpeed = 20;

        // Tree
        public static int TreeHealth = 3;

        public static void Update(GameTime gt)
        {
            DeltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            MsOld = MsCurr;
            MsCurr = Mouse.GetState();

            KbOld = KbCurr;
            KbCurr = Keyboard.GetState();

            LeftClick = MsCurr.LeftButton == ButtonState.Pressed && MsOld.LeftButton == ButtonState.Released;
            RightClick = MsCurr.RightButton == ButtonState.Pressed && MsOld.RightButton == ButtonState.Released;
            MiddleClick = MsCurr.MiddleButton == ButtonState.Pressed && MsOld.MiddleButton == ButtonState.Released;
        }
    }
}
