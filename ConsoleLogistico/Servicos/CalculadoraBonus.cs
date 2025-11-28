using System.Text.Json;
using ConsoleLogistico.Modelos;
using System.Linq;

namespace ConsoleLogistico.Servicos;
public class CalculadoraBonus
{
    private readonly string _arquivoVendas;
    public CalculadoraBonus(string arquivoVendas)
    {
        _arquivoVendas = arquivoVendas; //inicializa o caminho do arquivo de vendas
    }

    
    public List<Venda> LerVendas()
    {
        string json = File.ReadAllText(_arquivoVendas);
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("vendas", out var vendasElem)) //verifica se existe a propriedade "vendas"
        {
            return vendasElem
                .EnumerateArray()
                .Select(ve => new Venda //projeta cada elemento do array para um objeto Venda
                {
                    Vendedor = ve.GetProperty("vendedor").GetString() ?? string.Empty, //pega a propriedade "vendedor"
                    Valor = ve.GetProperty("valor").GetDouble() //pega a propriedade "valor"
                })
                .ToList(); //converte o resultado para uma lista
        }

        return new List<Venda>(); //retorna uma lista vazia se a propriedade "vendas" não existir
    }
    /*
        Criação de um método CalcularBonus que lê as vendas e calcula o bônus para cada vendedor.
        É retornado um dicionário onde a chave é o nome do vendedor e o valor é o bônus total calculado.
    */
    public Dictionary<string, double> CalcularBonus()
    {
        var vendas = LerVendas();
        var bonus = new Dictionary<string, double>();
        foreach (var venda in vendas) //itera sobre cada venda
        {
            //calcula o bônus com base no valor da venda
            double c;
            if (venda.Valor < 100) c = 0;
            else if (venda.Valor < 500) c = venda.Valor * 0.01;
            else c = venda.Valor * 0.05;

            if (!bonus.ContainsKey(venda.Vendedor))
                bonus.Add(venda.Vendedor, c); //adiciona o vendedor ao dicionário se não existir
            else
                bonus[venda.Vendedor] += c; //acumula o bônus se o vendedor já existir
        }
        return bonus; //retorna o dicionário com os bônus calculados
    }
}
