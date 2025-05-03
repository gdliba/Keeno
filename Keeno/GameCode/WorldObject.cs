using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Keeno
{
    enum ObjectState
    {
        Default,
        NotHarvestable,
        Dead
    }
    class WorldObject
    {
        public ObjectState State { get { return _state; } protected set { _state = value; } }
        protected ObjectState _state;
        protected ResourceType _resourceType;

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

        protected Rectangle _rect;
        protected Rectangle? _srcRect;
        protected Rectangle? _selectedTileSrcRect;

        protected Point _tilePosition;

        //protected float _workSpeed;
        //protected float _workDuration;

        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;

        protected int _health;
        //protected int _workerSlots;
        //protected int _resourceAmount;

        //protected bool _resourceHarvested;
        protected bool _isSelected;
        protected bool _canDropOff;
        protected bool _canUse;
        protected bool _cannotUse;
        protected bool _destroyMe;
        protected bool _impassable;
        public bool Impassable { get { return _impassable;} protected set { _impassable = value; } }

        protected float _destroySpeed;

        public Color Tint;
        public Rectangle Bounds { get{ return _rect; } protected set { _rect = value; } }
        public Vector2 Position { get { return new Vector2(_rect.X + _tileWidth / 2, _rect.Y + _tileHeight / 2); } }


        protected WorldObject(Point tilePosition,
            Rectangle sourceRect)
        {
            _state = ObjectState.Default;
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;

            _destroySpeed = .01f;


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
            _srcRect = sourceRect;
            Tint = Color.White;


            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);


            _health = 1;

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
                _tilePosition.Y + _tileHeight,
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
                _tileWidth,
                _tileHeight),
                Color.Yellow);

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
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
        public virtual void Update(GameTime gt)
        {
            if (!_isSelected)
            {
                _HGInteract.Reset();
                _HGDropOff.Reset();
                _HGDestroy.Reset();
                _HGCantInteract.Reset();
            }
            _isSelected = false;
        }
        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public virtual void OnInteract()
        {

        }
        
        public virtual void Selected(float playerWorkSpeed)
        {
            _isSelected = true;
        }
        public virtual void DestroyMe()
        {
            //ClearWorkerList();
            _state = ObjectState.Dead;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Black);  // Draw Position


            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            if (_state != ObjectState.Dead)
            {
                sb.Draw(_txr, _rect, _srcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.WolrdObjectLD);
                _HGWorkProgress.Draw(sb);
            }
        }
    }
    class Item : WorldObject
    {
        protected bool _isEquipped;
        public Item(Point position, Texture2D txr)
            : base(position,
                // sourceRect of the selectedTxr inside the tileset
                new Rectangle(
                  (Globals.ItemSelectedIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ItemSelectedIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
                  )
        {
            _isEquipped = false;
            _txr = txr;

            //_tilePosition.X = position.X / _tileWidth;
            //_tilePosition.Y = position.Y / _tileHeight;
            _rect = new Rectangle(position.X, position.Y, _tileWidth, _tileHeight);

            _impassable = false;
            _selectedTileSrcRect = _srcRect;
        }
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
        public void FollowPlayer(Point itemCarryPoint)
        {
            _isEquipped = true;
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;
        }
        public virtual void Place(Rectangle onThisTile)
        {
            _rect = onThisTile;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);
            //sb.Draw(_txr, _rect, Color.White);
            sb.Draw(_txr, _rect, null, Color.White, 0f,Vector2.Zero,SpriteEffects.None,.1f);

        }

    }
    class Blueprint : Item
    {
        protected Texture2D _blueprintTxr;
        public Blueprint(Point position, Texture2D txr)
            : base(position, txr)
        {
            _blueprintTxr = Assets.BlueprintTxr;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Draw(_blueprintTxr, _rect, null, Color.CornflowerBlue, 0f, Vector2.Zero, SpriteEffects.None, .1f);
        }

    }
    class BuildingBlueprint : Blueprint
    {
        protected Rectangle _stage1SrcRect, _stage2SrcRect, _stage3SrcRect;
        public BuildingBlueprint(Point position, Texture2D buildingSpritesheet)
            : base(position, buildingSpritesheet)
        {
            // The BuildingSpritesheet has 3 stages of the building given
            // and is intended to be drawn as 3 sprites one on top of the other
            _stage1SrcRect = _stage2SrcRect = _stage3SrcRect = new Rectangle(0, 0, _rect.Width, _rect.Height);
            _stage2SrcRect.X = _stage1SrcRect.X + _rect.Width;
            _stage3SrcRect.X = _stage2SrcRect.X + _rect.Width;
        }
        public override void Place(Rectangle onThisTile)
        {
            base.Place(onThisTile);
            
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_txr, _rect, _stage1SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_txr, _rect, _stage2SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_txr, _rect, _stage3SrcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_blueprintTxr, _rect, null, Color.CornflowerBlue, 0f, Vector2.Zero, SpriteEffects.None, Globals.ItemTxrLD);
        }
    }

    class WorkStation : WorldObject
    {
        protected List<Keeno> _workers;

        protected float _workSpeed;
        protected float _workDuration;
        protected float _playerWorkSpeed;


        protected int _workerSlots;
        protected int _resourceAmount;

        protected bool _resourceHarvested;


        public WorkStation(Point tilePosition, int globalTileIndex)
            : base
            (tilePosition,
                // sourceRect inside the tileset
                new Rectangle(
                  (globalTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (globalTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height))
        {
            _workers = new List<Keeno>();
            _resourceType = ResourceType.None;
            _resourceAmount = 0;
            _workSpeed = 0f;
            _workerSlots = 1;
            _workDuration = 10f;

            _resourceHarvested = false;
            _canDropOff = false;


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
                _tilePosition.Y + _tileHeight,
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

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
                _tileWidth,
                _tileHeight), Color.Red);

            _HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth,
                _tileHeight),
                Color.Yellow);
            #endregion
        }
        public override void Selected(float playerWorkSpeed)
        {
            base.Selected(playerWorkSpeed);
            _playerWorkSpeed = playerWorkSpeed;
        }
        public override void Update(GameTime gt)
        {
            if (_state == ObjectState.NotHarvestable)
                ClearWorkerList();

            if (_isSelected)
            {
                _resourceHarvested = _HGInteract.Update(Globals.E_KeyDown, _playerWorkSpeed);

                if (_state == ObjectState.NotHarvestable)
                    _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, _destroySpeed);
                if (_destroyMe)
                    DestroyMe();
                // Check if player has followers
                // And if there are available workerSlots
                if (_workerSlots > 0)
                {
                    _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, Globals.DropOffKeenoSpeed);
                }
            }
            if (_state != ObjectState.NotHarvestable)
            {
                if (_resourceHarvested)
                    HarvestResource(_resourceType, _resourceAmount);
            }

            // Set selected to false;
            // Reset all HG
            base.Update(gt);

            // Work out the ammount of work that needs to be put in
            // to complete the work
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
            float deltaFill = _workSpeed * (deltaTime / _workDuration);

            if (_state == ObjectState.Default)
                _resourceHarvested = _HGWorkProgress.Update(true, deltaFill);

            if (_health == 0)
            {
                _health--;
                _state = ObjectState.NotHarvestable;
            }

            foreach (Keeno keeno in _workers)
            {
                keeno.MoveTo(_tilePosition);
            }
            if (_resourceHarvested)
                HarvestResource(_resourceType, _resourceAmount);
        }
        #region Resources/Workers
        public virtual void HarvestResource(ResourceType type, int amount)
        {
            _health--;
            ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
            _HGInteract.Reset();

        }
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
            worker.SwitchToWorking();
            ReduceWorkerSlots();

            // Get the worker's workspeed and apply it to the WorldObject
            float kWorkspeed = worker.GetWorkSpeed();
            _workSpeed += kWorkspeed;
        }
        public void ClearWorkerList()
        {
            foreach (var keeno in _workers)
            {
                keeno.SwitchToIdle();
            }
            _workers.Clear();
        }
        public virtual bool CanDropOffWorker(Keeno worker)
        {
            if (_canDropOff && _workerSlots > 0)
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
        public virtual void ChangeTextureToNotHarvestable()
        {
      
        }
        public override void Draw(SpriteBatch sb)
        {
            switch (_state)
            {
                case ObjectState.Dead:
                    return;
                case ObjectState.Default:
                    if (_isSelected)
                    {
                        // HourGlasses
                        _HGInteract.Draw(sb);
                        _HGDropOff.Draw(sb);
                        // Input Promts
                        _buttonPrompt_E.Draw(sb);
                        _buttonPrompt_Q.Draw(sb);
                    }
                    break;
                case ObjectState.NotHarvestable:
                    ChangeTextureToNotHarvestable();
                    if (_isSelected)
                    {
                        _buttonPrompt_X.Draw(sb);
                        _HGDestroy.Draw(sb);
                    }
                    break;
            }
            //sb.Draw(_txr, _rect, _farmLandSrc, Color.White);
            base.Draw(sb);
        }
    }


    class Tree : WorkStation
    {
        private Texture2D _choppedTreeTxr;

        public Tree(Point tilePosition, int globalTileIndex)
            : base(tilePosition, globalTileIndex)
        {
            _resourceAmount = Globals.TreeWoodAmount;
            _health = Globals.TreeHealth;
            _workerSlots = 3;
            _resourceType = ResourceType.Wood;

            _choppedTreeTxr = Assets.ChoppedTreeTxr;
            _impassable = true;
        }
        public override void ChangeTextureToNotHarvestable()
        {
            _srcRect = null;
            _txr = _choppedTreeTxr;
        }
    }
    class Farm : WorkStation
    {
        private Rectangle _farmLandSrc;
        public Farm(Point tilePosition, int globalTileIndex) 
            : base(tilePosition, globalTileIndex)
        {
            _state = ObjectState.Default;
            _resourceType = ResourceType.Food;
            _workerSlots = 1;
            _destroySpeed = .01f;
            _resourceAmount = Globals.FarmFoodAmount;
            _txr = Assets.TilesetTxr;
            _health = Globals.FarmHealth;
            _resourceHarvested = false;
            _canDropOff = false;
            _impassable = false;


            _tileHeight = _tileWidth = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * Globals.Tile_Width_Height;
            _tilePosition.Y = tilePosition.Y * Globals.Tile_Width_Height;
            _tilesetColumns = Globals.TilemapColumns;

            #region ButtonPrompts and HG
            _farmLandSrc= new Rectangle(
                  (Globals.FarmLandTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.FarmLandTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
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
                _tilePosition.Y + _tileHeight,
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

            _HGDestroy = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y + _tileHeight,
                _tileWidth,
                _tileHeight), Color.Red);

            _HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth,
                _tileHeight),
                Color.Yellow);
            #endregion
        }
        public override void ChangeTextureToNotHarvestable()
        {

            _srcRect = new Rectangle(
                  (Globals.HarvestedFarmTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedFarmTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
        }
    }
    class TownCentre : WorldObject
    {
        private List<Keeno> _keenosISpawned;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenosISpwaned { get { return _keenosISpawned; } }

        public TownCentre(Point tilePosition) 
            : base(tilePosition,
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.TownCentreTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.TownCentreTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
              )
        {
            _keenosISpawned = new List<Keeno>();
        }
        public override void Update(GameTime gt)
        {

            if (_isSelected)
            {
                if (ResourceTracker.CanSpend(ResourceType.Food,
                ResourceTracker.KeenoCost))
                    _canUse = _HGInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
                else
                    _canUse = _HGCantInteract.Update(Globals.E_KeyDown, Globals.NeutralInteractSpeed);
            }
            base.Update(gt);
        }
        public override void OnInteract()
        {
            if (_canUse)
            {
                if (ResourceTracker.TrySpend(ResourceType.Food,
                ResourceTracker.KeenoCost))
                {
                    SpawnKeeno();
                }

                _HGInteract.Reset();
                _HGCantInteract.Reset();
            }
        }
        private void SpawnKeeno()
        {
            var newKeeno = new Keeno(Assets.KeenoTxr, 5, new Rectangle(_tilePosition.X, _tilePosition.Y, 16, 16), Assets.DebugPixelTxr);
            _keenosISpawned.Add(newKeeno);
            //Debug.WriteLine("Spawning Keeno: firing event");
            KeenoSpawned?.Invoke(newKeeno);
        }
        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);

            base.Draw(sb);
            if (_isSelected)
            {
                _HGCantInteract.Draw(sb);
                _HGInteract.Draw(sb);
                _buttonPrompt_E.Draw(sb);
            }
        }
    }
    class EmptyTile : WorldObject
    {

        public EmptyTile(Point tilePosition) 
            : base(tilePosition,
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.EmptyTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.EmptyTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
              )
        {
            _impassable = false;
        }
        public override void OnInteract()
        {

        }
    }
}
