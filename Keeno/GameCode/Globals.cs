using Microsoft.Xna.Framework.Input;

namespace Keeno
{
    static class Globals
    {

        public static bool MoveUP => Keyboard.GetState().IsKeyDown(Keys.W);
        public static bool MoveLEFT => Keyboard.GetState().IsKeyDown(Keys.A);
        public static bool MoveDOWN => Keyboard.GetState().IsKeyDown(Keys.S);
        public static bool MoveRIGHT => Keyboard.GetState().IsKeyDown(Keys.D);
        public static bool NoMovementKeysPressed { get { return !MoveUP && !MoveLEFT && !MoveDOWN && !MoveRIGHT; } }
    }
}
