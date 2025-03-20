using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GitHubActivity_CLI.Classes;

namespace GitHubActivity_CLI
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubActivityCLI");
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Error: Please provide a GitHub username (e.g., 'dotnet run -- octocat')");
                return;
            }
            string username = args[0];
            Console.WriteLine($"Recent Activities for {username}:");
            try
            {
                List<GitHubEvent> events = await FetchUserEventsAsync(username);
                if (events == null || events.Count == 0)
                {
                    Console.WriteLine("No recent public activity found for this user.");
                    return;
                }
                foreach (var e in events.Take(10))
                {
                    string repoName = e.Repo.Name;
                    string date = DateTime.Parse(e.CreatedAt).ToString("yyyy-MM-dd");

                    if (e.Type == "PushEvent")
                    {
                        int commitCount = e.Payload.Commits?.Count ?? 0;
                        Console.WriteLine($"Pushed {commitCount} commits to {repoName} on {date}");
                    }
                    else if (e.Type == "IssuesEvent" && e.Payload.Action == "opened")
                    {
                        Console.WriteLine($"Opened a new issue in {repoName} on {date}");
                    }
                    else if (e.Type == "WatchEvent" && e.Payload.Action == "started")
                    {
                        Console.WriteLine($"Starred {repoName} on {date}");
                    }
                    else
                    {
                        Console.WriteLine($"{e.Type} in {repoName} on {date}"); // Fallback for other event types
                    }
                }

            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error: Network issue or API unavailable.");
            }
            catch (JsonException)
            {
                Console.WriteLine("Error: Failed to parse API response.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected issue occurred ({ex.Message}).");
            }

        }
        static async Task<List<GitHubEvent>> FetchUserEventsAsync(string username)
        {
            string url = $"https://api.github.com/users/{username}/events";
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status {response.StatusCode}");
            }
            string json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json) || json == "[]")
            {
                return new List<GitHubEvent>();
            }
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var events = JsonSerializer.Deserialize<List<GitHubEvent>>(json, options);
                Console.WriteLine($"Deserialized {events.Count} events");
                return events;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<GitHubEvent>();
            }
        }
    }
}
