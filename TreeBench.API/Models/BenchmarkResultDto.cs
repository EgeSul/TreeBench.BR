namespace TreeBench.API.Models
{
    public class BenchmarkResultDto
    {
        public string TreeName { get; set; } = string.Empty;
        public int Mode { get; set; }
        public int NodeCount { get; set; }
        public double InsertTimeMs { get; set; }
        public double SearchTimeMs { get; set; }
        public double DeleteTimeMs { get; set; }
        public int MaxDepth { get; set; }
        public int MinDepth { get; set; }
        public int TotalRotations { get; set; }
        public double RamCostKb { get; set; }
    }
}