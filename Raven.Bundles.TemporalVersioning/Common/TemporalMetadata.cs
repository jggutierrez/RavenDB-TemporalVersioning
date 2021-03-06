﻿using System;
using Raven.Json.Linq;

#if CLIENT
namespace Raven.Client.Bundles.TemporalVersioning.Common
#else
namespace Raven.Bundles.TemporalVersioning.Common
#endif
{
    public class TemporalMetadata
    {
        public const string RavenTemporalEffective = "Raven-Temporal-Effective";
        public const string RavenDocumentTemporalRevision = "Raven-Document-Temporal-Revision";
        public const string RavenDocumentTemporalStatus = "Raven-Document-Temporal-Status";
        public const string RavenDocumentTemporalAssertedStart = "Raven-Document-Temporal-Asserted-Start";
        public const string RavenDocumentTemporalAssertedUntil = "Raven-Document-Temporal-Asserted-Until";
        public const string RavenDocumentTemporalEffectiveStart = "Raven-Document-Temporal-Effective-Start";
        public const string RavenDocumentTemporalEffectiveUntil = "Raven-Document-Temporal-Effective-Until";
        public const string RavenDocumentTemporalDeleted = "Raven-Document-Temporal-Deleted";
        public const string RavenDocumentTemporalPending = "Raven-Document-Temporal-Pending";

        private readonly RavenJObject _metadata;

        public TemporalMetadata(RavenJObject metadata)
        {
            _metadata = metadata;
        }

        public int RevisionNumber
        {
            get
            {
                var revision = _metadata.Value<int?>(RavenDocumentTemporalRevision);
                return revision.HasValue ? revision.Value : 0;
            }
            set
            {
                const string key = RavenDocumentTemporalRevision;

                if (value > 0)
                    _metadata[key] = value;
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public TemporalStatus Status
        {
            get
            {
                TemporalStatus status;
                return Enum.TryParse(_metadata.Value<string>(RavenDocumentTemporalStatus), out status)
                           ? status
                           : TemporalStatus.NonTemporal;
            }
            set
            {
                const string key = RavenDocumentTemporalStatus;

                if (value != TemporalStatus.NonTemporal)
                    _metadata[key] = value.ToString();
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public bool Deleted
        {
            get { return _metadata.Value<bool?>(RavenDocumentTemporalDeleted) ?? false; }
            set { _metadata[RavenDocumentTemporalDeleted] = value; }
        }

        public bool Pending
        {
            get { return _metadata.Value<bool?>(RavenDocumentTemporalPending) ?? false; }
            set { _metadata[RavenDocumentTemporalPending] = value; }
        }

        public DateTimeOffset? Effective
        {
            get
            {
                var dto = _metadata.Value<DateTimeOffset?>(RavenTemporalEffective);
                if (dto == null)
                {
                    Console.WriteLine();
                }
                return dto;
            }
            set
            {
                const string key = RavenTemporalEffective;

                if (value.HasValue)
                    _metadata[key] = RavenJToken.FromObject(value.Value);
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public DateTimeOffset? EffectiveStart
        {
            get { return _metadata.Value<DateTimeOffset?>(RavenDocumentTemporalEffectiveStart); }
            set
            {
                const string key = RavenDocumentTemporalEffectiveStart;

                if (value.HasValue)
                    _metadata[key] = value.Value;
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public DateTimeOffset? EffectiveUntil
        {
            get { return _metadata.Value<DateTimeOffset?>(RavenDocumentTemporalEffectiveUntil); }
            set
            {
                const string key = RavenDocumentTemporalEffectiveUntil;

                if (value.HasValue)
                    _metadata[key] = value.Value;
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public DateTimeOffset? AssertedStart
        {
            get { return _metadata.Value<DateTimeOffset?>(RavenDocumentTemporalAssertedStart); }
            set
            {
                const string key = RavenDocumentTemporalAssertedStart;

                if (value.HasValue)
                    _metadata[key] = value.Value;
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }

        public DateTimeOffset? AssertedUntil
        {
            get { return _metadata.Value<DateTimeOffset?>(RavenDocumentTemporalAssertedUntil); }
            set
            {
                const string key = RavenDocumentTemporalAssertedUntil;

                if (value.HasValue)
                    _metadata[key] = value.Value;
                else if (_metadata.ContainsKey(key))
                    _metadata.Remove(key);
            }
        }
    }
}
