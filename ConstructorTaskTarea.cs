using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    public class ConstructorTaskTarea
    {
        public static void Run()
        {
            const int N = 200_000;
            Console.WriteLine("=== constructor TaskTarea: new Task() y Start() ===\n");

            var t1 = new Task<long>(() =>
            {
                Console.WriteLine($"[T1] Inicia (thread {Environment.CurrentManagedThreadId})");
                long acc = 0;
                for (int i = 1; i <= N; i++) acc += (long)i * i;
                Console.WriteLine($"[T1] Termina");
                return acc;
            });

            var t2 = new Task<long>(() =>
            {
                Console.WriteLine($"[T2] Inicia (thread {Environment.CurrentManagedThreadId})");
                long acc = 0;
                for (int i = 1; i <= N; i++) acc += (long)i * i * i;
                Console.WriteLine($"[T2] Termina");
                return acc;
            });

            var t3 = new Task<long>(() =>
            {
                Console.WriteLine($"[T3] Inicia (thread {Environment.CurrentManagedThreadId})");
                long count = 0;
                for (int i = 2; i <= N; i++)
                {
                    bool prime = true;
                    for (int d = 2; d * d <= i; d++)
                        if (i % d == 0) { prime = false; break; }
                    if (prime) count++;
                }
                Console.WriteLine($"[T3] Termina");
                return count;
            });

            var t4 = new Task<BigInteger>(() =>
            {
                Console.WriteLine($"[T4] Inicia (thread {Environment.CurrentManagedThreadId})");
                int smallN = 1000;
                BigInteger prod = 1;
                for (int i = 1; i <= smallN; i++) prod *= i;
                Console.WriteLine($"[T4] Termina");
                return prod;
            });

            Console.WriteLine("[Principal] Tareas creadas, aÃºn no iniciadas.\n");

            t1.Start(); t2.Start(); t3.Start(); t4.Start();

            Task.WaitAll(t1, t2, t3, t4);

            Console.WriteLine("\n--- Resultados ---");
            Console.WriteLine($"Suma cuadrados (T1): {t1.Result}");
            Console.WriteLine($"Suma cubos (T2): {t2.Result}");
            Console.WriteLine($"Primos contados (T3): {t3.Result}");
            Console.WriteLine($"Factorial bytes (T4): {t4.Result.ToByteArray().LongLength}");
            Console.WriteLine("=== Fin constructor TaskTarea ===");
        }
    }
}
