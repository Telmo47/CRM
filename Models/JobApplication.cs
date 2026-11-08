using System.ComponentModel.DataAnnotations;

namespace CRM.Models;
public class JobApplication {
    public int Id { get; set; }
    [Required(ErrorMessage = "O Titulo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O titulo não pode ter mais de 100 caracteres.")]
    [Display(Name = "Titulo da candidatura")]
    public string Titulo { get; set; } = string.Empty;
    [StringLength(10000, ErrorMessage = "A descrição não pode ter mais de 10000 caracteres.")]
    [Display(Name = "Descrição da candidatura")]
    public string Descricao { get; set; } = string.Empty;
    [Required(ErrorMessage = "Os Requisitos são obrigatórios.")]
    [StringLength(1000, ErrorMessage = "Os requisitos não podem ter mais de 100 caracteres.")]
    public string Requisitos { get; set; } = string.Empty;
    [Range(0, 10000, ErrorMessage = "O salário tem de ser um valor positivo.")]
    [Display(Name = "Salário (€)")]
    public decimal? Salario { get; set; }
    [Required(ErrorMessage = "O Tipo de candidatura e trabalho pedido é necessário.")]
    [Display(Name = "Tipo de trabalho pedido na candidatura")]
    public string Tipo { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    [Display(Name = "Data da candidatura")]
    public DateTime DataCriacao { get; set; } 
    [DataType(DataType.Date)]
    [Display(Name = "Data da última atualização da candidatura")]
    public DateTime DataAtualizacao { get; set; } 
    [DataType(DataType.Date)]
    [Display(Name = "Data da expiração da candidatura")]
    public DateTime DataExpiracao { get; set; } 


public ApplicationStatus Estado {get; set;}

public int CompanyId {get; set;}

public Company? Company {get; set;}
}