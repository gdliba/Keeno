using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;




namespace Keeno.GameCode
{
    enum TextState
    {
        InGame,
        EndOfDay
    }
    class TextManager
    {
        private Vector2 _inGamePosition, _endOfDayPosition;
        public static Dictionary<string,TypewriterText> InGame = new ();
        public static Dictionary<string, TypewriterText> EndOfDay = new();

        private float _timer, _timerReset;
        private int _currIndex =-1;
        private int _lastInGameIndex, _counter;
        private TextState _state;
        public static readonly Dictionary<string, string> InGameText = new Dictionary<string, string>()
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
            {   "25 Keeno Milestone",   "Well done, you made it to <y>25 Keeno</y>!" },
            {   "25 Keeno Milestone2",  "Finally, prove you can <y>sustain 100 Keeno</y> for a whole day." },
            {   "100 Keeno Milestone",  "Well done, you made it to <y>100 Keeno</y>!" },
            {   "100 Keeno Milestone2", "Now survive a day with <y>100 Keeno</y>, don't let any <r>starve!</r>" },
            {   "100 Keeno Milestone Reset",  "Ouch, you were so close... <y>sustain 100 Keeno for 1 day</y>." },






        };

        public static readonly Dictionary<string, string> EndOfDayText = new Dictionary<string, string>()
        {
            {   "EndOfDay",            " " },
            {   "Test1234",             "You ended the day with<y>" + ResourceTracker.GetAmount(ResourceType.Keeno).ToString() + "</y>Keeno" }

        };
        public TextManager(TextState state) 
        {
            _state = state;
            _counter = 0;

            _inGamePosition = new Vector2(180, Globals.ScreenHeight - 50);
            _timer = 0f;
            _timerReset = 6f;
            foreach (var pair in InGameText)
            {
                string key = pair.Key;
                string text = pair.Value;

                InGame.Add(key, new TypewriterText(_inGamePosition, text));
            }

            _inGamePosition = new Vector2(180, Globals.ScreenHeight - 50);
            _timer = 0f;
            _timerReset = 6f;
            foreach (var pair in EndOfDayText)
            {
                string key = pair.Key;
                string text = pair.Value;

                EndOfDay.Add(key, new TypewriterText(_inGamePosition, text));
            }
            
        }
        public void SwitchToEndOfDay(int keenosThatStarved)
        {
            _lastInGameIndex = _currIndex;
            _state = TextState.EndOfDay;
            EndOfDay.Clear();

            string day = "day" + _counter++.ToString();
            int keenoAmount = ResourceTracker.GetAmount(ResourceType.Keeno);
            string endOfDayText = "You ended the day with <y>" + keenoAmount + "</y> Keeno";
            if (keenosThatStarved > 0)
            {
                string congratulations = "";
                string but = "";
                if (keenoAmount >= 100)
                    congratulations += " <y>Well Done!!!</y>";
                if (keenoAmount != 0)
                    but += " but";
                if (keenosThatStarved == 1) 
                    endOfDayText += "," + but + " <y>" + keenosThatStarved + "</y> has <r>Starved</r>!" + congratulations;
                else
                    endOfDayText += "," + but + " <y>" + keenosThatStarved + "</y> have <r>Starved</r>!" + congratulations;
            }
            else
                endOfDayText += ".";
                var temp = new TypewriterText(_inGamePosition, endOfDayText);
            EndOfDay.Add(day,temp);
            EndOfDay[day].SetActive();
            //temp.SetActive();
            //EndOfDay["EndOfDay"].SetActive();
        }
        public void SwitchToInGame()
        {
            _state = TextState.InGame;
            if (_lastInGameIndex ==-1)
                _lastInGameIndex++;
            string lastInGameText = InGameText.Keys.ToList()[_lastInGameIndex];
            SetActive(lastInGameText);
        }
        public void SetActive(string key)
        {


            switch (_state)
            {
                case TextState.InGame:
                    foreach (var item in InGame)
                    {
                        item.Value.Reset();
                    }

                    InGame[key].SetActive();
                    _currIndex = InGameText.Keys.ToList().IndexOf(key);


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
                        case "100 Keeno Milestone":

                            _timer = _timerReset;
                            break;
                    }
                    break;

                case TextState.EndOfDay:
                    break;
            }
            
        }
        public void Update()
        {

            switch (_state)
            {
                case TextState.InGame:
                    if (_timer > 0f)
                        _timer -= Globals.DeltaTime;
                    if (_timer < 0f)
                    {
                        if (_currIndex > InGameText.Keys.ToList().IndexOf("First Follower")
                            && _currIndex < InGameText.Keys.ToList().IndexOf("Gold")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Resource Interact")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Population")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Hunger1")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Hunger2")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Blueprint1")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Blueprint2")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("BuildersCabin")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("10 Keeno Milestone")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Houses1")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Houses2")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("25 Keeno Milestone")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("100 Keeno Milestone")
                            )

                        {
                            _currIndex++;
                            string nextKey = InGameText.Keys.ToList()[_currIndex];
                            SetActive(nextKey);
                        }
                        if (_currIndex == InGameText.Keys.ToList().IndexOf("Gold") && _timer < 0)
                            Reset();
                    }

                    foreach (var item in InGame)
                    {
                        item.Value.Update();
                    }
                    break;

                case TextState.EndOfDay:

                    foreach (var item in EndOfDay)
                    {
                        item.Value.Update();
                    }
                    break;
            }
            
        }
        public void Reset()
        {
            _counter = 0;
            _timer = 0f;

            foreach (var item in InGame)
            {
                item.Value.Reset();
            }
            foreach (var item in EndOfDay)
            {
                item.Value.Reset();
            }
        }
        public void CompleteReset()
        {
            Reset();
            InGame["Controls"].SetActive();
        }
        public void Start()
        {
            if (_currIndex > 0)
                return;
            InGame["Controls"].SetActive();
        }
        public void Draw(SpriteBatch sb)
        {
            switch (_state)
            {
                case TextState.InGame:
                    foreach (var item in InGame)
                    {
                        item.Value.Draw(sb);
                        #region Icons
                        // Housing
                        if (InGame["Housing"].IsActive)
                            sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                        if (InGame["Population"].IsActive)
                            sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 519, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                        if (InGame["Building"].IsActive)
                            sb.Draw(Assets.UIHousingIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 161, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                        // Food
                        if (InGame["Food"].IsActive)
                            sb.Draw(Assets.UIFoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                        if (InGame["Hunger1"].IsActive)
                            sb.Draw(Assets.UIFoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 392, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                        // Wood
                        if (InGame["Wood"].IsActive)
                            sb.Draw(Assets.UIWoodIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                        // Stone
                        if (InGame["Stone"].IsActive)
                            sb.Draw(Assets.UIStoneIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);

                        // Gold
                        if (InGame["Gold"].IsActive)
                            sb.Draw(Assets.UIGoldIconTxr, new Rectangle(new Point((int)_inGamePosition.X + 55, (int)_inGamePosition.Y + 2), new Point(32, 32)), Color.White);
                        #endregion
                    }
                    break;

                case TextState.EndOfDay:
                    foreach (var item in EndOfDay)
                    {
                        item.Value.Draw(sb);
                    }
                    break;
            }
        }
    }
}
