using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPModelsLibrary.Models
{
    public class SimplexResult
    {
        public double OptimalValue { get; set; }
        public double[] PrimalVariables { get; set; } = Array.Empty<double>();
        public bool IsUnbounded { get; set; }
        public bool IsInfeasible { get; set; }
        public List<TableauTemplate> Tableaus { get; set; } = new();
        public string Message { get; set; } = "";
    }
}
