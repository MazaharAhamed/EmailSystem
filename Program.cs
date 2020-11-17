using Amazon;
using System;
using System.Collections.Generic;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Http;

namespace AmazonSESSample
{
    class Program
    {
        static readonly string senderAddress = "mazaharahamed77@gmail.com";


        static readonly string receiverAddress = "testemailinn1@gmail.com";

        static readonly string awsAccessKey = "AKIAJE6AL6JVFEYZED5A";
        static readonly string awsSecretKey = "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";


        static readonly string textBody = "Amazon SES Test (.NET)\r\n"
                                        + "This email was sent through Amazon SES"
                                        + "using the AWS SDK for .NET.";


        static readonly string htmlBody = @"<html>
<head></head>
<body>
  <h1>Amazon SES Test (AWS SDK for .NET)</h1>
  <p>This email was sent with
    <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
    <a href='https://aws.amazon.com/sdk-for-net/'>
      AWS SDK for .NET</a>.</p>
</body>
</html>";

        static async Task Main(string[] args)
        {
            using (var client = new AmazonSimpleEmailServiceClient(awsAccessKey, awsSecretKey, RegionEndpoint.USWest2))
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                int counter = 0;

                int sent = 10;
                Parallel.For(0, sent, new ParallelOptions { MaxDegreeOfParallelism = 4 }, count =>
                {
                    int progress = Interlocked.Increment(ref counter);
                    var subject = "my subject " + counter.ToString();
                    SendEmailRequest sentEmailRequest = SendEmail(subject).Result;
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = client.SendEmailAsync(sentEmailRequest);
                    Console.WriteLine("The email was sent successfully.");
                });
                watch.Stop();
                TimeSpan ts = watch.Elapsed;

                Console.WriteLine($"It took {ts.Seconds} seconds");

                Console.ReadKey();
            }

        }
        private static async Task<SendEmailRequest> SendEmail(string subject)
        {

            var sendRequest = new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses =
                   new List<string> { receiverAddress }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = await GetWeatherAsyc()
                        }
                    }
                },

            };
            return sendRequest;
        }

        private static async Task<string> GetWeatherAsyc()
        {
            string weatherInfo;

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://weatherbit-v1-mashape.p.rapidapi.com/current?lon=78.491684&lat=17.38714"),
                Headers =
            {
                { "x-rapidapi-key", "5f8992a83fmsh231b95ee1aefef9p15ec40jsnc330fcab026b" },
                { "x-rapidapi-host", "weatherbit-v1-mashape.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                weatherInfo = await response.Content.ReadAsStringAsync();
            }
            return weatherInfo;
        }
    }
}


