namespace LPModelsLibrary.Models
{
    public class TableauTemplate
    {
        public double[,] Tableau { get; }
        public string[] ColHeaders { get; }
        public string[] RowHeaders { get; }
        public int Iteration { get; }
        public int? PivotRow { get; }
        public int? PivotCol { get; }
        public string Note { get; }

        public TableauTemplate(double[,] tableau, string[] colHeaders, string[] rowHeaders,
                               int iteration, int? pivotRow, int? pivotCol, string note = "")
        {
            // Clone everything so changes in future iterations don’t affect saved snapshots
            Tableau = (double[,])tableau.Clone();
            ColHeaders = (string[])colHeaders.Clone();
            RowHeaders = (string[])rowHeaders.Clone();
            Iteration = iteration;
            PivotRow = pivotRow;
            PivotCol = pivotCol;
            Note = note;
        }

        /// <summary>
        /// Pretty prints the tableau with row/col headers.
        /// Shows pivot position (if any).
        /// </summary>
        public override string ToString()
        {
            int m = Tableau.GetLength(0);
            int n = Tableau.GetLength(1);


            // Calculate column widths for nice alignment
            var widths = new int[n + 1];
            widths[0] = Math.Max(5, RowHeaders.Max(h => h.Length));
            for (int j = 0; j < n; j++)
            {
                int maxW = ColHeaders[j].Length;
                for (int i = 0; i < m; i++)
                    maxW = Math.Max(maxW, Tableau[i, j].ToString("0.###").Length);
                widths[j + 1] = Math.Max(6, maxW);
            }

            string line = new string('-', widths.Sum() + n + 1);

            // Print iteration header
            var s = $"Iteration {Iteration}" +
                    (PivotRow.HasValue && PivotCol.HasValue
                        ? $" | Pivot @ (row={RowHeaders[PivotRow.Value]}, col={ColHeaders[PivotCol.Value]})"
                        : "") +
                    (string.IsNullOrWhiteSpace(Note) ? "" : $" | {Note}") + Environment.NewLine;

            // Print column headers
            s += "".PadRight(widths[0]) + " | ";
            for (int j = 0; j < n; j++)
                s += ColHeaders[j].PadLeft(widths[j + 1]) + " ";
            s += Environment.NewLine + line + Environment.NewLine;

            // Print each row with header
            for (int i = 0; i < m; i++)
            {
                s += RowHeaders[i].PadRight(widths[0]) + " | ";
                for (int j = 0; j < n; j++)
                    s += Tableau[i, j].ToString("0.###").PadLeft(widths[j + 1]) + " ";
                s += Environment.NewLine;
            }
            s += line + Environment.NewLine;
            return s;
        }
    }
}
