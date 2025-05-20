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

            var players = new List<Player>();
            for (int i = 1; i <= playerCount; i++)
            {
                var player = new Player(i);
                player.Animals[Animal.Rabbit] = 1;
                players.Add(player);
            }

            _game = new Game(players);
            
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

            if (!_game.ExchangeRates.TryGetValue(target, out var rule))
                return RedirectToAction("Play");

            var player = _game.CurrentPlayer;
            var fromAnimal = rule.fromAnimal;
            var cost = rule.cost;

            if (!player.Animals.ContainsKey(fromAnimal) || player.Animals[fromAnimal] < cost)
                return RedirectToAction("Play");

            player.Animals[fromAnimal] -= cost;
            if (player.Animals[fromAnimal] == 0)
                player.Animals.Remove(fromAnimal);
            
            if (!player.Animals.ContainsKey(target))
                player.Animals[target] = 0;

            player.Animals[target] += 1;
            return RedirectToAction("Play");
        }
        
        //TODO: Add remove winner
        // public IActionResult RemoveWinner()
        // {}
    }   
}

