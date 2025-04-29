using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

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
        public static bool X_KeyDown => KeyDown(Keys.X);


        public static bool Q_KeyPress => KeyPress(Keys.Q);
        public static bool E_KeyPress => KeyPress(Keys.E);
        public static bool X_KeyPress => KeyPress(Keys.X);


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
        public const int InputsTilesetIndex_Q = 85;
        public const int InputsTilesetIndex_X = 156;

        #endregion

        //public static Rectangle TileSelectSrcRect { get { return new Rectangle(1, 1, 1, 1); } }

        // Player
        public static int PlayerMovementSpeed = 50;
        public static float DropOffKeenoSpeed = .03f;

        // Keeno
        //public static float KeenoMovementSpeed = 20;

        // Tree
        public static int TreeHealth = 10;

        // Layer Depths
        public static float ButtonPromptLD = .09f;
        public static float HourGlassLD = .085f;
        public static float KeenoLD = .081f;
        public static float SelectedTxrLD = .08f;
        public static float WolrdObjectLD = .01f;
        public static float MapLD = 0f;





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
    static class Assets
    {
        public static Texture2D KeenoTxr {  get; private set; }
        public static Texture2D DebugPixelTxr { get; private set; }
        public static Texture2D TilesetTxr { get; private set; }
        public static Texture2D MonochromaticTilesetTxr { get; private set; }
        public static Texture2D InputsTilesetTxr { get; private set; }
        public static Texture2D ChoppedTreeTxr {  get; private set; }


        public static void Load(ContentManager content)
        {
            KeenoTxr = content.Load<Texture2D>("Characters\\Keeno");

            DebugPixelTxr = content.Load<Texture2D>("Pixel");

            // TileSets
            TilesetTxr = content.Load<Texture2D>("SpriteSheets\\color_t");
            MonochromaticTilesetTxr = content.Load<Texture2D>("SpriteSheets\\mono_t");
            InputsTilesetTxr = content.Load<Texture2D>("SpriteSheets\\inputs_t");

            ChoppedTreeTxr = content.Load<Texture2D>("WorldObjects\\Flora\\choppedTree2");
        }
    }
    public enum ResourceType
    {
        None,
        Gold,
        Wood,
        Food
    }

    static class ResourceTracker
    {
        // Store resource Type and Amount
        private static readonly Dictionary<ResourceType, int> _amounts;

        // fired whenever any resource changes
        public static event Action<ResourceType, int> ResourceChanged;


        static ResourceTracker()
        {
            _amounts = Enum
                .GetValues<ResourceType>()
                .ToDictionary(rt => rt, rt => 0);
        }

        /// <summary>
        /// Returns the current amount of the given resource.
        /// </summary>
        public static int GetAmount(ResourceType type)
            => _amounts[type];

        /// <summary>
        /// Increases the given resource by a positive amount.
        /// </summary>
        public static void Add(ResourceType type, int amount)
        {
            // debugging
            //if (amount <= 0)
            //    throw new ArgumentException(
            //        "Must add a positive amount", nameof(amount));

            _amounts[type] += amount;
            ResourceChanged?.Invoke(type, _amounts[type]);
        }

        /// <summary>
        /// Tries to spend (subtract) the given cost from the specified resource.
        /// Returns true if successful, false if insufficient funds.
        /// </summary>
        public static bool TrySpend(ResourceType type, int cost)
        {
            // debugging
            //if (cost <= 0)
            //    throw new ArgumentException(
            //        "Cost must be positive", nameof(cost));

            if (_amounts[type] < cost)
                return false;

            _amounts[type] -= cost;
            ResourceChanged?.Invoke(type, _amounts[type]);
            return true;
        }

        /// <summary>
        /// Resets all resources back to zero.
        /// </summary>
        public static void Reset()
        {
            foreach (var key in _amounts.Keys.ToList())
            {
                _amounts[key] = 0;
                ResourceChanged?.Invoke(key, 0);
            }
        }
    }
}
