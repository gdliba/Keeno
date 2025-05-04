using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Keeno
{
    /// <summary>
    /// Class made to help with making both the Keeno and the player inherit
    /// their walk animation and relevant logic shared without making the Keeno
    /// an odd child of the player, by not also making them a MobileSpawnPoint
    /// </summary>
    class AnimatedKeeno2D : Animated2D
    {
        protected Vector2 _direction;
        public Vector2 Direction { get { return _direction; } }
        public Vector2 Position { get { return new(_position.X + _rect.Width / 2, _position.Y + _rect.Height / 2); } }
        protected Vector2 _previousPosition;

        protected bool _isWalking { get { return _velocity.Length() > 0; } }
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


        public Rectangle _targetDestinationBounds;
        public Rectangle Bounds { get { return _rect; } }

        protected Color _tint;
        protected Color _defaultTint;

        // test related
        protected Texture2D _testPixel;

        public AnimatedKeeno2D(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base(spriteSheet, fps, rect)
        {
            //_rect = new Rectangle(_rect.X + _rect.Width / 2, _rect.Y + _rect.Height / 2, _rect.Width / 2, _rect.Height / 2);
            _idleFPS = 1;

            _srcRect = new Rectangle(32, 0, rect.Width, rect.Height);
            _defaultFps = fps;

            // test related
            _testPixel = pixel;
            _drawBounds = false;
            _targetDestinationBounds = _rect;


            _tint = Color.White;
            _defaultTint = _tint;


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
            _previousPosition= _position;
        }
        public override void Update(GameTime gt)
        {
            AnimateKeeno(gt);
            _position += _velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            _rect.Location = _position.ToPoint();

            _isSelected = false;

            // Update the player's "would be" bounds in relation to
            // the direction they are moving in

            _targetDestinationBounds = new Rectangle(
                _rect.X + _rect.Width / 4 + (int)_direction.X,
                _rect.Y + _rect.Height / 4 + (int)_direction.Y,
                2 * _rect.Width / 3,
                2 * _rect.Height / 3);
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
        public Rectangle HandleMovement()
        {
            return _targetDestinationBounds;
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
        Idle,
        Following,
        Working,
        Dying,
        Dead
    }

    class Keeno : AnimatedKeeno2D
    {
        private float _moveTimer;
        private float _idleTimer;
        private float _moveTimerReset;
        private KeenoState _state;
        public KeenoState State { get{  return _state; } }

        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base (spriteSheet, fps, rect, pixel)
        {
            _state = KeenoState.Idle;
            _moveTimer = 3;
            _idleTimer = 3;
            _moveTimerReset = _moveTimer;
            _drawBounds = false;

            _defaultTint =_tint = new Color(Globals.RNG.Next(0, 256),
                Globals.RNG.Next(0, 256), Globals.RNG.Next(0, 256));

            _moveSpeed = Globals.RNG.Next(15, 26) + (float)Globals.RNG.NextDouble();

            _facingRight = Globals.RNG.Next(2) == 0;

            _workSpeed = 1f;
        }
        public override void Update(GameTime gt)
        {
            //MoveMe(MoveInThisDirection(gt));
            //if(destination!=_position.ToPoint())
            //    MoveTo(destination);

            switch (_state)
            {
                case KeenoState.Idle:
                    MoveInDirection(IdleAndMove(gt));
                    break;
                case KeenoState.Following:
                    break;
                case KeenoState.Dead:
                    break;
            }
            base.Update(gt);

        }
        public virtual void FollowPlayer(Point destination)
        {
            destination.X += _playerLocationOffsetX;
            destination.Y += _playerLocationOffsetY;
            MoveTo(destination);
        }
        public Vector2 IdleAndMove(GameTime gt)
        {
            float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;

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
            _state = KeenoState.Following;
        }
        public void SwitchToWorking()
        {
            _state = KeenoState.Working;
        }
        public void SwitchToIdle()
        {
            _state = KeenoState.Idle;
        }
        public void Selected()
        {
            _isSelected = true;
        }
    }
}
