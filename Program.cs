using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var demos = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
            {
                { "taskrun", () => WrapVoid(TaskRunTarea.Run) },
                { "constructor", () => WrapVoid(ConstructorTaskTarea.Run) },
                { "factory", () => WrapVoid(TaskFactoryStartNewTarea.Run) },
                { "attached", () => WrapVoid(TaskCreationOptionsAttachedToParentTarea.Run) },
                { "continue", () => WrapVoid(TaskContinueWithOnlyOnRanToCompletionTarea.Run) }
            };

            string choice = args.Length > 0 ? args[0] : null;

            while (true)
            {
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("\nElige demo para ejecutar (o 'list' para ver opciones, 'all' para ejecutar todos, 'exit' para salir):");
                    Console.WriteLine("  list | taskrun | constructor | factory | attached | continue | all | exit");
                    Console.Write("> ");
                    choice = Console.ReadLine()?.Trim();
                }

                if (string.IsNullOrEmpty(choice)) { choice = null; continue; }

                if (choice.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                if (choice.Equals("list", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Demos disponibles:");
                    foreach (var k in demos.Keys) Console.WriteLine(" - " + k);
                }
                else if (choice.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var kv in demos)
                    {
                        Console.WriteLine($"\n--- Ejecutando: {kv.Key} ---");
                        try { await kv.Value(); }
                        catch (Exception ex) { Console.WriteLine($"ERROR en {kv.Key}: {ex.Message}"); }
                    }
                }
                else if (demos.TryGetValue(choice, out var runFunc))
                {
                    Console.WriteLine($"\n--- Ejecutando: {choice} ---");
                    try { await runFunc(); }
                    catch (Exception ex) { Console.WriteLine($"ERROR: {ex}"); }
                }
                else
                {
                    Console.WriteLine($"Opción desconocida: '{choice}'");
                }

                // preparar siguiente iteración
                choice = null;
            }

            Console.WriteLine("Saliendo.");
            return 0;
        }

        static Task WrapVoid(Action action)
        {
            try
            {
                action();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        static Task WrapVoid(Func<Task> asyncFunc) => asyncFunc();
    }
}
