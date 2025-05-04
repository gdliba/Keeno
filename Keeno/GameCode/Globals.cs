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
        public static bool UpArrow_KeyPress=> KeyPress(Keys.Up);
        public static bool DownArrow_KeyPress => KeyPress(Keys.Down);
        public static bool Tab_KeyPress => KeyPress(Keys.Tab);

        public static bool E_FakeKeyPress => FakeKeyPress(Keys.E);


        /// <summary>
        /// Key is newly pressed this frame.
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>true in the frame it was pressed</returns>
        public static bool KeyPress(Keys key)
        {
            return KbCurr.IsKeyDown(key) && KbOld.IsKeyUp(key);
        }
        public static bool FakeKeyPress(Keys key)
        {
            return KbOld.IsKeyDown(key) && KbCurr.IsKeyUp(key);
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

        public const int OccupiedTileIndex = 0;
        public const int EmptyTileIndex = -1;
        public const int TileSelectedIndex = 624;
        public const int ItemSelectedIndex = 625;
        public const int BlueprintIndex = 767;

        public const int TreeTileIndex = 51;

        public const int FarmTileIndex1 = 309;
        public const int FarmTileIndex2 = 310;
        public const int HarvestedFarmTileIndex = 307;
        public const int FarmLandTileIndex = 306;

        public const int TownCentreTileIndex = 983;
        public const int WorkShopTileIndex = 984;

        public const int RockTileIndex = 103;
        public const int HarvestedRockTileIndex = 68;

        public const int GoldTileIndex = 522;
        public const int HarvestedGoldTileIndex = 218;






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
        public static float NeutralInteractSpeed = .02f;
        public static float DestroyInteractSpeed = .005f;


        // Keeno
        //public static float KeenoMovementSpeed = 20;

        // Tree
        public static int TreeHealth = 10;
        public static int TreeWoodAmount = 1;

        // Farm
        public static int FarmHealth = 5;
        public static int FarmFoodAmount = 1;

        // Stone
        public static int RockHealth = 1;
        public static int RockStoneAmount = 1;

        // Gold
        public static int GoldHealth = 1;
        public static int GoldGoldAmount = 1;

        // Layer Depths
        public static float ItemSelectedTxrLD = .098f;
        public static float ItemTxrLD = .097f;
        public static float BlueprintTxrLD = .096f;
        public static float PlayerLD = .095f;
        public static float ButtonPromptLD = .094f;
        public static float HourGlassLD = .093f;
        public static float KeenoLD = .092f;
        public static float SelectedTxrLD = .091f;
        public static float WolrdObjectLD = .090f;
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
        public static Texture2D TentsTxr { get; private set; }
        public static Texture2D BlueprintTxr { get; private set; }




        public static void Load(ContentManager content)
        {
            KeenoTxr = content.Load<Texture2D>("Characters\\Keeno");

            DebugPixelTxr = content.Load<Texture2D>("Pixel");

            // TileSets
            TilesetTxr = content.Load<Texture2D>("SpriteSheets\\color_t");
            MonochromaticTilesetTxr = content.Load<Texture2D>("SpriteSheets\\mono_t");
            InputsTilesetTxr = content.Load<Texture2D>("SpriteSheets\\inputs_t");
            ChoppedTreeTxr = content.Load<Texture2D>("WorldObjects\\Flora\\choppedTree2");
            TentsTxr = content.Load<Texture2D>("WorldObjects\\Buildings\\Houses\\tents_w");
            BlueprintTxr = content.Load<Texture2D>("WorldObjects\\Items\\scroll");
        }
    }
    public enum ResourceType
    {
        None,
        Food,
        Wood,
        Stone,
        Gold
    }

    static class ResourceTracker
    {
        public const int KeenoCost = 5;

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
        public static bool CanSpend(ResourceType type, int cost)
        {
            if (_amounts[type] < cost)
                return false;

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
