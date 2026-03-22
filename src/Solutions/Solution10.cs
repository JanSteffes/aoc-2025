using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using System.Diagnostics;
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
            var machines = ParseUtils.ParseIntoLines(inputData).Select(Machine.FromString).ToList();

            //var results = new ConcurrentBag<int>();
            //Parallel.ForEach(machines, machine =>
            //{
            //    var result = machine.GetShortestCombinationBfsWithJolate();
            //    results.Add(result);
            //});
            //var result = results.Sum();

            var result = 0L;
            foreach (var machine in machines)
            {
                result += machine.GetShortestCombinationBfsWithJolate();
            }
            return result.ToString();
        }
    }

    class Machine
    {
        private const char OffChar = '.';
        private const char OnChar = '#';

        public HashSet<int> TargetLightsToBeOn { get; }

        public int[] TargetJoltageValues { get; }

        public List<Button> Buttons { get; }

        public int NumberOfLights { get; }

        private readonly object LockObject = new object();

        public Machine(int numberOfLights, HashSet<int> targetState, List<Button> buttons) : this(numberOfLights, targetState, buttons, Array.Empty<int>())
        {
        }

        public Machine(int numberOfLights, HashSet<int> targetState, List<Button> buttons, int[] targetJoltage)
        {
            NumberOfLights = numberOfLights;
            TargetLightsToBeOn = targetState;
            Buttons = buttons;
            TargetJoltageValues = targetJoltage;
        }

        public static Machine FromString(string machineConfiguration)
        {
            var splitByEmptySpace = machineConfiguration.Split(' ');
            var targetStatePart = splitByEmptySpace.First();
            var targetStateSymbols = targetStatePart.Skip(1).SkipLast(1).ToList();
            var targetLightsToBeOn = targetStateSymbols.Select((value, index) => (ShouldBeOn: value == OnChar, Index: index)).Where(q => q.ShouldBeOn).Select(q => q.Index).ToHashSet();

            var buttonsPart = splitByEmptySpace.Skip(1).SkipLast(1).ToList();
            var buttons = buttonsPart.Select(Button.FromString).ToList();

            var joltagePart = splitByEmptySpace.Last();
            var targetJoltage = joltagePart.Trim('{').Trim('}').Split(",").Select(int.Parse).ToArray();

            return new Machine(targetStateSymbols.Count(), targetLightsToBeOn, buttons, targetJoltage);
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



        internal int GetShortestCombinationBfsWithJolate()
        {
            var statesToCheck = new List<State>{
                new State(NumberOfLights)
            };
            var presses = 0;
            while (true)
            {
                var newStates = new List<State>();
                presses++;
                if (!statesToCheck.Any())
                {
                    throw new InvalidOperationException("No new states to check, something went wrong!");
                }
                Debug.WriteLine($"Machine {this} - {presses} presses and {statesToCheck.Count} states to check..");
                var sw = Stopwatch.StartNew();
                foreach (var state in statesToCheck)
                {
                    if (this.ToString().StartsWith("[...#.]") && state.PressedButtons.SequenceEqual([0, 0, 1, 1, 1, 1, 1]))
                    {
                        state.ToWatch = true;
                    }

                    // press every button
                    for (var buttonIndex = 0; buttonIndex < Buttons.Count; buttonIndex++)
                    {
                        var button = Buttons[buttonIndex];
                        var newState = button.PressButton(state, buttonIndex);

                        // check if wanted state
                        if (newState.ActiveIndicatorLights.SetEquals(TargetLightsToBeOn) &&
                            newState.JoltageState.SequenceEqual(TargetJoltageValues))
                        {
                            return presses;
                        }
                        // check if joltage can lead to abortion of current state
                        if (JoltageTooHigh(newState.JoltageState))
                        {
                            continue;
                        }
                        //// check if already in newStates                   
                        //if (!newStates.Any(n => n.ActiveIndicatorLights.SetEquals(newState.ActiveIndicatorLights) && n.JoltageState.SequenceEqual(newState.JoltageState)))
                        //{
                        //    // add to new States
                        newStates.Add(newState);
                        //}
                    }
                }
                sw.Stop();
                Debug.WriteLine($"Took: {sw.Elapsed}");
                statesToCheck = newStates;
            }
        }

        private bool JoltageTooHigh(int[] joltageState)
        {
            for (var index = 0; index < joltageState.Length; index++)
            {
                if (joltageState[index] > TargetJoltageValues[index])
                {
                    return true;
                }
            }
            return false;
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

        public override string ToString()
        {
            return "[" + string.Join("", Enumerable.Repeat(0, NumberOfLights).Select((_, index) => TargetLightsToBeOn.Contains(index) ? "#" : ".")) + "], {" + string.Join(",", TargetJoltageValues) + "}";
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

        internal State PressButton(State state, int buttonIndex)
        {
            var newIndicatorLightsState = PressButton(state.ActiveIndicatorLights);
            var newJoltageState = CalculateNewJoltageState(state.JoltageState);
            var pressedButtons = new List<int>(state.PressedButtons);
            pressedButtons.Add(buttonIndex);
            var newState = new State(newIndicatorLightsState, newJoltageState, pressedButtons);
            if (state.ToWatch)
            {
                newState.ToWatch = true;
            }
            return newState;
        }

        private int[] CalculateNewJoltageState(int[] joltageState)
        {
            var newStateArray = new int[joltageState.Length];
            joltageState.CopyTo(newStateArray, 0);
            foreach (var indicatorLight in LightsToTrigger)
            {
                newStateArray[indicatorLight] = newStateArray[indicatorLight] + 1;
            }
            return newStateArray;
        }
    }

    internal class State
    {
        public HashSet<int> ActiveIndicatorLights { get; set; }

        public int[] JoltageState { get; set; }

        public List<int> PressedButtons { get; set; }

        public bool ToWatch { get; set; }

        public State(int joltageStates)
        {
            ActiveIndicatorLights = new HashSet<int>();
            JoltageState = new int[joltageStates];
            PressedButtons = [];
        }

        public State(HashSet<int> indicatorLights, int[] joltageStates, List<int> pressedButtons)
        {
            ActiveIndicatorLights = indicatorLights;
            JoltageState = joltageStates;
            PressedButtons = pressedButtons;
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", PressedButtons) + "} -> [" + string.Join(", ", ActiveIndicatorLights) + "], [" + string.Join(",", JoltageState) + "]";
        }
    }
}