﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;

namespace osu.Game.Graphics.Containers
{
    public abstract class HoldToConfirmContainer : Container
    {
        public Action Action;

        private const int fadeout_delay = 200;

        private bool fired;
        private bool confirming;

        /// <summary>
        /// Whether the overlay should be allowed to return from a fired state.
        /// </summary>
        protected virtual bool AllowMultipleFires => false;

        public Bindable<double> Progress = new BindableDouble();

        private Bindable<int> holdActivationDelay;

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            holdActivationDelay = config.GetBindable<int>(OsuSetting.UIHoldActivationDelay);
        }

        protected void BeginConfirm()
        {
            if (confirming || (!AllowMultipleFires && fired)) return;

            confirming = true;

            this.TransformBindableTo(Progress, 1, holdActivationDelay.Value * (1 - Progress.Value), Easing.Out).OnComplete(_ => Confirm());
        }

        protected virtual void Confirm()
        {
            Action?.Invoke();
            fired = true;
        }

        protected void AbortConfirm()
        {
            if (!AllowMultipleFires && fired) return;

            confirming = false;

            this.TransformBindableTo(Progress, 0, fadeout_delay, Easing.Out);
        }
    }
}
