using System;
using System.Globalization;
using System.IO;
using System.Text;
using ConsoleLogistico.Modelos;

namespace ConsoleLogistico.Servicos;

public static class MenuService
{
	public static int? MostrarMenu(string basePath)
	{
		var calcBonus = new CalculadoraBonus(Path.Combine(basePath, "vendedores.json"));
		var controleEstoque = new ControleEstoque(Path.Combine(basePath, "estoque.json"));
		CalculadoraJuros.CalculadoraJuros? calcJuros = null;

		try { calcJuros = new CalculadoraJuros.CalculadoraJuros(); } // tenta inicializar a calculadora de juros
        catch (Exception ex) 
		{ 
			Console.WriteLine($"Erro ao inicializar a CalculadoraJuros: {ex.Message}"); // loga o erro
            calcJuros = null; // continua sem a calculadora de juros
        }

		int? ultimaOpcao = null;
        // Loop do menu principal
        while (true)
		{
			Console.WriteLine("\nMENU:");
			Console.WriteLine("1) Comissão de vendedores");
			Console.WriteLine("2) Estoque de produtos");
			Console.WriteLine("3) Calculadora de juros");
			Console.WriteLine("4) Sair");
			Console.Write("Escolha uma opção: ");
			var opt = Console.ReadLine();
            // Ignora entradas vazias
            if (string.IsNullOrWhiteSpace(opt)) continue;

			if (opt == "4")
			{
				ultimaOpcao = 4;
				break; // Sai do loop e retorna a última opção
            }

			switch (opt)
			{
				case "1":
					var bonus = calcBonus.CalcularBonus(); // calcula o bônus dos vendedores
                    Console.WriteLine("\nBÔNUS POR VENDEDOR:");
					foreach (var b in bonus)
						Console.WriteLine($"{b.Key} = R$ {b.Value:F2}");
					ultimaOpcao = 1;
					break;

				case "2":
					MostrarEstoque(controleEstoque); // exibe o estoque e permite movimentações
                    ultimaOpcao = 2;
					break;

				case "3":
					if (calcJuros is null) // verifica se a calculadora de juros foi inicializada
                    {
						Console.WriteLine("Calculadora de juros não encontrada.");
						break;
					}
					ExecutarCalculadoraJuros(calcJuros); // executa a calculadora de juros
                    ultimaOpcao = 3;
					break;

				default:
					Console.WriteLine("Opção inválida.");
					break;
			}
		}

		return ultimaOpcao; // retorna a última opção selecionada
    }

    // Métodos auxiliares movidos para o serviço
    // aqui para manter o MenuService organizado
    private static void MostrarEstoque(ControleEstoque controle) // exibe o estoque e permite movimentações
    {
		var produtos = controle.ListarProdutos();
		Console.WriteLine("\nID\tDescrição\t\tEstoque"); // cabeçalho da tabela
        foreach (var p in produtos)
			Console.WriteLine($"{p.CodigoProduto}\t{p.Descricao}\t\t{p.Estoque}"); // exibe cada produto

        int? id = ObterIdProduto();
		if (id == null) return;

		var produto = controle.ObterProduto(id.Value);
		if (produto == null)
		{
			Console.WriteLine("Produto não encontrado.");
			return;
		}

		ExecutarMovimentacao(controle, produto); // executa a movimentação do produto selecionado
                                                 // parametros: controle de estoque e o produto selecionado
    }

    private static int? ObterIdProduto() // obtém o ID do produto do usuário
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

	private static void ExecutarMovimentacao(ControleEstoque controle, Produto produto)
	{
		while (true)
		{
			Console.WriteLine($"\nProduto: {produto.Descricao} - Estoque atual: {produto.Estoque}"); // exibe o produto e o estoque atual
            Console.WriteLine("1) Cadastrar estoque novo"); // opções de movimentação
            Console.WriteLine("2) Dar baixa no estoque"); // opções de movimentação
            Console.WriteLine("3) Voltar");
			Console.Write("Escolha: ");
			var op = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(op)) continue;
			if (op.Trim().Equals("3", StringComparison.OrdinalIgnoreCase)) return;

			if (!TentarObterTipoMovimentacao(op, out bool entrada)) continue;

			int quantidade = ObterQuantidade();
			if (quantidade < 0) continue; // inválido -> reexibe submenu

			var mov = CriarMovimentacao(entrada, quantidade);

			if (TentarAplicarMovimentacao(controle, mov, produto.CodigoProduto, out int estoqueFinal))
			{
				Console.WriteLine($"Estoque final do produto {produto.CodigoProduto}: {estoqueFinal}");
				return; // operação bem-sucedida volta ao submenu anterior
			}
			else
			{
				if (!PerguntarTentarNovamente()) return;
			}
		}
	}

	private static bool TentarObterTipoMovimentacao(string? op, out bool entrada)
	{
		entrada = false;
		if (string.IsNullOrWhiteSpace(op)) return false;
		if (op.Trim().Equals("3", StringComparison.OrdinalIgnoreCase)) return true;
		if (op == "1") { entrada = true; return true; }
		if (op == "2") { entrada = false; return true; }
		Console.WriteLine("Opção inválida. Tente novamente.");
		return false;
	}

	private static bool PerguntarTentarNovamente()
	{
		Console.Write("Tentar novamente? (s/N): ");
		var r = Console.ReadLine();
		return !string.IsNullOrWhiteSpace(r) && r.Trim().Equals("s", StringComparison.OrdinalIgnoreCase);
	}

	private static MovimentacaoEstoque CriarMovimentacao(bool entrada, int quantidade)
	{
		return new MovimentacaoEstoque
		{
			Id = new Random().Next(1, int.MaxValue),
			Descricao = entrada ? "Entrada manual" : "Saída manual",
			Quantidade = quantidade,
			Entrada = entrada
		};
	}

	private static bool TentarAplicarMovimentacao(ControleEstoque controle, MovimentacaoEstoque mov, int codigoProduto, out int estoqueFinal)
	{
		try
		{
			estoqueFinal = controle.AplicarMovimentacao(mov, codigoProduto);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao aplicar movimentação: {ex.Message}");
			estoqueFinal = 0;
			return false;
		}
	}

	private static int ObterQuantidade()
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

	private static void ExecutarCalculadoraJuros(CalculadoraJuros.CalculadoraJuros calc)
	{
		decimal valor;
		while (true)
		{
			Console.Write("Valor original (Enter para voltar): ");
			string? valorInput = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(valorInput)) { Console.WriteLine("Operação cancelada."); return; }
			if (decimal.TryParse(valorInput, NumberStyles.Number, CultureInfo.CurrentCulture, out valor)) break;
			Console.WriteLine("Valor inválido. Tente novamente.");
		}

		DateTime vencimento;
		while (true)
		{
			Console.Write("Data de vencimento (DD/MM/AAAA) (Enter para voltar): ");
			string? vencimentoInput = ReadMaskedDateInput();
			if (string.IsNullOrWhiteSpace(vencimentoInput)) { Console.WriteLine("Operação cancelada."); return; }
			if (DateTime.TryParseExact(vencimentoInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out vencimento)) break;
			Console.WriteLine("Data inválida. Tente novamente.");
		}

		decimal resultado = calc.Calcular(valor, vencimento);
		Console.WriteLine($"Valor final com juros: {resultado:C}");
	}

	private static string ReadMaskedDateInput()
	{
		var digits = new StringBuilder(8);
		int prevLen = 0;
		string prompt = "Data de vencimento (DD/MM/AAAA) (Enter para voltar): ";

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
}