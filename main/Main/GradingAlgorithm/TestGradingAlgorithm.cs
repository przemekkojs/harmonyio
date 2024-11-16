using System;
namespace Main.GradingAlgorithm
{
    public class TestGradingAlgorithm : IGradingAlgorithm
    {
        public (int, int, Dictionary<(int, (int, int, int)), List<int>>) Grade(string question, string answer, int maxPoints)
        {
            int points = question.Length == answer.Length ? 10 : 0;
            return (points, maxPoints, []);
        }
    }
}

