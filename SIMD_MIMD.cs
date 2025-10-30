
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace PlatformsAlghoritms
{
    // Interface defining the operation to be performed.
    public interface IOperation
    {
        Task Iniciar();
    }

    // Main program entry point.
    internal class Program
    {
        static async Task Main()
        {
            // To run a different example, change the class instantiation here.
            // For example: 
            // IOperation operacion = new Ejemplo1();
            // IOperation operacion = new Ejemplo3();
            // IOperation operacion = new ejemplo4();
            // IOperation operacion = new ejemplo4_1();
            // IOperation operacion = new ejemplo5_SIMD();
            // IOperation operacion = new ejemplo5_MIMD();
            // IOperation operacion = new ejemplo6_MIMD_Condicional();
            IOperation operacion = new ejemplo6_SIMD_Condicional();
            await operacion.Iniciar();
        }
    }

    // Example 1: Efficient sequential memory access.
    public class Ejemplo1 : IOperation
    {
        // Eficiente sin saltos en memoria y acceso secuencial
        public Task Iniciar()
        {
            return Task.Run(() =>
            {
                const int N = 100_000_000;
                int[] arreglo = new int[N];
                int suma = 0;

                // Inicialización
                for (int i = 0; i < N; i++)
                {
                    arreglo[i] = i % 100;
                }

                Stopwatch sw = Stopwatch.StartNew();

                // Sumar los elementos (alta carga de CPU y acceso a memoria secuencial)
                for (int i = 0; i < N; i++)
                {
                    suma += arreglo[i];
                }

                sw.Stop();

                Console.WriteLine($"La suma de {N} elementos es: {suma}");
                Console.WriteLine($"Tiempo de ejecución: {sw.ElapsedMilliseconds} ms");
            });
        }
    }

    // Example 2: Inefficient non-sequential memory access.
    public class Ejemplo2 : IOperation
    {
        // Ineficiente con saltos grandes en memoria y acceso no secuencial
        public Task Iniciar()
        {
            return Task.Run(() =>
            {
                const int N = 100_000_000;
                const int STRIDE = 64; // Saltos grandes entre accesos

                int[] arreglo = new int[N];
                int suma = 0;

                // Inicializar el arreglo
                for (int i = 0; i < N; i++)
                {
                    arreglo[i] = i % 100;
                }

                Stopwatch sw = Stopwatch.StartNew();

                // Acceso ineficiente: recorremos con saltos grandes (stride)
                for (int offset = 0; offset < STRIDE; offset++)
                {
                    for (int i = offset; i < N; i += STRIDE)
                    {
                        suma += arreglo[i];
                    }
                }

                sw.Stop();

                Console.WriteLine($"Suma con acceso ineficiente: {suma}");
                Console.WriteLine($"Tiempo de ejecución: {sw.ElapsedMilliseconds} ms");
            });
        }
    }

    // Example 3: SIMD (Single Instruction, Multiple Data) method.
    public class Ejemplo3 : IOperation
    {
        public Task Iniciar()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("=== Método SIMD ===");

                float[] a = new float[100_000_000];
                float[] b = new float[100_000_000];
                float[] result = new float[100_000_000];
                float totalSum = 0;

                // Inicializar arrays con valores
                for (int it = 0; it < a.Length; it++)
                {
                    a[it] = it % 100;
                    b[it] = it % 100;
                }

                int vectorSize = Vector<float>.Count;
                Console.WriteLine($"Vector<float>.Count: {Vector<float>.Count}");
                Console.WriteLine($"Is hardware accelerated: {Vector.IsHardwareAccelerated}");

                Stopwatch sw = Stopwatch.StartNew();

                int i = 0;

                // Process arrays using SIMD vectors for acceleration.
                for (i = 0; i <= a.Length - vectorSize; i += vectorSize)
                {
                    var va = new Vector<float>(a, i);
                    var vb = new Vector<float>(b, i);
                    var vr = va + vb;
                    vr.CopyTo(result, i);
                    totalSum += Vector.Dot(vr, Vector<float>.One);
                }

                // Procesar elementos restantes secuencialmente
                for (; i < a.Length; i++)
                {
                    result[i] = a[i] + b[i];
                }

                sw.Stop();
                Console.WriteLine($"Tiempo SIMD: {sw.ElapsedMilliseconds} ms");
                Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[^1]}");
                Console.WriteLine($"El resultado es: {totalSum}");
                Console.WriteLine();
            });
        }
    }
    
    public class ejemplo4 : IOperation
   {
       public async Task Iniciar()
       {
           Console.WriteLine("=== Método Secuencial ===");

           float[] a = new float[100000000];
           float[] b = new float[100000000];
           float[] result = new float[100000000];

           // Inicializar arrays con los mismos valores
           for (int i = 0; i < a.Length; i++)
           {
               a[i] = i % 100;
               b[i] = i % 100;
           }

           Stopwatch sw = Stopwatch.StartNew();

           // Suma secuencial elemento por elemento
           for (int i = 0; i < a.Length; i++)
           {
               result[i] = a[i] + b[i];
           }

           sw.Stop();
           Console.WriteLine($"Tiempo Secuencial: {sw.ElapsedMilliseconds} ms");
           Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
           Console.WriteLine($"Resultado total: {result.Sum()}");
           Console.WriteLine();
       }
   }

    public class ejemplo4_1 : IOperation
    {
        private readonly object _object = new object();

        public async Task Iniciar()
        {
            Console.WriteLine("=== Método Secuencial Mejorado ===");

            float[] a = new float[100000000];
            float[] b = new float[100000000];
            float[] result = new float[100000000];
            float totalSum = 0; // Acumulador para la suma total

            // Inicializar arrays con los mismos valores
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = i % 100;
                b[i] = i % 100;
            }

            int numTasks = Environment.ProcessorCount;
            int chunkSize = a.Length / numTasks;
            var tasks = new List<Task<float>>();

            Stopwatch sw = Stopwatch.StartNew();

            for (int t = 0; t < numTasks; t++)
            {
                int start = t * chunkSize;
                int end = (t == numTasks - 1) ? a.Length : start + chunkSize;
                tasks.Add(Task.Run(() =>
                {
                    float localSum = 0;
                    for (int i = start; i < end; i++)
                    {
                        result[i] = a[i] + b[i];
                        localSum += result[i];
                    }
                    return localSum;
                }));
            }

            var partialSums = await Task.WhenAll(tasks);
            totalSum = partialSums.Sum();

            sw.Stop();
            Console.WriteLine($"Tiempo Secuencial Mejorado: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
            Console.WriteLine($"El resultado es: {totalSum}");
            Console.WriteLine();
        }
    }

    public class ejemplo5_SIMD : IOperation
    {
        public async Task Iniciar()
        {
            Console.WriteLine("=== Ejemplo 5: SIMD ((a + b) * b) ===");

            float[] a = new float[100_000_000];
            float[] b = new float[100_000_000];
            float[] result = new float[100_000_000];
            float totalSum = 0;

            for (int it = 0; it < a.Length; it++)
            {
                a[it] = it % 100;
                b[it] = it % 100;
            }

            int vectorSize = Vector<float>.Count;
            Stopwatch sw = Stopwatch.StartNew();

            int i = 0;
            for (; i <= a.Length - vectorSize; i += vectorSize)
            {
                var va = new Vector<float>(a, i);
                var vb = new Vector<float>(b, i);
                var vr = (va + vb) * vb;
                vr.CopyTo(result, i);
                totalSum += Vector.Dot(vr, Vector<float>.One);
            }
            for (; i < a.Length; i++)
            {
                result[i] = (a[i] + b[i]) * b[i];
            }

            sw.Stop();
            Console.WriteLine($"Tiempo SIMD: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
            Console.WriteLine($"El resultado es: {totalSum}");
            Console.WriteLine();
        }
    }

    public class ejemplo5_MIMD : IOperation
    {
        public async Task Iniciar()
        {
            Console.WriteLine("=== Ejemplo 5: MIMD ((a + b) * b) en paralelo ===");

            float[] a = new float[100_000_000];
            float[] b = new float[100_000_000];
            float[] result = new float[100_000_000];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = i * 0.5f;
                b[i] = i * 0.3f;
            }

            int numTasks = Environment.ProcessorCount;
            int chunkSize = a.Length / numTasks;
            var tasks = new List<Task>();

            Stopwatch sw = Stopwatch.StartNew();

            for (int t = 0; t < numTasks; t++)
            {
                int start = t * chunkSize;
                int end = (t == numTasks - 1) ? a.Length : start + chunkSize;
                tasks.Add(Task.Run(() =>
                {
                    for (int i = start; i < end; i++)
                    {
                        result[i] = (a[i] + b[i]) * b[i];
                    }
                }));
            }

            await Task.WhenAll(tasks);

            sw.Stop();
            Console.WriteLine($"Tiempo MIMD: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
            Console.WriteLine();
        }
    }

  public class ejemplo6_MIMD_Condicional : IOperation
  {
      public async Task Iniciar()
      {
          Console.WriteLine("=== Ejemplo 6: MIMD con lógica condicional heterogénea ===");

          float[] a = new float[100_000_000];
          float[] b = new float[100_000_000];
          float[] result = new float[100_000_000];

          for (int i = 0; i < a.Length; i++)
          {
              a[i] = i % 3 == 0 ? i * 0.5f : i * 0.3f;
              b[i] = i % 2 == 0 ? i * 0.2f : i * 0.4f;
          }

          int numTasks = Environment.ProcessorCount;
          int partitionSize = a.Length / numTasks;
          var tasks = new List<Task>();

          Stopwatch sw = Stopwatch.StartNew();

          for (int t = 0; t < numTasks; t++)
          {
              int start = t * partitionSize;
              int end = (t == numTasks - 1) ? a.Length : start + partitionSize;
              tasks.Add(Task.Run(() =>
              {
                  for (int i = start; i < end; i++)
                  {
                      if (a[i] > b[i])
                          result[i] = a[i] + b[i];
                      else if (a[i] < b[i])
                          result[i] = a[i] - b[i];
                      else
                          result[i] = a[i] * b[i];
                  }
              }));
          }

          await Task.WhenAll(tasks);

          sw.Stop();
          Console.WriteLine($"Tiempo MIMD condicional: {sw.ElapsedMilliseconds} ms");
          Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
          Console.WriteLine();
      }
  }
public class ejemplo6_SIMD_Condicional : IOperation
{
    public async Task Iniciar()
    {
        Console.WriteLine("=== Ejemplo 6: SIMD con lógica condicional heterogénea ===");

        float[] a = new float[100_000_000];
        float[] b = new float[100_000_000];
        float[] result = new float[100_000_000];

        for (int it = 0; it < a.Length; it++)
        {
            a[it] = it % 3 == 0 ? it * 0.5f : it * 0.3f;
            b[it] = it % 2 == 0 ? it * 0.2f : it * 0.4f;
        }

        int vectorSize = Vector<float>.Count;
        Stopwatch sw = Stopwatch.StartNew();

        int i = 0;
        for (; i <= a.Length - vectorSize; i += vectorSize)
        {
            var va = new Vector<float>(a, i);
            var vb = new Vector<float>(b, i);

            // Simular condicionales usando máscaras
            var maskGreater = Vector.GreaterThan(va, vb);
            var maskLess = Vector.LessThan(va, vb);
            var maskEqual = Vector.Equals(va, vb);

            var sum = va + vb;
            var sub = va - vb;
            var mul = va * vb;

            // Seleccionar resultado según condición
            var vr = Vector.ConditionalSelect(maskGreater, sum,
                        Vector.ConditionalSelect(maskLess, sub, mul));

            vr.CopyTo(result, i);
        }

        // Procesar los elementos restantes escalarmente
        for (; i < a.Length; i++)
        {
            if (a[i] > b[i])
                result[i] = a[i] + b[i];
            else if (a[i] < b[i])
                result[i] = a[i] - b[i];
            else
                result[i] = a[i] * b[i];
        }

        sw.Stop();
        Console.WriteLine($"Tiempo SIMD condicional: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Primer resultado: {result[0]}, Último: {result[result.Length - 1]}");
        Console.WriteLine();
    }
}
}
