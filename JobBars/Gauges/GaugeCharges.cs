﻿using Dalamud.Plugin;
using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using static JobBars.UI.UIColor;

namespace JobBars.Gauges {
    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public GaugeVisualType Type;
        public ElementColor BarColor;
        public bool SameColor;
        public bool NoSoundOnFull;
    }

    public struct GaugesChargesPartProps {
        public Item[] Triggers;
        public float Duration;
        public float CD;
        public bool Bar;
        public bool Diamond;
        public int MaxCharges;
        public ElementColor Color;
    }

    public class GaugeCharges : Gauge {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.BarDiamondCombo, GaugeVisualType.Bar, GaugeVisualType.Diamond };

        private GaugeChargesProps Props;
        private readonly int TotalDiamonds = 0;
        private bool GaugeFull = true;

        public GaugeCharges(string name, GaugeChargesProps props) : base(name) {
            Props = props;
            Props.Type = GetConfigValue(Config.Type, Props.Type).Value;
            Props.NoSoundOnFull = GetConfigValue(Config.NoSoundOnFull, Props.NoSoundOnFull).Value;
            Props.BarColor = Config.GetColor(Props.BarColor);

            RefreshSameColor();
            foreach (var part in Props.Parts) {
                if (part.Diamond) {
                    TotalDiamonds += part.MaxCharges;
                }
            }
        }

        protected override void SetupUI() {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetGaugeColor(Props.BarColor);
                SetupDiamondColors();
                combo.SetTextColor(NoColor);
                combo.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIDiamond diamond) {
                SetupDiamondColors();
                diamond.SetMaxValue(TotalDiamonds);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetColor(Props.BarColor);
                gauge.SetTextColor(NoColor);
            }

            GaugeFull = true;
            SetGaugeValue(0, 0);
            SetDiamondValue(0, 0, TotalDiamonds);
        }

        private void SetupDiamondColors() {
            int diamondIdx = 0;
            foreach (var part in Props.Parts) {
                if (part.Diamond) {
                    if (UI is UIGaugeDiamondCombo combo) {
                        combo.SetDiamondColor(part.Color, diamondIdx, part.MaxCharges);
                    }
                    else if (UI is UIDiamond diamond) {
                        diamond.SetColor(part.Color, diamondIdx, part.MaxCharges);
                    }
                    diamondIdx += part.MaxCharges;
                }
            }
        }

        private void RefreshSameColor() {
            if (!Props.SameColor) return;
            for (int i = 0; i < Props.Parts.Length; i++) {
                Props.Parts[i].Color = Props.BarColor;
            }
        }

        public unsafe override void Tick(DateTime time, Dictionary<Item, BuffElem> buffDict) {
            bool barAssigned = false;
            int diamondIdx = 0;
            foreach (var part in Props.Parts) {
                foreach (var trigger in part.Triggers) {
                    if (trigger.Type == ItemType.Buff) {
                        var buffExists = buffDict.TryGetValue(trigger, out var buff);

                        if (part.Bar && !barAssigned && buffExists) {
                            barAssigned = true;
                            SetGaugeValue(buff.Duration / part.Duration, (int)Math.Round(buff.Duration));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(buffExists ? buff.StackCount : 0, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (buffExists) break;
                    }
                    else {
                        var recastActive = UIHelper.GetRecastActive(trigger.Id, out var timeElapsed);

                        if (part.Bar && !barAssigned && recastActive) {
                            barAssigned = true;
                            var currentTime = timeElapsed % part.CD;
                            var timeLeft = part.CD - currentTime;
                            SetGaugeValue(currentTime / part.CD, (int)Math.Round(timeLeft));
                        }
                        if (part.Diamond) {
                            SetDiamondValue(recastActive ? (int)Math.Floor(timeElapsed / part.CD) : part.MaxCharges, diamondIdx, part.MaxCharges);
                            diamondIdx += part.MaxCharges;
                        }
                        if (recastActive) break;
                    }
                }
            }
            if (!barAssigned) {
                SetGaugeValue(0, 0);
                if (!GaugeFull && !Props.NoSoundOnFull) UIHelper.PlaySeComplete(); // play the sound effect when full charges
            }
            GaugeFull = !barAssigned;
        }

        public override void ProcessAction(Item action) { }

        private void SetDiamondValue(int value, int start, int max) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetDiamondValue(value, start, max);
            }
            else if (UI is UIDiamond diamond) {
                diamond.SetValue(value, start, max);
            }
        }

        private void SetGaugeValue(float value, int textValue) {
            if (UI is UIGaugeDiamondCombo combo) {
                combo.SetText(textValue.ToString());
                combo.SetPercent(value);
            }
            else if (UI is UIGauge gauge) {
                gauge.SetText(textValue.ToString());
                gauge.SetPercent(value);
            }
        }

        protected override int GetHeight() => UI == null ? 0 : UI.GetHeight(0);

        protected override int GetWidth() => UI == null ? 0 : UI.GetWidth(0);

        public override GaugeVisualType GetVisualType() => Props.Type;

        protected override void DrawGauge(string _ID, JobIds job) {
            if (DrawColorOptions(_ID, Props.BarColor, out string newColorString, out var newColor)) {
                Props.BarColor = newColor;
                if (job == GaugeManager.Manager.CurrentJob) {
                    Props.BarColor = newColor;
                    Config.Color = newColorString;
                    Configuration.Config.Save();
                    UI?.SetColor(Props.BarColor);
                    RefreshSameColor();
                    SetupDiamondColors();
                }
            }

            if (DrawTypeOptions(_ID, ValidGaugeVisualType, Props.Type, out var newType)) {
                Config.Type = Props.Type = newType;
                Configuration.Config.Save();
                GaugeManager.Manager.ResetJob(job);
            }

            if (ImGui.Checkbox($"Don't Play Sound When Full {_ID}", ref Props.NoSoundOnFull)) {
                Config.Invert = Props.NoSoundOnFull;
                Configuration.Config.Save();
            }
        }
    }
}
