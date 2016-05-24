using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Genetic;
namespace AFGeneticDizi
{
    /// <summary>
    /// Merge sort için en kötü(en çok karşılaştırmanın yapılacağı) durumu bulur.
    /// </summary>
    class MSKKromozom : ChromosomeBase
    {
        /// <summary>
        /// Yerleşimi tutan dizi
        /// </summary>
        public int[] dizi;

        /// <summary>
        /// Sınıfın tamamında kullanılacak Random rastgele sayı üreteci
        /// </summary>
        Random r;

        /// <summary>
        /// Verilen boyut büyüklüğünde dizi oluşturur ve 1'den boyuta kadar sayıları içine yerleştirir
        /// </summary>
        /// <param name="boyut">Dizinin büyüklüğünü belirler</param>
        public MSKKromozom(int boyut)
        {
            r = new Random();
            dizi = new int[boyut];
            for (int i = 0; i < boyut; i++)
            {
                dizi[i] = i + 1;
            }
            karistir();
            Evaluate(new MSKFitness());//EliteSelection dışındaki seçme algoritmalarında fitness değerini güncellemiyor. O nedenle eklendi
        }
        /// <summary>
        /// Parametre olarak verilen dizinin kopyasını oluşturarak yeni bir MSKKromozom oluşturur
        /// </summary>
        /// <param name="dizi">kopyası oluşturulacak dizi</param>
        public MSKKromozom(int[] dizi)
        {
            r = new Random();
            this.dizi = (int[])dizi.Clone();//Deep copy gerekli, int dizisi olduğu için problem yok
            Evaluate(new MSKFitness());//EliteSelection dışındaki seçme algoritmalarında fitness değerini güncellemiyor. O nedenle eklendi
        }

        /// <summary>
        /// Mevcut diziyi karıştırır(shuffle)
        /// </summary>
        public void karistir()
        {
            for (int i = 0; i < dizi.Length; i++)
            {
                int konum = r.Next(dizi.Length);
                int temp = dizi[i];
                dizi[i] = dizi[konum];
                dizi[konum] = temp;
            }
        }
        /// <summary>
        /// Koromozomun bir kopyasını oluşturur. dizideki sayıların yerleşimi birebir aynıdır.
        /// </summary>
        /// <returns>kopyalanmış kromozom</returns>
        public override IChromosome Clone()
        {
            return new MSKKromozom(dizi);
        }

        /// <summary>
        /// Mevut kromozomu kullanarak yeni bir kromozom kopyası oluşturulur ve dizi karıştırılır
        /// </summary>
        /// <returns>Yeni karıştırılmış kromozom</returns>
        public override IChromosome CreateNew()
        {
            MSKKromozom yeni = new MSKKromozom(dizi);
            yeni.karistir();
            return yeni;
        }
        /// <summary>
        /// Çaprazlama işlemini yapar.
        /// </summary>
        /// <remarks>
        /// 1. ebeveyn olarak içinde bulunulan nesne kullanılır. 2. ebeveyn parametre olarak gelen kromozomdur.
        /// Her iki ebeveynin de Population sınıfının içindeki Crossover metodunda birer kopyası oluşturulur ve
        /// çaprazlama işlemi bu kopyalar üzerinde gerçekleşir. Dolayısıyla 2 adet çocuk oluşur. Ebeveynlerin
        /// kopyası çaprazlama sonucunda oluşturulan çocuklara dönüşür.
        /// </remarks>
        /// <param name="pair">2. ebeveyn</param>
        public override void Crossover(IChromosome pair)
        {
            MSKKromozom p = (MSKKromozom)pair;
            List<int> tumSayilar = new List<int>(dizi);//tüm sayıları listede tut
            int[] cd1 = (int[])dizi.Clone();//Çocuk dizi 1
            int[] cd2 = (int[])p.dizi.Clone();//Çocuk dizi 2
            int k = r.Next(dizi.Length - 1) + 1;//Çaprazlama noktası
            //int k = dizi.Length / 2;//Çaprazlama noktası ortadan(üstte rastgele seçim var)
            for (int i = 0; i < dizi.Length; i++)
            {
                if (i < k)
                    cd1[i] = cd2[i];
                else
                    cd2[i] = cd1[i];
            }

            TekrarlariSilOlmayaniYerlestir(cd1, tumSayilar);
            TekrarlariSilOlmayaniYerlestir(cd2, tumSayilar);
            dizi = cd1;
            p.dizi = cd2;
        }

        /// <summary>
        /// Çaprazlama sonrası dizide tekrarlı elemanları silip yerlerine dizide olmayanları yerleştirir
        /// Dizideki değerler ayrık olmalı. Aynı değerden iki tane bulunursa aşağıdaki fonksiyon exception fırlatır
        /// </summary>
        /// <param name="d">Çaprazlanmış tekrarlı kayıt içeren ve bazı elemanları içermeyen dizi</param>
        /// <param name="l">Tüm elemanların bulunduğu liste</param>
        void TekrarlariSilOlmayaniYerlestir(int[] d, List<int> l)
        {
            List<int> c1ts = new List<int>(l);//Tüm sayıların kopyasını oluştur
            List<int> c1mvc = new List<int>();//Mevcut sayıların tutulacağı liste
            for (int i = 0; i < d.Length; i++)
                c1ts.Remove(d[i]);//Dizide olanları listeden sil

            for (int i = 0; i < d.Length; i++)
            {
                if (c1mvc.Contains(d[i]))//mevcut listede varsa
                {
                    int ind = r.Next(c1ts.Count);//Olmayan rastgele bir elemanı seç
                    d[i] = c1ts[ind];//Olmayanı dizinin elemanına yaz
                    c1ts.Remove(c1ts[ind]);//eleman artık dizide bulunuyor, listeden sil

                }
                else
                    c1mvc.Add(d[i]);//mevcut listesine ekle
            }
        }
        /// <summary>
        /// Mevcut kromozomdaki dizilimi değiştirir
        /// </summary>
        public override void Generate()
        {
            karistir();
        }

        /// <summary>
        /// Mutasyon işlemi. Rastgele iki elemanın yerini değiştirir.
        /// </summary>
        public override void Mutate()
        {
            int k1 = r.Next(dizi.Length);
            int k2 = r.Next(dizi.Length);
            int temp = dizi[k1];
            dizi[k1] = dizi[k2];
            dizi[k2] = temp;
        }
    }
}
