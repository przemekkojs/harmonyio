using Main.Algorithm;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Main.Enumerations;

namespace Main.Pages
{
    public class GradeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;
        private readonly IGradingAlgorithm _algorithm;

        public Quiz Quiz { get; set; } = null!;
        public Dictionary<ApplicationUser, List<ExcersiseSolution>> UsersToSolutions { get; set; } = null!;
        public Dictionary<ExcersiseSolution, (int, int)> SolutionsToGradings { get; set; } = null!;
        public Dictionary<ApplicationUser, List<(int, int)>> UsersToGradings { get; set; } = null!;

        [BindProperty]
        public List<Grade> Grades { get; set; } = null!;
        [BindProperty]
        public List<List<int>> Points { get; set; } = null!;
        [BindProperty]
        public List<List<int>> Maxes { get; set; } = null!;

        public GradeModel(
            ApplicationRepository repository,
            UserManager<ApplicationUser> userManager,
            IGradingAlgorithm algorithm
        )
        {
            _repository = repository;
            _userManager = userManager;
            _algorithm = algorithm;
        }

        public async void OnGet() //int id
        {
            //TODO: REMOVE THIS, THE ID SHOULD BE A PARAMETER OF ONGET
            var allQuizes = await _repository.GetAllAsync<Quiz>();
            if (allQuizes.Count == 0)
                throw new InvalidOperationException("No quizes found in the repository. Go to /create page and create a quiz for testing purposes.");
            int id = allQuizes[2].Id;

            Quiz = (await _repository.GetAsync<Quiz>(
                q => q.Id == id,
                query => query
                    .Include(q => q.Excersises)
                    .ThenInclude(e => e.ExcersiseSolutions)
                    .ThenInclude(es => es.User)
            ))!;

            UsersToSolutions = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .GroupBy(es => es.User)
                .ToDictionary(g => g.Key, g => g.ToList());

            SolutionsToGradings = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions, (e, es) => (e.Question, es))
                .ToDictionary(
                    tuple => tuple.es,
                    tuple => _algorithm.Grade(tuple.Question, tuple.es.Answer)
                );

            UsersToGradings = new Dictionary<ApplicationUser, List<(int, int)>>();
            foreach (var userToSolutions in UsersToSolutions)
            {
                var user = userToSolutions.Key;
                var solutions = userToSolutions.Value;

                var gradings = new List<(int, int)>();
                foreach (var solution in solutions)
                    gradings.Add(SolutionsToGradings[solution]);

                UsersToGradings.Add(user, gradings);
            }
        }

        public async Task<IActionResult> OnPost()
        {

            return RedirectToPage("Index");
        }
    }
}
