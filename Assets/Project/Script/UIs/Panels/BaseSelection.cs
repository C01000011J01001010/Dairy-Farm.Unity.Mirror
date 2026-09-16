using CoreEngine;
using System;
using System.Collections;

namespace Temp
{
    public abstract class BaseSelection : BaseUi//, IInitialize
    {
        public override void Exit()
        {
            ClearButtonCallback();
        }

        public override IEnumerator Initialize()
        {
            SetButtonCallback();
            yield return null;
        }

        protected abstract void SetButtonCallback();
        protected abstract void ClearButtonCallback();
    }
}

