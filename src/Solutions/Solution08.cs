using aoc_2025.Interfaces;
using aoc_2025.SolutionUtils;
using System.Collections.Concurrent;

namespace aoc_2025.Solutions
{
    public class Solution08 : ISolution
    {
        private const int TestPointAmount = 20;
        private const int TestPairsToTake = 10;
        private const int RunPairsToTake = 1000;

        public string RunPartA(string inputData)
        {
            var pointLines = ParseUtils.ParseIntoLines(inputData);
            var points = pointLines.Select(Point3D.FromString).ToArray();

            var distances = CalculateDistances(points);

            var circuites = new List<Circuit>();
            var firstCircuit = new Circuit();
            var firstDistanceSet = distances.First();
            firstCircuit.AddDistanceSetPoints(firstDistanceSet);
            circuites.Add(firstCircuit);

            var pairsToTake = points.Length > TestPointAmount ? RunPairsToTake : TestPairsToTake;
            CalculateCircuits(distances.Skip(1), circuites, pairsToTake, out _);

            var result = CalculateProductOfThreeBiggestCircuits(circuites);

            return result.ToString();
        }

        public string RunPartB(string inputData)
        {
            var pointLines = ParseUtils.ParseIntoLines(inputData);
            var points = pointLines.Select(Point3D.FromString).ToArray();

            var distances = CalculateDistances(points);

            var circuites = new List<Circuit>();
            var firstCircuit = new Circuit();
            var firstDistanceSet = distances.First();
            firstCircuit.AddDistanceSetPoints(firstDistanceSet);
            circuites.Add(firstCircuit);

            CalculateCircuits(distances.Skip(1), circuites, int.MaxValue, out var lastMergedSet);

            var result = lastMergedSet.FirstPoint.X * lastMergedSet.SecondPoint.X;

            return result.ToString();
        }

        private static long CalculateProductOfThreeBiggestCircuits(List<Circuit> circuites)
        {
            var orderdByCount = circuites.OrderByDescending(c => c.Points.Count).Select(c => c.Points.Count).ToList();
            var threeLargest = orderdByCount.Take(3).ToList();
            var result = (long)threeLargest[0] * (long)threeLargest[1] * (long)threeLargest[2];
            return result;
        }

        /// <summary>
        /// Calculate distances between each point, remove duplicates and order by distance (ASC).
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private List<DistanceSet> CalculateDistances(Point3D[] points)
        {
            var distanceSetSets = new ConcurrentBag<List<DistanceSet>>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(points, parallelOptions, point =>
            {
                var mySets = new List<DistanceSet>();
                distanceSetSets.Add(mySets);
                foreach (var otherPoint in points)
                {
                    if (otherPoint == point)
                    {
                        continue;
                    }
                    var distance = Distance(point, otherPoint);
                    var distanceSet = new DistanceSet(point, otherPoint, distance);
                    mySets.Add(distanceSet);
                }
            });
            var flattened = distanceSetSets.SelectMany(q => q.ToList()).ToArray();
            var distincted = flattened.Distinct().ToArray();
            var orderdByDistance = distincted.OrderBy(d => d.Distance).ToList();
            return orderdByDistance;
        }

