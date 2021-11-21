﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Custom;
using JobBars.Gauges.GCD;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.UI;
using System;

namespace JobBars.Jobs {
    public static class DRK {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeDrkMPConfig($"MP ({UIHelper.Localize(JobIds.DRK)})", GaugeVisualType.BarDiamondCombo, new GaugeDrkMpProps {
                Color = UIColor.Purple,
                DarkArtsColor = UIColor.LightBlue,
                Segments = new[] { 0.3f, 0.6f, 0.9f }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.Delirium), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 5,
                MaxDuration = 10,
                Color = UIColor.Red,
                Increment = new []{
                    new Item(ActionIds.BloodSpiller),
                    new Item(ActionIds.Quietus)
                },
                Triggers = new []{
                    new Item(BuffIds.Delirium)
                }
            }),
            new GaugeGCDConfig(UIHelper.Localize(BuffIds.BloodWeapon), GaugeVisualType.Arrow, new GaugeSubGCDProps {
                MaxCounter = 5,
                MaxDuration = 10,
                Color = UIColor.DarkBlue,
                Triggers = new []{
                    new Item(BuffIds.BloodWeapon)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(UIHelper.Localize(BuffIds.Delirium), new BuffProps {
                CD = 90,
                Duration = 10,
                Icon = ActionIds.Delirium,
                Color = UIColor.Red,
                Triggers = new []{ new Item(BuffIds.Delirium) }
            }),
            new BuffConfig(UIHelper.Localize(ActionIds.LivingShadow), new BuffProps {
                CD = 120,
                Duration = 24,
                Icon = ActionIds.LivingShadow,
                Color = UIColor.Purple,
                Triggers = new []{ new Item(ActionIds.LivingShadow) }
            })
        };

        public static Cursor Cursors => new(JobIds.DRK, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[]{
            new CooldownConfig(UIHelper.Localize(ActionIds.LivingDead), new CooldownProps {
                Icon = ActionIds.LivingDead,
                Duration = 10,
                CD = 300,
                Triggers = new []{ new Item(BuffIds.LivingDead) }
            }),
            new CooldownConfig($"{UIHelper.Localize(ActionIds.Reprisal)} ({UIHelper.Localize(JobIds.DRK)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.DarkMissionary), new CooldownProps {
                Icon = ActionIds.DarkMissionary,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.DarkMissionary) }
            }),
            new CooldownConfig(UIHelper.Localize(ActionIds.TheBlackestNight), new CooldownProps {
                Icon = ActionIds.TheBlackestNight,
                Duration = 7,
                CD = 15,
                Triggers = new []{ new Item(ActionIds.TheBlackestNight) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconReplacer(UIHelper.Localize(BuffIds.Delirium), new IconProps {
                Icons = new [] { ActionIds.Delirium },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.Delirium), Duration = 10 }
                }
            }),
            new IconReplacer(UIHelper.Localize(BuffIds.BloodWeapon), new IconProps {
                Icons = new [] { ActionIds.BloodWeapon },
                Triggers = new[] {
                    new IconTriggerStruct { Trigger = new Item(BuffIds.BloodWeapon), Duration = 10 }
                }
            })
        };

        // DRK HAS A CUSTOM MP BAR, SO DON'T WORRY ABOUT THIS
        public static bool MP => false;
        public static float[] MP_SEGMENTS => null;

        public static bool GCD_ROLL => true;
    }
}