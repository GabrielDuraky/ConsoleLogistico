using System;

namespace CalculadoraJuros
{
    public class CalculadoraJuros
    {
        public decimal Calcular(decimal valorOriginal, DateTime vencimento)
        {
            DateTime hoje = DateTime.Now.Date;

            if (hoje <= vencimento)
                return valorOriginal;

            int diasAtraso = (hoje - vencimento).Days;

            decimal juros = valorOriginal * 0.025m * diasAtraso;

            return valorOriginal + juros;
        }
    }
}