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
        private static readonly string _ = new('=', 15);

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
        ///     Splits the loaded dataset into train and test datasets.
        ///     Returns the split train and test datasets.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        private static TrainTestData LoadData(MLContext mlContext)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentIssue>(_dataPath, hasHeader: false);
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
                .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentIssue.SentimentText))
                .Append(
                    mlContext.BinaryClassification.Trainers
                    .SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")
                );
            Console.WriteLine($"{_} Training The Model {_}");
            var model = estimator.Fit(splitTrainSet);
            Console.WriteLine($"{_} End Of Model Training {_}");
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
            Console.WriteLine();
            Console.WriteLine($"{_} Evaluating Model Accuracy With Test Data {_}");
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine("Model Quality Metrics Evaluation");
            Console.WriteLine(_);
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score:  {metrics.F1Score:P2}");
            Console.WriteLine($"{_} End Of Model Evaluating {_}");
            Console.WriteLine();
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
            PredictionEngine<SentimentIssue, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(model);
            SentimentIssue sentiment = new() { SentimentText = "This was a very bad steak" };
            var resultPrediction = predictionFunction.Predict(sentiment);
            Console.WriteLine();
            Console.WriteLine($"{_} Predicting Test Of Model With Single Sample And Test Dataset {_}");
            Console.WriteLine($"Sentiment:   {resultPrediction.SentimentText}");
            Console.WriteLine($"Prediction:  {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")}");
            Console.WriteLine($"Probability: {resultPrediction.Probability}");
            Console.WriteLine($"{_} End Of Predicting {_}");
            Console.WriteLine();
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
            IEnumerable<SentimentIssue> sentiments =
            [
                new SentimentIssue{ SentimentText = "This was a horrible meal" },
                new SentimentIssue{ SentimentText = "I love this spaghetti." }
            ];
            IDataView SentimentIssue = mlContext.Data.LoadFromEnumerable(sentiments);
            IDataView predictions = model.Transform(SentimentIssue);
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            Console.WriteLine();
            Console.WriteLine($"{_} Predicting Test Of Loaded Model With Multiple Samples {_}");
            Console.WriteLine();
            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment:   {prediction.SentimentText}");
                Console.WriteLine($"Prediction:  {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")}");
                Console.WriteLine($"Probability: {prediction.Probability}");
                Console.WriteLine();
            }
            Console.WriteLine($"{_} End Of Predicting {_}");
        }
    }
}
