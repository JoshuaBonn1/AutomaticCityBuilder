using RoadPlacer.Tools;
using System;
using UnityEngine;

namespace RoadPlacer.Actions
{
    public delegate Vector2 DelayedGetPosition();

    class TimeWait : AutoAction {
        public TimeWait() : base("TimeWait") { }
    }

    class MouseEvent : AutoAction
    {
        private readonly MouseOperations.MouseEventFlags mouseEvent;

        public MouseEvent(MouseOperations.MouseEventFlags mouseEvent) : base("MouseEvent")
        {
            this.mouseEvent = mouseEvent;
        }

        public override void Invoke()
        {
            MouseOperations.MouseEvent(mouseEvent);
        }
    }

    internal class AdjustCamera : ISelfRepeating
    {
        private readonly DelayedGetPosition getPosition;

        public AdjustCamera(DelayedGetPosition getPosition)
        {
            this.getPosition = getPosition;
        }

        private float GetMaxDistanceFromCenter()
        {
            return (Math.Min(Screen.width, Screen.height) / 2f) * 0.8f;
        }
        
        private Vector2 GetCenterScreen()
        {
            return DisplayPointTranslator.GetDisplayScreenResolution() / 2f;
        }

        public void Invoke()
        {
            Vector2 targetPosition = getPosition();
            Vector2 centerScreen = GetCenterScreen();
            Vector2 offsetVector = targetPosition - centerScreen;
            float xScale = Math.Abs(centerScreen.x / offsetVector.x);
            float yScale = Math.Abs(centerScreen.y / offsetVector.y);
            float finalScale = offsetVector.x < offsetVector.y ? yScale : xScale;
            Vector2 offsetFromCenterToEdge = offsetVector * finalScale;
            MouseOperations.SetCursorPosition(centerScreen + offsetFromCenterToEdge);
        }

        public void CleanUp()
        {
            MouseOperations.SetCursorPosition(GetCenterScreen());
        }

        public bool ExitCriteriaIsMet()
        {
            Vector2 targetPosition = getPosition();
            float distanceFromCenter = Vector2.Distance(targetPosition, GetCenterScreen());
            return distanceFromCenter <= GetMaxDistanceFromCenter();
        }

        public AutoAction GetExitAction()
        {
            return new TimeWait();
        }

        public string Name()
        {
            return "AdjustCamera";
        }
    }

    class MoveMouse : AutoAction
    {
        private readonly DelayedGetPosition getPosition;
        private readonly bool isUi;

        public MoveMouse(DelayedGetPosition getPosition, bool isUi = false) : base("MoveMouse")
        {
            this.getPosition = getPosition;
            this.isUi = isUi;
        }

        public override void Invoke()
        {
            Vector2 originalPosition = getPosition();
            if (isUi || DisplayPointTranslator.IsValidPoint(originalPosition))
            {
                MouseOperations.SetCursorPosition(originalPosition);
            }
            else
            {
                var oldNext = next;
                next = new AutoActionBuilder()
                    .Add(new SelfRepeatingAction(new AdjustCamera(getPosition)))
                    .Add(new MoveMouse(getPosition))
                    .Add(oldNext)
                    .Build(this);
            }
        }
    }

    class MouseClick : AutoActionGroup
    {
        public MouseClick(bool rightClick = false) : base("MouseClick")
        {
            MouseOperations.MouseEventFlags down = rightClick ? MouseOperations.MouseEventFlags.RightDown : MouseOperations.MouseEventFlags.LeftDown;
            MouseOperations.MouseEventFlags up = rightClick ? MouseOperations.MouseEventFlags.RightUp : MouseOperations.MouseEventFlags.LeftUp;
            next = new AutoActionBuilder()
                .Add(new MouseEvent(down))
                .Add(new MouseEvent(up))
                .Build(this);
        }
    }
}
