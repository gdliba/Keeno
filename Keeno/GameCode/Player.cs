using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Keeno
{
    enum PlayerState
    {
        Normal,
        BuildingMode,
        Dying,
        Dead
    }
    /// <summary>
    /// Player class. Inherits from AnimatedKeeno2D as the player animates in the same way as the Keeno.
    /// </summary>
    class Player : AnimatedKeeno2D
    {
        #region Variables
        public Vector2 RenderPosition => _position;

        private PlayerState _state;
        private bool _swapToNormalState;

        private float _footstepTimer;
        private Point _defaultStartingPos;

        public event Action FirstInteraction, FirstFollower, FirstBluePrint, FirstKeeno, FirstStone;
        private bool _firstInteraction, _firstFollower, _firstBluePrint, _firstKeeno, _firstStone;
        public List<Keeno> Followers {  get  { return _followers; } }
        private readonly List<Keeno> _keenos;
        private readonly List<Keeno> _keenosNearPlayer;
        private readonly List<Keeno> _followers;

        private readonly List<WorldObject> _objectsNearPlayer;
        private readonly List<EmptyTile> _emptyTilesNearPlayer;

        private Point _itemCarryPoint { get { return new((int)_position.X, (int)_position.Y - 5); } }
        private Item? _itemCarrying;

        public Rectangle InteractionRange { get { return _interactionRange; } }
        private Rectangle _tileTargetedRect;
        private Rectangle _interactionRange;
        private Vector2 _lastMovementDirection;
        #endregion

        /// <summary>
        /// Player Constructor. Gets told where to spawn, takes in the Map to access the list of
        /// WorldObjects (including items), the list of Keenos in Game to be able to enlist them as followers.
        /// Fps for player (when walking) is higher than the rest of the Keeno.
        /// </summary>
        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Map map, List<Keeno> keenos)
            : base(spriteSheet, fps, rect, map)
        {
            // Tutorial related bools (they trigger specific text to appear given specific player actions)
            _firstStone = true;
            _firstKeeno = true;
            _firstBluePrint = true;
            _firstFollower = true;
            _firstInteraction = true;

            // Default values
            _drawBounds = false;
            _state = PlayerState.Normal;
            _defaultStartingPos = new Point(rect.X+_rect.Width/2, rect.Y+ _rect.Height/ 2);
            _moveSpeed = Globals.PlayerMovementSpeed;
            _swapToNormalState = false;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);
            _lastMovementDirection = Vector2.UnitY;
            _itemCarrying = null;
            _workSpeed = 10f;
            _footstepTimer = 0;

            // List initialisations
            _objectsNearPlayer = new List<WorldObject>();
            _emptyTilesNearPlayer = new List<EmptyTile>();
            _keenosNearPlayer = new List<Keeno>();
            _followers = new List<Keeno>();
            _keenos = keenos;
        }
        /// <summary>
        /// Player Hard Reset method. Resets the player completely to the original starting values.
        /// Some values are missing as they are never changed in the current state of the game:
        /// For example the player's "_workSpeed"
        /// </summary>
        public void Reset()
        {
            _swapToNormalState = false;
            _firstStone = true;
            _firstKeeno = true;
            _firstBluePrint = true;
            _firstFollower = true;
            _firstInteraction = true;
            _followers.Clear();
            _state = PlayerState.Normal;
            _position = _defaultStartingPos.ToVector2();
            _itemCarrying = null;
        }
        /// <summary>
        /// Player Soft Reset method. Resets only immediatelly usefull values.
        /// Essentially the same as the previous minus the "Tutorial" text related variables.
        /// </summary>
        public void DayReset()
        {
            _swapToNormalState = false;
            _followers.Clear();
            _state = PlayerState.Normal;
            _position = _defaultStartingPos.ToVector2();
            _itemCarrying = null;
        }
        /// <summary>
        /// Player's Update method. 
        /// </summary>
        /// <param name="gt"></param>
        public override void Update(GameTime gt)
        {
            #region Tutorial Related
            // These did not strictly need to be in the player class, they did seem somewhat appropriate in here though,
            // given that Game1 is already subscribing to player Events. It can be argued that only the last 
            // of the 3 events should be in the player class and that the other 2 should be in the GameManager.
            if (_firstKeeno && ResourceTracker.CanSpend(ResourceType.Food, ResourceTracker.KeenoCost))
            {
                FirstKeeno?.Invoke();
                _firstKeeno = false;
            }
            if (_firstStone && ResourceTracker.CanSpend(ResourceType.Stone, 1))
            {
                FirstStone?.Invoke();
                _firstStone = false;
            }
            if (_firstFollower && _followers.Count == 1)
            {
                FirstFollower?.Invoke();
                _firstFollower = false;
            }
            #endregion

            // Play the footstep sounds at a speed that makes sense
            if (_isWalking && _footstepTimer <= 0)
            {
                PlayFootstep();
                // Reset timer based on speed ("23/_moveSpeed" feels appropriate ONLY paired with the current, 5, FPS)
                _footstepTimer = (23 / _moveSpeed); 
            }
            _footstepTimer -= Globals.DeltaTime;


            _objectsNearPlayer.Clear();
            _emptyTilesNearPlayer.Clear();

            // Decide which movement is safe before applying it.
            ColisionDependantMovement();

            // Animate the player and apply the safe velocity.
            base.Update(gt);

            UpdateInteractionRange();

            // tell the item you are holding to follow you
            _itemCarrying?.FollowPlayer(_itemCarryPoint);

            // Tell all followers to follow the player.
            foreach (var keeno in _followers)
            {
                keeno.FollowPlayer(_position.ToPoint());
            }

            // Only update the _tileTargetedRect (Used to choose where to playe buildings in "BuildMode")
            // when you walk.
            if (_isWalking)
                _tileTargetedRect = new Rectangle(
                    _rect.X + 2*_rect.Width / 5 + (int)_direction.X*25,
                    _rect.Y + 2*_rect.Height / 5 + (int)_direction.Y*25,
                     _rect.Width / 4,
                     _rect.Height / 4);



            switch (_state)
            {
                case PlayerState.BuildingMode:
                    BuildingMode();

                    break;
                default:
                    Player_Object_Interaction();
                    Player_Keeno_Interaction(gt);

                    break;
            }

        }
        /// <summary>
        /// When in build mode the player can only interact with empty tiles
        /// in order to place the equipped blueprint.
        /// Ok the player can also collect GoldCoins, it felt weird not being able to.
        /// </summary>
        private void BuildingMode()
        {
            if (_swapToNormalState)
            {
                _swapToNormalState = false;
                _state = PlayerState.Normal;
            }

            // Allow the player to gather gold coins if in Build Mode
            foreach (Item item in _worldObjects.OfType<GoldCoin>())
            {
                if (_rect.Intersects(item.CoreRect) && item is GoldCoin goldCoin)
                {
                    if (_rect.Intersects(goldCoin.CoreRect))
                        goldCoin.GatherGoldCoin();
                    break;
                }
            }

            #region Select Closes Empty Tile

            // Loop through the List of worldObjects of type "EmptyTile"
            foreach (EmptyTile tile in _worldObjects.OfType<EmptyTile>())
            {
                if (tile is not OccupiedTile) 
                {
                    if (_tileTargetedRect.Intersects(tile.Bounds))
                        _emptyTilesNearPlayer.Add(tile);
                }
            }
            // Sort the list by distance to the player
            var sortedEmptyTileList = _emptyTilesNearPlayer.OrderBy(x => x.DistanceTo(
                _tileTargetedRect.Location.ToVector2())).ToList();
            #endregion

            if (sortedEmptyTileList.Count > 0)
            {
                // Call the Selected method of the closest Empty Tile
                sortedEmptyTileList[0].Selected();
                if(_itemCarrying !=null && Globals.E_KeyPress)
                {
                    _itemCarrying.Place(sortedEmptyTileList[0].Bounds);
                    // You are no longer carrying an item, change state
                    _itemCarrying = null;
                    _swapToNormalState = true;
                }
            }
        }

        /// <summary>
        /// Method in charge of dictating how the player interacts with all GameObjects.
        /// </summary>
        private void Player_Object_Interaction()
        {
            // Gather Gold Coin if walking over it
            foreach (Item item in _worldObjects.OfType<GoldCoin>())
            {
                if (_rect.Intersects(item.CoreRect) && item is GoldCoin goldCoin)
                {
                    if (_rect.Intersects(goldCoin.CoreRect))
                        goldCoin.GatherGoldCoin();
                    break;
                }
            }

            #region Sort WorldObjects By Distance
            // Loop through the List of worldObjects
            for (var i = 0; i < _worldObjects.Count; i++)
            {
                // only consider tiles that aren't empty or dead
                if (InteractionRange.Contains(_worldObjects[i].Position)
                    && _worldObjects[i] is not EmptyTile
                    && _worldObjects[i] is SelectableWorldObject
                    && _worldObjects[i].State != ObjectState.Dead)
                    _objectsNearPlayer.Add(_worldObjects[i]);
            }
            // Sort the list by distance to the player
            Vector2 positionBasedOnDirection =
                Position + _lastMovementDirection * 10f;
            var sortedWorldObjectList = 
                _objectsNearPlayer.OrderBy(x => x.DistanceTo(positionBasedOnDirection)).ToList();
            #endregion

            if (sortedWorldObjectList.Count > 0)
            {
                // if it IS AN ITEM
                if (sortedWorldObjectList[0] is Item selectedItem && selectedItem is not GoldCoin)
                {
                    // Call the Selected method of the closest World Object
                    selectedItem.Selected(_state != PlayerState.BuildingMode);
                    if (Globals.E_KeyPress && selectedItem is not ShopBuildingBlueprint)
                    {
                        _itemCarrying = selectedItem as Item;
                        _state = PlayerState.BuildingMode;
                    }
                    // If the item is a ShopBlueprint
                    else if (selectedItem is ShopBuildingBlueprint shopBlueprint)
                    {
                        // Cycle the blueprints
                        if (Globals.Q_KeyPress)
                            shopBlueprint.OnQInteract();
                        // Buy the bluprint
                        else if (Globals.E_KeyPress)
                        {
                            // Tutorial related
                            if (_firstBluePrint)
                            {
                                _firstBluePrint = false;
                                FirstBluePrint?.Invoke();
                            }
                            _itemCarrying = shopBlueprint.OnInteract(_itemCarryPoint);
                            // if you've baught it, you've equipped it, thus, switch your mode
                            if (_itemCarrying != null)
                                _state = PlayerState.BuildingMode;
                        }
                    }
                }

                // if selected World object IS a WORKSTATION
                else if (sortedWorldObjectList[0] is WorkStation selectedWorkStation)
                {
                    // let game1 know to show this is the first interaction with a WORKSTATION
                    if (_firstInteraction && 
                        selectedWorkStation is not BuilderCabin
                        && selectedWorkStation is not Bell)
                    {
                        FirstInteraction?.Invoke();
                        _firstInteraction = false;
                    }


                    // if it's Gold
                    if(selectedWorkStation is GoldFromation goldFormation)
                    {
                        if(goldFormation.State == ObjectState.Broken &&
                            _rect.Intersects(goldFormation.CoreRect))
                            goldFormation.GatherGoldCoin();
                    }
                    // Select said WORKSTATION
                    selectedWorkStation.Selected(_workSpeed, _followers.Count > 0);
                    // Call OnInteract when E is pressed
                    if (Globals.E_KeyPress)
                        selectedWorkStation.OnInteract();
                    // if you have followers
                    if (_followers.Count > 0)
                    {
                        // Check if the WORKSTATION has available worker slots
                        if (selectedWorkStation.CanDropOffWorker(_followers[0])) // Give the follower to the WORKSTATION
                        {
                            var sfx = Assets.PlayerDroppingOffFollowerSFX.CreateInstance();
                            sfx.Play();
                            _followers.RemoveAt(0);
                        }
                    }
                }
                // if selected World object IS a BUILDING
                else if (sortedWorldObjectList[0] is Building building)
                {
                    if (building.Type == BuildingType.FarmLand)
                    {
                        building.Selected(_workSpeed, _followers.Count > 0);
                        // if you have followers
                        if (_followers.Count > 0 && building.Farm != null)
                        {
                            // Check if the WORKSTATION has available worker slots
                            if (building.Farm.CanDropOffWorker(_followers[0])) // Give the follower to the WORKSTATION
                            {
                                var sfx = Assets.PlayerDroppingOffFollowerSFX.CreateInstance();
                                sfx.Play();
                                _followers.RemoveAt(0);
                            }
                        }
                    }
                    else
                    building.Selected();
                }
                // if selected World object IS a TOWNCENTRE
                else if (sortedWorldObjectList[0] is TownCentre townCentre)
                {
                    townCentre.Selected();
                }
                // if selected World object IS a BUILDERS CABIN
                else if (sortedWorldObjectList[0] is BuilderCabin builderCabin)
                {
                    builderCabin.Selected();
                }
                // if selected World object IS a SHOP
                else if (sortedWorldObjectList[0] is Shop shop)
                {
                    shop.Selected();
                }
            }
        }
        private void UpdateInteractionRange()
        {
            _interactionRange.X = (int)_position.X - _rect.Width;
            _interactionRange.Y = (int)_position.Y - _rect.Height;
        }
        /// <summary>
        /// Method that handles the player's movement and collision detection. 
        /// It checks for potential collisions with the map and adjusts 
        /// the player's velocity accordingly to prevent moving into non-walkable areas.
        /// </summary>
        private void ColisionDependantMovement()
        {
            SetDirection();

            if (_direction == Vector2.Zero)
            {
                _velocity = Vector2.Zero;
                return;
            }

            // Calculate and preserve movement direction and speed
            Vector2 normalizedDirection = Vector2.Normalize(_direction);

            Vector2 intendedVelocity =
                normalizedDirection * _moveSpeed;

            Vector2 intendedMovement =
                intendedVelocity * Globals.DeltaTime;

            Vector2 collisionTestPosition = _position;
            Vector2 allowedVelocity = Vector2.Zero;

            // Test horizontal movement
            if (intendedMovement.X != 0f)
            {
                Vector2 horizontalPosition = new Vector2(
                    _position.X + intendedMovement.X,
                    _position.Y
                );

                Rectangle horizontalBounds =
                    GetCollisionBoundsAt(horizontalPosition);

                if (_map.IsWalkable(horizontalBounds))
                {
                    collisionTestPosition.X = horizontalPosition.X;
                    allowedVelocity.X = intendedVelocity.X;
                }
            }

            // Test vertical movement from the horizontally resolved position.
            if (intendedMovement.Y != 0f)
            {
                Vector2 verticalPosition = new Vector2(
                    collisionTestPosition.X,
                    _position.Y + intendedMovement.Y
                );

                Rectangle verticalBounds =
                    GetCollisionBoundsAt(verticalPosition);

                if (_map.IsWalkable(verticalBounds))
                {
                    allowedVelocity.Y = intendedVelocity.Y;
                }
            }

            _velocity = allowedVelocity;
        }
        /// <summary>
        /// Method that returns a rectangle that represents the player's collision bounds 
        /// at a given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Rectangle GetCollisionBoundsAt(Vector2 position)
        {
            return new Rectangle(
                (int)position.X + _rect.Width / 4,
                (int)position.Y + _rect.Height / 4,
                _rect.Width / 2,
                _rect.Height / 2
            );
        }
        /// <summary>
        /// Method that dictates the way the player interacts with the Keeno.
        /// </summary>
        /// <param name="gt"></param>
        private void Player_Keeno_Interaction(GameTime gt)
        {
            _keenosNearPlayer.Clear();
            // loop through all keenos in game
            for (var i = 0; i < _keenos.Count; i++)
            {
                // check if they are inside the player's Interaction range
                if (InteractionRange.Intersects(_keenos[i].Bounds) && _keenos[i].State == KeenoState.Idle) 
                {
                    // if they are, add them to the "near player" list
                    _keenosNearPlayer.Add(_keenos[i]);              
                }
            }

            // sort the list by closest first
            var sortedKeenoList = _keenosNearPlayer.OrderBy(x => x.DistanceTo(Position)).ToList();   
            // if the list is populated
            if (sortedKeenoList.Count > 0)
            {
                sortedKeenoList[0].Selected();              // trigger the Keeno's "Selected" method
                                    
                if (Globals.Q_KeyPress)                     // if the relevant key is pressed
                {
                    sortedKeenoList[0].SwitchToFollowing(); // Switch the closest keeno's state to "Following"
                    _followers.Add(sortedKeenoList[0]);     // add it to the list of followers
                }
            }
        }
        /// <summary>
        /// Method that picks between 2 different footstep SFX and randomises 
        /// Pitch and Volume in order to create some variance in footstep SFX.
        /// </summary>
        public void PlayFootstep()
        {
            bool temp = Globals.RNG.Next(2) == 0;
            SoundEffectInstance instance = Assets.Footstep1Sfx.CreateInstance();
            if (temp)
                instance = Assets.Footstep2Sfx.CreateInstance();

            // Randomize pitch
            instance.Pitch = (float)(Globals.RNG.NextDouble() * .5);
            // Randomize volume
            instance.Volume = (float)(0.3 + Globals.RNG.NextDouble() * 0.4);
            instance.Play();
        }
        /// <summary>
        /// Method that sets the player's desired direction.
        /// Also updates the last movement direction if the player is moving.
        /// </summary>
        public void SetDirection()
        {
            _direction = Vector2.Zero;
            _velocity = Vector2.Zero;

            if (Globals.W_KeyDown) _direction.Y -= 1; // UP
            if (Globals.S_KeyDown) _direction.Y += 1; // Down
            if (Globals.A_KeyDown) _direction.X -= 1; // Left
            if (Globals.D_KeyDown) _direction.X += 1; // Right

            if (_direction != Vector2.Zero)
            {
                _lastMovementDirection = Vector2.Normalize(_direction);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _rect.Location = _position.ToPoint();

            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw Bounds
            if (_drawBounds)
            {
                var pixel = Assets.DebugPixelTxr;
                sb.Draw(pixel, Position, Color.Black);                          // Draw Player Position

                sb.Draw(pixel, _tileTargetedRect, Color.Green * .8f);           // Draw _tileTargetedRect
                sb.Draw(pixel, TargetDestinationBounds, Color.White * .8f);     // Draw _targetDestinationBound
                sb.Draw(pixel, Bounds, Color.Blue * .7f);                       // Draw Player Bounds
                sb.Draw(pixel, _interactionRange, Color.Red * .75f);            // Draw interactionRange
            }

            // Draw Player
            sb.Draw(
                    _txr,
                    RenderPosition,
                    _srcRect,
                    _tint,
                    0f,
                    Vector2.Zero,
                    1f,
                    flip,
                    Globals.PlayerLD);  
        }
    }
}
