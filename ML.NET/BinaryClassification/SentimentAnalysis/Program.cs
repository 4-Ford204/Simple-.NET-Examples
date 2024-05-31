using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using static Microsoft.ML.DataOperationsCatalog;

namespace ML.NET.BinaryClassification.SentimentAnalysis
{
    public class Program
    {
        private static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "../../../yelp_labelled.txt");

        public static void Main(string[] args)
        {
            MLContext mlContext = new();
            TrainTestData splitDataView = LoadData(mlContext);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);
            UseModelWithSingleItem(mlContext, model);
            UseModelWithBatchItems(mlContext, model);
        }

        /// <summary>
        ///     Loads the data.
        ///     Split the loaded dataset into train and test datasets.
        ///     Returns the split train and test datasets.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        private static TrainTestData LoadData(MLContext mlContext)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;
        }

        /// <summary>
        ///     Extracts and transforms the data.
        ///     Trains the model.
        ///     Predicts sentiment based on test data.
        ///     Returns the model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="splitTrainSet"></param>
        /// <returns></returns>
        private static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                .Append(
                    mlContext.BinaryClassification.Trainers
                    .SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")
                );
            Console.WriteLine("=============== Create and train the model ===============");
            var model = estimator.Fit(splitTrainSet);
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();
            return model;
        }

        /// <summary>
        ///     Loads the test data.
        ///     Creates the BinaryClassification evaluator.
        ///     Evaluates the model and creates metrics.
        ///     Displays the metrics.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        /// <param name="splitTestSet"></param>
        private static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            Console.WriteLine("=============== Evaluating model accuracy with test data ===============");
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score:  {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
        }

        /// <summary>
        ///     Creates a single comment of test data.
        ///     Predicts sentiment based on test data.
        ///     Combines test data and predictions for reporting.
        ///     Displays the predicted results.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            SentimentData sentiment = new() { SentimentText = "This was a very bad steak" };
            var resultPrediction = predictionFunction.Predict(sentiment);
            Console.WriteLine();
            Console.WriteLine("=============== Prediction test of model with a single sample and test dataset ===============");
            Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");
            Console.WriteLine("=============== End of Predictions ===============");
        }

        /// <summary>
        ///     Creates batch test data 
        ///     Predicts sentiment based on test data.
        ///     Combines test data and predictions for reporting.
        ///     Displays the predicted results.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        private static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            IEnumerable<SentimentData> sentiments =
            [
                new SentimentData{ SentimentText = "This was a horrible meal" },
                new SentimentData{ SentimentText = "I love this spaghetti." }
            ];
            IDataView sentimentData = mlContext.Data.LoadFromEnumerable(sentiments);
            IDataView predictions = model.Transform(sentimentData);
            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            Console.WriteLine();
            Console.WriteLine("=============== Prediction test of loaded model with multiple samples ===============");
            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            }
            Console.WriteLine("=============== End of predictions ===============");
        }
    }
}
