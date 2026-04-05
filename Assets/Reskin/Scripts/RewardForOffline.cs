using System;
using Game.Domain;
using Game.UI.Hud;
using UnityEngine;

public class OfflineReward : BaseHudWithModel<GameModel>
{
    [SerializeField] private RealTime _realTime;

    protected override void OnEnable()
    {
        _realTime.AbsenceTimeCalculated += OnAbsenceTimeCalculated;
    }
    
    protected override void OnDisable()
    {
        _realTime.AbsenceTimeCalculated -= OnAbsenceTimeCalculated;
    }

    protected override void OnModelChanged(GameModel model)
    {
        print($"model.TotalEarnedCash: {model.TotalEarnedCash}");
    }
 
    private void OnAbsenceTimeCalculated(TimeSpan absenceTime)
    {
        Debug.Log($"Игрок отсутствовал ZXC: {absenceTime}");
    }
}
