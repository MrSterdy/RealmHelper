﻿namespace RealmHelper.Application.Models.Minecraft.Requests;

public class InviteLinkRequest
{
    public string Type { get; set; } = default!;
    
    public long WorldId { get; set; }
}