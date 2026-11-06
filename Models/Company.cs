public class Company{
    public int Id { get; set;}
    public string Name { get; set;}
    public string Website { get; set;} 
    public string Country { get; set;}
    public string City { get; set;}
    // Para dizer que tipo de tecnologia a empresa usa e o que preciso saber (javascript, C#, Python, etc...)
    public string Tec { get; set;}
}

public ICollection<Candidaturas> Candidaturas { get; set; } = new List<Candidaturas>();
