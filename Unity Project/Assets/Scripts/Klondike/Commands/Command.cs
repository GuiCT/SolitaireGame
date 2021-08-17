using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klondike.Commands
{
    public abstract class Command
    {
        protected static DataManager dataManager;

        protected Command()
        {
            if (dataManager == null)
            {
                dataManager = MonoBehaviour.FindObjectOfType<DataManager>();
            }
        }
        public abstract void Execute();

        public abstract void Undo();
    }
}
