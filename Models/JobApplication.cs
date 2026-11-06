namespace CRM.Models;
public class JobApplication {
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Requisitos { get; set; } = string.Empty;
    public string? Responsabilidades { get; set; }
    public string? Salario { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } 
    public DateTime DataAtualizacao { get; set; } 
    public DateTime DataExpiracao { get; set; } 
    public DateTime DataPublicacao { get; set; }


public ApplicationStatus Estado {get; set;}

public int CompanyId {get; set;}

public Company? Company {get; set;}
}