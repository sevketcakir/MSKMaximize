using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Genetic;

namespace AFGeneticDizi
{
    class Program
    {
        static void Main(string[] args)
        {

            //Örnek 1: Population size=10, iteration count=20
            Console.WriteLine("Örnek 1:");
            Population p = new Population(10, new MSKKromozom(8), new MSKFitness(), new EliteSelection());
            for (int i = 0; i < 20; i++)//20 nesil
            {
                p.RunEpoch();
                MSKKromozom k = (MSKKromozom)p.BestChromosome;
                Console.WriteLine("En iyi uygunluk:{0} dizi:{1}", k.Fitness, String.Join(",", k.dizi));
            }
            //Örnek 2:Başlangıç dizisini kendimiz belirliyoruz. Population size=20, iteration count=10
            Console.WriteLine("Örnek 2:");
            MSKKromozom baslangicKromozomu = new MSKKromozom(new int[] { 17,4,1,6,8,44,20,22,19,100,95,255,26,52,9,2});
            
            //Seçme algoritmasını rastgele belirle
            ISelectionMethod f = new EliteSelection();
            int r = new Random().Next(3);
            if (r == 1)
            {
                f = new RankSelection();
                Console.WriteLine("Seçim Algoritması: RankSelection");
            }
            else if (r == 2)
            {
                f = new RouletteWheelSelection();
                Console.WriteLine("Seçim Algoritması: RouletteWheelSelection");
            }
            else
                Console.WriteLine("Seçim Algoritması: EliteSelection");

            p = new Population(50, baslangicKromozomu, new MSKFitness(), f);
            p.CrossoverRate = 0.5;
            p.MutationRate = 0.2;
            for (int i = 0; i < 250; i++)//300 nesil
            {
                p.RunEpoch();
                MSKKromozom k = (MSKKromozom)p.BestChromosome;
                Console.WriteLine("En iyi uygunluk:{0} dizi:{1}", k.Fitness, String.Join(",", k.dizi));
            }

            //Bitiş beklemesi
            Console.ReadKey();
        }
    }
}
