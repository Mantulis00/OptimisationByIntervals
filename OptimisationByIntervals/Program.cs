using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OptimisationByIntervals
{
    internal class Optimiser
    {
        private static double e = 0.001;
        private static double a = 5, b = 3;

        private double FValue(double x)
        {
            return Math.Pow((x * x - a), 2) / b - 1;
        }
        private double FValueD(double x)
        {
            return 4 / 3 * x * (x * x - 5);
        }

        private double FValueD2(double x)
        {
            return (12 * x * x - 20 ) / 3;
        }

        private delegate void OptimiseMethod(ref double l, ref double r, ref double L);

        internal enum OptimiseMethods
        {
            DevideInHalfs,
            DevideByGoldenRatio,
            Newton,
        }

        private OptimiseMethod SelectedOptimiseMethod (OptimiseMethods method)
        {
            if (method == OptimiseMethods.DevideInHalfs)
                return OptimiseByDevidingInHalf;
            else if (method == OptimiseMethods.DevideByGoldenRatio)
                return OptimiseByDevidingByGoldenRatio;
            else
                return OptimiseByNewton;
        }



        List<List<double>> SamplePoints;

        private void RecordPoints(double l, double r)
        {
            if (SamplePoints == null) SamplePoints = new List<List<double>>();

            if (l==r)
                SamplePoints.Add(new List<double>() { l });
            else
                SamplePoints.Add(new List<double>() { l, r });

        }

        List<string> Results;

        private void PrintRecordPoints()
        {
            if (SamplePoints == null) return;

            Results = new List<string>();

            for (int x = 0; x < SamplePoints.Count; x++)
            {
                Console.WriteLine();
                Results.Add("");
                for (int y = 0; y < SamplePoints[x].Count; y++)
                {
                    Console.WriteLine(SamplePoints[x][y]);
                    Results.Add(SamplePoints[x][y].ToString().Replace(',', '.') + "\t" + FValue(SamplePoints[x][y]).ToString().Replace(',', '.'));
                }
            }
            WriteRecordedPoints();
        }

        private  void WriteRecordedPoints()
        {
             File.WriteAllLinesAsync("SamplePoints.txt", Results);
        }

        internal double Optimise(double l, double r, OptimiseMethods method)
        {
            OptimiseMethod optimiseMethod = SelectedOptimiseMethod(method);
            


            double L = double.PositiveInfinity;
            if (method == OptimiseMethods.Newton)
            {
                double M = (r + l)/2;
                l = M;
                r = M;
            }

            int counter = 0;
            while (Math.Abs(L )> e)
            {
                counter++;
                RecordPoints(l, r);
                optimiseMethod(ref l, ref r, ref L) ;
            }

            PrintRecordPoints();
            Console.WriteLine("\n\nTook: " + counter + " Iterations");
            return (l + r) / 2; ;
        }

        internal void OptimiseByDevidingInHalf(ref double l,ref double r, ref double L)
        {
            L = (r - l) / 2;
            double offset = L / 4;
            double xl = l + offset;
            double xr = r - offset;
            double xm = (l + r) / 2;

            double fm = FValue(xm);
            double fl = FValue(xl);
            double fr = FValue(xr);

            if (fl >= fm && fr >= fm)
            {
                l = xl;
                r = xr;
            }
            else if (fl <= fm)
                r = xm;
            else if (fr <= fm)
                l = xm;
        }

        private static double tau = 0.61803398875;
        internal void OptimiseByDevidingByGoldenRatio(ref double l, ref double r, ref double L)
        {
            L = (r - l) / 2;
            double offset = L * tau;
            double xl = l + offset;
            double xr = r - offset;

            double fl = FValue(xl);
            double fr = FValue(xr);

            if (fr < fl)
            {
                l = xr ;
                r = l + offset;
            }
            else 
            {
                r = xl ;
                l = r - offset;
            }
        }

        internal void OptimiseByNewton(ref double l, ref double r, ref double L)
        {
            double xi = (l+r)/ 2;
            L = FValueD(xi) / Math.Abs(FValueD2(xi));
            l = xi - L;
            r = l;
            Console.WriteLine(l);
        }
    }


    class Program
    {
        private static double from = 0, to = 10;

        static void Main(string[] args)
        {
            Optimiser optimiser = new Optimiser();
            double xmin = optimiser.Optimise(from, to, Optimiser.OptimiseMethods.DevideByGoldenRatio);


            Console.WriteLine(xmin);
        }
    }
}
