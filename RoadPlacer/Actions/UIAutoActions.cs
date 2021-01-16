using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace RoadPlacer.Actions
{
    class ClickUiComponent : AutoActionGroup
    {
        private readonly UIComponent component;

        private Vector2 GetComponentLocationOnDisplay()
        {
            Vector3 centerOfComponent = component.absolutePosition;
            centerOfComponent.x += component.width / 2f;
            centerOfComponent.y += component.height / 2f;
            return Tools.DisplayPointTranslator.ConvertUIPointToMousePoint(centerOfComponent);
        }

        public ClickUiComponent(UIComponent component) : base("ClickUiComponent")
        {
            this.component = component;
            next = new AutoActionBuilder()
                .Add(new MoveMouse(GetComponentLocationOnDisplay, true))
                .Add(new MouseClick())
                .Build(this);
        }
    }

    class EncapsulatingRoadSelector : AutoActionGroup
    {
        public EncapsulatingRoadSelector(AutoAction middle) : base("EncapsulatingRoadSelector")
        {
            var componentChain = MainToolstripNavigator.Instance.GetComponentChain(new List<string>
            {
                "Roads",
                "RoadsSmall",
                "Basic Road"
            });
            var builder = new AutoActionBuilder();
            foreach (var component in componentChain)
            {
                builder.Add(new ClickUiComponent(component));
            }
            next = builder
                .Add(middle)
                .Add(new ClickUiComponent(componentChain[0]))
                .Build(this);
        }
    }
}
