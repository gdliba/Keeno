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
    /// an odd child of the player with less autonomy.
    /// </summary>
    class AnimatedKeeno2D : Animated2D
    {
        #region Variables
        protected Map _map;
        protected readonly List<WorldObject> _worldObjects;

        protected Vector2 _direction;
        public Vector2 Direction { get { return _direction; } }
        public Vector2 Position { get { return new(_position.X + _rect.Width / 2, _position.Y + _rect.Height / 2); } }
        protected Vector2 _previousPosition;
        protected Vector2 _startingPosition;
        protected Vector2 _playerLocationOffset;


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
        #endregion
        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect, Map map)
            : base(spriteSheet, fps, rect)
        {
            _startingPosition = new Vector2(rect.X+_rect.Width/2, rect.Y);
            _srcRect = new Rectangle(32, 0, rect.Width, rect.Height);

            // For aesthetic/stylistic choices the Keeno have a slower idle animation
            _idleFPS = 1;
            _defaultFps = fps;

            // test related
            _drawBounds = false;

            // Remember your original colour
            _tint = Color.White;
            _defaultTint = _tint;

            if (map != null)
            {
                _map = map;
                _worldObjects = _map.WorldObjects;
            }

            // Offset where you sit in relation to the player's location
            // when following the player
            _playerLocationOffset = new Vector2(Globals.RNG.Next(-20, 21), Globals.RNG.Next(-20, 21));

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
        /// <summary>
        /// Method in charge of the Keeno's animation
        /// </summary>
        /// <param name="gt"></param>
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
        /// <summary>
        /// Takes in a Direction (Vector2) and Moves the Keeno towards it using its movement speed.
        /// </summary>
        /// <param name="direction"></param>
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
        /// <summary>
        /// Takes in a Destination (point) and moves towards it until it's close enough,
        /// then it snaps to the point.
        /// </summary>
        /// <param name="destination"></param>
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
        /// <summary>
        /// Returns the distance to the Vector2 given to it.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
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
                sb.Draw(Assets.DebugPixelTxr, Bounds, Color.Blue * 1f);


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
    /// <summary>
    /// Class that determines the behaviours of the Keeno
    /// </summary>
    class Keeno : AnimatedKeeno2D
    {
        #region Variables
        private SoundEffectInstance _workInst, _dropOffInst, _constructingInst;

        private List<WorldObject> _dropOffPoints;
        private List<WorldObject> _buildingsAwaitingResources;
        private List<WorldObject> _buildingUnderConstruction;
        private Building _buildingImDeliveringTo;
        private Point _closestDropOffPoint;
        private Point _closestBuildingAwaitingResources;
        private Point _closestBuildingUnderConstruction;
        private Point _placeOfWork;

        // The resource you are carrying.
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
        #endregion
        private Rectangle _itemCarrySpot { get { return new Rectangle(_rect.X, _rect.Y-_rect.Height/4, _rect.Width, _rect.Height); } }
        /// <summary>
        /// Keeno constructor randomises a few values to make each Keeno feel and look a little more unique:
        /// Each Keeno has a random Colour and, within a certain range, movement speed.
        /// For gamplay reasons, certain variables are set to constant values, that are not RNG dependant,
        /// like "_workspeed" and "_carryingMovementSpeed".
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="fps"></param>
        /// <param name="rect"></param>
        /// <param name="map"></param>
        /// <param name="isInStartScreen"></param>
        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Map map, bool isInStartScreen)
            : base (spriteSheet, fps, rect, map)
        {
            if (isInStartScreen)
                _state = KeenoState.StartScreen;
            else
                _state = KeenoState.Idle;

            // For debug
            _drawBounds = false;

            // Starting/Default values
            _isCarryingResource = false;
            _workSpeed = 1f;
            _carryingMovementSpeed = 10f;
            _moveSpeed = _normalMoveSpeed = Globals.RNG.Next(20, 26) + (float)Globals.RNG.NextDouble();
            // Randomly face right or left
            _facingRight = Globals.RNG.Next(2) == 0;
            // Pick a colour within the range. (Range is intentionally clamped to stray away from White and Black)
            _defaultTint =_tint = new Color(Globals.RNG.Next(25, 226), Globals.RNG.Next(25, 226), Globals.RNG.Next(25, 226));


            // Only used in the IdleAndMove methods (used in start, endofday, gameover screens)
            _moveTimer = .5f+(float)Globals.RNG.Next(4);
            _idleTimer = .5f+(float)Globals.RNG.Next(4);
            _moveTimerReset = _moveTimer;

            #region List initialisations and Point sets
            _dropOffPoints = new List<WorldObject>();
            _closestDropOffPoint = Point.Zero;

            _buildingUnderConstruction = new List<WorldObject>();
            _closestBuildingUnderConstruction = Point.Zero;


            _buildingsAwaitingResources = new List<WorldObject>();
            _closestBuildingAwaitingResources = Point.Zero;
            #endregion

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
                    // when idle you're not carrying resources
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

                    // if there's a building that is ready to construct
                    if(ScanForBuildingsUnderConstruction())
                        break;
                    // look for buildings that are awaiting resources
                    ScanForBuildingsAwaitingResources();
                    // if you've found one you're prompted to carry a resource to it
                    // thus if that checks out, change your state
                    if (_isCarryingResource)
                    {
                        _state = KeenoState.DeliveringMaterials;
                    }
                    break;

                case KeenoState.Building:
                    DontCarryResource();
                    // if you're not walking you're at the Construction site and are "building"
                    if (!_isWalking)
                        PlayConstructingSound();
                    break;

                case KeenoState.Following:
                    DontCarryResource();
                    break;

                case KeenoState.Working:
                    // if you're not walking you're at the WorkStation and are "working"
                    if (!_isWalking)
                        PlayWorkSound();
                    DontCarryResource();
                    break;

                case KeenoState.DroppingOff:
                    StopWorkSound();
                    CarryResource();
                    MoveTo(_closestDropOffPoint);
                    // once you've arrived at the closest dropoff point
                    if (_position == _closestDropOffPoint.ToVector2())
                    {
                        // check that is still there, or if the player has removed it
                        FindClosestDropOffPoint();

                        // once you've arrived at the closest dropoff point
                        if (_position == _closestDropOffPoint.ToVector2())
                        {
                            ResourceTracker.Add(_resourceType, _resourceAmmount);
                            _state = KeenoState.Working;
                        }
                        // if the player has removed the closest dropoff point,
                        // you'll have to re evaluate which one is the closest drop off point
                        else
                            MoveTo(_closestDropOffPoint);
                    }
                    break;

                case KeenoState.DroppingOffAndIdle:
                    // You end up in this state if you're dropping off the last resource of a workstation,
                    // if the bell is rung, or if the player mines the last resource of the workstation
                    // you were working on.
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
                        SwitchToIdle();
                    break;

                    case KeenoState.WalkingToBuilderCabin:
                    DontCarryResource();
                    StopConstructingSound();

                    if (ScanForBuildingsUnderConstruction())
                        break;

                    WalkToBuilderCabin();
                    if (_position == _placeOfWork.ToVector2())
                        SwitchToReadyToBuild();
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
            // Base.Update is responsible for animation mostly
            base.Update(gt);
        }
        /// <summary>
        /// At the start of a new day go (teleport essentially) to the closest dropoffpoint.
        /// </summary>
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
        /// <summary>
        /// If you're carrying resources when the bell is rung, drop them off.
        /// 
        /// This code is not connected to the following as this code must only be called once.
        /// </summary>
        public void PlayerRangBell()
        {
            if (_state == KeenoState.DeliveringMaterials)
            {
                // If you were delivering materials to a building,
                // let that building know that the materials you promised won't be delivered.
                _buildingImDeliveringTo.DontTakeThisResource(_resourceType);
            }
            if (_state != KeenoState.Following)
                _state = KeenoState.BelRing;
        }
        /// <summary>
        /// If the player has rang the bell, drop what you're doing and go to the closest dropoff point.
        /// Then Idle.
        /// </summary>
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
        #region Sounds
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
        #endregion
        #region Scan/Find Buildings
        /// <summary>
        /// Loops through the list of worldObjects in the map class and finds the closest (if any)
        /// Building under construciton.
        /// </summary>
        /// <returns>   True if there is one
        ///             False if none are found. </returns>
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
        /// <summary>
        /// Loops through the list of worldObjects in the map class and finds the closest
        /// Building Awaiting Resources.
        /// </summary>
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
        /// <summary>
        /// Loops through the list of worldObjects in the map class and finds the closest
        /// Drop off point.
        /// The closest drop off point is then stored in the "_closestDropOffPoint" variable.
        /// </summary>
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
                sortedDropOffPointList[0].Position.X - Globals.Tile_Width_Height / 2,
                sortedDropOffPointList[0].Position.Y - Globals.Tile_Width_Height / 2).ToPoint();
        }
        #endregion
        /// <summary>
        /// Given that after every resource delivery or building build you return to the Builders Cabin,
        /// Remember where the Builders Cabin is.
        /// </summary>
        /// <param name="builderCabin"></param>
        public void RememberThisBuilderCabin(Point builderCabin)
        {
            builderCabin = new Point(builderCabin.X - Globals.Tile_Width_Height / 2,
                builderCabin.Y - Globals.Tile_Width_Height / 2);

            _placeOfWork = builderCabin;
        }
        public void WalkToBuilderCabin()
        {
            MoveTo(_placeOfWork);
        }
        /// <summary>
        /// Remove one resource from the player's total to then "carry and bring" to the building that needs it.
        /// </summary>
        /// <param name="type"></param>
        public void PickUpOneResource(ResourceType type)
        {
            ResourceTracker.Spend(type, 1);
            _isCarryingResource = true;
            _resourceType = type;
        }
        /// <summary>
        /// When prompted to, follow the player, but at apply your offset
        /// </summary>
        /// <param name="destination"></param>
        public virtual void FollowPlayer(Point destination)
        {
            destination += _playerLocationOffset.ToPoint();
            MoveTo(destination);
        }
        /// <summary>
        /// Method that tells the Keeno to pick a random direction to walk to.
        /// Used in Start, EndOfDay and GameOver Screens.
        /// </summary>
        /// <returns></returns>
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
        #region Resource Carrying and Movement Speed
        /// <summary>
        /// If you're carrying a resource, walk slow and flip the bool to true
        /// </summary>
        private void CarryResource()
        {
            if(_resourceType == ResourceType.None)
                return;
            WalkSlow();
            _isCarryingResource = true;
        }
        /// <summary>
        /// If you're not carrying a resource, walk fast and flip the bool to false
        /// </summary>
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
        #endregion
        #region Drop Off Resources
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
        #endregion
        #region Switch State to:
        public void SwitchToNewDay()
        {
            if (_state == KeenoState.DeliveringMaterials)
            {
                _buildingImDeliveringTo.DontTakeThisResource(_resourceType);
            }
            _state = KeenoState.NewDay;
        }
        public void SwitchToWalkingToBuilderCabin()
        {
            _state = KeenoState.WalkingToBuilderCabin;
        }
        public void SwitchToBuilding()
        {
            _state = KeenoState.Building;
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
            _velocity = Vector2.Zero;
            _state = KeenoState.Idle;
        }
        public void SwitchWalkingToIdleSpot()
        {
            _state = KeenoState.WalkingToIdleSpot;
        }
        #endregion
        
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
                sb.Draw(Assets.DebugPixelTxr, Bounds, Color.Blue * 1f);


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw actual sprite
            if (_isSelected)
                _tint = Color.White;
            else if (_state ==  KeenoState.Idle && !_isSelected)
                _tint = Color.Gray;
            else
                _tint = _defaultTint;


            // Swap the texture of the resource that you're carrying appropriatelly
            if (_isCarryingResource)
            {
                // if you're carrying a resource, switch spritesheet to match your state
                _txr = Assets.KeenoCarryingTxr;

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
                // Then draw it
                sb.Draw(_resourceTxr, _itemCarrySpot, null, Color.White, 0f,
                    Vector2.Zero, flip, Globals.ResourceBeingCarriedTxrLD);
            }
            else
                _txr = Assets.KeenoTxr;

            // Darw on a different scale if you're not in the playing screen as
            // the playing screen has the camera zoom applied to it.
            if (_state != KeenoState.StartScreen)
                sb.Draw(_txr, new Vector2(_rect.X, _rect.Y), _srcRect, _tint, 0f, Vector2.Zero, .9f, flip, Globals.KeenoLD);
            else
                sb.Draw(_txr, new Vector2(_rect.X, _rect.Y), _srcRect, _tint, 0f, Vector2.Zero, 2f, flip, Globals.MapLD);
        }
    }
}
