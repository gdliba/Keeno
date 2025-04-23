using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Keeno
{

    class Keeno : AnimatedKeeno2D
    {
        private float _moveTimer;
        private float _moveTimerReset;

        public Keeno(Texture2D spriteSheet, int fps, Rectangle rect, Texture2D pixel)
            : base (spriteSheet, fps, rect, pixel)
        {
            _moveTimer = 3;
            _moveTimerReset = _moveTimer;
            _moveSpeed = Globals.KeenoMovementSpeed;
            _drawBounds = false;
        }

        public override void updateme(GameTime gt)
        {
            //MoveMe(MoveInThisDirection(gt));

            base.updateme(gt);
        }
        public Vector2 MoveInThisDirection(GameTime gt)
        {
            if (_moveTimer >= 0)
            {
                _moveTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                RollRandomDirection();
                _moveTimer = _moveTimerReset;
            }
            return _direction;
        }
        public void RollRandomDirection()
        {
            _direction = new Vector2(Game1.RNG.Next(-1, 2), Game1.RNG.Next(-1, 2));
        }
        public float DistanceTo(Vector2 destination)
        {
            return (destination - _position).Length();
        }
        public void Selected()
        {
            _isSelected = true;
        }
    }
}
