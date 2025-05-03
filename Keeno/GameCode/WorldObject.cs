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



        protected List<Keeno> _workers;

        protected Texture2D _tilesetTxr;
        protected Texture2D _selectedTileTileset;
        protected Texture2D _testPixel;
        protected Texture2D _txr;

        protected Rectangle _rect;
        protected Rectangle _srcRect;
        protected Rectangle _selectedTileSrcRect;

        protected Point _tilePosition;

        protected float _workSpeed;
        protected float _workDuration;

        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesetColumns;

        protected int _health;
        protected int _workerSlots;
        protected int _resourceAmount;

        protected bool _canHarvestResource;
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


        protected WorldObject(Rectangle bounds,
            Rectangle sourceRect)
        {
            _state = ObjectState.Default;
            _testPixel = Assets.DebugPixelTxr; 
            _impassable = true;
            _isSelected = false;
            _canDropOff = false;
            _destroyMe = false;
            _canUse = false;
            _canHarvestResource = false;

            _selectedTileTileset = Assets.MonochromaticTilesetTxr;
            _tilesetTxr = Assets.TilesetTxr;
            _rect = bounds;
            _srcRect = sourceRect;
            Tint = Color.White;

            _tilesetColumns = Globals.TilemapColumns;
            _tileWidth = _tileHeight = Globals.Tile_Width_Height;

            _selectedTileSrcRect = 
                new Rectangle   (Globals.TileSelectedIndex % _tilesetColumns * _tileWidth,
                                (Globals.TileSelectedIndex / _tilesetColumns) * _tileHeight,
                                _tileWidth, _tileHeight);

            _workers = new List<Keeno>();
            _workSpeed = 0f;
            _workerSlots = 3;
            _workDuration = 10f;
            _health = 1;
            _resourceType = ResourceType.None;
            _resourceAmount = 0;

            _HGWorkProgress = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X,
                _tilePosition.Y,
                _tileWidth,
                _tileHeight),
                Color.Yellow);
        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - Position).Length();
        }
        public virtual void Update(GameTime gt)
        {
            _isSelected = false;

            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
            float deltaFill = _workSpeed * (deltaTime / _workDuration);

            if (_state == ObjectState.Default)
                _canHarvestResource = _HGWorkProgress.Update(true, deltaFill);

            if(_health == 0)
            {
                _state = ObjectState.NotHarvestable;
                _health--;
            }

            foreach (Keeno keeno in _workers)
            {
                keeno.MoveTo(_tilePosition);
            }
            if (_canHarvestResource)
                HarvestResource(_resourceType, _resourceAmount);
        }
        public virtual void HarvestResource(ResourceType type, int amount)
        {
            _health--;
            ResourceTracker.Add(type, amount);
            _HGWorkProgress.Reset();
        }

        /// <summary>
        /// Called when the player “interacts” with this object
        /// </summary>
        public virtual void OnInteract()
        {

        }
        
        public virtual void Selected(bool IsConditionMet,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            _isSelected = true;
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
        public void DestroyMe()
        {
            ClearWorkerList();
            _state = ObjectState.Dead;
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
        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Black);  // Draw Position


            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            sb.Draw(_tilesetTxr, _rect, _srcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.WolrdObjectLD);

            if (_state == ObjectState.Default) 
                _HGWorkProgress.Draw(sb);


        }
    }
    class Item : WorldObject
    {
        public Item(Point position, Texture2D txr)
            : base(
                  new Rectangle(position.X, 
                              position.Y,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height),
                // sourceRect of the selectedTxr inside the tileset
                new Rectangle(
                  (Globals.ItemSelectedIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.ItemSelectedIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
                  )
        {
            _txr = txr;
            _tilePosition = position;
            _impassable = false;
            _selectedTileSrcRect = _srcRect;
            //_srcRect = new Rectangle(0,0,16,16);
        }
        public void Selected(bool IsConditionMet)
        {
            if(IsConditionMet)
                _isSelected = true;
        }
        public void OnInteract(Point itemCarryPoint)
        {
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;

        }
        public void FollowPlayer(Point itemCarryPoint)
        {
            _rect.X = itemCarryPoint.X;
            _rect.Y = itemCarryPoint.Y;
        }
        public void Place(Rectangle onThisTile)
        {
            _rect = onThisTile;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (_isSelected)
                sb.Draw(_selectedTileTileset, _rect, _selectedTileSrcRect, Tint, 0, Vector2.Zero, SpriteEffects.None, Globals.SelectedTxrLD);
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);
            sb.Draw(_txr, _rect, Color.White);
            sb.Draw(_txr, _rect, null, Color.White, 0f,Vector2.Zero,SpriteEffects.None,.1f);

        }

    }
    //class BluePrint : Item()
    //{

    //}






    //class WorkStation : WorldObject
    //{
    //    protected List<Keeno> _workers;
    //    protected int _workerSlots;

    //    public WorkStation(
    //        Texture2D texture,
    //        Rectangle bounds,
    //        Rectangle sourceRect,
    //        int tilesetColumns,
    //        int tileWidth,
    //        int tileHeight,
    //        Texture2D testPixel
    //    ) : base(texture, bounds, sourceRect, tilesetColumns, tileWidth, tileHeight, testPixel)
    //    {
    //        _workers = new List<Keeno>();
    //        _workerSlots = 5;
    //        _health = 5;
    //    }

    //    public override void OnInteract()
    //    {
    //        Console.WriteLine("WorkStation interacted with!");
    //    }
    //    public virtual void ReduceWorkerSlots()
    //    {
    //        if (_workerSlots > 0)
    //            _workerSlots--;
    //    }
    //    public virtual void IncreaseWorkerSlots()
    //    {
    //        _workerSlots++;
    //    }
    //    public virtual void TakeWorker(Keeno worker)
    //    {
    //        _workers.Add(worker);
    //    }
    //}


    class Tree : WorldObject
    {
        private Texture2D _fallenTreeTxr;
        private Texture2D _choppedTreeTxr;

        private bool _canChop;


        public Tree(Point tilePosition)
            : base(
                  // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * Globals.Tile_Width_Height,
                              tilePosition.Y * Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.TreeTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.TreeTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height))
                {
            _state = ObjectState.Default;
            _resourceType = ResourceType.Wood;
            _workerSlots = 3;
            _destroySpeed = .01f;
            _resourceAmount = Globals.TreeWoodAmount;
            _tilesetTxr = Assets.TilesetTxr;
            _health = Globals.TreeHealth;
            _canChop = false;
            _canHarvestResource = false;
            _canDropOff = false;
            _impassable = true;


            _tileHeight = _tileWidth = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * _tileWidth;
            _tilePosition.Y = tilePosition.Y * _tileHeight;
            _tilesetColumns = Globals.TilemapColumns;
            _choppedTreeTxr = Assets.ChoppedTreeTxr;

            #region ButtonPrompts and HG
            _buttonPrompt_E = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X+_tileWidth/2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _buttonPrompt_Q = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X-_tileWidth/2,
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

        public override void Selected(bool playerHasFollowers,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);

            _canHarvestResource = _HGInteract.Update(Globals.E_KeyDown, playerWorkSpeed);

            if (_state == ObjectState.NotHarvestable)
                _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, _destroySpeed);
            if (_destroyMe) 
                DestroyMe();

            // Check if player has followers
            // And if there are available workerSlots
            if (playerHasFollowers && _workerSlots > 0)
            {
                _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, dropOffSpeed);
            }

        }
        public override void Update(GameTime gt)
        {
            if (_state == ObjectState.NotHarvestable)
                ClearWorkerList();
            base.Update(gt);
        }
        public override void OnInteract()
        {
            if (_state != ObjectState.NotHarvestable)
            {
                if (_canHarvestResource)
                    HarvestResource(ResourceType.Wood, _resourceAmount);
            }
        }
        public override void HarvestResource(ResourceType type, int amount)
        {
            base.HarvestResource(type, amount);
            _HGInteract.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {
            //sb.Draw(_testPixel, Bounds, Color.Red * .75f);



            if (_state != ObjectState.NotHarvestable)
            {
                base.Draw(sb);
                if (_isSelected)
                {
                    // HourGlasses
                    _HGInteract.Draw(sb);
                    _HGDropOff.Draw(sb);
                    // Input Promts
                    _buttonPrompt_E.Draw(sb);
                    _buttonPrompt_Q.Draw(sb);
                }
            }
            else if (_state == ObjectState.NotHarvestable)
            {
                sb.Draw(_choppedTreeTxr, _rect, Color.White);
                if (_isSelected)
                {
                    _buttonPrompt_X.Draw(sb);
                    _HGDestroy.Draw(sb);
                }
            }
        }
    }
    class Farm : WorldObject
    {
        private Rectangle _farmLandSrc;
        public Farm(Point tilePosition) 
            : base(
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * Globals.Tile_Width_Height,
                              tilePosition.Y * Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.FarmTileIndex1 % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.FarmTileIndex1/ Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)

              )
        {
            _state = ObjectState.Default;
            _resourceType = ResourceType.Food;
            _workerSlots = 1;
            _destroySpeed = .01f;
            _resourceAmount = Globals.FarmFoodAmount;
            _tilesetTxr = Assets.TilesetTxr;
            _health = Globals.FarmHealth;
            _canHarvestResource = false;
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
        public override void Selected(bool playerHasFollowers,
     float playerWorkSpeed,
     float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);

            _canHarvestResource = _HGInteract.Update(Globals.E_KeyDown, playerWorkSpeed);

            if (_state == ObjectState.NotHarvestable)
                _destroyMe = _HGDestroy.Update(Globals.X_KeyDown, _destroySpeed);
            if (_destroyMe)
                DestroyMe();

            // Check if player has followers
            // And if there are available workerSlots
            if (playerHasFollowers && _workerSlots > 0)
            {
                _canDropOff = _HGDropOff.Update(Globals.Q_KeyDown, dropOffSpeed);
            }

        }
        public override void Update(GameTime gt)
        {
            if (_state == ObjectState.NotHarvestable)
                ClearWorkerList();
            base.Update(gt);
        }
        public override void OnInteract()
        {
            if (_state != ObjectState.NotHarvestable)
            {
                if (_canHarvestResource)
                    HarvestResource(ResourceType.Food, _resourceAmount);
            }
        }
        public override void HarvestResource(ResourceType type, int amount)
        {
            base.HarvestResource(type, amount);
            _HGInteract.Reset();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (_state != ObjectState.NotHarvestable)
            {
                if (_isSelected)
                {
                    // HourGlasses
                    _HGInteract.Draw(sb);
                    _HGDropOff.Draw(sb);
                    // Input Promts
                    _buttonPrompt_E.Draw(sb);
                    _buttonPrompt_Q.Draw(sb);
                }
            }
            else if (_state == ObjectState.NotHarvestable)
            {
                _srcRect = new Rectangle(
                  (Globals.HarvestedFarmTileIndex % _tilesetColumns) * _tileWidth,
                  (Globals.HarvestedFarmTileIndex / _tilesetColumns) * _tileHeight,
                  _tileWidth,
                  _tileHeight);
                if (_isSelected)
                {
                    _buttonPrompt_X.Draw(sb);
                    _HGDestroy.Draw(sb);
                }
            }
            //sb.Draw(_txr, _rect, _farmLandSrc, Color.White);
            base.Draw(sb);
        }
    }
    class TownCentre : WorldObject
    {

        private Color _tint;
        private List<Keeno> _keenosISpawned;
        public event Action<Keeno> KeenoSpawned;
        public List<Keeno> KeenosISpwaned { get { return _keenosISpawned; } }

        public TownCentre(Point tilePosition) 
            : base(
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * Globals.Tile_Width_Height,
                              tilePosition.Y * Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.TownCentreTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.TownCentreTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
              )
        {
            _keenosISpawned = new List<Keeno>();
            _impassable = true;
            _canUse = false;
            _state = ObjectState.Default;
            Tint = Color.White;

            _tileHeight = _tileWidth = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * Globals.Tile_Width_Height;
            _tilePosition.Y = tilePosition.Y * Globals.Tile_Width_Height;
            _tilesetColumns = Globals.TilemapColumns;
            _tilesetTxr = Assets.TilesetTxr;


            _buttonPrompt_E = new ButtonPrompt(Assets.InputsTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Globals.InputsTilesetIndex_E);

            _HGInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Yellow);

            _HGCantInteract = new HourGlass(Assets.MonochromaticTilesetTxr,
                new Rectangle(_tilePosition.X + _tileWidth / 2,
                _tilePosition.Y - _tileHeight,
                _tileWidth,
                _tileHeight), Color.Red);
        }
        public override void Selected(bool playerHasFollowers,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);
            if (ResourceTracker.CanSpend(ResourceType.Food, 
                ResourceTracker.KeenoCost))
                _canUse = _HGInteract.Update(Globals.E_KeyDown, playerWorkSpeed);
            else
                _canUse = _HGCantInteract.Update(Globals.E_KeyDown, playerWorkSpeed);
        }
        public override void Update(GameTime gt)
        {
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
                    _HGInteract.Reset();
                }
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
            : base(
                // world‐space bounds: tilePosition * tileSize
                new Rectangle(tilePosition.X * Globals.Tile_Width_Height,
                              tilePosition.Y * Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height,
                              Globals.Tile_Width_Height),
                // sourceRect inside the tileset
                new Rectangle(
                  (Globals.EmptyTileIndex % Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  (Globals.EmptyTileIndex / Globals.TilemapColumns) * Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height,
                  Globals.Tile_Width_Height)
              )
        {
            _impassable = false;
            _canUse = false;
            _state = ObjectState.Default;
            _tilesetTxr = Assets.TilesetTxr;
            Tint = Color.White;

            _tileHeight = _tileWidth = Globals.Tile_Width_Height;
            _tilePosition.X = tilePosition.X * Globals.Tile_Width_Height;
            _tilePosition.Y = tilePosition.Y * Globals.Tile_Width_Height;
            _tilesetColumns = Globals.TilemapColumns;
        }
        public override void Selected(bool playerHasFollowers,
            float playerWorkSpeed,
            float dropOffSpeed)
        {
            base.Selected(playerHasFollowers, playerWorkSpeed, dropOffSpeed);
        }
        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        public override void OnInteract()
        {

        }
    }
}
