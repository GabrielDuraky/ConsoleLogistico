using System;

namespace ConsoleLogistico.Modelos
{
    public class MovimentacaoEstoque // Classe para representar uma movimentação de estoque

    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public bool Entrada { get; set; } // true para entrada, false para saída
    }
}
