using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngineRM
{
    public class GameLoop
    {
        private Game _myGame;
        public bool Running { get; private set; }
        public double dElapsedTime { get; set; }

        public GameLoop(Game game)
        {
            _myGame = game;
        }

        /// <summary>
        /// Start GameLoop
        /// </summary>
        public async void Start()
        {
            if (_myGame == null)
                throw new ArgumentException("Game not loaded!");

            dElapsedTime = 0;

            // Load game content
            _myGame.Load();

            // Set gameloop state
            Running = true;

            // Set previous game time
            DateTime _previousGameTime = DateTime.Now;

            while (Running)
            {
                // Calculate the time elapsed since the last game loop cycle
                TimeSpan GameTime = DateTime.Now - _previousGameTime;
                // Update the current previous game time
                _previousGameTime = _previousGameTime + GameTime;

                // Update the game
                dElapsedTime = GameTime.TotalMilliseconds / 1000;
                _myGame.Update();

                _myGame.Draw();


                await Task.Delay(1);
            }

        }

        /// <summary>
        /// Stop GameLoop
        /// </summary>
        public void Stop()
        {
            Running = false;
            _myGame?.Unload();
        }
    }
}
