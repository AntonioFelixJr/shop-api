using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    //[Table("aCategory")]
    public class Category
    {
        [Key]
        public int Id { get; set;}

        [Required(ErrorMessage = "Esse campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        //[DataType("nvarchar")]
        public string Title { get; set; }
    }
}