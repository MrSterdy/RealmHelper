﻿namespace RealmHelper.Realm.Bedrock.Application.Models.Requests;

public class PermissionChangeRequest
{
    public string Permission { get; set; } = default!;
}

public class PlayerPermissionChangeRequest : PermissionChangeRequest
{
    public string Xuid { get; set; } = default!;
}