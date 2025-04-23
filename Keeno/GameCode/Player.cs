using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Keeno
{
    class Player : MobileSwarmPoint
    {
        private Map _map;
        private Rectangle _interactionRange;
        private readonly List<Keeno> _keenos;
        private readonly List<Keeno> keenosNearPlayer;
        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _objectsNearPlayer;

        public Rectangle InteractionRange { get { return _interactionRange; } }


        public Player(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel, Map map, List<Keeno> keenos)
            : base(spriteSheet, fps, rect, pixel)
        {
            _moveSpeed = Globals.PlayerMovementSpeed;
            _drawBounds = false;
            _interactionRange = new Rectangle((int)_position.X - rect.Width, (int)_position.Y - rect.Height, rect.Width * 3, rect.Height * 3);
            _map = map;

            _worldObjects = map.WorldObjects;
            _objectsNearPlayer = new List<WorldObject>();

            _keenos = keenos;
            keenosNearPlayer = new List<Keeno>();
        }
        public override void updateme(GameTime gt)
        {
            Player_Object_Interaction();
            ColisionDependantMovement();

            Player_Keeno_Interaction();
            
            base.updateme(gt);
        }
        private void Player_Keeno_Interaction()
        {
            // clear the list
            keenosNearPlayer.Clear();
            // loop through all keenos in game
            for (var i = 0; i < _keenos.Count; i++)
            {
                if (InteractionRange.Intersects(_keenos[i].Bounds)) // check if they are inside the player's Interaction range
                    keenosNearPlayer.Add(_keenos[i]);               // if they are, add them to the "near player" list
            }
            var sortedKeenoList = keenosNearPlayer.OrderBy(x => x.DistanceTo(Position)).ToList();   // sort the list by closest first
            // if the list is populated
            if (sortedKeenoList.Count > 0)
            {
                sortedKeenoList[0].Selected();                                                      // trigger the Keeno's "Selected" method          
                if (Globals.PickUpKeeno)                                                            // if the relevant key is pressed
                {
                    Vector2 distanceToPlayer = new Vector2(Position.X - sortedKeenoList[0].Position.X, 
                        Position.Y - sortedKeenoList[0].Position.Y);
                    distanceToPlayer.Normalize();
                    sortedKeenoList[0].MoveMe(distanceToPlayer);                                    // move the selected Keeno towards the player (kinda)
                }
            }
        }
        private void ColisionDependantMovement()
        {
            // Player - movement
            if (_map.IsWalkable(HandleInput()))
            {
                MoveMe(Direction);
            }
            else
            {
                MoveMe(Vector2.Zero);
            }
        }
        private void Player_Object_Interaction()
        {
            // Clear the List of worldObjects that the are in range with the player
            _objectsNearPlayer.Clear();
            for (var i = 0; i < _map.WorldObjects.Count; i++)
            {
                if (InteractionRange.Intersects(_map.WorldObjects[i].Bounds))
                {
                    _objectsNearPlayer.Add(_map.WorldObjects[i]);
                }
            }
            // Sort the list
            var sortedList = _objectsNearPlayer.OrderBy(x => x.DistanceTo(Position)).ToList();

            if (sortedList.Count > 0)
            {
                sortedList[0].Selected();
                if (Globals.Interact)
                    sortedList[0].OnInteract();
            }
        }

        public Rectangle HandleInput()
        {
            _direction = Vector2.Zero;

            if (Globals.MoveUP) _direction.Y -= 1;
            if (Globals.MoveDOWN) _direction.Y += 1;
            if (Globals.MoveLEFT) _direction.X -= 1;
            if (Globals.MoveRIGHT) _direction.X += 1;

            return _targetDestinationBounds;
        }
        public override void drawme(SpriteBatch sb)
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
                    Vector2.Zero, flip, 0f);

            // Draw test pixel
            if (_drawBounds)
            {
                sb.Draw(_testPixel, _interactionRange, Color.Red * .75f);               // Draw interactionRange
                sb.Draw(_testPixel, Bounds, Color.Blue * .7f);                          // Draw Player Bounds
                sb.Draw(_testPixel, _targetDestinationBounds, Color.White * .75f);      // Draw _targetDestinationBound
                sb.Draw(_testPixel, new Vector2(Position.X, Position.Y), Color.Black);  // Draw Player Position
            }
            //base.drawme(sb);
        }
    }
}
