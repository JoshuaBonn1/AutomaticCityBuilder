using UnityEngine;

namespace RoadPlacer.Actions
{
    class ActionProvider
    {
        internal delegate void Action();
        private AutoAction currentAction;
        private readonly float minimumTimeGap = 0.05f;
        private float timePassed = 0f;

        public void AddAction(AutoAction action)
        {
            if (currentAction == null)
            {
                currentAction = action;
                return;
            }
            AutoAction lastAction = currentAction;
            while (currentAction.next != null)
            {
                lastAction = lastAction.next;
            }
            lastAction.next = action;
        }

        private bool TimeForNextAction(float timeDelta)
        {
            timePassed += timeDelta;
            if (timePassed > minimumTimeGap)
            {
                timePassed = 0f;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Action GetNextAction(float timeDelta)
        {
            if (!TimeForNextAction(timeDelta) || currentAction == null)
            {
                return () => { };
            }
            while (!currentAction.invokeable)
            {
                currentAction = currentAction.next;
            }
            return () =>
            {
                Debug.Log(GetActionPath(currentAction));
                currentAction.Invoke();
                currentAction = currentAction.next;
            };
        }

        private string GetActionPath(AutoAction lowestAction)
        {
            string actionPath = "";
            AutoAction currentAction = lowestAction;
            while (currentAction != null)
            {
                actionPath = currentAction.name + ": " + actionPath;
                currentAction = currentAction.parent;
            }
            return actionPath;
        }
    }
}
