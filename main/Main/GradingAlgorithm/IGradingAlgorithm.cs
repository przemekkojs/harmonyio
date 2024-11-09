using System;
namespace Main.GradingAlgorithm
{
	public interface IGradingAlgorithm
	{
		/// <summary>
		/// Grades an answer based on the question
		/// </summary>
		/// <returns>Tuple of (points, maxPoints, algorithmOpinion)</returns>
		public (int, int, string) Grade(string question, string answer);
	}
}

