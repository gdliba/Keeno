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
        Building,
        Dying,
        Dead
    }
    class Player : MobileSwarmPoint
    {
        private Map _map;

        private PlayerState _state;

        private Rectangle _interactionRange;

        private readonly List<Keeno> _keenos;
        private readonly List<Keeno> _keenosNearPlayer;
        private readonly List<Keeno> _followers;

        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _objectsNearPlayer;
        private readonly List<WorldObject> _emptyTilesNearPlayer;
        private readonly List<BuildingBlueprint> _itemsNearPlayer;

        private Rectangle _tileTargetedRect;



        public Rectangle InteractionRange { get { return _interactionRange; } }


        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel, Map map, List<Keeno> keenos)
            : base(spriteSheet, fps, rect, pixel)
        {
            _state = PlayerState.Normal;
            _moveSpeed = Globals.PlayerMovementSpeed;
            _drawBounds = false;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);
            _map = map;

            _worldObjects = map.WorldObjects;
            _objectsNearPlayer = new List<WorldObject>();
            _emptyTilesNearPlayer = new List<WorldObject>();

            _keenos = keenos;
            _keenosNearPlayer = new List<Keeno>();
            _followers = new List<Keeno>();
            _workSpeed = .02f;

        }
        public override void Update(GameTime gt)
        {
            if (_isWalking)
                _tileTargetedRect = new Rectangle(
                    _rect.X + _rect.Width / 4 + (int)_direction.X*20,
                    _rect.Y + _rect.Height / 4 + (int)_direction.Y*20,
                     _rect.Width / 3,
                     _rect.Height / 3);

            if (Globals.Tab_KeyPress)
                _state++;
            if (_state == PlayerState.Dead)
                _state = PlayerState.Normal;
            if(_state == PlayerState.Building)
            {
                BuildingMode();
            }
            if (_state != PlayerState.Building)
            {
                Player_Object_Interaction();
                
            }
            ColisionDependantMovement();
            Player_Keeno_Interaction(gt);
            //Player_Item_Interaction();

            base.Update(gt);
        }
        private void BuildingMode()
        {
            #region Sort By Distance
            // Clear the List of worldObjects that the are in range with the player
            _emptyTilesNearPlayer.Clear();

            foreach (var tile in _map.WorldObjects.OfType<EmptyTile>())
            {
                if (_tileTargetedRect.Intersects(tile.Bounds))
                    _emptyTilesNearPlayer.Add(tile);
            }
            // Sort the list
            var sortedList = _emptyTilesNearPlayer.OrderBy(x => x.DistanceTo(
                _tileTargetedRect.Location.ToVector2())).ToList();
            #endregion
            if (sortedList.Count > 0)
            {
                // Call the Selected method of the closest World Object
                sortedList[0].Selected(_state == PlayerState.Building,
                    _workSpeed,Globals.DropOffKeenoSpeed);

            }
        }
        private void Player_Item_Interaction()
        {
            #region Sort By Distance

            _itemsNearPlayer.Clear();
            for (var i = 0; i < ; i++)
            {
                // only consider tiles that aren't empty
                if (InteractionRange.Intersects(_map.WorldObjects[i].Bounds)
                    && _map.WorldObjects[i].GetType() != typeof(EmptyTile))
                    _objectsNearPlayer.Add(_map.WorldObjects[i]);
            }
            // Sort the list
            var sortedList = _objectsNearPlayer.OrderBy(x => x.DistanceTo(Position)).ToList();
            #endregion
            if (sortedList.Count > 0)
            {
                // Call the Selected method of the closest World Object
                sortedList[0].Selected(_followers.Count > 0,
                    _workSpeed, Globals.DropOffKeenoSpeed);
                if (Globals.E_KeyDown)
                    sortedList[0].OnInteract();

                // When pressing Q, if there are keenos following the player
                // Go to that location
                if (_followers.Count > 0 && Globals.Q_KeyDown)
                {
                    if (sortedList[0].CanDropOffWorker(_followers[0]))
                        _followers.RemoveAt(0);
                }
            }
        }

        private void Player_Object_Interaction()
        {
            #region Sort By Distance
            // Clear the List of worldObjects that the are in range with the player
            _objectsNearPlayer.Clear();
            for (var i = 0; i < _map.WorldObjects.Count; i++)
            {
                // only consider tiles that aren't empty
                if (InteractionRange.Intersects(_map.WorldObjects[i].Bounds)
                    && _map.WorldObjects[i].GetType() != typeof(EmptyTile))
                    _objectsNearPlayer.Add(_map.WorldObjects[i]);
            }
            // Sort the list
            var sortedList = _objectsNearPlayer.OrderBy(x => x.DistanceTo(Position)).ToList();
            #endregion
            if (sortedList.Count > 0)
            {
                // Call the Selected method of the closest World Object
                sortedList[0].Selected(_followers.Count > 0,
                    _workSpeed, Globals.DropOffKeenoSpeed);
                if (Globals.E_KeyDown)
                    sortedList[0].OnInteract();

                // When pressing Q, if there are keenos following the player
                // Go to that location
                if (_followers.Count > 0 && Globals.Q_KeyDown)
                {
                    if (sortedList[0].CanDropOffWorker(_followers[0]))
                        _followers.RemoveAt(0);
                }
            }
        }
        private void ColisionDependantMovement()
        {
            // Player - movement
            if (_map.IsWalkable(HandleInput()))
                MoveInDirection(Direction);
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
            // tell all followers to follow the player
            foreach (var keeno in _followers)
            {
                keeno.FollowPlayer(_position.ToPoint());
            }
        }

        public Rectangle HandleInput()
        {
            _direction = Vector2.Zero;

            if (Globals.W_KeyDown) _direction.Y -= 1; // UP
            if (Globals.S_KeyDown) _direction.Y += 1; // Down
            if (Globals.A_KeyDown) _direction.X -= 1; // Left
            if (Globals.D_KeyDown) _direction.X += 1; // Right

            return _targetDestinationBounds;
        }
        public override void Draw(SpriteBatch sb)
        {
            // Make sure the rectangle moves and is drawn in the right position
            _interactionRange.X = (int)_position.X - _rect.Width;
            _interactionRange.Y = (int)_position.Y - _rect.Height;

            // Make sure the rectangle moves and is drawn in the right position
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;


            // determine when to flip the sprite (making it look to the RIGHT)
            var flip = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // Draw Player
            sb.Draw(_txr, _rect, _srcRect, _tint, 0f,
                    Vector2.Zero, flip, .1f);

            // Draw test pixel
            if (_drawBounds)
            {
                sb.Draw(_testPixel, _tileTargetedRect, Color.Green * .8f);      // Draw _tileTargetedRect
                sb.Draw(_testPixel, _targetDestinationBounds, Color.White * .8f);      // Draw _targetDestinationBound
                sb.Draw(_testPixel, Bounds, Color.Blue * .7f);                          // Draw Player Bounds
                sb.Draw(_testPixel, _interactionRange, Color.Red * .75f);               // Draw interactionRange
                sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Black);  // Draw Player Position
            }
        }
    }
}
