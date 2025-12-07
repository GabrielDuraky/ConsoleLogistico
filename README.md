# ConsoleLogistico

Aplicação console em C# para simular operações de:

- Controle de estoque (entrada/saída)  
- Cálculo de comissões/bônus por vendas  
- Cálculo de juros por atraso (multa diária)  

Esse programa foi desenvolvido como prototipo para fins educacionais e de aprendizado em C#,
focando em manipulação de dados, cálculos financeiros e operações básicas de estoque.
Desenvolvido na plataforma .NET 8.0+, utilizando conceitos de orientação a objetos e boas práticas de código.


---

## Funcionalidades

1. **Controle de estoque** — permite registrar movimentações de entrada ou saída de produtos, com atualização automática da quantidade em estoque.  
2. **Cálculo de comissão/bônus** — recebe uma lista de vendas (JSON), agrupa por vendedor e aplica regras de bônus conforme regra definida.  
3. **Cálculo de juros por atraso** — a partir do valor original e data de vencimento, calcula o valor corrigido considerando multa diária (ex: 2,5% ao dia).  

---

## Como rodar

1. Instale o [.NET SDK 8.0+] no seu sistema.  
   https://dotnet.microsoft.com/pt-br/download/dotnet/8.0

2. Clone este repositório:  
   ```bash
   git clone https://github.com/GabrielDuraky/ConsoleLogistico.git
   ```

Realizado isso, será possível navegar até a pasta do projeto e compilar utilizando o comando:  
   ```bash
   dotnet build
   ```
e em sequencia executar com:
   ```bash
   dotnet run
   ```
ou, se preferir, abra o projeto em uma IDE como Visual Studio ou JetBrains Rider e execute diretamente por lá.

3. Siga as instruções no console para utilizar as funcionalidades disponíveis.
	
   ```
	MENU:
	1) Comissão de vendedores
	2) Estoque de produtos
	3) Calculadora de juros 
	4) Sair
	Escolha uma opção:
   ```
	

## Cenarios de teste

1. Calculo de comissão dos vendedores:
	Saida esperada:
   ```
    BÔNUS POR VENDEDOR:
	João Silva = R$ 495,68
	Maria Souza = R$ 465,95
	Carlos Oliveira = R$ 379,37
	Ana Lima = R$ 404,98
   ```

2. Testar movimentação de estoque para entrada e saída de produtos com entradas válidas e inválidas.
	Saida esperada:
   ```
    Produto: Caderno Universitário - Estoque atual: 75
    2) Dar baixa no estoque
    3) Voltar
    Escolha: 2
    Quantidade: 76
    Erro ao aplicar movimentação: Quantidade solicitada (76) excede o estoque atual (75).
    Tentar novamente? (s/N):
   ```

3. Calcular juros por atraso, considerando diferentes datas de vencimento e valores originais a 2,5% ao dia.

## Estrutura do projeto
- `Program.cs` — ponto de entrada da aplicação, gerencia o menu e fluxo principal.
- `Models/` — contém as classes de modelo (Produto, Venda).
- `Services/` — contém as classes de serviço para lógica de negócio (ControleEstoque, CalculadoraBonus, CalculadoraJuros).
- `Data/` — contém dados de exemplo em JSON para vendas.