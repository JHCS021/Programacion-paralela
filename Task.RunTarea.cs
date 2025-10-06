using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    public class TaskRunTarea
    {
        public static async Task Run()
        {
            const int N = 1_000_000;
            Console.WriteLine("=== Task.RunTarea: Paralelo con Task.Run ===\n");

            var t1 = Task.Run(() =>
            {
                Console.WriteLine($"[T1] Inicio (thread {Environment.CurrentManagedThreadId})");
                long sum = 0;
                for (int i = 1; i <= N; i++)
                {
                    sum += i;
                    if ((i & 0x3FFFF) == 0) Thread.Sleep(1);
                }
                Console.WriteLine($"[T1] Fin (thread {Environment.CurrentManagedThreadId})");
                return sum;
            });

            var t2 = Task.Run(() =>
            {
                Console.WriteLine($"[T2] Inicio (thread {Environment.CurrentManagedThreadId})");
                long acc = 0;
                for (int i = 1; i <= N; i++)
                {
                    acc += i % 1000;
                    if ((i & 0x4FFFF) == 0) Thread.Sleep(1);
                }
                Console.WriteLine($"[T2] Fin (thread {Environment.CurrentManagedThreadId})");
                return acc;
            });

            var t3 = Task.Run(() =>
            {
                Console.WriteLine($"[T3] Inicio (thread {Environment.CurrentManagedThreadId})");
                double acc = 0;
                for (int i = 1; i <= N; i++)
                {
                    acc += Math.Pow(i % 100, 2);
                    if ((i & 0x2FFFF) == 0) Thread.Sleep(1);
                }
                Console.WriteLine($"[T3] Fin (thread {Environment.CurrentManagedThreadId})");
                return acc;
            });

            var all = Task.WhenAll(t1, t2, t3);
            var monitor = Task.Run(async () =>
            {
                while (!all.IsCompleted)
                {
                    Console.Write("[Principal] trabajando...");
                    await Task.Delay(300);
                }
            });

            await all;
            await monitor;

            Console.WriteLine("\n--- Resultados ---");
            Console.WriteLine($"T1 (sum): {t1.Result}");
            Console.WriteLine($"T2 (acc): {t2.Result}");
            Console.WriteLine($"T3 (powAcc): {t3.Result}");
            Console.WriteLine("=== Fin Task.RunTarea ===");
        }
    }
}
