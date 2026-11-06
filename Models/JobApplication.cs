public class Candidatura {
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public string Requisitos { get; set; }
    public string Responsabilidades { get; set; }
    public string Salario { get; set; }
    public string Localizacao { get; set; }
    public string Tipo { get; set; }
    public string Status { get; set; }
    public string DataCriacao { get; set; }
    public string DataAtualizacao { get; set; }
    public string DataExpiracao { get; set; }
    public string DataPublicacao { get; set; }
}

public EstadoApplication {get; set;}

public int CompanyId {get; set;}

public Company? Company {get; set;}