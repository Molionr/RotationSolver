﻿using ImGuiScene;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal interface ICustomCombo
    {
        Role Role { get; }
        uint[] JobIDs { get; }

        bool IsEnabled { get; set; }

        string JobName { get; }

        ActionConfiguration Config { get; }

        SortedList<DescType, string> Description { get; }
        Dictionary<string, string> CommandShow { get; }
        TextureWrap Texture{ get; }

        string OnCommand(string args);
        bool TryInvoke(out IAction newAction);
    }
}