using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoadPlacer.Actions
{

    public class MainToolstripNavigator
    {
        private static MainToolstripNavigator instance;
        public static MainToolstripNavigator Instance
        {
            get {
                if (instance == null)
                {
                    instance = new MainToolstripNavigator();
                }
                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        private readonly ToolbarNode mainToolbarRoot;

        private MainToolstripNavigator()
        {
            var mainToolStrip = Singleton<GameMainToolbar>.instance.component as UITabstrip;
            Dictionary<string, ToolbarNode> firstOptions = new Dictionary<string, ToolbarNode>();
            mainToolbarRoot = new ToolbarNode(mainToolStrip.name, mainToolStrip, firstOptions);
            foreach (var tab in mainToolStrip.tabs)
            {
                if (tab.name.Contains("Separator")
                    || tab.name.Equals("Money")
                    || tab.name.Equals("Policies"))
                {
                    continue;
                }
                Dictionary<string, ToolbarNode> secondOptions = new Dictionary<string, ToolbarNode>();
                ToolbarNode oneDeep = new ToolbarNode(tab.name, tab, secondOptions);
                firstOptions[tab.name] = oneDeep;

                var subPanelType = Type.GetType(tab.name + "GroupPanel, Assembly-CSharp");
                var subPanel = mainToolStrip.GetComponentInContainer(tab, subPanelType) as GeneratedGroupPanel;
                var subTabStrip = subPanelType
                    .GetField("m_Strip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                    .GetValue(subPanel) as UITabstrip;
                foreach (var subTab in subTabStrip.components)
                {
                    Dictionary<string, ToolbarNode> thirdOptions = new Dictionary<string, ToolbarNode>();
                    ToolbarNode twoDeep = new ToolbarNode(tab.name, tab, thirdOptions);
                    secondOptions[subTab.name] = twoDeep;

                    var subsubType = Type.GetType(tab.name + "Panel, Assembly-CSharp");
                    var subsubPanel = subTabStrip.GetComponentInContainer(subTab, subsubType) as GeneratedScrollPanel;
                    foreach (var component in subsubPanel.childComponents)
                    {
                        ToolbarNode threeDeep = new ToolbarNode(component.name, component, null);
                        thirdOptions[component.name] = threeDeep;
                    }
                }
            }
        }

        internal struct ToolbarNode
        {
            internal readonly string name;
            internal readonly UIComponent component;
            internal readonly Dictionary<string, ToolbarNode> options;

            public ToolbarNode(string name, UIComponent component, Dictionary<string, ToolbarNode> options)
            {
                this.name = name;
                this.component = component;
                this.options = options;
            }
        }

        public List<UIComponent> GetComponentChain(List<string> names)
        {
            return GetComponentChainRecursive(mainToolbarRoot, names);
        }

        private List<UIComponent> GetComponentChainRecursive(ToolbarNode node, List<string> names)
        {
            if (names.Count == 0)
            {
                return new List<UIComponent>();
            }
            string currentName = names[0];
            if (node.options.ContainsKey(currentName))
            {
                var nextTreeNode = node.options[currentName];
                var chain = GetComponentChainRecursive(nextTreeNode, names.GetRange(1, names.Count - 1));
                chain.Insert(0, nextTreeNode.component);
                return chain;
            }
            else
            {
                throw new Exception(currentName + " does not exist in " + node.name);
            }

        }
    }
}
