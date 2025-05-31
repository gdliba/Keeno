using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;




namespace Keeno.GameCode
{
    class TextManager
    {
        private Vector2 _inGamePosition, _endOfDayPosition;
        public static Dictionary<string,TypewriterText> Tutorials = new ();
        private float _timer, _timerReset;
        private int _currIndex =-1;

        public static readonly Dictionary<string, string> TutorialText = new Dictionary<string, string>()
        {
            {   "Controls",             "Use <y>W</y>/<g>A</g>/<r>S</r>/<b>D</b> to move." },
            {   "First Keeno",          "Press <y>Q</y> while near an idle <g>Keeno</g> to make it <y>follow you</y>." },
            {   "Resource Interact",    "<y>Hold E</y> when near a Resource to <y>Harvest it</y>." },
            {   "Resource Debris",      "Once depleted, resources leave debris.<y>Hold X</y> to <r>remove</r> it." },
            {   "Buy Keeno",            "Go to the <y>Town Centre</y> to grow a <g>Keeno</g>." },
            {   "First Follower",       "<y>Hold Q</y> when near a Resource to assign a <g>Keeno</g> work. " },
            {   "Housing",              "The:   icon indicates the <y>Housing Space</y> you have." },
            {   "Food",                 "The:   icon indicates the <g>Food</g> you have." },
            {   "Wood",                 "The:   icon indicates the <g>Wood</g> you have." },
            {   "Stone",                "The:   icon indicates the <g>Stone</g> you have." },
            {   "Gold",                 "The:   icon indicates the <y>Gold</y> you have." },
            {   "Population",           "Each <g>Keeno</g> requires 1 <y>Housing Space</y>.(   )" },
            {   "Hunger1",              "Each <g>Keeno</g> requires 1 <y>Food</y> (   ) a day to survive." },
            {   "Hunger2",              "Make sure to <y>1 Food per Keeno</y> by the end of the day." },
            {   "Hunger3",              "Or your <g>Keeno</g> will <r>Die of hunger</r>!" },
            {   "Building",             "To increase   , buy a <y>Tent or House Blueprint</y> at the <y>Shop</y>." },
            {   "Blueprint1",           "<y>Press E</y> to <y>place</y> the <b>Blueprint</b> on the highlighted tile." },
            {   "Blueprint2",           "Only <g>Keeno</g> working at the <y>Builders Cabin<y/> can build." },
            {   "Building Resources",   "Approach a building to see the <y>Required Materials</y>." },
            {   "BuildersCabin",        "Make sure to assign a <g>Keeno</g> to work at the <y>Builders Cabin</y>." },
            {   "Bell",                 "If all your <g>Keeno</g> are busy, try using the <y>Bell</y>." },
            {   "10 Keeno Challenge",   "Try to make it to <y>10 Keeno</y>." },
            {   "10 Keeno Milestone",   "Well done, you made it to <y>10 Keeno</y>!" },
            {   "10 Keeno Milestone2",  "Can you make it to <y>25 Keeno</y>?" },
            {   "Houses1",              "With <y>Stone</y> you can build more useful buildings." },
            {   "Houses2",              "Discover what other <b>Blueprints</b> the <y>Shop</y> sells!" },
            {   "Shop Shuffle",         "<y>Press Q</y> on the <y>Shop's</y> <b>Blueprint</b> to check out the rest." },
            {   "25 Keeno Milestone",   "Well done, you made it to <y>25</y> <g>Keeno</g>!" },
            {   "25 Keeno Milestone2",  "Finally, prove you can <y>sustain 100 Keeno</y> for a whole day." }




        };

        public static readonly Dictionary<string, string> EndOfDayText = new Dictionary<string, string>()
        {
            {   "EndOfDay",             "You ended the day with<y>" + ResourceTracker.GetAmount(ResourceType.Keeno).ToString() + "</y>Keeno" },
        };
        public TextManager() 
        {
            _inGamePosition = new Vector2(180, Globals.ScreenHeight - 50);
            _timer = 0f;
            _timerReset = 6f;
            foreach (var pair in TutorialText)
            {
                string key = pair.Key;
                string text = pair.Value;

                Tutorials.Add(key, new TypewriterText(_inGamePosition, text));
            }
        }

        public void SetActive(string key)
        {

            foreach (var item in Tutorials)
            {
                item.Value.Reset();
            }

            Tutorials[key].SetActive();
            _currIndex = TutorialText.Keys.ToList().IndexOf(key);


            switch (key)
            {
                case "Resource Interact":
                case "Housing":
                case "Food":
                case "Wood":
                case "Stone":
                case "Gold":
                case "Population":
                case "Hunger1":
                case "Hunger2":

                case "Blueprint1":
                case "Blueprint2":
                case "BuildersCabin":
                case "10 Keeno Milestone":
                case "Houses1":
                case "Houses2":
                case "25 Keeno Milestone":

                    _timer = _timerReset;
                    break;
            }
        }
        public void Update()
        {
            if (_timer > 0f)
                _timer -= Globals.DeltaTime;
            if (_timer < 0f)
            {
                if (_currIndex > TutorialText.Keys.ToList().IndexOf("First Follower") 
                    && _currIndex < TutorialText.Keys.ToList().IndexOf("Gold")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Resource Interact")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Population")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Hunger1")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Hunger2")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Blueprint1")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Blueprint2")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("BuildersCabin")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("10 Keeno Milestone")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Houses1")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("Houses2")
                    || _currIndex == TutorialText.Keys.ToList().IndexOf("25 Keeno Milestone")

                    )

                {
                    _currIndex++;
                    string nextKey = TutorialText.Keys.ToList()[_currIndex];
                    SetActive(nextKey);
                }
                if (_currIndex == TutorialText.Keys.ToList().IndexOf("Gold") && _timer < 0)
                    Reset();
            }
                
            foreach (var item in Tutorials)
            {
                item.Value.Update();
            }
        }
        public void Reset()
        {
            _timer = 0f;
            foreach (var item in Tutorials)
            {
                item.Value.Reset();
            }
        }
        public void CompleteReset()
        {
            foreach (var item in Tutorials)
            {
                item.Value.Reset();
            }
            Tutorials["Controls"].SetActive();
        }
        public void Start()
        {
            if (_currIndex > 0)
                return;
            Tutorials["Controls"].SetActive();
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (var item in Tutorials)
            {
                item.Value.Draw(sb);
                if (Tutorials["Housing"].IsActive)
                    sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X+55,(int)_inGamePosition.Y+2), new Point(32, 32)), Color.White);
                if (Tutorials["Population"].IsActive) 
                    sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 519, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                if (Tutorials["Building"].IsActive)
                    sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 161, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                if (Tutorials["Food"].IsActive)
                    sb.Draw(Assets.UIFoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                if (Tutorials["Hunger1"].IsActive) 
                    sb.Draw(Assets.UIFoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 392, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                if (Tutorials["Wood"].IsActive)
                    sb.Draw(Assets.UIWoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                if (Tutorials["Stone"].IsActive)
                    sb.Draw(Assets.UIStoneIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                if (Tutorials["Gold"].IsActive)
                    sb.Draw(Assets.UIGoldIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
            }
        }
    }
}
