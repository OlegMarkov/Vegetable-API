using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegetable.Entities
{
    public class Log    
    {
        public Guid Id { get; set; }

        [MaxLength(30)]
        public string Level { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

    }
}
