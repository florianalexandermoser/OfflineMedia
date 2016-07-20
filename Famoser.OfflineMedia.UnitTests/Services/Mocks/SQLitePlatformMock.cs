﻿using SQLite.Net.Interop;

namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    public class SQLitePlatformMock : ISQLitePlatform
    {
        public ISQLiteApi SQLiteApi { get; }
        public IStopwatchFactory StopwatchFactory { get; }
        public IReflectionService ReflectionService { get; }
        public IVolatileService VolatileService { get; }
    }
}
