using System;
using System.Collections;
using System.Collections.Generic; 
using Modding;
using Modding.Blocks; 
using UnityEngine;

namespace TBCStusSpace
{
	public class AdArmorModule : SingleInstance<AdArmorModule>
	{
        //装甲スクリプト貼り付けないブロック群
        public Dictionary<int, Type> BlockDict = new Dictionary<int, Type>
        {
            //コアブロック
            {0, typeof(NoArmorScript) },
            //ボム
            {23, typeof(NoArmorScript) },
            //炸裂式ロケット
            {59, typeof(NoArmorScript) },
            //ファイヤーボール
            {31, typeof(NoArmorScript) },
            //丸石
            {36, typeof(NoArmorScript) },
            //熱気球
            {74, typeof(NoArmorScript) },
            //カメラブロック
            {58, typeof(NoArmorScript) },


        };
        public override string Name
        {
            get
            {
                return "AdArmor";
            }
        }
        public void Awake()
        {
            //ブロック設置時に生じるイベント関数作成
            Events.OnBlockInit += new Action<Block>(AdArmorScript);
        }
        public void AdArmorScript(Block block)
        {
            BlockBehaviour internalObject = block.BuildingBlock.InternalObject;

            //スクリプトの貼り付け
            if(BlockDict.ContainsKey(internalObject.BlockID))
            {
                Type type = BlockDict[internalObject.BlockID];
                try
                {
                    
                    if (internalObject.GetComponent(type) == null)
                    {
                        internalObject.gameObject.AddComponent(type);
                    }
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    if(internalObject.GetComponent<ArmorScript>() == null)
                    {
                        internalObject.gameObject.AddComponent<ArmorScript>();
                    }
                }
                catch
                {

                }
            }
        }
    }
    public class NoArmorScript : MonoBehaviour
    {

    }
    public class ArmorScript : ModBlockBehaviour
    {
        private Rigidbody rigidbody;
        private ConfigurableJoint jointchange;
        private HingeJoint hingejointchange;

        private MSlider ArmorSlider;

        private ConfigurableJoint jointobject;

        public float armorthickness = 25f;
        public float armorvalue = 25f;
        private float changevalue = 0f;
        private float jointvalue = 0f;
        private float jointvalue2 = 0f;
        private float hingejointvalue = 0.0f;
        private float Dmass = 1.0f;
        private int i = 1;
        public void FixedUpdate()
        {
        }
        public void Update()
        {
        }
        public override void OnBlockPlaced()
        {
            
            Debug.Log("placed");
            
            //重量、接続強度をいじるための準備
            rigidbody = GetComponent<Rigidbody>();
            jointchange = GetComponent<ConfigurableJoint>();
            //貼り付けられたブロックがそれぞれ持っているか持っていないかを判定
            if (this.transform.Find("TriggerForJoint2"))
            {
                jointobject = this.transform.Find("TriggerForJoint2").transform.Find("Joint").GetComponent<ConfigurableJoint>();
                jointvalue2 = jointobject.breakForce;
            }
            if (jointchange == null)
            {
                hingejointchange = GetComponent<HingeJoint>();
            }
            if (jointchange)
            {
                jointvalue = jointchange.breakForce;
            }
            if (hingejointchange)
            {
                hingejointvalue = hingejointchange.breakForce;
            }
            Dmass = rigidbody.mass;
        }
        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
            Debug.Log("Start");
            StartCoroutine(StateChange(armorvalue));
        }
        public override void BuildingFixedUpdate()
        {
            base.BuildingFixedUpdate();
            if(armorvalue != ArmorSlider.Value)
            {
                armorvalue = ArmorSlider.Value;
            }
           
            if(changevalue != (float)(Math.Log(armorvalue, 25f)))
            {
                changevalue = (float)(Math.Log(armorvalue, 25f));
                if (this.transform.Find("TriggerForJoint2"))
                {
                    jointobject.breakForce = jointvalue2 / changevalue;
                    jointobject.breakTorque = jointvalue2 / changevalue;
                }

            }
        }
        public override void SafeAwake()
        {
            //装甲厚のスライダーと値を取得
            ArmorSlider = ((ModBlockBehaviour)this).AddSlider("Armor thickness", "ArmorThickness", armorthickness, 10f, 175f);
            ArmorSlider.DisplayInMapper = true;
        }
        public void Awake()
        {
            Debug.Log("Awake");
        }
        //public void BlockStateChanger(float armorvalue)
        //{
        //    //根本接続の強度はビルド中と、シミュ開始後に1回ずつ処理されてしまうためシミュ開始後遅れて処理を行う
        //    changevalue = (float)(Math.Log(armorvalue, 25f));
        //    if (StatMaster.startingMachines)
        //    {
        //        StartCoroutine(StateChange(armorvalue));
        //    }
        //    //頭接続はシミュ開始後はできないため別でビルド中に行う
        //    if (!StatMaster.startingMachines)
        //    {
        //    }

        //}
        public IEnumerator StateChange(float armorvalue)
        {
            yield return new WaitForFixedUpdate();
            if ((float)(Math.Log(armorvalue, 25f)) < 0.5f)
            {
                changevalue = 0.5f;
            }
            if(rigidbody)
            {
                rigidbody.mass = Dmass * armorvalue / 25f;
            }
            if (jointchange)
            {
                if (float.IsInfinity(jointchange.breakForce))
                {
                    jointchange.breakForce = 60000f / changevalue;
                    jointchange.breakTorque = 60000f / changevalue;
                }
                else
                {
                    jointchange.breakForce = jointvalue / changevalue;
                    jointchange.breakTorque = jointvalue / changevalue;
                }
            }
            if (hingejointchange)
            {
                hingejointchange.breakForce = hingejointvalue / changevalue;
                hingejointchange.breakTorque = hingejointvalue / changevalue;
            }
        }

    }

}
