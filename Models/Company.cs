namespace CRM.Models;
public class Company{
    public int Id { get; set;}
    public string Nome { get; set;} = string.Empty;
    public string? Website { get; set;} 
    public string Pais { get; set;} = string.Empty;
    public string Cidade { get; set;} = string.Empty;
    // Para dizer que tipo de tecnologia a empresa usa e o que preciso saber (javascript, C#, Python, etc...)
    public string? Tec { get; set;} = string.Empty;


public ICollection<JobApplication> Candidaturas { get; set; } = new List<JobApplication>();
}