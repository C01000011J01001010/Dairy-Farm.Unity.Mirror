using CoreEngine;
using System;
using System.Collections;

namespace Temp
{
    public abstract class BaseSelection : BaseUi//, IInitialize
    {
        public override void OnExit()
        {
            base.OnExit();
            ClearButtonCallback();
        }

        protected override IEnumerator OnInitialize()
        {
            yield return base.OnInitialize();
            SetButtonCallback();
            yield return null;
        }

        protected abstract void SetButtonCallback();
        protected abstract void ClearButtonCallback();
    }
}

