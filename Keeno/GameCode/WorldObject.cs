using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Keeno
{
    enum ObjectState
    {
        AwaitingResourceDelivery,
        UnderConstruction,
        Neutral,
        Harvestable,
        Broken,
        Dead
    }
    enum BuildingType
    {
        Tent,
        House,
        ResourceStorage,
        FarmLand,
        Bridge
    }
    enum BuildingLevel { One, Two, Three,}
    /// <summary>
    /// All GameObjects inherit from this class.
    /// This class is designed to work for a very broad range of applications, 
    /// thus, some methods aren't used by some children.
    /// It could be argued that this class could have been simpler and it could
    /// have had another couple, more specialised children inherit from it.
    /// </summary>
    class WorldObject
    {
        #region Variables
        public ObjectState State { get { return _state; } protected set { _state = value; } }
        protected ObjectState _state;
        protected ResourceType _resourceType;

        protected string _name;
        protected SpriteFont _descriptionFont;

        protected ButtonPrompt _buttonPrompt_E;
        protected ButtonPrompt _buttonPrompt_Q;
        protected ButtonPrompt _buttonPrompt_X;

        protected HourGlass _HGInteract;
        protected HourGlass _HGDropOff;
        protected HourGlass _HGDestroy;
        protected HourGlass _HGWorkProgress;
        protected HourGlass _HGCantInteract;

        protected Texture2D _txr;
        protected Texture2D _selectedTileTileset;
        protected Texture2D _testPixel;

        public Rectangle Bounds { get{ return _rect; } protected set { _rect = value; } }
        public Rectangle CoreRect { get { return _coreRect; } protected set { _coreRect = value; } }
        protected Rectangle _rect;
        protected Rectangle? _srcRect;
        protected Rectangle? _selectedTileSrcRect;
        protected Rectangle _coreRect;

        public Vector2 Position { get { return new Vector2(_rect.X + _tileWidth / 2, _rect.Y + _tileHeight / 2); } }
        public Point TilePosition { get { return _tilePosition; } protected set { _tilePosition = value; } }
        protected Point _tilePosition;


        protected float _txrRotationDegrees;
        protected float _txrRotationRadians;

        public int Health { get { return _health; } protected set { _health = value; } }
        protected int _health;
        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;


        public bool Impassable { get { return _impassable;} protected set { _impassable = value; } }
        protected bool _flipped;
        protected bool _isSelected;
        protected bool _canBeSelectedWhenBroken;
        protected bool _canDropOff;
        protected bool _canUse;
        protected bool _cannotUse;
        protected bool _destroyMe;
        protected bool _impassable;
        protected bool _isDropOffPointActive;
        protected bool _specialInteraction;

        protected Color _tint;
        protected Color _uiColour, _fontColour;
        #endregion

        /// <summary>
        /// Constructor. Takes in the tile position and the index of the tile.
        /// Uses the tile index and the spritesheet (that it recovers from the Assets class)
        /// to determine the location of the source rectangle.
        /// </summary>
        public WorldObject(Point tilePosition, int globalTileIndex)
        {
            _uiColour = Color.White;
            _fontColour = new Color(207,198,184);
            _name = "name";
            _descriptionFont = Assets.MonogramDescriptionFont;
            _state = ObjectState.Harvestable;
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;
            _cannotUse = false;
            _flipped = false;
            _canBeSelectedWhenBroken = true;
            _isDropOffPointActive = false;
            _specialInteraction = false;


            _testPixel = Assets.DebugPixelTxr; 
            _selectedTileTileset = Assets.MonochromaticTilesetTxr;
            _txr = Assets.TilesetTxr;

            _tilesetColumns = Globals.TilemapColumns;
            _tileWidth = _tileHeight = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * _tileWidth;
            _tilePosition.Y = tilePosition.Y * _tileHeight;


            _rect = new Rectangle(_tilePosition.X,
                              _tilePosition.Y,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height);
            _srcRect = new Rectangle(
                  (globalTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (globalTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
            _tint = Color.White;


            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);

            _health = 1;
            _txrRotationDegrees = 0f;

            CreateLoadingBarsAndPrompts();
        }

        /// <summary>
        /// Extracted the method only to keep the Constructor cleaner.
        /// </summary>
        protected virtual void CreateLoadingBarsAndPrompts()
        {
            #region ButtonPrompts and HG
            _buttonPrompt_E = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _buttonPrompt_Q = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X - _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_Q);

            _buttonPrompt_X = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight + 8,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_X);

            _HGInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Yellow);

            _HGDropOff = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X - _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.White);

            _HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth+1,
                _tileHeight),
                Color.Yellow);

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight + 8,
                _tileWidth,
                _tileHeight), Color.Red);

            _HGCantInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Red);
            #endregion
        }

        public float DistanceTo(Vector2 destination)
        {
            return (destination - Position).Length();
        }
        public bool GetDropOffPointState()
        {
            return _isDropOffPointActive;
        }

        /// <summary>
        /// Method that updates the HourGlasses,
        /// sets selected to false and converts the sprite rotation
        /// (rotation of when the object takes damage).
        /// </summary>
        /// <param name="gt"></param>
        public virtual void Update(GameTime gt)
        {
            // Change the HourGlass positions if propts are hidden
            if (Globals.HidePromtsAndNames)
            {
                _HGInteract.ChangePosition(_HGWorkProgress.Bounds);
                _HGDestroy.ChangePosition(_HGWorkProgress.Bounds);
                _HGDropOff.ChangePosition(_HGWorkProgress.Bounds);
            }
            else
            {
                _HGInteract.DefaultPosition();
                _HGDestroy.DefaultPosition();
                _HGDropOff.DefaultPosition();
            }
            if (!_isSelected)
            {
                _HGInteract.Reset();
                _HGDropOff.Reset();
                _HGDestroy.Reset();
                _HGCantInteract.Reset();
            }
            _isSelected = false;
            _txrRotationRadians = MathHelper.ToRadians(_txrRotationDegrees);
        }

        /// <summary>
        /// Called when the player “interacts” with this object.
        /// </summary>
        public virtual void OnInteract()
        {

        }

        /// <summary>
        /// Called when the object is to be destroyed.
        /// Extracted it to a method for modularity and so that
        /// more complex chlidren can override it.
        /// </summary>
        public virtual void DestroyMe()
        {
            
            _state = ObjectState.Dead;
        }

        /// <summary>
        /// Called by the Map class to reset the game.
        /// Destined to be overriden.
        /// </summary>
        public virtual void DestroyMeAndMyWorkers()
        {
            _state = ObjectState.Dead;
        }

        /// <summary>
        /// Displays the name of the building if it has one with a slight backdrop to help readability.
        /// </summary>
        /// <param name="sb"></param>
        protected virtual void TextDescription(SpriteBatch sb)
        {
            if (Globals.HidePromtsAndNames)
                return;
            Vector2 textSize = _descriptionFont.MeasureString(_name);
            sb.DrawString(_descriptionFont, _name, new Vector2(_rect.Center.X - textSize.X / 2, _rect.Bottom - textSize.Y / 4), _fontColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            sb.DrawString(_descriptionFont, _name, new Vector2(_rect.Center.X - textSize.X / 2, _rect.Bottom - textSize.Y / 4) + new Vector2(.5f), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None,  .098f);

        }

        /// <summary>
        /// Method that draws the Selected outline and 
        /// </summary>
        public virtual void SelectedDraw(SpriteBatch sb)
        {
            switch (_state)
            {
                case ObjectState.Harvestable:
                    if (_isSelected)
                        sb.Draw(_selectedTileTileset,_rect , _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    break;
                case ObjectState.Neutral:
                    if (_isSelected)
                    {
                        TextDescription(sb);
                        sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    }
                    break;
                case ObjectState.Broken:
                    if (_isSelected && _canBeSelectedWhenBroken)
                        sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
                    break;
            }
        }

        /// <summary>
        /// Main draw method for the class.
        /// </summary>
        public virtual void Draw(SpriteBatch sb)
        {
            // Needed to rotate the sprite slightly when it takes damage.
            Vector2 origin = new Vector2(_rect.Width / 2f, _rect.Height / 2f);

            // Determine when to flip the sprite (making it look to the RIGHT)
            var flip = _flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SelectedDraw(sb);

            if (_state != ObjectState.Dead)
            {
                sb.Draw(_txr, Position, _srcRect, _tint, _txrRotationRadians, origin, 1, flip, Globals.WolrdObjectLD);
                _HGWorkProgress.Draw(sb);
            }
        }
    }
    /// <summary>
    /// This class's only purpose is so that the player cannot simply select
    /// any WorldObject that doesn't serve a purpose.
    /// </summary>
    class SelectableWorldObject : WorldObject
    {
        public SelectableWorldObject(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {

        }
        public virtual void Selected()
        {
            _isSelected = true;
        }
    }

    /// <summary>
    /// Base class for other items.
    /// </summary>
    class Item : SelectableWorldObject
    {
        protected bool _isEquipped;
        public Item(Point position, Texture2D txr, int index)
            : base(position,index)
        {
            _isEquipped = false;
            _txr = txr;

            _impassable = false;
            _selectedTileSrcRect = _srcRect;
            CreateLoadingBarsAndPrompts();
        }

        /// <summary>
        /// A more secure "Selected" method compared to the parent class.
        /// </summary>
        public virtual void Selected(bool IsConditionMet)
        {
            if(IsConditionMet)
                _isSelected = true;
        }

        public virtual void OnInteract(Point itemCarryPoint)
        {
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;
        }

        /// <summary>
        /// Called when the player is holding the item.
        /// </summary>
        public void FollowPlayer(Point itemCarryPoint)
        {
            _isEquipped = true;
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;
        }

        /// <summary>
        /// Called when the player places the item.
        /// </summary>
        public virtual void Place(Rectangle onThisTile)
        {
            _rect = onThisTile;
        }

        /// <summary>
        /// Class draw method. Overrides the parent one, 
        /// as it has not need for the complexity of the parent method.
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_txr, _rect, null, Color.White, 0f,Vector2.Zero,SpriteEffects.None,.1f);
        }

    }
    /// <summary>
    /// Simple class for the coins scattered accross the map.
    /// </summary>
    class GoldCoin : Item
    {
        public GoldCoin(Point position)
            : base(position, null, Globals.ItemSelectedIndex)
        {
            _txr = Assets.TilesetTxr;
            _srcRect = new Rectangle(
                            (Globals.GoldCoinTileIndex % _tilesetColumns) * _tileWidth,
                            (Globals.GoldCoinTileIndex / _tilesetColumns) * _tileHeight,
                            _tileWidth,
                            _tileHeight);
            _coreRect = new Rectangle(_rect.X + _rect.Width / 4, _rect.Y + _rect.Height / 4, _rect.Width / 2, _rect.Height / 2);

        }

        /// <summary>
        /// Method called by the player when intersecting your "coreRect" (small rect in the centre of rect)
        /// Gives the player 1 gold and removes itself.
        /// </summary>
        public void GatherGoldCoin()
        {
            ResourceTracker.Add(ResourceType.Gold, 1);
            _state = ObjectState.Dead;
        }

        /// <summary>
        /// Overrides parent method as this one uses a source rect.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.Gold, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_txr, _rect, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.WolrdObjectLD);
        }
    }
    /// <summary>
    /// Base Blueprint class for BuildingBlueprint that will be used later on.
    /// Basically draws a blue scroll on the position of the item.
    /// </summary>
    class Blueprint : Item
    {
        protected Texture2D _blueprintTxr;
        public Blueprint(Point position, Texture2D txr)
            : base(position, txr, Globals.ItemSelectedIndex)
        {
            _blueprintTxr = Assets.BlueprintTxr;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Draw(_blueprintTxr, _rect, null, Color.CornflowerBlue, 0f, Vector2.Zero, SpriteEffects.None, .1f);
        }

    }

    /// <summary>
    /// Inherits from Blueprint so that it inherits the Item class methods,
    /// but also draws the Blueprint Draw.
    /// Expands on the base functionality though, as it spawns a building once placed.
    /// It would have been more modular and easy to edit if I had also kept this class
    /// as a base class of sorts and have the children inherit from it.
    /// I would do that given the time.
    /// </summary>
    class BuildingBlueprint : Blueprint
    {
        public event Action<Building> BuildingSpawned;

        protected List<Rectangle> _stageSrcRects;

        protected BuildingType _buildingType;

        protected Texture2D _buildingTxr;

        protected Building _building;

        public BuildingBlueprint(Point position, BuildingType type)
            : base(position, null)
        {
            _buildingType = type;

            switch (type)
            {
                case BuildingType.Tent:
                    _txr = Assets.TentsWhiteTxr;
                    break;
                case BuildingType.House:
                    _txr = Assets.HousesWhiteTxr;
                    break;
                case BuildingType.ResourceStorage:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                  (Globals.FarmTileIndex1 % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FarmTileIndex1/ Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;

            }

            // The BuildingSpritesheet (in the case of "Tent" and "House") has 3 stages
            // and is intended to be drawn as 3 sprites one on top of the other.
            // Depending of the building's level
            _stageSrcRects = new List<Rectangle>();
            _stageSrcRects.Add(new Rectangle(0, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height));
        }

        /// <summary>
        /// Overrides parent method.
        /// Once placed it fires the event and spawns a building of the 
        /// same type as this Blueprint.
        /// It then flags itself as dead so it's removed by the Map class.
        /// </summary>
        public override void Place(Rectangle onThisTile)
        {
            Assets.BuildingPlacedSFX.Play();

            switch (_buildingType)
            {
                case BuildingType.Tent:
                    _blueprintTxr = Assets.TentsTxr;
                    break;
                case BuildingType.House:
                    _blueprintTxr = Assets.HousesTxr;
                    break;
                default:
                    break;

            }

            base.Place(onThisTile);
            _building = new Building(new Point(onThisTile.X, onThisTile.Y), _blueprintTxr, _buildingType);
            BuildingSpawned?.Invoke(_building);
            // Remove Blueprint
            _state = ObjectState.Dead;
        }

        /// <summary>
        /// Class Draw method. Overrides parent method as it has more nuance.
        /// Houses and Tents are drawn in 3 stages on top of the blueprint.
        /// ResourceStorage buildings and FarmLands are drawn normally.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            if (_state == ObjectState.Dead)
                return;
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Color.DeepSkyBlue, 0, Vector2.Zero, SpriteEffects.None, Globals.ItemSelectedTxrLD);

            if(_buildingType == BuildingType.ResourceStorage || _buildingType == BuildingType.FarmLand)
            {
                var temp = new Rectangle(_rect.X + _rect.Width / 5, _rect.Y+ _rect.Height / 6, 2*_rect.Width / 3, 2*_rect.Height / 3);
                sb.Draw(_txr, temp, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
            }
            else
            {
                // Draw all 3 sprites
                for (int i = 0; i < 3; i++)
                {
                    sb.Draw(_txr, _rect, _stageSrcRects[i], Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
                }
            }
                sb.Draw(_blueprintTxr, _rect, null, Color.RoyalBlue, 0f, Vector2.Zero, SpriteEffects.None, Globals.BlueprintTxrLD);
        }
    }

    /// <summary>
    /// The ShopBuildingBlueprint is a BuildingBlueprint that changes its type
    /// to display to the player the buildings they can purchase and place.
    /// This is the big upside of NOT having the BuildingBLueprint be a base
    /// class and have the Types of buildings be children of it, this class
    /// is noticably more straight forward to create as it is simply switching type.
    /// </summary>
    class ShopBuildingBlueprint : BuildingBlueprint
    {
        private Texture2D _swapIconTxr;
        private float _flashingFontTimer, _flashingFontTimerReset;
        private int _price;
        public event Action<BuildingBlueprint> BuildingBlueprintPurchaced;

        /// <summary>
        /// Constructor assigns the price to the blueprint currently displayed
        /// and sets some base values to minor variables.
        /// </summary>
        public ShopBuildingBlueprint(Point position, BuildingType type)
            : base(position, type)
        {
            _buildingType = type;
            _swapIconTxr = Assets.UISwapIconTxr;
            _flashingFontTimer = 0;
            _flashingFontTimerReset = .3f;

            // Set the price
            switch (_buildingType)
            {
                case BuildingType.Tent:
                    _price = Globals.TentBLGoldPrice;
                    break;
                case BuildingType.House:
                    _price = Globals.HouseBLGoldPrice;
                    break;
                case BuildingType.ResourceStorage:
                    _price = Globals.ResourceStorageBLGoldPrice;
                    break;

            }
        }

        /// <summary>
        /// Simply counts down the flashing font timer that changes the colour of the
        /// font momentarily if the player attempts to buy a blueprint while
        /// not posessing sufficient funds.
        /// </summary>
        public override void Update(GameTime gt)
        {
            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _uiColour = Color.Red;
            }
            else
            {
                _uiColour = Color.White;
            }
            base.Update(gt);
        }

        /// <summary>
        /// Swap method for the class: switches the BuildingType and updates
        /// the relevant values appropriatelly.
        /// </summary>
        public virtual void OnQInteract()
        {
            // Play the appropriate sound
            var sound = Assets.BlueprintShuffleSFX;
            var soundInst = sound.CreateInstance();
            soundInst.Volume = .8f;
            soundInst.Play();

            // Switch type, wrap back around once you reach the one before bridge
            // (I don't want the player to be able to build bridges wherever they want).
            _buildingType = (BuildingType) ((int) (_buildingType + 1) % (int) BuildingType.Bridge);

            switch (_buildingType)
            {
                case BuildingType.Tent:
                    _txr = Assets.TentsWhiteTxr;
                    _price = Globals.TentBLGoldPrice;
                    break;
                case BuildingType.House:
                    _txr = Assets.HousesWhiteTxr; 
                    _price = Globals.HouseBLGoldPrice;
                    break;
                case BuildingType.ResourceStorage:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _price = Globals.ResourceStorageBLGoldPrice;
                    _srcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _txr = Assets.MonochromaticTilesetTxr;
                    _srcRect = new Rectangle(
                 (Globals.FarmTileIndex1 % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                 (Globals.FarmTileIndex1 / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                 Globals.Tile_Width_Height,
                 Globals.Tile_Width_Height);
                    _price = Globals.FarmLandBLGoldPrice;
                    break;
            }
            return;
        }

        /// <summary>
        /// Method called when the player attempts to "Buy" the blueprint.
        /// Returns null if the player cannot buy it, returns the
        /// corresponding blueprint if the player can purchase it.
        /// </summary>
        public Item OnInteract(Point itemCarryPoint)
        {
            if (ResourceTracker.CanSpend(ResourceType.Gold, _price))
            {
                var sound = Assets.BuySFX;
                var soundInst = sound.CreateInstance();
                soundInst.Volume = .8f;
                soundInst.Play();

                ResourceTracker.Spend(ResourceType.Gold, _price);
                var bp = new BuildingBlueprint(Point.Zero, _buildingType);
                BuildingBlueprintPurchaced?.Invoke(bp);
                bp.OnInteract(itemCarryPoint);
                return bp;
            }
            _flashingFontTimer = _flashingFontTimerReset;
            return null;
        }

        /// <summary>
        /// Method that Draws the Interaction UI that displays the price 
        /// of the currently selected blueprint.
        /// </summary>
        protected void PurchaceUI(SpriteBatch sb)
        {
            string priceText = _price.ToString();
            Vector2 priceTextSize = _descriptionFont.MeasureString(priceText);
            Vector2 priceTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

            
            sb.DrawString(_descriptionFont, priceText, priceTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 goldUIPos = new Vector2(priceTextPos.X - 8, priceTextPos.Y + 4);
            sb.Draw(Assets.UIGoldIconTxr, goldUIPos, null, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);
        }

        /// <summary>
        /// Class Draw method: inherits from the base method and draws 
        /// the ButtonPrompts and "Swap" icon if they aren't hidden.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (_isSelected)
            {
                PurchaceUI(sb);
                if (!Globals.HidePromtsAndNames)
                {
                    _buttonPrompt_E.Draw(sb);
                    _buttonPrompt_Q.Draw(sb);
                    var temp = new Rectangle(_rect.X-6-_tileWidth, _rect.Y-_tileHeight, _rect.Width, _rect.Height);
                    sb.Draw(_swapIconTxr, temp, Color.White);
                }
            }
        } 
    }
    /// <summary>
    /// Buildings are the WorldObjects that the player can Build or Upgrade.
    /// With the exception of the Bridge/Broken Bridge, buildings are placed
    /// by purchasing BuildingBlueprints from the ShopBuildingBlueprint and then
    /// interacting with an empty tile.
    /// 
    /// The same objections that were mentioned int the "BuildingBlueprint" 
    /// are valid in this case and it would be optimal to separate the various
    /// building types into their own classes.
    /// </summary>
    class Building : SelectableWorldObject, IDropOffPoint
    {
        protected Rectangle _riverSrcRect;
        public BuildingType Type { get { return _buildingType; } protected set { _buildingType = value; } }
        protected BuildingType _buildingType;

        // Worker/Work related
        protected List<Keeno> _workers;
        protected float _workSpeed;
        protected int _workerSlots;
        protected float _workDuration;
        public Vector2 Position => base.Position;

        public event Action<WorkStation> WorkStationSpawned;
        public Farm Farm { get { return _farm; } }
        protected Farm _farm;

        protected List<Rectangle> _stageSrcRects;
        protected Rectangle _buildingSrcRect;

        protected Texture2D _defaultTxr;

        protected bool _canBeUpgraded;
        protected bool _canAffordUpgrade;
        protected bool _toggleUpgrade;
        protected bool _constructionComplete;
        protected bool _singleTxrDraw;

        protected int _currLevel;
        protected int _populationCountExtention;
        protected int _woodCost, _stoneCost, _woodUpgradeCost, _stoneUpgradeCost;
        protected int _woodDelivered, _stoneDelivered;
        protected int _woodPromissed, _stonePromissed;
        protected int _totalWoodSpent, _totalStoneSpent;
        protected int _goldPrice;


        protected float _flashingFontTimer, _flashingFontTimerReset;

        /// <summary>
        /// Given the building type, the constructor determines what building to spawn.
        /// It then changes the variable according to the type.
        /// </summary>
        public Building(Point position, Texture2D BuildingSpriteSheet, BuildingType type)
            :base(position, -1)
        {
            _defaultTxr = _txr = BuildingSpriteSheet;
            _state = ObjectState.AwaitingResourceDelivery;
            _buildingType = type;
            _woodCost = -1;
            _stoneCost = -1;
            _populationCountExtention = 0;
            _woodDelivered = _woodPromissed = 0;
            _stoneDelivered = _stonePromissed = 0;
            _woodUpgradeCost = _stoneUpgradeCost = -1;
            _totalWoodSpent = _totalWoodSpent = 0;
            _workerSlots = 10;
            _workSpeed = 0f; 
            _workDuration = 6f;

            _currLevel = 0;
            _impassable = false;
            _canAffordUpgrade = false;
            _toggleUpgrade = false;
            _canBeUpgraded = true;
            _constructionComplete = false;
            _isDropOffPointActive = false;
            _singleTxrDraw = false;

            _tilePosition.X = position.X;
            _tilePosition.Y = position.Y;

            _rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);
            _workers = new List<Keeno>();

            _stageSrcRects = new List<Rectangle>();
            _stageSrcRects.Add(new Rectangle(0, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[0].X + _rect.Width, 0, _rect.Width, _rect.Height));
            _stageSrcRects.Add(new Rectangle(_stageSrcRects[1].X + _rect.Width, 0, _rect.Width, _rect.Height));

            _riverSrcRect = new Rectangle(
                  (Globals.RiverTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.RiverTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);


            // Apply Resource costs appropriately
            switch (type)
            {
                case BuildingType.Tent:
                    _name = "Tent";
                    _goldPrice = Globals.TentBLGoldPrice;
                    _woodCost = Globals.TentWoodCost;
                    _stoneCost = Globals.TentStoneCost;
                    _populationCountExtention = Globals.TentPopulationAddition;
                    _woodUpgradeCost = Globals.TentUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.TentUpgradeStoneCost;
                    break;
                case BuildingType.House:
                    _name = "House";
                    _goldPrice = Globals.HouseBLGoldPrice;
                    _woodCost = Globals.HouseWoodCost;
                    _stoneCost = Globals.HouseStoneCost;
                    _woodUpgradeCost = Globals.HouseUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.HouseUpgradeStoneCost;
                    _populationCountExtention = Globals.HousePopulationAddition;
                    break;
                case BuildingType.ResourceStorage:
                    _name = "Storage";
                    _goldPrice = Globals.ResourceStorageBLGoldPrice;
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.MonochromaticTilesetTxr;

                    _woodCost = Globals.ResourceStorageWoodCost;
                    _stoneCost = Globals.ResourceStorageStoneCost;
                    _woodUpgradeCost = Globals.ResourceStorageUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.ResourceStorageUpgradeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.ResourceStorageTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ResourceStorageTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.Bridge:
                    _state = ObjectState.Neutral;
                    _name = "Broken Bridge";
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.TilesetTxr;

                    _woodCost = Globals.BridgeWoodCost;
                    _stoneCost = Globals.BridgeStoneCost;
                    _woodUpgradeCost = Globals.BridgeUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.BridgeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.BrokenBridgeTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.BrokenBridgeTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;
                case BuildingType.FarmLand:
                    _name = "Farm Land";
                    _goldPrice = Globals.FarmLandBLGoldPrice;
                    _currLevel = 2;
                    _singleTxrDraw = true;
                    _txr = Assets.TilesetTxr;

                    _woodCost = Globals.FarmLandWoodCost;
                    _stoneCost = Globals.FarmLandStoneCost;
                    _woodUpgradeCost = Globals.FarmLandUpgradeWoodCost;
                    _stoneUpgradeCost = Globals.FarmLandUpgradeStoneCost;
                    _buildingSrcRect = new Rectangle(
                  (Globals.FarmLandTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FarmLandTileIndex/ Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                    break;

            }
            CreateLoadingBarsAndPrompts();

            _flashingFontTimer = 0f;
            _flashingFontTimerReset = .1f;
        }

        /// <summary>
        /// The Update method is in charge of managing the Keeno working on Delivering Resources
        /// and working on Constructing the building.
        /// It is also in charge of tracking the resources needed and the resources that have been delivered.
        /// </summary>
        public override void Update(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
            // Only declare that you can be upgraded if your level is under 3.
            _canBeUpgraded = _currLevel < 3 ? true : false;

            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _uiColour = Color.Red;
            }
            else
            {
                _uiColour = Color.White;
            }

            _workSpeed = 0f;
            // Sum up the workspeed of all workers assigned to work on this Building.
            foreach (var worker in _workers)
            {
                if (!worker.IsWalking)  // Only apply when they have arrived at the Building.
                {
                    // Get the worker's workspeed and apply it to the Building.
                    float kWorkspeed = worker.GetWorkSpeed();
                    _workSpeed += kWorkspeed;
                }
            }

            // Worker Work Logic
            float workerFill = _workSpeed * (deltaTime / _workDuration);
            _constructionComplete = _HGWorkProgress.Update(true, workerFill);


            switch (_state)
            {
                case ObjectState.Neutral:

                    // Only the ResourceStorage can act as a dropoff point
                    if(_buildingType == BuildingType.ResourceStorage)
                        _isDropOffPointActive = true;

                    // Upgrade related
                    if (_canBeUpgraded)
                    {
                        if (ResourceTracker.CanSpend(ResourceType.Wood, _woodUpgradeCost)
                            && ResourceTracker.CanSpend(ResourceType.Stone, _stoneUpgradeCost))
                            _canAffordUpgrade = true;
                        else
                            _canAffordUpgrade = false;
                    }

                    if(_toggleUpgrade)  // Triggered once player toggles upgrade.
                    {
                        ResetDeliveredResources();
                        _woodCost = _woodUpgradeCost;
                        _stoneCost = _stoneUpgradeCost;
                        _HGInteract.Reset();
                        _HGWorkProgress.Reset();
                        _toggleUpgrade = false;
                        _state = ObjectState.AwaitingResourceDelivery;
                    }

                    // If you're a bridge and are fixed, you are no longer impassable.
                    if(_buildingType == BuildingType.Bridge && _currLevel == 3)
                    {
                        _name = "Bridge";
                        _impassable = false;
                        break;
                    }

                    // Player Work Logic.
                    if (_isSelected)
                    {
                        // if player can upgrade the building
                        if (_canBeUpgraded && _canAffordUpgrade)
                        {
                            // Interaction.
                            _toggleUpgrade = _HGInteract.Update(Globals.E_KeyDown, Globals.UpgradeInteractSpeed);
                        }
                        // if player cannot afford upgrade 
                        else if (Globals.E_KeyDown)
                        {
                            _flashingFontTimer = _flashingFontTimerReset;
                        }
                        // ONLY IF you're NOT a bridge you can be deleted
                        if (_buildingType != BuildingType.Bridge)
                            _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, Globals.DestroyInteractSpeed);
                    }
                    // if you're a FarmLand, you're not impassable
                    if (_buildingType == BuildingType.FarmLand)
                    {
                        _impassable = false;
                        break;
                    }
                    _impassable = true;
                    break;

                case ObjectState.AwaitingResourceDelivery:

                    // if you're a FarmLand, you're not impassable
                    if (_buildingType != BuildingType.FarmLand)
                        _impassable = true;

                    // if all the resources needed have been delivered
                    if (_woodDelivered == _woodCost
                            && _stoneDelivered == _stoneCost)
                        _state = ObjectState.UnderConstruction;

                    // if farm is not null, destroy it
                    _farm?.DestroyMe();
                    _farm = null;
                    break;

                    case ObjectState.UnderConstruction:
                    // if you're a FarmLand, you're not impassable
                    if (_buildingType != BuildingType.FarmLand)
                        _impassable = true;

                    if (_constructionComplete)
                    {
                        // Play the appropriate sound
                        var sound = Assets.BuildingUpgradedSFX;
                        var soundInst = sound.CreateInstance();
                        soundInst.Volume = .4f;
                        soundInst.Play();

                        // Track the resources the player has spent on this building,
                        // as the player will get a full refund when destroying the building.
                        _totalWoodSpent += _woodDelivered;
                        _totalStoneSpent += _stoneDelivered;
                        ResourceTracker.Add(ResourceType.Housing, _populationCountExtention);

                        // Notify the Keeno working on this building that
                        // the construction has been completed.
                        ClearWorkerList();
                        _constructionComplete = false;
                        _state = ObjectState.Neutral;

                        // If you're a FarmLand
                        if(_buildingType == BuildingType.FarmLand)
                        {
                            if (_farm is null)
                            {
                                int x, y;
                                x = _tilePosition.X / 16;
                                y = _tilePosition.Y / 16;

                                // FARMLAND SPECIFIFC INTERACTION
                                // Spawn a farm (WorkStation)
                                _farm = new Farm(new Point(x,y), Globals.FarmTileIndex1, true);
                                WorkStationSpawned?.Invoke(_farm);
                            }
                            break;
                        }
                        _currLevel++;
                    }
                    break;
                default:
                    _impassable = false;
                    break;
            }

            // If the player has used the "destroy" interaction
            if (_destroyMe)
            {
                var sound = Assets.BuildingRemovedSFX;
                var soundInst = sound.CreateInstance();
                soundInst.Volume = .8f;
                soundInst.Play();

                // Refund the resources spent on the building
                ResourceTracker.Spend(ResourceType.Housing, _currLevel * _populationCountExtention);
                ResourceTracker.Add(ResourceType.Wood, _totalWoodSpent);
                ResourceTracker.Add(ResourceType.Stone, _totalStoneSpent);
                ResourceTracker.Add(ResourceType.Gold, _goldPrice);

                DestroyMe();
            }

            // Tell workers where to go
            foreach (Keeno keeno in _workers)
            {
                var temp = Position.ToPoint();
                temp.X -= 8;
                temp.Y -= 8;
                if (keeno.State == KeenoState.Building)
                    keeno.MoveTo(temp);
            }
            base.Update(gt);

        }

        /// <summary>
        /// Method called by Map to Reset the game completely.
        /// </summary>
        public override void DestroyMeAndMyWorkers()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                _workers.RemoveAt(i);
            }
            base.DestroyMeAndMyWorkers();
        }
        private void ResetDeliveredResources()
        {
            _woodDelivered = 0;
            _stoneDelivered = 0;
        }

        /// <summary>
        /// Method the Keeno working on the building call 
        /// once they successfully deliver a resource to the building.
        /// </summary>
        public void TakeThisResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:
                    _woodDelivered++;
                    _woodPromissed--;
                        break;
                case ResourceType.Stone:
                    _stoneDelivered++;
                    _stonePromissed--;
                    break;
            }
        }

        /// <summary>
        /// Method called by the Keeno working on the building
        /// when they are interrupted. (Bell is rung, day has ended)
        /// Updates the real total of resources delivered to the Building
        /// so that it doesn't falsly expect a resource from a Keeno that
        /// cannot/will not bring it.
        /// </summary>
        public void DontTakeThisResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Wood:
                    _woodPromissed--;
                    break;
                case ResourceType.Stone:
                    _stonePromissed--;
                    break;
            }
        }

        /// <summary>
        /// Method the Keeno check to determine what resource to bring to the
        /// Building.
        /// 
        /// Method is LOCKED, so that only one Keeno can check this at a time.
        /// (Avoids overpromissed resources. 
        /// Example: 10 Keeno are waiting to build and player builds something that 
        /// requirs 1 Wood. All Keeno call this method and see that they can bring wood
        /// on the same update, thus they bring 10 Wood.)
        /// </summary>
        public ResourceType CheckCosts()
        {
            lock (this)
            {
                ResourceType type = GetNextDeliverableResource();

                switch (type)
                {
                    case ResourceType.Wood:
                        _woodPromissed++;
                        break;

                    case ResourceType.Stone:
                        _stonePromissed++;
                        break;
                }

                return type;
            }
        }
        public ResourceType GetNextDeliverableResource()
        {
            int woodNeeded = _woodCost - (_woodDelivered + _woodPromissed);

            if (woodNeeded > 0 && ResourceTracker.CanSpend(ResourceType.Wood, 1))
                return ResourceType.Wood;

            int stoneNeeded = _stoneCost - (_stoneDelivered + _stonePromissed);

            if (stoneNeeded > 0 && ResourceTracker.CanSpend(ResourceType.Stone, 1))
                return ResourceType.Stone;

            return ResourceType.None;
        }

        /// <summary>
        /// Determines what to do when selected depending on the building type.
        /// </summary>
        public virtual void Selected(float playerWorkSpeed, bool condition)
        {
            if (_buildingType == BuildingType.Bridge && _currLevel == 3)
                return;
            // if there's a farm on this tile, or the farm is depleated
            if (_farm == null || _farm.State == ObjectState.Broken)
                base.Selected();
            else
                _farm.Selected(playerWorkSpeed, condition);
        }

        #region Workers
        // Method that manage the Keeno Assigned to "work" on this building.
        //// See WorkStation Methods with the same Names \\\\
        public virtual void ReduceWorkerSlots()
        {
            if (_workerSlots > 0)
                _workerSlots--;
        }
        public virtual void IncreaseWorkerSlots()
        {
            _workerSlots++;
        }
        public virtual void TakeThisWorker(Keeno worker)
        {
            _workers.Add(worker);
            if(_state == ObjectState.UnderConstruction)
                worker.SwitchToBuilding();
            //else
                //worker.SwitchToWorking();
            ReduceWorkerSlots();
        }
        public void ClearWorkerList()
        {
            foreach (var worker in _workers)
            {
                worker.SwitchToWalkingToBuilderCabin();
                IncreaseWorkerSlots();
            }
            _canDropOff = false;
            //_canUse = false;
            _cannotUse = false;
            _HGDropOff.Reset();
            _HGInteract.Reset();
            _HGCantInteract.Reset();
            _HGWorkProgress.Reset();

            _workers.Clear();
        }
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_workerSlots > 0)
            {
                TakeThisWorker(worker);
                return true;
            }
            return false;
        }
        #endregion

        #region Draw Methods
       
        /// <summary>
        /// Draws the UI displaying the remaining resources the Keeno need to deliver to the building
        /// for it to become Under Construction (ready to be built).
        /// </summary>
        /// <param name="sb"></param>
        protected void ResourcesNeededUI(SpriteBatch sb)
        {
            string woodDelivered = _woodDelivered.ToString();
            Vector2 woodDeliveredTextSize = _descriptionFont.MeasureString(woodDelivered);
            Vector2 woodDeliveredTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

            string stoneDeliveredText = _stoneDelivered.ToString();
            Vector2 stoneDeliveredTextSize = _descriptionFont.MeasureString(stoneDeliveredText);
            Vector2 stoneDeliveredTextPos = new Vector2(_rect.Right + 16, woodDeliveredTextPos.Y + woodDeliveredTextSize.Y / 2 + 2);

            if (_woodCost > 0)
            {
                sb.DrawString(_descriptionFont, woodDelivered + "/" + _woodCost, woodDeliveredTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                Vector2 woodUIPos = new Vector2(woodDeliveredTextPos.X - 8, woodDeliveredTextPos.Y + 4);
                sb.Draw(Assets.UIWoodIconTxr, woodUIPos, null, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);
            }
            if (_stoneCost > 0)
            {
                sb.DrawString(_descriptionFont, stoneDeliveredText + "/" + _stoneCost, stoneDeliveredTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                Vector2 stoneUIPos = new Vector2(stoneDeliveredTextPos.X - 8, stoneDeliveredTextPos.Y + 5);
                sb.Draw(Assets.UIStoneIconTxr, stoneUIPos, null, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);

            }
        }

        /// <summary>
        /// Draws UI that displays the relevant resource costs needed
        /// to Upgrade the building.
        /// </summary>
        protected void UpgradeUI(SpriteBatch sb)
        {
            if (_canBeUpgraded)
            {
                _buttonPrompt_E.Draw(sb);
                string woodUpgradeText = _woodUpgradeCost.ToString();
                Vector2 woodUpgradetextSize = _descriptionFont.MeasureString(woodUpgradeText);
                Vector2 woodUpgradeTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);

                string stoneUpgradeText = _stoneUpgradeCost.ToString();
                Vector2 stoneUpgradetextSize = _descriptionFont.MeasureString(stoneUpgradeText);
                Vector2 stoneUpgradeTextPos = new Vector2(_rect.Right + 16, woodUpgradeTextPos.Y+ woodUpgradetextSize.Y/2+2);
                
                if(_woodUpgradeCost > 0)
                {
                    sb.DrawString(_descriptionFont, woodUpgradeText, woodUpgradeTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                    Vector2 woodUIPos = new Vector2(woodUpgradeTextPos.X - 8, woodUpgradeTextPos.Y + 4);
                    sb.Draw(Assets.UIWoodIconTxr, woodUIPos, null, _uiColour, 0f, Vector2.Zero, 1,SpriteEffects.None,Globals.InGameUILD);
                }
                if(_stoneUpgradeCost > 0)
                {
                    sb.DrawString(_descriptionFont, stoneUpgradeText, stoneUpgradeTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
                    Vector2 stoneUIPos = new Vector2(stoneUpgradeTextPos.X - 8, stoneUpgradeTextPos.Y + 5);
                    sb.Draw(Assets.UIStoneIconTxr, stoneUIPos, null, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, Globals.InGameUILD);

                }
            }
        }

        /// <summary>
        /// Changes Txr to indicate the building is Under Construction.
        /// </summary>
        /// <param name="sb"></param>
        public void UnderConstructionDraw(SpriteBatch sb)
        {
            _txr = Assets.TilesetTxr;
            _srcRect = new Rectangle(
              (Globals.ConstructionSiteTileIndex % _tilesetColumns) * _tileWidth,
              (Globals.ConstructionSiteTileIndex / _tilesetColumns) * _tileHeight,
              _tileWidth,
              _tileHeight);
            sb.Draw(_txr, _rect, _srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
        }
        /// <summary>
        /// Draw method for buildings such as Tens and Houses that have
        /// multiple level dependant drawing stages 
        /// </summary>
        /// <param name="sb"></param>
        public void CurrLevelDraw(SpriteBatch sb)
        {
            _txr = _defaultTxr;
            // Draw based on level
            for (int i = 0; i < _currLevel; i++)
            {
                sb.Draw(_txr, _rect, _stageSrcRects[i], Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
            }
        }

        /// <summary>
        /// Draw Method for the building that don't have multiple txrs that
        /// draw based on the building level.
        /// </summary>
        public void SingleTxrDraw(SpriteBatch sb)
        {
            // Draw the River texture
            if (_buildingType == BuildingType.Bridge)
                sb.Draw(_txr, _rect, _riverSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.RiverLD);

            // Draw the fixed bridge
            if (_buildingType == BuildingType.Bridge && _currLevel == 3)
            {
                _buildingSrcRect= new Rectangle(
                  (Globals.FixedBridgeTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FixedBridgeTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height);
                sb.Draw(_txr, _rect, _buildingSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);
                return;
            }

            sb.Draw(_txr, _rect, _buildingSrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.BuildingLD);

        }

        /// <summary>
        /// Draws SelectedTxr, Text Description, Upgrade UI accordingly
        /// </summary>
        /// <param name="sb"></param>
        public override void SelectedDraw(SpriteBatch sb)
        {
            if (_isSelected)
            {

                switch (_state)
                {
                    case ObjectState.AwaitingResourceDelivery:
                        ResourcesNeededUI(sb);
                        break;
                    case ObjectState.Neutral:
                        TextDescription(sb);
                        _HGInteract.Draw(sb);
                        _HGDestroy.Draw(sb);
                        if (_buildingType != BuildingType.Bridge)
                            _buttonPrompt_X.Draw(sb);
                        // if the structure isn't upgradable
                        if (_woodUpgradeCost == -1 || _stoneUpgradeCost == -1)
                            break;
                        UpgradeUI(sb);
                        break;
                }

                // draw selected outline
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, _tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            }

        }

        /// <summary>
        /// Overall draw method, connects all the previous method and slots them into the appropriate
        /// situations depending on the Building State.
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            // Don't draw if dead
            if(_state == ObjectState.Dead)
                return;

            SelectedDraw(sb);

            // Draw according to Current Building Type
            switch (_state)
            {
                case ObjectState.AwaitingResourceDelivery:
                    UnderConstructionDraw(sb);
                    break;
                case ObjectState.UnderConstruction:
                    UnderConstructionDraw(sb);
                    _HGWorkProgress.Draw(sb);
                    break;
                default:
                    if (_singleTxrDraw)
                        SingleTxrDraw(sb);
                    else
                        CurrLevelDraw(sb);
                    break;
            }
        }
        #endregion

    }
    /// <summary>
    /// WorkStations are all the WorldObjects that Keenos can work at:
    /// Trees, Farms, Rocks...
    /// </summary>
    class WorkStation : SelectableWorldObject
    {
        #region Variables

        protected List<Keeno> _workers;

        protected SoundEffect _workSFX;
        protected SoundEffect? _workstationDepletedSFX;

        protected Texture2D _tilesetTxr;
        protected Texture2D _whiteTxr;
        protected Texture2D _defaultTxr;

        protected float _workSpeed;
        protected float _workDuration;
        protected float _playerWorkSpeed;
        protected float _flashingTxrTimer;
        protected float _flashingTxrTimerReset;

        protected int _workerSlots;
        protected int _resourceAmount;

        protected bool _diesWhenBroken;
        protected bool _playerHarvestedResource;
        protected bool _workerHarvestedResource;
        protected bool _hasToBeCollected;
        protected bool _selectedCondition;      // in most cases checking if player has followers
        protected bool _flashesWhenHarvested;
        protected bool _brokenByPlayer;         // checking if the player broke the resource
        protected bool _hasPlayedDepletedSFX;

        #endregion
        public WorkStation(Point tilePosition, int globalTileIndex)
            : base (tilePosition, globalTileIndex)
        {

            _workers = new List<Keeno>();
            // Default values
            _resourceType = ResourceType.None;
            _resourceAmount = 0;
            _workSpeed = 0f;
            _workerSlots = 1;
            _workDuration = 10f;
            _health = 1;

            _defaultTxr = _txr;
            _whiteTxr = Assets.MonochromaticTilesetTxr;
            _tilesetTxr = Assets.TilesetTxr;

            _playerHarvestedResource = false;
            _canDropOff = false;
            _selectedCondition = false;
            _canBeSelectedWhenBroken = true;
            _flashesWhenHarvested = true;
            _hasToBeCollected = false;
            _diesWhenBroken = false;
            _brokenByPlayer = false;
            _hasPlayedDepletedSFX = false;

            _coreRect = new Rectangle(_rect.X+_rect.Width/4,_rect.Y+_rect.Height/4, _rect.Width/2, _rect.Height/2);


            _flashingTxrTimer = 0;
            _flashingTxrTimerReset = .02f;
        }

        /// <summary>
        /// More nuanced than the Parent method as it takes in a couple variables
        /// from the player.
        /// </summary>
        public virtual void Selected(float playerWorkSpeed, bool condition)
        {
            base.Selected();
            _playerWorkSpeed = playerWorkSpeed;
            // in most cases checking if player has followers
            _selectedCondition = condition;
        }

        /// <summary>
        /// The Update method is in charge of managing the Keeno working the WorkStation.
        /// It also determines how to behave given the object state.
        /// </summary>
        public override void Update(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

            // if this WorkStation flashes when a unit of resource is harvested
            if (_flashesWhenHarvested)
            {
                // apply the appropriate effect
                if (_flashingTxrTimer > 0)
                {
                    _flashingTxrTimer -= deltaTime;
                    _txr = _whiteTxr;
                }
                else
                {
                    _txr = _defaultTxr;
                    _txrRotationDegrees = 0f;
                }
            }

            _workSpeed = 0f;
            foreach (var worker in _workers)
            {
                if (!worker.IsWalking)  // Only apply when they have arrived at the Workstation
                {
                    // Get the worker's workspeed and apply it to the Workstation
                    float kWorkspeed = worker.GetWorkSpeed();
                    _workSpeed += kWorkspeed;
                }
            }

            switch (_state)
            {
                case ObjectState.Broken:
                    ClearWorkerList();

                    if (_diesWhenBroken)
                    {
                        _state = ObjectState.Dead;
                        break;
                    }

                    if(!_canBeSelectedWhenBroken)
                        break;

                    if (_isSelected)
                    {
                        // The only interaction the player can perform is delete the workstation
                        _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, Globals.DestroyInteractSpeed);
                        _playerHarvestedResource = false;
                        _workerHarvestedResource = false;
                    }
                    break;

                case ObjectState.Harvestable:

                    if (_isSelected)
                    {
                        // Player Work Logic
                        float playerFill = _playerWorkSpeed * (deltaTime / _workDuration);
                        // Interaction
                        _playerHarvestedResource = _HGInteract.Update(Globals.E_KeyDown, playerFill);
                        // worker DropOff
                        if (_workerSlots > 0 && _selectedCondition)
                        {
                            _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, Globals.DropOffKeenoSpeed);
                        }
                    }
                    // Worker Work Logic
                    float workerFill = _workSpeed * (deltaTime / _workDuration);

                    _workerHarvestedResource = _HGWorkProgress.Update(true, workerFill);

                    // Tell workers where to go
                    foreach (Keeno keeno in _workers)
                    {
                        if(keeno.State == KeenoState.Working)
                            keeno.MoveTo(_tilePosition);
                    }
                    // Harvest Resource
                    if (_playerHarvestedResource)
                    {
                        if (!_hasToBeCollected)
                            PlayerHarvestedResource(_resourceType, _resourceAmount);
                        else
                            PlayerHarvestedResource(ResourceType.None, 0);
                    }
                    // Worker Harvested Resource
                    if (_workerHarvestedResource)
                    {
                        if (!_hasToBeCollected)
                            WorkerHarvestedResource(_resourceType, _resourceAmount);
                        else
                            WorkerBrokeResource();
                    }

                    break;

                    // This case is only really called by the Builders Cabin
                    case ObjectState.Neutral:

                    if (_isSelected)
                    {
                        if (_specialInteraction)
                        {
                            DoSpecialInteraction();
                            _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
                        }
                        // worker DropOff
                        if (_workerSlots > 0 && _selectedCondition && _state != ObjectState.Broken)
                        {
                            _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, Globals.DropOffKeenoSpeed);
                        }
                    }
                    // Tell workers where to go
                    foreach (Keeno keeno in _workers)
                    {
                        if (keeno.State == KeenoState.ReadyToBuild)
                            keeno.MoveTo(_tilePosition);
                    }
                    break;
            }
            if (_destroyMe)
                DestroyMe();

            // Health Check
            if (_health == 0)
            {
                if(_workstationDepletedSFX != null)
                {
                    var temp = _workstationDepletedSFX.CreateInstance();
                    temp.Play();
                }
                _hasPlayedDepletedSFX = true;
                _health--;
                _state = ObjectState.Broken;
            }

            // Set selected to false;
            // Reset all HG
            base.Update(gt);
        }
        
        /// <summary>
        /// To be overriten by Builders cabin.
        /// </summary>
        public virtual void DoSpecialInteraction(){}

        /// <summary>
        /// Called by Map to Completely reset the game/Map.
        /// </summary>
        public override void DestroyMeAndMyWorkers()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                _workers[i].Die();
            }
            base.DestroyMeAndMyWorkers();
        }

        #region Resources/Workers

        /// <summary>
        /// When the Keeno assigned to work at this WorkStation have performed the
        /// ammount of work required, call this method.
        /// 
        /// Tells the worker to drop off the resource collected.
        /// </summary>
        public virtual void WorkerHarvestedResource(ResourceType type, int amount)
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                if (!_workers[i].IsWalking)
                {
                    _workers[i].DropOffResources(type, amount);
                    break;
                }
            }
            ApplyHitEffect();
            _health--;
            //ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();
        }

        /// <summary>
        /// Similar to the previous method, but the resource harvested
        /// was the last one the Workstation had, therefore
        /// the Keeno should not return to the WorkStation.
        /// </summary>
        public virtual void WorkerBrokeResource()
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                if (!_workers[i].IsWalking)
                {
                    _workers[i].SwitchWalkingToIdleSpot();
                    break;
                }
            }
            ApplyHitEffect();
            _health--;
            //ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }

        /// <summary>
        /// Called when the player is the one harvesting the resource.
        /// The player instantly gains the resource.
        /// </summary>
        public virtual void PlayerHarvestedResource(ResourceType type, int amount)
        {
            ApplyHitEffect();
            _health--;
            if (_health == 0)
                _brokenByPlayer = true;
            ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }

        /// <summary>
        /// Visual effect applied to the WorkStation when a resource has been harvested.
        /// </summary>
        public virtual void ApplyHitEffect()
        {
            if (_flashesWhenHarvested)
            {
                // Apply a slight rotation and reset the flashing timer
                // so that the Workstation flashes when hit
                float rand = Globals.RNG.Next(0, 2) == 0 ? 3 : -3;
                _txrRotationDegrees = rand;
                _flashingTxrTimer = _flashingTxrTimerReset;
            }
        }

        /// <summary>
        /// When a worker has been assigned to this Workstation,
        /// reduce the slots available for more workers.
        /// </summary>
        public virtual void ReduceWorkerSlots()
        {
            if (_workerSlots > 0)
                _workerSlots--;
        }

        /// <summary>
        /// When a worker leaves the WorkStation,
        /// increase the slots available for more workers.
        /// </summary>
        public virtual void IncreaseWorkerSlots()
        {
            _workerSlots++;
        }

        /// <summary>
        /// When the player drops off a Keeno to work on this WorkStation,
        /// Add this worker to your list of workers.
        /// </summary>
        public virtual void TakeThisWorker(Keeno worker)
        {
            worker.TakeWorkSoundEffect(_workSFX);

            _workers.Add(worker);
            if (_state == ObjectState.Harvestable)
                worker.SwitchToWorking();
            else if(_state == ObjectState.Neutral)
                worker.SwitchToReadyToBuild();
            ReduceWorkerSlots();
        }

        /// <summary>
        /// Empties the list of workers and tells them what to do depending on
        /// circumstance.
        /// Resets relevant bools and HourGlasses.
        /// </summary>
        public void ClearWorkerList()
        {
            foreach (var keeno in _workers)
            {
                if(_hasToBeCollected)
                    keeno.SwitchWalkingToIdleSpot();
                else if(keeno.State == KeenoState.Working && !_brokenByPlayer)
                {
                    if(keeno.IsWalking)
                        keeno.DropOffAndIdle(_resourceType, 0);
                    else
                        keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                }
                else if (keeno.State == KeenoState.DroppingOff && !_brokenByPlayer)
                    keeno.DropOffAndIdle(_resourceType, _resourceAmount);
                else if(_brokenByPlayer)
                    keeno.SwitchWalkingToIdleSpot();
                // in case this workstation still exists and can be harvested once more
                // reset the total amount of worker slots to the default
                IncreaseWorkerSlots();
            }
            _canDropOff = false;
            _canUse = false;
            _cannotUse = false;
            _HGDropOff.Reset();
            _HGInteract.Reset();
            _HGCantInteract.Reset();
            _HGWorkProgress.Reset();

            _workers.Clear();
        }

        /// <summary>
        /// Before the player drops off a Keeno, this method is called
        /// to determine wether the player can Drop it off.
        /// </summary>
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_canDropOff && _workerSlots > 0
                && _state !=ObjectState.Broken && _state != ObjectState.Dead)
            {
                TakeThisWorker(worker);
                _HGDropOff.Reset();
                return true;
            }
            return false;
        }
        #endregion

        public override void DestroyMe()
        {
            ClearWorkerList();
            // Set state to Dead
            base.DestroyMe();
        }
        public virtual void SetHealthTo(int healthTotal)
        {
            _health = healthTotal;
            _state = ObjectState.Harvestable;
            ChangeTextureBackToDefault();
        }

        /// <summary>
        /// Method that is overriden by children that have different
        /// textures when broken. For example Trees and stumps.
        /// </summary>
        public virtual void ChangeTextureToBroken()
        {
      
        }
        /// <summary>
        /// Method that is overriden by children that have different
        /// textures when broken. For example Trees and stumps.
        /// </summary>
        public virtual void ChangeTextureBackToDefault()
        {

        }

        /// <summary>
        /// Overall draw method, connects all the previous method and slots them into the appropriate
        /// situations depending on the ObjectState.
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            switch (_state)
            {
                case ObjectState.Dead:
                    return;

                case ObjectState.Harvestable:
                    if (_isSelected)
                    {
                        // HourGlasses
                        _HGInteract.Draw(sb);
                        _HGDropOff.Draw(sb);
                        // Input Promts
                        _buttonPrompt_E.Draw(sb);
                        if(_workerSlots > 0 && _selectedCondition)
                            _buttonPrompt_Q.Draw(sb);
                    }
                    break;

                case ObjectState.Neutral:
                    if (_isSelected)
                    {
                        _HGDropOff.Draw(sb);

                        if (_workerSlots > 0 && _selectedCondition)
                            _buttonPrompt_Q.Draw(sb);
                    }
                    break;

                case ObjectState.Broken:
                    ChangeTextureToBroken();
                    if (!_canBeSelectedWhenBroken)
                        break;
                    if (_isSelected)
                    {
                        _buttonPrompt_X.Draw(sb);
                        _HGDestroy.Draw(sb);
                    }
                    break;
            }
            base.Draw(sb);
        }
    }
    #region Resources / Breakables
    // The following are variations of WorkStations.
    // Their main differences are the resource type recieved when
    // harvesting them, the time required to collect resources from them
    // and visual differences.

    class Tree : WorkStation
    {
        private Texture2D _choppedTreeTxr;

        public Tree(Point tilePosition, int globalTileIndex, bool isBroken)
            : base(tilePosition, globalTileIndex)
        {
            _resourceType = ResourceType.Wood;
            _resourceAmount = Globals.TreeWoodAmount;
            _health = Globals.TreeHealth;
            _workerSlots = Globals.TreeWorkerSlots;
            _workDuration = Globals.TreeWorkAmount;

            _choppedTreeTxr = Assets.ChoppedTreeTxr;
            _impassable = true;

            _workSFX = Assets.WoodCuttingSFX;

            if (isBroken)
                Health = 0;
        }
        public override void ChangeTextureToBroken()
        {
            _srcRect = null;
            _txr = _choppedTreeTxr;
        }
    }
    class Farm : WorkStation
    {
        private Rectangle _farmLandSrc;
        public Farm(Point tilePosition, int globalTileIndex, bool isPlayerMade) 
            : base(tilePosition, globalTileIndex)
        {
            _workSFX = Assets.WorkingOnFarmSFX;
            _resourceType = ResourceType.Food;
            _resourceAmount = Globals.FarmFoodAmount;
            _health = Globals.FarmHealth;
            _workerSlots = Globals.FarmWorkerSlots;
            _workDuration = Globals.FarmWorkAmount;

            // If it the one spawned by the FarmLand Building
            if (isPlayerMade)
            {
                _workDuration = Globals.PlayerMadeFarmWorkAmount;
                _health = Globals.PlayerMadeFarmHealth;
            }

            _txr = Assets.TilesetTxr;
            _impassable = false;

            _farmLandSrc = new Rectangle(
                  (Globals.FarmLandTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.FarmLandTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void ChangeTextureToBroken()
        {
            _srcRect = new Rectangle(
                  (Globals.HarvestedFarmTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedFarmTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        public override void ChangeTextureBackToDefault()
        {
            _srcRect = new Rectangle(
                  (Globals.FarmTileIndex1 % _tilesetColumns) * _tileWidth,
                  (Globals.FarmTileIndex1 / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class RockFormation : WorkStation
    {
        public RockFormation(Point tilePosition, int globalTileIndex, bool isBroken)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _resourceType = ResourceType.Stone;
            _resourceAmount = Globals.RockStoneAmount;
            _health = Globals.RockHealth;
            _workerSlots = Globals.RockWorkerSlots;
            _workDuration = Globals.RockWorkAmount;
            _workSFX = Assets.StoneCuttingSFX;

            _impassable = true;

            _srcRect = null;
            _txr = Assets.RockTxr;
            _defaultTxr = _txr;
            _whiteTxr = Assets.WhiteRockTxr;

            if (isBroken)
                _state = ObjectState.Broken;
        }
        public override void ChangeTextureToBroken()
        {
            _impassable = false ;
            _txr = _tilesetTxr;
            _srcRect = new Rectangle(
                  (Globals.HarvestedRockTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedRockTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class GoldFromation : WorkStation
    {
        public GoldFromation(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _resourceType = ResourceType.Gold;
            _resourceAmount = Globals.GoldGoldAmount;
            _health = Globals.GoldHealth;
            _workerSlots = Globals.GoldWorkerSlots;
            _workDuration = Globals.GoldWorkAmount;
            _workSFX = Assets.StoneCuttingSFX;

            _impassable = true;
            _canBeSelectedWhenBroken = false;
            _flashesWhenHarvested = false;
            _hasToBeCollected = true;

            _txr = Assets.GoldOreTxr;
            _srcRect = null;
        }
        public override void ChangeTextureToBroken()
        {
            _impassable = false;
            _txr = _tilesetTxr;
            _srcRect = new Rectangle(
                  (Globals.GoldCoinTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.GoldCoinTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
        /// <summary>
        /// Method called when the player collides with the 
        /// Gold Coin spawned by this WorkStation.
        /// </summary>
        public void GatherGoldCoin()
        {
            PlayerHarvestedResource(_resourceType, _resourceAmount);
            _state = ObjectState.Dead;
        }
    }
    class BreakableWall : WorkStation
    {
        public BreakableWall(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _workstationDepletedSFX = Assets.RockBrokenSFX;
            _workDuration = Globals.BreakableWallWorkAmount;
            _workerSlots = Globals.BreakableWallWorkerSlots;
            _resourceType = ResourceType.None;
            _resourceAmount = 0;
            _health = Globals.BreakableWallHealth;
            _workSFX = Assets.StoneCuttingSFX;

            _impassable = true;
            _diesWhenBroken = true;
        }
    }
    #endregion

    /// <summary>
    /// Interface that a couple WorldObjects inherit.
    /// Allows Keeno to drop resources off at said WorldObject.
    /// </summary>
    interface IDropOffPoint
    {
        public Vector2 Position { get; }
    }

    /// <summary>
    /// The TownCentre is a vital part of the game.
    /// It's the only way to spawn Keeno, it's the player's first dropoff point.
    /// Once the player can purchase a Keeno, fires an event to notify that a keeno should spawn.
    /// </summary>
    class TownCentre : SelectableWorldObject, IDropOffPoint 
    {
        private Map _map;
        private List<Keeno> _keenosISpawned;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenosISpwaned { get { return _keenosISpawned; } }
        public Vector2 Position => base.Position;

        private float _flashingFontTimer, _flashingFontTimerReset;

        public TownCentre(Point tilePosition, int globalTileIndex, Map map) 
            : base(tilePosition, globalTileIndex)
        {
            _name = "Town Centre";
            _state = ObjectState.Neutral;
            _keenosISpawned = new List<Keeno>();
            _isDropOffPointActive = true;
            _map = map;
            _flashingFontTimer = 0f;
            _flashingFontTimerReset = .1f;

            ResourceTracker.Add(ResourceType.Housing, 5);
        }

        /// <summary>
        /// In charge of all the TC's behaviuours.
        /// </summary>
        public override void Update(GameTime gt)
        {
            // Cheat to spawn Keeno without spending resources
            // Also ignores Housing Cap
            if (Globals.UpArrow_KeyDown && Globals.K_KeyDown && Globals.LeftShift_KeyDown)
                SpawnKeeno();

            // apply the appropriate effect
            if (_flashingFontTimer > 0)
            {
                _flashingFontTimer -= Globals.DeltaTime;
                _uiColour = Color.Red;
            }
            else
                _uiColour = Color.White;
            
            // When selected check for player input and attempt to "buy" a Keeno.
            if (_isSelected)
            {
                // if the player can spend the following resources
                if (ResourceTracker.CanSpend(ResourceType.Food,
                    ResourceTracker.KeenoCost) && ResourceTracker.HasHousingSpace(1))
                    _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
                else if (Globals.E_KeyDown)
                    _flashingFontTimer = _flashingFontTimerReset;
                else 
                    _canUse = false; 
            }
            else
            _HGInteract.Reset();

            if (_canUse && _isSelected)
            {
                if (ResourceTracker.CanSpend(ResourceType.Food,
                    ResourceTracker.KeenoCost) && ResourceTracker.HasHousingSpace(1))
                {
                    ResourceTracker.Spend(ResourceType.Food,
                    ResourceTracker.KeenoCost);
                    SpawnKeeno();
                    _uiColour = Color.White;
                }
                _HGInteract.Reset();
            }
            base.Update(gt);
        }
        public override void OnInteract()
        {

        }

        /// <summary>
        /// Extracted this method to keep the Update cleaner.
        /// Fires the "KeenoSpawned" event.
        /// </summary>
        private void SpawnKeeno()
        {
            var SFX = Assets.KeenoSpawnSFX.CreateInstance();
            SFX.Play();

            Rectangle temp = new Rectangle(_rect.X-_rect.Width/2, _rect.Y , 16, 16);
            var newKeeno = new Keeno(Assets.KeenoTxr, 5, temp, _map, false);
            _keenosISpawned.Add(newKeeno);
            KeenoSpawned?.Invoke(newKeeno);

            // The Keeno Resource only exists to visually track them in UI.
            ResourceTracker.Add(ResourceType.Keeno, 1);
        }

        /// <summary>
        /// Draws TC, SelectedTxr, the UI Display for buying Keeno and the
        /// Appropriate HourGlasses.
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (_isSelected)
            {
                _HGCantInteract.Draw(sb);
                _HGInteract.Draw(sb);
                KeenoCostDisplay(sb);
            }
        }

        /// <summary>
        /// Method that displays the appropriate UI to show the player the cost of buying Keeno.
        /// </summary>
        /// <param name="sb"></param>
        private void KeenoCostDisplay(SpriteBatch sb)
        {
            _buttonPrompt_E.Draw(sb);
            string keenoFoodCostText = ResourceTracker.KeenoCost.ToString();
            Vector2 KeenoFoodCostTextSize = _descriptionFont.MeasureString(keenoFoodCostText);
            Vector2 keenoFoodCostTextPos = new Vector2(_rect.Right + 16, _rect.Top - 16);
            
            sb.DrawString(_descriptionFont, keenoFoodCostText, keenoFoodCostTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 foodUIPos = new Vector2(keenoFoodCostTextPos.X - 8, keenoFoodCostTextPos.Y + 4);
            sb.Draw(Assets.UIFoodIconTxr, foodUIPos, _uiColour);

            string keenoHousingCostText = "1";
            Vector2 keenoHousingCostTextSize = _descriptionFont.MeasureString(keenoHousingCostText);
            Vector2 keenoHousingCostTextPos = new Vector2(_rect.Right + keenoHousingCostTextSize.X + 10, _rect.Top - 16+ KeenoFoodCostTextSize.Y);

            sb.DrawString(_descriptionFont, keenoHousingCostText, keenoHousingCostTextPos, _uiColour, 0f, Vector2.Zero, 1, SpriteEffects.None, .099f);
            Vector2 housingUIPos = new Vector2(keenoHousingCostTextPos.X - 8, keenoHousingCostTextPos.Y + 5);
            sb.Draw(Assets.UIHousingIconTxr, housingUIPos, _uiColour);
        }
    }
    class BuilderCabin : WorkStation
    {
        public BuilderCabin(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            //_workerSlots = 10;
            _workerSlots = 20;

            _name = "Builders Cabin";
        }
        /// <summary>
        /// Only difference from the parent class is the Keeno working on the Builders Cabin
        /// are prompted to remember where the it is, as once they finish building, they must
        /// return.
        /// </summary>
        public override void Update(GameTime gt)
        {
            foreach (var worker in _workers)
            {
                worker.RememberThisBuilderCabin(Position.ToPoint());
            }
            base.Update(gt);
        }
    }
    /// <summary>
    /// The Bell is a special type of WorkStation:
    /// It takes no workers, it can only be interacted with by the player.
    /// The only reason it's a WorkStation is because it's conventient in the player class
    /// given the time constrain. Given more time I'd just have it be a Selectable WorldObject,
    /// OR I'd make child of SelectableWorldObject called InteractableWorldObject.
    /// </summary>
    class Bell : WorkStation
    {
        public event Action BellRung;
        public Bell(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            _workerSlots = 0;
            _name = "Bell";
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void OnInteract()
        {
            Assets.BellSoundSFX.Play();
            BellRung.Invoke();
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                _buttonPrompt_E.Draw(sb);
            base.Draw(sb);
        }
    }

    /// <summary>
    /// The Shop only really displays its name to notify to the player that the
    /// blueprint on top of it is the shop's bluprint... without the Shop,
    /// the blueprint is somewhat awkwardly in the open and the player doesn't have
    /// sufficient context for it.
    /// </summary>
    class Shop : SelectableWorldObject
    {
        public Shop(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _state = ObjectState.Neutral;
            _name = "Shop";
        }
    }
    #region Tile Property Related
    /// <summary>
    /// Door is simply a WorldObject the player can walk over.
    /// </summary>
    class Door : WorldObject
    {
        public Door(Point position, int globalTileIndex)
            : base(position, globalTileIndex)
        {
            _impassable = false;
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }

    /// <summary>
    /// EmptyTile is a SelectableWorldObject that the player interacts with
    /// only in BuildMode when placing Buildings. Like the Door it is not impassable.
    /// </summary>
    class EmptyTile : SelectableWorldObject
    {
        private bool _rngFoliage;

        public EmptyTile(Point tilePosition, int globalTileIndex) 
            : base(tilePosition, globalTileIndex)
        {
            _impassable = false;

            // For some visual interest, EmptyTile attempts to spawn some sort of foliage.
            // BUT not bellow Y == 11, as that's where the caves are.
            // It would be weird to have grass in the caves.
            if (_tilePosition.Y > 11* _tileHeight)
            {
                _rngFoliage = Globals.RNG.Next(3) == 0;
            }
            if (_rngFoliage)
            {
                _tint = _tint * .4f;
                int temp = Globals.RNG.Next(3);
                if (temp == 0)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
                else if (temp == 1)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex2 % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex2 / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
                else if (temp == 2)
                    _srcRect = new Rectangle(
                      (Globals.FoliageTileIndex3 % _tilesetColumns) * _tileWidth,
                      (Globals.FoliageTileIndex3 / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
            }
            else
            { 
                _srcRect = new Rectangle(
                      (Globals.EmptyTileIndex % _tilesetColumns) * _tileWidth,
                      (Globals.EmptyTileIndex / _tilesetColumns) * _tileHeight,
                      _tileWidth,
                      _tileHeight);
            }
        }
        public override void OnInteract()
        {
        }
        public void Die()
        {
            _state = ObjectState.Dead;
            _srcRect = new Rectangle(
                 (Globals.EmptyTileIndex% _tilesetColumns) * _tileWidth,
                 (Globals.EmptyTileIndex/ _tilesetColumns) * _tileHeight,
                 _tileWidth,
                 _tileHeight);
        }
    }

    /// <summary>
    /// OccupiedTile is simply a tile that spawns under a Building, so that the player
    /// cannot build another building on top of it.
    /// </summary>
    class OccupiedTile : EmptyTile
    {
        public OccupiedTile(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _impassable = false;
        }
        public override void OnInteract()
        {

        }
    }
    #endregion
}
