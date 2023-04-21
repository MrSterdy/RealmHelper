using RealmHelper.Realm.Common.Abstractions.Types;

namespace RealmHelper.Realm.Common.Abstractions.Models;

public class Backup
{
    public string Id { get; set; } = default!;
    
    public long Size { get; set; }

    public BackupMetadata Metadata { get; set; } = default!;
}

public class BackupMetadata
{
    public string Name { get; set; } = default!;

    public string Version { get; set; } = default!;
    
    public Difficulty Difficulty { get; set; }
    
    public GameMode GameMode { get; set; }
}

public class BackupFile
{
    public string Name { get; set; } = default!;
    public string FileName { get; set; } = default!;

    public Stream Content { get; set; } = default!;
    public string ContentType { get; set; } = default!;
}