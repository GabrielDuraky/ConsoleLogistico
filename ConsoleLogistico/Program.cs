using ConsoleLogistico.Servicos;

string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
int? ultimaOpcao = MenuService.MostrarMenu(basePath);

// opcional: tratar resultado no main
if (ultimaOpcao is not null)
    Console.WriteLine($"Aplicação finalizada. Última opção escolhida: {ultimaOpcao}");

