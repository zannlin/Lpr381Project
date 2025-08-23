// Test your stuff here!
using LPModelsLibrary;
using LPModelsLibrary.Models;
using System;
using System.IO;
using System.Linq;
class Program
{

    static void Main(string[] args)
    {
        string[] content = readContents();
        LinearModel model = new LinearModel(content);
        double[] objectiveFunction = model.ObjectiveFuntion;
        double[,] constraint = model.Constraints;
        double[] rhs = model.RightHandSide;
        SimplexResult result = PrimalSimplex.Solve(constraint, rhs, objectiveFunction);

        CuttingPlaneSimplex cp = new CuttingPlaneSimplex(result);
        foreach (var item in result.Tableaus)
        {
            Console.WriteLine(item.ToString());
        }
        cp.solveCuttingPlane();
        Console.WriteLine(cp.cuttingPlaneResult.printTables());

    }
    
    public static string[] readContents()
    {
        string filepath = @"C:\\Users\\thape\\source\\repos\\Lpr381Project\\LPModelsLibrary\\Models\\Data.txt";
        string[] content = File.ReadAllLines(filepath);
        return content;
    }
}

