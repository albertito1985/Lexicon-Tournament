using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        public string Title { get; set; }

        protected NotFoundException(string message, string title = "Not found") : base(message) 
        {
            Title = title;
        }
    }

    public class TournamentNotFoundException : NotFoundException
    {
        public TournamentNotFoundException(int id) : base($"The tournament with ID {id} does not exist")
        {
            
        }
    }

    public class GameNotFoundException : NotFoundException
    {
        public GameNotFoundException(int id) : base($"The game with ID {id} does not exist")
        {
            
        }
    }

    public class GameTitleNotFoundException : NotFoundException
    {
        public GameTitleNotFoundException(string title) : base($"The game with title {title} does not exist")
        {

        }
    }

}
