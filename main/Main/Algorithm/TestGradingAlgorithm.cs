using System;
namespace Main.Algorithm
{
    public class TestGradingAlgorithm : IGradingAlgorithm
    {
        public (int, int) Grade(string question, string answer)
        {
            int maxPoints = 10;
            int points = question.Length == answer.Length ? 10 : 0;
            return (points, maxPoints);
        }
    }
}

