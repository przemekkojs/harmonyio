using System;
namespace Main.Enumerations
{
	public enum QuizState
	{
		NotStarted,
		Open,
		Closed,
	} 

    public static class QuizStateExtensions
    {
        public static string AsString(this QuizState state)
        {
            return state switch
            {
                QuizState.NotStarted => "Nie rozpoczęte",
                QuizState.Open => "Otwarte",
                QuizState.Closed => "Zakończone",
                _ => "Nieznany stan"
            };
        }
    }
}

