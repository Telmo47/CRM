public class Company{
    public int Id { get; set;}
    public string Nome { get; set;}
    public string Website { get; set;} 
    public string País { get; set;}
    public string Cidade { get; set;}
    // Para dizer que tipo de tecnologia a empresa usa e o que preciso saber (javascript, C#, Python, etc...)
    public string Tec { get; set;}


public ICollection<ApplicationStatus> Candidaturas { get; set; } = new List<Candidaturas>();
}