namespace LPModelsLibrary.Models
{
    public class TableauTemplate
    {
        public double[,] Tableau { get; set; }
        public string[] ColHeaders { get; set; }
        public string[] RowHeaders { get; set; }
        public int Iteration { get; set; }
        public int? PivotRow { get; set; }
        public int? PivotCol { get; set; }
        public string Note { get; set; }

        public TableauTemplate(double[,] tableau, string[] colHeaders, string[] rowHeaders,
                               int iteration, int? pivotRow, int? pivotCol, string note = "")
        {
            // Clone to avoid mutation across iterations
            Tableau = (double[,])tableau.Clone();
            ColHeaders = (string[])colHeaders.Clone();
            RowHeaders = (string[])rowHeaders.Clone();
            Iteration = iteration;
            PivotRow = pivotRow;
            PivotCol = pivotCol;
            Note = note;
        }

      
        public override string ToString()
        {
            int m = Tableau.GetLength(0); // rows
            int n = Tableau.GetLength(1); // cols

            
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

            
            var s = $"Iteration {Iteration}" +
                    (PivotRow.HasValue && PivotCol.HasValue
                        ? $" | Pivot @ (row={RowHeaders[PivotRow.Value]}, col={ColHeaders[PivotCol.Value]})"
                        : "") +
                    (string.IsNullOrWhiteSpace(Note) ? "" : $" | {Note}") +
                    Environment.NewLine;

            // Column headers
            s += "".PadRight(widths[0]) + " | ";
            for (int j = 0; j < n; j++)
                s += ColHeaders[j].PadLeft(widths[j + 1]) + " ";
            s += Environment.NewLine + line + Environment.NewLine;

            // Print each row
            for (int i = 0; i < m; i++)
            {
                string rowHeader = RowHeaders[i];
                s += rowHeader.PadRight(widths[0]) + " | ";

                for (int j = 0; j < n; j++)
                {
                    string val = Tableau[i, j].ToString("0.###"); 
                    s += val.PadLeft(widths[j + 1]) + " ";
                }

                s += Environment.NewLine;
            }

            s += line + Environment.NewLine;
            return s;
        }
    }
}
