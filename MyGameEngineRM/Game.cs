using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameEngineRM
{
    public class Game
    {

        public RenderWindow renderWindow { get; set; }

        public Game(RenderWindow rWindow)
        {
            this.renderWindow = rWindow;
        }

        public Game()
        {

        }

        /// <summary>
        /// Load assets eg graphics, music etc
        /// </summary>
        public virtual void Load()
        {
            // Load graphics
            // Turn off game music
        }

        /// <summary>
        /// Unload assets eg graphics, music etc
        /// </summary>
        public void Unload()
        {
            // Unload graphics
            // Turn off game music
        }

        /// <summary>
        /// Standard OnUpdate event
        /// </summary>
        public virtual void Update()
        {
  
            
        }

        /// <summary>
        /// Standard OnPaint event
        /// </summary>
        public virtual void Draw()
        {

        }

    }
}
