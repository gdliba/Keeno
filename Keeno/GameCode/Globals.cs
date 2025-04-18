using Microsoft.Xna.Framework.Input;

namespace Keeno
{
    static class Globals
    {
        static KeyboardState _keyboard;
        public static void UpdateInput() => _keyboard = Keyboard.GetState();

        public static bool MoveUP => _keyboard.IsKeyDown(Keys.W);
        public static bool MoveLEFT => _keyboard.IsKeyDown(Keys.A);
        public static bool MoveDOWN => _keyboard.IsKeyDown(Keys.S);
        public static bool MoveRIGHT => _keyboard.IsKeyDown(Keys.D);
        public static bool NoMovementKeysPressed { get { return !MoveUP && !MoveLEFT && !MoveDOWN && !MoveRIGHT; } }
    }
}
