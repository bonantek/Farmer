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
                players.Add(new Player(i));
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
            return RedirectToAction("Play");
        }


    }   
}

