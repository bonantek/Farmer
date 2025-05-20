using Microsoft.AspNetCore.Mvc;
using SuperFarmer.Models;

namespace SuperFarmer.Controllers
{
    public class GameController : Controller
    {
        private static Game? _game;
        
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult StartGame(int playerCount)
        {
            if (playerCount < 2 || playerCount > 4)
            {
                ModelState.AddModelError("", "Number of player must be 2 - 4");
                return View("Index");
            }

            _game = Game.InitializeWithPlayers(playerCount);
            
            return RedirectToAction("Play");
        }
        
        public IActionResult Play()
        {
            if (_game == null)
                return RedirectToAction("Index");

            return View(_game);
        }
        
        [HttpPost]
        public IActionResult NextTurn()
        {
            _game?.NextTurn();
            return RedirectToAction("Play");
        }
        
        [HttpPost]
        public IActionResult RollDice()
        {
            _game?.RollDice();
            _game?.AnimalEating();
            _game?.Breed();
            
            var winnerIndex = _game.CheckVictory();

            if (winnerIndex != null)
            {
                ViewBag.WinnerIndex = winnerIndex;
                return View("Victory", _game);
            }

            return RedirectToAction("Play");
        }
        
        public IActionResult ShowExchangeOptions(string animal)
        {
            if (_game == null || !Enum.TryParse<Animal>(animal, out var ownedAnimal))
                return RedirectToAction("Play");

            var player = _game.CurrentPlayer;
            
            int playerHas = player.Animals.ContainsKey(ownedAnimal) ? player.Animals[ownedAnimal] : 0;

            var availableExchanges = _game.ExchangeRates
                .Where(rate => rate.Value.fromAnimal == ownedAnimal && playerHas >= rate.Value.cost)
                .Select(rate => Tuple.Create(rate.Key, rate.Value.cost))
                .ToList();

            ViewBag.OwnedAnimal = ownedAnimal;
            ViewBag.Exchanges = availableExchanges;


            return View("ExchangeOptions", _game);
        }
        
        
        [HttpPost]
        public IActionResult MakeExchange(string targetAnimal)
        {
            if (_game == null || _game.DiceRolledThisTurn)
                return RedirectToAction("Play");

            if (!Enum.TryParse<Animal>(targetAnimal, out var target))
                return RedirectToAction("Play");

            var player = _game.CurrentPlayer;

            var success = player.TryExchange(target, _game.ExchangeRates, _game.Bank);

            if (!success)
                TempData["Error"] = "Wymiana nie jest możliwa. Sprawdź liczbę zwierząt i bank.";

            return RedirectToAction("Play");
        }

        
        //TODO: Add remove winner
        // public IActionResult RemoveWinner()
        // {}
    }   
}

