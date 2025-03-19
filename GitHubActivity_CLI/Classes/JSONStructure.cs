using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GitHubActivity_CLI.Classes
{
    using System.Text.Json.Serialization;

    public class GitHubEvent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("repo")]
        public RepoInfo Repo { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("payload")]
        public Payload Payload { get; set; }
    }

    public class RepoInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Payload
    {
        [JsonPropertyName("commits")]
        public List<Commit>? Commits { get; set; } // Nullable since not all events have commits

        [JsonPropertyName("action")]
        public string? Action { get; set; } // Nullable since not all events have action
    }

    public class Commit
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
