using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Exceptions
{
    public abstract class BadRequestException : Exception
    {
        public string Title { get; set; }

        protected BadRequestException(string message, string title = "BadRequest") : base(message) 
        {
            Title = title;
        }
    }

    public class GameBadRequestException : BadRequestException
    {
        public GameBadRequestException(string message) : base(message)
        {
            
        }
    }

}