        private static void CalculateCircuits(IEnumerable<DistanceSet> distances, List<Circuit> circuites, int pairsToTake, out DistanceSet lastMergedSet)
        {
            var connectionsMade = 0;
            var isTest = pairsToTake == TestPairsToTake;
            lastMergedSet = distances.First();
            foreach (var distanceSet in distances.Take(pairsToTake))
            {
                var matchingCircuits = circuites.Where(c => c.PointOfDistanceSetInCircuit(distanceSet)).ToList();
                if (isTest && matchingCircuits.Any())
                {
                    var added = matchingCircuits.First().AddDistanceSetPoints(distanceSet);
                    if (added)
                    {
                        connectionsMade++;
                    }
                }
                else if (matchingCircuits.Any())
                {
                    // add to first matching
                    var firstCircuit = matchingCircuits.First();
                    var added = firstCircuit.AddDistanceSetPoints(distanceSet);
                    // if there are others, merge them
                    foreach (var circuit in matchingCircuits.Skip(1))
                    {
                        // stupidly add points of other circuits to the first
                        foreach (var point in circuit.Points)
                        {
                            added |= firstCircuit.Points.Add(point);
                        }
                        // and remove current from list of circuits
                        circuites.Remove(circuit);
                    }
                    if (added)
                    {
                        connectionsMade++;
                        lastMergedSet = distanceSet;
                    }
                }
                else
                {
                    var newCircuit = new Circuit();
                    circuites.Add(newCircuit);
                    newCircuit.AddDistanceSetPoints(distanceSet);
                    connectionsMade++;
                    lastMergedSet = distanceSet;
                }
                if (isTest && connectionsMade == pairsToTake)
                {
                    break;
                }
            }
        }

        private double Distance(Point3D firstPoint, Point3D otherPoint)
        {
            var first = MinusSquared(firstPoint.X, otherPoint.X);
            var second = MinusSquared(firstPoint.Y, otherPoint.Y);
            var third = MinusSquared(firstPoint.Z, otherPoint.Z);
            var sum = first + second + third;
            var root = Math.Sqrt(sum);
            return root;
        }

        private double MinusSquared(double first, double second)
        {
            return Math.Pow((first - second), 2);
        }
    }

    public class Circuit
    {
        public HashSet<Point3D> Points { get; set; } = [];

        public bool AddDistanceSetPoints(DistanceSet set)
        {
            var didAddPoint = false;
            didAddPoint |= Points.Add(set.FirstPoint);
            didAddPoint |= Points.Add(set.SecondPoint);
            return didAddPoint;
        }

        public bool PointOfDistanceSetInCircuit(DistanceSet set)
        {
            return Points.Contains(set.FirstPoint) || Points.Contains(set.SecondPoint);
        }

        public override string ToString()
        {
            return Points.Count.ToString();
        }
    }

    public class DistanceSet : IEquatable<DistanceSet>
    {
        public Point3D FirstPoint { get; }
        public Point3D SecondPoint { get; }
        public double Distance { get; }

        public DistanceSet(Point3D firstPoint, Point3D secondPoint, double distance)
        {
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            Distance = distance;
        }

        public bool ContainsPair(Point3D firstPoint, Point3D secondPoint)
        {
            if ((firstPoint == FirstPoint && secondPoint == SecondPoint) ||
                (firstPoint == SecondPoint && secondPoint == FirstPoint)
                )
            {
                return true;
            }
            return false;
        }

        public bool Equals(DistanceSet? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            // Reihenfolge-unabhängig: (A,B) == (other.A, other.B) ODER (A,B) == (other.B, other.A)
            return (Equals(FirstPoint, other.FirstPoint) && Equals(SecondPoint, other.SecondPoint)) ||
                   (Equals(FirstPoint, other.SecondPoint) && Equals(SecondPoint, other.FirstPoint));
        }

        public override bool Equals(object? obj) => Equals(obj as DistanceSet);

        public override int GetHashCode()
        {
            // Reihenfolge-unabhängiger Hash: sortiere die Item-Hashes deterministisch
            int h1 = FirstPoint.GetHashCode();
            int h2 = SecondPoint.GetHashCode();
            return h1 <= h2 ? HashCode.Combine(h1, h2) : HashCode.Combine(h2, h1);
        }

        public override string ToString() => $"[{FirstPoint} -> {SecondPoint}: {Distance}]";
    }


    public class Point3D : IEquatable<Point3D>
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public static Point3D FromString(string line)
        {
            var parts = line.Split(",").Select(i => double.Parse(i.Trim())).ToArray();
            return new Point3D(parts[0], parts[1], parts[2]);
        }

        public bool Equals(Point3D? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            return X == other.X && Y == other.Y && Z == other.Z;
        }
        public override bool Equals(object? obj) => Equals(obj as Point3D);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    }

}