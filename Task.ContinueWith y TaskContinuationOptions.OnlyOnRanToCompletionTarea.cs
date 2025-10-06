using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramacionParalela
{
    public class TaskContinueWithOnlyOnRanToCompletionTarea
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Task.ContinueWith y TaskContinuationOptions.OnlyOnRanToCompletionTarea ===\n");

            var hornear = Task.Run(() =>
            {
                Console.WriteLine("HornearGalletas: inicio");
                Thread.Sleep(2000);
                Console.WriteLine("HornearGalletas: fin");
                return 24;
            });

            var prepararEmpaque = Task.Run(() =>
            {
                Console.WriteLine("PrepararEmpaque: inicio");
                Thread.Sleep(1000);
                Console.WriteLine("PrepararEmpaque: fin");
                return true;
            });

            var enfriar = hornear.ContinueWith(t =>
            {
                Console.WriteLine("  EnfriarGalletas: inicio");
                Thread.Sleep(1500);
                Console.WriteLine("  EnfriarGalletas: fin");
                return t.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);

            var both = Task.WhenAll(hornear, prepararEmpaque);

            var empacar = both.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    Console.WriteLine("    EmpacarGalletas: inicio");
                    Thread.Sleep(1000);
                    Console.WriteLine("    EmpacarGalletas: fin");
                    return true;
                }
                else
                {
                    Console.WriteLine("    EmpacarGalletas: no ejecutada");
                    return false;
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            var enviar = empacar.ContinueWith(t =>
            {
                Console.WriteLine("      EnviarATienda: inicio");
                Thread.Sleep(1000);
                Console.WriteLine("      EnviarATienda: fin");
            });

            await Task.WhenAll(enfriar, empacar, enviar);
            Console.WriteLine("\nTodas las tareas completadas.");
            Console.WriteLine("=== Fin Task.ContinueWith y TaskContinuationOptions.OnlyOnRanToCompletionTarea ===");
        }
    }
}
