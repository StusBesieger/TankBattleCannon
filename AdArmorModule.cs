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
        //���b�X�N���v�g�\��t���Ȃ��u���b�N�Q
        public Dictionary<int, Type> BlockDict = new Dictionary<int, Type>
        {
            //�R�A�u���b�N
            {0, typeof(NoArmorScript) },
            //�{��
            {23, typeof(NoArmorScript) },
            //�y�􎮃��P�b�g
            {59, typeof(NoArmorScript) },
            //�t�@�C���[�{�[��
            {31, typeof(NoArmorScript) },
            //�ې�
            {36, typeof(NoArmorScript) },
            //�M�C��
            {74, typeof(NoArmorScript) },
            //�J�����u���b�N
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
            //�u���b�N�ݒu���ɐ�����C�x���g�֐��쐬
            Events.OnBlockInit += new Action<Block>(AdArmorScript);
        }
        public void AdArmorScript(Block block)
        {
            BlockBehaviour internalObject = block.BuildingBlock.InternalObject;

            //�X�N���v�g�̓\��t��
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
            
            //�d�ʁA�ڑ����x�������邽�߂̏���
            rigidbody = GetComponent<Rigidbody>();
            jointchange = GetComponent<ConfigurableJoint>();
            //�\��t����ꂽ�u���b�N�����ꂼ�ꎝ���Ă��邩�����Ă��Ȃ����𔻒�
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
            //���b���̃X���C�_�[�ƒl���擾
            ArmorSlider = ((ModBlockBehaviour)this).AddSlider("Armor thickness", "ArmorThickness", armorthickness, 10f, 175f);
            ArmorSlider.DisplayInMapper = true;
        }
        public void Awake()
        {
            Debug.Log("Awake");
        }
        //public void BlockStateChanger(float armorvalue)
        //{
        //    //���{�ڑ��̋��x�̓r���h���ƁA�V�~���J�n���1�񂸂�������Ă��܂����߃V�~���J�n��x��ď������s��
        //    changevalue = (float)(Math.Log(armorvalue, 25f));
        //    if (StatMaster.startingMachines)
        //    {
        //        StartCoroutine(StateChange(armorvalue));
        //    }
        //    //���ڑ��̓V�~���J�n��͂ł��Ȃ����ߕʂŃr���h���ɍs��
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
