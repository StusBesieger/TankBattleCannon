using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Modding;
using Modding.Serialization;
using Modding.Modules;
using Modding.Blocks;
using Modding.Common;
using skpCustomModule;
using Vector3 = UnityEngine.Vector3;
using USlider = UnityEngine.UI.Slider;

namespace TBCStusSpace
{
    public class AddMachineStatusUI : SingleInstance<AddMachineStatusUI>
    {
        private int repeatspan = 1000;
        private int timeElapsed = 0;
        private int PlayerNumber = 0;
        private int playercount = 0;
        private BlockBehaviour PlayerNameObject;
        private List<Player> players;
        private int BlockPlayerID;
        private string nametext;
        private string[] playername;
        private Machine machine;
        private GameObject ChildObject;
        private ArmorScript[] armorScript;
        private float totalArmor = 0f;
        private float totaldispersalArmor = 0f;
        private float[] armorAverage ;
        private float[] armorDispersal;
        private bool isOwnerSame = false;
        private Rect windowRect = new Rect(0, 800, 1000, 100);
        private int windowId;
        private Transform BuildingMachineObject; 

        public override string Name
        {
            get
            {
                return "TBCGUI";
            }
        }
        private void FixedUpdate()
        {
            timeElapsed += 1;
                if (timeElapsed >= repeatspan)
                {
                    StateUpdate();
                    timeElapsed = 0;
                }
        }
        public void Awake()
        {
            windowId = ModUtility.GetWindowId();
        }
        public void Update()
        {
        }
        public void StateUpdate()
        {
            PlayerNumber = 0;
            players = Player.GetAllPlayers();
            Machine[] PMachines = FindObjectsOfType<Machine>();
            foreach (Machine PMachine in PMachines)
            {
                machine = PMachine.GetComponent<Machine>();
                Debug.Log(machine);
                BuildingMachineObject = PMachine.transform.Find("Building Machine");
                Debug.Log(BuildingMachineObject);
                //各ブロックのコンポーネントを取得と装甲平均値計算
                for (int i = 0; i < machine.BlockCount; i++)
                {
                    ChildObject = BuildingMachineObject.transform.GetChild(i).gameObject;
                    armorScript[i] = ChildObject.GetComponent<ArmorScript>();
                    totalArmor += armorScript[i].armorthickness;
                }
                armorAverage[PlayerNumber] = (float)Math.Round(totalArmor / machine.BlockCount*10)/10;
                totalArmor = 0;
                //装甲値の分散計算
                for (int i = 0; i < machine.BlockCount; i++)
                {
                    totaldispersalArmor += (float)Math.Pow(armorScript[i].armorthickness - armorAverage[PlayerNumber], 2);
                }
                armorDispersal[PlayerNumber] = (float)Math.Round(totaldispersalArmor / machine.BlockCount*10)/10;
                totaldispersalArmor = 0f;
                //プレイヤー名を取得
                PlayerNameObject = BuildingMachineObject.transform.GetChild(0).gameObject.GetComponent<BlockBehaviour>();
                if (StatMaster.isMP)
                {
                    BlockPlayerID = PlayerNameObject.ParentMachine.PlayerID;
                }
                else
                {
                    BlockPlayerID = 0;
                }
                players = Player.GetAllPlayers();

                foreach (Player child in players)
                {
                    if (child.InternalObject.networkId == (ushort)BlockPlayerID)
                    {
                        nametext = child.InternalObject.name;
                        playername[PlayerNumber] = nametext;
                    }
                }

                PlayerNumber++;
            }
            playercount = PlayerNumber;
        }
        public void OnGUI()
        {
            if(!StatMaster.isMainMenu )
            {
                windowRect = GUILayout.Window(windowId, windowRect, delegate (int windowId)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal("box");

                    GUILayout.Label("Player Name");
                    GUILayout.Label("Armor Average");
                    GUILayout.Label("Armor Dispersal");

                    GUILayout.EndHorizontal();

                    for (int i = 0; i < playercount; i++)
                    {
                        GUILayout.BeginHorizontal("box");

                        GUILayout.Label(playername[i]);
                        GUILayout.Label(armorAverage[i].ToString());
                        GUILayout.Label(armorDispersal[i].ToString());

                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Label("TBCTest");
                    GUILayout.EndVertical();
                    GUI.DragWindow();
                }
                , "Machine Status");
            }
        }


    }
}
