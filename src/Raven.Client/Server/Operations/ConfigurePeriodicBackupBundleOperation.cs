﻿using System.Net.Http;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Session;
using Raven.Client.Http;
using Raven.Client.Json;
using Raven.Client.Json.Converters;
using Raven.Client.Server.PeriodicBackup;
using Sparrow.Json;

namespace Raven.Client.Server.Operations
{
    public class UpdatePeriodicBackupOperation : IServerOperation<UpdatePeriodicBackupOperationResult>
    {
        private readonly PeriodicBackupConfiguration _configuration;
        private readonly string _databaseName;

        public UpdatePeriodicBackupOperation(PeriodicBackupConfiguration configuration, string databaseName)
        {
            _configuration = configuration;
            _databaseName = databaseName;
        }

        public RavenCommand<UpdatePeriodicBackupOperationResult> GetCommand(DocumentConventions conventions, JsonOperationContext context)
        {
            return new UpdatePeriodicBackupCommand(_configuration, _databaseName, context);
        }
    }

    public class UpdatePeriodicBackupCommand : RavenCommand<UpdatePeriodicBackupOperationResult>
    {
        private readonly PeriodicBackupConfiguration _configuration;
        private readonly string _databaseName;
        private readonly JsonOperationContext _context;

        public UpdatePeriodicBackupCommand(PeriodicBackupConfiguration configuration, string databaseName, JsonOperationContext context)
        {
            _configuration = configuration;
            _databaseName = databaseName;
            _context = context;
        }

        public override bool IsReadRequest => false;

        public override HttpRequestMessage CreateRequest(ServerNode node, out string url)
        {
            url = $"{node.Url}/admin/periodic-backup/update?name={_databaseName}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new BlittableJsonContent(stream =>
                {
                    var config = EntityToBlittable.ConvertEntityToBlittable(_configuration, DocumentConventions.Default, _context);
                    _context.Write(stream, config);
                })
            };

            return request;
        }

        public override void SetResponse(BlittableJsonReaderObject response, bool fromCache)
        {
            if (response == null)
                ThrowInvalidResponse();

            Result = JsonDeserializationClient.ConfigurePeriodicBackupBundleOperationResult(response);
        }
    }

    public class UpdatePeriodicBackupOperationResult
    {
        public long ETag { get; set; }

        public long TaskId { get; set; }
    }
}

