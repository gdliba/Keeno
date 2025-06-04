using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Keeno.GameCode
{
    enum TextState
    {
        InGame,
        EndOfDay,
        GameOver
    }
    /// <summary>
    /// Class that handles what text (of type TypewriterText) is displayed on screen.
    /// </summary>
    class TextManager
    {
        private Vector2 _inGamePosition, _endOfDayPosition;
        public static Dictionary<string,TypewriterText> InGame = new ();
        public static Dictionary<string, TypewriterText> EndOfDay = new();
        public static Dictionary<string, TypewriterText> GameOver = new();


        private float _timer, _timerReset;
        private int _currIndex =-1;
        private int _lastInGameIndex, _counter;
        private TextState _state;

        /// <summary>
        /// Dictionary with all the text displayed during gameplay (includes colour tags).
        /// </summary>
        public static readonly Dictionary<string, string> InGameText = new Dictionary<string, string>()
        {
            {   "Controls",             "Use <y>W</y>/<g>A</g>/<r>S</r>/<b>D</b> to move." },
            {   "First Keeno",          "Press <y>Q</y> while near an idle <g>Keeno</g> to make it <y>follow you</y>." },
            {   "Resource Interact",    "<y>Hold E</y> when near a Resource to <y>Harvest it</y>." },
            {   "Resource Debris",      "Once depleted, resources leave debris.<y>Hold X</y> to <r>remove</r> it." },
            {   "Buy Keeno",            "Go to the <y>Town Centre</y> to grow a <g>Keeno</g>." },
            {   "First Follower",       "<y>Hold Q</y> when near a Resource to assign a <g>Keeno</g> work. " },
            {   "Keeno Work",           "<g>Keeno</g> will work until the resource is depleted." },
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
        /// <summary>
        /// Constructor for TextManager. Sets default values and populates "InGameText" Dictionary.
        /// Takes in a state, for flexibilty in use. Not really necessary in this case.
        /// </summary>
        public TextManager(TextState state) 
        {
            _state = state;
            _counter = 0;
            _timer = 0f;
            _timerReset = 6f;

            _inGamePosition = new Vector2(180, Globals.ScreenHeight - 50);
            _endOfDayPosition = new Vector2(100, 64);

            foreach (var pair in InGameText)
            {
                string key = pair.Key;
                string text = pair.Value;

                InGame.Add(key, new TypewriterText(_inGamePosition, text));
            }
        }
        /// <summary>
        /// Switches TextManager state to "GameOver" thus drawing the GameOver related text.
        /// There is no Dictionary for this one, as it takes in values that change throughout
        /// gameplay and has to update. Instead the strings that should appear on screen are 
        /// created when this method is called.
        /// </summary>
        public void SwitchToGameOver()
        {
            // Clear the list, as, the next time this method is called, the Key
            // for the entry in the Dictionary will be the same and will crash
            // the program by attempting to add the same enty twice.
            GameOver.Clear();
            _state = TextState.GameOver;
            int offset = 64;
            _counter++;

            #region Create Strings and set them Active
            string gameoverText = "<y>Well done! </y>You have proven your leadership. The <g>Keeno</g> are safe in your capable hands!";
            string totalDays =  "You reached the goal in: <y>"  + _counter + " Days</y>";
            string totalFood =  "You collected a total of: <y>" + ResourceTracker.GrandTotalFood.ToString()     + " Food</y>";
            string totalWood =  "You collected a total of: <y>" + ResourceTracker.GrandTotalWood.ToString()     + " Wood</y>";
            string totalStone = "You collected a total of: <y>" + ResourceTracker.GrandTotalStone.ToString()    + " Stone</y>";
            string totalGold =  "You collected a total of: <y>" + ResourceTracker.GrandTotalGold.ToString()     + " Gold</y>";

            var gOtemp = new TypewriterText(_endOfDayPosition, gameoverText);
            GameOver.Add("GameOver", gOtemp);
            GameOver["GameOver"].SetActive();

            Vector2 dayCountPos = new Vector2(_endOfDayPosition.X, _endOfDayPosition.Y + offset);
            var dayTemp = new TypewriterText(dayCountPos, totalDays);
            GameOver.Add("totalDays", dayTemp);
            GameOver["totalDays"].SetActive();

            Vector2 foodPos = new Vector2(_endOfDayPosition.X, dayCountPos.Y + offset);
            var foodTemp = new TypewriterText(foodPos, totalFood);
            GameOver.Add("totalFood", foodTemp);
            GameOver["totalFood"].SetActive();

            Vector2 woodPos = new Vector2(_endOfDayPosition.X, foodPos.Y + offset);
            var woodTemp = new TypewriterText(woodPos, totalWood);
            GameOver.Add("totalWood", woodTemp);
            GameOver["totalWood"].SetActive();

            Vector2 stonePos = new Vector2(_endOfDayPosition.X, woodPos.Y + offset);
            var stoneTemp = new TypewriterText(stonePos, totalStone);
            GameOver.Add("totalStone", stoneTemp);
            GameOver["totalStone"].SetActive();

            Vector2 goldPos = new Vector2(_endOfDayPosition.X, stonePos.Y + offset);
            var goldTemp = new TypewriterText(goldPos, totalGold);
            GameOver.Add("totalGold", goldTemp);
            GameOver["totalGold"].SetActive();
            #endregion
        }
        /// <summary>
        /// Switches TextManager state to "EndOfDay" thus drawing the EndOfDay related text.
        /// There is no Dictionary for this one, as it takes in values that change throughout
        /// gameplay and has to update. Instead the strings that should appear on screen are 
        /// created when this method is called.
        /// </summary>
        public void SwitchToEndOfDay(int keenosThatStarved)
        {
            // Clear the list, as, the next time this method is called, the Key
            // for the entry in the Dictionary will be the same and will crash
            // the program by attempting to add the same enty twice.
            GameOver.Clear();
            EndOfDay.Clear();
            // Remember where you left off so that you can display the text again
            // at the start of the next day
            _lastInGameIndex = _currIndex;
            _state = TextState.EndOfDay;
            _counter++;

            #region Create Strings and set them Active
            string day = "<y>Day</y>: " + _counter.ToString();
            int keenoAmount = ResourceTracker.GetAmount(ResourceType.Keeno);
            string endOfDayText = "You ended the day with <y>" + keenoAmount + "</y> Keeno";

            if (keenosThatStarved > 0)
            {
                string congratulations = "";
                string but = "";
                if (keenoAmount >= 100)
                    congratulations += " <y>Really close... Survive a day with 100 Keeno, without any </y><r>starving</r>.";
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

            string tempName = _counter.ToString();
            var dayText = new TypewriterText(_endOfDayPosition, day);
            EndOfDay.Add(tempName, dayText);
            EndOfDay[tempName].SetActive();
            #endregion
        }
        /// <summary>
        /// Switches TextManager state to "InGame" and remembers where it left off,
        /// thus displaying the last line of text it was displaying the day before.
        /// </summary>
        public void SwitchToInGame()
        {
            _state = TextState.InGame;
            if (_lastInGameIndex ==-1)
                _lastInGameIndex++;
            string lastInGameText = InGameText.Keys.ToList()[_lastInGameIndex];
            SetActive(lastInGameText);
        }
        /// <summary>
        /// Sets the text with the key given to Active (see TypeWriterText.SetActive).
        /// Some lines of text will have a timer associated to them and will display 
        /// another line of text at the end of said timer. Update will take care of that.
        /// </summary>
        public void SetActive(string key)
        {
            switch (_state)
            {
                case TextState.InGame:
                    // Remove what isn't currently active from screen.
                    // Thus only displaying the Active string.
                    foreach (var item in InGame)
                    {
                        item.Value.Reset();
                    }

                    InGame[key].SetActive();
                    _currIndex = InGameText.Keys.ToList().IndexOf(key);

                    // The Dictionary entries with these keys have timer associated
                    // to them and will display another line of text after said timer.
                    switch (key)
                    {
                        case "Resource Interact":
                        case "First Follower":
                        case "Housing":
                        case "Food":
                        case "Wood":
                        case "Stone":
                        case "Gold":
                        case "Population":
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

                default:
                    break;
            }
            
        }
        /// <summary>
        /// Updates all text depending on state. 
        /// If any text has a timer associated to it, count down.
        /// </summary>
        public void Update()
        {
            switch (_state)
            {
                case TextState.InGame:
                    if (_timer > 0f)
                        _timer -= Globals.DeltaTime;
                    if (_timer < 0f)
                    {
                        // If the current text that is active is one of these:
                        if (   _currIndex >  InGameText.Keys.ToList().IndexOf("Keeno Work")
                            && _currIndex <  InGameText.Keys.ToList().IndexOf("Gold")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("First Follower")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Resource Interact")
                            || _currIndex == InGameText.Keys.ToList().IndexOf("Population")
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
                            // Once the timer runs out, display the next entry in the Dictionary.
                            _currIndex++;
                            string nextKey = InGameText.Keys.ToList()[_currIndex];
                            SetActive(nextKey);
                        }
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
                case TextState.GameOver:

                    foreach (var item in GameOver)
                    {
                        item.Value.Update();
                    }
                    break;
            }
            
        }
        /// <summary>
        /// Resets counter and timer to 0.
        /// Resets all TypeWriterTexts.
        /// </summary>
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
            foreach (var item in GameOver)
            {
                item.Value.Reset();
            }
        }
        /// <summary>
        /// When the GameOver Screen has been reached, this harder reset is called.
        /// </summary>
        public void CompleteReset()
        {
            Reset();
            SwitchToInGame();
            _lastInGameIndex = -1;
            Start();
        }
        /// <summary>
        /// Initial Text on screen
        /// </summary>
        public void Start()
        {
            if (_currIndex > 0)
                return;
            InGame["Controls"].SetActive();
        }
        /// <summary>
        /// Simple draw method for the class. 
        /// Calls the individual draw methods of the TypeWriterTexts.
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            switch (_state)
            {
                case TextState.InGame:
                    foreach (var item in InGame)
                    {
                        item.Value.Draw(sb);

                        #region Icons

                        // Given that I haven't integrated a built in method in the
                        // TypeWriterText to display icons (due to time constraint)
                        // I'm manually adding them here. A little tedious,
                        // but it adds a lot to the charm/aesthetic of the initial
                        // tutorialisation of the game.

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
                case TextState.GameOver:
                    foreach (var item in GameOver)
                    {
                        item.Value.Draw(sb);
                    }
                    break;
            }
        }
    }
}
