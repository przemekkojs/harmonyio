using System;
namespace Main.GradingAlgorithm
{
    public class TestGradingAlgorithm : IGradingAlgorithm
    {
        public (int, int, string) Grade(string question, string answer, int maxPoints)
        {
            int points = question.Length == answer.Length ? 10 : 0;
            return (points, maxPoints, "Fatalnie.");
        }
    }
}

