using ColossalFramework;
using ColossalFramework.Math;
using ICities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RoadPlacer
{
    public class RoadPlacerMod : IUserMod
    {
        public string Name => "Road Placer Mod";

        public string Description => "This mod places some roads around";
    }

    public class RoadFinisher : ThreadingExtensionBase
    {
        public bool go = false;
        private Actions.ActionProvider actionProvider;

        public override void OnCreated(IThreading threading)
        {
            base.OnCreated(threading);
            actionProvider = new Actions.ActionProvider();
            Actions.MainToolstripNavigator.Reset();
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);
            if (Input.GetKeyDown(KeyCode.J))
            {
                go = true;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                go = false;
                actionProvider.AddAction(RoadConnector.PopulateRoadConnection());
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                go = false;
                actionProvider.AddAction(new Actions.EncapsulatingRoadSelector(new Actions.TimeWait()));
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                go = false;
                actionProvider.AddAction(new Actions.EncapsulatingRoadSelector(RoadConnector.PopulateRoadConnection()));
                Debug.Log("Ready to connect");
            }
            if (go)
            {
                actionProvider.GetNextAction(realTimeDelta)();
            }
        }
    }

    internal class RoadConnector
    {
        public static Actions.AutoAction PopulateRoadConnection()
        {
            List<NetNode> openNodes = new List<NetNode>();
            for (ushort i = 1; i < NetManager.instance.m_nodes.m_size; i++)
            {
                NetNode val = NetManager.instance.m_nodes.m_buffer[i];
                int areaIndex = GameAreaManager.instance.GetAreaIndex(val.m_position);
                GameAreaManager.instance.GetTileXZ(areaIndex, out int areaX, out int areaZ);
                if (GameAreaManager.instance.IsUnlocked(areaX, areaZ))
                {
                    if (val.CountSegments() == 1)
                    {
                        openNodes.Add(val);
                        if (openNodes.Count == 2) break;
                    }
                }
            }
            var action = new Actions.ConnectTwoNodes(openNodes[0], openNodes[1]);
            Debug.Log(openNodes[0]);
            Debug.Log(openNodes[1]);
            return action;
        }
    }
}
