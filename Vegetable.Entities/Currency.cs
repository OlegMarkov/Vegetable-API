using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegetable.Entities
{
    public class Currency
    {
        public int Id { get; set; }

        [MaxLength(3)]
        public string CurrencyCode { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string Symbol { get; set; }
    }
}
