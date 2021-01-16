using UnityEngine;

namespace RoadPlacer.Actions
{
    abstract class AutoAction
    {
        public readonly string name;
        public readonly bool invokeable;
        public AutoAction parent;
        public AutoAction next;

        public AutoAction(string name, bool invokeable)
        {
            this.name = name;
            this.invokeable = invokeable;
        }

        public AutoAction(string name) : this(name, true) { }

        virtual public void Invoke()
        {
            // Do nothing. Command is delegated to next actions
        }
    }

    abstract class AutoActionGroup : AutoAction
    {
        public AutoActionGroup(string name) : base(name, false) { }
    }

    class SelfRepeatingAction : AutoAction
    {
        private AutoAction exitAction;
        private readonly ISelfRepeating strategy;

        public SelfRepeatingAction(ISelfRepeating strategy) : base("Repeating" + strategy.Name())
        {
            this.strategy = strategy;
        }

        private void SetupExitAction()
        {
            if (exitAction == null)
            {
                exitAction = strategy.GetExitAction();
                if (exitAction != null)
                {
                    exitAction.parent = this;
                    exitAction.next = next;
                }
                else
                {
                    exitAction = next;
                }
                next = this;
            }
        }

        public sealed override void Invoke()
        {
            SetupExitAction();
            if (strategy.ExitCriteriaIsMet())
            {
                strategy.CleanUp();
                next = exitAction;
            }
            else
            {
                strategy.Invoke();
            }
        }
    }

    internal interface ISelfRepeating
    {
        void CleanUp();
        bool ExitCriteriaIsMet();
        AutoAction GetExitAction();
        void Invoke();
        string Name();
    }
}
