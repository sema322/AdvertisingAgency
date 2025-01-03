using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingAgency.Models
{
    public class Advertising : Entity
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public Category category { get; set; }
        public DateTime DateStart { get; set; }
        public int Duration { get; set; }
        public int ClientId { get; set; }
        public Client client { get; set; }
        public bool IsActive { get; set; }
        public bool IsCorrect { get; set; }
    }
}
