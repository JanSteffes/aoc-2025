using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using System.Text.RegularExpressions;

namespace aoc_2025.Solutions
{
    public class Solution10 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var machines = ParseUtils.ParseIntoLines(inputData).Select(Machine.FromString).ToList();
            var result = 0L;
            foreach (var machine in machines)
            {
                result += machine.GetShortestCombinationBfs();
            }
            return result.ToString();
        }

        public string RunPartB(string inputData)
        {
            throw new NotImplementedException();
        }
    }

    class Machine
    {
        private const char OffChar = '.';
        private const char OnChar = '#';

        public HashSet<int> TargetLightsToBeOn { get; }

        public List<Button> Buttons { get; }

        public int NumberOfLights { get; }

        private readonly object LockObject = new object();

        public Machine(int numberOfLights, HashSet<int> targetState, List<Button> buttons)
        {
            NumberOfLights = numberOfLights;
            TargetLightsToBeOn = targetState;
            Buttons = buttons;
        }

        public static Machine FromString(string machineConfiguration)
        {
            var splitByEmptySpace = machineConfiguration.Split(' ');
            var targetStatePart = splitByEmptySpace.First();
            var targetStateSymbols = targetStatePart.Skip(1).SkipLast(1).ToList();
            var targetLightsToBeOn = targetStateSymbols.Select((value, index) => (ShouldBeOn: value == OnChar, Index: index)).Where(q => q.ShouldBeOn).Select(q => q.Index).ToHashSet();

            var joltagePart = splitByEmptySpace.Last();
            var buttonsPart = splitByEmptySpace.Skip(1).SkipLast(1).ToList();
            var buttons = buttonsPart.Select(Button.FromString).ToList();


            return new Machine(targetStateSymbols.Count(), targetLightsToBeOn, buttons);
        }

        internal int GetShortestCombinationBfs()
        {
            var statesToCheck = new List<HashSet<int>> {
                new HashSet<int>()
            };
            var presses = 0;
            while (true)
            {
                var newStates = new List<HashSet<int>>();
                presses++;
                foreach (var state in statesToCheck)
                {
                    // press every button
                    foreach (var button in Buttons)
                    {
                        var newState = button.PressButton(state);
                        // check if wanted state
                        if (newState.SetEquals(TargetLightsToBeOn))
                        {
                            return presses;
                        }
                        // check if already in newStates                   
                        if (!newStates.Any(n => n.SetEquals(newState)))
                        {
                            // add to new States
                            newStates.Add(newState);
                        }
                    }
                }
                statesToCheck = newStates;
            }
        }

        /// <summary>
        /// Don't use! Super slow because stupid!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal int GetShortestCombinationDfs()
        {
            // create all possible combinations from every button
            // abort if pattern already occured
            var shortestCount = int.MaxValue;
            foreach (var button in Buttons)
            {
                var startState = new HashSet<int>();
                var newState = button.PressButton(startState);
                // TODO: rewrite: don't use "dfs", but "bfs", e.g. don't follow path A till the end, but take every path at the same time.
                // e.g. press button a, b,c,d... and keep those states. Then, press every button for every state and check if done -> return all at once (SHOULD be way faster, but may consumes more memory?)
                var combinations = GetShortestCombinationDfsRecursive([startState, newState], 1, ref shortestCount);
                if (combinations == -1)
                {
                    continue;
                }
                lock (LockObject)
                {
                    if (shortestCount > combinations)
                    {
                        shortestCount = combinations;
                    }
                }
            }
            if (shortestCount == int.MaxValue)
            {
                throw new Exception("Failed to get any valid combination!");
            }
            return shortestCount;
        }

        private int GetShortestCombinationDfsRecursive(List<HashSet<int>> previousStates, int currentNumberOfPresses, ref int overallShortestCount)
        {
            var currentShortestCount = int.MaxValue;
            if (previousStates.Count > 100)
            {
                return -1;
            }
            foreach (var button in Buttons)
            {
                var currentState = new HashSet<int>(previousStates.Last());
                var newState = button.PressButton(currentState);
                var matchesPreviousState = previousStates.Any(q => q.SetEquals(newState));
                if (matchesPreviousState)
                {
                    continue;
                }
                if (TargetLightsToBeOn.SetEquals(newState))
                {
                    return currentNumberOfPresses + 1;
                }
                lock (LockObject)
                {
                    if (overallShortestCount <= currentNumberOfPresses + 1)
                    {
                        // there's already some shorter path
                        continue;
                    }
                }
                var newPreviousStates = new List<HashSet<int>>(previousStates);
                newPreviousStates.Add(newState);
                var combinations = GetShortestCombinationDfsRecursive(newPreviousStates, currentNumberOfPresses + 1, ref overallShortestCount);
                if (combinations == -1)
                {
                    continue;
                }
                if (currentShortestCount > combinations)
                {
                    currentShortestCount = combinations;
                }
            }
            return currentShortestCount;
        }
    }

    class Light
    {
        public bool On { get; set; }

        public bool Off => !On;

        public int Index { get; }

        public Light(int index) : this(index, false)
        {

        }

        public Light(int index, bool state)
        {
            On = state;
            Index = index;
        }

        public void ChangeState()
        {
            On = !On;
        }
    }

    class Button
    {
        private const char ButtonLightsSeperator = ',';
        private const string RegexString = "(\\d+)";

        public HashSet<int> LightsToTrigger { get; }

        public Button(HashSet<int> ligthsToTrigger)
        {
            LightsToTrigger = ligthsToTrigger;
        }
        internal static Button FromString(string buttonString)
        {
            var numbers = new Regex(RegexString).Matches(buttonString).Select(v => int.Parse(v.Value)).ToHashSet();
            return new Button(numbers);
        }

        public HashSet<int> PressButton(HashSet<int> currentState)
        {
            var newState = new HashSet<int>(currentState);
            foreach (var lightToTrigger in LightsToTrigger)
            {
                if (newState.Contains(lightToTrigger))
                {
                    newState.Remove(lightToTrigger);
                }
                else
                {
                    newState.Add(lightToTrigger);
                }
            }
            return newState;
        }

        public override string ToString()
        {
            return "[" + string.Join(",", LightsToTrigger) + "]";
        }


    }
}