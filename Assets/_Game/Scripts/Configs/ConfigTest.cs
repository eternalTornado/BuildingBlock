using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStringers.Framework;

public class ConfigTestRecord
{
    public int id;
    public string desc;
}

public class ConfigTest : CSConfigData<ConfigTestRecord>
{
    public ConfigTest(): base() { }

    public override void RebuildIndex()
    {
        RebuildIndexByField<int>("id");
    }

    public ConfigTestRecord GetConfigById(int id)
    {
        return GetRecordByIndex("id", id);
    }
}
