//-----------------------------------------------------------------------
// <copyright file="ReplicationDestination.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sparrow;
using Sparrow.Json.Parsing;

namespace Raven.Client.Documents.Replication
{
    /// <summary>
    /// Data class for replication destination
    /// </summary>
    public abstract class ReplicationNode : IEquatable<ReplicationNode>
    {
        /// <summary>
        /// The name of the connection string specified in the 
        /// server configuration file. 
        /// Override all other properties of the destination
        /// </summary>

        private string _url;

        /// <summary>
        /// Gets or sets the URL of the replication destination
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get => _url;
            set => _url = value?.TrimEnd('/');
        }

        /// <summary>
        /// The database to use
        /// </summary>
        public string Database;

        public bool Disabled;

        public bool Equals(ReplicationNode other) => IsEqualTo(other);

        public virtual bool IsEqualTo(ReplicationNode other)
        {
            return string.Equals(Database, other.Database, StringComparison.OrdinalIgnoreCase) && 
                   Disabled == other.Disabled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ReplicationNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)CalculateStringHash(Database);
                hashCode = (hashCode * 397) ^ Disabled.GetHashCode();
                return hashCode;
            }
        }

        protected static ulong CalculateStringHash(string s)
        {
            return string.IsNullOrEmpty(s) ? 0 : Hashing.XXHash64.Calculate(s, Encodings.Utf8);
        }

        public virtual DynamicJsonValue ToJson()
        {
            var json = new DynamicJsonValue
            {
                [nameof(Database)] = Database,
                [nameof(Url)] = Url,
                [nameof(Disabled)] = Disabled
            };

            return json;
        }

        public override string ToString()
        {
            var str = $"{Url} - {Database}";
            if (Disabled)
                str += " - DISABLED";
            return str;
        }

        public abstract string FromString();
    }

   
}
