using ConsoleLogistico.Servicos;

string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Data"));
int? ultimaOpcao = MenuService.MostrarMenu(basePath); // Chama o menu e captura a última opção escolhida

if (ultimaOpcao is not null)
    Console.WriteLine($"Aplicação finalizada. Última opção escolhida: {ultimaOpcao}");