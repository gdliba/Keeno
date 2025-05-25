using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Diagnostics;

namespace Keeno
{
    enum PlayerState
    {
        Normal,
        BuildingMode,
        Dying,
        Dead
    }
    class Player : AnimatedKeeno2D
    {
        //private Map _map;

        private PlayerState _state;
        private bool _swapToNormalState;

        private Rectangle _interactionRange;

        private Point _itemCarryPoint { get { return new((int)_position.X, (int)_position.Y - 5); } }

        private readonly List<Keeno> _keenos;
        private readonly List<Keeno> _keenosNearPlayer;
        private readonly List<Keeno> _followers;

        private readonly List<WorldObject> _objectsNearPlayer;
        private readonly List<EmptyTile> _emptyTilesNearPlayer;
        private readonly List<Item> _itemsNearPlayer;
        private Item? _itemCarrying;

        private Rectangle _tileTargetedRect;

        public Rectangle InteractionRange { get { return _interactionRange; } }


        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel, Map map, List<Keeno> keenos)
            : base(spriteSheet, fps, rect, pixel, map)
        {
            _state = PlayerState.Normal;
            _moveSpeed = Globals.PlayerMovementSpeed;
            _drawBounds = false;
            _swapToNormalState = false;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);

            //_worldObjects = map.WorldObjects;
            _objectsNearPlayer = new List<WorldObject>();
            _emptyTilesNearPlayer = new List<EmptyTile>();
            _itemsNearPlayer = new List<Item>();


            _keenos = keenos;
            _keenosNearPlayer = new List<Keeno>();
            _followers = new List<Keeno>();
            _workSpeed = 5f;

