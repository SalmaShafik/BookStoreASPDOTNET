using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1Psight.Models;

namespace WebApplication1Psight.ViewModel
{
    public class ProductViewModel
    {
        public string? Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        public SelectList? Categories { get; set; }

    }

}
