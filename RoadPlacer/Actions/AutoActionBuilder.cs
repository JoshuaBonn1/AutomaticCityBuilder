using System.Collections.Generic;

namespace RoadPlacer.Actions
{
    class AutoActionBuilder
    {
        private readonly List<AutoAction> autoActions = new List<AutoAction>();

        public AutoActionBuilder Add(AutoAction action)
        {
            autoActions.Add(action);
            return this;
        }

        public AutoAction Build(AutoAction parent)
        {
            autoActions[0].parent = parent;
            for (int i = 0; i < autoActions.Count - 1; i++)
            {
                AutoAction currentAction = autoActions[i];
                while (currentAction.next != null)
                {
                    currentAction = currentAction.next;
                }
                AutoAction nextAction = autoActions[i + 1];
                nextAction.parent = parent;
                currentAction.next = nextAction;
            }
            return autoActions[0];
        }
    }
}
