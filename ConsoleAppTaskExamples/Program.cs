using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTaskExamples
{
    class Program
    {

        //static CancellationTokenSource cts = new CancellationTokenSource();

        //static TaskFactory<int> factory = new TaskFactory<int>(
        //   cts.Token,
        //   TaskCreationOptions.PreferFairness,
        //   TaskContinuationOptions.ExecuteSynchronously,
        //   new CustomScheduler());

        static  void Main(string[] args)
        {
            //TaskTesting();

            //TaskRunAsync();
            //TaskFactoryStartNew();


            TaskResult();

            TaskFactoryArray();
            Console.ReadKey();
        }

        static void TaskTesting()
        {
            Action<object> action = (object obj) => 
            {
                Console.WriteLine($"Task: {Task.CurrentId}," +
                    $" Obj: {obj}, " +
                    $"Thread: {Thread.CurrentThread.ManagedThreadId}");
            };

            //Creado pero no iniciado
            Task task1 = new Task(action, "alpha");

            //Crea un Task y lo inicia
            Task task2 = Task.Factory.StartNew(action, "beta");
            //Bloquea el principal hilo para demostra que t2 se esta ejecutando
            task2.Wait();

            //Inicia task1
            task1.Start();
            Console.WriteLine($"t1 has started. Managed Id: {Thread.CurrentThread.ManagedThreadId}");
            //Espera a que la tarea termine
            task1.Wait();

            string taskData = "delta";

            Task task3 = Task.Run(() => {
                Console.WriteLine($"Task: {Task.CurrentId}, Obj:{taskData}, Thread: {Thread.CurrentThread.ManagedThreadId}");
            });

            task3.Wait();


            Task task4 = new Task(action, "gamma");
            task4.RunSynchronously();

            task4.Wait();


        }


        static async void TaskRunAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                int counter = 0;
            for(counter = 0; counter<10000000; counter++) { }
                Console.WriteLine(counter);
            });
        }

        static void TaskFactoryStartNew()
        {
            Task t = Task.Factory.StartNew(() => {
                // Just loop.
                int ctr = 0;
                for (ctr = 0; ctr <= 1000000; ctr++)
                { }
                Console.WriteLine("Finished {0} loop iterations",
                                  ctr);
            });
            t.Wait();
        }

        static void TaskResult()
        {
            var t = Task<int>.Run<int>(() =>
            {
                // Just loop.
                int max = 1000000;
                int ctr = 0;
                for (ctr = 0; ctr <= max; ctr++)
                {
                    if (ctr == max / 2 && DateTime.Now.Hour <= 12)
                    {
                        ctr++;
                        break;
                    }
                }
                return ctr;
            });

            Console.WriteLine($"Result: {t.Result.ToString("N0")}");
        }

        static void TaskFactoryArray()
        {
            Task<string[]>[] tasks = new Task<string[]>[2];

            string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            tasks[0] = Task<string[]>.Factory.StartNew(() => Directory.GetDirectories(pathDocument));
            tasks[1] = Task<string[]>.Factory.StartNew(() => Directory.GetFiles(pathDocument));

            Task.Factory.ContinueWhenAll(tasks, continueAction =>
            {
                Console.WriteLine($"Path Documents {pathDocument} contains:");
                Console.WriteLine($"Directories: {tasks[0].Result.Length}");
                Console.WriteLine($"Files: {tasks[1].Result.Length}");
            });
        }
    }
}
