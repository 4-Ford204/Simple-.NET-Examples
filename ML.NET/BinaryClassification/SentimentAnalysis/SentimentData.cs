using Microsoft.ML.Data;

namespace ML.NET.BinaryClassification.SentimentAnalysis
{
    public class SentimentIssue
    {
        [LoadColumn(0)]
        public string? SentimentText { get; set; }
        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction : SentimentIssue
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
