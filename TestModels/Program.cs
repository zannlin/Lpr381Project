using LPModelsLibrary.Models;

double[,] A = {
            { 11, 8, 6, 14, 10, 10 },
            { 1, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 1 }
        };
double[] b = { 40, 1, 1, 1, 1, 1, 1 };
double[] c = { 2, 3, 3, 5, 2, 4 };
char[] constraintTypes = { 'L', 'L', 'L', 'L', 'L', 'L', 'L' };
bool[] isInteger = { true, true, true, true, true, true };

var result = BranchAndBound.TestRootNode(A, b, c, constraintTypes, isInteger);
var result2 = BranchAndBound.TestChildNodes(A, b, c, constraintTypes, isInteger);
Console.WriteLine(result.ToString());
Console.WriteLine("\n\n=================================================\n\n");
Console.WriteLine(result2.ToString());