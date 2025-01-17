﻿using ECommons.DalamudServices;
using ECommons.GameHelpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.Commands;

public static partial class RSCommands
{
    private static string _stateString = "Off", _specialString = string.Empty;

    internal static string EntryString => _stateString + (DataCenter.SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {DataCenter.SpecialTimeLeft:F2}s");

    private static void UpdateToast()
    {
        if (!Service.Config.GetValue(Basic.Configuration.PluginConfigBool.ShowInfoOnToast)) return;

        Svc.Toasts.ShowQuest(" " + EntryString, new Dalamud.Game.Gui.Toast.QuestToastOptions()
        {
            IconId = 101,
        });
    }

    private static unsafe void DoStateCommandType(StateCommandType stateType, int index = -1) => DoOneCommandType(EnumTranslations.ToSayout, role =>
    {
        if (DataCenter.State)
        {
            if (DataCenter.IsManual && stateType == StateCommandType.Manual
                && Service.Config.GetValue(Basic.Configuration.PluginConfigBool.ToggleManual))
            {
                stateType = StateCommandType.Cancel;
            }
            else if (stateType == StateCommandType.Auto)
            {
                if (Service.Config.GetValue(Basic.Configuration.PluginConfigBool.ToggleAuto))
                {
                    stateType = StateCommandType.Cancel;
                }
                else
                {
                    if (index == -1)
                    {
                        index = Service.Config.GetValue(Basic.Configuration.PluginConfigInt.TargetingIndex) + 1;
                    }
                    index %= Service.Config.GlobalConfig.TargetingTypes.Count;
                    Service.Config.SetValue(Basic.Configuration.PluginConfigInt.TargetingIndex, index);
                }
            }
        }

        switch (stateType)
        {
            case StateCommandType.Cancel:
                DataCenter.State = false;
                break;

            case StateCommandType.Auto:
                DataCenter.IsManual = false;
                DataCenter.State = true;
                ActionUpdater.AutoCancelTime = DateTime.MinValue;
                break;

            case StateCommandType.Manual:
                DataCenter.IsManual = true;
                DataCenter.State = true;
                ActionUpdater.AutoCancelTime = DateTime.MinValue;
                break;
        }

        _stateString = stateType.ToStateString(role);
        UpdateToast();
        return stateType;
    });

    private static void DoSpecialCommandType(SpecialCommandType specialType, bool sayout = true) => DoOneCommandType(sayout ? EnumTranslations.ToSayout : (s, r) => string.Empty, role =>
    {
        _specialString = specialType.ToSpecialString(role);
        DataCenter.SetSpecialType(specialType);
        if (sayout) UpdateToast();
        return specialType;
    });

    private static void DoOneCommandType<T>(Func<T, JobRole, string> sayout, Func<JobRole, T> doingSomething)
        where T : struct, Enum
    {
        //Get job role.
        var role = Player.Object.ClassJob.GameData.GetJobRole();

        T type = doingSomething(role);

        //Saying out.
        if (Service.Config.GetValue(Basic.Configuration.PluginConfigBool.SayOutStateChanged)) SpeechHelper.Speak(sayout(type, role));
    }
}
