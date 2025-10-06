using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    public class TaskFactoryStartNewTarea
    {
        public static async Task Main()
        {
            const int N = 500_000;
            Console.WriteLine("=== Task.Factory.StartNewTarea ===\n");
            Console.WriteLine($"[Principal] Hilo principal: {Environment.CurrentManagedThreadId}\n");

            var sw = Stopwatch.StartNew();

            var longRunning = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"[LongRunning] Inicio en hilo {Environment.CurrentManagedThreadId}");
                long acc = 0;
                for (int i = 0; i < N; i++)
                {
                    acc += i;
                    if ((i & 0x7FFFF) == 0) Thread.Sleep(1);
                }
                Console.WriteLine($"[LongRunning] Fin en hilo {Environment.CurrentManagedThreadId}");
                return acc;
            }, TaskCreationOptions.LongRunning);

            var regular = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"[Regular] Inicio en hilo {Environment.CurrentManagedThreadId}");
                long acc = 0;
                for (int i = 0; i < N; i++)
                {
                    acc += i;
                    if ((i & 0x7FFFF) == 0) Thread.Sleep(1);
                }
                Console.WriteLine($"[Regular] Fin en hilo {Environment.CurrentManagedThreadId}");
                return acc;
            });

            Console.WriteLine("[Principal] Tareas disparadas (una LongRunning, otra regular).");

            await Task.WhenAll(longRunning, regular);
            sw.Stop();

            Console.WriteLine($"\nResultados: LongRunning={longRunning.Result}, Regular={regular.Result}");
            Console.WriteLine($"Tiempo total: {sw.Elapsed.TotalSeconds:F2}s");
            Console.WriteLine("=== Fin Task.Factory.StartNewTarea ===");
        }
    }
}
