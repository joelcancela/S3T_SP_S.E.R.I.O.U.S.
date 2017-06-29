namespace RandomForest_Test
{
    public class testdforestunit
    {
        public static bool testdforest(bool silent)
        {
            bool result = new bool();
            int ncmax = 0;
            int nvmax = 0;
            int passcount = 0;
            int nvars = 0;
            int nclasses = 0;
            bool waserrors = new bool();
            bool basicerrors = new bool();
            bool procerrors = new bool();


            //
            // Primary settings
            //
            nvmax = 4;
            ncmax = 3;
            passcount = 10;
            basicerrors = false;
            procerrors = false;
            waserrors = false;

            //
            // Tests
            //
            testprocessing(ref procerrors);
            for (nvars = 1; nvars <= nvmax; nvars++)
            {
                for (nclasses = 1; nclasses <= ncmax; nclasses++)
                {
                    basictest1(nvars, nclasses, passcount, ref basicerrors);
                }
            }
            basictest2(ref basicerrors);
            basictest3(ref basicerrors);
            basictest4(ref basicerrors);
            basictest5(ref basicerrors);

            //
            // Final report
            //
            waserrors = basicerrors || procerrors;
            if (!silent)
            {
                System.Console.Write("RANDOM FOREST TEST");
                System.Console.WriteLine();
                System.Console.Write("TOTAL RESULTS:                           ");
                if (!waserrors)
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* PROCESSING FUNCTIONS:                  ");
                if (!procerrors)
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* BASIC TESTS:                           ");
                if (!basicerrors)
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                if (waserrors)
                {
                    System.Console.Write("TEST SUMMARY: FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST SUMMARY: PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }
    }
}