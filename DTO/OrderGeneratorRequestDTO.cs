namespace OrderGenerator.API.DTO;

public class OrderGeneratorRequestDTO
{
    public string Simbolo { get; set; }
    public char Lado { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
}
