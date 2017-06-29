using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RF arf = new RF();
                arf.load("training2016");
                //double[,] csv = arf.lireCSV("C:\\Users\\SkyFox07\\Desktop\\S.E.R.I.O.U.S\\RandomForest Test\\training.csv");
                //arf.construire(csv);
                double[] test ={0.08433082,0.7804754,2.457085,0.06795433,0.156119,2.400094,0.06809825,0.2187633,2.463094,0.0734688,0.5946662,2.471828,-0.1322393,0.4814137,2.466888,-0.3841726,0.6268275,2.374712,-0.5442294,0.7943421,2.273928,-0.5798915,0.8609042,2.251663,0.2928995,0.4662945,2.471816,0.5644839,0.5934131,2.428205,0.7411795,0.7960345,2.36842,0.7882125,0.8916008,2.368788,-0.02221244,0.06426347,2.382554,-0.02780497,-0.4504006,2.441619,-0.03686488,-0.8113767,2.40493,-0.05324397,-0.8867548,2.333922,0.1550516,0.06012384,2.380966,0.1740653,-0.4602699,2.401186,0.1714364,-0.8145916,2.386933,0.1665507,-0.8844827,2.315816};
                //Console.Write("Resultats comparé : 0: " + arf.compare()[1]);
                Console.WriteLine();
                Array.ForEach(arf.classify(test), x => Console.WriteLine(String.Format("{0:F6}", x)));//100
                Array.ForEach(arf.classify(arf.recupPoint()), x => Console.WriteLine(String.Format("{0:F6}", x)));//1
                Console.WriteLine(); 
                //Array.ForEach(test, x => Console.WriteLine(String.Format("{0:F6}", x)));
                Console.WriteLine();
                Console.Write("FIN TRAITEMENT");
                //arf.save("training2016");
                Console.ReadLine();

            //RDFClassifier r=new RDFClassifier();

        }
    }
}
