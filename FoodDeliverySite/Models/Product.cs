using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace FoodDeliverySite.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [MaxLength(30)]
        [Display(Name= "Наименование")]
        [Required(ErrorMessage ="Заполните название")]
        public string Name { get; set; }

        [Display(Name = "Категория")]
        public string Category { get; set; }

        [Display(Name = "Цена")]
        [Range(1,200000, ErrorMessage ="Цена  должна быть больше 1 и меньше 200000")]
        public decimal Price { get; set; }

        [Display(Name = "Количество")]
        [Range(1, 100000, ErrorMessage = "Цена  должна быть больше 1 и меньше 100000")]
        public int Count { get; set; }

        //----------------------------------------
        public string ImagePath { get; set; }
    }
}
