using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keeno
{
    /// <summary>
    /// Class made to help with making both the Keeno and the player inherit
    /// their walk animation and relevant logic shared without making the Keeno
    /// an odd child of the player
    /// </summary>
    class AnimatedKeeno2D : Animated2D
    {
        protected Map _map;
        protected readonly List<WorldObject> _worldObjects;


        protected Vector2 _direction;
        public Vector2 Direction { get { return _direction; } }
        public Vector2 Position { get { return new(_position.X + _rect.Width / 2, _position.Y + _rect.Height / 2); } }
        protected Vector2 _previousPosition;
        protected Vector2 _startingPosition;


        protected bool _isWalking { get { return _velocity.Length() > 0; } }
        public bool IsWalking { get { return _isWalking; }}

        protected bool _wasWalking;
        protected bool _facingRight;
        protected bool _drawBounds;
        protected bool _isSelected;

        protected float _moveSpeed;
        protected float _workSpeed;

        protected int _defaultFps;
        protected int _idleFPS;
        protected int _playerLocationOffsetX;
        protected int _playerLocationOffsetY;

        public Rectangle TargetDestinationBounds
        {
            get { return new(_rect.X + _rect.Width / 4 + (int)_direction.X,
                            _rect.Y + _rect.Height / 4 + (int)_direction.Y,
                             _rect.Width / 2,
                             _rect.Height / 2);
            }
        }

        public Rectangle Bounds { get { return _rect; } }

        protected Color _tint;
        protected Color _defaultTint;

        // test related
        protected Texture2D _testPixel;

        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel, Map map)
            : base(spriteSheet, fps, rect)
        {
            _startingPosition = new Vector2(rect.X+_rect.Width/2, rect.Y);
            //_rect = new Rectangle(_rect.X + _rect.Width / 2, _rect.Y + _rect.Height / 2, _rect.Width / 2, _rect.Height / 2);
            _idleFPS = 1;

            _srcRect = new Rectangle(32, 0, rect.Width, rect.Height);
            _defaultFps = fps;

            // test related
            _testPixel = pixel;
            _drawBounds = false;
            //_targetDestinationBounds = _rect;
            _tint = Color.White;
            _defaultTint = _tint;

            if (map != null)
            {
                _map = map;
                _worldObjects = _map.WorldObjects;
            }

            // Offset where you sit in relation to the player's location
            // when following the player
            _playerLocationOffsetX += Globals.RNG.Next(-20, 21);
            _playerLocationOffsetY += Globals.RNG.Next(-20, 21);

            // Centre the position to the sprite's centre, as it's not used
            // to Draw the sprite like it could be in the parent classes
            _position = new Vector2(_rect.X + _rect.Width / 2, _rect.Y + _rect.Height / 2);

            // Set the previous position to be the same as the current one.
            // This allows to later randomise what direction the Keeno
            // face when they first spawn
            _previousPosition = _position;
        }
        public override void Update(GameTime gt)
        {
            AnimateKeeno(gt);
            _position += _velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            _rect.Location = _position.ToPoint();

            _isSelected = false;
        }
        private void AnimateKeeno(GameTime gt)
        {
            _updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * _framesPerSecond;

            #region Walking/Idle Animations
            // Detect transitions between walking and idle
            if (_isWalking != _wasWalking)  // if they don't match up it means it's transitioning
            {
                _updateTrigger = 0;

                if (_isWalking)
                {
                    // Reset to start of walking frames
                    _srcRect.X = _srcRect.Width * 2;
                }
                else
                {
                    // Reset to start of idle frames
                    _srcRect.X = _srcRect.Width;
                }
            }

            if (_isWalking)
            {
                _framesPerSecond = _defaultFps; // switch FPS to the deault

                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;

                    // Loop walking frames (3 and 4)
                    if (_srcRect.X >= _srcRect.Width * 4)
                        _srcRect.X = _srcRect.Width * 2;
                }
            }
            else
            {
                _framesPerSecond = _idleFPS;    // switch FPS to _idleFPS (slower animation)

                if (_updateTrigger >= 1)
                {
                    _updateTrigger = 0;
                    _srcRect.X += _srcRect.Width;

                    // Loop idle frames (2 and 3)
                    if (_srcRect.X >= _srcRect.Width * 3)
                        _srcRect.X = _srcRect.Width;
                }
            }
            #endregion
            #region Adjust Facing
            //if moving towards the right
            if (_position.X > _previousPosition.X)
            {
                _facingRight = true;
            }
            // else if moving towards the left
            else if (_position.X < _previousPosition.X)
            {
                _facingRight = false;
            }
            #endregion
            // Update for next frame
            _wasWalking = _isWalking;
            // Track if the player is moving to the right
            // (used to flip the sprite accordingly)
            _previousPosition = _position;
        }
        public virtual void MoveInDirection(Vector2 direction)
        {
            if (direction != Vector2.Zero)
            {
                // normalize direction to prevent diagonal movement from being faster
                direction.Normalize();

                _velocity = direction * _moveSpeed;
            }
            else
            {
                _velocity = Vector2.Zero;
            }
        }
        public virtual void MoveTo(Point destination)
        {
            Vector2 vectorDistance = destination.ToVector2() - _position;
            if (vectorDistance.Length() > .5f)
            {
                vectorDistance.Normalize();
                _velocity = vectorDistance * _moveSpeed;
            }
            else
            {
                _position = destination.ToVector2();
                _velocity = Vector2.Zero;
            }
        }
        public virtual float DistanceTo(Vector2 destination)
        {
            return (destination - _position).Length();
        }
        public virtual float GetWorkSpeed()
        {
            return _workSpeed;
        }
        public override void Draw(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            // Draw test pixel
            if (_drawBounds)
                sb.Draw(_testPixel, Bounds, Color.Blue * 1f);


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw actual sprite
            if (_isSelected)
                _tint = Color.White;
            else
                _tint = _defaultTint;
            sb.Draw(_txr, _rect, _srcRect, _tint, 0f,
                    Vector2.Zero, flip, Globals.KeenoLD);
        }
    }
    public enum KeenoState
    {
        StartScreen,
        Idle,
        ReadyToBuild,
        Following,
        Working,
        DroppingOff,
        DroppingOffAndIdle,
        DeliveringMaterials,
        WalkingToIdleSpot,
        WalkingToBuilderCabin,
        Building,
        BelRing,
        NewDay,
        Dying,
        Dead
    }

    class Keeno : AnimatedKeeno2D
    {
        private SoundEffectInstance _workInst, _dropOffInst, _constructingInst;
        private List<WorldObject> _dropOffPoints;
        private Point _closestDropOffPoint;

        private List<WorldObject> _buildingsAwaitingResources;
        private List<WorldObject> _buildingUnderConstruction;
        private Building _buildingImDeliveringTo;
        private Point _closestBuildingAwaitingResources;
        private Point _closestBuildingUnderConstruction;
        private Point _placeOfWork;


        private ResourceType _resourceType;
        private int _resourceAmmount;
        private Texture2D _resourceTxr;

        private float _normalMoveSpeed;
        private float _carryingMovementSpeed;
        private float _moveTimer;
        private float _idleTimer;
        private float _moveTimerReset;
        private bool _isCarryingResource;
        private KeenoState _state;
        public KeenoState State { get{  return _state; } }

        private Rectangle _itemCarrySpot { get { return new Rectangle(_rect.X, _rect.Y-_rect.Height/4, _rect.Width, _rect.Height); } }

        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel, Map map, bool isInStartScreen)
            : base (spriteSheet, fps, rect, pixel, map)
        {
            if (isInStartScreen)
                _state = KeenoState.StartScreen;
            else
                _state = KeenoState.Idle;
            _moveTimer = .5f+(float)Globals.RNG.Next(4);
            _idleTimer = .5f+(float)Globals.RNG.Next(4);
            _moveTimerReset = _moveTimer;
            _drawBounds = false;
            _isCarryingResource = false;

            _defaultTint =_tint = new Color(Globals.RNG.Next(25, 226),
                Globals.RNG.Next(25, 226), Globals.RNG.Next(25, 226));

            _moveSpeed = _normalMoveSpeed = Globals.RNG.Next(20, 26) + (float)Globals.RNG.NextDouble();
            //_moveSpeed = _normalMoveSpeed = Globals.RNG.Next(40, 56) + (float)Globals.RNG.NextDouble();

            _carryingMovementSpeed = 10f;

            //_carryingMovementSpeed = _moveSpeed;

            _facingRight = Globals.RNG.Next(2) == 0;

            _workSpeed = 1f;

            _dropOffPoints = new List<WorldObject>();
            _closestDropOffPoint = Point.Zero;

            _buildingUnderConstruction = new List<WorldObject>();
            _closestBuildingUnderConstruction = Point.Zero;


            _buildingsAwaitingResources = new List<WorldObject>();
            _closestBuildingAwaitingResources = Point.Zero;

            _constructingInst = Assets.ConstructingSFX.CreateInstance();
            _constructingInst.Volume = .2f;
            _resourceAmmount = 1;
        }
        public override void Update(GameTime gt)
        {
            switch (_state)
            {
                case KeenoState.StartScreen:
                    MoveInDirection(IdleAndMove());
                    break;
                case KeenoState.Idle:
                    DontCarryResource();
                    break;

                    case KeenoState.DeliveringMaterials:
                    CarryResource();
                    // go to the construction site
                    MoveTo(_closestBuildingAwaitingResources);
                        // if you have arrived there
                        if (_position == _closestBuildingAwaitingResources.ToVector2())
                        {
                            var sound = Assets.ResourceDeliveredSFX;
                            var soundInst = sound.CreateInstance();
                            soundInst.Volume = .8f;
                            soundInst.Play();

                            _buildingImDeliveringTo.TakeThisResource(_resourceType);
                            _state = KeenoState.WalkingToBuilderCabin;
                            _isCarryingResource = false;
                        }
                        break;

                case KeenoState.ReadyToBuild:
                    DontCarryResource();

                    //if there's a building that is ready to construct
                    if(ScanForBuildingsUnderConstruction())
                        break;

                    ScanForBuildingsAwaitingResources();
                    if (_isCarryingResource)
                    {
                        _state = KeenoState.DeliveringMaterials;
                    }
                    break;

                case KeenoState.Building:
                    DontCarryResource();
                    if (!_isWalking)
                        PlayConstructingSound();
                    break;

                case KeenoState.Following:
                    DontCarryResource();
                    break;

                case KeenoState.Working:
                    if(!_isWalking)
                        PlayWorkSound();
                    DontCarryResource();
                    break;

                case KeenoState.DroppingOff:
                    StopWorkSound();
                    CarryResource();
                    MoveTo(_closestDropOffPoint);
                    if (_position == _closestDropOffPoint.ToVector2())
                    {
                        FindClosestDropOffPoint();
                        if (_position == _closestDropOffPoint.ToVector2())
                        {
                            ResourceTracker.Add(_resourceType, _resourceAmmount);
                            _state = KeenoState.Working;
                        }
                        else
                            MoveTo(_closestDropOffPoint);
                    }
                    break;

                case KeenoState.DroppingOffAndIdle:
                    StopWorkSound();
                    CarryResource();
                    FindClosestDropOffPoint();
                    MoveTo(_closestDropOffPoint);
                    if (_position == _closestDropOffPoint.ToVector2())
                    {
                        FindClosestDropOffPoint();
                        if(_position == _closestDropOffPoint.ToVector2())
                        {
                            ResourceTracker.Add(_resourceType, _resourceAmmount);
                            SwitchToIdle();
                        }
                        else
                            MoveTo(_closestDropOffPoint);
                    }
                    break;

                    case KeenoState.WalkingToIdleSpot:
                    DontCarryResource();
                    StopConstructingSound();
                    StopWorkSound();
                    FindClosestDropOffPoint();
                    MoveTo(_closestDropOffPoint);
                    if (_position == _closestDropOffPoint.ToVector2())
                    {
                        _state = KeenoState.Idle;
                    }
                    break;

                    case KeenoState.WalkingToBuilderCabin:
                    DontCarryResource();
                    StopConstructingSound();
                    if (ScanForBuildingsUnderConstruction())
                        break;

                    WalkToBuilderCabin();
                    if (_position == _placeOfWork.ToVector2())
                        _state = KeenoState.ReadyToBuild;
                    break;

                case KeenoState.BelRing:
                    DoBellRing();
                    break;

                case KeenoState.NewDay:
                    DoNewDay();
                    break;

                case KeenoState.Dead:
                    break;
            }
            base.Update(gt);

        }
        public void NewDay()
        {
            if (_state == KeenoState.DeliveringMaterials)
            {
                _buildingImDeliveringTo.DontTakeThisResource(_resourceType);
            }
            _state = KeenoState.NewDay;
        }
        public void DoNewDay()
        {
            FindClosestDropOffPoint();
            _position = _closestDropOffPoint.ToVector2();

            if (_isCarryingResource)
            {
                _state = KeenoState.DroppingOffAndIdle;

            }
            else
            {
                _state = KeenoState.WalkingToIdleSpot;
            }
        }
        public void PlayerRangBell()
        {
            if (_state == KeenoState.DeliveringMaterials)
            {
                _buildingImDeliveringTo.DontTakeThisResource(_resourceType);
            }
            if (_state != KeenoState.Following)
                _state = KeenoState.BelRing;
        }
        public void DoBellRing()
        {
            if (_isCarryingResource)
            {
                DropOffAndIdle(_resourceType, _resourceAmmount);
            }
            else
            {
                _state = KeenoState.WalkingToIdleSpot;
            }
            
        }
        public void Die()
        {
            _state = KeenoState.Dead;
        }
        public void PlayWorkSound()
        {
            if (_workInst == null)
                return;
            _workInst.Play();
        }
        public void StopWorkSound()
        {
            if (_workInst == null)
                return;
            _workInst.Stop();
        }
        public void PlayConstructingSound()
        {
            if (_constructingInst == null)
                return;
            _constructingInst.Play(); 
        }
        public void StopConstructingSound()
        {
            if (_constructingInst == null)
                return;
            _constructingInst.Stop();
        }
        public void TakeWorkSoundEffect(SoundEffect workSound)
        {
            if (workSound==null)
                return;
            _workInst = workSound.CreateInstance();
            _workInst.Volume = .3f;
        }
        private void CarryResource()
        {
            if(_resourceType == ResourceType.None)
                return;
            WalkSlow();
            _isCarryingResource = true;
        }
        private void DontCarryResource()
        {
            _isCarryingResource = false;
            WalkNormal();
        }
        private void WalkNormal()
        {
            _moveSpeed = _normalMoveSpeed;
        }
        private void WalkSlow()
        {
            _moveSpeed = _carryingMovementSpeed;
        }
        public bool ScanForBuildingsUnderConstruction()
        {
            _buildingUnderConstruction.Clear();
            // Loop through the list of worldObjects
            foreach (var worldObject in _worldObjects)
            {
                // find the buildings under construction
                if (worldObject is Building building)
                    if (building.State == ObjectState.UnderConstruction)
                        _buildingUnderConstruction.Add(building);
            }
            // if the list is populated (the worker notices that there's a building
            // waiting to be constructed)
            if (_buildingUnderConstruction.Count > 0)
            {
                // Sort the list
                var sortedBuildingUnderConstruction = _buildingUnderConstruction.OrderBy(x => x.DistanceTo(Position)).ToList();

                // Find the closest one
                var targetBuilding = sortedBuildingUnderConstruction[0] as Building;
                // go to the closest one
                targetBuilding.CanDropOffWorker(this);
                return true;
            }
            return false;
        }
        public void ScanForBuildingsAwaitingResources()
        {
            _buildingsAwaitingResources.Clear();
            // Loop through the list of worldObjects
            foreach (var worldObject in _worldObjects)
            {
                // find the buildings awaiting resources
                if (worldObject is Building building)
                    if(building.State == ObjectState.AwaitingResourceDelivery)
                        _buildingsAwaitingResources.Add(building);
            }
            if(_buildingsAwaitingResources.Count > 0)
            {
                // Sort the list
                var sortedBuildingAwaitingResources = _buildingsAwaitingResources.OrderBy(x => x.DistanceTo(Position)).ToList();

                // Check if you can deliver any resources to any buildings awaiting resources
                // checking from closest to furthest
                for (int i = 0; i < sortedBuildingAwaitingResources.Count; i++)
                {
                    var closest = sortedBuildingAwaitingResources[i] as Building;
                    // Check what resources you can afford to bring to the building awaiting resources
                    ResourceType type = closest.CheckCosts();
                    // If you can afford to bring resources
                    // And the site still needs it
                    if (type != ResourceType.None)
                    {
                        PickUpOneResource(type);

                        _closestBuildingAwaitingResources = new Vector2(
                            sortedBuildingAwaitingResources[i].Position.X - Globals.Tile_Width_Height / 2,
                            sortedBuildingAwaitingResources[i].Position.Y - Globals.Tile_Width_Height / 2).ToPoint();
                        // break the loop, don't check any other 
                        // Construction sites.
                        _buildingImDeliveringTo = sortedBuildingAwaitingResources[i] as Building;
                        break;
                    }
                }
            }
        }
        public void RememberThisBuilderCabin(Point builderCabin)
        {
            builderCabin = new Point(builderCabin.X - Globals.Tile_Width_Height / 2,
                builderCabin.Y - Globals.Tile_Width_Height / 2);

            _placeOfWork = builderCabin;
        }
        public void SwitchToWalkingToBuilderCabin()
        {
            _state = KeenoState.WalkingToBuilderCabin;
        }
        public void SwitchToBuilding()
        {
            _state = KeenoState.Building;
        }
        public void WalkToBuilderCabin()
        {
            MoveTo(_placeOfWork);
        }
        public void PickUpOneResource(ResourceType type)
        {
            ResourceTracker.Spend(type, 1);
            _isCarryingResource = true;
            _resourceType = type;
        }
        public virtual void FollowPlayer(Point destination)
        {
            destination.X += _playerLocationOffsetX;
            destination.Y += _playerLocationOffsetY;
            MoveTo(destination);
        }
        public Vector2 IdleAndMove()
        {
            float deltaTime = Globals.DeltaTime;

            if (_moveTimer > 0)
            {
                // Movement phase
                _moveTimer -= deltaTime;
                return _direction;
            }
            else if (_idleTimer > 0)
            {
                // Idle phase
                _idleTimer -= deltaTime;
                return Vector2.Zero;
            }
            else
            {
                // Transition: reset timers and pick a new direction
                RollRandomDirection();

                _moveTimer = _moveTimerReset;
                _idleTimer = _moveTimerReset;

                return _direction;
            }
        }
        public void RollRandomDirection()
        {
            _direction = new Vector2(Globals.RNG.Next(-1, 2), Globals.RNG.Next(-1, 2));
        }
        public void SwitchToFollowing()
        {
            var temp = Assets.PlayerAddingFollowerSFX.CreateInstance();
            temp.Play();
            _state = KeenoState.Following;
        }
        public void SwitchToWorking()
        {
            _state = KeenoState.Working;
        }
        public void SwitchToReadyToBuild()
        {
            _state = KeenoState.ReadyToBuild;
        }
        public void SwitchToIdle()
        {
            _state = KeenoState.Idle;
        }
        public void SwitchWalkingToIdleSpot()
        {
            _state = KeenoState.WalkingToIdleSpot;
        }
        public void DropOffResources(ResourceType type, int amount)
        {
            FindClosestDropOffPoint();
            _state = KeenoState.DroppingOff;
            _resourceType = type;
            _resourceAmmount = amount;
        }
        public void DropOffAndIdle(ResourceType type, int amount)
        {
            FindClosestDropOffPoint();
            _state = KeenoState.DroppingOffAndIdle;
            _resourceType = type;
            _resourceAmmount = amount;
        }
        private void FindClosestDropOffPoint()
        {
            _dropOffPoints.Clear();
            // Loop through the list of worldObjects
            foreach (var worldObject in _worldObjects)
            {
                // find the drop off points and add them to the list
                if (worldObject is IDropOffPoint && worldObject.GetDropOffPointState())
                    _dropOffPoints.Add(worldObject);
            }
            // Sort the list
            var sortedDropOffPointList = _dropOffPoints.OrderBy(x => x.DistanceTo(Position)).ToList();
            //Find the closest DropOffPoint
            _closestDropOffPoint = sortedDropOffPointList[0].Position.ToPoint();
            _closestDropOffPoint = new Vector2(
                sortedDropOffPointList[0].Position.X-Globals.Tile_Width_Height/2, 
                sortedDropOffPointList[0].Position.Y - Globals.Tile_Width_Height / 2).ToPoint();
        }
        public void Selected()
        {
            _isSelected = true;
        }
        public override void Draw(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;

            // Draw test pixel
            if (_drawBounds)
                sb.Draw(_testPixel, Bounds, Color.Blue * 1f);


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw actual sprite
            if (_isSelected)
                _tint = Color.White;
            else if (_state ==  KeenoState.Idle && !_isSelected)
                _tint = Color.Gray;
            else
                _tint = _defaultTint;
            //sb.Draw(_txr, _rect, _srcRect, _tint, 0f,
            //        Vector2.Zero, flip, Globals.KeenoLD);
            if(_isCarryingResource)
                _txr = Assets.KeenoCarryingTxr;
            else
                _txr = Assets.KeenoTxr;
            if(_state != KeenoState.StartScreen)
                sb.Draw(_txr, new Vector2(_rect.X,_rect.Y), _srcRect, _tint, 0f, Vector2.Zero, .9f, flip, Globals.KeenoLD);
            else
                sb.Draw(_txr, new Vector2(_rect.X, _rect.Y), _srcRect, _tint, 0f, Vector2.Zero, 2f, flip, Globals.MapLD);




            if (_isCarryingResource)
            {
                switch (_resourceType)
                {
                    case ResourceType.Wood:
                        _resourceTxr = Assets.UIWoodTxr;
                        break;
                        case ResourceType.Stone:
                        _resourceTxr = Assets.UIStoneTxr;
                        break;
                        case ResourceType.Food:
                        _resourceTxr = Assets.UIFoodTxr;
                        break;
                }

                sb.Draw(_resourceTxr, _itemCarrySpot, null, Color.White, 0f,
                    Vector2.Zero, flip, Globals.ResourceBeingCarriedTxrLD);
            }
        }
    }
}
