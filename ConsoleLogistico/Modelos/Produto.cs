using System;

namespace ConsoleLogistico.Modelos
{
    public class Produto // Classe para representar um produto no estoque
    {
        public int CodigoProduto { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public double Estoque { get; set; }
    }
}