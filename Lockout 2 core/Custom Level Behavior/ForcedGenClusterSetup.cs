using GTFO.API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LevelGeneration;
using AIGraph;
using GameData;

namespace Lockout_2_core
{
    public class ForcedGenClusterSetup : MonoBehaviour
    {
        public void Awake()
        {
            Patch_LG_LateGeomorphScanJob.OnLateGeomorphScanDone += SetupGenCluster;
        }
        public void SetupGenCluster()
        {
            if (m_IsSetup) return;
            m_IsSetup = true;

            m_GenCluster = gameObject.GetComponent<LG_PowerGeneratorCluster>();
            m_Area = m_GenCluster.GetComponentInParent<LG_Area>();
            m_Node = m_Area.m_courseNode;
            m_Layer = m_Node.LayerType;

            m_Objective = WardenObjectiveManager.Current.m_activeWardenObjectives[LG_LayerType.MainLayer];
            m_ObjectiveTypeMemory = m_Objective.Type;

            L.Debug($"Layer: {m_Layer}\nObjecive Type: {m_Objective.Type}");

            m_Objective.Type = eWardenObjectiveType.CentralGeneratorCluster;

            L.Debug($"Is gen cluster null? {m_GenCluster == null}");

            var termItem = m_GenCluster.gameObject.AddComponent<LG_GenericTerminalItem>();
            termItem.m_spawnNode = m_Node;
            m_GenCluster.m_terminalItemComp = termItem;
            m_GenCluster.SpawnNode = m_Node;

            m_GenCluster.Setup(0);
            m_Objective.Type = m_ObjectiveTypeMemory;
        }

        public LG_PowerGeneratorCluster m_GenCluster;
        public bool m_IsSetup = false;

        public LG_Area m_Area;
        public AIG_CourseNode m_Node;
        public LG_LayerType m_Layer;
        public WardenObjectiveDataBlock m_Objective;
        public eWardenObjectiveType m_ObjectiveTypeMemory;
    }
}