            //_itemCarryPoint = new Point(0, 0);
            _itemCarrying = null;

        }
        public override void Update(GameTime gt)
        {
            _objectsNearPlayer.Clear();
            _emptyTilesNearPlayer.Clear();

            base.Update(gt);
            ColisionDependantMovement();

            // update the point that the items the player is carrying follows
            //_itemCarryPoint = new Point((int)_position.X, (int)_position.Y - 5);
            // tell the item you are holding to follow you
            _itemCarrying?.FollowPlayer(_itemCarryPoint);

            // tell all followers to follow the player
            foreach (var keeno in _followers)
            {
                keeno.FollowPlayer(_position.ToPoint());
            }

            if (_isWalking)
                _tileTargetedRect = new Rectangle(
                    _rect.X + 2*_rect.Width / 5 + (int)_direction.X*25,
                    _rect.Y + 2*_rect.Height / 5 + (int)_direction.Y*25,
                     _rect.Width / 4,
                     _rect.Height / 4);


            if (Globals.Tab_KeyPress)
                _state++;
            if (_state == PlayerState.Dead)
                _state = PlayerState.Normal;
            if(_state == PlayerState.BuildingMode)
            {
                BuildingMode();
            }
            if (_state != PlayerState.BuildingMode)
            {
                Player_Object_Interaction();
                Player_Keeno_Interaction(gt);
            }

        }
        private void BuildingMode()
        {
            if (_swapToNormalState)
            {
                _swapToNormalState = false;
                _state = PlayerState.Normal;
            }

            #region Select Closes Empty Tile
            // Clear the List of worldObjects that the are in range with the player

            foreach (EmptyTile tile in _worldObjects.OfType<EmptyTile>())
            {
                if (tile is not OccupiedTile) 
                {
                    if (_tileTargetedRect.Intersects(tile.Bounds))
                        _emptyTilesNearPlayer.Add(tile);
                }
            }
            // Sort the list
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
                    _itemCarrying = null;
                    _swapToNormalState = true;
                }
            }
        }

        private void Player_Object_Interaction()
        {
            #region Sort By Distance
            // Clear the List of worldObjects that the are in range with the player
            for (var i = 0; i < _map.WorldObjects.Count; i++)
            {
                // only consider tiles that aren't empty
                if (InteractionRange.Contains(_map.WorldObjects[i].Position)
                    && _map.WorldObjects[i] is not EmptyTile
                    && _map.WorldObjects[i] is SelectableWorldObject
                    && _map.WorldObjects[i].State != ObjectState.Dead)
                    _objectsNearPlayer.Add(_map.WorldObjects[i]);
            }
            // Sort the list
            Vector2 positionBasedOnDirection = new Vector2(Position.X + 5f * _direction.X, Position.Y + 5f * _direction.Y);
            var sortedWorldObjectList = _objectsNearPlayer.OrderBy(x => x.DistanceTo(positionBasedOnDirection)).ToList();
            #endregion
            if (sortedWorldObjectList.Count > 0)
            {
                // if it IS AN ITEM
                if (sortedWorldObjectList[0] is Item selectedItem)
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
                            _itemCarrying = shopBlueprint.OnInteract(_itemCarryPoint);
                            if (_itemCarrying != null)
                                _state = PlayerState.BuildingMode;
                        }
                    }
                }

                // if selected World object IS a WORKSTATION
                else if (sortedWorldObjectList[0] is WorkStation selectedWorkStation)
                {
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
                // At this time only TownCentre
                else if (sortedWorldObjectList[0] is TownCentre townCentre)
                {
                    townCentre.Selected();
                }
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
                else if (sortedWorldObjectList[0] is BuilderCabin builderCabin)
                {
                    builderCabin.Selected();
                }
            }
        }
        private void ColisionDependantMovement()
        {

            // Player movement
            SetDirection();
            
            if (_direction != Vector2.Zero && _map.IsWalkable(TargetDestinationBounds))
            {
                MoveInDirection(_direction);
            }
            else
                MoveInDirection(Vector2.Zero);
        }
        private void Player_Keeno_Interaction(GameTime gt)
        {
            _keenosNearPlayer.Clear();
            // loop through all keenos in game
            for (var i = 0; i < _keenos.Count; i++)
            {
                if (InteractionRange.Intersects(_keenos[i].Bounds) && _keenos[i].State == KeenoState.Idle) // check if they are inside the player's Interaction range
                {
                    _keenosNearPlayer.Add(_keenos[i]);              // if they are, add them to the "near player" list
                    //_keenos.RemoveAt(i);
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
                    _followers.Add(sortedKeenoList[0]); // add it to the list of followers
                }
            }
        }

        public void SetDirection()
        {
            _direction = Vector2.Zero;
            _velocity = Vector2.Zero;

            if (Globals.W_KeyDown) _direction.Y -= 1; // UP
            if (Globals.S_KeyDown) _direction.Y += 1; // Down
            if (Globals.A_KeyDown) _direction.X -= 1; // Left
            if (Globals.D_KeyDown) _direction.X += 1; // Right
        }

        public override void Draw(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _interactionRange.X = (int)_position.X - _rect.Width;
            _interactionRange.Y = (int)_position.Y - _rect.Height;

            // Make sure the rectangle moves and is drawn in the right position
            _rect.Location = _position.ToPoint();



            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            // Draw test pixel
            if (_drawBounds)
            {
                sb.Draw(_testPixel, Position, Color.Black);  // Draw Player Position

                sb.Draw(_testPixel, _tileTargetedRect, Color.Green * .8f);      // Draw _tileTargetedRect
                sb.Draw(_testPixel, TargetDestinationBounds, Color.White * .8f);      // Draw _targetDestinationBound
                sb.Draw(_testPixel, Bounds, Color.Blue * .7f);                          // Draw Player Bounds
                sb.Draw(_testPixel, _interactionRange, Color.Red * .75f);               // Draw interactionRange
            }
            // Draw Player
            sb.Draw(_txr, _rect, _srcRect, _tint, 0f,
                    Vector2.Zero, flip, Globals.PlayerLD);
        }
    }
}
