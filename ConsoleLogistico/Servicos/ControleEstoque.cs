using System.Text.Json;
using ConsoleLogistico.Modelos;

namespace ConsoleLogistico.Servicos;

public class ControleEstoque
{
	private readonly string _arquivoEstoque; // Caminho do arquivo JSON de estoque
    private readonly List<Produto> _produtos; // Lista interna de produtos no estoque

    public ControleEstoque(string arquivoEstoque)
	{
		_arquivoEstoque = arquivoEstoque;
		_produtos = LerEstoque(); // Carrega o estoque ao inicializar
    }

	private List<Produto> LerEstoque()
	{
		try
		{
			string json = File.ReadAllText(_arquivoEstoque); // Lê o conteúdo do arquivo JSON
            using var doc = JsonDocument.Parse(json); 

			var produtos = new List<Produto>(); // Lista para armazenar os produtos lidos
            if (!doc.RootElement.TryGetProperty("estoque", out var estoqueEl) || estoqueEl.ValueKind != JsonValueKind.Array) 
				return produtos; // Retorna lista vazia se a propriedade "estoque" não existir ou não for um array

            foreach (var p in estoqueEl.EnumerateArray()) // Itera sobre cada produto no array
            {
				if (!TentarObterProduto(p, out var produto) || produto is null) // Tenta obter o produto
                    continue;

				produtos.Add(produto); // Adiciona o produto à lista
            }

			return produtos;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Falha ao ler estoque ('{_arquivoEstoque}'): {ex.Message}"); 
			return new List<Produto>(); // Retorna lista vazia em caso de erro
        }
	}

	private static bool TentarObterProduto(JsonElement p, out Produto? produto) 
	{
		produto = null;

		// CodigoProduto
		if (!p.TryGetProperty("codigoProduto", out var codigoEl) || !codigoEl.TryGetInt32(out var codigo))
		{
			Console.WriteLine("Produto ignorado: codigoProduto ausente ou inválido. JSON: " + p.GetRawText());
			return false;
		}

		// Descricao 
		string descricao = string.Empty;
		if (p.TryGetProperty("descricao", out var descEl) && descEl.ValueKind == JsonValueKind.String)
			descricao = descEl.GetString() ?? string.Empty;
		else if (p.TryGetProperty("descricaoProduto", out var descProdEl) && descProdEl.ValueKind == JsonValueKind.String)
			descricao = descProdEl.GetString() ?? string.Empty;

		// Estoque 
		double estoqueVal = 0;
		if (p.TryGetProperty("estoque", out var estEl))
		{
			if (estEl.ValueKind == JsonValueKind.Number && estEl.TryGetDouble(out var d))
				estoqueVal = d;
			else if (estEl.ValueKind == JsonValueKind.String)
			{
				var estStr = estEl.GetString();
				if (!string.IsNullOrWhiteSpace(estStr) && double.TryParse(estStr, out var parsed))
					estoqueVal = parsed;
			}
		}

		produto = new Produto
		{
			CodigoProduto = codigo,
			Descricao = descricao,
			Estoque = estoqueVal
		};
		return true;
	}

	public Produto? ObterProduto(int codigo) =>
		_produtos.FirstOrDefault(p => p.CodigoProduto == codigo);

    // Retorna a lista de produtos para consumo pela UI/Program
    public IReadOnlyList<Produto> ListarProdutos() =>
        _produtos.AsReadOnly();

	public int AplicarMovimentacao(MovimentacaoEstoque mov, int codigoProduto)
	{
		var produto = ObterProduto(codigoProduto);
		if (produto == null)
			throw new ArgumentException("Produto não encontrado.");
		if (mov.Entrada)
			produto.Estoque += mov.Quantidade; // Adições ao estoque
        else
			produto.Estoque -= mov.Quantidade; // Saidas do estoque

        return (int)produto.Estoque; // Estoque final
	}
}
