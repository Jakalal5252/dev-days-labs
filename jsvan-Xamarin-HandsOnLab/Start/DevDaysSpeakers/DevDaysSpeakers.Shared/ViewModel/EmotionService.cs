using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Emotion;
using System.Linq;

namespace DevDaysSpeakers.ViewModel
{
    public class EmotionService
    {
        private static async Task<Emotion[]> GetHappinessAsync(string url)
        {
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(url);
            var emotionClient = new EmotionServiceClient("c296dbefe58a4ffba6bce379bed0abc6");

            var emotionResults = await emotionClient.RecognizeAsync(stream);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScoreAsync(string url)
        {
            Emotion[] emotionResults = await GetHappinessAsync(url);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Count();
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}