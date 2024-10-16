using System;
namespace Main.Algorithm
{
	public interface IGradingAlgorithm
	{
		/// <summary>
		/// Grades an answer based on the question
		/// </summary>
		/// <returns>Tuple of (points, maxPoints)</returns>
		public (int, int) Grade(string question, string answer);
	}
}

