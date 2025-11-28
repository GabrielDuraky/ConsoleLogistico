using ConsoleLogistico.Modelos;
using ConsoleLogistico.Servicos;
using System.Globalization;
using System.Text;

string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Data"); // Caminho base para os arquivos de dados

var calcBonus = new CalculadoraBonus(Path.Combine(basePath, "vendedores.json"));
var controleEstoque = new ControleEstoque(Path.Combine(basePath, "estoque.json"));
CalculadoraJuros.CalculadoraJuros? calcJuros = null;

// tenta instanciar a calculadora de juros se disponível
try { calcJuros = new CalculadoraJuros.CalculadoraJuros(); } catch { /* se não existir, opção ficará indisponível */ }

while (true)
{
	Console.WriteLine("\nMENU:");
	Console.WriteLine("1) Comissão de vendedores");
	Console.WriteLine("2) Estoque de produtos");
	Console.WriteLine("3) Calculadora de juros");
	Console.WriteLine("4) Sair");
	Console.Write("Escolha uma opção: ");
	var opt = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(opt)) continue;

	if (opt == "4") break;

	switch (opt)
	{
		case "1":
			var bonus = calcBonus.CalcularBonus();
			Console.WriteLine("\nBÔNUS POR VENDEDOR:");
			foreach (var b in bonus)
				Console.WriteLine($"{b.Key} = R$ {b.Value:F2}");
			break;

		case "2":
			MostrarEstoque(controleEstoque);
			break;

		case "3":
			if (calcJuros is null)
			{
				Console.WriteLine("Calculadora de juros não encontrada.");
				break;
			}
			ExecutarCalculadoraJuros(calcJuros);
			break;

		default:
			Console.WriteLine("Opção inválida.");
			break;
	}
}

static void MostrarEstoque(ControleEstoque controle)
{
	var produtos = controle.ListarProdutos();
	Console.WriteLine("\nID\tDescrição\t\tEstoque");
	foreach (var p in produtos)
		Console.WriteLine($"{p.CodigoProduto}\t{p.Descricao}\t\t{p.Estoque}");

	int? id = ObterIdProduto();
	if (id == null) return;

	var produto = controle.ObterProduto(id.Value);
	if (produto == null)
	{
		Console.WriteLine("Produto não encontrado.");
		return;
	}

	ExecutarMovimentacao(controle, produto);
}

static int? ObterIdProduto()
{
	Console.Write("\nDigite o ID do produto (ou Enter para voltar): ");
	var idInput = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(idInput)) return null;
	if (!int.TryParse(idInput, out var id))
	{
		Console.WriteLine("ID inválido.");
		return null;
	}
	return id;
}

static void ExecutarMovimentacao(ControleEstoque controle, Produto produto)
{
	Console.WriteLine($"\nProduto: {produto.Descricao} - Estoque atual: {produto.Estoque}");
	Console.WriteLine("1) Adicionar quantidade");
	Console.WriteLine("2) Remover quantidade");
	Console.Write("Escolha: ");
	var op = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(op)) return;

	int quantidade = ObterQuantidade();
	if (quantidade < 0) return;

	var mov = new MovimentacaoEstoque
	{
		Id = new Random().Next(1, int.MaxValue),
		Descricao = op == "1" ? "Entrada manual" : "Saída manual",
		Quantidade = quantidade,
		Entrada = op == "1"
	};

	try
	{
		int estoqueFinal = controle.AplicarMovimentacao(mov, produto.CodigoProduto);
		Console.WriteLine($"Estoque final do produto {produto.CodigoProduto}: {estoqueFinal}");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Erro ao aplicar movimentação: {ex.Message}");
	}
}

static int ObterQuantidade()
{
	Console.Write("Quantidade: ");
	var qInput = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(qInput) || !double.TryParse(qInput, out var quantidade) || quantidade < 0)
	{
		Console.WriteLine("Quantidade inválida.");
		return -1;
	}
	return (int)quantidade;
}

static void ExecutarCalculadoraJuros(CalculadoraJuros.CalculadoraJuros calc)
{
	Console.Write("Valor original: ");
	string? valorInput = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(valorInput))
	{
		Console.WriteLine("Valor inválido.");
		return;
	}
	if (!decimal.TryParse(valorInput, NumberStyles.Number, CultureInfo.CurrentCulture, out var valor))
	{
		Console.WriteLine("Valor inválido.");
		return;
	}

	Console.Write("Data de vencimento (DD/MM/AAAA): ");
	string? vencimentoInput = ReadMaskedDateInput();
	if (string.IsNullOrWhiteSpace(vencimentoInput)
		|| !DateTime.TryParseExact(vencimentoInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime vencimento))
	{
		Console.WriteLine("Data inválida.");
		return;
	}

	decimal resultado = calc.Calcular(valor, vencimento);
	Console.WriteLine($"Valor final com juros: {resultado:C}");
}

static string ReadMaskedDateInput()
{
	var digits = new StringBuilder(8);
	int prevLen = 0;
	string prompt = "Data de vencimento (DD/MM/AAAA): ";

	while (true)
	{
		var key = Console.ReadKey(true);

		if (key.Key == ConsoleKey.Enter)
		{
			Console.WriteLine();
			return FormatDigits(digits.ToString());
		}

		if (key.Key == ConsoleKey.Backspace)
		{
			RemoveLastDigit(digits);
		}
		else
		{
			TryAppendDigit(key, digits);
		}

		string formatted = FormatDigits(digits.ToString());
		Console.Write("\r" + prompt + formatted + new string(' ', Math.Max(0, prevLen - formatted.Length)));
		prevLen = formatted.Length;
	}

	static string FormatDigits(string d)
	{
		if (d.Length <= 2) return d;
		if (d.Length <= 4) return d.Substring(0, 2) + "/" + d.Substring(2);
		return d.Substring(0, 2) + "/" + d.Substring(2, 2) + "/" + d.Substring(4);
	}

	static void RemoveLastDigit(StringBuilder digits)
	{
		if (digits.Length > 0)
			digits.Length--;
	}

	static void TryAppendDigit(ConsoleKeyInfo key, StringBuilder digits)
	{
		char c = key.KeyChar;
		if (char.IsDigit(c) && digits.Length < 8)
			digits.Append(c);
	}
}

