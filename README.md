# ConsoleLogistico

Aplicação console em C# para simular operações de:

- Controle de estoque (entrada/saída)  
- Cálculo de comissões/bônus por vendas  
- Cálculo de juros por atraso (multa diária)  

---

## Funcionalidades

1. **Controle de estoque** — permite registrar movimentações de entrada ou saída de produtos, com atualização automática da quantidade em estoque.  
2. **Cálculo de comissão/bônus** — recebe uma lista de vendas (JSON), agrupa por vendedor e aplica regras de bônus conforme regra definida.  
3. **Cálculo de juros por atraso** — a partir do valor original e data de vencimento, calcula o valor corrigido considerando multa diária (ex: 2,5% ao dia).  

---

## Como rodar

1. Instale o [.NET SDK 7.0+ ou 8.0+] no seu sistema.  
2. Clone este repositório:  
   ```bash
   git clone https://github.com/GabrielDuraky/ConsoleLogistico.git
