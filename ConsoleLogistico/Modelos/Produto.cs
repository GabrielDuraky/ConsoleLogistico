using System;

namespace ConsoleLogistico.Modelos
{
	public class Produto
	{
		public int CodigoProduto { get; set; }
		public string Descricao { get; set; } = string.Empty;
		public double Estoque { get; set; }
	}
}