using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RandomForest_Test
{
    class RF
    {
        private alglib.decisionforest rf;
        private int longue;
        private int larg;

        public RF()
        {
           /* classify(new double[] { 0.2, 1.3 });

            save("test.rdf");
            load("test.rdf");*/
        }

        public double [,] lireCSV(String chemin) {
            //code issu de http://csharphelper.com/blog/2014/11/read-a-csv-file-into-an-array-in-c/
            // Get the file's text.
            string whole_file = System.IO.File.ReadAllText(chemin);

            // Split into lines.
            whole_file = whole_file.Replace('\n', '.');
            string[] lines = whole_file.Split(new char[] { '.' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            longue = lines.Length;
            larg = lines[0].Split(';').Length;
            Console.Write("longue = Lignes" +longue);
            Console.WriteLine();
            Console.Write("larg = var" + larg);
            Console.WriteLine();

            // Allocate the data array.
            double[,] values = new double[longue, larg];

            // Load the array.
            for (int r = 1; r < longue; r++)
            {
                string[] line_r = lines[r].Split(';');
                for (int c = 0; c < larg; c++)
                {
                    values[r, c] = double.Parse(line_r[c]);
                    /*Console.Write(values[r, c]);
                    Console.Write("-----------");*/
                    
                }
                //Console.WriteLine();
                
            }
            

           
            // Return the values.
            return values;

        }
        public void construire(double[,] lu)
        {
             
            rf = new alglib.decisionforest();
            int npoints = longue-1;
            int nvars = larg-1;
            int nclasses = 1;
            int ntrees = 100;
            int info = 0;
            double r = 0.66;
            alglib.dfreport rep = new alglib.dfreport();
            alglib.dfbuildrandomdecisionforest(lu, npoints, nvars, nclasses, ntrees, r,  out info, out rf, out rep);
            //Entrée, Samples(nombre de sets), nbFeatures20, nclasse4, trees 100, r 0.66,...
            

        }

        
        public void capture(int delais)
        {

        }

        public double[] compare()
        {
            double[] a = new double[longue*larg];
            a = recupPoint();
            double [] result = new double[100];
            alglib.dfprocess(rf, a, ref result);
            return result;
        }

        public double[] recupPoint()
        {
            double[] test =
            {
                                            0.09773261,0.7722498,2.465372,
                                            0.04864892,0.1286325,2.391867,
                                            0.04674485,0.1914105,2.458848,
                                            0.07321983,0.582469,2.487596,
                                            -0.1364136,0.4701006,2.457186,
                                            -0.3756322,0.5818073,2.352197,
                                            -0.5264673,0.7829014,2.25194,
                                            -0.5710838,0.8443389,2.223359,
                                            0.2525386,0.3924423,2.480077,
                                            0.2783741,0.1957804,2.51267,
                                            0.2449274,-0.02161448,2.355518,
                                            0.2512022,-0.1050758,2.377504,
                                            -0.04203493,0.04833375,2.36372,
                                            -0.04322145,-0.4813257,2.438059,
                                            -0.03522355,-0.8314683,2.40165,
                                            -0.06385408,-0.8918286,2.324171,
                                            0.1309389,0.03553721,2.378356,
                                            0.163982,-0.4944602,2.399478,
                                            0.167676,-0.833747,2.384649,
                                            0.1717297,-0.891655,2.303471
            };
            return test;

        }

        public void load(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string str = sr.ReadToEnd(); //READ
                    alglib.dfunserialize(str, out rf);
                    sr.Close();
                }
            }
            catch (Exception)
            {
                // ends up here :-(
            }
        }

        public void save(string fileName)
        {
            if (rf == null)
            {
                return;
            }
            string str = "";
            alglib.dfserialize(rf, out str);
            using (TextWriter tw = new StreamWriter(fileName, false))
            {
                tw.WriteLine(str);
                tw.Close();
            }
        }


        public double[] classify(double[] data)
        {
            if (rf == null)
            {
                throw new InvalidOperationException("Random Decision Forest not initialized");
            }

            double[] result = new double[1];
            alglib.dfprocess(rf, data, ref result);
            return result;
        }




       
    }
}