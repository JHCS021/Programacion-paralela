using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    public class TaskCreationOptionsAttachedToParentTarea
    {
        public static async Task Run()
        {
            Console.WriteLine("=== TaskCreationOptions.AttachedToParentTarea ===\n");

            Task parent = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"[Padre] Inicia (thread {Environment.CurrentManagedThreadId})");

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("  [Hija1] Inicia");
                    Thread.Sleep(800);
                    Console.WriteLine("  [Hija1] Termina");
                }, TaskCreationOptions.AttachedToParent);

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("  [Hija2] Inicia");
                    Thread.Sleep(500);
                    Console.WriteLine("  [Hija2] Termina");
                }, TaskCreationOptions.AttachedToParent);

                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("  [HijaNoAttach] Inicia");
                    Thread.Sleep(300);
                    Console.WriteLine("  [HijaNoAttach] Termina");
                });

                Console.WriteLine("[Padre] Fin mÃ©todo padre (esperarÃ¡ hijas attach).");
            });

            await parent;
            Console.WriteLine("\n[Principal] Padre completado (incluye hijas AttachedToParent).");
            Console.WriteLine("=== Fin TaskCreationOptions.AttachedToParentTarea ===");
        }
    }
}
