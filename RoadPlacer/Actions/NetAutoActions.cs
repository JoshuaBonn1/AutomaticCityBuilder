using UnityEngine;

namespace RoadPlacer.Actions
{
    class ClickNode : AutoActionGroup
    {
        private NetNode node;

        private Vector2 GetNodePosition()
        {
            Debug.Log(node.m_infoIndex);
            Debug.Log(node.m_position);
            return Tools.DisplayPointTranslator.ConvertWorldPointToMousePoint(node.m_position);
        }

        public ClickNode(NetNode node) : base("ClickNode")
        {
            this.node = node;
            next = new AutoActionBuilder()
                .Add(new MoveMouse(GetNodePosition))
                .Add(new MouseClick())
                .Build(this);
        }
    }

    class ConnectTwoNodes : AutoActionGroup
    {
        public ConnectTwoNodes(NetNode startNode, NetNode endNode) : base("ConnectTwoNodes")
        {
            next = new AutoActionBuilder()
                .Add(new ClickNode(startNode))
                .Add(new ClickNode(endNode))
                .Add(new MouseClick(true))
                .Build(this);
        }
    }
}
