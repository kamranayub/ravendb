using System.Net;

namespace Raven.NewClient.Client.Document
{
    public class OpenSessionOptions
    {
        public string Database { get; set; }
        public ICredentials Credentials { get; set; }
        public bool ForceReadFromMaster { get; set; }
    }
}
