using OrderGenerator.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OrderGenerator.API.DTO;

public class OrderGeneratorRequestDTO
{
    [RegularExpression("^(PETR4|VALE3|VIIA4)$", ErrorMessage = "Símbolo só pode ser PETR4 ou VALE3 ou VIIA4")]
    public string Simbolo { get; set; }

    [RegularExpression("^[12]$", ErrorMessage = "Lado só pode usar os valores '1' ou '2'")]
    public char Lado { get; set; }

    [Range(1, 99999, ErrorMessage = "Quantidade precisa ser entre 1 e 99.999")]
    public int Quantidade { get; set; }
    
    [Range(0.01, 10000, ErrorMessage = "Preço precisar ser maior que 0 e menor que 1.000")]
    [MultipleOf(0.01, ErrorMessage = "Preço precisa ser múltiplo de 0,01")]
    public decimal Preco { get; set; }

}
