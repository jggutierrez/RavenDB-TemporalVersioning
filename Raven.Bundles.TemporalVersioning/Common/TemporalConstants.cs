﻿#if CLIENT
namespace Raven.Client.Bundles.TemporalVersioning.Common
#else
namespace Raven.Bundles.TemporalVersioning.Common
#endif
{
    public static class TemporalConstants
    {
        public const string BundleName = "TemporalVersioning";
        public const string TemporalKeySeparator = "/temporalrevisions/";
        public const string TemporalRevisionsIndex = "Raven/TemporalRevisions";

        public const string TemporalEffectiveDate = "Temporal-Effective-Date";
        public const string RavenDocumentTemporalRevision = "Raven-Document-Temporal-Revision";
        public const string RavenDocumentTemporalStatus = "Raven-Document-Temporal-Status";
        public const string RavenDocumentTemporalEffectiveStart = "Raven-Document-Temporal-Effective-Start";
        public const string RavenDocumentTemporalEffectiveUntil = "Raven-Document-Temporal-Effective-Until";
        public const string RavenDocumentTemporalDeleted = "Raven-Document-Temporal-Deleted";
        public const string RavenDocumentTemporalPending = "Raven-Document-Temporal-Pending";
    }
}