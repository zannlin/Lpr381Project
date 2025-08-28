using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class RevisedPrimalSimplex
    {
        public static SimplexResult Solve(double[,] A, double[] b, double[] c, double eps = 1e-9)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            if (b.Any(x => x < -eps))
            {
                return new SimplexResult
                {
                    IsInfeasible = true,
                    Message = "Infeasible: RHS contains negative values. Phase I required."
                };
            }

            // Initial basis (slack variables)
            int[] basis = Enumerable.Range(n, m).ToArray();

            // Initial B inverse (identity)
            double[,] B_inv = new double[m, m];
            for (int i = 0; i < m; i++) B_inv[i, i] = 1.0;

            // Initial x_B = b
            double[] x_B = (double[])b.Clone();

            // Headers
            string[] colHeaders = new string[n + m + 1];
            for (int j = 0; j < n; j++) colHeaders[j] = $"x{j + 1}";
            for (int j = 0; j < m; j++) colHeaders[n + j] = $"s{j + 1}";
            colHeaders[n + m] = "rhs";

            string[] rowHeaders = new string[m + 1];
            for (int i = 0; i < m; i++) rowHeaders[i] = colHeaders[basis[i]];
            rowHeaders[m] = "z";

            var result = new SimplexResult();
            int iteration = 0;

            // Build initial tableau for T-1
            double[,] initialT = BuildInitialTableau(A, b, c, m, n, eps);
            result.Tableaus.Add(new TableauTemplate(initialT, colHeaders, rowHeaders, iteration + 1, null, null, $"T-{iteration + 1}"));

            StringBuilder output = new StringBuilder();
            output.AppendLine("===");
            output.AppendLine($"Iteration {iteration + 1} | T-{iteration + 1}");
            output.AppendLine(FormatTableau(initialT, rowHeaders, colHeaders));
            output.AppendLine(FormatIteration(B_inv, c, basis, n, m, x_B, A, b, rowHeaders, colHeaders, eps));

            while (true)
            {
                iteration++;

                // Compute pi = c_B * B_inv
                double[] c_B = new double[m];
                for (int i = 0; i < m; i++)
                {
                    int var = basis[i];
                    c_B[i] = var < n ? c[var] : 0;
                }
                double[] pi = new double[m];
                for (int i = 0; i < m; i++)
                {
                    pi[i] = Dot(c_B, GetColumn(B_inv, i));
                }

                // Find entering variable
                int? pivotColumn = null;
                double mostNeg = -eps;
                for (int j = 0; j < n + m; j++)
                {
                    if (basis.Contains(j)) continue;
                    double reduced_cost = 0;
                    for (int i = 0; i < m; i++)
                    {
                        double a_ij = j < n ? A[i, j] : (j - n == i ? 1.0 : 0.0);
                        reduced_cost += pi[i] * a_ij;
                    }
                    reduced_cost -= j < n ? c[j] : 0;
                    if (reduced_cost < mostNeg)
                    {
                        mostNeg = reduced_cost;
                        pivotColumn = j;
                    }
                }

                if (pivotColumn == null)
                {
                    double optimalValue = Dot(c_B, x_B);
                    double[] x = new double[n + m];
                    for (int i = 0; i < m; i++) x[basis[i]] = x_B[i];

                    result.OptimalValue = optimalValue;
                    result.PrimalVariables = x.Take(n).ToArray();
                    result.Message = "Optimal solution found.";

                    double[,] finalT = BuildFinalTableau(A, b, c, m, n, basis, B_inv, pi, eps);
                    result.Tableaus.Add(new TableauTemplate(finalT, colHeaders, rowHeaders, iteration + 1, null, null, "T-*"));
                    output.AppendLine("===");
                    output.AppendLine($"Iteration {iteration + 1} | T-*");
                    output.AppendLine(FormatTableau(finalT, rowHeaders, colHeaders));
                    output.AppendLine(FormatIteration(B_inv, c, basis, n, m, x_B, A, b, rowHeaders, colHeaders, eps));
                    output.AppendLine($"Optimal Value: {optimalValue:F3}");
                    result.Message = output.ToString();
                    break;
                }

                // Compute u = B_inv * a_pivot
                double[] u = new double[m];
                for (int i = 0; i < m; i++)
                {
                    u[i] = Dot(GetColumn(B_inv, i), GetColumn(A, pivotColumn.Value, n, m));
                }

                // Minimum ratio test
                int? pivotRow = null;
                double bestRatio = double.PositiveInfinity;
                for (int i = 0; i < m; i++)
                {
                    if (u[i] > eps)
                    {
                        double ratio = x_B[i] / u[i];
                        if (ratio < bestRatio - 1e-15 || (Math.Abs(ratio - bestRatio) <= 1e-15 && i < pivotRow))
                        {
                            bestRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == null)
                {
                    result.IsUnbounded = true;
                    result.Message = "Unbounded: no leaving variable found.";
                    result.Tableaus.Add(new TableauTemplate(new double[m + 1, n + m + 1], colHeaders, rowHeaders, iteration + 1, null, pivotColumn, "Unbounded"));
                    output.AppendLine("===");
                    output.AppendLine($"Iteration {iteration + 1} | Unbounded");
                    output.AppendLine(FormatIteration(B_inv, c, basis, n, m, x_B, A, b, rowHeaders, colHeaders, eps));
                    result.Message = output.ToString();
                    break;
                }

                // Update x_B
                for (int i = 0; i < m; i++)
                {
                    if (i == pivotRow) continue;
                    x_B[i] -= bestRatio * u[i];
                }
                x_B[pivotRow.Value] = bestRatio;

                // Update basis
                basis[pivotRow.Value] = pivotColumn.Value;

                // Update rowHeaders
                rowHeaders[pivotRow.Value] = colHeaders[pivotColumn.Value];

                // Update B_inv
                double u_pivot = u[pivotRow.Value];
                for (int i = 0; i < m; i++)
                {
                    if (i == pivotRow) continue;
                    double factor = u[i] / u_pivot;
                    for (int j = 0; j < m; j++)
                    {
                        B_inv[i, j] -= factor * B_inv[pivotRow.Value, j];
                    }
                }
                for (int j = 0; j < m; j++)
                {
                    B_inv[pivotRow.Value, j] /= u_pivot;
                }

                // Reconstruct tableau for display
                double[,] T = BuildFinalTableau(A, b, c, m, n, basis, B_inv, pi, eps);
                result.Tableaus.Add(new TableauTemplate(T, colHeaders, rowHeaders, iteration + 1, pivotRow, pivotColumn, $"T-{iteration + 1}"));
                output.AppendLine("===");
                output.AppendLine($"Iteration {iteration + 1} | Pivot @ (row={rowHeaders[pivotRow.Value]}, col={colHeaders[pivotColumn.Value]}) | T-{iteration + 1}");
                output.AppendLine(FormatTableau(T, rowHeaders, colHeaders));
                output.AppendLine(FormatIteration(B_inv, c, basis, n, m, x_B, A, b, rowHeaders, colHeaders, eps ));
            }

            result.Message = output.ToString();
            return result;
        }

        private static double[,] BuildInitialTableau(double[,] A, double[] b, double[] c, int m, int n, double eps)
        {
            int rows = m + 1;
            int cols = n + m + 1;
            double[,] T = new double[rows, cols];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) T[i, j] = A[i, j];
                for (int j = 0; j < m; j++) T[i, n + j] = (i == j) ? 1.0 : 0.0;
                T[i, cols - 1] = b[i];
            }
            for (int j = 0; j < n; j++) T[rows - 1, j] = -c[j];

            // Clean small values
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;

            return T;
        }

        private static double[,] BuildFinalTableau(double[,] A, double[] b, double[] c, int m, int n, int[] basis, double[,] B_inv, double[] pi, double eps)
        {
            int cols = n + m + 1;
            double[,] T = new double[m + 1, cols];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n + m; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < m; k++)
                    {
                        double a_kj = j < n ? A[k, j] : (j - n == k ? 1.0 : 0.0);
                        sum += B_inv[i, k] * a_kj;
                    }
                    T[i, j] = sum;
                }
                T[i, cols - 1] = b[i];
            }
            for (int j = 0; j < n + m; j++)
            {
                double sum = 0;
                for (int k = 0; k < m; k++)
                {
                    double a_kj = j < n ? A[k, j] : (j - n == k ? 1.0 : 0.0);
                    sum += pi[k] * a_kj;
                }
                T[m, j] = sum - (j < n ? c[j] : 0);
            }
            T[m, cols - 1] = Dot(pi, b);

            // Clean small values
            for (int i = 0; i < m + 1; i++)
                for (int j = 0; j < cols; j++)
                    if (Math.Abs(T[i, j]) < eps) T[i, j] = 0.0;

            return T;
        }

        private static double Dot(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        private static double[] GetColumn(double[,] matrix, int col)
        {
            int rows = matrix.GetLength(0);
            double[] column = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                column[i] = matrix[i, col];
            }
            return column;
        }

        private static double[] GetColumn(double[,] A, int col, int n, int m)
        {
            double[] column = new double[m];
            for (int i = 0; i < m; i++)
            {
                column[i] = col < n ? A[i, col] : (col - n == i ? 1.0 : 0.0);
            }
            return column;
        }

        private static string FormatTableau(double[,] T, string[] rowHeaders, string[] colHeaders)
        {
            StringBuilder sb = new StringBuilder();
            int cols = colHeaders.Length;
            int rows = T.GetLength(0);

            sb.Append("     |");
            for (int j = 0; j < cols - 1; j++)
            {
                sb.Append($" {colHeaders[j],6}");
            }
            sb.AppendLine($" {colHeaders[cols - 1],6}");
            sb.AppendLine(new string('-', 62));

            for (int i = 0; i < rows; i++)
            {
                sb.Append($"{rowHeaders[i],6} |");
                for (int j = 0; j < cols - 1; j++)
                {
                    sb.Append($" {T[i, j],6:F3}");
                }
                sb.AppendLine($" {T[i, cols - 1],6:F3}");
            }
            sb.AppendLine(new string('-', 62));

            return sb.ToString();
        }

        private static string FormatIteration(double[,] B_inv, double[] c, int[] basis, int n, int m, double[] x_B, double[,] A, double[] b, string[] rowHeaders, string[] colHeaders, double eps)
        {
            StringBuilder sb = new StringBuilder();

            // Xbv
            sb.AppendLine("Xbv");
            for (int i = 0; i < m; i++)
            {
                sb.AppendLine(rowHeaders[i]);
            }

            // B
            sb.AppendLine("B");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sb.Append(B_inv[i, j].ToString("0.###") + " ");
                }
                sb.AppendLine();
            }

            // Cbv
            sb.AppendLine("Cbv");
            for (int i = 0; i < m; i++)
            {
                sb.AppendLine((basis[i] < n ? c[basis[i]] : 0).ToString("0.###"));
            }

            // B-1
            sb.AppendLine("B-1");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sb.Append(B_inv[i, j].ToString("0.###") + " ");
                }
                sb.AppendLine();
            }

            // CbvB-1
            sb.AppendLine("CbvB-1");
            for (int i = 0; i < m; i++)
            {
                double sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += (basis[j] < n ? c[basis[j]] : 0) * B_inv[j, i];
                }
                sb.AppendLine(sum.ToString("0.###"));
            }

            // Price Out
            sb.AppendLine("Price Out");
            sb.AppendLine("Xnbv");
            for (int j = 0; j < n; j++)
            {
                if (basis.Contains(j)) continue;
                sb.AppendLine(colHeaders[j]);
            }

            sb.AppendLine("Cnbv");
            for (int j = 0; j < n; j++)
            {
                if (basis.Contains(j)) continue;
                sb.AppendLine(c[j].ToString("0.###"));
            }

            // A1 (using the pivot column, approximated as first non-basic for simplicity)
            sb.AppendLine("A1");
            int pivotCol = basis.Contains(0) ? 1 : 0;
            for (int i = 0; i < m; i++)
            {
                sb.AppendLine(A[i, pivotCol].ToString("0.###"));
            }

            // b
            sb.AppendLine("b");
            for (int i = 0; i < m; i++)
            {
                sb.AppendLine(b[i].ToString("0.###"));
            }

            // θ
            sb.AppendLine("θ");
            for (int i = 0; i < m; i++)
            {
                double a_i_pivot = A[i, pivotCol];
                if (a_i_pivot > eps)
                {
                    sb.AppendLine((b[i] / a_i_pivot).ToString("0.###"));
                }
                else
                {
                    sb.AppendLine("N/A");
                }
            }

            return sb.ToString();
        }
    }
}