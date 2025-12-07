using System;

namespace CalculadoraJuros
{
    public class CalculadoraJuros // Classe para calcular juros sobre um valor original com base na data de vencimento
    {
        public decimal Calcular(decimal valorOriginal, DateTime vencimento)
        {
            DateTime hoje = DateTime.Now.Date; // Obtém a data atual sem a parte do tempo

            if (hoje <= vencimento)
                return valorOriginal;

            int diasAtraso = (hoje - vencimento).Days;

            decimal juros = valorOriginal * 0.025m * diasAtraso;

            return valorOriginal + juros;
        }
    }
}