using System.ComponentModel.DataAnnotations;

namespace AdvertisingAgency.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Login {  get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        public string Password { get; set; }
    }
}
